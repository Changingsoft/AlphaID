using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDSubjects.RealName;
public class RealNameManager
{
    IRealNameStore store;

    public RealNameManager(IRealNameStore store)
    {
        this.store = store;
    }

    public RealNameInfo? GetRealNameInfo(NaturalPerson person)
    {
        return this.store.FindByPersonId(person.Id);
    }
}
