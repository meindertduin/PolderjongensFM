import {VuexOidcClientSettings} from "vuex-oidc";

export const oidcSettings : VuexOidcClientSettings = {
    authority: 'https://localhost:5001',
    clientId: 'pjfm_web_client',
    redirectUri: 'https://localhost:8080/oidc-callback',
    responseType: 'code',
    scope: 'openid profile IdentityServerApi Role',
    postLogoutRedirectUri: 'https://localhost:8080',
    silentRedirectUri: "https://localhost:8080/oidc-client-silent-renew.html",
    automaticSilentRenew: true,
}