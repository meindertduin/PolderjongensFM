<template>
  <div>
    <!-- Authenticated -->
    <v-bottom-navigation fixed v-if="userAuthenticated">
      <template>
        <v-row justify="center">
          <v-dialog v-model="playerTimerOverlayActive" persistent max-width="600">
            <PlayerTimeSelectComponent />
          </v-dialog>
        </v-row>
      </template>
      <v-btn v-if="!playbackConnected" @click="togglePlayerTimerOverlay" block>
        <span>Start</span>
        <v-icon>mdi-play</v-icon>
      </v-btn>
      <v-btn v-else-if="playbackConnected" @click="disconnectWithPlayer" block>
        <span>Stop</span>
        <v-icon>mdi-pause</v-icon>
      </v-btn>
    </v-bottom-navigation>
    
    <!-- Not Authenticated -->
    <v-bottom-navigation fixed v-else>
      <v-btn @click="signInOidcClient" block>
        <span>Inloggen</span>
        <v-icon>mdi-play</v-icon>
      </v-btn>
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

  get userAuthenticated():boolean {
    return this.$store.getters['userModule/userAuthenticated'];
  }

  private signInOidcClient(): void{
    // TODO: route the user to the login page
  }
  
  private togglePlayerTimerOverlay(){
    const userSpotifyAuthenticated: boolean = this.$store.getters["userModule/userSpotifyAuthenticated"];
    
    // open playerTimeOverlay only when user is spotify authenticated
    if (userSpotifyAuthenticated) {
      this.$store.commit('playbackModule/TOGGLE_PLAYER_TIMER_OVERLAY');
    } else {
      window.location.href = process.env.VUE_APP_API_BASE_URL + "/api/spotify/account/authenticate"
    }
  }

  private disconnectWithPlayer(){
    this.$store.commit('playbackModule/SET_CONNECTED_STATUS', false);

    this.$store.getters['playbackModule/getRadioConnection']?.invoke("DisconnectWithPlayer")
        .then(() => console.log("Disconnected with player"))
        .catch((err:any) => console.log(err));
  }
}
</script>
<style scoped>

</style>