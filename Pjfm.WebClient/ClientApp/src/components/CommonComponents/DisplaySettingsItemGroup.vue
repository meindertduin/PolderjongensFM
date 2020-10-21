<template>
    <v-list-group @click.stop.prevent>
        <template v-slot:activator>
            <v-list-item-content>
                <v-list-item-title v-text="'DisplaySettings'"></v-list-item-title>
            </v-list-item-content>
        </template>
        <v-list-item @click.stop.prevent>
            <v-list-item-content class="pa-2">
                <v-switch
                        v-model="darkModeSwitch"
                        flat
                        label="Dark Mode"
                ></v-switch>
            </v-list-item-content>
        </v-list-item>
    </v-list-group>
</template>

<script lang="ts">
    import Vue from 'vue'
    import Component from 'vue-class-component'

    @Component({
        name: 'DisplaySettingsItemGroup',
    })
    export default class DisplaySettingsItemGroup extends Vue {
        private darkModeSwitch:boolean = false;
    }
</script>

<style scoped>

</style>