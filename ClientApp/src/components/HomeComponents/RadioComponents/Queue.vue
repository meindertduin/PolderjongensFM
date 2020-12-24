<template>
    <v-row>
      <v-col>
          <v-card class="pa-2" outlined round>
              <div v-if="playbackState === 2">
                <div v-if="queue.filter(x => x.queueNum === 0).length > 0">
                  <QueueTracksList :span-title="`Watchrij ${playbackStateString}`" :tracks="queue.filter(track => track.queueNum === 0).slice(0, 10)"  
                                   empty-message="null"/>
                </div>
                <QueueTracksList :span-title="`Filler wachtrij`" :tracks="queue.filter(track => track.queueNum === 2).slice(0, 3)" 
                                 :empty-message="'De verzoekjes pool is op dit moment leeg... doe snel een verzoekje om hem te vullen!'" />
                <QueueTracksList :span-title="`Filler wachtrij`" :tracks="queue.filter(track => track.queueNum === 2).slice(0, 3)" 
                                 :empty-message="null" />
              </div>
              <div v-else>
                <QueueTracksList :span-title="`Wachtrij - ${playbackStateString}}`" :tracks="queue.slice(0, 10)" :empty-message="null" />
              </div>
          </v-card>
      </v-col>
    </v-row>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import {Watch} from "vue-property-decorator";
import {playbackState, trackDto, userPlaybackInfo} from "@/common/types";
import QueueTracksList from "@/components/HomeComponents/RadioComponents/QueueTracksList.vue";

interface queueTrack {
  track: trackDto,
  user: string,
  icon: string,
  queueNum: number,
  chipClass: string,
}

@Component({
  name: 'Queue',
  components: {
    QueueTracksList
  }
})
export default class Queue extends Vue {
  private queue: Array<queueTrack> = [];
  
  private modeColors = [
      "purple",
      "primary",
      "red",
  ]
  
  private modeChipColor :string = this.modeColors[0]    

  created(){
    this.updateRadio();
  }

  get playbackInfo():userPlaybackInfo{
      return this.$store.getters['playbackModule/getPlaybackInfo'];
  }
  
  
  get playbackState():playbackState | null{
    return this.$store.getters['playbackModule/getPlaybackState'];
  }
  
  get playbackStateString():string{
    const state:playbackState | null = this.playbackState;
    if (state === null) return "Playback mode";
    switch (state){
      case 0:
        this.modeChipColor = this.modeColors[0]
        return "DJ Only";
      case 1:
        this.modeChipColor = this.modeColors[1]
        return "Verzoekjes aan"
      case 2:
        this.modeChipColor = this.modeColors[2]
        return "Random verzoekjes aan"
    }
    return "Dj Only";
  } 
  
  @Watch('playbackInfo')
  updateRadio(){
      if (this.playbackInfo){
          this.queue = [];
          this.playbackInfo.priorityQueuedTracks.forEach((track) => {
              this.queue.push({
                  track: track,
                  user: 'DJ',
                  queueNum: 0,
                  chipClass: "purple purple--text text--lighten-2",
                  icon: 'mdi-account-music',
              })
          })
        
          this.playbackInfo.secondaryQueuedTracks.forEach((track) => {
              this.queue.push({
                  track: track,
                  user: track.user.displayName,
                  queueNum: 1,
                  chipClass: "orange orange--text",
                  icon: 'mdi-account',
              })
          })

          this.playbackInfo.fillerQueuedTracks.forEach((track) => {
              this.queue.push({
                  track: track,
                  user: 'AutoDJ',
                  queueNum: 2,
                  chipClass: "grey grey--text",
                  icon: 'mdi-robot',
              })
          })

          console.log(this.queue);
      }
  }
    }
</script>
<style scoped>
  .random-secondary-list {
    max-height: 400px;
    overflow: auto;
  }
</style>