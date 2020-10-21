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
      
      <v-btn v-if="!oidcAuthenticated" @click="signInOidcClient">
        Sign in
      </v-btn>
      <v-btn text v-else @click="signOutOidcClient">
        Sign out
      </v-btn>
      
      <v-menu bottom offset-y>
        <template v-slot:activator="{ on, attrs }">
          <v-btn icon text v-bind="attrs" v-on="on">
            <v-icon>mdi-cogs</v-icon>
          </v-btn>
        </template>
        <v-list>
          <DisplaySettingsItemGroup />
        </v-list>
      </v-menu>
      
    </v-app-bar>

    <v-main>
      <RouterView />
    </v-main>
  </v-app>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from "vue-class-component";
import DisplaySettingsItemGroup from "@/components/CommonComponents/DisplaySettingsItemGroup.vue";

@Component({
  name: 'App',
  components: {
    DisplaySettingsItemGroup,
  }
})
export default class App extends Vue{
  
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
