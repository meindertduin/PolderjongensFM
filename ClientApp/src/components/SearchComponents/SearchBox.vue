<template>
  <div>
        <v-alert v-if="userIsMod === false || this.$store.getters['userSettingsModule/getMakeRequestAsMod'] === false" 
                 type="info" colored-border border="left" color="orange">
          Je hebt momenteel {{userRequestedAmount.toString()}} / {{ maxRequestsPerUser }} van het maximaal aantal verzoekjes in de wachtrij staan.
        </v-alert>
        <v-card>
          <v-tabs
              v-model="tab"
              fixed-tabs
              dark
          >
            <v-tab>
              Mijn Spotify
            </v-tab>
            <v-tab>
              Zoeken
            </v-tab>
          </v-tabs>
          <v-tabs-items v-model="tab">
            <!-- refactor to seperate components -->
            <v-tab-item>
              <v-card flat>
                <div v-if="!loading">
                  <v-expansion-panels accordion class="mb-5">
                    <v-expansion-panel
                        v-for="(playlist, i) in this.playlists"
                        :key="i"
                    >
                      <v-expansion-panel-header @click="togglePlaylistDialog(playlist.id, playlist.name)">{{ i + 1 }}. {{ playlist.name }}</v-expansion-panel-header>
                    </v-expansion-panel>
                  </v-expansion-panels>
                </div>
                <div class="text-center" v-if="loading">
                  <v-progress-circular class="ma-4" :size="250" color="orange" indeterminate v-if="loading"></v-progress-circular>
                </div>
              </v-card>
            </v-tab-item>
            <!--  -->
            <!-- refactor to seperate components -->
            <v-tab-item>
              <v-card flat>
                <v-card-text>
                  <v-textarea 
                      outlined 
                      counter="100" 
                      v-model="requestMessage" 
                      :rules="[formRules.counterMax100]"
                      label="Verstuur een berichtje bij je verzoek">
                  </v-textarea>
                  <v-text-field prepend-icon="mdi-magnify" label="Zoek naar artiesten of nummers" v-on:keyup="searchBarKeyUp($event)" v-model="query"></v-text-field>

                  <v-list dense>
                    <v-list-item-group
                        color="primary"
                        v-model="selectedSearchTrackItem"
                    >
                      <v-list-item
                          v-for="(result, i) in searchResults"
                      >
                        <v-list-item-content @click="requestSong(result)">
                          {{i + 1}}. {{result.artists[0]}} - {{ result.title }}
                        </v-list-item-content>
                      </v-list-item>
                    </v-list-item-group>
                  </v-list>
                </v-card-text>
              </v-card>
            </v-tab-item>
            <!--  -->
          </v-tabs-items>
        </v-card>
    <template>
        <v-row justify="center">
          <v-dialog v-model="playlistDialogActive" persistent max-width="1200">
            <Playlist :key="activePlaylistId" :playlist-id="activePlaylistId" :playlist-name="activePlaylistName"/>
          </v-dialog>
        </v-row>
      </template>
  </div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import JQuery from 'jquery'
import {alertInfo, trackDto} from "@/common/types";
import {AxiosResponse} from "axios";
import {Watch} from "vue-property-decorator";
import PlayerTimeSelectComponent from "@/components/HomeComponents/PlayerTimeSelectComponent.vue";
import Playlist from "@/components/SearchComponents/Playlist.vue";
import {formRules} from '@/common/objects';
import {trackRequest} from "@/store/playbackModule";

// @ts-ignore
window.$ = JQuery

@Component({
  name: 'SearchBox',
  components: {
    Playlist,
  }
})
export default class SearchBox extends Vue {
  private query = '';
  private searchResults = [];
  private requestMessage:string = "";
  
  get searchResultsLength(){
    return this.searchResults.length;
  }
  
  get formRules() {
    return formRules;
  }
  
  @Watch("searchResultsLength")
  onSearchResultsChange(newValue:any){
    this.selectedSearchTrackItem = 0
  }
  
  
  private activePlaylistId: string | null = null
  private activePlaylistName: string | null = null
  
  private playlists: Array<any> = [];
  
  private loading = true;
  private tab = null;
  
