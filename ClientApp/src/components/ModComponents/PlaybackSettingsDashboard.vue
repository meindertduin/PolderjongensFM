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
                                    :value="selectedTerm"
                                    :max="5"
                                    step="1"
                                    ticks="always"
                                    tick-size="4"
                            ></v-slider>
                        </div>
                        <div class="text-h6 ma-2">Staat</div>
                        <v-select :items="stateItems" v-model="selectedState" outlined label="Playback staat"></v-select>
                    </v-card>
                </v-col>
            </v-row>
            <v-row>
                <v-col class="col-12">
                    <div class="switch-container">
                        <v-switch
                                @click="showConfirmNotification = true"
                                class="ma-2"
                                v-model="playbackOn"
                                label="Playback aan/uit"
                                color="red"
                                :value="playbackOn"
                                hide-details
                        ></v-switch>
                        <v-snackbar v-model="showConfirmNotification">
                            <template v-slot:action="{ attrs }">
                                <v-row justify="center">
                                    <v-btn
                                            color="green"
                                            text
                                            v-bind="attrs"
                                            @click="handleConfirmPlaybackSet">
                                        Confirm
                                    </v-btn>
                                    <v-btn
                                            color="white"
                                            text
                                            v-bind="attrs"
                                            @click="handleRejectPlaybackSet">
                                        Reject
                                    </v-btn>
                                </v-row>
                            </template>
                        </v-snackbar>
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
                    <v-btn class="ma-2" large width="100%" @click="handleReset">
                        Reset Playback
                    </v-btn>
                </v-col>
                <v-col class="col-6">
                    <v-btn class="ma-2" large width="100%" @click="handleSkip">
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
    import {playbackSettings, playbackState} from "@/common/types";
    import {Watch} from "vue-property-decorator";

    @Component({
        name: "PlaybackSettingsDashboard",
    })
    export default class PlaybackSettingsDashboard extends Vue{
        private playbackOn: boolean | null = null;
        private selectedTerm: number = 0;
        private terms :any[] = ['short', 'short-med', 'med', 'med-long', 'long', 'all'];
        private stateItems :any[] = [{text: 'Dj-mode', value: 0}, {text: 'wachtrij-mode', value: 1}, {text: 'random-mode', value: 2}]
        
        private showConfirmNotification :boolean = false;
        
        private selectedState :any | null = null;
        
        @Watch("selectedState")
        onSelectedStateChanged(newValue, oldValue){
            this.$axios.put(`api/playback/mod/setPlaybackState?playbackState=${this.selectedState}`)
                .catch((err:any) => console.log(err));
        }
        
        private loadedPlaybackSettings: playbackSettings | null = null;
        
        created(){
            this.$axios.get('api/playback/mod/playbackSettings')
                .then(({ data }: { data: playbackSettings}) => {
                    this.playbackOn = data.isPlaying? data.isPlaying : null;
                    this.selectedTerm = data.playbackTermFilter;
                    
                    switch (data.playbackState) {
                        case playbackState["Dj-mode"]:
                            this.selectedState = this.stateItems[0].value;
                            break;
                        case playbackState["wachtrij-mode"]:
                            this.selectedState = this.stateItems[1].value;
                            break;
                        case playbackState["random-mode"]:
                            this.selectedState = this.stateItems[2].value;
                            break;
                    }
                })
                .catch((err:any) => console.log(err));
        }
        
        
        async handleConfirmPlaybackSet(){
            this.showConfirmNotification = false;
            try {
                if (this.playbackOn){
                    await this.$axios.put('api/playback/mod/on');
                }
                else{
                    await this.$axios.put('api/playback/mod/off');
                }
            }
            catch (e) {
                console.log(e.message);
                this.playbackOn = this.playbackOn? null: true;
            }
        }
        
        async handleRejectPlaybackSet(){
            this.playbackOn = this.playbackOn? null: true;
            this.showConfirmNotification = false;
        }
        
        
        async handleReset(){
            await this.$axios.put(`api/playback/mod/setTerm?term=${this.selectedTerm}`);
            
            await this.$axios.put('api/playback/mod/reset');
        }
        
        handleSkip(){
            this.$axios.put('api/playback/mod/skip');
        }
        
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