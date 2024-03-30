namespace IdSubjects.WechatWebLogin;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="store"></param>
/// <param name="oAuth2Service"></param>
public class WechatLoginSessionManager(IWechatLoginSessionStore store,
    OAuth2Service oAuth2Service)
{
    /// <summary>
    /// 构造回调URI
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    public async Task<string> BuildCallBackUriAsync(string sessionId)
    {
        var session = await FindAsync(sessionId) ?? throw new ArgumentException("会话无效。");

        //向federal.changingsoft.com发起资源所有者密码凭据授予流（ROPC），拿取访问WebAPI的令牌
        var oAuth2Result = await oAuth2Service.GetResourceOwnerPasswordCredentialTokenAsync(session.ClientId, session.ClientSecret, session.WechatUser!.UserPrincipalName, session.WechatUser.UserSecret, session.Resource);

        var queryData = new Dictionary<string, string?>
        {
            {"access_token", oAuth2Result!.AccessToken },
            {"expires_in", oAuth2Result.ExpiresIn.ToString() },
            {"refresh_token", oAuth2Result.RefreshToken },
            {"refresh_token_expires_in", oAuth2Result.RefreshTokenExpiresIn.ToString() },
            {"wx_access_token", session.WechatOAuthToken },
            {"wx_access_token_expires_in", session.WechatOAuthTokenExpiresIn.ToString() },
            {"openid", session.OpenId },
            {"resource", oAuth2Result.Resource },
        };

        var queryString = await new FormUrlEncodedContent(queryData).ReadAsStringAsync();
        return session.RedirectUri + $"?{queryString}";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public Task UpdateAsync(WechatLoginSession session)
    {
        return store.UpdateAsync(session);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    public async Task<WechatLoginSession?> FindAsync(string sessionId)
    {
        await store.CleanExpiredSessionsAsync();
        return await store.FindAsync(sessionId);
    }

    // ReSharper disable IdentifierTypo
    // ReSharper disable InconsistentNaming
    private const int UF_ACCOUNTDISABLE = 0x0002;
    private const int UF_PASSWD_NOTREQD = 0x0020;
    private const int UF_PASSWD_CANT_CHANGE = 0x0040;
    private const int UF_NORMAL_ACCOUNT = 0x0200;
    private const int UF_DONT_EXPIRE_PASSWD = 0x10000;
    private const int UF_SMARTCARD_REQUIRED = 0x40000;
    private const int UF_PASSWORD_EXPIRED = 0x800000;
    // ReSharper restore InconsistentNaming
    // ReSharper restore IdentifierTypo
}
