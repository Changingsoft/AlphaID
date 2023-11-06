namespace IDSubjects.RealName;
public class RealNameManager
{
    private readonly IRealNameStore store;

    public RealNameManager(IRealNameStore store)
    {
        this.store = store;
    }

    public RealNameInfo? GetRealNameInfo(NaturalPerson person)
    {
        return this.store.FindByPersonId(person.Id);
    }
}
