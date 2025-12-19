using AlphaId.EntityFramework;
using AlphaId.EntityFramework.Admin;
using AlphaId.EntityFramework.DirectoryAccountManagement;
using AlphaId.EntityFramework.IdSubjects;
using AlphaId.EntityFramework.RealName;
using AlphaId.EntityFramework.SecurityAuditing;
using AlphaIdPlatform.Admin;
using AlphaIdPlatform.DependencyInjection;
using AlphaIdPlatform.JoinOrgRequesting;
using Microsoft.EntityFrameworkCore;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class AlphaIdPlatformBuilderExtensions
{
    public static AlphaIdPlatformBuilder AddEntityFramework(this AlphaIdPlatformBuilder builder, Action<DbContextOptionsBuilder> options)
    {
        builder.IdSubjects.AddDefaultStores().AddDbContext(options);
        builder.AuditLog.AddDefaultStore().AddDbContext(options);
        builder.DirectoryLogon.AddDefaultStores().AddDbContext(options);
        builder.RealName.AddDefaultStores().AddDbContext(options);

        //Admin
        builder.Services.AddScoped<IUserInRoleStore, UserInRoleStore>();
        builder.Services.AddDbContext<OperationalDbContext>(options);

        builder.Organizational.AddOrganizationStore<OrganizationStore>();
        builder.AddJoinOrganizationInvitationStore<JoinOrganizationInvitationStore>();

        builder.Services.AddScoped<IJoinOrganizationRequestStore, JoinOrganizationRequestStore>();

        builder.Services.AddDbContext<AlphaIdDbContext>(options);
        return builder;
    }
}
