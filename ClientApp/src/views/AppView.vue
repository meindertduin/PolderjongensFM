<template>
  <v-container>
    <Radio />
    <div v-if="isMod">
      <v-row justify="center">
        <v-col v-if="isMod" md="10">
          <PlaybackSettingsDashboard  />
        </v-col>
      </v-row>
      <v-row justify="center">
        <v-col md="10">
          <UserIncludeSettingsDashboard />
        </v-col>
      </v-row>
    </div>
  </v-container>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'
    import PlaybackSettingsDashboard from "@/components/ModComponents/PlaybackSettingsDashboard.vue";
    import UserIncludeSettingsDashboard from "@/components/ModComponents/UserIncludeSettingsDashboard.vue";
    import Radio from "@/components/HomeComponents/Radio.vue";

    @Component({
    name: 'AppView',
    components: {
        UserIncludeSettingsDashboard,
        Radio,
        PlaybackSettingsDashboard  
    }
  })
  export default class AppView extends Vue {
    get isMod(){
        return this.$store.getters['profileModule/isMod'];
    }
      
    navigate(uri : string) : void{
      this.$router.push(uri);
    }
  }
</script>

<style>
  #sticky-button{
      position: sticky;
      bottom: 10px;
      right: 10px;
  }
</style>