namespace IdSubjects.DirectoryLogon;

/// <summary>
/// LDAP类型。
/// </summary>
public enum LdapType
{
    /// <summary>
    /// Active Directory Domain Services.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    ADDS,

    /// <summary>
    /// Active Directory Lightweight Directory Services.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    ADLDS
}