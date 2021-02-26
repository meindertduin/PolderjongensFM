<template>
  <v-card>
    <v-card-title>
      PJFM - Instellingen
    </v-card-title>
    <v-card-text>
      Bij het luisteren naar PJFM wordt je Spotify tijdelijk bestuurd door de PJFM app.<br><br>
      Hieronder kan je aangeven hoelang de PJFM app je Spotify mag besturen.<br><br>
      Als je tijdens het luisteren wilt dat PJFM stopt met het besturen van jouw account kan je op STOP onder in het scherm klikken.<br><br>
      <v-spacer></v-spacer>
      <v-row justify="center">
        <v-col class="col-8 col-md-10">
          <v-select label="afpseel apparaat" outlined v-model="selectedDevice" :items="userDevices" item-text="name"></v-select>
        </v-col>
      </v-row>
      <v-row class="mb-4" justify="center">
        <v-col class="col-6 col-md-8">
          <v-select
              label="afspeeltijd"
              outlined
              :items="playbackTimeOptions"
              v-model="selectedPlaybackTime"
              item-text="text"
          ></v-select>
        </v-col>
        <v-col class="col-2 col-md-2">
          <v-btn color="orange darken-1" dark depressed fab @click="initializePlayerConnection">
            <v-icon large>
              mdi-play
            </v-icon>
          </v-btn>
        </v-col>
      </v-row>
    </v-card-text>
    <v-card-actions>
      <v-btn color="red" text @click="togglePlayerTimerOverlay" width="100%">
        Annuleren
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from "vue-class-component";

interface playbackDevice {
  id: string,
  isActive: boolean,
  isPrivateSession: boolean,
  isRestricted: boolean,
  name: string,
  type: string,
  volumePercent: number,
}

interface timeOption {
  text: string,
  minutesAmount: number,
}

@Component({
  name: 'PlayerTimeSelectComponent',
})
export default class PlayerTimeSelectComponent extends Vue {
  private userDevices: Array<playbackDevice> = [];
  private selectedDevice: playbackDevice | null = null;
  
  private playbackTimeOptions: Array<timeOption> = [
    { text: "5 minuten", minutesAmount: 5 },
    { text: "10 minutem", minutesAmount: 10 },
    { text: "15 minuten", minutesAmount: 15},
    { text: "20 minuten", minutesAmount: 20},
    { text: "30 minuten", minutesAmount: 30},
    { text: "1 uur", minutesAmount: 60},
    { text: "2 uur", minutesAmount: 120 },
    { text: "4 uur", minutesAmount: 240},
    { text: "8 uur", minutesAmount: 480},
    { text: "Hele dag!", minutesAmount: 1440 },
  ];
  
  private selectedPlaybackTime: timeOption = { text: "30 minuten", minutesAmount: 30 };
  
  get loggedInUserProfile(){
    return this.$store.getters["profileModule/userProfile"];
  }
  
  created(){
    // @ts-ignore
    this.$axios.get(process.env.VUE_APP_API_BASE_URL + `/api/playback/devices`)
      .then(({data}:{data:Array<playbackDevice> | null}) => {
         if (data){
           this.userDevices = data.filter(d => !d.isPrivateSession && !d.isRestricted);
           if (this.userDevices.length > 0){
             const activeDevice: playbackDevice | undefined = this.userDevices.find(d => d.isActive);
             if (activeDevice){
               this.selectedDevice = activeDevice;
             } else {
               this.selectedDevice = this.userDevices[0];
             }
           }
         }
      })
  }

  initializePlayerConnection(){
    const selectedMinutes: number = this.selectedPlaybackTime.minutesAmount;
    if (selectedMinutes <= 0) return;
    // checks before connecting if user is spotify authenticated
    if (this.loggedInUserProfile !== null){
      this.connectWithPlayer(selectedMinutes);
    }
  }
  
  connectWithPlayer(minutes: number){
    this.$store.getters['playbackModule/getRadioConnection']?.invoke("ConnectWithPlayer", minutes, this.selectedDevice)
        .then(() => {
          this.$store.commit('playbackModule/SET_SUBSCRIBE_TIME', minutes);
        })
        .catch((err:any) => console.log(err))
        .finally(() => {
          this.togglePlayerTimerOverlay();
        });
  }

  togglePlayerTimerOverlay(){
    this.$store.commit('playbackModule/TOGGLE_PLAYER_TIMER_OVERLAY');
  }
  
}
</script>

<style scoped>

</style>