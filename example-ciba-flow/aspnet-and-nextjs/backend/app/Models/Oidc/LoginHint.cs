using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ExampleAspnet.Models.Oidc;

/// <summary>
/// Represents the user's login hint.
/// </summary>
/// <param name="Scheme">Only 'nnin' is supported at the moment.</param>
/// <param name="Value">The NNIN (Norwegian National Identification Number) value.</param>
public record LoginHint(
    [RegularExpression(@"^nnin$", ErrorMessage = "Expected scheme value nnin")]
    [property: JsonPropertyName("scheme")]
    string Scheme,
    [RegularExpression(@"^\d{11}$", ErrorMessage = "Invalid NNIN value")]
    [property: JsonPropertyName("value")]
    string Value
);