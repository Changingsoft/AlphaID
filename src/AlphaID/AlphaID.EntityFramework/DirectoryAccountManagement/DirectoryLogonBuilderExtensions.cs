using IdSubjects.DirectoryLogon;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AlphaId.EntityFramework.DirectoryAccountManagement;

public static class DirectoryLogonBuilderExtensions
{
    public static DirectoryLogonBuilder AddDefaultStores(this DirectoryLogonBuilder builder)
    {
        builder.AddDirectoryServiceStore<DirectoryServiceDescriptorStore>();
        builder.AddLogonAccountStore<DirectoryAccountStore>();
        return builder;
    }

    public static DirectoryLogonBuilder AddDbContext(this DirectoryLogonBuilder builder,
        Action<DbContextOptionsBuilder> options)
    {
        builder.Services.AddDbContext<DirectoryLogonDbContext>(options);
        return builder;
    }
}