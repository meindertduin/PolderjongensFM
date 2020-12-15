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
        <v-list-item link @click="navigate('/register')">
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
</template>

<script lang="ts">
import Vue from 'vue';
import Component from "vue-class-component";

@Component({
  name: 'AppSideBar',
})
export default class AppSideBar extends Vue{
  get sideBar(): boolean {
    this.sideBarOpen = this.$store.getters["userSettingsModule/getSidebarOpenState"];
  }
  
  private sideBarOpen:boolean = false;
  
  get oidcAuthenticated():any|null{
    return this.$store.getters['oidcStore/oidcIsAuthenticated'];
  }
  
  get playbackSettings():any|null{
    return this.$store.getters['playbackModule/getPlaybackSettings']
  }

  private navigate(uri : string){
    this.$router.push(uri);
  }
  
  private songRequestIsAvailable(): boolean {
    if(!this.oidcAuthenticated) return false; // Login check
    if(this.playbackSettings.playbackState == 0) return false; // Request mode check
    
    // Add max song request check
    
    return true;
  }

  private signInOidcClient(){
    this.$store.dispatch('oidcStore/authenticateOidc');
  }

  private signOutOidcClient(){
    this.$store.dispatch('oidcStore/signOutOidc');
  }

}
</script>

<style scoped>

</style>