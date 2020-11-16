<template>
    <v-card class="users-settings-card">
        <v-card-title>
            <v-row justify="center">
                <div class="text-h4">Included users</div>
            </v-row>
        </v-card-title>
        <v-card-text>
            <div class="included-users-container">
                <div class="included-user-field" v-for="(user, index) in includedUsers">
                    <v-row justify="end">
                        <v-col>
                            <div class="mx-2">{{user.displayName}}</div>
                        </v-col>
                        <v-col>
                            <div v-if="user.member" class="blue--text">Pjfm</div>
                        </v-col>
                        <v-col>
                            <v-btn small text color="orange" @click="excludeUser(user)">remove</v-btn>
                        </v-col>
                    </v-row>
                </div>
            </div>
        </v-card-text>
    </v-card>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from "vue-class-component";
    import axios from 'axios'
    import {applicationUser} from "@/common/types";

    @Component({
        name: "IncludedUsersDisplay",
    })
    export default class IncludedUsersDisplay extends Vue {
        get includedUsers():Array<applicationUser>{
            return this.$store.getters['modModule/getIncludedUsers'];
        }
        
        created(){
            this.$store.dispatch('modModule/loadIncludedUsers');
        }
        
        excludeUser(user: applicationUser){
            axios.post("api/playback/mod/exclude",user)
                .then((response) => {
                    this.$store.commit('modModule/REMOVE_INCLUDED_USER', user);
                    console.log(response)
                })
                .catch((err) => console.log(err));
        }
    }
</script>

<style scoped>
    .included-users-container{
        max-height: 600px;
        flex-wrap: wrap;
        overflow-y: auto;
        overflow-x: hidden;
    }
    
    .included-user-field{
        display: flex;
        flex-direction: row;
        margin: 4px;
    }
    
    .users-settings-card{
        max-height: 700px;
    }
</style>