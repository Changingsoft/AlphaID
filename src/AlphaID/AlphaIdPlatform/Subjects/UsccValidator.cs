using IdSubjects;
using IdSubjects.Subjects;

namespace AlphaIdPlatform.Subjects;

internal class UsccValidator : OrganizationIdentifierValidator
{
    public override IdOperationResult Validate(OrganizationIdentifier identifier)
    {
        if (identifier.Type != OrganizationIdentifierType.UnifiedSocialCreditCode)
            return IdOperationResult.Success;
        if (!UnifiedSocialCreditCode.TryParse(identifier.Value, out UnifiedSocialCreditCode uscc))
            return IdOperationResult.Failed("Invalid unified social credit code.");

        identifier.Value = uscc.ToString();
        return IdOperationResult.Success;
    }
}