# Alpha ID

## 1 概述

Alpha ID 是人员和组织的标识基础设施。它提供自然人、组织及它们之间关系的管理。

Alpha ID 是一个标识提供者（Identity Provider），提供自然人身份认证（登录），并向依赖方应用提供用户标识服务。简化组织内各类系统的用户管理和验证活动，可轻松实现单点登录（Single Sign on, SSO）场景应用。

Alpha ID 可以与其他外部标识基础设施建立外部登录验证，它遵循OAuth2.0/OpenID Connect等规范支持单点登录解决方案，并提供API服务接口支持标识信息的检索、利用。

详细信息，请参阅[发行说明](docs/ReleaseNotes.md)。

## 开始

Alpha ID 由一个身份验证应用程序、标识管理应用程序和一个WebAPI接口程序组成。身份验证系统是 Alpha ID 的核心，承担IdP的职责并处理用户登录、应用授权等业务；标识管理系统用来全面管理Alpha ID中存储的对象，包括自然人、组织、客户端等。WebAPI是一个接口服务，可向其他业务系统提供组织或个人查找检索等业务能力。

## 开发

有关开发活动的详细信息，请参阅[开发指南](docs/Development.md)。

## 部署

有关部署的详细信息，请参阅[部署指南](docs/Deployment.md)。

## Contribution



## Security issues

有关安全事件和详细信息，请参阅[安全说明](SECURITY.md)。

