import Axios from "axios";

import { GetterTree, MutationTree, ActionTree } from "vuex"
import {applicationUser} from "@/common/types";

class State {
    public userProfile: applicationUser | null = null; 
    public isMod : boolean = false;
}

const mutations = <MutationTree<State>>{
    SET_USER_PROFILE: (state, profile:applicationUser) => state.userProfile = profile,
    SET_USER_MOD_STATE: (state, value:boolean) => state.isMod = value,
}

const getters = <GetterTree<State, any>>{
    userProfile: (state) => state.userProfile,
    isMod: state => state.isMod,
}

const actions = <ActionTree<State, any>>{
    getUserProfile({commit}){
        return Axios.get('api/auth/profile')
            .then(({data}) => commit('SET_USER_PROFILE', data.data))
            .catch(err => console.log(err));
    },
    loadModState({commit}){
        Axios.get('api/auth/mod')
            .then((response) => {
                commit('SET_USER_MOD_STATE', response.data);
                console.log(response.data);
            })
            .catch((err) => console.log(err));
    }
}

const ProfileModule = {
    namespaced: true,
    state: new State(),
    mutations: mutations,
    getters: getters,
    actions: actions
}

export default ProfileModule;