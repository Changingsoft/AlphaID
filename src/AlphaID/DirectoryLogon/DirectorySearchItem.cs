namespace DirectoryLogon;

/// <summary>
/// Directory Search Item.
/// </summary>
/// <param name="Name"> Directory entry name. </param>
/// <param name="SAMAccountName"> SAM Account Name </param>
/// <param name="UserPrincipalName"> User Principal Name </param>
/// <param name="ObjectGUID"> Object GUID. </param>
/// <param name="DN"> Distinguished Name. </param>
/// <param name="DisplayName"> Display Name </param>
/// <param name="Mobile"> Mobile phone number with E.164 format. </param>
/// <param name="Company"> Company name. </param>
/// <param name="Department"> Department name. </param>
/// <param name="Title"> Title name. </param>
public record DirectorySearchItem(string Name,
                                  string? SAMAccountName,
                                  string UserPrincipalName,
                                  Guid ObjectGUID,
                                  string DN,
                                  string? DisplayName,
                                  string? Mobile,
                                  string? Company,
                                  string? Department,
                                  string? Title);
