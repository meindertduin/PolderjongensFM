import 'core-js/fn/promise'
import { vuexOidcProcessSilentSignInCallback } from 'vuex-oidc'

vuexOidcProcessSilentSignInCallback()
    .then(() => console.log("silent-redirect"));