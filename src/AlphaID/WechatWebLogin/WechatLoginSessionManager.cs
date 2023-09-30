using Newtonsoft.Json;
using System.DirectoryServices;

namespace WechatWebLogin;

/// <summary>
/// 
/// </summary>
public class WechatLoginSessionManager
{
    private readonly IWechatServiceProvider wechatServiceProvider;
    private readonly IWechatLoginSessionStore store;
    private readonly IWechatAppClientStore wechatSPAConfidentialClientStore;
    private readonly OAuth2Service oAuth2Service;
    private readonly IWechatUserIdentifierStore identifierStore;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="wechatServiceProvider"></param>
    /// <param name="store"></param>
    /// <param name="wechatSPAConfidentialClientStore"></param>
    /// <param name="oAuth2Service"></param>
    /// <param name="identifierStore"></param>
    public WechatLoginSessionManager(IWechatServiceProvider wechatServiceProvider,
                                     IWechatLoginSessionStore store,
                                     IWechatAppClientStore wechatSPAConfidentialClientStore,
                                     OAuth2Service oAuth2Service,
                                     IWechatUserIdentifierStore identifierStore)
    {
        this.wechatServiceProvider = wechatServiceProvider;
        this.store = store;
        this.wechatSPAConfidentialClientStore = wechatSPAConfidentialClientStore;
        this.oAuth2Service = oAuth2Service;
        this.identifierStore = identifierStore;
    }

    /// <summary>
    /// 创建一个微信认证会话。
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="clientId"></param>
    /// <param name="resource"></param>
    /// <param name="redirectUri"></param>
    /// <returns></returns>
    public async Task<WechatLoginSession> CreateAsync(string appId, string clientId, string resource, string redirectUri)
    {
        var client = await this.wechatSPAConfidentialClientStore.FindAsync(clientId) ?? throw new InvalidOperationException("无法找到客户端信息。");
        var redirectUriObject = new Uri(redirectUri);
        var uriMatch = false;
        foreach (var uriStr in client.RedirectUriList)
        {
            var uriStrObj = new Uri(uriStr);
            if (uriStrObj.IsBaseOf(redirectUriObject))
                uriMatch = true;
        }

        if (!uriMatch)
            throw new InvalidOperationException("无效的RedirectURI");

        var session = new WechatLoginSession(appId, clientId, client.Secret, resource, redirectUri);
        await this.store.CreateAsync(session);

        return session;
    }

    /// <summary>
    /// 微信登录。
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<WechatLoginSession> WechatLoginAsync(string sessionId, string code)
    {
        var session = await this.FindAsync(sessionId) ?? throw new ArgumentException("会话无效。");

        //获取access_token和openid
        var tokenClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.weixin.qq.com")
        };
        var queryData = new Dictionary<string, string?>
        {
            {"appid", session.WechatAppId },
            {"secret", await this.wechatServiceProvider.GetSecretAsync(session.WechatAppId) },
            {"code", code },
            {"grant_type", "authorization_code" },
        };

