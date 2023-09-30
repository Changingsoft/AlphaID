import { TokenRenewMode } from './oidc';

export const configurationAlphaID = {
    client_id: '54f9fd3e-04eb-4485-ab29-be82425f9089',
    redirect_uri: window.location.origin + '/authentication/callback',
    silent_redirect_uri: window.location.origin + '/authentication/silent-callback',
    scope: 'openid profile',
    authority: 'https://auth.changingsoft.com/',
    service_worker_relative_url: '/OidcServiceWorker.js',
    service_worker_only: false,
    extras: { acr_values: 'idp:federal.changingsoft.com' },
};
