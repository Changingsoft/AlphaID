using IdSubjects.Subjects;

namespace IdSubjects;

/// <summary>
/// Person builder.
/// </summary>
/// <remarks>
/// Create a builder use user name and person name info.
/// </remarks>
/// <param name="userName"></param>
/// <param name="personName"></param>
/// <exception cref="ArgumentException"></exception>
public class PersonBuilder(string userName, PersonNameInfo personName)
{
    private readonly NaturalPerson _person = new(userName, personName);

    /// <summary>
    /// Initialize a person builder.
    /// </summary>
    public PersonBuilder()
        : this(string.Empty, string.Empty)
    { }

    /// <summary>
    /// Create a builder use user name and full name
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="fullName"></param>
    public PersonBuilder(string userName, string fullName)
        : this(userName, new PersonNameInfo(fullName))
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public PersonBuilder SetUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("User name is blank or empty");
        _person.UserName = userName;
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="personName"></param>
    /// <returns></returns>
    public PersonBuilder SetPersonName(PersonNameInfo personName)
    {
        _person.PersonName = personName;
        return this;
    }

    /// <summary>
    /// Set mobile phone number.
    /// </summary>
    /// <param name="mobilePhoneNumber"></param>
    /// <param name="confirmed"></param>
    /// <returns></returns>
    public PersonBuilder SetMobile(MobilePhoneNumber? mobilePhoneNumber, bool confirmed = false)
    {
        _person.PhoneNumber = mobilePhoneNumber?.ToString();
        _person.PhoneNumberConfirmed = mobilePhoneNumber.HasValue && confirmed;
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <param name="confirmed"></param>
    public PersonBuilder SetEmail(string? email, bool confirmed = false)
    {
        _person.Email = email;
        _person.EmailConfirmed = email != null && confirmed;
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public NaturalPerson Build()
    {
        //Ensure person
        if (string.IsNullOrWhiteSpace(_person.UserName))
            throw new InvalidOperationException("Can not build person because user name is null or blank.");
        if (string.IsNullOrWhiteSpace(_person.PersonName.FullName))
            throw new InvalidOperationException("Can not build person because full name of person name is null or blank.");

        return _person;
    }
}
