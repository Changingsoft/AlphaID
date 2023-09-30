## Getting Started

```sh
npm install @axa-fr/react-oidc --save

# If you have a "public" folder, the 2 files will be created :
# ./public/OidcServiceWorker.js <-- will be updated at each "npm install"
# ./public/OidcTrustedDomains.js <-- won't be updated if already exist
```

If you need a very secure mode where refresh_token and access_token will be hide behind a service worker that will proxify requests.
The only file you should edit is "OidcTrustedDomains.js".

```javascript
// OidcTrustedDomains.js

// Add bellow trusted domains, access tokens will automatically injected to be send to
// trusted domain can also be a path like https://www.myapi.com/users,
// then all subroute like https://www.myapi.com/useers/1 will be authorized to send access_token to.

// Domains used by OIDC server must be also declared here
const trustedDomains = {
  default: ["https://demo.duendesoftware.com", "https://www.myapi.com/users"],
};
```
