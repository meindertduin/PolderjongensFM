<template>
    <v-row>
      <v-col>
          <v-card 
              class="pa-2" 
              outlined 
              round
          >             
              <span class="overline grey--text">Wachtrij</span><br>
                  <v-list dense>
                  <v-list-item-group
                    class=""
                  >
                    <v-list-item
                      v-for="(item, i) in queue.slice(0, 10)"
                      :key="i"
                    >
                      <v-list-item-content>
                        <v-list-item-title>
                          {{i + 1}}. {{item.track.artists[0]}} - {{item.track.title}} <span class="grey--text float-right">{{item.user}}</span>
                        </v-list-item-title>
                      </v-list-item-content>
                    </v-list-item>
                  </v-list-item-group>
                </v-list>
          </v-card>
      </v-col>
    </v-row>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'
    import {Watch} from "vue-property-decorator";
    import {HubConnectionBuilder, TransferFormat, LogLevel, HubConnection} from "@microsoft/signalr";
    import {djPlaybackInfo} from "@/common/types";

    @Component({
        name: 'Queue',
    })
    export default class Queue extends Vue { 
      private queue = [];

      created(){
        this.updateRadio();
      }

      get playbackInfo():djPlaybackInfo{
          return this.$store.getters['playbackModule/getPlaybackInfo'];
      }
      
      @Watch('playbackInfo')
      updateRadio(){
          if (this.playbackInfo){
              this.queue = [];
              this.playbackInfo.priorityQueuedTracks.forEach((track) => {
                  this.queue.push({
                      track: track,
                      user: 'DJ'
                  })
              })

              this.playbackInfo.secondaryQueuedTracks.forEach((track) => {
                  this.queue.push({
                      track: track,
                      user: track.user.displayName
                  })
              })

              this.playbackInfo.fillerQueuedTracks.forEach((track) => {
                  this.queue.push({
                      track: track,
                      user: 'AutoDJ'
                  })
              })

              console.log(this.queue);
          }
      }
    }
</script>
