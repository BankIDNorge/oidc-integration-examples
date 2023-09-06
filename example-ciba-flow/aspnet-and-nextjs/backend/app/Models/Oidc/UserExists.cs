using System.Text.Json.Serialization;

namespace ExampleAspnet.Models.Oidc;

public record UserExistsRequest(
    [property: JsonPropertyName("loginHint")]
    List<LoginHint> LoginHint
);