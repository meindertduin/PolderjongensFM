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
                <v-list dense>
                  <v-list-item-group
                      class=""
                  >
                    <v-list-item>
                      <v-list-item-content>
                        <v-list-item-title>
                          1. Mijn Top 50 (vier weken) <span class="grey--text float-right">Playlist</span>
                        </v-list-item-title>
                      </v-list-item-content>
                    </v-list-item>
                    <v-list-item
                    >
                      <v-list-item-content>
                        <v-list-item-title>
                          2. Mijn Top 50 (zes maanden) <span class="grey--text float-right">Playlist</span>
                        </v-list-item-title>
                      </v-list-item-content>
                    </v-list-item>
                    <v-list-item
                    >
                      <v-list-item-content>
                        <v-list-item-title>
                          3. Mijn Top 50 (all-time) <span class="grey--text float-right">Playlist</span>
                        </v-list-item-title>
                      </v-list-item-content>
                    </v-list-item>
                  </v-list-item-group>
                </v-list>
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

window.$ = JQuery

@Component({
  name: 'SearchBox',
})
export default class SearchBox extends Vue { 
  private query = '';
  private searchResults = [];
  
  private shortTopTracks = [];
  private mediumTopTracks = [];
  private longTopTracks = [];
  
  private loading = false;
  private tab = null;
  
  created(){
    this.fetchTopTracks();
  }

  searchBarKeyUp(e){
    clearTimeout($.data(this, 'timer'));

    if (e.keyCode == 13)
      this.search(true);
    else
      $(this).data('timer', setTimeout(this.search, 500));
  }
  
  search(force){
    console.log('search');
    if (!force && this.query.length < 3) return;
    this.loading = true;
    this.items = []

    this.loading = true;
    
    this.$axios.post(`https://localhost:5001/api/playback/search`, {
      query: this.query,
      type: 'track'
    }).then((response:AxiosResponse) => {
      this.searchResults = response.data;
      this.loading = false;
    })
  }
  requestSong(track : trackDto) {
    this.$axios.put(`https://localhost:5001/api/playback/request/${track.id}`).then((response:AxiosResponse) => {
      this.$router.push('/');
    }).catch((error:any) => {
      console.log(error);
    })
  }

  fetchTopTracks() {
    this.$axios.get(`https://localhost:5001/api/playlist/top-tracks?term=short_term`).then((response:AxiosResponse) => {
      this.shortTopTracks = response.data;
      console.log(this.shortTopTracks);
    }).catch((error:any) => {
      console.log(error);
    })
   
    this.$axios.get(`https://localhost:5001/api/playlist/top-tracks?term=medium_term`).then((response:AxiosResponse) => {
      this.mediumTopTracks = response.data;
      console.log(this.mediumTopTracks);
    }).catch((error:any) => {
      console.log(error);
    })
    
    this.$axios.get(`https://localhost:5001/api/playlist/top-tracks?term=long_term`).then((response:AxiosResponse) => {
      this.longTopTracks = response.data;
      console.log(this.longTopTracks);
    }).catch((error:any) => {
      console.log(error);
    })
  }
}
</script>