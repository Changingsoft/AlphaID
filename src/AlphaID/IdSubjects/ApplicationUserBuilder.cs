using IdSubjects.Subjects;

namespace IdSubjects;

/// <summary>
///     Person builder.
/// </summary>
/// <remarks>
///     Create a builder use user name and person name info.
/// </remarks>
/// <param name="userName"></param>
/// <param name="familyName"></param>
/// <param name="givenName"></param>
/// <param name="middleName"></param>
/// <param name="nickName"></param>
/// <exception cref="ArgumentException"></exception>
public class ApplicationUserBuilder<T>(string userName, string? familyName, string? givenName, string? middleName = null, string? nickName = null)
where T : ApplicationUser, new()
{
    private readonly T _user = new()
    {
        UserName = userName,
        FamilyName = familyName,
        GivenName = givenName,
        MiddleName = middleName,
        NickName = nickName
    };
    
    /// <summary>
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public ApplicationUserBuilder<T> SetUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("User name is blank or empty");
        _user.UserName = userName;
        return this;
    }

    /// <summary>
    ///     Set mobile phone number.
    /// </summary>
    /// <param name="mobilePhoneNumber"></param>
    /// <param name="confirmed"></param>
    /// <returns></returns>
    public ApplicationUserBuilder<T> SetMobile(MobilePhoneNumber? mobilePhoneNumber, bool confirmed = false)
    {
        _user.PhoneNumber = mobilePhoneNumber?.ToString();
        _user.PhoneNumberConfirmed = mobilePhoneNumber.HasValue && confirmed;
        return this;
    }

    /// <summary>
    /// </summary>
    /// <param name="email"></param>
    /// <param name="confirmed"></param>
    public ApplicationUserBuilder<T> SetEmail(string? email, bool confirmed = false)
    {
        _user.Email = email;
        _user.EmailConfirmed = email != null && confirmed;
        return this;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public T Build()
    {
        //Ensure person
        if (string.IsNullOrWhiteSpace(_user.UserName))
            throw new InvalidOperationException("Can not build person because user name is null or blank.");

        return _user;
    }
}