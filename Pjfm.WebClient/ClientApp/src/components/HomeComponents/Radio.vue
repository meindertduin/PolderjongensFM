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
        <SongInformation v-if="radioConnection" v-bind:radioConnection="radioConnection"/>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'
    import SongInformation from "@/components/HomeComponents/RadioComponents/SongInformation.vue";
    import {HubConnectionBuilder, TransferFormat, LogLevel, HubConnection} from "@microsoft/signalr";

    @Component({
        name: 'Radio',
        components: {
            SongInformation,
        }
    })
    export default class Radio extends Vue {
        private radioConnection: HubConnection | null = null; 

        get oidcAuthenticated(){
            return this.$store.getters['oidcStore/oidcIsAuthenticated'];
        }
        
        async connect() {
            if (this.radioConnection != null) {
                await this.radioConnection.stop();
            }

            this.radioConnection = new HubConnectionBuilder()
                .withUrl("/radio")
                .build();

            this.radioConnection.start()
                .then(() => console.log("connection started"));
        }
    }
</script>

<style scoped>

</style>