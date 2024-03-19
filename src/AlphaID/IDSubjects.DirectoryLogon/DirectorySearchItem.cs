namespace IdSubjects.DirectoryLogon;

/// <summary>
/// Directory Search Item.
/// </summary>
/// <param name="Name"> Directory entry name. </param>
/// <param name="SamAccountName"> SAM Account Name </param>
/// <param name="UserPrincipalName"> User Principal Name </param>
/// <param name="ObjectGuid"> Object GUID. </param>
/// <param name="Dn"> Distinguished Name. </param>
/// <param name="DisplayName"> Display Name </param>
/// <param name="Mobile"> PhoneNumber phone number with E.164 format. </param>
/// <param name="Company"> Company name. </param>
/// <param name="Department"> Department name. </param>
/// <param name="Title"> Title name. </param>
public record DirectorySearchItem(string Name,
                                  string? SamAccountName,
                                  string? UserPrincipalName,
                                  Guid ObjectGuid,
                                  string Dn,
                                  string? DisplayName,
                                  string? Mobile,
                                  string? Company,
                                  string? Department,
                                  string? Title);
