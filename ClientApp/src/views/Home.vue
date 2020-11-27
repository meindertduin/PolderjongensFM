<template>
  <div>
    <v-row justify="center">
      <v-col cols="11" md="10">
        <v-row justify="center">
          <v-col cols="10" md="8">
            <Radio />
          </v-col>
          <v-col md="4">
              <PlaybackSettingsDashboard v-if="isMod" />
              <LiveChat v-else />
          </v-col>
        </v-row>
          <v-row v-if="isMod">
              <v-col class="col-12">
                  <UserIncludeSettingsDashboard />
              </v-col>
          </v-row>
      </v-col>
    </v-row>
  </div>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'
    import Radio from "@/components/HomeComponents/Radio";
    import LiveChat from "@/components/HomeComponents/Livechat";
    import PlaybackSettingsDashboard from "@/components/ModComponents/PlaybackSettingsDashboard.vue";
    import UserIncludeSettingsDashboard from "@/components/ModComponents/UserIncludeSettingsDashboard.vue";
    import {playbackState, playerUpdateInfo} from "@/common/types";

    @Component({
    name: 'Home',
    components: {
        UserIncludeSettingsDashboard,
        Radio,
        LiveChat,
        PlaybackSettingsDashboard  
    }
  })
  export default class Home extends Vue {
    get isMod(){
        return this.$store.getters['profileModule/isMod'];
    }

      get playbackState():playbackState{
          const playbackSettings:playerUpdateInfo = this.$store.getters['playbackModule/getPlaybackInfo'];
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