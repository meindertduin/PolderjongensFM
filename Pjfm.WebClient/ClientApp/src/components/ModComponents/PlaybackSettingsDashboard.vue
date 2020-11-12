<template>
    <v-card 
            class="pa-2"
            outlined
            round>
        <v-card-title>
            Mod panel
        </v-card-title>
        <v-card-text>
            <v-row>
                <v-col>
                    <v-card class="pa-2">
                        <div class="selects-container">
                            <div class="text-h6 ma-2">Termijn</div>
                            <v-slider
                                    v-model="selectedTerm"
                                    :tick-labels="terms"
                                    :max="5"
                                    step="1"
                                    ticks="always"
                                    tick-size="4"
                            ></v-slider>
                        </div>
                        <div class="text-h6 ma-2">Staat</div>
                        <v-select :items="stateItems" v-model="selectedState" outlined label="Playback staat">

                        </v-select>
                        <v-card-actions>
                            <v-btn class="ma-2" color="green" width="100%">Update Playback</v-btn>
                        </v-card-actions>
                    </v-card>
                </v-col>
            </v-row>
            <v-row>
                <v-col class="col-12">
                    <div class="switch-container">
                        <v-switch
                                class="ma-2"
                                v-model="playbackOn"
                                label="Playback aan/uit"
                                color="red"
                                value="red"
                                hide-details
                        ></v-switch>
                    </div>
                </v-col>
                <v-divider dark></v-divider>
            </v-row>
            <v-row justify="center">
                <v-col class="col-12">
                </v-col>
            </v-row>
            <v-row>
                <v-col class="col-6">
                    <v-btn class="ma-2" large width="100%">
                        Reset Playback
                    </v-btn>
                </v-col>
                <v-col class="col-6">
                    <v-btn class="ma-2" large width="100%">
                        Skip nummer
                    </v-btn>
                </v-col>
            </v-row>
        </v-card-text>
    </v-card>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from "vue-class-component";
    
    @Component({
        name: "PlaybackSettingsDashboard",
    })
    export default class PlaybackSettingsDashboard extends Vue{
        private playbackOn: boolean = false;
        private selectedTerm: number = 0;
        private terms :string[] = ['short', 'short-med', 'med', 'med-long', 'long', 'all'];
        private stateItems :string[] = ['Dj-mode', 'wachtrij-mode','random-mode']
        private selectedState :string | null = null;
    }
        
</script>

<style scoped>
    .switch-container{
        display: flex;
        flex-direction: row;
        flex-wrap: wrap;
    }
    .selects-container{
        
    }
</style>