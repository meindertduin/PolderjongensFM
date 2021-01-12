<template>
  
  <v-navigation-drawer
      v-model="sideBarOpen"
      :value="sideBar"
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
      <v-list-item link v-if="songRequestIsAvailable()" @click="navigate('/search')">
        <v-list-item-action>
          <v-icon>mdi-playlist-plus</v-icon>
        </v-list-item-action>
        <v-list-item-content>
          <v-list-item-title>Verzoekje doen</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <v-list-item disabled link v-if="!songRequestIsAvailable()">
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
        <v-list-item link @click="redirect( process.env.VUE_APP_API_BASE_URL + '/Account/Register')">
          <v-list-item-action>
            <v-icon>mdi-logout-variant</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>Registreren</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
        <v-divider></v-divider>
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
        <v-divider></v-divider>
      </div>
      <v-list-item>
        <v-list-item-content>
          <!-- Playback Connected -->
          <v-list-item-subtitle v-if="isPlaybackConnected()">Verbonden tot: <span class="orange--text">{{ subscribeEndTime }}</span></v-list-item-subtitle>
          <!-- No Playback -->
          <v-list-item-subtitle v-if="!isPlaybackConnected()" class="red--text">Niet verbonden met PJFM</v-list-item-subtitle>
          <!--  -->
        </v-list-item-content>
      </v-list-item>
    </v-list>
  </v-navigation-drawer>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from "vue-class-component";
import {Watch} from "vue-property-decorator";

@Component({
  name: 'AppSideBar',
})
export default class AppSideBar extends Vue{
  get sideBar(): boolean {
    const sideBarOpen = this.$store.getters["userSettingsModule/getSidebarOpenState"];
    this.sideBarOpen = sideBarOpen;
    return sideBarOpen;
  }
  
  private sideBarOpen:boolean = false;
  
  @Watch("sideBarOpen")
  onSideBarOpenChange(newValue:boolean){
    if (!newValue){
      this.$store.commit("userSettingsModule/SET_SIDE_BAR", false);
    }
  }
  
  get oidcAuthenticated():any|null{
    return this.$store.getters['oidcStore/oidcIsAuthenticated'];
  }
  
  get playbackState():any|null{
    return this.$store.getters['playbackModule/getPlaybackState']
  }

  get connectedState():any|null{
    return this.$store.getters['playbackModule/getConnectedState']
  }

  get subscribeEndDate():any|null{
    return this.$store.getters['playbackModule/getSubscribeEndDate']
  }

  get subscribeEndTime():any|null{
    return this.subscribeEndDate.toISOString().substr(11, 5);
  }

  get isMod(){
    return this.$store.getters['profileModule/isMod'];
  }
  
  private isPlaybackConnected() : boolean{
    if (!this.connectedState) return false;
    // More checks?
    
    return true;
  }

  private navigate(uri : string){
    this.$router.push(uri).catch(() => {});
  }
  
  private songRequestIsAvailable(): boolean {
    if(!this.oidcAuthenticated) return false; // Login check
    if(this.playbackState == 0 && this.isMod == false) return false; // Request mode check
    
    // Add max song request check
    
    return true;
  }

  private signInOidcClient(){
    this.$store.dispatch('oidcStore/authenticateOidc');
  }

  private redirect(uri: string){
    window.location.href = uri;
  }

  private signOutOidcClient(){
    this.$store.dispatch('oidcStore/signOutOidc');
  }

}
</script>

<style scoped>

</style>