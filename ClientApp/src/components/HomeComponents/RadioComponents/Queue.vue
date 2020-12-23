<template>
    <v-row>
      <v-col>
          <v-card 
              class="pa-2" 
              outlined 
              round
          >
              <div class="random-queue" v-if="playbackState === 2">
                <div v-if="queue.filter(x => x.queueNum === 0).length > 0">
                  <span class="overline grey--text">Prioriteit tracks</span><br>
                  <v-list dense>
                    <v-list-item-group>
                      <v-list-item
                          v-for="(item, i) in queue.filter(x => x.queueNum === 0).slice(0, 10)"
                          :key="i"
                      >
                        <v-list-item-content>
                          <v-list-item-title style="color: yellow">
                            {{item.track.artists[0]}} - {{item.track.title}} <span class="grey--text float-right">{{item.user}}</span>
                          </v-list-item-title>
                        </v-list-item-content>
                      </v-list-item>
                    </v-list-item-group>
                  </v-list>
                </div>
                
                <div>
                  <span class="overline grey--text">Verzoekjes-pool</span><br>
                  <v-list dense>
                    <v-list-item-group class="random-secondary-list">
                      <v-list-item
                          v-for="(item, i) in queue.filter(x => x.queueNum === 1)"
                          :key="i"
                      >
                        <v-list-item-content>
                          <v-list-item-title style="color: orange">
                            {{item.track.artists[0]}} - {{item.track.title}} <span class="grey--text float-right">{{item.user}}</span>
                          </v-list-item-title>
                        </v-list-item-content>
                      </v-list-item>
                      <v-list-item v-if="queue.filter(x => x.queueNum === 1).length <= 0">
                        <v-list-item-content>
                          <v-list-item-title style="color: orange">
                            De verzoekjes pool is op dit moment leeg... doe snel een verzoekje om hem te vullen!
                          </v-list-item-title>
                        </v-list-item-content>
                      </v-list-item>
                    </v-list-item-group>
                  </v-list>
                </div>
                <div>
                  <span class="overline grey--text">Filler watchtrij</span><br>
                  <v-list dense>
                    <v-list-item-group>
                      <v-list-item
                          v-for="(item, i) in queue.slice(0, 3)"
                          :key="i"
                      >
                        <v-list-item-content>
                          <v-list-item-title style="color: orangered">
                            {{item.track.artists[0]}} - {{item.track.title}} <span class="grey--text float-right">{{item.user}}</span>
                          </v-list-item-title>
                        </v-list-item-content>
                      </v-list-item>
                    </v-list-item-group>
                  </v-list>
                </div>
              </div>
            
            
            
              <div v-else class="normal-queue">
                <span class="overline grey--text">Wachtrij - {{playbackStateString}}</span><br>
                <v-list>
                  <v-list-item-group>
                    <v-list-item
                        v-for="(item, i) in queue.slice(0, 10)"
                        :key="i"
                    >
                      <v-list-item-content>
                        <v-list-item-title>
                            {{i + 1}}. {{item.track.artists[0]}} - {{item.track.title}}
                        </v-list-item-title>
                      </v-list-item-content>
                      <v-list-item-icon>
                        <v-chip
                            :class="item.chipClass"
                            outlined
                        >
                          <v-icon left>{{item.icon}}</v-icon>
                          {{item.user}}
                        </v-chip>
                      </v-list-item-icon>
                    </v-list-item>
                  </v-list-item-group>
                </v-list>
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

interface queueTrack {
  track: trackDto,
  user: string,
  icon: string,
  queueNum: number,
  chipClass: string,
}

@Component({
  name: 'Queue',
})
export default class Queue extends Vue {
  private queue: Array<queueTrack> = [];
  
  private modeColors = [
      "purple",
      "primary",
      "red",
  ]
  
  private queueTracksColors = [
      "yellow",
      "orange",
      "orangered"
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