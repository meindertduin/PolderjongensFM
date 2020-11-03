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
    import {HubConnectionBuilder, TransferFormat, LogLevel} from "@microsoft/signalr";

    @Component({
        name: 'Radio',
    })
    export default class Radio extends Vue {
        get oidcAuthenticated(){
            return this.$store.getters['oidcStore/oidcIsAuthenticated'];
        }
        
        connect(){
            const connection = new HubConnectionBuilder()
                .withUrl("/radio")
                .build();

            connection?.on("ReceivePlayingTrackInfo", (trackInfo) => {
                console.log(trackInfo);
            })
            connection.start()
                .then(() => console.log("connection started"));
            
        }
    }
</script>

<style scoped>

</style>