using IDSubjects.ChineseName;
using IDSubjects.Subjects;

namespace IDSubjects;

/// <summary>
/// Person builder.
/// </summary>
public class PersonBuilder
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    /// <exception cref="ArgumentException"></exception>
    public PersonBuilder(string userName)
    {
        this.Person = new NaturalPerson(userName);
    }

    /// <summary>
    /// Set mobile phone number.
    /// </summary>
    /// <param userName="mobilePhoneNumber"></param>
    /// <returns></returns>
    public PersonBuilder SetMobile(MobilePhoneNumber mobilePhoneNumber, bool confirmed = false)
    {
        this.Person.PhoneNumber = mobilePhoneNumber.ToString();
        this.Person.PhoneNumberConfirmed = confirmed;
        return this;
    }

    /// <summary>
    /// Use chinese person userName info.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public PersonBuilder UseChinesePersonName(ChinesePersonName name)
    {
        this.Person.SetName(name);
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    public void SetEmail(string email)
    {
        this.Person.Email = email;
    }

    /// <summary>
    /// Gets person.
    /// </summary>
    public NaturalPerson Person { get; }
}
