using Microsoft.EntityFrameworkCore;
using AlphaIdPlatform.JoinOrgRequesting;

namespace AlphaId.EntityFramework;
public class AlphaIdDbContext(DbContextOptions<AlphaIdDbContext> options) : DbContext(options)
{
    public virtual DbSet<JoinOrganizationRequest> JoinOrganizationRequests { get; protected set; } = null!;
}
