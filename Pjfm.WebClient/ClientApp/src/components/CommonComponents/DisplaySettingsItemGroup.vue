<template>
    <v-list-group @click.stop.prevent>
        <template v-slot:activator>
            <v-list-item-content>
                <v-list-item-title v-text="'DisplaySettings'"></v-list-item-title>
            </v-list-item-content>
        </template>
        <v-list-item>
            <v-list-item-content class="pa-2">
                <v-switch @click.stop.prevent
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
    import {Watch} from "vue-property-decorator";

    @Component({
        name: 'DisplaySettingsItemGroup',
    })
    export default class DisplaySettingsItemGroup extends Vue {
        
        private darkModeSwitch:boolean = false;

        created(){
            this.darkModeSwitch = this.$store.getters['userSettings/getDarkModeState'];
        }
        
        @Watch('darkModeSwitch')
        private onDarkModeSwitchChanged(newValue:boolean, oldValue:boolean){
            const userSettings:userSettings = this.$store.getters['userSettingsModule/loadUserSettings'];
            this.$vuetify.theme.dark = !userSettings.darkMode;
            console.log(newValue)
            this.$store.dispatch('userSettingsModule/setDarkMode', newValue);
        }
    }
</script>

<style scoped>

</style>