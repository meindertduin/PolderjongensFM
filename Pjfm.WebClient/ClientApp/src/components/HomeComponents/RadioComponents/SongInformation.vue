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
              <span class="subtitle-2 orange--text">{{ convertMsToMMSS(elapsedTime) }} - {{ convertMsToMMSS(songDuration) }} </span>
          </v-card>
      </v-col>
      <v-col>
        <v-card
          class="pa-2"
          outlined
          round
          >
          <span class="overline grey--text">VOLGENDE POKOE</span><br>
          <span class="subtitle-1">Slipknot - Spit It Out</span><br>
          <span class="subtitle-2 orange--text">00:00 - 04:20</span>
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
      private elapsedTime = null;
      private timer = null;

      get songDuration(){
          if(this.currentSongInfo != null){
              let duration = new Date(this.currentSongInfo.duration).getTime();
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
