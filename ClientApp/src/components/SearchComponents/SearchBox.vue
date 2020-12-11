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
            <v-tab-item>
              <v-card flat>
                <v-card-text>
                  <v-text-field prepend-icon="mdi-magnify" label="Zoek naar artiesten of nummers" v-on:keyup="searchBarKeyUp($event)" v-model="query"></v-text-field>

                  <v-list dense>
                    <v-list-item-group
                        color="primary"
                    >
                      <v-list-item
                          v-for="(result, i) in results"
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
            <v-tab-item>
              <v-card flat>
                <v-card-text>Niet zoeken</v-card-text>
              </v-card>
            </v-tab-item>
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
  private results = [];
  private loading = false;
  private tab = null;
  private items = [
    { tab: 'Zoeken', content: 'Tab 1 Content' },
    { tab: 'Mijn Spotify', content: 'Tab 2 Content' },
  ];

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
    
    this.$axios.post(`https://localhost:5001/api/playback/search`, {
      query: this.query,
      type: 'track'
    }).then((response:AxiosResponse) => {
      this.results = response.data;
      console.log(response.data);
    }).catch(() => {})
    .then(() => {
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
}
</script>