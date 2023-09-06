import type { BIDUser } from "@/lib/types";
import { decodeJwt } from "jose";
import NextAuth from "next-auth";

const handler = NextAuth({
  pages: {
    signIn: "/",
    signOut: "/",
    error: "/",
    verifyRequest: "/",
  },
  providers: [
    {
      id: "bid",
      name: "BankID",
      type: "oauth",
      wellKnown: process.env.NEXTAUTH_PROVIDER_CONFIGURATION,
      authorization: {
        params: {
          scope: "openid profile nnin_altsub",
        },
      },
      clientId: process.env.NEXTAUTH_CLIENT_ID,
      clientSecret: process.env.NEXTAUTH_CLIENT_SECRET,
      idToken: true,
      checks: ["pkce", "state", "nonce"],
      profile(profile) {
        return {
          id: profile.sub,
          name: profile.name,
        };
      },
    },
  ],
  callbacks: {
    // Creates a JWT that is returned to the client during sign-in.
    // This is not the same as the JWT access_token that is returned by BankID.
    jwt({ token, account }) {
      if (account) {
        // Store the access_token in the JWT, so it can be used to make calls to the BankID API.
        // We don't store additional information here to minimize the size of the JWT.
        token.access_token = account.access_token;
        token.id_token = account.id_token;
      }

      // This JWT is encrypted as a JWE and will be stored in a cookie.
      return token;
    },
    // Returns session data to the client.
    // The token is the JWT that was created in the jwt callback.
    session({ session, token }) {
      const payload = decodeJwt(token["id_token"] as string);
      const user = session.user as Partial<BIDUser>;
      user.id = payload.sub as string;
      user.givenName = payload.given_name as string;
      user.familyName = payload.family_name as string;
      user.birthdate = payload.birthdate as string;
      user.nnin = payload.nnin_altsub as string;

      return session;
    },
    signIn({ account }) {
      if (!account?.id_token) {
        return false;
      }
      const idToken = decodeJwt(account.id_token);
      // Verify the 'acr' claim to make sure the user has authenticated with the
      // appropriate level of assurance (LOA).
      return true;
    },
  },
});

export { handler as GET, handler as POST };