        var response = await tokenClient.GetAsync($"sns/oauth2/access_token?{await new FormUrlEncodedContent(queryData).ReadAsStringAsync()}");
        var result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()) as dynamic;

        var accessToken = (string?)result!.access_token;
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new WechatException($"{(int)result.errcode}:{(string?)result.errmsg}");
        }

        session.WechatOAuthToken = accessToken;
        session.OpenId = (string)result.openid;
        session.WechatOAuthTokenExpiresIn = (int)result.expires_in;
        session.WechatOauthTokenExpires = DateTime.Now.AddSeconds((double)result.expires_in);

        await this.UpdateAsync(session);

        session.WechatUser = await this.identifierStore.FindAsync(session.WechatAppId, session.OpenId);

        return session;
    }


    /// <summary>
    /// 绑定用户。
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="personId"></param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
    public async Task BindingPersonAsync(string sessionId, string personId)
    {
        var session = await this.FindAsync(sessionId) ?? throw new ArgumentException("会话无效。");
        var wechatUserId = await this.identifierStore.FindAsync(session.WechatAppId, session.OpenId);
        if (wechatUserId != null)
            throw new InvalidOperationException("相关绑定已存在。");

        //开始创建微信登录专用账号
        var upn = $"{session.WechatAppId}_{session.OpenId}@changingsoft.com";
        var userSecret = "QAZwsx123";

        using var rootEntry = new DirectoryEntry("LDAP://OU=WechatUsers,OU=People,DC=changingsoft,DC=com");
        using var searcher = new DirectorySearcher(rootEntry);

        //检查ou是否存在，如果没有，则创建一个。
        searcher.Filter = $"(ou={session.WechatAppId})";
        searcher.SearchScope = SearchScope.OneLevel;
        DirectoryEntry ouEntry;
        var ouSearchResult = searcher.FindOne();
        if (ouSearchResult == null)
        {
            ouEntry = rootEntry.Children.Add($"OU={session.WechatAppId}", "organizationalUnit");
            ouEntry.CommitChanges();
        }
        else
        {
            ouEntry = ouSearchResult.GetDirectoryEntry();
        }

        //检查ou下是否存在账号。
        searcher.SearchRoot = ouEntry;
        searcher.Filter = $"(name={session.OpenId})";
        var accountSearchResult = searcher.FindOne();
        if (accountSearchResult == null)
        {
            //在这个ou下，创建一个账号。
            var userEntry = ouEntry.Children.Add($"CN={session.OpenId}", "user");
            userEntry.Properties["userPrincipalName"].Value = upn;
            userEntry.Properties["info"].Value = personId;
            userEntry.CommitChanges();
            userEntry.Invoke("SetPassword", userSecret);
            var uac = UF_NORMAL_ACCOUNT + UF_DONT_EXPIRE_PASSWD;
            userEntry.Properties["userAccountControl"].Value = uac;
            userEntry.CommitChanges();
        }
        else
        {
            var currentUserEntry = accountSearchResult.GetDirectoryEntry();
            currentUserEntry.Properties["userPrincipalName"].Value = upn;
            currentUserEntry.Properties["info"].Value = personId;
            currentUserEntry.CommitChanges();
            currentUserEntry.Invoke("SetPassword", userSecret);
            var uac = UF_NORMAL_ACCOUNT + UF_DONT_EXPIRE_PASSWD;
            currentUserEntry.Properties["userAccountControl"].Value = uac;
            currentUserEntry.CommitChanges();
        }



        wechatUserId = new WechatUserIdentifier(session.WechatAppId, session.OpenId, personId)
        {
            UserPrincipalName = upn,
            UserSecret = userSecret,
        };

        await this.identifierStore.CreateAsync(wechatUserId);
    }

    /// <summary>
    /// 构造回调URI
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    public async Task<string> BuildCallBackUriAsync(string sessionId)
    {
        var session = await this.FindAsync(sessionId) ?? throw new ArgumentException("会话无效。");

        //向federal.changingsoft.com发起资源所有者密码凭据授予流（ROPC），拿取访问WebAPI的令牌
        var oAuth2Result = await this.oAuth2Service.GetResourceOwnerPasswordCredentialTokenAsync(session.ClientId, session.ClientSecret, session.WechatUser!.UserPrincipalName, session.WechatUser.UserSecret, session.Resource);

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
    public async Task UpdateAsync(WechatLoginSession session)
    {
        await this.store.UpdateAsync(session);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    public async Task<WechatLoginSession?> FindAsync(string sessionId)
    {
        await this.store.CleanExpiredSessionsAsync();
        return await this.store.FindAsync(sessionId);
    }

    private const int UF_ACCOUNTDISABLE = 0x0002;
    private const int UF_PASSWD_NOTREQD = 0x0020;
    private const int UF_PASSWD_CANT_CHANGE = 0x0040;
    private const int UF_NORMAL_ACCOUNT = 0x0200;
    private const int UF_DONT_EXPIRE_PASSWD = 0x10000;
    private const int UF_SMARTCARD_REQUIRED = 0x40000;
    private const int UF_PASSWORD_EXPIRED = 0x800000;
}
