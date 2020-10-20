<template>
  <div class="about">
    <v-btn @click="getUserMessage">User</v-btn>
    <v-btn @click="getModMessage">Mod</v-btn>
  </div>
</template>

<script lang="ts">

  import Vue from 'vue'
  import Component from 'vue-class-component'
  import Axios from "axios";

  @Component({
    name: 'About',
  })
  export default class About extends Vue{
    private getUserMessage(){
      Axios.get('https://localhost:5001/api/test/user')
        .then((response) => {
        console.log(response.data);
      })
    }

    private getModMessage(){
      Axios.get('https://localhost:5001/api/test/mod')
              .then((response) => {
                console.log(response.data);
              })
    }
  }
</script>