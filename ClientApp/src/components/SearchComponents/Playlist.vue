<template>
  <div>
      <v-card>
        <v-card-title class="text-center">
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
        <div class="text-center">
          <v-btn class="ma-4" @click="togglePlaylistDialog">Afsluiten</v-btn>
        </div>
      </v-card>
  </div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import {Prop} from "vue-property-decorator";
import {AxiosResponse} from "axios";
import {alertInfo, trackDto, userPlaybackSettings} from "@/common/types";

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
    this.populateTracks();
  }
  
  private togglePlaylistDialog(){
    this.$store.commit('profileModule/TOGGLE_PLAYLIST_DIALOG');
  }

  private requestSong(track){
    this.$axios.put(`https://localhost:5001/api/playback/request/${track.id}`).then((response: AxiosResponse) => {
      this.togglePlaylistDialog();
      let alert : alertInfo = { type: "success", message: `${track.artist} - ${track.name} toegevoegd aan de wachtrij.` }
      this.$store.commit('alertModule/SET_ALERT', alert);
      this.$router.push('/');
    }).catch((error: any) => {
      let alert : alertInfo = { type: "error", message: error.response.data.message }
      this.$store.commit('alertModule/SET_ALERT', alert);
      this.$router.push('/');
    })
  }
  
  private populateTracks() {
    switch(this.playlistId){
      case "1":
        this.getTopTracksPlaylist(1);
        break;
      case "2":
        this.getTopTracksPlaylist(2);
        break;
      case "3":
        this.getTopTracksPlaylist(3);
        break;
      default:
        this.getPlaylist();
    }
  }

  private getPlaylist(){
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

  private getTopTracksPlaylist(term: number) {
    let termString: string = "short_term";
    
    switch(term) {
      case 1:
        termString = "short_term";
        break;
      case 2:
        termString = "medium_term";
        break;
      case 3:
        termString = "long_term";
        break;
    }
    
    this.$axios.get(`https://localhost:5001/api/playlist/top-tracks?term=${termString}`).then((trackResponse: AxiosResponse) => {
      trackResponse.data.items.forEach((track) => {
        this.tracks.push({
          name: track.name,
          artist: track.artists[0].name,
          album: track.album.name,
          id: track.id,
        });
        
        this.loading = false;
      })
    }).catch((error: any) => {
      console.log(error);
    })
  }
}
</script>