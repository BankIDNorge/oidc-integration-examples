import { Session } from "next-auth";

export type BIDSession = Omit<Session, "user"> & { user: BIDUser };

export interface BIDUser {
  id: string;
  name: string;
  givenName: string;
  familyName: string;
  birthdate: string;
  nnin: string;
}
