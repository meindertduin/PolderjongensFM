<template>
    <v-card
            class="pa-2 users-settings-card"
            outlined
            round
    >
        <span class="overline grey--text">Zoeken</span><br>
        <v-text-field prepend-icon="mdi-magnify" label="Zoeken" v-on:keyup="searchBarKeyUp($event)" v-model="query"></v-text-field>
        <v-divider></v-divider>
        <span class="overline grey--text">Resultaten</span><br>
        <div class="text-center">
            <v-progress-circular :size="250" color="orange" indeterminate v-if="loading"></v-progress-circular>
        </div>
        <v-list dense v-if="searchUsersResult.length > 0 && !loading">
            <v-list-item-group class="results-container">
                <v-list-item v-for="(user, i) in searchUsersResult" :key="i" @click="addUser(user)">
                    <v-list-item-content>
                        <v-row>
                            <v-col>
                                <div class="user-details-container">
                                    <div class="text-h6 mx-2">
                                        {{i + 1}}. {{user.displayName}}
                                    </div>
                                    <div v-if="user.member" class="text-h6 blue--text mx-2">
                                        pjfm
                                    </div>
                                </div>
                            </v-col>
                        </v-row>
                    </v-list-item-content>
                </v-list-item>
            </v-list-item-group>
        </v-list>
    </v-card>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from "vue-class-component";
    import axios from 'axios'
    import {applicationUser, trackDto} from "@/common/types";

    @Component({
        name: "searchUserComponent",
    })
    export default class searchUserComponent extends Vue {
        private query = '';
        private searchUsersResult : Array<applicationUser> = [];
        private loading = false;
        
        get includedUsers():Array<applicationUser>{
            return this.$store.getters['modModule/getIncludedUsers'];
        }

        created(){
            this.loadMembers();
        }
        
        loadMembers():Promise<void>{
            this.loading = true;
            return axios.get('api/user/members')
                .then(({data}:{data:Array<applicationUser>}) => {
                    this.searchUsersResult = data.filter(x => {
                        const user = this.includedUsers.find(l => l.id === x.id);
                        if (user){
                            return false;
                        }
                        else{
                            return true;
                        }
                    });
                })
                .catch((err) => console.log(err))
                .finally(() => {
                    this.loading = false
                });
        }

        searchBarKeyUp(e){
            clearTimeout($.data(this, 'timer'));

            if (e.keyCode == 13)
                this.search(true);
            else
                $(this).data('timer', setTimeout(this.search, 500));
        }
        
        search(force){
            if (!force && this.query.length < 3){
                this.loadMembers();
                return;
            }
            this.loading = true;
            this.items = []

            axios.get(`https://localhost:5001/api/user/search?query=${this.query}`)
                .then((response) => {
                    this.searchUsersResult = response.data;
                    console.log(response.data);
                })
                .catch((err) => console.log(err))
                .finally(() => this.loading = false);
        }
        
        addUser(user: applicationUser){
            axios.post('api/playback/mod/include', user)
                .then((response) => {
                    this.$store.commit('modModule/ADD_INCLUDED_USER', user);
                    this.searchUsersResult = this.searchUsersResult.filter(x => x.id !== user.id);
                })
            .catch((err) => console.log(err));
        }
    }
</script>

<style scoped>
    .user-details-container{
        display: flex;
        flex-direction: row;
        flex-wrap: nowrap;
    }

    .users-settings-card{
        max-height: 700px;
    }
    .results-container{
        max-height: 500px;
        overflow-x: hidden;
        overflow-y: auto;
    }
</style>