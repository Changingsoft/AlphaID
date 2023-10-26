# Init Data

Alpha ID 正常工作需要一些内置数据。没有系统工具支持编辑和删除这些数据，但你可以通过访问数据库来编辑他们。

**请注意，内置数据对系统运行至关重要！您必须充分了解该系统并做好备份回退工作后，再编辑这些内置数据。**

## 客户端

|Name|Client ID|Client Type|Workflow|PKCE|Description|
|---|---|---|---|---|---|
|Admin Center||Credentials|Authorization Code|Yes|这是Admin Center运行所必须的客户端信息。|

## Scopes

系统默认内置的Scopes如下：

|Name|Display Name|Description|Required|Emp|Show in disco|
|---|---|---|---|---|---|
|realname|实名信息|客户端可以获取用户的实名信息，如身份证等|False|True|True|
|user_imper||||||
|membership|组织关系和成员身份|客户端可以获取用户的组织关系和成员身份|False|True|True|


## Identity Resources

系统内置2个默认的标识资源：

|Name|Display Name|Description|Required|Emp|Show in disco|Issue Claims|
|---|---|---|---|---|---|---|
|openid||||||sub|
|profile||||||
