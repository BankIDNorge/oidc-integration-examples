using System.Text.Json.Serialization;

namespace ExampleAspnet.Models;

public record ErrorResponse(
    [property: JsonPropertyName("error")]
    string Error,
    [property: JsonPropertyName("errorDescription")]
    string ErrorDescription
)
{
    public static readonly ErrorResponse InternalServerError =
        new("internal_server_error", "An internal server error occurred.");

    public static readonly ErrorResponse GrantTimeout =
        new("grant_timeout", "Grant timeout.");

    public static readonly ErrorResponse InvalidSession =
        new("invalid_session", "The session has expired or is invalid.");

    public static readonly ErrorResponse AuthFlowError =
        new("auth_flow_error", "An error occurred during the authentication flow.");
}