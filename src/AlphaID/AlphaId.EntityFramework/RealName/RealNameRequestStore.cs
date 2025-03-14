﻿using IdSubjects;
using IdSubjects.RealName.Requesting;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework.RealName;

internal class RealNameRequestStore(RealNameDbContext dbContext) : IRealNameRequestStore
{
    public IQueryable<RealNameRequest> Requests => dbContext.RealNameRequests.AsNoTracking();

    public async Task<RealNameRequest?> FindByIdAsync(int id)
    {
        return await dbContext.RealNameRequests.FindAsync(id);
    }

    public async Task<IdOperationResult> CreateAsync(RealNameRequest request)
    {
        dbContext.RealNameRequests.Add(request);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }

    public async Task<IdOperationResult> UpdateAsync(RealNameRequest request)
    {
        dbContext.RealNameRequests.Update(request);
        await dbContext.SaveChangesAsync();
        return IdOperationResult.Success;
    }
}