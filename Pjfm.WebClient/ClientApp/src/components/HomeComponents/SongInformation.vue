<template>
    <v-row>
      <v-col>
        <v-card
          class="pa-2"
          outlined
          round
        >
          <span class="overline grey--text">HUIDIGE POKOE</span><br>
          <span class="subtitle-1">Rick Astley - Never Gonna Give You Up</span><br>
          <span class="subtitle-2 orange--text">01:13 - 03:45</span>
        </v-card>
      </v-col>
      <v-col>
       <v-card
          class="pa-2"
          outlined
          round
        >
          <span class="overline grey--text">VOLGENDE POKOE</span><br>
          <span class="subtitle-1">Slipknot - Spit It Out</span><br>
          <span class="subtitle-2 orange--text">00:00 - 04:20</span>
        </v-card>
      </v-col>
    </v-row>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'
    import {HubConnectionBuilder, TransferFormat, LogLevel, HubConnection} from "@microsoft/signalr";

    @Component({
        name: 'SongInformation',
    })
    export default class SongInformation extends Vue {

    }
</script>
