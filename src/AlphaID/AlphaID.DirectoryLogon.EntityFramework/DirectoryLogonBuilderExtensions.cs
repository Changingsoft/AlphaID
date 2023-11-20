using IdSubjects.DirectoryLogon;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AlphaId.DirectoryLogon.EntityFramework;
public static class DirectoryLogonBuilderExtensions
{
    public static DirectoryLogonBuilder AddDefaultStores(this DirectoryLogonBuilder builder)
    {
        builder.AddDirectoryServiceStore<DirectoryServiceStore>();
        builder.AddLogonAccountStore<LogonAccountStore>();
        return builder;
    }

    public static DirectoryLogonBuilder AddDbContext(this DirectoryLogonBuilder builder, Action<DbContextOptionsBuilder> options)
    {
        builder.Services.AddDbContext<DirectoryLogonDbContext>(options);
        return builder;
    }
}
