using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDSubjects;
public abstract class OrganizationIdentifierValidator
{
    public abstract IdOperationResult Validate(OrganizationIdentifier identifier);
}
