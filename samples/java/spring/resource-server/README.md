# Java Spring Resource Server 示例

这是一个使用spring-boot技术构建的示例，示例是一个资源服务器（通常来说就是提供WebAPI接口服务的程序）。

## 基本原理

spring框架的jwt token验证已经集成在了spring security中，因此只需要简单配置即可启用。

基本的配置步骤如下：
1. 引入`spring-boot-starter-oauth2-resource-server`
2. 配置 `spring.security.oauth2.resourceserver.jwt.issuer-uri`，指向IdP
3. 配置 `SecurityFilterChain`，启用身份验证和授权

## 已知问题

### 对header.typ=at+jwt的JWT令牌的支持

详见：https://github.com/spring-projects/spring-security/issues/10272

spring 目前无法处理typ值为at+jwt的令牌，若要执行处理，需要替换NimbusJwtDecoder：

``` java
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
```