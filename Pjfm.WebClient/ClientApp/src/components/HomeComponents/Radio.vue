<template>
    <div>
        <v-row>
        <v-col>
            <v-btn block class="orange" v-if="oidcAuthenticated" @click="connect">
                <span class="" v-if="radioConnection">Synchroniseren</span>
                <span class="" v-else>Klik hier om te verbinden</span>
            </v-btn>
        </v-col>
        </v-row>
        <v-row>
        <v-col>
            <v-card
            class="pa-2"
            outlined
            round
            v-if="currentSongInfo"
            >
                <span class="overline grey--text">HUIDIGE POKOE</span><br>
                <span class="subtitle-1">{{ currentSongInfo.artist }} - {{ currentSongInfo.title }}</span><br>
                <span class="subtitle-2 orange--text">{{ convertMsToMMSS(elapsedTime) }} - {{ convertMsToMMSS(songDuration) }} </span>
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
        </div>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'
    import {HubConnectionBuilder, TransferFormat, LogLevel, HubConnection} from "@microsoft/signalr";

    @Component({
        name: 'Radio',
    })
    export default class Radio extends Vue {
        private currentSongInfo = null;
        private elapsedTime = null;
        private timer = null;
        private radioConnection: HubConnection | null = null; 

        get oidcAuthenticated(){
            return this.$store.getters['oidcStore/oidcIsAuthenticated'];
        }

        get songDuration(){
            if(this.currentSongInfo != null){
                let duration = new Date(this.currentSongInfo.duration).getTime();
                return duration;
            }

            return 0;
        }

        created() : void {
            //this.connect();
        }

        updateElapsedTime() : void {
            let now = new Date();

            if(this.currentSongInfo != null){
                let elapsed = now.getTime() - new Date(this.currentSongInfo.startingTime).getTime();
                this.elapsedTime = elapsed;
                
                if(this.timer == null){
                    this.timer = setInterval(() => {
                        this.elapsedTime += 1000;
                    }, 1000)
                }
            }
        }

        convertMsToMMSS(ms) : string {
            let date = new Date(null);
            date.setMilliseconds(ms);
            
            return date.toISOString().substr(14, 5);
        }
        
        async connect() {
            if (this.radioConnection != null) {
                await this.radioConnection.stop();
            }

            this.radioConnection = new HubConnectionBuilder()
                .withUrl("/radio")
                .build();

            this.radioConnection?.on("ReceivePlayingTrackInfo", (trackInfo) => {
                this.currentSongInfo = {
                    artist: trackInfo.currentPlayingTrack.artists[0],
                    title: trackInfo.currentPlayingTrack.title,
                    startingTime: trackInfo.startingTime,
                    duration: trackInfo.currentPlayingTrack.songDurationMs
                }

                this.updateElapsedTime();

                console.log(trackInfo);
            })
            this.radioConnection.start()
                .then(() => console.log("connection started"));
        }
    }
</script>

<style scoped>

</style>