<template>
      <v-card
          class="pa-2"
          outlined
          round
      >
        <span class="overline grey--text">Included</span><br>
        <v-list dense>
          <v-list-item-group>
            <v-list-item v-for="(user, i) in includedUsers" @click="excludeUser(user)">
              <v-list-item-content>
                <v-list-item-title>
                  {{i + 1}}. {{user.displayName}} <span class="grey--text float-right" v-if="user.member">PJ</span>
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