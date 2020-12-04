<template>
  <v-app>

    <v-navigation-drawer
        v-model="drawer"
        app
        clipped
    >
      <v-list rounded>
        <v-subheader>Menu</v-subheader>
        <v-list-item link @click="navigate('/')">
          <v-list-item-action>
            <v-icon>mdi-home</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>Radio</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-list-item link @click="navigate('/search')">
          <v-list-item-action>
            <v-icon>mdi-playlist-plus</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>Verzoekje doen</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-divider></v-divider>

        <v-subheader>Account</v-subheader>
        <div v-if="!oidcAuthenticated">
          <v-list-item link @click="signInOidcClient()">
            <v-list-item-action>
              <v-icon>mdi-account</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>Inloggen</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
          <v-list-item link @click="redirect('/register')">
            <v-list-item-action>
              <v-icon>mdi-logout-variant</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>Registreren</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </div>
        <div v-else>
          <v-list-item link @click="signOutOidcClient()">
            <v-list-item-action>
              <v-icon>mdi-logout-variant</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>Uitloggen</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </div>
      </v-list>

    </v-navigation-drawer>
    
    
    <v-app-bar
        app
        clipped-left
    >

      <v-app-bar-nav-icon @click.stop="drawer = !drawer" />
      <v-toolbar-title>PJFM</v-toolbar-title>
      <v-spacer></v-spacer>

      <span class="align-bottom overline grey--text" v-if="oidcAuthenticated">INGELOGD ALS <span class="orange--text">{{userProfile.userName}}</span></span>
      <v-img
          class="mx-2 float-right"
          src="/assets/logo.png"
          max-height="40"
          max-width="40"
          contain
      ></v-img>
    </v-app-bar>

    <!-- Sizes your content based upon application components -->
    <v-main>
      
      <!-- Provides the application the proper gutter -->
      <v-container fluid>
        
        <!-- If using vue-router -->
        <router-view></router-view>
      </v-container>
    </v-main>


    <template>
      <v-row justify="center">
        <v-dialog
            v-model="dialog"
            persistent
            max-width="600"
        >
          <v-card>
            <v-card-title>
              PJFM - Instellingen
            </v-card-title>
            <v-card-text>
              Bij het luisteren naar PJFM wordt je Spotify tijdelijk bestuurd door de PJFM app.<br><br>
              Hieronder kan je aangeven hoelang de PJFM app je Spotify mag besturen.<br><br>
              Als je tijdens het luisteren wilt dat PJFM stopt met het besturen van jouw account kan je op STOP onder in het scherm klikken.<br><br>
              <v-spacer></v-spacer>
              <v-radio-group
                  v-model="subscribeDuration"
                  row
                  class="align-content-center"
              >
                <v-radio
                    label="30 minuten"
                    value="5"
                ></v-radio>
                <v-radio
                    label="2 uur"
                    value="10"
                ></v-radio>
                <v-radio
                    label="4 uur"
                    value="15"
                ></v-radio>
              </v-radio-group>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn
                  color="orange darken-1"
                  text
                  @click="connectWithPlayer()"
              >
                Prima!
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-dialog>
      </v-row>
    </template>
    
    
    <v-bottom-navigation fixed>


        <v-btn v-if="oidcAuthenticated && !playbackConnected" @click="openDialog" block>
          <span>Start</span>
          <v-icon>mdi-play</v-icon>
        </v-btn>
        <v-btn v-else="oidcAuthenticated && playbackConnected" @click="disconnectWithPlayer" block>
          <span>Stop</span>
          <v-icon>mdi-pause</v-icon>
        </v-btn>
    </v-bottom-navigation>
  </v-app>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from "vue-class-component";
import DisplaySettingsItemGroup from "@/components/CommonComponents/DisplaySettingsItemGroup.vue";
import {userSettings} from "@/common/types";
import {Watch} from "vue-property-decorator";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";

@Component({
  name: 'App',
  components: {
    DisplaySettingsItemGroup,
  }
})

export default class App extends Vue{
  private dialog: boolean = false;
  private drawer: boolean = null;
  private subscribeDuration: number = 0;
  private playbackConnected: boolean = false;
  private radioConnection: HubConnection | null = null;
  
  created(){
    this.setUserPreferences();
    this.setRadioConnection();
  }
  
  private test(){
    console.log(this.subscribeDuration);
  }
  
  private async setRadioConnection():void{
    let radioConnection: HubConnection | null = null;

    // if (this.$store.getters['playbackModule/getRadioConnection'] != null) {
    //   await this.$store.getters['playbackModule/getRadioConnection'].stop();
    // }
    
    radioConnection = new HubConnectionBuilder()
        .withUrl("https://localhost:5001/radio")
        .build();

    radioConnection.start()
        .then(() => console.log("radio connection started"));

    radioConnection.on("ReceivePlayingTrackInfo", (trackInfo) =>
        this.$store.commit('playbackModule/SET_PLAYBACK_INFO', trackInfo));

    radioConnection.on("ISConnected", (connected:boolean) =>{
        this.playbackConnected = connected;
    })
    
    this.$store.commit('playbackModule/SET_RADIO_CONNECTION', radioConnection);
  }
  
  private openDialog(){
    this.dialog = true;
  }
  
  private setUserPreferences():void{
    const userSettings:userSettings = this.$store.getters['userSettingsModule/loadUserSettings'];
    this.$vuetify.theme.dark = userSettings.darkMode;
  }

  get oidcAuthenticated():any|null{
    return this.$store.getters['oidcStore/oidcIsAuthenticated'];
  }
  
  get accessToken():string{
    return this.$store.state.oidcStore.access_token;
  }
  
  get userProfile(){
    return this.$store.getters['profileModule/userProfile'];
  }
  
  @Watch('accessToken')
  setAxiosInterceptor(newValue:any, oldValue:any){
      this.$axios.interceptors.request.use(
              (config:any) => {
                config.headers.common["Authorization"] = `Bearer ${newValue}`;
                config.withCredentials = true;
                return config;
            },        
      )
      this.$store.dispatch('profileModule/loadModState');

    this.$store.dispatch('profileModule/loadModState');
    this.$store.dispatch('profileModule/getUserProfile');
  }

  private navigate(uri){
    this.$router.push(uri);
  }
  
  private signInOidcClient(){
    this.$store.dispatch('oidcStore/authenticateOidc');
  }
  
  private signOutOidcClient(){
    this.$store.dispatch('oidcStore/signOutOidc');
  }

  private connectWithPlayer(){
    this.dialog = false;
    
    this.$store.getters['playbackModule/getRadioConnection']?.invoke("ConnectWithPlayer", parseInt(this.subscribeDuration))
        .then(() => console.log("connection started with player"))
        .catch((err) => console.log(err));
  }

  private disconnectWithPlayer(){
    this.playbackConnected = false;

    this.$store.getters['playbackModule/getRadioConnection']?.invoke("DisconnectWithPlayer")
        .then(() => console.log("Disconnected with player"))
        .catch((err) => console.log(err));
  }
}
</script>
