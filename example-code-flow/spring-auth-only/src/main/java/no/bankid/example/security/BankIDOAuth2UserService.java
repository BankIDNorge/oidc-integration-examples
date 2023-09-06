package no.bankid.example.security;

import org.springframework.core.convert.converter.Converter;
import org.springframework.http.RequestEntity;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.oauth2.client.http.OAuth2ErrorResponseErrorHandler;
import org.springframework.security.oauth2.client.userinfo.OAuth2UserRequest;
import org.springframework.security.oauth2.client.userinfo.OAuth2UserRequestEntityConverter;
import org.springframework.security.oauth2.client.userinfo.OAuth2UserService;
import org.springframework.security.oauth2.core.OAuth2AccessToken;
import org.springframework.security.oauth2.core.OAuth2AuthenticationException;
import org.springframework.security.oauth2.core.OAuth2Error;
import org.springframework.security.oauth2.core.user.DefaultOAuth2User;
import org.springframework.security.oauth2.core.user.OAuth2User;
import org.springframework.security.oauth2.core.user.OAuth2UserAuthority;
import org.springframework.security.oauth2.jwt.Jwt;
import org.springframework.security.oauth2.jwt.JwtDecoder;
import org.springframework.util.Assert;
import org.springframework.util.StringUtils;
import org.springframework.web.client.RestClientException;
import org.springframework.web.client.RestOperations;
import org.springframework.web.client.RestTemplate;

import java.util.LinkedHashSet;
import java.util.Map;
import java.util.Set;

/**
 * An {@link OAuth2UserService} implementation that supports JWT-encoded user info response.
 * This implementation is mainly based on {@link org.springframework.security.oauth2.client.userinfo.DefaultOAuth2UserService}.
 * Spring's implementation is not compatible with BankID's OpenID Connect provider due to the lack of JWT support for the userinfo endpoint.
 */
public class BankIDOAuth2UserService implements OAuth2UserService<OAuth2UserRequest, OAuth2User> {
    private static final String MISSING_USER_INFO_URI_ERROR_CODE = "missing_user_info_uri";

    private static final String MISSING_USER_NAME_ATTRIBUTE_ERROR_CODE = "missing_user_name_attribute";

    private static final String INVALID_USER_INFO_RESPONSE_ERROR_CODE = "invalid_user_info_response";

    private final Converter<OAuth2UserRequest, RequestEntity<?>> requestEntityConverter = new OAuth2UserRequestEntityConverter();

    private final JwtDecoder jwtDecoder;

    private final RestOperations restOperations;


    public BankIDOAuth2UserService(JwtDecoder jwtDecoder) {
        this.jwtDecoder = jwtDecoder;
        RestTemplate restTemplate = new RestTemplate();
        restTemplate.setErrorHandler(new OAuth2ErrorResponseErrorHandler());
        this.restOperations = restTemplate;
    }

    @Override
    public OAuth2User loadUser(OAuth2UserRequest userRequest) throws OAuth2AuthenticationException {
        Assert.notNull(userRequest, "userRequest cannot be null");
        if (!StringUtils
            .hasText(userRequest.getClientRegistration().getProviderDetails().getUserInfoEndpoint().getUri())) {
            OAuth2Error oauth2Error = new OAuth2Error(MISSING_USER_INFO_URI_ERROR_CODE,
                "Missing required UserInfo Uri in UserInfoEndpoint for Client Registration: "
                    + userRequest.getClientRegistration().getRegistrationId(),
                null);
            throw new OAuth2AuthenticationException(oauth2Error, oauth2Error.toString());
        }
        String userNameAttributeName = userRequest.getClientRegistration().getProviderDetails().getUserInfoEndpoint()
            .getUserNameAttributeName();
        if (!StringUtils.hasText(userNameAttributeName)) {
            OAuth2Error oauth2Error = new OAuth2Error(MISSING_USER_NAME_ATTRIBUTE_ERROR_CODE,
                "Missing required \"user name\" attribute name in UserInfoEndpoint for Client Registration: "
                    + userRequest.getClientRegistration().getRegistrationId(),
                null);
            throw new OAuth2AuthenticationException(oauth2Error, oauth2Error.toString());
        }
        RequestEntity<?> request = this.requestEntityConverter.convert(userRequest);
        Map<String, Object> userAttributes = this.requestUserAttributes(userRequest, request);
        Set<GrantedAuthority> authorities = new LinkedHashSet<>();
        authorities.add(new OAuth2UserAuthority(userAttributes));
        OAuth2AccessToken token = userRequest.getAccessToken();
        for (String authority : token.getScopes()) {
            authorities.add(new SimpleGrantedAuthority("SCOPE_" + authority));
        }
        return new DefaultOAuth2User(authorities, userAttributes, userNameAttributeName);
    }

    private Map<String, Object> requestUserAttributes(OAuth2UserRequest userRequest, RequestEntity<?> request) {
        try {
            ResponseEntity<String> response = this.restOperations.exchange(request, String.class);
            Jwt jwt = this.jwtDecoder.decode(response.getBody());
            return jwt.getClaims();
        } catch (OAuth2AuthenticationException ex) {
            OAuth2Error oauth2Error = ex.getError();
            StringBuilder errorDetails = new StringBuilder();
            errorDetails.append("Error details: [");
            errorDetails.append("UserInfo Uri: ")
                .append(userRequest.getClientRegistration().getProviderDetails().getUserInfoEndpoint().getUri());
            errorDetails.append(", Error Code: ").append(oauth2Error.getErrorCode());
            if (oauth2Error.getDescription() != null) {
                errorDetails.append(", Error Description: ").append(oauth2Error.getDescription());
            }
            errorDetails.append("]");
            oauth2Error = new OAuth2Error(INVALID_USER_INFO_RESPONSE_ERROR_CODE,
                "An error occurred while attempting to retrieve the UserInfo Resource: " + errorDetails,
                null);
            throw new OAuth2AuthenticationException(oauth2Error, oauth2Error.toString(), ex);
        } catch (RestClientException ex) {
            OAuth2Error oauth2Error = new OAuth2Error(INVALID_USER_INFO_RESPONSE_ERROR_CODE,
                "An error occurred while attempting to retrieve the UserInfo Resource: " + ex.getMessage(), null);
            throw new OAuth2AuthenticationException(oauth2Error, oauth2Error.toString(), ex);
        }
    }
}