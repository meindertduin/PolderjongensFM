<template>
  <div>
        <v-card>
          <v-tabs
              v-model="tab"
              fixed-tabs
              background-color="primary"
              dark
          >
            <v-tab>
              Zoeken
            </v-tab>
            <v-tab>
              Mijn Spotify
            </v-tab>
          </v-tabs>
          <v-tabs-items v-model="tab">
            <!-- refactor to seperate components -->
            <v-tab-item>
              <v-card flat>
                <v-card-text>
                  <v-text-field prepend-icon="mdi-magnify" label="Zoek naar artiesten of nummers" v-on:keyup="searchBarKeyUp($event)" v-model="query"></v-text-field>

                  <v-list dense>
                    <v-list-item-group
                        color="primary"
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
import {trackDto} from "@/common/types";
import {AxiosResponse} from "axios";
import {Watch} from "vue-property-decorator";
import PlayerTimeSelectComponent from "@/components/HomeComponents/PlayerTimeSelectComponent.vue";
import Playlist from "@/components/SearchComponents/Playlist.vue";

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
  private activePlaylistId: string | null = null
  private activePlaylistName: string | null = null
  
  private playlists = [];
  
  private loading = true;
  private tab = null;
  
  created() {
    this.loading = true;
    
    this.fetchPlaylists().then(() => {
      this.loading = false;
    })
  }

  searchBarKeyUp(e) {
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

  get playlistDialogActive():boolean{
    return this.$store.getters['profileModule/isPlaylistDialogActive'];
  }

  search(force) {
    console.log('search');
    if (!force && this.query.length < 3) return;
    this.loading = true;
    this.items = []

    this.loading = true;

    this.$axios.post(`https://localhost:5001/api/playback/search`, {
      query: this.query,
      type: 'track'
    }).then((response: AxiosResponse) => {
      this.searchResults = response.data;
      this.loading = false;
    })
  }

  requestSong(track: trackDto) {
    this.$axios.put(`https://localhost:5001/api/playback/request/${track.id}`).then((response: AxiosResponse) => {
      this.$router.push('/');
    }).catch((error: any) => {
      console.log(error);
    })
  }

  async fetchPlaylists(){
    this.$axios.get(`https://localhost:5001/api/playlist`).then((playlistResponse: AxiosResponse) => {
      playlistResponse.data.items.forEach((playlist) => {
        this.playlists.push({ id: playlist.id, name: playlist.name})
      })
    }).catch((error: any) => {
      console.log(error);
    })
  }
}
</script>