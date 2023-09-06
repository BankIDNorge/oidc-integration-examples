export interface LoginHint {
  scheme: string;
  value: string;
}

export interface ConfirmPaymentRequest {
  amount: string;
  currency: string;
  loginHint: LoginHint;
  signals: Signals;
}

export interface ConfirmPaymentResponse {
  bindingMessage: string;
}

export type UserCheckRequest = LoginHint;

export interface UserCheckResponse {
  exists: boolean;
}

export interface PollSuccessResponse {
  success: true;
}

export interface ErrorResponse {
  error: string;
  errorDescription: string;
}

export type Signals = {
  browserJavaEnabled: boolean;
  browserJavascriptEnabled: boolean;
  browserLanguage: string;
  browserColorDepth: string;
  browserScreenHeight: string;
  browserScreenWidth: string;
  browserTZ: string;
  browserTzIana: string;
  browserUserAgent: string;
  browserWebAuthnSupported: boolean;
  browserPlatformAuthenticatorAvailable: boolean;
  browserMaxTouchPoints: number;
  browserWebdriver: boolean;
};

export const isError = <T extends object>(
  object: T | ErrorResponse,
): object is ErrorResponse => {
  return "error" in object;
};
