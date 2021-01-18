<template>
    <div>
        <Alert :key="alertId"/>
        <div style="margin: 10px;">
          <SongInformation/>
          <Queue/>
        </div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'
    import SongInformation from "@/components/HomeComponents/RadioComponents/SongInformation.vue";
    import Queue from "@/components/HomeComponents/RadioComponents/Queue.vue";
    import {HubConnectionBuilder, TransferFormat, LogLevel, HubConnection} from "@microsoft/signalr";
    import {Watch} from "vue-property-decorator";
    import Alert from "@/components/CommonComponents/Alert.vue";

    @Component({
        name: 'Radio',
        components: {
            SongInformation,
            Queue,
            Alert
        },
    })
    export default class Radio extends Vue {
      get alertId(){
        return this.$store.getters['alertModule/alertId'];
      }
      
      get oidcAuthenticated(){
          return this.$store.getters['oidcStore/oidcIsAuthenticated'];
      }

      get radioConnection(){
        return this.$store.getters['playbackModule/radioConnection'];
      }

      get errorMessage(){
        return this.$store.getters['errorModule/errorMessage'];
      }
        
      navigate(uri : string) : void{
          this.$router.push(uri);
      }
    }
</script>

<style scoped>

</style>