using IDSubjects.RealName;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaID.EntityFramework;
public class RealNameStore : IRealNameStore
{
    IDSubjectsDbContext db;

    public RealNameStore(IDSubjectsDbContext db)
    {
        this.db = db;
    }

    public RealNameInfo? FindByPersonId(string id)
    {
        return this.db.RealNameInfos.FirstOrDefault(x => x.PersonId == id);
    }
}
