<template>
  <v-app-bar
      app
      clipped-left
  >
    <v-app-bar-nav-icon @click.stop="toggleDrawer" />
    <v-toolbar-title>PJFM</v-toolbar-title>
    <v-spacer></v-spacer>

    <span class="align-bottom overline grey--text" v-if="userProfile != null">INGELOGD ALS <span class="orange--text">{{userProfile.userName}}</span></span>
    <v-img
        class="mx-2 float-right"
        src="/assets/logo.png"
        max-height="40"
        max-width="40"
        contain
    ></v-img>
  </v-app-bar>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from "vue-class-component";

@Component({
  name: 'AppBar',
})
export default class AppBar extends Vue{
  get userProfile(){
    return this.$store.getters['profileModule/userProfile'];
  }
  
  toggleDrawer(){
    this.$store.commit('userSettingsModule/TOGGLE_SIDE_BAR');
  }
}
</script>

<style scoped>

</style>