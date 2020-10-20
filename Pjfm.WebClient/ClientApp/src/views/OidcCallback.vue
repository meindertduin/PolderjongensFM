<template>
    <div>
        {{message}}
    </div>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'
    
    @Component({
        name: 'OidcCallback',
    })
    export default class OidcCallback extends Vue{
        private oidcSignInCallback():Promise<string>{
            return this.$store.dispatch('oidcStore/oidcSignInCallback')
        }
        
        created(){
            console.log("oidc callback")
            this.oidcSignInCallback()
                .then((redirectPath) => {
                    this.$router.push(redirectPath)
                })
                .catch((err) => {
                    console.error(err)
                    this.$router.push('/signin-oidc-error') // Handle errors any way you want
                })
        }
    }
</script>

<style scoped>

</style>