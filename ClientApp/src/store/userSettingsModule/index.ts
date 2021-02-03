import { GetterTree, MutationTree, ActionTree } from "vuex"
import {defaultModLocalSettings, defaultSettings} from "@/common/objects";
import {userSettings, modLocalSettings} from "@/common/types"

class State {
    public sideBarOpen:boolean = false;
}

const mutations = <MutationTree<State>>{
    TOGGLE_SIDE_BAR: state => state.sideBarOpen = ! state.sideBarOpen,
    SET_SIDE_BAR: (state, isOpen:boolean) => state.sideBarOpen = isOpen, 
}

const getters = <GetterTree<State, any>>{
    getSidebarOpenState: state => state.sideBarOpen,
    loadUserSettings: () => {
        const userSettings = localStorage.getItem("userSettings");
        if (userSettings){
            const userSettingsObject:userSettings = JSON.parse(userSettings);
            return userSettingsObject
        }

        return defaultSettings;
    },
    
    getDarkModeState: (state ,getters) => {
        const userSettingsObject:userSettings = getters['loadUserSettings'];
        return userSettingsObject.darkMode;
    },

    getModLocalSettings: () => {
        const modSettings = localStorage.getItem("modSettings");
        if (modSettings){
            return JSON.parse(modSettings);
        } else{
            localStorage.setItem("modSettings", JSON.stringify(defaultModLocalSettings));
            return defaultModLocalSettings;
        }
    },

    getMakeRequestAsMod: (state, getters) => {
        const localSettings: modLocalSettings = getters["getModLocalSettings"];
        return localSettings.requestAsMod;
    },
}

const actions = <ActionTree<State, any>>{
    setDarkMode({getters}, value: boolean){
        let userSettings: userSettings = getters['loadUserSettings'];
        userSettings.darkMode = value;
        localStorage.setItem('userSettings', JSON.stringify(userSettings));
    },
    setAsModRequest({getters}, value: boolean) {
        let modLocalSettings: modLocalSettings = getters['getModLocalSettings'];
        modLocalSettings.requestAsMod = value;
        localStorage.setItem('modSettings', JSON.stringify(modLocalSettings));
    }
}

const UserSettingsModule = {
    namespaced: true,
    state: new State(),
    mutations: mutations,
    getters: getters,
    actions: actions
}

export default UserSettingsModule;