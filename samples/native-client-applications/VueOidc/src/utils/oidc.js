/**
 * Author: ZongAn
 * Date: 2023/4/21
 * Description:
 */
import Oidc from 'oidc-client';

const oidcClient = new Oidc.UserManager({
  authority: "https://auth.changingsoft.com",
  client_id: '[OIDC_CLIENT_ID]',
  redirect_uri: `${ location.origin }/callback`,
  silent_redirect_uri: `${ location.origin }/callback`,
  response_type: 'code',
  scope: 'openid profile user_impersonation',
  accessTokenExpiringNotificationTime: 60 * 5,
  automaticSilentRenew: true,
  revokeAccessTokenOnSignout: true,
  loadUserInfo: true
})

export default oidcClient