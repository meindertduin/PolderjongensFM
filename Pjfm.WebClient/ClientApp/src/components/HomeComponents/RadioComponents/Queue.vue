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
                      v-for="(track, i) in queue.priority.slice(0, 10)"
                      :key="i"
                    >
                      <v-list-item-content>
                        <v-list-item-title>
                          {{i + 1}}. {{track.artists[0]}} - {{track.title}}
                        </v-list-item-title>
                      </v-list-item-content>
                    </v-list-item>
                    <v-list-item
                      v-for="(track, i) in queue.filler.slice(0, 10)"
                      :key="i"
                    >
                      <v-list-item-content>
                        <v-list-item-title>
                          {{i + 1}}. {{track.artists[0]}} - {{track.title}}
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

    @Component({
        name: 'Queue',
        props: ['radioConnection'],
    })
    export default class Queue extends Vue { 
      private queue = {
        filler: null,
        priority: null,
      };

      created(){
        this.updateRadio();
      }

      @Watch('radioConnection')
      updateRadio(){
        this.radioConnection?.on("ReceivePlayingTrackInfo", (trackInfo) => {
          console.log(trackInfo);
          this.queue.filler = trackInfo.fillerQueuedTracks;
          this.queue.priority = trackInfo.priorityQueuedTracks;
        })
      }
    }
</script>
