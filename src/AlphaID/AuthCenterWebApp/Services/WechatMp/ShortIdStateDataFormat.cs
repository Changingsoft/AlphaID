using Microsoft.AspNetCore.Authentication;

namespace AuthCenterWebApp.Services.WechatMp;

public class ShortIdStateDataFormat : ISecureDataFormat<AuthenticationProperties>
{
    public string Protect(AuthenticationProperties data)
    {
        var stateId = Guid.NewGuid().ToString("N");
        WechatMpOAuthHandler.StateStore.Save(stateId, data);
        return stateId;
    }

    public string Protect(AuthenticationProperties data, string? purpose)
    {
        throw new NotImplementedException();
    }

    public AuthenticationProperties? Unprotect(string? protectedText)
    {
        if (protectedText == null)
            return null;
        return WechatMpOAuthHandler.StateStore.Get(protectedText);
    }

    public AuthenticationProperties? Unprotect(string? protectedText, string? purpose)
    {
        throw new NotImplementedException();
    }
}