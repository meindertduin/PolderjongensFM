<template>
    <div>
        <SongInformation/>
        <Queue/>
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
        },
    })
    export default class Radio extends Vue {
        get oidcAuthenticated(){
            return this.$store.getters['oidcStore/oidcIsAuthenticated'];
        }

      get radioConnection(){
        return this.$store.getters['playbackModule/radioConnection'];
      }
        
      navigate(uri : string) : void{
          this.$router.push(uri);
      }
    }
</script>

<style scoped>

</style>