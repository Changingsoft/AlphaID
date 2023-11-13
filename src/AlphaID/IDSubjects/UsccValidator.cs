using IDSubjects.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDSubjects;
internal class UsccValidator:OrganizationIdentifierValidator
{
    public override IdOperationResult Validate(OrganizationIdentifier identifier)
    {
        if(identifier.Type != OrganizationIdentifierType.UnifiedSocialCreditCode)
            return IdOperationResult.Success;
        if(!UnifiedSocialCreditCode.TryParse(identifier.Value, out var uscc))
            return IdOperationResult.Failed("Invalid unified social credit code.");

        identifier.Value = uscc.ToString();
        return IdOperationResult.Success;
    }
}
