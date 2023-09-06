using System.Text.Json.Serialization;

namespace ExampleAspnet.Models.Oidc;

/// <summary>
/// The response provided by the CIBA token endpoint.
/// </summary>
public record CibaTokenResponse
{
    [JsonPropertyName("id_token")]
    public string IdToken { get; init; }
}