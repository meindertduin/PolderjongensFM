<template>
    <v-card>
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
        private includedUsers : Array<applicationUser> = [];
        
        created(){
            axios.get('api/playback/mod/include')
                .then((response) => {
                    this.includedUsers = response.data;
                    console.log(response)
                })
            .catch((err) => console.log(err));
        }
        
        excludeUser(user: applicationUser){
            axios.post("api/playback/mod/exclude",user)
                .then((response) => console.log(response))
                .catch((err) => console.log(err));
        }
    }
</script>

<style scoped>
    .included-users-container{
        max-height: 400px;
        flex-wrap: wrap;
        overflow-y: auto;
        overflow-x: hidden;
    }
    
    .included-user-field{
        display: flex;
        flex-direction: row;
        margin: 4px;
    }
</style>