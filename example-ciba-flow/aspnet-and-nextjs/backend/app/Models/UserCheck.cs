using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ExampleAspnet.Models;

public record UserCheckRequest(
    [RegularExpression(@"^nnin$", ErrorMessage = "Expected scheme value nnin")]
    [property: JsonPropertyName("scheme")]
    [Required]
    string Scheme,
    [RegularExpression(@"^\d{11}$", ErrorMessage = "Invalid NNIN value")]
    [property: JsonPropertyName("value")]
    [Required]
    string Value
);

public record UserCheckResponse(
    [property: JsonPropertyName("exists")]
    bool Exists
);