import axios from '@/plugins/axios';

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
    getUserProfile(context){
        return axios.get('api/auth/profile',
            {
                baseURL: process.env.VUE_APP_API_BASE_URL,
                withCredentials: true,
                headers: {
                    authorization: `Bearer ${context.rootState.oidcStore.access_token}`
                }
            })
            .then(({data}) => {
                context.commit('SET_USER_PROFILE', data.data)
            })
            .catch(err => console.log(err));
    },
    loadModState(context){
        axios.get('api/auth/mod',
            {
                baseURL: process.env.VUE_APP_API_BASE_URL,
                withCredentials: true,
                headers: {
                    authorization: `Bearer ${context.rootState.oidcStore.access_token}`
                }
            })
            .then((response) => {
                context.commit('SET_USER_MOD_STATE', response.data);
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