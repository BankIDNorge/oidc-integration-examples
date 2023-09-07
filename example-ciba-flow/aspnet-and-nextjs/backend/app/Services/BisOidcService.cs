using System.IdentityModel.Tokens.Jwt;
using ExampleAspnet.Exceptions;
using ExampleAspnet.Models;
using ExampleAspnet.Models.Oidc;
using ExampleAspnet.Storage;
using Microsoft.IdentityModel.Tokens;

namespace ExampleAspnet.Services;

public interface IBisOidcService
{
    public Task<PermissionResponse> RegisterPermission(PermissionRequest permission);

    public Task<BcAuthorizeToken> BackchannelAuthorize(string loginHintToken, string message);

    public Task<UserCheckResponse> UserExistsAsync(string value, string scheme = "nnin");

    public Task<CibaTokenResponse> GetToken(string authReqId);

    public Task<TokenValidationResult> ValidateIdTokenAsync(string token);

    public Task<TokenValidationResult> ValidateGrantTokenAsync(string token);

    public Task<string> PollGrant(string id, CancellationToken cancellationToken);
}

public class BisOidcService : IBisOidcService
{
    private readonly ICache _cache;
    private readonly IApiClient _apiClient;
    private readonly IOidcService _oidcService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BisOidcService> _logger;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();

    public BisOidcService(
        ICache cache,
        IApiClient apiClient,
        IOidcService oidcService,
        IConfiguration configuration,
        ILogger<BisOidcService> logger
    )
    {
        _cache = cache;
        _apiClient = apiClient;
        _oidcService = oidcService;
        _configuration = configuration;
        _logger = logger;
    }

    private async Task<BisOidcConfiguration> GetOidcConfigurationAsync()
    {
        var cachedConfiguration = _cache.GetData<BisOidcConfiguration>("bis_oidc_configuration");

        if (cachedConfiguration != null)
        {
            return cachedConfiguration;
        }

        _logger.LogInformation("Retrieving configuration from BankID with Biometrics OIDC");
        var configurationUri = _configuration["BANKID_BIOMETRICS_OPENID_CONFIGURATION_URI"] ??
                               throw new InvalidOperationException(
                                   "Missing configuration key BANKID_BIOMETRICS_OPENID_CONFIGURATION_URI");

        var config = await _apiClient.GetJsonAsync<BisOidcConfiguration>(configurationUri);

        _cache.AddData("bis_oidc_configuration", config, DateTime.UtcNow.AddHours(1));
        return config;
    }

    public async Task<UserCheckResponse> UserExistsAsync(string value, string scheme)
    {
        var configuration = await GetOidcConfigurationAsync();
        var accessToken = await _oidcService.AcquireTokenAsync();

        var request = new UserExistsRequest(new List<LoginHint> { new(scheme, value) });
        return await _apiClient.PostJsonAsync<UserCheckResponse>(
            configuration.PermissionsUri + "user-exists", request, accessToken);
    }

    /// <summary>
    /// Registers a payment with BankID Substantial OIDC.
    /// </summary>
    /// <param name="permission">The permission to register. See <see cref="PermissionRequest"/></param>
    /// <returns></returns>
    public async Task<PermissionResponse> RegisterPermission(PermissionRequest permission)
    {
        var configuration = await GetOidcConfigurationAsync();
        var accessToken = await _oidcService.AcquireTokenAsync();
        return await _apiClient.PostJsonAsync<PermissionResponse>(
            configuration.PermissionsUri, permission, accessToken);
    }

