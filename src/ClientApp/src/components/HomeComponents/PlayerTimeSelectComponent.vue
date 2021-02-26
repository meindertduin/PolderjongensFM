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
      <v-row
          class="mb-4"
          justify="space-between"
      >
        <v-col class="text-left">
                  <span
                      class="display-3 font-weight-light"
                      v-text="selectedTimeString"
                  ></span>
        </v-col>
        <v-col class="text-right">
          <v-btn color="orange darken-1" dark depressed fab @click="initializePlayerConnection">
            <v-icon large>
              mdi-play
            </v-icon>
          </v-btn>
        </v-col>
      </v-row>
      <v-slider
          v-model="sliderUnit"
          color="red"
          track-color="grey"
          always-dirty
          :min="sliderMin"
          :max="sliderMax"
      >
        <template v-slot:prepend>
          <v-icon
              color="green"
              @click="decrement"
          >
            mdi-minus
          </v-icon>
        </template>

        <template v-slot:append>
          <v-icon color="green" @click="increment">
            mdi-plus
          </v-icon>
        </template>
      </v-slider>
      <v-row justify="center">
        <v-col class="col-8 col-md-10">
          <v-select outlined v-model="selectedDevice" :items="userDevices" item-text="name" return-object single-line></v-select>
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


@Component({
  name: 'PlayerTimeSelectComponent',
})
export default class PlayerTimeSelectComponent extends Vue {
  private sliderUnit = 0;
  private sliderMin : number = 0;
  private sliderMax : number = 1000;
  
  private userDevices: Array<playbackDevice> = [];
  private selectedDevice: playbackDevice | null = null;
  
  get selectedTimeString():string{
      let minutes = this.getSelectedMinutes();
      let hours = Math.floor(minutes / 60);
            
      if (minutes < 60) return `${minutes} minuten`
      return `${hours} uur`
  }
  
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
  
  getSelectedMinutes():number{
    return Math.floor(Math.pow(this.sliderUnit / 40, 2));
  }
  
  
  initializePlayerConnection(){
    const minutes = this.getSelectedMinutes();
    if (minutes <= 0) return;
    // checks before connecting if user is spotify authenticated
    if (this.loggedInUserProfile !== null){
      this.connectWithPlayer(minutes);
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
  
  increment(){
    this.sliderUnit += 10;
  }

  decrement(){
    this.sliderUnit -= 10;
  }

  togglePlayerTimerOverlay(){
    this.$store.commit('playbackModule/TOGGLE_PLAYER_TIMER_OVERLAY');
  }
  
}
</script>

<style scoped>

</style>