import { GetterTree, MutationTree, ActionTree } from "vuex"
import {defaultSettings} from "@/common/objects";
import {userSettings} from "@/common/types"

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
}

const actions = <ActionTree<State, any>>{
    setDarkMode({getters}, value: boolean){
        let userSettings: userSettings = getters['loadUserSettings'];
        userSettings.darkMode = value;
        localStorage.setItem('userSettings', JSON.stringify(userSettings));
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