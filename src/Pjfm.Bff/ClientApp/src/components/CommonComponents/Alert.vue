<template>
    <div>
      <v-alert :type="alertType" v-model="alert" dismissible transition="fade-transition">
        {{ alertMessage }}
      </v-alert>
    </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from "vue-class-component";
import {Watch} from "vue-property-decorator";

@Component({
  name: 'Alert',
})
export default class Alert extends Vue{
  private alert : boolean = this.alertType != null;
  
  created(){
    this.alert = this.alertType != null;
    
    setTimeout(() => {
      this.alert = false;
    }, 10000)
  }
  
  get alertType(){
    return this.$store.getters['alertModule/alertType'];
  }

  get alertMessage(){
    return this.$store.getters['alertModule/alertMessage'];
  }

  @Watch("alert")
  onAlertChange(alertState : boolean){
    if(!alertState){
      this.$store.commit('alertModule/SET_ALERT', null);
    }
  }
}
</script>

<style scoped>

</style>