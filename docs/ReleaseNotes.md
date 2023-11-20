# Alpha ID 发行说明

Alpha ID 尚未正式发布，目前已实现的特性如下。在正式发布时，特性可能会有所调整。

## 特性

### 主体 Subjects

* 自然人、组织以及组织成员身份
* 针对自然人和组织的自助服务
  * 组织可以邀请自然人加入组织
* 组织成员身份的可见性
* 提供搜索组织和个人的 WebAPI
* Support Real-Name validation.

### 身份 Identity

* 完整支持 OAuth/OpenID Connect 有关协议
  * 支持 OIDC 的管理
  * 支持颁发所有 OpenID Connect 预定的声明类型
  * 依赖方可以通过 acr-values 指示 Alpha ID 使用指定的外部 IdP

### 账户管理 Account Management

### 验证 Authentication

* 使用密码本地登录
  * 可以移除密码以禁用本地登录
* 支持外部登录
* 支持多因子身份验证
  * 支持通过TOPT验证器实施第二因子身份验证
* 对登录失败计数并实施锁定
* 支持密码有效期和强制更改
* Supports binding an exists account after external login.

### 安全性 Security

* 注册阶段实施 CHAPCHA 检查
* 支持安全审计日志（参见[安全审计](SecurityAuditing.md)）

### 外观 Appearances

* 多语言支持，(en-US and zh-CN).
* 友好 URL (in Auth Center Web Application).

### 系统 System

* 对等多实例结构（启用共享服务端会话时），意味着对NLB友好。

-------

## 将来考虑实现的功能特性

（不分先后顺序）

* 提供对下游LDAP目录的管理能力，如 Microsoft Active Directory 或类似 LDAP 产品。
* 拟计划提供简单 RADIUS 协议
* 拟实现每客户端 Pairwise sub 声明值，以提高个人隐私跟踪难度