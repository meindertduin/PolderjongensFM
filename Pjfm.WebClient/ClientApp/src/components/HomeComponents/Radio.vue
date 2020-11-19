<template>
    <div>
        <v-row>
          <v-col>
            <v-btn block class="green ma-2" v-if="oidcAuthenticated && !playbackConnected" @click="connectWithPlayer">
              <span class="" v-if="radioConnection">Luisteren</span>
              <span class="" v-else>Klik hier om te verbinden</span>
            </v-btn>
            <v-btn block class="red ma-2" v-if="oidcAuthenticated && playbackConnected" @click="disconnectWithPlayer">
              <span class="" v-if="radioConnection">Stoppen</span>
            </v-btn>
          </v-col>
        </v-row>
        <v-footer height="auto" color="transparent" fixed>
          <v-row row wrap>
            <v-col>
              <v-btn class="float-right mx-1"  fab dark color="orange" @click="navigate('/search')">
                <v-icon large>mdi-magnify</v-icon>
              </v-btn>
            </v-col>
          </v-row>
        </v-footer>
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
        private playbackConnected: boolean = false;

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
            this.playbackConnected = true;
          
            this.radioConnection?.invoke("ConnectWithPlayer")
                .then(() => console.log("connection started with player"))
                .catch((err) => console.log(err));
        }
        
        disconnectWithPlayer(){
            this.playbackConnected = false;
          
            this.radioConnection?.invoke("DisconnectWithPlayer")
                .then(() => console.log("Disconnected with player"))
                .catch((err) => console.log(err));
        } 
    }
</script>

<style scoped>

</style>