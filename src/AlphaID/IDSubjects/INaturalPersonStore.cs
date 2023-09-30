using Microsoft.AspNetCore.Identity;

namespace IDSubjects;

/// <summary>
/// 定义一个具有特定功能特性的自然人Store接口。
/// </summary>
public interface INaturalPersonStore :
    IUserLoginStore<NaturalPerson>,
    IUserClaimStore<NaturalPerson>,
    IUserPasswordStore<NaturalPerson>,
    IUserSecurityStampStore<NaturalPerson>,
    IUserEmailStore<NaturalPerson>,
    IUserLockoutStore<NaturalPerson>,
    IUserPhoneNumberStore<NaturalPerson>,
    IQueryableUserStore<NaturalPerson>,
    IUserTwoFactorStore<NaturalPerson>,
    IUserAuthenticationTokenStore<NaturalPerson>,
    IUserAuthenticatorKeyStore<NaturalPerson>,
    IUserTwoFactorRecoveryCodeStore<NaturalPerson>
{ }
