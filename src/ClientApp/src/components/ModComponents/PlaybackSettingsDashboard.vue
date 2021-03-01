<template>
    <v-card class="pa-2" outlined round>
        <v-card-title>
            Mod panel
        </v-card-title>
        <v-card-text>
            <v-row>
                <v-col>
                    <v-card class="pa-2">
                      <v-select :items="fillerQueueStates" v-model="activeFillerQueueState" outlined :value="fillerQueueState" label="FillerQueue staat"></v-select>
                      <div v-if="activeFillerQueueState === 0">
                        <div class="text-h6 my-3">TopTracks Opties</div>
                        <div class="selects-container">
                          <div class="text-h6 ma-2">Termijn</div>
                          <v-slider
                              v-model="selectedTerm"
                              :tick-labels="terms"
                              :value="playbackTermFilter"
                              :max="5"
                              step="1"
                              ticks="always"
                              tick-size="4"
                          ></v-slider>
                        </div>
                      </div>
                      <div v-else-if="activeFillerQueueState === 1">
                        <GenreBrowsingOptionsDisplay /> 
                      </div>
                    </v-card>
                </v-col>
            </v-row>
            <v-row>
              <v-col>
                <v-card class="pa-2">
                  <div class="text-h6 ma-2">Staat</div>
                  <v-select :items="stateItems" v-model="selectedState" :value="playbackState" outlined label="Playback staat"></v-select>
                  <div class="text-h6 ma-2">Max requests gebruiker</div>
                  <v-select :disabled="playbackState === 0" :items="maxRequestItems" v-model="maxRequestAmount" :value="maxUserRequestAmount" outlined label="Playback staat"></v-select>
                </v-card>
              </v-col>
            </v-row>
            <v-row>
                <v-col class="col-12">
                    <div class="switch-container">
                        <v-switch
                                @click="showConfirmNotification = true"
                                class="ma-2"
                                v-model="isPlaying"
                                label="Playback aan/uit"
                                color="red"
                                :value="playbackOn"
                                hide-details
                        ></v-switch>
                        <v-snackbar v-model="showConfirmNotification">
                            <template v-slot:action="{ attrs }">
                                <v-row justify="center">
                                    <v-btn
                                            color="green"
                                            text
                                            v-bind="attrs"
                                            @click="handleConfirmPlaybackSet">
                                        Confirm
                                    </v-btn>
                                    <v-btn
                                            color="white"
                                            text
                                            v-bind="attrs"
                                            @click="handleRejectPlaybackSet">
                                        Reject
                                    </v-btn>
                                </v-row>
                            </template>
                        </v-snackbar>
                    </div>
                </v-col>
                <v-divider dark></v-divider>
            </v-row>
            <v-row justify="center">
                <v-col class="col-12">
                </v-col>
            </v-row>
            <v-row>
                <v-col class="col-6">
                    <v-btn class="ma-2" large width="100%" @click="handleReset">
                        Reset Playback
                    </v-btn>
                </v-col>
                <v-col class="col-6">
                    <v-btn class="ma-2" large width="100%" @click="handleSkip">
                        Skip nummer
                    </v-btn>
                </v-col>
            </v-row>
        </v-card-text>
    </v-card>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from "vue-class-component";
import {fillerQueueType} from "@/common/types";
import {Watch} from "vue-property-decorator";
import GenreBrowsingOptionsDisplay from "@/components/ModComponents/GenreBrowsingOptionsDisplay.vue";

@Component({
        name: "PlaybackSettingsDashboard",
        components: {
            GenreBrowsingOptionsDisplay,
        },
    })
    export default class PlaybackSettingsDashboard extends Vue{
        get playbackOn(){
          const playbackOn = this.$store.getters['playbackModule/getPlayingStatus'];
          this.isPlaying = playbackOn;
          return playbackOn;
        }  
        
        get playbackTermFilter(){
          const selectedTerm = this.$store.getters['modModule/getPlaybackTermFiler'];
          this.selectedTerm = selectedTerm;
          return selectedTerm;
        }
        
        get playbackState(){
          const selectedState = this.$store.getters['modModule/getPlaybackState'];
          if (this.selectedState === null){
            this.selectedState = selectedState;
          }
          return selectedState;
        }
        
        get fillerQueueState() {
          const fillerQueueState = this.$store.getters['modModule/getFillerQueueState']
          if (this.activeFillerQueueState === null) {
            this.activeFillerQueueState = fillerQueueState;
          }
          return fillerQueueState;
        }
        
        get maxUserRequestAmount(){
          const maxRequestAmount = this.$store.getters['modModule/getMaxRequestsPerUser'];
          if (this.maxRequestAmount === null){
            this.maxRequestAmount = maxRequestAmount;
          }
          return maxRequestAmount;
        }
      
        private isPlaying:boolean = false;
        private selectedTerm: number = 0;
        
        private terms :any[] = ['short', 'short-med', 'med', 'med-long', 'long', 'all'];
        private stateItems :any[] = [
            {text: 'Dj-mode', value: 0}, 
            {text: 'wachtrij-mode', value: 1}, 
            {text: 'random-mode', value: 2}, 
            {text: 'round-robin', value: 3},
          ];
        private fillerQueueStates: any[] = [
          { text: 'Gebruikers Top Tracks', value: 0 },
          { text: 'Genre Browsing', value: 1 },
        ]
      
        private maxRequestItems: any[] = [
          {text: "1", value: 1},
          {text: "2", value: 2},
          {text: "3", value: 3},
          {text: "4", value: 4},
          {text: "5", value: 5},
          {text: "10", value: 10},
          {text: "20", value: 20},
        ];
               
        private showConfirmNotification :boolean = false;
        
        private selectedState :any | null = null;
        private activeFillerQueueState: fillerQueueType | null = null;
        
        @Watch("selectedState")
        onSelectedStateChanged(newValue:any, oldValue:any){
          if (oldValue === null) return;
          if (oldValue === newValue) return;
          
          // @ts-ignore
          this.$axios.put(`api/playback/mod/setPlaybackState?playbackState=${this.selectedState}`)
              .catch((err:any) => console.log(err));
        }
        
        @Watch("activeFillerQueueState")
        onActiveFillerQueueChange(newValue: any, oldValue:any){
          if (oldValue === null) return;
          if (oldValue === newValue) return;

          // Todo: send request to change fillerQueueState
        }

        private maxRequestAmount : number | null = null;
      
        async handleConfirmPlaybackSet(){
            this.showConfirmNotification = false;
            try {
                if (this.isPlaying){
                    // @ts-ignore
                    await this.$axios.put('api/playback/mod/on');
                }
                else{
                    // @ts-ignore
                    await this.$axios.put('api/playback/mod/off');
                }
            }
            catch (e) {
              console.log(e)
            }
        }

        async handleRejectPlaybackSet(){
          this.showConfirmNotification = false;
        }

        async handleReset(){
          // set to default value if null
          if (this.maxRequestAmount === 0){
            this.maxRequestAmount = 3;
          }
          // @ts-ignore
          await this.$axios.put(`api/playback/mod/reset?maxRequestAmount=${this.maxRequestAmount}&term=${this.selectedTerm}`);
        }
  
        handleSkip(){
          // @ts-ignore
          this.$axios.put('api/playback/mod/skip');
        }
    }
        
</script>

<style scoped>
    .switch-container{
        display: flex;
        flex-direction: row;
        flex-wrap: wrap;
    }
    .selects-container{
        
    }
</style>