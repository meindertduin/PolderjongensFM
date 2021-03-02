<template>
  <div>
    <div class="text-h6 my-3">Genre-browsing opties</div>
    <v-row justify="center">
      <v-col class="col-12">
        <v-combobox
        v-model="selectedTracks"
        chips
        clearable
        item-text="title"
        label="Geselecteerde tracks"
        multiple
        prepend-icon="mdi-filter-variant"
        solo
        outlined
        >
        <template v-slot:selection="{ attrs, item, select, selected }">
          <v-chip
              v-bind="attrs"
              :input-value="selected"
              close
              @click="select"
              @click:close="remove(item)"
          >
            <strong>{{ item.title }} : {{ item.artists.join(", ")}}</strong>&nbsp;
          </v-chip>
        </template>
        </v-combobox> 
      </v-col>
      <v-col class="col-12">
        <v-autocomplete
            v-model="selectedTrack"
            :items="tracks"
            :loading="isLoading"
            :search-input.sync="tracksQuery"
            color="white"
            hide-no-data
            hide-selected
            item-text="title"
            label="Zoek nummers seed"
            placeholder="Start typen om te zoeken"
            prepend-icon="mdi-magnify"
            return-object
            outlined
        ></v-autocomplete>
      </v-col>
    </v-row>
    <v-row justify="center">
      <v-col class="col-6">
        <v-autocomplete
            v-model="browserQueueSettings.genre"
            :loading="genresLoading"
            :items="genres"
            :search-input.sync="searchInput"
            cache-items
            class="mx-4"
            flat
            hide-no-data
            hide-details
            label="Genre"
            solo-inverted
            dark
            outlined
        ></v-autocomplete>
      </v-col>
      <v-col class="col-6">
        <v-select 
            v-model="browserQueueSettings.tempo"
            :items="selectableValues"
            label="Tempo"
            outlined
        ></v-select>  
      </v-col>
    </v-row>
    <v-row>
      <v-col class="col-6">
        <v-select
            v-model="browserQueueSettings.instrumentalness"
            :items="selectableValues"
            label="instrumenteelheid"
            outlined
        ></v-select>
      </v-col>
      <v-col class="col-6">
        <v-select
            v-model="browserQueueSettings.popularity"
            :items="selectableValues"
            label="populariteit"
            outlined
        ></v-select>
      </v-col>
    </v-row>
    <v-row>
      <v-col class="col-6">
        <v-select
            v-model="browserQueueSettings.energy"
            :items="selectableValues"
            label="Energie"
            outlined
        ></v-select>
      </v-col>
      <v-col class="col-6">
        <v-select
            v-model="browserQueueSettings.danceAbility"
            :items="selectableValues"
            label="Dansbaarheid"
            outlined
        ></v-select>
      </v-col>
    </v-row>
    <v-row>
      <v-col class="col-6">
        <v-select
            v-model="browserQueueSettings.valence"
            :items="selectableValues"
            label="Valenciteit"
            outlined
        ></v-select>
      </v-col>
      <v-col class="col-6">
        <div v-if="sendMessage !== null" class="orange--text">
          {{sendMessage}}
        </div>
        <v-btn @click="applySettings" width="100%" color="green">Opties toepassen</v-btn>
      </v-col>
    </v-row>
  </div>
</template>

<script lang="ts">
import Component from "vue-class-component";
import Vue from "vue";
import {browserQueueSettings, trackDto} from "@/common/types";
import {AxiosResponse} from "axios";
import {Watch} from "vue-property-decorator";

@Component({
  name: "GenreBrowsingOptionsDisplay"
})
export default class GenreBrowsingOptionsDisplay extends Vue {
    private selectableValues = [
      { text: "Maakt niet uit", value: 0 },
      { text: "Minimaal", value: 1 },
      { text: "Beetje", value: 2 },
      { text: "Gemiddeld", value: 3 },
      { text: "Veel", value: 4 },
      { text: "maximaal", value: 5 },
    ]
  
    private searchInput: string | null = null;
    private genres: string[] = [];
    private genresLoading: boolean = false;
    
    private tracksQuery: string | null = null;
    private selectedTrack: trackDto | null = null;
    
    @Watch("selectedTrack")
    onSelectedTrackChange(newValue: trackDto) {
      this.selectedTracks.push(newValue);
    }
    
    private selectedTracks: Array<trackDto> = [];
    
    created(){
      // @ts-ignore
      this.$axios.get("api/playback/mod/spotifyGenres")
          .then((response: AxiosResponse) => {
            this.genres = response.data.genres;
          });
    }
    
    private currentQueueSettings: browserQueueSettings | null = null;
    private sendMessage: string | null = null;
    private tracks: Array<trackDto> = [];
    private isLoading: boolean = false;
    
    get browserQueueSettings(): browserQueueSettings {
      const settings = this.$store.getters["modModule/getBrowserQueueSettings"];
      if (this.currentQueueSettings === null) {
        this.currentQueueSettings = settings;
      }
      return settings;
    }
    
    applySettings():void{
      if (this.currentQueueSettings === null) return; 
      this.browserQueueSettings.seedTracks = this.selectedTracks.map(track => track.id);
      this.browserQueueSettings.seedArtists = this.selectedTracks.map(track => track.mainArtistId);
      
      // @ts-ignore
      this.$axios.post("api/playback/mod/browserQueueSettings", this.browserQueueSettings)
        .then((response:AxiosResponse) => {
          if (response.status === 200) {
            this.sendMessage = "Insetllingen zijn toegepast";
          }
        })
      .catch(() => {
        this.sendMessage = "Iets ging fout bij het versturen van nieuwe instellingen";
      })
      .finally(() => {
        setTimeout(() => { this.sendMessage = null}, 5000);
      });
    }
    
    @Watch("tracksQuery")
    searchTracks(newValue: string):void {
      if (newValue.length <= 0) return;
      if (this.isLoading) return;

      this.isLoading = true;
     
      // @ts-ignore
      this.$axios.post(process.env.VUE_APP_API_BASE_URL + `/api/playback/search`, {
        query: newValue,
        type: 'track'
      }).then((response: AxiosResponse) => {
        this.tracks = response.data;
        this.tracks.forEach(track => track.title = track.title + ": " + track.artists.join(", "))
      }).finally(() => this.isLoading = false);
    }
    
    remove(track: trackDto) {
      this.selectedTracks.splice(this.selectedTracks.indexOf(track), 1);
      this.selectedTracks = [...this.selectedTracks];
    }
}
</script>

<style scoped>

</style>