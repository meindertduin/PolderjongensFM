<template>
  <div>
    <v-bottom-navigation fixed v-if="oidcAuthenticated">
      <template>
        <v-row justify="center">
          <v-dialog v-model="playerTimerOverlayActive" persistent max-width="600">
            <PlayerTimeSelectComponent />
          </v-dialog>
        </v-row>
      </template>
      <v-btn v-if="oidcAuthenticated && !playbackConnected" @click="togglePlayerTimerOverlay" block>
        <span>Start</span>
        <v-icon>mdi-play</v-icon>
      </v-btn>
      <v-btn v-else="playbackConnected" @click="disconnectWithPlayer" block>
        <span>Stop</span>
        <v-icon>mdi-pause</v-icon>
      </v-btn>
    </v-bottom-navigation>
    <v-bottom-navigation fixed v-else>
      
    </v-bottom-navigation>
  </div>
</template>

<script lang="ts">

import Vue from 'vue';
import Component from "vue-class-component";
import PlayerTimeSelectComponent from "@/components/HomeComponents/PlayerTimeSelectComponent.vue";

@Component({
  name: 'AppBottomBar',
  components: {
    PlayerTimeSelectComponent,
  }
})
export default class AppBottomBar extends Vue{
  get playbackConnected():boolean{
    return this.$store.getters['playbackModule/getConnectedState'];
  }
  
  get playerTimerOverlayActive():boolean{
    return this.$store.getters['playbackModule/getPlayerTimerOverlayActive'];
  }

  get oidcAuthenticated():any|null{
    return this.$store.getters['oidcStore/oidcIsAuthenticated'];
  }
  
  private togglePlayerTimerOverlay(){
    this.$store.commit('playbackModule/TOGGLE_PLAYER_TIMER_OVERLAY');
  }

  private disconnectWithPlayer(){
    this.$store.commit('playbackModule/SET_CONNECTED_STATUS', false);

    this.$store.getters['playbackModule/getRadioConnection']?.invoke("DisconnectWithPlayer")
        .then(() => console.log("Disconnected with player"))
        .catch((err) => console.log(err));
  }
}
</script>
<style scoped>

</style>