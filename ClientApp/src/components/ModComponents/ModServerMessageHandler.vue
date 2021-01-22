<template>
  <v-snackbar v-if="currentMessage" :color="currentMessage.error? 'red': 'dark'" v-model="snackBarOpen" :timeout="snackBarTimeOut">
    {{currentMessage.message}}
    <template v-slot:action="{ attrs }">
      <v-btn color="blue" text v-bind="attrs" @click="snackBarOpen = false">
        Sluiten
      </v-btn>
    </template>
  </v-snackbar>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from "vue-class-component";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {djPlaybackInfo, hubServerMessage, playbackSettings} from "@/common/types";

@Component({
  name: "ModServerMessageHandler",
})
export default class ModServerMessageHandler extends Vue{
  private snackBarOpen :boolean = true;
  private snackBarTimeOut = 5000;
  private djHubSocketConnection: HubConnection | null = null;
  
  private currentMessage : hubServerMessage | null = null;
  
  created(){
    this.connectToDjHub();
  }
  
  connectToDjHub(){
    this.djHubSocketConnection = new HubConnectionBuilder()
        .withUrl(`${process.env.VUE_APP_API_BASE_URL}/radio/dj`)
        .build();

    this.djHubSocketConnection?.on("ServerMessage", (message: hubServerMessage) => {
      this.snackBarOpen = true;
      this.currentMessage = message;
    })
    this.djHubSocketConnection?.on("PlaybackSettings", (settings: playbackSettings) => {
      this.$store.commit("modModule/SET_PLAYBACK_SETTINGS", settings);
    })
    this.djHubSocketConnection?.on("ReceiveDjPlaybackInfo", (playbackInfo: djPlaybackInfo) => {
      this.$store.commit("modModule/SET_DJ_PLAYBACK_INFO", playbackInfo);
    })
    
    this.djHubSocketConnection.start()
      .then(() => {
        
      })
  }
}
</script>

<style scoped>

</style>