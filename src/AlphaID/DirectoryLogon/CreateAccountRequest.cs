namespace DirectoryLogon;

/// <summary>
/// Request for create account
/// </summary>
public class CreateAccountRequest
{
    /// <summary>
    /// Account Name
    /// </summary>
    public string AccountName { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public string SAMAccountName { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public string UpnLeftPart { get; set; } = default!;

    /// <summary>
    /// InitPassword.
    /// </summary>
    public string InitPassword { get; set; } = default!;

    /// <summary>
    /// Directory Service Id.
    /// </summary>
    public int ServiceId { get; set; }

    /// <summary>
    /// DisplayName
    /// </summary>
    public string DisplayName { get; set; } = default!;

    /// <summary>
    /// E164 Mobile.
    /// </summary>
    public string? E164Mobile { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Surname { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? GivenName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Phonetic Surname
    /// </summary>
    public string? PinyinSurname { get; set; }

    /// <summary>
    /// Phonetic Given Name.
    /// </summary>
    public string? PinyinGivenName { get; set; }
}