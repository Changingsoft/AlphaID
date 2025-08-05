# 位于服务器上的应用

您的应用程序实际运行在服务器上，适用此场景。例如JSP、PHP、ASP、ASP.NET等。

## 可用的授权模式

|授权类型|说明|
|---|---|
|authorization_code|授权码。推荐的标准的OAuth授权工作流。|

> 原则上IdP不许可其他授权模式。若为兼容旧应用或有其他限制条件，请咨询IdP的运营方以评估其他授权模式。

## 工作流程

### 身份验证（授权码模式）

身份验证简要过程如下：

1. 用户访问资源服务。资源服务判断用户是否已证明身份，若未证明身份，则应发起身份验证挑战、将用户重定向到IdP，引导用户完成身份验证。
2. 用户在IdP提供的安全界面验证自己的身份。若验证成功，IdP将发出授权码，引导用户回到资源服务的回调地址。
3. 资源服务处理回调获得授权码，然后联系IdP的令牌节点，换取访问令牌（access_token）和（或）身份令牌（id_token）。
4. 资源服务携带access_token访问用户信息终结点（userinfo endpoint），以获取完整的用户信息。

资源服务可以以身份令牌验证用户身份，持有访问令牌用于在后续以用户身份请求其他资源服务。

#### 验证挑战

资源服务应构造适当的URL query，并发出302跳转，将用户重定向到IdP的authorize终结点。以下给出了GET请求authorize终结点的参数示例：

``` HTTP
GET /authorize?client_id=<client_id>&redirect_uri=<redirect_uri>&response_type=code&scope=<scope>&code_challenge=<code_challenge>&code_challenge_method=<code_callenge_method>&response_mode=<response_mode>&nonce=<nonce>&state=<state>
Host: <IdP的域名>
...

```
|名称|必需|说明|
|---|---|---|
|client_id|是||
|redirect_uri|是|回调地址，必须是一个完整的URL，且必须在与注册客户端所提供的回调地址清单中。|
|response_type|||
|scope|||
|code_challenge|N/A|[参见PKCE](../KB/PKCE.md)|
|code_challenge_method|N/A|[参见PKCE](../KB/PKCE.md)|
|response_mode|||
|nonce|||
|state|否|资源服务自定的值，身份验证通过后回到资源服务时原样返回|

#### 返回授权码

IdP成功验证用户身份后，将根据redirect_uri、response_type、response_mode的设置，以恰当的方式将授权码返回给资源服务。

以下示例显示了IdP响应302跳转，以构造了URL Query的方式向资源服务返回授权码：

``` HTTP
HTTP/1.1 302 Found
Location: <YOUR_REDIRECT_URI>?code=SplxlOBeZQQYbYS6WxSbIA
```

#### 获取令牌

资源服务携带授权码，向令牌终结点获取令牌。

``` HTTP
POST /token
Content-Type: application/x-www-form-urlencoded

grant_type=authorization_code
&code=SplxlOBeZQQYbYS6WxSbIA
&client_id=YOUR_CLIENT_ID
&client_secret=YOUR_CLIENT_SECRET
&code_verifier=dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk
```
|参数|必须|描述|
|---|---|---|
|grant_type|是|其值应为`authorization_code`。|
|code|是|上一步获得的授权码。|
|client_id|是|客户端的ID。|
|client_secret|是|客户端机密。|
|code_verifier|*|[参见PKCE](../KB/PKCE.md)|

## 相关参考

开发过程中，你可以使用已实现的客户端库，以提高开发效率，降低潜在安全风险。请参阅[OIDC客户端库](../KB/OidcClientLibs.md)。