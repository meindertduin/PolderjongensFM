<template>
    <div>
        <v-btn @click="getUserMessage">User</v-btn>
        <v-btn @click="getModMessage">Mod</v-btn>
        <v-btn @click="getUserTopTracks">Top Tracks</v-btn>
        <v-btn @click="refreshToken">Refresh token</v-btn>
        <v-btn @click="playlists">Playlists</v-btn>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'
    import {AxiosResponse} from "axios";

    @Component({
        name: 'Test',
    })
    export default class Test extends Vue{
        private getUserMessage(){
            this.$axios.get('/api/test/user')
                .then((response:AxiosResponse<any>) => console.log(response.data))
                .catch((err:any) => console.log(err));
        }

        private getModMessage(){
            this.$axios.get('/api/test/mod')
                .then((response:AxiosResponse<any>) => console.log(response.data))
                .catch((err:any) => console.log(err));
        }

        private getUserTopTracks(){
            this.$axios.get('/api/test/toptracks')
                .then((response:AxiosResponse<any>) => console.log(response.data))
                .catch((err:any) => console.log(err));
        }
        
        private refreshToken(){
            this.$axios.get('/api/test/refreshToken')
                .then((response:AxiosResponse<any>) => console.log(response.data))
                .catch((err:any) => console.log(err));
        }
        
        private playlists(){
            this.$axios.get('api/playlist')
                .then((response:AxiosResponse) => console.log(response.data))
                .catch((err:any) => console.log(err));
        }
    }
</script>

<style scoped>

</style>