<template>
    <v-row justify="center">
      <v-col class="col-12 col-sm-11 col-md-10 col-lg-10">
          <v-card class="pa-2" outlined round>
              <div class="body-2 orange--text text-end">
                momenteel {{listenersCount}} luisteraars
              </div>
              <div v-if="playbackState === 2">
                <div v-if="queue.filter(x => x.queueNum === 0).length > 0">
                  <QueueTracksList :span-title="`Dj wachtrij`" :tracks="queue.filter(track => track.queueNum === 0).slice(0, 10)"  
                                   empty-message="null"/>
                </div>
                <QueueTracksList :span-title="`verzoekjes pool - Random Verzoekjes`" :tracks="queue.filter(track => track.queueNum === 1)" 
                                 :empty-message="'De verzoekjes pool is op dit moment leeg... doe snel een verzoekje om hem te vullen!'" />
                <QueueTracksList :span-title="`Filler wachtrij`" :tracks="queue.filter(track => track.queueNum === 2).slice(0, 3)" 
                                 :empty-message="null" />
              </div>
              <div v-else>
                <QueueTracksList :span-title="`Wachtrij - ${playbackStateString}`" :tracks="queue.slice(0, 10)" :empty-message="null" />
              </div>
          </v-card>
      </v-col>
    </v-row>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import {Watch} from "vue-property-decorator";
import {playbackState, queueTrack, trackDto, userPlaybackInfo} from "@/common/types";
import QueueTracksList from "@/components/HomeComponents/RadioComponents/QueueTracksList.vue";

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
      "orange",
  ]
  
  private modeChipColor :string = this.modeColors[0]    

  created(){
    this.updateRadio();
  }

  get playbackInfo():userPlaybackInfo{
      return this.$store.getters['playbackModule/getPlaybackInfo'];
  }
  
  get listenersCount():number {
    return this.$store.getters['playbackModule/getListenersCount'];
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
      case 3:
        this.modeChipColor = this.modeColors[3];
        return "Verdeelde verzoekjes aan";
    }
    return "Dj Only";
  } 
  
  @Watch('playbackInfo')
  updateRadio(){
      if (this.playbackInfo){
          this.queue = [];
          this.playbackInfo.priorityQueuedTracks.forEach((track) => {
              this.queue.push({
                  id: track.id,
                  track: track,
                  user: 'DJ',
                  queueNum: 0,
                  chipClass: "purple purple--text text--lighten-2",
                  icon: 'mdi-account-music',
                  message: track.message?? undefined,
              })
          })
        
          this.playbackInfo.secondaryQueuedTracks.forEach((track) => {
              this.queue.push({
                  id: track.id,
                  track: track,
                  user: track.user.displayName,
                  queueNum: 1,
                  chipClass: "orange orange--text",
                  icon: 'mdi-account',
                  message: track.message?? undefined,
              })
          })

          this.playbackInfo.fillerQueuedTracks.forEach((track) => {
              this.queue.push({
                  id: track.id,
                  track: track,
                  user: 'AutoDJ',
                  queueNum: 2,
                  chipClass: "grey grey--text",
                  icon: 'mdi-robot',
                  message: undefined,
              })
          })
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