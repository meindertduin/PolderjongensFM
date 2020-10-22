<template>
  <div>
    <v-row justify="center">
      <v-col cols="11" md="10">
        <v-row justify="center">
          <v-col cols="10" md="8">
            <Radio />
          </v-col>
          <v-col md="4">
            <LiveChat />
          </v-col>
        </v-row>
      </v-col>
    </v-row>
  </div>
</template>

<script lang="ts">
  import Vue from 'vue'
  import Component from 'vue-class-component'
  import Radio from "@/components/HomeComponents/Radio";
  import LiveChat from "@/components/HomeComponents/Livechat";
  
  @Component({
    name: 'Home',
    components: {
      Radio,
      LiveChat,
    }
  })
  export default class Home extends Vue {
    
  }
</script>

