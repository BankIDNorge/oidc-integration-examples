using System.Text.Json.Serialization;

namespace ExampleAspnet.Models.Oidc;

/// <summary>
/// The response provided by the backchannel authorize endpoint (bc-authorize).
/// </summary>
/// <see href="https://developer.bankid.no/bankid-with-biometrics/apis/oidc-api/"/>
public class BcAuthorizeToken
{
    [JsonPropertyName("auth_req_id")]
    public string AuthReqId { get; set; }
}