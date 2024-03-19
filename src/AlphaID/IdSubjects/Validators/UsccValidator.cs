using IdSubjects.Subjects;

namespace IdSubjects.Validators;
internal class UsccValidator : OrganizationIdentifierValidator
{
    public override IdOperationResult Validate(OrganizationIdentifier identifier)
    {
        if (identifier.Type != OrganizationIdentifierType.UnifiedSocialCreditCode)
            return IdOperationResult.Success;
        if (!UnifiedSocialCreditCode.TryParse(identifier.Value, out var uscc))
            return IdOperationResult.Failed("Invalid unified social credit code.");

        identifier.Value = uscc.ToString();
        return IdOperationResult.Success;
    }
}
