<template>
  <div>
    <div class="text-h6 my-3">Genre-browsing opties</div>
    
    <div :class="`${currentSeedAmount <= 5? 'orange--text': 'red--text'}`">{{ currentSeedAmount }}/5 seeds gebruikt</div>
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
        readonly
        outlined
        >
        <template v-slot:selection="{ attrs, item, select, selected }">
          <v-chip
              v-bind="attrs"
              :input-value="selected"
              close
              @click="select"
              @click:close="removeTrack(item)"
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
        <v-combobox
            v-model="selectedGenres"
            chips
            clearable
            label="Geselecteerde genres"
            multiple
            prepend-icon="mdi-filter-variant"
            solo
            readonly
            outlined
        >
          <template v-slot:selection="{ attrs, item, select, selected }">
            <v-chip
                v-bind="attrs"
                :input-value="selected"
                close
                @click="select"
                @click:close="removeGenre(item)"
            >
              <strong>{{ item }}</strong>&nbsp;
            </v-chip>
          </template>
        </v-combobox>
      </v-col>
      <v-col class="col-6">
        <v-autocomplete
            v-model="selectedGenre"
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
            prepend-icon="mdi-magnify"
            dark
            outlined
        ></v-autocomplete>
      </v-col>
    </v-row>
    <v-row justify="center">
      <v-col class="col-6">
        <v-select
            v-model="browserQueueSettings.valence"
            :items="selectableValues"
            label="Valenciteit"
            outlined
        ></v-select>
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
      <v-col class="col-12">
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
    private selectedTracks: Array<trackDto> = [];
    
    @Watch("selectedTrack")
    onSelectedTrackChange(newValue: trackDto) {
      if (!this.selectedTracks.includes(newValue)) {
        this.selectedTracks.push(newValue);
      }
    }
    
    private selectedGenre: string | null = null;
    private selectedGenres: string[] = [];

    @Watch("selectedGenre")
    onSelectedGenreChange(newValue: string) {
      if(!this.selectedGenres.includes(newValue)) {
        this.selectedGenres.push(newValue);
      }
    }
  
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
      const settings: browserQueueSettings = this.$store.getters["modModule/getBrowserQueueSettings"];
      if (this.currentQueueSettings === null) {
        this.currentQueueSettings = settings;
        this.selectedGenres = settings.genres;
        
        if (settings.seedTracks.length > 0){
          // @ts-ignore
          this.$axios
              .get(`track/multiple?trackIds=${settings.seedTracks.join(",")}`)
              .then((response: AxiosResponse) => this.selectedTracks = response.data);
        }
      }
      return settings;
    }
    
    get currentSeedAmount():number {
      return this.selectedTracks.length + this.selectedGenres.length;
    }
    
    applySettings():void{
      if (this.currentQueueSettings === null) return;
      if (this.currentSeedAmount > 5) return;

      this.browserQueueSettings.genres = this.selectedGenres;
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
    
    removeTrack(track: trackDto) {
      this.selectedTracks.splice(this.selectedTracks.indexOf(track), 1);
      this.selectedTracks = [...this.selectedTracks];
    }
    
   removeGenre(genre: string) {
      this.selectedGenres = this.selectedGenres.filter((g: string) => g !== genre);
    }
}
</script>

<style scoped>

</style>