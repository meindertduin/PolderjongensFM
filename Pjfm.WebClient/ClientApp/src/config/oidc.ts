import {VuexOidcClientSettings} from "vuex-oidc";

export const oidcSettings : VuexOidcClientSettings = {
    authority: 'https://localhost:5001',
    clientId: 'pjfm_web_client',
    redirect_uri: 'https://localhost:5001/oidc-callback',
    responseType: 'id_token token',
    scope: 'openid profile',
}