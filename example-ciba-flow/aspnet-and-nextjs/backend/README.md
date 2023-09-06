# ASP.NET CIBA flow example Backend

The backend for the example application to demonstrate payment authentication using the CIBA flow, built using ASP.NET.

## Prerequisites

1. [dotnet](https://dotnet.microsoft.com/en-us/download) v6

## Getting Started

See the [parent readme](../README.md#getting-started-with-docker) for instructions on how to get started.

## Development

First, install the dependencies by restoring the project:

```bash
cd /app
dotnet restore
```

Then, run the project:

```bash
dotnet run
```

## Stack

- [ASP.NET](https://dotnet.microsoft.com/en-us/apps/aspnet)
- C#

## Endpoints

The OpenAPI definition automatically generated from the endpoint attributes
in [the controller](app/Controllers/PaymentController.cs).
The Swagger UI should be available at `http://localhost:7244/swagger/index.html` in development mode.

## Folder structure

- [app](app) - The project directory
    - [Program.cs](app/Program.cs) - The entrypoint for the backend, this is where the application is started.
    - [Controllers](app/Controllers) - The controllers directory, where the controllers reside. This is where the
      endpoints are defined.
    - [Exceptions](app/Exceptions) - The exceptions directory, where the custom exceptions reside.
    - [Models](app/Models) - The models directory, where the models reside. This contains the models for the request and
      response bodies, as well as the models for the BankID (with Biometrics) OIDC API.
    - [Services](app/Services) - The services directory, where the services reside. This contains the services for the
      BankID OIDC API and the BankID with Biometrics OIDC API.
    - [Storage](app/Storage) - The storage directory, where the cache logic resides. This contains the cache logic for
      the access token (exchanged using client credentials flow) and OIDC configuration.
    - [Utils](app/Utils)
    - [Dockerfile](app/Dockerfile) - The Dockerfile for the backend, which is used to build the backend image.
    - [appsettings.example.json](app/appsettings.example.json) - The example appsettings file, which should be copied
      to `appsettings.json` and filled out with the appropriate values.
