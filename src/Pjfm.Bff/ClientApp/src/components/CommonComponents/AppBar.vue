<template>
  <v-app-bar
      app
      clipped-left
  >
    <v-app-bar-nav-icon @click.stop="toggleDrawer"/>
    <v-toolbar-title>PJFM</v-toolbar-title>
    <v-spacer></v-spacer>
    {{ screenSize }}
    <span class="align-bottom overline grey--text" v-if="user != null && this.$vuetify.breakpoint.width > 600">INGELOGD ALS <span
        class="orange--text">{{ user.userName }}</span></span>
    <span v-else-if="user != null" class="orange--text">{{ user.userName }}</span>
    <v-img
        v-if="this.$vuetify.breakpoint.width > 600"
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
import {User} from "@/store/userModule/user";

@Component({
  name: 'AppBar',
})
export default class AppBar extends Vue {
  get user(): User {
    return this.$store.getters['userModule/user'];
  }

  get screenSize(): number {
    return this.$store.getters["userSettingsModule/getScreenSize"];
  }

  toggleDrawer(): void {
    this.$store.commit('userSettingsModule/TOGGLE_SIDE_BAR');
  }
}
</script>

<style scoped>

</style>