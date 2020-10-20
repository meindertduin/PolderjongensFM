import {VuexOidcClientSettings} from "vuex-oidc";
import {UserManager, WebStorageStateStore} from "oidc-client";

export const oidcSettings = {
    authority: 'https://localhost:5001',
    clientId: 'pjfm_web_client',
    redirectUri: 'https://localhost:5001/oidc-callback',
    responseType: 'code',
    scope: 'openid profile IdentityServerApi Role',
    postLogoutRedirectUri: 'https://localhost:5001',
    userStore: new WebStorageStateStore({ store: window.localStorage }),
}