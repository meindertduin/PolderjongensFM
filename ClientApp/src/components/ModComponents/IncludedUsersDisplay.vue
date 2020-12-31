<template>
      <v-card
          class="pa-2"
          outlined
          round
          style="margin-bottom: 90px"
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
    import {applicationUser} from "@/common/types";
    import {AxiosResponse} from "axios";

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
            // @ts-ignore
            this.$axios.post("api/playback/mod/exclude", user)
                .then((response:AxiosResponse) => {
                    this.$store.commit('modModule/REMOVE_INCLUDED_USER', user);
                    console.log(response)
                })
                .catch((err:any) => console.log(err));
        }
    }
</script>
