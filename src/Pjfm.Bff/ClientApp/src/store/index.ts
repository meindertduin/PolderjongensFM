import Vue from 'vue'
import Vuex, {ActionTree, GetterTree, MutationTree} from 'vuex'
import {vuexOidcCreateStoreModule} from "vuex-oidc";
import {oidcSettings} from "@/config/oidc";
import ProfileModule from "@/store/profileModule";
import UserSettingsModule from "@/store/userSettingsModule";
import ModModule from "@/store/modModule";
import PlaybackModule from "@/store/playbackModule";
import AlertModule from "@/store/alertModule";
import UserModule from "@/store/userModule";

Vue.use(Vuex)

export default new Vuex.Store({
  modules: {
      profileModule: ProfileModule,
      userSettingsModule: UserSettingsModule,
      modModule: ModModule,
      playbackModule: PlaybackModule,
      alertModule: AlertModule,
      userModule: UserModule,
  }
})
