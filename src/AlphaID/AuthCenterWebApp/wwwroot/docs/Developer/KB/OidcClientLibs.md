# OpenID Connect 客户端库

应用开发过程中，你可以借助已有实现的客户端库，来处理OAuth/OIDC认证的细节，以提高生产效率，降低潜在的安全缺陷。

以下列出了在一些常见开发平台和框架下可以使用的客户端库：

* asp.net core

``` cmd
nuget install Microsoft.AspNetCore.Authentication.OpenIdConnect
```

在Program.cs中添加OIDC身份验证：

``` c#
builder.Services.AddAuthentication()
    .AddOpenIdConnect(options =>
    {
        options.Authority = "<IdP Authority>";
        options.ClientId = "<Client ID>";
        options.ClientSecret = "<Client Secret>";
        options.ResponseType = OpenIdConnectResponseType.Code;
        //options.Scope.Add(OpenIdConnectScope.Email);
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
    });
```

* java spring on maven

``` xml
<dependency>
    <groupId>org.springframework.security</groupId>
    <artifactId>spring-security-oauth2-client</artifactId>
</dependency>
```

* node npm

```
npm install oidc-client
```

## 相关参考

更多客户端接入示例，请参阅Github [AlphaId.Samples](https://github.com/Changingsoft/AlphaId.Samples.git)。