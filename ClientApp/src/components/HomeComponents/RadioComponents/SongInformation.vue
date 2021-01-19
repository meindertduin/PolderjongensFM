<template>
  <v-row justify="center">
    <v-col class="d-block col-lg-5 col-12 col-sm-11 col-md-10">
      <v-card class="pa-2" outlined round v-if="currentTrackInfo">
        <span class="overline grey--text">NU AFGESPEELD</span><br>
        <span class="subtitle-1">{{ currentTrackInfo.title }} - {{ currentTrackInfo.artists[0] }}</span><br>
        <v-card-actions>
          <span class="subtitle-2 orange--text">{{ convertMsToMMSS(elapsedTime) }} - {{ convertMsToMMSS(currentTrackDuration) }} </span>
          <v-spacer></v-spacer>
          <v-chip :class="getUserChipData(currentTrackInfo).chipClass" outlined>
            <v-icon left>{{getUserChipData(currentTrackInfo).icon}}</v-icon>
            {{ getUserChipData(currentTrackInfo).user }}
          </v-chip>
        </v-card-actions>
      </v-card>
    </v-col>
    <v-col class="d-none d-md-block col-lg-5 col-12 col-sm-11 col-md-10">
      <v-card class="pa-2" outlined round v-if="nextTrackInfo">
        <span class="overline grey--text">VOLGEND NUMMER</span><br>
        <div v-if="playbackState !== 2 || secondaryTracksQueue.length === 0">
          <span class="subtitle-1">{{ nextTrackInfo.title }} - {{ nextTrackInfo.artists[0] }}</span><br>
          <v-card-actions>
            <span class="subtitle-2 orange--text">00:00 - {{ convertMsToMMSS(nextTrackDuration) }}</span>
            <v-spacer></v-spacer>
            <v-chip :class="getUserChipData(nextTrackInfo).chipClass" outlined>
              <v-icon left>{{getUserChipData(nextTrackInfo).icon}}</v-icon>
              {{ getUserChipData(nextTrackInfo).user }}
            </v-chip>
          </v-card-actions>
        </div>
        <div v-else>
          <span class="subtitle-1">Random van artist unkown </span><br>
          <v-card-actions>
            <span class="subtitle-2 orange--text">00:00 - ??:??</span>
            <v-spacer></v-spacer>
            <v-chip :class="getUserChipData(nextTrackInfo).chipClass" outlined>
              <v-icon left>{{getUserChipData(nextTrackInfo).icon}}</v-icon>
              {{ getUserChipData(nextTrackInfo).user }}
            </v-chip>
          </v-card-actions>
        </div>
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
        private elapsedTime: number | null = null;
        private timer: any = null;

        get playbackState():number | null{
          return this.$store.getters['playbackModule/getPlaybackState'];
        }
        
        getUserChipData(track:trackDto):object{
          if (track.trackType === 2) {
            return {
              user: track.user.displayName,
              chipClass: "orange orange--text",
              icon: 'mdi-account',
            }
          }
          if (track.trackType === 1){
            return {
              user: 'DJ',
              chipClass: "purple purple--text text--lighten-2",
              icon: 'mdi-account-music',
              }
          }
          else{
            return {
              user: "AutoDj",
              chipClass: "grey grey--text",
              icon: "mdi-robot",
            }
          }
        }
        
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
  
        get nextTrackInfo():trackDto | null{
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
        onCurrentTrackInfoChange(newValue:any){
          this.updateElapsedTime();
        }
      
        @Watch("nextTrackInfo")
        onNextTrackInfoChange(newValue:any){
          this.updateElapsedTime();
        }
        
        
        updateElapsedTime() : void {
          let now = new Date();

          if(this.currentTrackInfo){
              this.elapsedTime = now.getTime() - new Date(this.currentTrackStartingTime).getTime();
              if(this.timer == null){
                  this.timer = setInterval(() => {
                      if (this.elapsedTime){
                        this.elapsedTime += 1000;
                      }
                  }, 1000)
              }
          }
        } 

        convertMsToMMSS(ms:number) : string {
            let date = new Date(0);
            date.setMilliseconds(ms);

            return date.toISOString().substr(14, 5);
        }
    }
</script>
