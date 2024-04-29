using IdSubjects.SecurityAuditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AlphaId.EntityFramework.SecurityAuditing;

public static class AuditLogBuilderExtensions
{
    public static AuditLogBuilder AddDefaultStore(this AuditLogBuilder builder)
    {
        builder.AddLogStore<SecurityLogStore>();
        return builder;
    }

    public static AuditLogBuilder AddDbContext(this AuditLogBuilder builder, Action<DbContextOptionsBuilder> options)
    {
        builder.Services.AddDbContext<LoggingDbContext>(options);
        return builder;
    }
}