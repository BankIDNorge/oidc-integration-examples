import "./globals.css";

import { Roboto } from "next/font/google";
import SessionWrapper from "@/app/components/SessionWrapper";

const roboto = Roboto({
  weight: ["400", "500"],
  subsets: ["latin"],
});

export const metadata = {
  title: "BankID OIDC Example",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en" className={roboto.className}>
      <body>
        <SessionWrapper>{children}</SessionWrapper>
      </body>
    </html>
  );
}
