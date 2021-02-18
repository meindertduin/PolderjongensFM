<template>
    <v-card class="pa-2" outlined round>
        <span class="overline grey--text">Not Included</span><br>
        <div class="text-center">
          <v-progress-circular :size="250" color="orange" indeterminate v-if="loading"></v-progress-circular>
        </div>
        <v-list dense v-if="selectableUsers.length > 0 && !loading">
            <v-list-item-group>
                <v-list-item v-for="(user, i) in selectableUsers" :key="i" @click="addUser(user)">
                    <v-list-item-content>
                      <v-list-item-title>
                        {{i + 1}}. {{user.displayName}}
                        <span class="green--text float-right ml-4" v-if="user.spotifyAuthenticated">Auth</span>
                        <span class="grey--text float-right" v-if="user.member">PJ </span>
                      </v-list-item-title>
                    </v-list-item-content>
                </v-list-item>
            </v-list-item-group>
        </v-list>
    </v-card>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from "vue-class-component";
    import axios, {AxiosResponse} from 'axios'
    import {applicationUser, trackDto} from "@/common/types";

    @Component({
        name: "searchUserComponent",
    })
    export default class searchUserComponent extends Vue {
        private searchUsersResult : Array<applicationUser> = [];
        private loading = false;
        
        get selectableUsers():Array<applicationUser>{
            const includedUsers :Array<applicationUser> = this.$store.getters['modModule/getIncludedUsers'];
            const loadedUsers :Array<applicationUser> = this.$store.getters['modModule/getLoadedUsers'];
            
            return loadedUsers.filter(u => {
                const user = includedUsers.find(l => l.id === u.id);
                if (user){
                    return false;
                }
                else{
                    return true;
                }
            })
        }

        created(){
            this.$store.dispatch('modModule/loadIncludedUsers');
            this.$store.dispatch('modModule/loadUsers');
        }
        
        addUser(user: applicationUser){
            // @ts-ignore          
            this.$axios.post('api/playback/mod/include', user)
                .then((response:AxiosResponse) => {
                    this.$store.commit('modModule/ADD_INCLUDED_USER', user);
                    this.searchUsersResult = this.searchUsersResult.filter(x => x.id !== user.id);
                })
            .catch((err:any) => console.log(err));
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