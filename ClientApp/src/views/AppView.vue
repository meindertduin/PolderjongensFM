<template>
  <div>
    <v-row justify="center">
      <v-row justify="center">
        <v-col cols="12" md="10">
          <Radio />
        </v-col>
      </v-row>
    </v-row>
    <div v-if="isMod">
      <v-row justify="center">
        <v-col v-if="isMod" md="10">
          <PlaybackSettingsDashboard  />
        </v-col>
      </v-row>
      <v-row justify="center">
        <v-col md="10">
          <UserIncludeSettingsDashboard />
        </v-col>
      </v-row>
    </div>
  </div>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'
    import Radio from "@/components/HomeComponents/Radio";
    import LiveChat from "@/components/HomeComponents/Livechat";
    import PlaybackSettingsDashboard from "@/components/ModComponents/PlaybackSettingsDashboard.vue";
    import UserIncludeSettingsDashboard from "@/components/ModComponents/UserIncludeSettingsDashboard.vue";
    import {playbackState, djPlaybackInfo} from "@/common/types";

    @Component({
    name: 'AppView',
    components: {
        UserIncludeSettingsDashboard,
        Radio,
        LiveChat,
        PlaybackSettingsDashboard  
    }
  })
  export default class AppView extends Vue {
    get isMod(){
        return this.$store.getters['profileModule/isMod'];
    }

      get playbackState():playbackState{
          const playbackSettings:djPlaybackInfo = this.$store.getters['playbackModule/getPlaybackInfo'];
          if (playbackSettings){
              return playbackSettings.playbackSettings.playbackState;
          }
          return playbackState['Dj-mode'];
      }

    navigate(uri : string) : void{
      this.$router.push(uri);
    }
  }
</script>

<style>
  #sticky-button{
      position: sticky;
      bottom: 10px;
      right: 10px;
  }
</style>