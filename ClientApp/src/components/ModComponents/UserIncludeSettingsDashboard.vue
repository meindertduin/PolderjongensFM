<template>
    <v-card>
        <v-card-text>
            <v-row>
                <v-col class="col-12 col-md-6">
                    <SearchUserComponent />
                </v-col>
                <v-col class="col-12 col-md-6">
                    <IncludedUsersDisplay />
                </v-col>
            </v-row>
        </v-card-text>
    </v-card>
</template>

<script lang="ts">
    import Vue from 'vue';
    import Component from "vue-class-component";
    import IncludedUsersDisplay from "@/components/ModComponents/IncludedUsersDisplay.vue";
    import SearchUserComponent from "@/components/ModComponents/SearchUserComponent.vue";

    @Component({
        name: "UserIncludeSettingsDashboard",
        components: {SearchUserComponent, IncludedUsersDisplay},
    })
    export default class UserIncludeSettingsDashboard extends Vue {
        
    }
</script>

<style scoped>
    
</style>