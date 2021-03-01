<template>
  <div>
    <div class="text-h6 my-3">Genre-browsing opties</div>
    <v-row justify="center">
      <v-col class="col-6">
        <v-select label="Genre" outlined></v-select>
      </v-col>
      <v-col class="col-6">
        <v-select 
            v-model="tempo"
            :items="selectableValues"
            label="Tempo"
            outlined
        ></v-select>  
      </v-col>
    </v-row>
    <v-row>
      <v-col class="col-6">
        <v-select
            v-model="instrumentalness"
            :items="selectableValues"
            label="instrumenteelheid"
            outlined
        ></v-select>
      </v-col>
      <v-col class="col-6">
        <v-select
            v-model="popularity"
            :items="selectableValues"
            label="populariteit"
            outlined
        ></v-select>
      </v-col>
    </v-row>
    <v-row>
      <v-col class="col-6">
        <v-select
            v-model="energy"
            :items="selectableValues"
            label="Energie"
            outlined
        ></v-select>
      </v-col>
      <v-col class="col-6">
        <v-select
            v-model="danceAbility"
            :items="selectableValues"
            label="Dansbaarheid"
            outlined
        ></v-select>
      </v-col>
    </v-row>
    <v-row>
      <v-col class="col-6">
        <v-select
            v-model="valence"
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
  
    private tempo: number | null = null;
    private instrumentalness: number | null = null;
    private popularity: number | null = null;
    private energy: number | null = null;
    private danceAbility: number | null = null;
    private valence: number | null = null;
}
</script>

<style scoped>

</style>