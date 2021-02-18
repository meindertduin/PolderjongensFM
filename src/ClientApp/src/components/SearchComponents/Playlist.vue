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
            v-model="selectedTracks"
            :footer-props="{ itemsPerPageOptions: [10]}"
            show-select
            :headers="computedHeaders"
            :items="tracks"
            :search="search"
            :loading="loading"
            loading-text="Laden..."
        ></v-data-table>
        <div class="text-center">
          <v-btn outlined color="orange" class="ma-4" @click="requestSelectedSongs">Aanvragen</v-btn>
          <v-btn outlined color="red" class="ma-4" @click="togglePlaylistDialog">Afsluiten</v-btn>
        </div>
      </v-card>
  </div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import {Prop, Watch} from "vue-property-decorator";
import {AxiosResponse} from "axios";
import {alertInfo, trackDto, userPlaybackSettings} from "@/common/types";

@Component({
  name: 'Playlist',
})
export default class Playlist extends Vue {
  private loading = true;
  
  private selectedTracks = [];
  
  @Watch("selectedTracks")
  private onSelectedTracksChange(newValue:any, oldValue:any){
    const isMod:boolean = this.$store.getters['profileModule/isMod'];
    
    if (!isMod && newValue.length > this.maxSelectedAmount){
      this.$nextTick(() => {
        this.selectedTracks = oldValue;
      })
    } else if(this.$store.getters['userSettingsModule/getMakeRequestAsMod'] && newValue.length > this.maxSelectedAmount) {
      this.selectedTracks = oldValue;
    }
  }
  
  get maxRequestsPerUser():number{
    return this.$store.getters['playbackModule/getMaxRequestsPerUser'];
  }

  get userRequestedAmount():number{
    return this.$store.getters['profileModule/userRequestedAmount'];
  }
  
  get maxSelectedAmount(){
    return this.maxRequestsPerUser - this.userRequestedAmount;
  }
  
  @Prop({type: String, required: true}) 
  readonly playlistId !: string

  @Prop({type: String, required: true})
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
  
  private tracks: Array<any> = [];

  created(){
    this.populateTracks();
    window.addEventListener('keydown', this.closePlaylistDialogKeyEvent);
  }
  
  beforeDestroy(){
    window.removeEventListener('keydown', this.closePlaylistDialogKeyEvent);
  }
  
  private closePlaylistDialogKeyEvent(e:any){
    if (e.key == 'Escape'){
      this.$store.commit('profileModule/SET_PLAYLIST_DIALOG', false);
    }
  }
  
  private togglePlaylistDialog(){
    this.$store.commit('profileModule/TOGGLE_PLAYLIST_DIALOG');
  }

  private async requestSong(track:any){
    let alert : alertInfo | null = null;

    this.$store.dispatch('playbackModule/requestTrack', {
      trackId: track.id,
      message: undefined,
    })
        .then((response: AxiosResponse) => {
          this.$store.commit('alertModule/SET_ALERT', 
              { type: "success", message: `${track.artist} - ${track.name} toegevoegd aan de wachtrij.` });
        })
        .catch((error: any) => {
          this.$store.commit('alertModule/SET_ALERT', 
              { type: "error", message: error.response.data.message });
        });
  }
  
  // TODO: Cleanup
  private requestSelectedSongs(){
    this.selectedTracks.forEach((track) => {
       this.requestSong(track);
    })
    
    this.togglePlaylistDialog();
    this.$router.push('/');
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
    // @ts-ignore
    this.$axios.get(process.env.VUE_APP_API_BASE_URL + `/api/playlist/tracks?playlistId=${this.playlistId}`).then((results: AxiosResponse) => {
      results.data.results.forEach((trackResponse:any) => {
        trackResponse.items.forEach((track:any) => {
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
    
    // @ts-ignore
    this.$axios.get(process.env.VUE_APP_API_BASE_URL + `/api/playlist/top-tracks?term=${termString}`).then((trackResponse: AxiosResponse) => {
      trackResponse.data.items.forEach((track:any) => {
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