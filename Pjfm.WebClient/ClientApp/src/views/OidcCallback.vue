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
                    const url = `https://accounts.spotify.com/authorize` + 
                        `?client_id=ebc49acde46148eda6128d944c067b5d` + 
                        `&response_type=code` +
                        `&redirect_uri=https://localhost:5001/api/spotify/account/callback` + 
                        `&scope=user-top-read user-read-private streaming user-read-playback-state`;
                    
                    location.href = url;
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