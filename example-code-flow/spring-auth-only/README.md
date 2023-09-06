# example-spring

An example application integrating with BankID OpenID Connect, built using Spring Boot.

## Prerequisites

1. JDK 17+
2. [mkcert](https://github.com/FiloSottile/mkcert)
3. [nss](https://developer.mozilla.org/en-US/docs/Mozilla/Projects/NSS) (Optional; required for Firefox)
4. [IntelliJ IDEA](https://www.jetbrains.com/idea/) (Optional; recommended for development)

## Getting Started

1. [Register a test application for OpenID Connect](https://developer.bankid.no/bankid-with-biometrics/testing/).
    * Use `https://localhost:3000/api/auth/callback` as the redirect URI

2. Set up self-signed certificates
    ```bash
    mkcert -install
    mkcert -pkcs12 -p12-file certs/localhost.p12 localhost
    ```

3. Build the application
    ```bash
    ./gradlew bootJar
    ```

4. Run the application
   ```bash
   export BANKID_CLIENT_ID=<client-id>
   export BANKID_CLIENT_SECRET=<client-secret>
   java -jar -Dspring.profiles.active=default,dev \
     build/libs/example-spring-0.0.1-SNAPSHOT.jar
   ```
   The application will be available at `https://localhost:3000`.
   Visiting https://localhost:3000/user should initiate the authentication flow and eventually respond with the user's
   info when the flow is complete.

## Stack

- Spring Boot
- Spring Security
- Java

## How Does This Work?

This example uses the [Spring Security](https://spring.io/projects/spring-security) package to handle authentication.
Contrary to the Next.js example, the session in this example is stateful, meaning that the user's session is stored the
memory. This is the default behavior of Spring Security, and it can impact the scalability of the application. It is
recommended to implement a stateless session instead, but that is outside the scope of this example.

### Project Files

- [application.yml](src/main/resources/application.yml) - The configuration file for the application. The BankID OIDC
  provider is defined here.
- [application-dev.yml](src/main/resources/application-dev.yml) - The configuration file used to enable HTTPS using
  self-signed TLS certificate during development.
- [SecurityConfig.java](src/main/java/no/bankid/example/security/SecurityConfig.java) - The configuration class for
  Spring Security. This class defines various security-related settings, such as the authentication provider. **Make
  sure** to read the comments in this file carefully.
- [BankIDAuthorizationRequestResolver.java](src/main/java/no/bankid/example/security/BankIDAuthorizationRequestResolver.java) -
  This class is responsible for resolving the authorization request URI and append the `login_hint` parameter to the
  request. **Make sure** to read the comments in this file carefully.
