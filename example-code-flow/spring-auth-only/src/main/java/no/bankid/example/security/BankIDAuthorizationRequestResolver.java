package no.bankid.example.security;

import jakarta.servlet.http.HttpServletRequest;
import org.springframework.security.oauth2.client.registration.ClientRegistrationRepository;
import org.springframework.security.oauth2.client.web.DefaultOAuth2AuthorizationRequestResolver;
import org.springframework.security.oauth2.client.web.OAuth2AuthorizationRequestResolver;
import org.springframework.security.oauth2.core.endpoint.OAuth2AuthorizationRequest;

import java.util.Map;

/**
 * An implementation of {@link OAuth2AuthorizationRequestResolver} that adds a login_hint parameter to the authorization request.
 */
public class BankIDAuthorizationRequestResolver implements OAuth2AuthorizationRequestResolver {
    private final OAuth2AuthorizationRequestResolver defaultResolver;

    public BankIDAuthorizationRequestResolver(ClientRegistrationRepository repository, String authorizationRequestBaseUri) {
        this.defaultResolver = new DefaultOAuth2AuthorizationRequestResolver(
            repository,
            authorizationRequestBaseUri
        );
    }

    @Override
    public OAuth2AuthorizationRequest resolve(HttpServletRequest request) {
        OAuth2AuthorizationRequest authorizationRequest = this.defaultResolver.resolve(request);
        if (authorizationRequest != null) {
            return this.customizeAuthorizationRequest(authorizationRequest, request);
        }
        return null;
    }

    @Override
    public OAuth2AuthorizationRequest resolve(HttpServletRequest request, String clientRegistrationId) {
        OAuth2AuthorizationRequest authorizationRequest = this.defaultResolver.resolve(request, clientRegistrationId);
        if (authorizationRequest != null) {
            return this.customizeAuthorizationRequest(authorizationRequest, request);
        }
        return null;
    }

    /**
     * Adds a login_hint parameter to the authorization request if specified in the request by the user.
     * Otherwise, it will use the login_hint "BIS" for BankID Substantial.
     * <p>
     * You should NOT allow the user to specify the login_hint parameter in a production application,
     * unless you have a good reason to do so.
     */
    private OAuth2AuthorizationRequest customizeAuthorizationRequest(OAuth2AuthorizationRequest authorizationRequest, HttpServletRequest request) {
        String loginHint = request.getParameter("login_hint");
        if (loginHint == null) {
            loginHint = "BIS";
        }
        return OAuth2AuthorizationRequest.from(authorizationRequest)
            .additionalParameters(Map.of("login_hint", loginHint))
            .build();
    }
}
