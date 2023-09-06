import "./globals.css";
import { Roboto } from "next/font/google";
import clsx from "clsx";

const roboto = Roboto({
  weight: ["400", "500"],
  subsets: ["latin"],
});

export const metadata = {
  title: "TopShop",
  description: "The best shop, ever.",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body
        className={clsx(
          roboto.className,
          "bg-background-secondary text-text-light",
        )}
      >
        {children}
      </body>
    </html>
  );
}
