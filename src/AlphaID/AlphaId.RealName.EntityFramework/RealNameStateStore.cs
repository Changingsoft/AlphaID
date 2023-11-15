using IdSubjects.RealName;

namespace AlphaId.RealName.EntityFramework;
internal class RealNameStateStore : IRealNameStateStore
{
    private readonly RealNameDbContext dbContext;

    public RealNameStateStore(RealNameDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public RealNameState? FindById(string id)
    {
        return this.dbContext.RealNameStates.Find(id);
    }
}
