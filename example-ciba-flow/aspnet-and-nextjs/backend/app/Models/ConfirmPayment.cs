using System.Text.Json.Serialization;
using ExampleAspnet.Models.Oidc;

namespace ExampleAspnet.Models;

public record ConfirmPaymentRequest(
    [property: JsonPropertyName("amount")]
    string Amount,
    [property: JsonPropertyName("currency")]
    string Currency,
    [property: JsonPropertyName("loginHint")]
    LoginHint LoginHint,
    [property: JsonPropertyName("signals")]
    ClientSignals Signals
);

public record ConfirmPaymentResponse(
    [property: JsonPropertyName("bindingMessage")]
    string BindingMessage
);