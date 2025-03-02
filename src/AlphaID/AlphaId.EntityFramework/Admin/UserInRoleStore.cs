﻿using AlphaIdPlatform.Admin;

namespace AlphaId.EntityFramework.Admin;

public class UserInRoleStore(OperationalDbContext dbContext) : IUserInRoleStore
{
    public IQueryable<UserInRole> UserInRoles => dbContext.UserInRoles;

    public async Task CreateAsync(UserInRole item)
    {
        dbContext.UserInRoles.Add(item);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(UserInRole item)
    {
        dbContext.UserInRoles.Remove(item);
        await dbContext.SaveChangesAsync();
    }
}