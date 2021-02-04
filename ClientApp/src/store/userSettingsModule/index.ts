import { GetterTree, MutationTree, ActionTree } from "vuex"
import {defaultModLocalSettings, defaultSettings} from "@/common/objects";
import {userSettings, modLocalSettings} from "@/common/types"

class State {
    public sideBarOpen:boolean = false;
    
    public modLocalSettings: modLocalSettings | null = null;
}

const mutations = <MutationTree<State>>{
    TOGGLE_SIDE_BAR: state => state.sideBarOpen = ! state.sideBarOpen,
    SET_SIDE_BAR: (state, isOpen:boolean) => state.sideBarOpen = isOpen, 
    
    SET_MOD_LOCAL_SETTINGS: (state, settings:modLocalSettings) => state.modLocalSettings = settings,
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

    getMakeRequestAsMod: (state) => state.modLocalSettings? state.modLocalSettings.requestAsMod: false,
}

const actions = <ActionTree<State, any>>{
    loadModLocalSettings({commit}) {
        const modSettings = localStorage.getItem("modSettings");
        let modSettingObj: modLocalSettings | null = null;
        if (modSettings){
            modSettingObj = JSON.parse(modSettings);

        } else{
            modSettingObj = defaultModLocalSettings;
            localStorage.setItem("modSettings", JSON.stringify(defaultModLocalSettings));
        }

        commit('SET_MOD_LOCAL_SETTINGS', modSettingObj);
    },
    
    setDarkMode({getters}, value: boolean){
        let userSettings: userSettings = getters['loadUserSettings'];
        userSettings.darkMode = value;
        localStorage.setItem('userSettings', JSON.stringify(userSettings));
    },
    
    setAsModRequest({commit, getters}, value: boolean) {
        let modLocalSettings: modLocalSettings = getters['getModLocalSettings'];
        modLocalSettings.requestAsMod = value;
        commit('SET_MOD_LOCAL_SETTINGS', modLocalSettings);
        localStorage.setItem('modSettings', JSON.stringify(modLocalSettings));
    },
}

const UserSettingsModule = {
    namespaced: true,
    state: new State(),
    mutations: mutations,
    getters: getters,
    actions: actions
}

export default UserSettingsModule;