using AdminWebApp.Domain.Security;

namespace AdminWebApp.Infrastructure.DataStores;
public class UserInRoleStore : IUserInRoleStore
{
    private readonly OperationalDbContext dbContext;

    public UserInRoleStore(OperationalDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IQueryable<UserInRole> UserInRoles => this.dbContext.UserInRoles;

    public async Task CreateAsync(UserInRole item)
    {
        this.dbContext.UserInRoles.Add(item);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(UserInRole item)
    {
        this.dbContext.UserInRoles.Remove(item);
        await this.dbContext.SaveChangesAsync();
    }
}
