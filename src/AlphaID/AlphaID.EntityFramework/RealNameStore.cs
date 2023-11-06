using IDSubjects.RealName;

namespace AlphaID.EntityFramework;
public class RealNameStore : IRealNameStore
{
    private readonly IDSubjectsDbContext db;

    public RealNameStore(IDSubjectsDbContext db)
    {
        this.db = db;
    }

    public RealNameInfo? FindByPersonId(string id)
    {
        return this.db.RealNameInfos.FirstOrDefault(x => x.PersonId == id);
    }
}
