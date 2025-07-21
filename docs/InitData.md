# Init Data

Alpha ID 正常工作需要一些内置数据。没有系统工具支持编辑和删除这些数据，但你可以通过访问数据库来编辑他们。

**请注意，内置数据对系统运行至关重要！您必须充分了解该系统并做好备份回退工作后，再编辑这些内置数据。**

## 客户端

|Name|Client ID|Client Type|Workflow|PKCE|Description|
|---|---|---|---|---|---|
|Admin Center||Credentials|Authorization Code|Yes|这是Admin Center运行所必须的客户端信息。|

## Scopes

系统内置的Scope：

|Name|Display Name|Description|Required|Emp|Show in disco|
|---|---|---|---|---|---|
|||||||


## Identity Resources

系统内置的标识资源：

|Name|Display Name|Description|Required|Emphasize|Show in disco|Issue Claims|
|---|---|---|---|---|---|---|
|openid|您的用户标识符|您的Id|是|否|是|sub|
|profile|用户配置文件|您的基本信息，如姓名等|否|是|是|birthdate, family_name, gender, given_name, locale, middle_name, name, nickname, picture, preferred_username, profile, search_hint, updated_at, website, zoneinfo|
|email|您的电子邮件地址|您的电子邮件地址|否|是|是|email, email_verified|
|address|您的邮政地址|您的邮政地址|否|是|是|address|
|phone|您的移动电话号码|您的移动电话号码|否|是|是|phone_number, phone_number_verified|
