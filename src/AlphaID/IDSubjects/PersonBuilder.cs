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
    /// <param userName="name"></param>
    /// <exception cref="ArgumentException"></exception>
    public PersonBuilder(string userName)
    {
        this.Person = new NaturalPerson
        {
            UserName = userName
        };
    }

    /// <summary>
    /// Set mobile phone numebr.
    /// </summary>
    /// <param userName="mobilePhoneNumber"></param>
    /// <returns></returns>
    public PersonBuilder SetMobile(MobilePhoneNumber mobilePhoneNumber, bool confirmed = false)
    {
        this.Person.Mobile = mobilePhoneNumber.ToString();
        this.Person.PhoneNumberConfirmed = confirmed;
        return this;
    }

    /// <summary>
    /// Use chinese person userName info.
    /// </summary>
    /// <param userName="name"></param>
    /// <returns></returns>
    public PersonBuilder UseChinesePersonName(ChinesePersonName name)
    {
        this.Person.SetName(name);
        return this;
    }

    /// <summary>
    /// Add bank account to person.
    /// </summary>
    /// <param userName="accountNumber"></param>
    /// <param userName="bankName"></param>
    /// <param userName="accountName"></param>
    /// <returns></returns>
    public PersonBuilder AddBankAccount(string accountNumber, string bankName, string accountName)
    {
        if (string.IsNullOrWhiteSpace(accountName))
        {
            throw new ArgumentException($"“{nameof(accountName)}”不能为 null 或空白。", nameof(accountName));
        }

        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new ArgumentException($"“{nameof(accountNumber)}”不能为 null 或空白。", nameof(accountNumber));
        }

        if (string.IsNullOrWhiteSpace(bankName))
        {
            throw new ArgumentException($"“{nameof(bankName)}”不能为 null 或空白。", nameof(bankName));
        }

        this.Person.BankAccounts.Add(new PersonBankAccount()
        {
            AccountName = accountName.Trim(),
            AccountNumber = accountNumber.Trim(),
            BankName = bankName.Trim(),
        });
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
