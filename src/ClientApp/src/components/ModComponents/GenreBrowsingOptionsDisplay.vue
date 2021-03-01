<template>
  <div>
    <div class="text-h6 my-3">Genre-browsing opties</div>
    <v-row justify="center">
      <v-col class="col-6">
        <v-select label="Genre" outlined></v-select>
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
        <v-btn width="100%" color="green">Opties toepassen</v-btn>
      </v-col>
    </v-row>
  </div>
</template>

<script lang="ts">
import Component from "vue-class-component";
import Vue from "vue";
import {browserQueueSettings} from "@/common/types";

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
  
    private currentQueueSettings: browserQueueSettings | null = null;
    
    get browserQueueSettings(): browserQueueSettings {
      const settings = this.$store.getters["modModule/getBrowserQueueSettings"];
      if (this.currentQueueSettings === null) {
        this.currentQueueSettings = settings;
      }
      return settings;
    }
}
</script>

<style scoped>

</style>