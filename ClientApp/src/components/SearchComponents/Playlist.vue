<template>
  <div>
      <v-card>
        <v-card-title class="text-center">
          <span class="overline orange--text">{{ playlistName }}</span><v-btn class="align-content-end" @click="togglePlaylistDialog">X</v-btn>
        </v-card-title>
              <v-text-field
                  v-model="search"
                  append-icon="mdi-magnify"
                  label="Search"
                  class="ma-2"
                  single-line
                  hide-details
              ></v-text-field>
        <v-data-table
            :headers="headers"
            :items="tracks"
            :search="search"
        ></v-data-table>
      </v-card>
  </div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import {Prop} from "vue-property-decorator";
import {AxiosResponse} from "axios";

@Component({
  name: 'Playlist',
})
export default class Playlist extends Vue {
  @Prop({type: Object, required: true}) 
  readonly playlistId !: string

  @Prop({type: Object, required: true})
  readonly playlistName !: string
  
  private search:string|null = null;
  private headers = [
    { text: 'Titel', value: 'name' },
    { text: 'Artiest', value: 'artist' },
    { text: 'Album', value: 'album' },
    { text: '', value: 'duration' },
  ];
  private tracks = [];

  created(){
    this.tracks = [];
    
    this.populateTracks()
  }
  
  private togglePlaylistDialog(){
    this.$store.commit('profileModule/TOGGLE_PLAYLIST_DIALOG');
  }
  
  private populateTracks() {
    this.$axios.get(`https://localhost:5001/api/playlist/tracks?playlistId=${this.playlistId}`).then((results: AxiosResponse) => {
      results.data.results.forEach((trackResponse) => {
        trackResponse.items.forEach((track) => {
          this.tracks.push({
            name: track.track.name,
            artist: track.track.artists[0].name,
            album: track.track.album.name,
            duration: this.convertMsToMMSS(track.track.duration_ms),
          });
        })
      })
    }).catch((error: any) => {
      console.log(error);
    })
  }

  private convertMsToMMSS(ms) : string {
    let date = new Date(null);
    date.setMilliseconds(ms);

    return date.toISOString().substr(14, 5);
  }
}
</script>