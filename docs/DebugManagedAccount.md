# 调试托管账户

托管账户主要是指在目录服务中管理的用户账户。AlphaID支持对托管账户的关联管理，例如同步修改登录密码等。

## 调试目录服务账户

我们使用ADLDS来模拟目录服务，要调试目录服务账户，你需要先准备ADLDS相关服务。

- 目录服务相关资料在`/eng/ADLDS`文件夹内。
- 作为调试，我们仅对示例用户`liubei`添加了关联的目录账户。

以下是准备ADLDS的步骤。

### 1、添加ADLDS功能。

请从控制面板--程序--打开关闭Windows功能，添加Active Directory轻型目录服务。

![安装Active Directory轻型目录服务](/docs/Toturials/Install-AD-LDS.jpg)

### 2、从开始菜单找到打开“Active Directory轻型目录服务安装向导”

实例名和描述可以随意填写，实例名将用作Windows服务的名称。如下图。
![实例名称和描述](/docs/Toturials/Add-ADLDS-instance-wizard1.png)

请确保端口号为LDAP常用端口，如下图。

> 如果你计划使用其他端口号，请务必在对应sql脚本中修改Server字段，加上端口号。

![端口号](/docs/Toturials/Add-ADLDS-instance-wizard2.png)

创建一个名为`DC=changingsoft,DC=com`的命名分区

![命名分区](/docs/Toturials/Add-ADLDS-instance-wizard3.png)

添加用户类架构

![用户类架构](/docs/Toturials/Add-ADLDS-instance-wizard4.png)

### 3、从恢复实例状态

请从LDF文件恢复用户条目。它位于`/eng/ADLDS/users.ldf`。

``` powershell
ldifde -i -f "<LDF_FILE>" -s localhost -v
```

这将把相关调试用户添加到目录中。要浏览目录内容，可以使用ADSI编辑器或其他目录浏览器程序。


### 4、在调试数据库中执行sql脚本

在调试用数据库中，执行`/eng/ADLDS/AddDirectoryServiceAccounts.sql`脚本。

现在，你就可以开始进行调试，观察和验证托管账户管理的设计行为。

### 5、导出和备份

若开发过程中涉及目录数据变动需要导出，可使用以下命令

``` powershell
ldifde -f "<OUTPUT_FILE>" -s localhost -d DC=changingsoft,DC=com -r "(objectClass=user)" -l dn,objectClass,cn,distinguishedName,instanceType,name,objectCategory,msDS-UserAccountDisabled,sn,givenName,displayName,mobile -v
```