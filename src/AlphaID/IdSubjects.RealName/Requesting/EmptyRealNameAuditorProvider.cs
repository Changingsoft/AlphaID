namespace IdSubjects.RealName.Requesting;
internal class EmptyRealNameAuditorProvider : IRealNameRequestAuditorProvider
{
    public IEnumerable<IRealNameRequestAuditor> GetAuditors()
    {
        return Enumerable.Empty<IRealNameRequestAuditor>();
    }
}
