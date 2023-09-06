using System.Text.Json.Serialization;

namespace ExampleAspnet.Models;

public record AuthorizeRequest;

public record AuthorizeResponse(
    [property: JsonPropertyName("success")]
    bool Success
);