# 基于Vue3实现Oidc登录

[![Node.js version](https://img.shields.io/badge/Node.js-v18.12.1-green)](https://nodejs.org/en/)
[![npm version](https://img.shields.io/badge/npm-v8.19.2-blue)](https://www.npmjs.com/)
[![vue version](https://img.shields.io/badge/vue-v3.2.47-success)](https://www.npmjs.com/package/vue)
[![oidc version](https://img.shields.io/badge/oidcclient-v1.11.5-success)](https://www.npmjs.com/package/oidc-client)
[![vite version](https://img.shields.io/badge/vite-v4.3.0-success)](https://www.npmjs.com/package/vite)


## 第一步：初始化oidc用户管理实例
```js
const oidcClient = new Oidc.UserManager({
  authority: "https://auth.changingsoft.com",                // 发现文档地址，只需填写域名，其他路径会自动拼接
  client_id: '[此处填写分配到的client_id]',                 
  redirect_uri: `${ location.origin }/callback`,          // 登录成功后的回调地址
  silent_redirect_uri: `${ location.origin }/callback`,   // 静默刷新token的回调地址
  response_type: 'code',
  scope: 'openid profile user_impersonation',
  accessTokenExpiringNotificationTime: 60 * 5,            // token过期前5分钟刷新token
  automaticSilentRenew: true,                             // 自动刷新token
  revokeAccessTokenOnSignout: true,                       // 退出时清除token
  loadUserInfo: true                                      // 是否需要加载用户信息
})
```

## 第二步：登录
```js
oidcClient.signinRedirect()
```

## 第三步：获取认证信息
在回调页面加载时调用`oidcClient.signinRedirectCallback()`获取认证信息，然后跳转到首页
```js
oidcClient.signinRedirectCallback().then(() => {
  location.href = '/'
})
```
页面刷新后可通过`oidcClient.getUser()`获取认证信息