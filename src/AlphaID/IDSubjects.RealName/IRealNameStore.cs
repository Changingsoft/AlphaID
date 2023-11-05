namespace IDSubjects.RealName;

public interface IRealNameStore
{
    RealNameInfo? FindByPersonId(string id);
}