import type { Signals } from "@/lib/types";

const isWebAuthnPlatformAuthenticatorAvailable = async (): Promise<boolean> => {
  if (
    "PublicKeyCredential" in window &&
    "isUserVerifyingPlatformAuthenticatorAvailable" in
      window.PublicKeyCredential
  ) {
    return window.PublicKeyCredential.isUserVerifyingPlatformAuthenticatorAvailable().catch(
      () => false,
    );
  }
  return false;
};

export const getSignals = async (): Promise<Signals> => ({
  browserJavaEnabled: navigator.javaEnabled(),
  browserJavascriptEnabled: true,
  browserLanguage: navigator.language,
  browserColorDepth: window.screen.colorDepth.toString(),
  browserScreenHeight: window.screen.height.toString(),
  browserScreenWidth: window.screen.width.toString(),
  browserTZ: new Date().getTimezoneOffset().toString(),
  browserTzIana: Intl.DateTimeFormat().resolvedOptions().timeZone,
  browserUserAgent: navigator.userAgent,
  browserWebAuthnSupported: "PublicKeyCredential" in window,
  browserPlatformAuthenticatorAvailable:
    await isWebAuthnPlatformAuthenticatorAvailable(),
  browserMaxTouchPoints: navigator.maxTouchPoints,
  browserWebdriver: navigator.webdriver,
});
