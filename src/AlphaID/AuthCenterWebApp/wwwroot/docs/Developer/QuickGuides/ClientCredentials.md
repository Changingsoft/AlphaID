# 基于客户端身份验证的应用

应用程序活动不需要最终用户参与，但服务者需要识别是客户端身份，例如定时执行的程序。

## 对接和工作过程

发起服务调用前，须向IdP发起身份验证，身份验证通过后，您将获得AccessToken。

按要求携带AccessToken向服务者发起调用。

服务者将验证您的身份并执行授权检查，决定允许获拒绝调用。

AccessToken具有有效期，您的应用必须自己管理缓存AccessToken。

## 工作流

### 身份验证

客户端尚未获得AccessToken，或AccessToken已过期时，必须向IdP发起身份验证。

向IdP的Token终结点发起POST请求，请求体form-urlencoded方式携带以下参数：

``` HTTP
POST /connect/token HTTP/1.1
Host: <IdP的域名>
Content-Type: application/x-www-form-urlencoded
Content-Length: <Length of body>

client_id=<client_id>&client_secret=<client_secret>&grant_type=client_credentials&scope=<scopes>
```

> 所有**参数的值**都必须使用url-cncode转码。

|参数|必需|说明|
|---|---|---|
|client_id|是|您的应用的唯一标识符。|
|client_secret|是|您的应用的密钥。|
|grant_type|是|必须为`client_credentials`。|
|scope *|否|您请求的权限范围，多个范围用空格分隔。|

> *如果未提供scope，则颁发的令牌将包括客户端登记时所允许的所有ApiScope。

如果验证成功，将返回如下JSON结果：

``` JSON
{
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IkRCQTNCREZBMTIzRUE5QkFGMEFDMDk0NUM3MzdDMjZGIiwidHlwIjoiYXQ...",
    "expires_in": 3600,
    "token_type": "Bearer",
    "scope": "public"
}
```
|字段|说明|
|---|---|
|access_token|访问令牌，将在后续用于API接口调用时证明身份。|
|expires_in|失效的时间，以秒为单位，自当前时刻起经历多少秒后该令牌失效。|
|token_type|通常为`Bearer`。|
|scope|此令牌的应用范围，通常是客户端允许的，并在请求时声明的权限范围。|

如果失败，返回JSON结果，指示错误信息：

``` JSON
{
    "error": "invalid_client"
}
```

> **获得令牌后，请按生存期保存和复用令牌。令牌终结点具有速率限制，请勿在每次服务调用时都请求令牌，以避免速率限制。**

### 调用服务

向服务节点发起调用时，在请求头添加`Authorization`字段，带上AccessToken以证明身份。以下为一个调用示例：

``` HTTP
GET /service HTTP1.1
Host: <服务的域名>
Authorization: Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IkRCQTNCREZBMTIzRUE5QkFGMEFDMDk0NUM3MzdDMjZGIiwidHlwIjoiYXQ...
...
```

> `Authorization`头的格式为`<token-type> <value>`。

## 相关参考

开发过程中，你可以使用已实现的客户端库，以提高开发效率，降低潜在安全风险。请参阅[OIDC客户端库](../KB/OidcClientLibs.md)。