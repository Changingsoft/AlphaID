using System.DirectoryServices.AccountManagement;

namespace IdSubjects.DirectoryLogon;
internal class PrincipalContextHelper
{
    public static PrincipalContext GetRootContext(DirectoryService directoryService)
    {
        ArgumentNullException.ThrowIfNull(directoryService);

        ContextType contextType = directoryService.Type switch
        {
            LdapType.ADDS => ContextType.Domain,
            LdapType.ADLDS => ContextType.ApplicationDirectory,
            _ => throw new NotSupportedException("不支持的LDAP类型。")
        };
        ContextOptions contextOption = ContextOptions.Negotiate | ContextOptions.Signing | ContextOptions.Sealing; //执行修改密码必须的标记。
        PrincipalContext ctx = new(contextType, directoryService.ServerAddress, directoryService.RootDn,
            contextOption, directoryService.UserName, directoryService.Password);
        return ctx;
    }

    public static PrincipalContext GetUserContainerContext(DirectoryService directoryService)
    {
        ContextType contextType = directoryService.Type switch
        {
            LdapType.ADDS => ContextType.Domain,
            LdapType.ADLDS => ContextType.ApplicationDirectory,
            _ => throw new NotSupportedException("不支持的LDAP类型。")
        };
        ContextOptions
            contextOption = ContextOptions.Negotiate | ContextOptions.Signing | ContextOptions.Sealing; //执行修改密码必须的标记。
        PrincipalContext ctx = new(contextType, directoryService.ServerAddress, directoryService.DefaultUserAccountContainer,
            contextOption | ContextOptions.Sealing, directoryService.UserName, directoryService.Password);
        return ctx;
    }

}
