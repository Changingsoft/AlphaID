package com.tobaccoid.samples.resourceserver;

import com.nimbusds.jose.JOSEObjectType;
import com.nimbusds.jose.proc.DefaultJOSEObjectTypeVerifier;
import com.nimbusds.jose.proc.SecurityContext;
import org.springframework.context.annotation.Bean;
import org.springframework.http.HttpMethod;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.oauth2.jwt.JwtDecoder;
import org.springframework.security.oauth2.jwt.NimbusJwtDecoder;
import org.springframework.security.web.SecurityFilterChain;

@EnableWebSecurity
public class ResourceServerConfig {
    @Bean
    SecurityFilterChain securityFilterChain(HttpSecurity http) throws Exception{
        http.authorizeRequests(
                auth -> auth.antMatchers(HttpMethod.GET, "/api/**")
                        .hasAnyAuthority("SCOPE_profile")
                        .anyRequest()
                        .authenticated())
                .oauth2ResourceServer(oauth2 -> oauth2.jwt());

        return http.build();
    }

    @Bean
    JwtDecoder jwtDecoder() {
        DefaultJOSEObjectTypeVerifier<SecurityContext> verifier =
                new DefaultJOSEObjectTypeVerifier<>(new JOSEObjectType("at+jwt"));
        NimbusJwtDecoder decoder = NimbusJwtDecoder.withJwkSetUri("https://auth.changingsoft.com/.well-known/openid-configuration/jwks")
                .jwtProcessorCustomizer((processor) -> processor.setJWSTypeVerifier(verifier))
                .build();
        // ... any other decoder settings
        return decoder;
    }
}
