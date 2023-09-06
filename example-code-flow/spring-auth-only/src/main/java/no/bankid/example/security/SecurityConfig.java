package no.bankid.example.security;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.oauth2.client.oidc.userinfo.OidcUserService;
import org.springframework.security.oauth2.client.registration.ClientRegistrationRepository;
import org.springframework.security.oauth2.client.web.OAuth2AuthorizationRequestRedirectFilter;
import org.springframework.security.oauth2.jwt.JwtDecoder;
import org.springframework.security.oauth2.jwt.NimbusJwtDecoder;
import org.springframework.security.web.SecurityFilterChain;

@Configuration
public class SecurityConfig {
    private final ClientRegistrationRepository repository;

    @Autowired
    public SecurityConfig(ClientRegistrationRepository repository) {
        this.repository = repository;
    }

    @Bean
    public SecurityFilterChain filterChain(HttpSecurity http) throws Exception {
        return http
            .authorizeHttpRequests(config -> {
                // Authenticate ANY requests. This is only for demonstration purposes.
                // Configure this to match your application's security requirements.
                config.anyRequest().authenticated();
            })
            .oauth2Login(config -> config
                .authorizationEndpoint(endpointConfig ->
                    // Set the custom authorization request resolver that adds a login_hint parameter to the authorization request.
                    // This prevents the need to hardcode the authorization endpoint URI in the application.
                    endpointConfig.authorizationRequestResolver(new BankIDAuthorizationRequestResolver(
                        this.repository,
                        OAuth2AuthorizationRequestRedirectFilter.DEFAULT_AUTHORIZATION_REQUEST_BASE_URI
                    ))
                )
                .redirectionEndpoint(endpointConfig -> {
                    // This change is not required. It is only here to match the existing redirect_uri in our test application.
                    // The base URI must match redirect_uri specified in the application configuration.
                    // Alternatively, this can be removed if you would like to use Spring's default callback URI.
                    endpointConfig.baseUri("/api/auth/callback");
                })
                .userInfoEndpoint(endpointConfig -> {
                    // Set the custom OidcUserService that is able to decode the ID token in JWT format.
                    // See the bean below.
                    endpointConfig.oidcUserService(this.oidcUserService());
                }))
            .build();
    }

    /**
     * Returns a custom implementation of the OidcUserService that is able to decode the ID token in JWT format.
     *
     * @return a new instance of {@link BankIDOAuth2UserService}
     * @see BankIDOAuth2UserService
     */
    @Bean
    public OidcUserService oidcUserService() {
        OidcUserService userService = new OidcUserService();
        String jwkSetUri = this.repository.findByRegistrationId("bankid").getProviderDetails().getJwkSetUri();
        JwtDecoder jwtDecoder = NimbusJwtDecoder.withJwkSetUri(jwkSetUri).build();
        userService.setOauth2UserService(new BankIDOAuth2UserService(jwtDecoder));
        return userService;
    }
}
