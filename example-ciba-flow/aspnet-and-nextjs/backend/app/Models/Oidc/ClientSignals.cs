using System.Text.Json.Serialization;

namespace ExampleAspnet.Models.Oidc;

public record ClientSignals
{
    [JsonPropertyName("browserJavaEnabled")]
    public bool JavaEnabled { get; init; }

    [JsonPropertyName("browserJavascriptEnabled")]
    public bool JavascriptEnabled { get; init; }

    [JsonPropertyName("browserLanguage")]
    public string? Language { get; init; }

    [JsonPropertyName("browserColorDepth")]
    public string? ColorDepth { get; init; }

    [JsonPropertyName("browserScreenHeight")]
    public string? ScreenHeight { get; init; }

    [JsonPropertyName("browserScreenWidth")]
    public string? ScreenWidth { get; init; }

    [JsonPropertyName("browserTZ")]
    public string? Timezone { get; init; }

    [JsonPropertyName("browserTzIana")]
    public string? TimezoneIana { get; init; }

    [JsonPropertyName("browserUserAgent")]
    public string? UserAgent { get; init; }

    [JsonPropertyName("browserWebAuthnSupported")]
    public bool WebAuthnSupported { get; init; }

    [JsonPropertyName("browserPlatformAuthenticatorAvailable")]
    public bool PlatformAuthenticatorAvailable { get; init; }

    [JsonPropertyName("browserMaxTouchPoints")]
    public uint? MaxTouchPoints { get; init; }

    [JsonPropertyName("browserWebdriver")]
    public bool Webdriver { get; init; }
}