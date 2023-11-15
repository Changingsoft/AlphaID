using Microsoft.AspNetCore.Identity;

namespace IdSubjects;

/// <summary>
/// 一个关于NaturalPerson的锚点接口以便于实现NaturalPersonStore.
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
