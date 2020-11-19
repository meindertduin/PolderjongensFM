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



      @Watch('radioConnection')
      updateRadio(){
        this.radioConnection?.on("ReceivePlayingTrackInfo", (trackInfo) => {
          this.currentSongInfo = {
              artist: trackInfo.currentPlayingTrack.artists[0],
              title: trackInfo.currentPlayingTrack.title,
              startingTime: trackInfo.startingTime,
              duration: trackInfo.currentPlayingTrack.songDurationMs
          }
          
          if((Array.isArray(trackInfo.priorityQueuedTracks) && trackInfo.priorityQueuedTracks.length)){
              this.nextSongInfo = {
                artist: trackInfo.priorityQueuedTracks[0].artists[0],
                title: trackInfo.priorityQueuedTracks[0].title,
                startingTime: trackInfo.priorityQueuedTracks[0].startingTime,
                duration: trackInfo.priorityQueuedTracks.songDurationMs
              }  
          }else if((Array.isArray(trackInfo.secondaryQueuedTracks) && trackInfo.secondaryQueuedTracks.length)){
              this.nextSongInfo = {
                artist: trackInfo.secondaryQueuedTracks[0].artists[0],
                title: trackInfo.secondaryQueuedTracks[0].title,
                startingTime: trackInfo.secondaryQueuedTracks[0].startingTime,
                duration: trackInfo.secondaryQueuedTracks[0].songDurationMs
              }
          }else{
              this.nextSongInfo = {
                artist: trackInfo.fillerQueuedTracks[0].artists[0],
                title: trackInfo.fillerQueuedTracks[0].title,
                startingTime: trackInfo.fillerQueuedTracks[0].startingTime,
                duration: trackInfo.fillerQueuedTracks[0].songDurationMs
              }
          }
          
          this.updateElapsedTime();
        })
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
