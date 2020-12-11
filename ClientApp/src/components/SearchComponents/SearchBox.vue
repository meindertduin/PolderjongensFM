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
                        <v-expansion-panel-header>{{ i + 1 }}. {{ playlist.name }}</v-expansion-panel-header>
                        <v-expansion-panel-content>
                          <v-list dense>
                            <v-list-item-group>
                              <v-list-item v-for="(track, z) in playlist.tracks" @click="requestSong(track)">
                                <v-list-item-content>
                                  <v-list-item-title>
                                    {{z + 1}}. {{ track.artists[0].name }} - {{ track.name }} <span class="grey--text float-right">Track</span>
                                  </v-list-item-title>
                                </v-list-item-content>
                              </v-list-item>
                            </v-list-item-group>
                          </v-list>
                        </v-expansion-panel-content>
                      </v-expansion-panel>
                    </v-expansion-panels>
                  </div>
              </v-card>
            </v-tab-item>
            <!--  -->
          </v-tabs-items>
        </v-card>
  </div>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import JQuery from 'jquery'
import {trackDto} from "@/common/types";
import {AxiosResponse} from "axios";
import {Watch} from "vue-property-decorator";

window.$ = JQuery

@Component({
  name: 'SearchBox',
})
export default class SearchBox extends Vue {
  private query = '';
  private searchResults = [];
  
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
    await this.$axios.get(`https://localhost:5001/api/playlist/top-tracks?term=short_term`).then((response: AxiosResponse) => {
      this.playlists.push({name: "Mijn Top 50 (vier weken)", tracks: response.data.items})
    }).catch((error: any) => {
      console.log(error);
    })

    await this.$axios.get(`https://localhost:5001/api/playlist/top-tracks?term=medium_term`).then((response: AxiosResponse) => {
      this.playlists.push({name: "Mijn Top 50 (zes maanden)", tracks: response.data.items})
    }).catch((error: any) => {
      console.log(error);
    })
    
    await this.$axios.get(`https://localhost:5001/api/playlist/top-tracks?term=long_term`).then((response: AxiosResponse) => {
      this.playlists.push({name: "Mijn Top 50 (all-time)", tracks: response.data.items})
    }).catch((error: any) => {
      console.log(error);
    })

    await this.$axios.get(`https://localhost:5001/api/playlist`).then((playlistResponse: AxiosResponse) => {
      playlistResponse.data.items.forEach((playlist) => {
        this.$axios.get(`https://localhost:5001/api/playlist/tracks?playlistId=${playlist.id}`).then((trackResponse: AxiosResponse) => {
          var playlistTrackArray = [];
          
          trackResponse.data.items.forEach((track) => {
            playlistTrackArray.push(track.track);
          })

          this.playlists.push({name: playlist.name, tracks: playlistTrackArray})
        })
        }).catch((error: any) => {
          console.log(error);
        })
      }).catch((error: any) => {
      console.log(error);
    })
  }
}
</script>