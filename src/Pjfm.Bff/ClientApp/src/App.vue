<template>
  <v-app>
    <AppSideBar/>
    <AppBar/>
    <v-main>
      <v-container fluid>
        <router-view></router-view>
      </v-container>
    </v-main>
    <AppBottomBar/>
    <ModServerMessageHandler v-if="isMod"/>
  </v-app>

</template>

<script lang="ts">
import Vue from 'vue';
import Component from "vue-class-component";
import DisplaySettingsItemGroup from "@/components/CommonComponents/DisplaySettingsItemGroup.vue";
import ModServerMessageHandler from "@/components/ModComponents/ModServerMessageHandler.vue";
import AppSideBar from "@/components/CommonComponents/AppSideBar.vue";
import AppBar from "@/components/CommonComponents/AppBar.vue";
import AppBottomBar from "@/components/CommonComponents/AppBottomBar.vue";
import {setupApplication} from "@/helpers/application-setup";

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

export default class App extends Vue {

  created() {
    setupApplication(this);
  }

  get isMod(): boolean {
    return this.$store.getters['userModule/userIsMod'];
  }
}
</script>
