<template>
  <v-app-bar
      app
      clipped-left
  >
    <v-app-bar-nav-icon @click.stop="toggleDrawer"/>
    <v-toolbar-title>PJFM</v-toolbar-title>
    <v-spacer></v-spacer>
    {{ screenSize }}
    <span class="align-bottom overline grey--text" v-if="username != null && this.$vuetify.breakpoint.width > 600">INGELOGD ALS <span
        class="orange--text">{{ username }}</span></span>
    <span v-else-if="username != null" class="orange--text">{{ username }}}</span>
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

@Component({
  name: 'AppBar',
})
export default class AppBar extends Vue {
  get username(): string | undefined {
    return this.$store.getters['userModule/username'];
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