<template>
      <v-card
          class="pa-2"
          outlined
          round
      >
          <span class="overline grey--text">Zoeken</span><br>
          <v-text-field prepend-icon="mdi-magnify" label="Zoeken" v-on:keyup="searchBarKeyUp($event)" v-model="query"></v-text-field>
          <v-divider></v-divider>
        <span class="overline grey--text">Resultaten</span><br>
        <div class="text-center">
          <v-progress-circular :size="250" color="orange" indeterminate v-if="loading"></v-progress-circular>
        </div>
        <v-list dense v-if="results.length > 0 && !loading">
          <v-list-item-group
              class=""
          >
            <v-list-item
                v-for="(item, i) in results"
                :key="i"
                @click="requestSong(item)"
            >
              <v-list-item-content>
                <v-list-item-title>
                  {{i + 1}}. {{item.artists[0]}} - {{item.title}}
                </v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </v-list-item-group>
        </v-list>
      </v-card>
</template>

<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component'
import JQuery from 'jquery'
import {trackDto} from "@/common/types";

import Axios from 'axios';
window.$ = JQuery

@Component({
  name: 'SearchBox',
})
export default class SearchBox extends Vue { 
  private query = '';
  private results = [];
  private loading = false;

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
    
    Axios.post(`https://localhost:5001/api/playback/search`, {
      query: this.query,
      type: 'track'
    }).then((response) => {
      this.results = response.data;
      console.log(response.data);
    }).catch(() => {})
    .then(() => {
      this.loading = false;
    })
  }
  requestSong(track : trackDto) {
    Axios.put(`https://localhost:5001/api/playback/mod/request/${track.id}`).then((response) => {
      this.$router.push('/');
    }).catch((error) => {
      console.log(error);
    })
  }
}
</script>