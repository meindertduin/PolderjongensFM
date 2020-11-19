<template>
    <div>
        <v-row>
            <v-col>
                <v-btn block class="orange ma-2" v-if="oidcAuthenticated" @click="connectWithPlayer">
                    <span class="" v-if="radioConnection">Synchroniseren</span>
                    <span class="" v-else>Klik hier om te verbinden</span>
                </v-btn>
                <v-btn block class="red ma-2" v-if="oidcAuthenticated" @click="disconnectWithPlayer">
                    <span class="" v-if="radioConnection">Disconnect Player</span>
                </v-btn>
            </v-col>
            <v-col>
              <v-btn block class="orange ma-2" v-if="oidcAuthenticated" @click="navigate('/search')">
                <span class="" v-if="radioConnection">Verzoekje doen!</span>
              </v-btn>
            </v-col>  
        </v-row>
        <SongInformation v-if="radioConnection" v-bind:radioConnection="radioConnection"/>
        <Queue v-if="radioConnection" v-bind:radioConnection="radioConnection"/>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'
    import SongInformation from "@/components/HomeComponents/RadioComponents/SongInformation.vue";
    import Queue from "@/components/HomeComponents/RadioComponents/Queue.vue";
    import {HubConnectionBuilder, TransferFormat, LogLevel, HubConnection} from "@microsoft/signalr";

    @Component({
        name: 'Radio',
        components: {
            SongInformation,
            Queue
        }
    })
    export default class Radio extends Vue {
        private radioConnection: HubConnection | null = null;

        get oidcAuthenticated(){
            return this.$store.getters['oidcStore/oidcIsAuthenticated'];
        }

        created(){
            this.connectToRadioSocket();
        }
        
        navigate(uri : string) : void{
            this.$router.push(uri);
        }
        
        async connectToRadioSocket() {
            if (this.radioConnection != null) {
                await this.radioConnection.stop();
            }

            this.radioConnection = new HubConnectionBuilder()
                .withUrl("/radio")
                .build();

            this.radioConnection.start()
                .then(() => console.log("connection started"));
        }
        
        connectWithPlayer(){
            this.radioConnection?.invoke("ConnectWithPlayer")
                .then(() => console.log("connection started with player"))
                .catch((err) => console.log(err));
        }
        
        disconnectWithPlayer(){
            this.radioConnection?.invoke("DisconnectWithPlayer")
                .then(() => console.log("Disconnected with player"))
                .catch((err) => console.log(err));
        } 
    }
</script>

<style scoped>

</style>