using IDSubjects;
using IDSubjects.RealName;
using IDSubjects.Subjects;

namespace AlphaIDPlatform.Business;

/// <summary>
/// Person Registration Builder.
/// </summary>
public class PersonRegistrationBuilder
{
    private readonly ChinesePersonNameFactory processor;
    private readonly PersonRegistration registration;

    /// <summary>
    /// Gets a value indicate that create login account after registration completed.
    /// </summary>
    public bool LoginAccountEnabled { get; private set; }

    /// <summary>
    /// Initialize builder.
    /// </summary>
    /// <param name="displayName"></param>
    /// <param name="mobile"></param>
    /// <param name="sex"></param>
    public PersonRegistrationBuilder(string displayName, MobilePhoneNumber mobile, Sex? sex = null)
    {
        this.processor = new ChinesePersonNameFactory();
        this.registration = new PersonRegistration(displayName, mobile, sex);
    }

    /// <summary>
    /// Apply chinese ID card to registration.
    /// </summary>
    /// <param name="chineseIDCard"></param>
    /// <returns></returns>
    public PersonRegistrationBuilder ApplyChineseIDCard(ChineseIDCardInfo chineseIDCard)
    {
        if (chineseIDCard is null)
        {
            throw new ArgumentNullException(nameof(chineseIDCard));
        }

        this.registration.ChineseIDCard = chineseIDCard;

        var cpersonName = this.processor.Create(chineseIDCard.Name);

        this.registration.DisplayName = chineseIDCard.Name;
        this.registration.Surname = cpersonName.Surname;
        this.registration.GivenName = cpersonName.GivenName;
        this.registration.PhoneticDisplayName = $"{cpersonName.PhoneticSurname} {cpersonName.PhoneticGivenName}".Trim();
        this.registration.Sex = chineseIDCard.Sex;

        return this;
    }

    /// <summary>
    /// Enable login account.
    /// </summary>
    /// <returns></returns>
    public PersonRegistrationBuilder EnableLoginAccount()
    {
        this.LoginAccountEnabled = true;
        return this;
    }

    /// <summary>
    /// Enable mail service.
    /// </summary>
    /// <returns></returns>
    public PersonRegistrationBuilder EnableMailBox()
    {
        this.registration.EnableMailService = true;
        return this;
    }

    /// <summary>
    /// make person join in a organization.
    /// </summary>
    /// <param name="organization"></param>
    /// <param name="department"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public PersonRegistrationBuilder JoinOrganization(GenericOrganization organization, string department, string title)
    {
        this.registration.OrganizationMembers.Add(new OrganizationMemberDescriptor(organization.Id.ToString().ToUpper(), department, title));
        return this;
    }

    /// <summary>
    /// Build registration.
    /// </summary>
    /// <returns></returns>
    public PersonRegistration Build()
    {
        if (this.LoginAccountEnabled)
        {
            if (this.registration.ChineseIDCard == null)
                throw new InvalidOperationException("未登记身份证时，不能创建登录账号。");

            var cardNumber = this.registration.ChineseIDCard.CardNumber;
            var idCardLast4 = cardNumber[^4..].ToLower();
            var mobileLast4 = this.registration.Mobile?.ToString()[^4..];
            var accountPinyin = this.registration.PhoneticDisplayName?.ToLower();
            this.registration.LoginAccount = new LoginAccountDescriptor
            {
                PrimaryAccountName = accountPinyin + idCardLast4, //姓名全拼+身份证后4位
                SecandaryAccountName = accountPinyin + mobileLast4 //姓名全拼+移动电话号码后4位。
            };
        }

        return this.registration.EnableMailService && !this.LoginAccountEnabled
            ? throw new InvalidOperationException("未创建登录账号时，不能启用邮件服务。")
            : this.registration;
    }
}
