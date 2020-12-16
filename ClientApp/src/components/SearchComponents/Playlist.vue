<template>
  <div>
      <v-card>
        <v-card-title class="text-center">
          <v-btn class="align-content-end" @click="togglePlaylistDialog">X</v-btn>
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
            :headers="computedHeaders"
            :items="tracks"
            :search="search"
            @click:row="requestSong"
            :loading="loading"
            loading-text="Laden..."
        ></v-data-table>
      </v-card>
  </div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import {Prop} from "vue-property-decorator";
import {AxiosResponse} from "axios";
import {trackDto} from "@/common/types";

@Component({
  name: 'Playlist',
})
export default class Playlist extends Vue {
  private loading = true;
  
  @Prop({type: Object, required: true}) 
  readonly playlistId !: string

  @Prop({type: Object, required: true})
  readonly playlistName !: string

  
  get computedHeaders(){
    return this.headers.filter((header) => {
      return header.value != 'id'
    })
  }
  
  private search:string|null = null;
  private headers = [
    { text: 'Titel', value: 'name' },
    { text: 'Artiest', value: 'artist' },
    { text: 'Album', value: 'album' },
    { text: 'Hidden', value: 'id' },
  ];
  private tracks = [];

  created(){
    this.tracks = [];
    this.populateTracks()
  }
  
  private togglePlaylistDialog(){
    this.$store.commit('profileModule/TOGGLE_PLAYLIST_DIALOG');
  }

  private requestSong(track){
    this.$axios.put(`https://localhost:5001/api/playback/request/${track.id}`).then((response: AxiosResponse) => {
      this.togglePlaylistDialog();
      this.$router.push('/');
    }).catch((error: any) => {
      console.log(error);
    })
  }
  
  private populateTracks() {
    this.$axios.get(`https://localhost:5001/api/playlist/tracks?playlistId=${this.playlistId}`).then((results: AxiosResponse) => {
      results.data.results.forEach((trackResponse) => {
        trackResponse.items.forEach((track) => {
          this.tracks.push({
            name: track.track.name,
            artist: track.track.artists[0].name,
            album: track.track.album.name,
            id: track.track.id,
          });
        })
      })
      
      this.loading = false;
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