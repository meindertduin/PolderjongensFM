<template>
    <div>

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
            this.oidcSignInCallback()
                .then((redirectPath) => {
                  location.href = redirectPath;
                })
                .catch((err) => {
                    console.error(err);
                    this.$router.push('/signin-oidc-error');
                })
        }
    }
</script>

<style scoped>

</style>