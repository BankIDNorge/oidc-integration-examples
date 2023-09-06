import type { BIDSession } from "@/lib/types";

export interface UserCardProps {
  session: BIDSession;
}

export default function UserCard({ session }: UserCardProps) {
  return (
    <div className="flex flex-col gap-1 p-4 rounded-lg bg-background-light text-text-light w-96">
      <span className="font-bold">
        {session.user.familyName}, {session.user.givenName}
      </span>
      <p>
        {session.user.nnin} - Born {session.user.birthdate}
      </p>
    </div>
  );
}
