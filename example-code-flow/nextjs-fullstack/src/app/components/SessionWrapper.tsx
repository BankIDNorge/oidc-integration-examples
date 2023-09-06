"use client";

import { ReactNode } from "react";
import { SessionProvider } from "next-auth/react";

export interface SessionWrapperProps {
  children: ReactNode;
}

export default function SessionWrapper({ children }: SessionWrapperProps) {
  return <SessionProvider>{children}</SessionProvider>;
}
