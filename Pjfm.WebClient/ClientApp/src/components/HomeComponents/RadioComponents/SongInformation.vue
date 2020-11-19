<template>
    <v-row>
      <v-col>
          <v-card
          class="pa-2"
          outlined
          round
          v-if="currentSongInfo"
          >
              <span class="overline grey--text">HUIDIGE POKOE</span><br>
              <span class="subtitle-1">{{ currentSongInfo.artist }} - {{ currentSongInfo.title }}</span><br>
              <span class="subtitle-2 orange--text">{{ convertMsToMMSS(elapsedTime) }} - {{ convertMsToMMSS(currentSongDuration) }} </span>
          </v-card>
      </v-col>
      <v-col>
        <v-card
          class="pa-2"
          outlined
          round
          v-if="nextSongInfo"
          >
          <span class="overline grey--text">VOLGENDE POKOE</span><br>
          <span class="subtitle-1">{{ nextSongInfo.artist }} - {{ nextSongInfo.title }}</span><br>
          <span class="subtitle-2 orange--text">00:00 - {{ convertMsToMMSS(nextSongDuration) }}</span>
        </v-card>
      </v-col>
    </v-row>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'
    import {Watch} from "vue-property-decorator";
    import {HubConnectionBuilder, TransferFormat, LogLevel, HubConnection} from "@microsoft/signalr";
    import {playerUpdateInfo} from "@/common/types";

    @Component({
        name: 'SongInformation',
        props: ['radioConnection'],
    })
    export default class SongInformation extends Vue {
        private currentSongInfo = null;
        private nextSongInfo = null;
        private elapsedTime = null;
        private timer = null;

        get currentSongDuration(){
            if(this.currentSongInfo != null){
                let duration = new Date(this.currentSongInfo.duration).getTime();
                return duration;
            }

            return 0;
        }

        get nextSongDuration(){
            if(this.nextSongInfo != null){
                console.log(this.nextSongInfo);
                let duration = new Date(this.nextSongInfo.duration == null ?  new Date(0).getTime() : this.nextSongInfo.duration).getTime();
                return duration;
            }

            return 0;
        }
        
        get playbackInfo():playerUpdateInfo{
            return this.$store.getters['playbackModule/getPlaybackInfo'];
        }

      


      @Watch('playbackInfo')
      updateRadio(){
          if (this.playbackInfo){
              this.currentSongInfo = {
                  artist: this.playbackInfo.currentPlayingTrack.artists[0],
                  title: this.playbackInfo.currentPlayingTrack.title,
                  startingTime: this.playbackInfo.startingTime,
                  duration: this.playbackInfo.currentPlayingTrack.songDurationMs
              }

              if((Array.isArray(this.playbackInfo.priorityQueuedTracks) && this.playbackInfo.priorityQueuedTracks.length)){
                  this.nextSongInfo = {
                      artist: this.playbackInfo.priorityQueuedTracks[0].artists[0],
                      title: this.playbackInfo.priorityQueuedTracks[0].title,
                      startingTime: this.playbackInfo.priorityQueuedTracks[0].startingTime,
                      duration: this.playbackInfo.priorityQueuedTracks.songDurationMs
                  }
              }else if((Array.isArray(this.playbackInfo.secondaryQueuedTracks) && this.playbackInfo.secondaryQueuedTracks.length)){
                  this.nextSongInfo = {
                      artist: this.playbackInfo.secondaryQueuedTracks[0].artists[0],
                      title: this.playbackInfo.secondaryQueuedTracks[0].title,
                      startingTime: this.playbackInfo.secondaryQueuedTracks[0].startingTime,
                      duration: this.playbackInfo.secondaryQueuedTracks[0].songDurationMs
                  }
              }else{
                  this.nextSongInfo = {
                      artist: this.playbackInfo.fillerQueuedTracks[0].artists[0],
                      title: this.playbackInfo.fillerQueuedTracks[0].title,
                      startingTime: this.playbackInfo.fillerQueuedTracks[0].startingTime,
                      duration: this.playbackInfo.fillerQueuedTracks[0].songDurationMs
                  }
              }

              this.updateElapsedTime();
          }
      }
        
      created() : void {
        this.updateRadio();
      }

      updateElapsedTime() : void {
            let now = new Date();

            if(this.currentSongInfo != null){
                let elapsed = now.getTime() - new Date(this.currentSongInfo.startingTime).getTime();
                this.elapsedTime = elapsed;
                
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