    /// <summary>
    /// Initiates the CIBA flow by calling the bc-authorize endpoint. The permission must be registered prior to calling this method.
    /// </summary>
    /// <param name="loginHintToken">The permission token given by the permission endpoint. See <see cref="RegisterPermission"/></param>
    /// <param name="message">The binding message given by the permission endpoint.</param>
    /// <returns>returns the response body or null if an error occurs</returns>
    public async Task<BcAuthorizeToken> BackchannelAuthorize(string loginHintToken, string message)
    {
        var scopeAuth = _configuration["BANKID_BIOMETRICS_SCOPE"] ?? throw new InvalidOperationException(
            "Missing configuration key BANKID_BIOMETRICS_SCOPE");
        var configuration = await GetOidcConfigurationAsync();
        var accessToken = await _oidcService.AcquireTokenAsync();

        var bodyValues = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string?>("login_hint_token", loginHintToken),
            new KeyValuePair<string, string?>("scope", scopeAuth),
            new KeyValuePair<string, string?>("binding_message", message),
        });

        return await _apiClient.PostFormUrlEncodedAsync<BcAuthorizeToken>(
            configuration.BackchannelAuthenticationUri, bodyValues, accessToken);
    }

    /// <summary>
    /// Fetches the token by calling the token endpoint. The token may not be ready yet, as the user has not accepted the payment.
    /// </summary>
    /// <param name="authReqId">The authorization request id given by bc-authorize endpoint.</param>
    /// <returns><see cref="CibaTokenResponse"/></returns>
    public async Task<CibaTokenResponse> GetToken(string authReqId)
    {
        var grantType = _configuration["BANKID_TOKEN_GRANT_TYPE"]
                        ?? throw new InvalidOperationException("Missing configuration key BANKID_TOKEN_GRANT_TYPE");

        var configuration = await GetOidcConfigurationAsync();
        var accessToken = await _oidcService.AcquireTokenAsync();

        var bodyValues = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", grantType),
            new KeyValuePair<string, string>("auth_req_id", authReqId),
        });

        return await _apiClient.PostFormUrlEncodedAsync<CibaTokenResponse>(
            configuration.TokenUri, bodyValues, accessToken);
    }

    /// <summary>
    /// Fetches the Json Web Key Set (JWKS) for the grant token. This will be used to validate the token.
    /// The JWKS is cached for 30 minutes.
    /// </summary>
    /// <returns><see cref="JsonWebKeySet"/></returns>
    public async Task<JsonWebKeySet> GetGrantsJwksAsync()
    {
        var cachedJwks = _cache.GetData<JsonWebKeySet>("bis_jwks_grants");

        if (cachedJwks != null)
        {
            return cachedJwks;
        }

        var configuration = await GetOidcConfigurationAsync();
        var jwksRaw = await _apiClient.GetStringAsync(configuration.JwksUriGrants, null);

        var jwks = JsonWebKeySet.Create(jwksRaw);
        _cache.AddData("bis_jwks_grants", jwks, DateTime.UtcNow.AddMinutes(30));

        return jwks;
    }

    /// <summary>
    /// Fetches the Json Web Key Set (JWKS) for the ID token. This will be used to validate the token.
    /// The JWKS is cached for 30 minutes.
    /// </summary>
    /// <returns><see cref="JsonWebKeySet"/></returns>
    public async Task<JsonWebKeySet> GetJwksAsync()
    {
        var cachedJwks = _cache.GetData<JsonWebKeySet>("bis_jwks");

        if (cachedJwks != null)
        {
            return cachedJwks;
        }

        var configuration = await GetOidcConfigurationAsync();
        var jwksRaw = await _apiClient.GetStringAsync(configuration.JwksUri, null);

        var jwks = JsonWebKeySet.Create(jwksRaw);
        _cache.AddData("bis_jwks", jwks, DateTime.UtcNow.AddMinutes(30));

        return jwks;
    }

    /// <summary>
    /// Validates the provided <paramref name="token"/> against the Json Web Key Set (JWKS) for the ID token.
    /// </summary>
    /// <param name="token">The ID token in JWT format.</param>
    /// <returns>The validation result.</returns>
    public async Task<TokenValidationResult> ValidateIdTokenAsync(string token)
    {
        // The audience claim in the ID token must match the merchant's client id.
        var clientId = _configuration["BANKID_CLIENT_ID"]
                       ?? throw new InvalidOperationException("Missing configuration key BANKID_CLIENT_ID");

        var configuration = await GetOidcConfigurationAsync();
        var jwks = await GetJwksAsync();
        var tokenValidationResult = await _jwtSecurityTokenHandler.ValidateTokenAsync(token,
            new TokenValidationParameters
            {
                IssuerSigningKeys = jwks.GetSigningKeys(),
                ValidAlgorithms = configuration.SupportedIdTokenSigningAlgorithms,
                ValidIssuer = configuration.Issuer,
                ValidAudience = clientId,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true
            });
        return tokenValidationResult;
    }

    /// <summary>
    /// Validates the provided <paramref name="token"/> against the Json Web Key Set (JWKS) for the grant token.
    /// </summary>
    /// <param name="token">The grant token in JWT format.</param>
    /// <returns>The validation result.</returns>
    public async Task<TokenValidationResult> ValidateGrantTokenAsync(string token)
    {
        var configuration = await GetOidcConfigurationAsync();
        var jwks = await GetGrantsJwksAsync();
        var tokenValidationResult = await _jwtSecurityTokenHandler.ValidateTokenAsync(token,
            new TokenValidationParameters
            {
                IssuerSigningKeys = jwks.GetSigningKeys(),
                ValidAlgorithms = configuration.SupportedIdTokenSigningAlgorithms,
                ValidIssuer = configuration.Issuer,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = false
            });
        return tokenValidationResult;
    }

    /// <summary>
    /// Polls for the grant token until it is received or the time window has passed.
    /// </summary>
    /// <param name="id">The registered permission id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The signed grant token.</returns>
    /// <exception cref="GrantTimeoutException"></exception>
    public async Task<string> PollGrant(string id, CancellationToken cancellationToken)
    {
        var configuration = await GetOidcConfigurationAsync();
        var accessToken = await _oidcService.AcquireTokenAsync();

        var counter = 0;
        while (counter++ < 10 && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                return await _apiClient.GetStringAsync(configuration.PermissionsUri + id + "/grant", accessToken);
            }
            catch (ApiClientException e)
            {
                _logger.LogDebug("Poll grant attempt {} failed: {}", counter, e);
            }

            await Task.Delay(500, cancellationToken);
        }

        throw new GrantTimeoutException();
    }
}