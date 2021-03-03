<template>
  
</template>

<script lang="ts">
import Component from "vue-class-component";
import Vue from "vue";
import { vuexOidcProcessSilentSignInCallback } from 'vuex-oidc'

@Component({
  name: 'OidcSilentCallback',
})
export default class OidcSilentCallback extends Vue{
    created(){
      vuexOidcProcessSilentSignInCallback()
    }
}
</script>

<style scoped>

</style>