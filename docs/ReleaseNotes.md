# Alpha ID Release Notes

## Features

### Subjects

* Natural person and organization membership.
* Self-servicing for person and organization.
  * Invite person join in organization
* Membership visibility.
* Provided WebAPI for Searching person and organization.
* Support Real-Name validation.

### Identity

* Fully supports OpenID Connect protocol spec 1.0
* Supports issue all claim types that defined from OpenID Connect spec

### Account Management

* Manage downstream LDAP (often as Active directory) infrastructure.

### Authentication

* Local login with Password
  * Can be disabled when password not exists
* Supports external logins
  * Supports specify IdP for login from Relying party.
* Supports 2FA, via TOPT authenticator app.
* Lockout when sign in check failed.
* Supports force user must change password.
* Supports binding an exists account after external login.

### Security

* Supports CHAPCHA check when signing up.

### Appearances

* Multi language support (en-US and zh-CN).
* Frendly URL (in Auth Center Web Application).

### System

* Supports Peer instances (when enable shared server side session feature).
  It means you can use NLB easily.