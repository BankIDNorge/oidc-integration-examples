package no.bankid.example.model;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import org.springframework.security.oauth2.core.oidc.user.OidcUser;

import java.util.Map;

@JsonSerialize
public record UserViewModel(
    @JsonProperty String sub,
    @JsonProperty String name,
    @JsonProperty String givenName,
    @JsonProperty String familyName,
    @JsonProperty String birthdate,
    @JsonProperty Map<String, Object> claims
) {
    public static UserViewModel from(OidcUser user) {
        return new UserViewModel(
            user.getSubject(),
            user.getUserInfo().getFullName(),
            user.getUserInfo().getGivenName(),
            user.getUserInfo().getFamilyName(),
            user.getUserInfo().getBirthdate(),
            user.getClaims()
        );
    }
}
