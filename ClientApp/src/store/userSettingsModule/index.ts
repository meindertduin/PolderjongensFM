import { GetterTree, MutationTree, ActionTree } from "vuex"
import {defaultSettings} from "@/common/objects";
import {userSettings} from "@/common/types"
import get = Reflect.get;

class State {

}

const mutations = <MutationTree<State>>{
}

const getters = <GetterTree<State, any>>{
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
    }
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