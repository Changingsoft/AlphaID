using IdSubjects.Subjects;

namespace IdSubjects;

/// <summary>
///     Person builder.
/// </summary>
/// <remarks>
///     Create a builder use user name and person name info.
/// </remarks>
/// <param name="userName"></param>
/// <param name="personName"></param>
/// <exception cref="ArgumentException"></exception>
public class ApplicationUserBuilder(string userName, PersonNameInfo personName)
{
    private readonly ApplicationUser _user = new(userName, personName);

    /// <summary>
    ///     Initialize a person builder.
    /// </summary>
    public ApplicationUserBuilder()
        : this(string.Empty, string.Empty)
    {
    }

    /// <summary>
    ///     Create a builder use user name and full name
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="fullName"></param>
    public ApplicationUserBuilder(string userName, string fullName)
        : this(userName, new PersonNameInfo(fullName))
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public ApplicationUserBuilder SetUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("User name is blank or empty");
        _user.UserName = userName;
        return this;
    }

    /// <summary>
    /// </summary>
    /// <param name="personName"></param>
    /// <returns></returns>
    public ApplicationUserBuilder SetPersonName(PersonNameInfo personName)
    {
        _user.PersonName = personName;
        return this;
    }

    /// <summary>
    ///     Set mobile phone number.
    /// </summary>
    /// <param name="mobilePhoneNumber"></param>
    /// <param name="confirmed"></param>
    /// <returns></returns>
    public ApplicationUserBuilder SetMobile(MobilePhoneNumber? mobilePhoneNumber, bool confirmed = false)
    {
        _user.PhoneNumber = mobilePhoneNumber?.ToString();
        _user.PhoneNumberConfirmed = mobilePhoneNumber.HasValue && confirmed;
        return this;
    }

    /// <summary>
    /// </summary>
    /// <param name="email"></param>
    /// <param name="confirmed"></param>
    public ApplicationUserBuilder SetEmail(string? email, bool confirmed = false)
    {
        _user.Email = email;
        _user.EmailConfirmed = email != null && confirmed;
        return this;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ApplicationUser Build()
    {
        //Ensure person
        if (string.IsNullOrWhiteSpace(_user.UserName))
            throw new InvalidOperationException("Can not build person because user name is null or blank.");
        if (string.IsNullOrWhiteSpace(_user.PersonName.FullName))
            throw new InvalidOperationException(
                "Can not build person because full name of person name is null or blank.");

        return _user;
    }
}