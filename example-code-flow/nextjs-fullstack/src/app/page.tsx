"use client";

import LoginButton from "@/app/components/LoginButton";
import UserCard from "@/app/components/UserCard";
import LogoutButton from "@/app/components/LogoutButton";
import type { BIDSession } from "@/lib/types";
import { useSearchParams } from "next/navigation";
import { useSession } from "next-auth/react";
import { useState } from "react";

export default function Home() {
  const { data, status } = useSession();
  const [useSubstantial, setUseSubstantial] = useState(true);
  const params = useSearchParams();

  return (
    <main className="mx-auto max-w-screen-lg flex flex-col items-center gap-2 px-4 py-8">
      <h1 className="text-2xl font-medium">BankID OIDC Example</h1>
      <p className="mb-4">
        {data ? `Welcome, ${data.user?.name}` : "Welcome to the example app!"}
      </p>
      {status == "loading" && (
        <>
          <p>Loading...</p>
        </>
      )}
      {status == "unauthenticated" && (
        <>
          <LoginButton loginHint={useSubstantial ? "BIS" : "BID"} />
          <div className="flex flex-row gap-2 text-gray-600 mt-2">
            <input
              id="bis-checkbox"
              type="checkbox"
              checked={useSubstantial}
              onChange={() => setUseSubstantial((v) => !v)}
            />
            <label htmlFor="bis-checkbox">Use BankID Substantial</label>
          </div>
        </>
      )}
      {status === "authenticated" && (
        <>
          <UserCard session={data as BIDSession} />
          <LogoutButton />
        </>
      )}
      {params.has("error") && (
        <div className="bg-red-500 p-4 rounded-lg font-bold text-white mt-4">
          Authentication error: {params.get("error")}
        </div>
      )}
    </main>
  );
}
