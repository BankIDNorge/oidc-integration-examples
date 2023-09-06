import type { ReactNode } from "react";
import clsx from "clsx";

export interface CardContainerProps {
  children: ReactNode;
  center?: boolean;
}

const CardContainer = ({ children, center }: CardContainerProps) => (
  <main className="flex min-h-screen flex-col justify-center max-w-sm mx-auto">
    <div
      className={clsx(
        "flex flex-col bg-background-light justify-center gap-8 rounded-2xl p-16",
        center && "items-center",
      )}
    >
      {children}
    </div>
  </main>
);

export default CardContainer;
