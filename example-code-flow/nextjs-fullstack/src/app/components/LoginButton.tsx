import BankIDIconWhite from "@/assets/logo/DNA-element-white.svg";
import { getCsrfToken } from "next-auth/react";

export interface LoginButtonProps {
  loginHint: string;
}

export default function LoginButton({ loginHint }: LoginButtonProps) {
  const handleLogin = async () => {
    // TODO: handle error?
    const res = await fetch(`/api/auth/signin/bid?login_hint=${loginHint}`, {
      method: "post",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      body: new URLSearchParams({
        csrfToken: (await getCsrfToken()) as string,
        json: "true",
      }),
    });

    const data = await res.json();
    window.location.href = data.url;
  };
  return (
    <button
      onClick={handleLogin}
      className="flex flex-row items-center gap-3.5 bg-background-light hover:bg-background-light-hover text-text-light text-lg font-medium px-6 py-2 rounded-full"
    >
      <BankIDIconWhite className="w-8 h-8" />
      Log in with BankID
    </button>
  );
}
