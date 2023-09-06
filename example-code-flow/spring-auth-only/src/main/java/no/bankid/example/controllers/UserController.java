package no.bankid.example.controllers;

import no.bankid.example.model.UserResponse;
import no.bankid.example.model.UserViewModel;
import org.springframework.security.oauth2.client.authentication.OAuth2AuthenticationToken;
import org.springframework.security.oauth2.core.oidc.user.OidcUser;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.ResponseBody;

import java.security.Principal;

@Controller
@RequestMapping("/user")
public class UserController {
    @GetMapping
    public @ResponseBody UserResponse getUser(Principal principal) {
        if (principal instanceof OAuth2AuthenticationToken token && token.getPrincipal() instanceof OidcUser user) {
            return new UserResponse(UserViewModel.from(user));
        }
        return new UserResponse(null);
    }
}
