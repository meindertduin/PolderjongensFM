import Vue from 'vue'
import Vuex, {ActionTree, GetterTree, MutationTree} from 'vuex'
import {vuexOidcCreateStoreModule} from "vuex-oidc";
import {oidcSettings} from "@/config/oidc";
import ProfileModule from "@/store/profileModule";
import UserSettingsModule from "@/store/userSettingsModule";
import ModModule from "@/store/modModule";
import PlaybackModule from "@/store/playbackModule";
import AlertModule from "@/store/alertModule";

Vue.use(Vuex)

export default new Vuex.Store({
  modules: {
    oidcStore: vuexOidcCreateStoreModule(
        oidcSettings, 
        {namespaced: true, isAuthenticatedBy: "access_token"},
        {
          userLoaded: (user) => console.log("OIDC user is loaded"),
          userUnloaded: () => console.log('OIDC user is unloaded'),
          accessTokenExpiring: () => console.log('Access token will expire'),
          accessTokenExpired: () => console.log('Access token did expire'),
          silentRenewError: () => console.log('OIDC user is unloaded'),
          userSignedOut: () => console.log('OIDC user is signed out'),
          oidcError: (payload) => console.log('OIDC error', payload),
          automaticSilentRenewError: (payload) => console.log('OIDC automaticSilentRenewError', payload)
        }), 
      profileModule: ProfileModule,
      userSettingsModule: UserSettingsModule,
      modModule: ModModule,
      playbackModule: PlaybackModule,
      alertModule: AlertModule
  }
})
