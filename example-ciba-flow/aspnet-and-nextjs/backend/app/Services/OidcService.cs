using ExampleAspnet.Models.Oidc;
using ExampleAspnet.Storage;

namespace ExampleAspnet.Services;

public interface IOidcService
{
    public Task<string> AcquireTokenAsync();
}

public class OidcService : IOidcService
{
    private readonly ICache _cache;
    private readonly IApiClient _apiClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OidcService> _logger;

    public OidcService(ICache cache, IApiClient apiClient, IConfiguration configuration, ILogger<OidcService> logger)
    {
        _cache = cache;
        _apiClient = apiClient;
        _configuration = configuration;
        _logger = logger;
    }

    private async Task<OidcConfiguration> GetOidcConfigurationAsync()
    {
        var cachedConfiguration = _cache.GetData<OidcConfiguration>("oidc_configuration");

        if (cachedConfiguration != null)
        {
            return cachedConfiguration;
        }

        _logger.LogInformation("Retrieving configuration from BankID OIDC");

        var configurationUri = _configuration["BANKID_OPENID_CONFIGURATION_URI"] ??
                               throw new InvalidOperationException(
                                   "Missing configuration key BANKID_OPENID_CONFIGURATION_URI");

        var config = await _apiClient.GetJsonAsync<OidcConfiguration>(configurationUri);

        _cache.AddData("oidc_configuration", config, DateTime.UtcNow.AddHours(1));
        return config;
    }

    /// <summary>
    /// Acquires an access token from the BankID OIDC using client credentials flow.
    /// </summary>
    /// <returns></returns>
    public async Task<string> AcquireTokenAsync()
    {
        // TODO: Stale-while-revalidate
        var cachedToken = _cache.GetData<string>("access_token");
        if (cachedToken != null)
        {
            return cachedToken;
        }

        _logger.LogInformation("Acquiring access token from BankID OIDC");

        var configuration = await GetOidcConfigurationAsync();

        var clientId = _configuration["CLIENT_ID"];
        var clientSecret = _configuration["CLIENT_SEC"];
        var scope = _configuration["BANKID_SCOPE"];

        var body = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string?>("grant_type", "client_credentials"),
            new KeyValuePair<string, string?>("scope", scope)
        });

        var res = await _apiClient.PostFormUrlEncodedAsync<OidcTokenResponse>(
            configuration.TokenUri, body, (user: clientId, pass: clientSecret));

        var token = res.AccessToken.Replace("\"", "");

        // This will cache the token for its lifetime, leaving 1 minute as a buffer
        _cache.AddToken("access_token", token);

        return token;
    }
}