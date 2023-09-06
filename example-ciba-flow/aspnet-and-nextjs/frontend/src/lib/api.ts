import {
  ConfirmPaymentRequest,
  ConfirmPaymentResponse,
  ErrorResponse,
  PollSuccessResponse,
  UserCheckRequest,
  UserCheckResponse,
} from "@/lib/types";

const BASE_URI: string =
  process.env.NEXT_PUBLIC_API_URI ?? "http://localhost:7244";

const post = async <TRequest, TResponse>(
  path: string,
  body: TRequest,
): Promise<TResponse> => {
  const res = await fetch(BASE_URI + path, {
    method: "POST",
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  });
  return res.json();
};

export const userCheck = async (
  request: UserCheckRequest,
): Promise<UserCheckResponse | ErrorResponse> =>
  post("/api/usercheck", request);

export const confirmPayment = async (
  request: ConfirmPaymentRequest,
): Promise<ConfirmPaymentResponse | ErrorResponse> =>
  post("/api/confirmpayment", request);

export const pollResult = async (): Promise<PollSuccessResponse> => {
  const res = await fetch(`${BASE_URI}/api/poll`, {
    method: "POST",
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({}),
  });
  const json = await res.json();

  if (!res.ok) {
    throw json;
  }

  return json;
};
