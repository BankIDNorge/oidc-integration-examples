# example-nextjs

The frontend for the example application to demonstrate payment authentication using the CIBA flow, built using Next.js.

## Prerequisites

1. [Node.js](https://nodejs.org/en/) v18 LTS
2. [pnpm](https://pnpm.io/) v8

## Getting Started

See the [parent readme](../README.md#getting-started-with-docker) for instructions on how to get started.

## Development

First, install the dependencies:

```bash
pnpm install
```

Then, run the development server:

```bash
pnpm dev
```

Now you should be able to open a browser at `http://localhost:3000` to test out the application. Do note that the
backend is required for the frontend to function properly.

## Stack

- [Next.js](https://nextjs.org/)
- [React](https://react.dev/)
- [Tailwind CSS](https://tailwindcss.com/)
- [TypeScript](https://www.typescriptlang.org/)

## Folder structure

- [src/app](src/app) - The app directory, where the pages and components reside.
  See [Next.js' documentation](https://nextjs.org/docs/getting-started/project-structure#app-routing-conventions) for
  more information.
- [src/lib](src/lib) - The lib directory, where the utility functions and type definitions reside.
- [src/assets](src/assets) - The assets directory, where the icons and other assets reside.
- [Dockerfile](Dockerfile) - The Dockerfile for the frontend, this is used to build the frontend container image.

## How does this work?

1. The user navigates to the [home page](src/app/page.tsx) of the frontend. They are presented with a form where they
   can enter the desired payment amount and currency. After submitting the form, the user is redirected to
   the [payment page](src/app/payment/page.tsx).
2. The [payment page](src/app/payment/page.tsx) prompts the user for their national identity number (nnin). After
   submitting the form, a check is performed to ensure that the user has enabled BankID with Biometrics.
3. The [payment page](src/app/payment/page.tsx) will then send a request to the backend to register the payment with
   BankID with Biometrics. The backend will then initiate the authentication flow using CIBA.
4. The user will be presented with
   a [binding message](https://openid.net/specs/openid-client-initiated-backchannel-authentication-core-1_0.html#auth_request)
   on the payment page, which they can compare with the message on their BankID app.
5. The payment page will periodically poll for the authentication status. Once the user has authenticated, the payment
   page will redirect the user to the [success page](src/app/success/page.tsx).

> **Note:** if any errors occur during the authentication flow, the user will be redirected to
> the [error page](src/app/error/page.tsx). This example does not display any error messages to the user, but you can
> easily add this functionality yourself.
