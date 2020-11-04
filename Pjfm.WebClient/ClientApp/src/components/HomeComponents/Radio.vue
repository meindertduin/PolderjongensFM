<template>
    <div>
        <v-btn v-if="oidcAuthenticated" @click="connect">
            Connect
        </v-btn>
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
        get oidcAuthenticated(){
            return this.$store.getters['oidcStore/oidcIsAuthenticated'];
        }
        
        private radioConnection: HubConnection | null = null; 
        
        async connect() {
            if (this.radioConnection != null) {
                await this.radioConnection.stop();
            }

            this.radioConnection = new HubConnectionBuilder()
                .withUrl("/radio")
                .build();

            this.radioConnection?.on("ReceivePlayingTrackInfo", (trackInfo) => {
                console.log(trackInfo);
            })
            this.radioConnection.start()
                .then(() => console.log("connection started"));
        }
    }
</script>

<style scoped>

</style>