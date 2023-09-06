import { getCsrfToken } from "next-auth/react";

export default function LogoutButton() {
  const handleLogin = async () => {
    // TODO: handle error?
    const res = await fetch(`/api/auth/signout/bid`, {
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
      className="bg-background-light hover:bg-background-light-hover text-text-light text-lg font-medium px-6 py-2 rounded-full"
    >
      Log out
    </button>
  );
}
