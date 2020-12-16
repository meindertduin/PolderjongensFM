<template>
    <v-row>
      <v-col class="d-block">
          <v-card class="pa-2" outlined round v-if="currentTrackInfo">
              <span class="overline grey--text">HUIDIGE POKOE</span><br>
              <span class="subtitle-1">{{ currentTrackInfo.artists[0] }} - {{ currentTrackInfo.title }}</span><br>
              <span class="subtitle-2 orange--text">{{ convertMsToMMSS(elapsedTime) }} - {{ convertMsToMMSS(currentTrackDuration) }} </span>
          </v-card>
      </v-col>
      <v-col class="d-none d-md-block">
        <v-card class="pa-2" outlined round v-if="nextTrackInfo">
          <span class="overline grey--text">VOLGENDE POKOE</span><br>
          <span class="subtitle-1">{{ nextTrackInfo.artists[0] }} - {{ nextTrackInfo.title }}</span><br>
          <span class="subtitle-2 orange--text">00:00 - {{ convertMsToMMSS(nextTrackDuration) }}</span>
        </v-card>
      </v-col>
    </v-row>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import {djPlaybackInfo, trackDto} from "@/common/types";
import {Watch} from "vue-property-decorator";

@Component({
        name: 'SongInformation',
    })
    export default class SongInformation extends Vue {
        private elapsedTime = null;
        private timer = null;
        
        get currentTrackDuration(){
            const currentTrackInfo = this.currentTrackInfo;
            if(currentTrackInfo){
              return new Date(currentTrackInfo.songDurationMs).getTime();
            }

            return 0;
        }

        get nextTrackDuration(){
          const nextTrackInfo = this.nextTrackInfo;
            if(nextTrackInfo){
              return new Date(nextTrackInfo.songDurationMs).getTime();
            }

            return 0;
        }

        get priorityTracksQueue():Array<trackDto>{
          return this.$store.getters['playbackModule/getPriorityQueuedTracks'];
        }
        
        get fillerTracksQueue():Array<trackDto>{
          return this.$store.getters['playbackModule/getFillerQueuedTracks'];
        }
        
        get secondaryTracksQueue():Array<trackDto>{
          return this.$store.getters['playbackModule/getSecondaryQueuedTracks'];
        }
  
        get currentTrackInfo():trackDto{
          return this.$store.getters['playbackModule/getCurrentTrack'];
        }
        
        get currentTrackStartingTime():string {
          return this.$store.getters['playbackModule/getCurrentTrackStartingTime'];
        }
  
        get nextTrackInfo():trackDto{
          let result = null;
          
          if (this.priorityTracksQueue.length > 0){
            result = this.priorityTracksQueue[0];
          } else if(this.secondaryTracksQueue.length > 0){
            result = this.secondaryTracksQueue[0];
          } else if(this.fillerTracksQueue.length > 0){
            result = this.fillerTracksQueue[0];
          }
          this.updateElapsedTime();
          return result;
        }

        @Watch("currentTrackInfo")
        onCurrentTrackInfoChange(newValue){
          this.updateElapsedTime();
        }
      
        @Watch("nextTrackInfo")
        onNextTrackInfoChange(newValue){
          this.updateElapsedTime();
        }
        
        
        updateElapsedTime() : void {
          let now = new Date();

          if(this.currentTrackInfo){
              this.elapsedTime = now.getTime() - new Date(this.currentTrackStartingTime).getTime();
              if(this.timer == null){
                  this.timer = setInterval(() => {
                      this.elapsedTime += 1000;
                  }, 1000)
              }
          }
        } 

        convertMsToMMSS(ms) : string {
            let date = new Date(null);
            date.setMilliseconds(ms);

            return date.toISOString().substr(14, 5);
        }
    }
</script>
