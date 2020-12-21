<template>
  <v-app>
    <AppSideBar />
    <AppBar />
    <v-main>
      <v-container fluid>
        <router-view></router-view>
      </v-container>
    </v-main>
    <AppBottomBar />
    <ModServerMessageHandler v-if="isMod" />
  </v-app>
  
</template>

<script lang="ts">
import Vue from 'vue';
import Component from "vue-class-component";
import DisplaySettingsItemGroup from "@/components/CommonComponents/DisplaySettingsItemGroup.vue";
import {userPlaybackInfo, userPlaybackSettings, userSettings} from "@/common/types";
import {Watch} from "vue-property-decorator";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import ModServerMessageHandler from "@/components/ModComponents/ModServerMessageHandler.vue";
import AppSideBar from "@/components/CommonComponents/AppSideBar.vue";
import AppBar from "@/components/CommonComponents/AppBar.vue";
import AppBottomBar from "@/components/CommonComponents/AppBottomBar.vue";
import {vuexOidcUtils} from "vuex-oidc";
import parseJwt = vuexOidcUtils.parseJwt;

@Component({
  name: 'App',
  components: {
    DisplaySettingsItemGroup,
    AppBottomBar,
    ModServerMessageHandler,
    AppSideBar,
    AppBar,
  }
})

export default class App extends Vue{
  
  created(){
    this.setUserPreferences();
    this.setRadioConnection();
  }

  get isMod(){
    return this.$store.getters['profileModule/isMod'];
  }
  
  private async setRadioConnection():void {
    let radioConnection: HubConnection | null = null;

    radioConnection = new HubConnectionBuilder()
        .withUrl("https://localhost:5001/radio")
        .build();

    radioConnection.start()
        .then(() => console.log("radio connection started"));

    radioConnection.on("ReceivePlaybackInfo", (playbackInfo: userPlaybackInfo) => {
        console.log(playbackInfo)
        this.$store.commit('playbackModule/SET_PLAYBACK_INFO', playbackInfo);
    });

    radioConnection.on("IsConnected", (connected:boolean) => {
        this.$store.commit('playbackModule/SET_CONNECTED_STATUS', connected);
    });
    
    radioConnection.on("SubscribeTime", ((minutes: number) => {
      this.$store.commit('playbackModule/SET_SUBSCRIBE_TIME', minutes);  
    }))
    
    radioConnection.on("ReceivePlayingStatus", (isPlaying:boolean) => {
      this.$store.commit("playbackModule/SET_PLAYBACK_PLAYING_STATUS", isPlaying);
    });
    
    radioConnection.on("PlaybackSettings", (playbackSettings: userPlaybackSettings) => {
      this.$store.commit("playbackModule/SET_PLAYBACK_SETTINGS", playbackSettings)
    })
    
    this.$store.commit('playbackModule/SET_RADIO_CONNECTION', radioConnection);
  }
  
  private setUserPreferences():void{
    const userSettings:userSettings = this.$store.getters['userSettingsModule/loadUserSettings'];
    this.$vuetify.theme.dark = userSettings.darkMode;
  }
  
  get accessToken():string{
    return this.$store.state.oidcStore.access_token;
  }
  
  @Watch('accessToken')
  setAxiosInterceptor(newValue:any, oldValue:any){
    console.log(parseJwt(newValue));
      this.$axios.interceptors.request.use(
              (config:any) => {
                config.headers.common["Authorization"] = `Bearer ${newValue}`;
                config.withCredentials = true;
                return config;
            },        
      )

    this.$store.commit("profileModule/SET_USER_CLAIMS", parseJwt(newValue));
    this.$store.dispatch('profileModule/getUserProfile');
  }
  
  
}
</script>
