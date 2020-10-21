<template>
  <v-app>
    <v-app-bar
      app
      color="primary"
      dark
      dense
    >
      <v-app-bar-nav-icon></v-app-bar-nav-icon>
      <v-toolbar-title class="display-1 font-weight-bold">PolderjongensFM</v-toolbar-title>
      <v-spacer></v-spacer>

      <v-toolbar-title>Goededag {{userName}}</v-toolbar-title>
      
      <v-btn v-if="!oidcAuthenticated" @click="signInOidcClient">
        Sign in
      </v-btn>
      <v-btn v-else @click="signOutOidcClient">
        Sign out
      </v-btn>
    </v-app-bar>

    <v-main>
      <RouterView />
    </v-main>
  </v-app>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from "vue-class-component";

@Component({
  name: 'App',
})
export default class App extends Vue{
  private userName:string = "";
  
  get oidcAuthenticated():any|null{
    return this.$store.getters['oidcStore/oidcIsAuthenticated'];
  }
  
  private signInOidcClient(){
    this.$store.dispatch('oidcStore/authenticateOidc');
  }
  
  private signOutOidcClient(){
    this.$store.dispatch('oidcStore/signOutOidc');
  }
}
</script>
