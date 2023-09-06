using System.Text.Json.Serialization;

namespace ExampleAspnet.Models.Oidc;

/// <summary>
/// The response provided by BankID OIDC's token endpoint.
/// This is used to acquire an access token using the client credentials flow.
/// </summary>
public class OidcTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}