using System.Text.Json.Serialization;

namespace ExampleAspnet.Models.Oidc;

/// <summary>
/// The OIDC configuration provided by BankID's OIDC. 
/// </summary>
public class OidcConfiguration
{
    [JsonPropertyName("token_endpoint")]
    public string TokenUri { get; set; }
}

/// <summary>
/// The OIDC configuration provided by BankID with Biometric's OIDC.
/// </summary>
public class BisOidcConfiguration
{
    [JsonPropertyName("issuer")]
    public string Issuer { get; set; }
    
    [JsonPropertyName("permissions_endpoint")]
    public string PermissionsUri { get; set; }

    [JsonPropertyName("backchannel_authentication_endpoint")]
    public string BackchannelAuthenticationUri { get; set; }

    [JsonPropertyName("token_endpoint")]
    public string TokenUri { get; set; }

    [JsonPropertyName("jwks_uri")]
    public string JwksUri { get; set; }

    [JsonPropertyName("jwks_uri_grants")]
    public string JwksUriGrants { get; set; }

    [JsonPropertyName("id_token_signing_alg_values_supported")]
    public IList<string> SupportedIdTokenSigningAlgorithms { get; set; }
}