  private selectedSearchTrackItem :number = 0;
  
  created() {
    this.fetchPlaylists().then(() => {
      this.loading = false;
    })
    
    window.addEventListener('keydown', this.registerKeyDownEvents);
  }

  beforeDestroy(){
    window.removeEventListener('keydown', this.registerKeyDownEvents);
  }
  
  private registerKeyDownEvents(e:any){
    if (e.key == 'ArrowUp' && this.selectedSearchTrackItem > 0){
      this.selectedSearchTrackItem -=1
    }
    if (e.key == 'ArrowDown' && this.searchResults.length > this.selectedSearchTrackItem){
      this.selectedSearchTrackItem += 1;
    }
    if (e.key == 'Enter' && this.searchResults.length > 0) {
      this.requestSong(this.searchResults[this.selectedSearchTrackItem])
    }
  }
  
  searchBarKeyUp(e:any) {
    clearTimeout($.data(this, 'timer'));

    if (e.keyCode == 13)
      this.search(true);
    else
      $(this).data('timer', setTimeout(this.search, 500));
  }

  private togglePlaylistDialog(playlistId: string, playlistName: string){
    this.$store.commit('profileModule/TOGGLE_PLAYLIST_DIALOG');
    this.activePlaylistId = playlistId;
    this.activePlaylistName = playlistName;
  }

  get userIsMod():boolean{
    return this.$store.getters["profileModule/isMod"]
  }
  
  get playlistDialogActive():boolean{
    return this.$store.getters['profileModule/isPlaylistDialogActive'];
  }

  get maxRequestsPerUser():number{
    return this.$store.getters['playbackModule/getMaxRequestsPerUser'];
  }

  get userRequestedAmount():number{
    return this.$store.getters['profileModule/userRequestedAmount'];
  }

  get userProfile(){
    return this.$store.getters['profileModule/userProfile'];
  }

  search(force:any) {
    if (!force && this.query.length < 3) return;
    this.loading = true;

    this.loading = true;

    // @ts-ignore
    this.$axios.post(process.env.VUE_APP_API_BASE_URL + `/api/playback/search`, {
      query: this.query,
      type: 'track'
    }).then((response: AxiosResponse) => {
      this.searchResults = response.data;
      this.loading = false;
    })
  }

  requestSong(track: trackDto) {
    if (this.requestMessage.length > 100) return;
    if (this.checkCertainSongs(track.id)){
      const payload: trackRequest = {
        trackId: track.id,
        message: this.requestMessage.length > 0? this.requestMessage : undefined,
      }
      
      this.requestMessage = "";

      this.$store.dispatch('playbackModule/requestTrack', payload)
          .then((response: AxiosResponse) => {
            let alert : alertInfo = { type: "success", message: `${track.artists[0]} - ${track.title} toegevoegd aan de wachtrij.` }
            this.$store.commit('alertModule/SET_ALERT', alert);
            this.$router.push('/');
          })
          .catch((error: any) => {
            let alert : alertInfo = { type: "error", message: error.response.data.message }
            this.$store.commit('alertModule/SET_ALERT', alert);
            this.$router.push('/');
          });
    }
  }
  
  checkCertainSongs(trackId:string):boolean{
    if (trackId === "4uLU6hMCjMI75M1A2tKUQC"){
      location.href = `${process.env.VUE_APP_API_BASE_URL}/easterEggs/rickRoll/index.html`
      return false;
    }
    return true;
  }

  private fetchPlaylists():Promise<void>{
    this.playlists.push({ id: "1", name: `${this.userProfile.displayName}'s Top 50 (vier weken)`})
    this.playlists.push({ id: "2", name: `${this.userProfile.displayName}'s Top 50 (zes maanden)`})
    this.playlists.push({ id: "3", name: `${this.userProfile.displayName}'s Top 50 (all-time)`})

    // @ts-ignore
    return this.$axios.get(process.env.VUE_APP_API_BASE_URL + `/api/playlist`).then((playlistResponse: AxiosResponse) => {
      playlistResponse.data.items.forEach((playlist:any) => {
        this.playlists.push({ id: playlist.id, name: playlist.name})
      })
      this.loading = false;
    }).catch((error: any) => {});
  }
}
</script>