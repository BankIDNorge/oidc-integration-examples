using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ExampleAspnet.Models.Oidc;

public record PermissionStatement
{
    [JsonPropertyName("nonce")]
    public string Nonce { get; init; }

    [JsonPropertyName("id")]
    public string Id { get; init; }
}

public record PaymentV1 : PermissionStatement
{
    [JsonPropertyName("payments")]
    public IList<PaymentBasePermissionV1> Payments { get; init; }
}

/// <summary>
/// This must satisfy PSD2 RTS Article 4 (Authentication code) and Article 5 (Dynamic Linking).
/// </summary>
/// <see href="https://eur-lex.europa.eu/legal-content/EN/TXT/HTML/?uri=CELEX:32018R0389&from=EN#d1e493-23-1"/>
public record PaymentBasePermissionV1
{
    [JsonPropertyName("paymentId")]
    public string PaymentId { get; init; }

    /// <summary>
    /// The amount given with fractional digits, where fractions must be compliant to the currency definition.
    /// Up to 14 significant figures. Negative amounts are signed by minus. The decimal separator is a dot.
    /// </summary>
    [JsonPropertyName("amount")]
    public string Amount { get; init; }

    /// <summary>
    /// ISO 4217 Alpha 3 currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    /// <summary>
    /// Human readable payee.
    /// </summary>
    [JsonPropertyName("creditorName")]
    public string CreditorName { get; set; }
}

/// <summary>
/// Represents a permission object. This needs to be registered prior to initiating a CIBA flow.
/// </summary>
public record PermissionRequest
{
    [JsonPropertyName("type")]
    [Required]
    public string Type { get; set; } = null!;

    [JsonPropertyName("loa")]
    public string? Loa { get; set; }

    [JsonPropertyName("iat")]
    [Required]
    public long Iat { get; set; }

    [JsonPropertyName("exp")]
    [Required]
    public long Exp { get; set; }

    [JsonPropertyName("permission")]
    [RegularExpression(@"^[a-zA-Z0-9\+/]*={0,2}$")]
    [Required]
    public string Permission { get; set; } = null!;

    [JsonPropertyName("intents")]
    public IList<string>? Intents { get; set; } = new List<string>();

    [JsonPropertyName("loginHint")]
    public IList<LoginHint> LoginHint { get; set; } = new List<LoginHint>();

    [JsonPropertyName("signals")]
    public ClientSignals? Signals { get; set; }
}

/// <summary>
/// The response returned after registering a permission.
/// </summary>
public record PermissionResponse
{
    [JsonPropertyName("id")]
    public string PermissionId { get; init; }

    [JsonPropertyName("permissionToken")]
    public string PermissionToken { get; init; }

    [JsonPropertyName("bindingMessage")]
    public string BindingMessage { get; init; }
}