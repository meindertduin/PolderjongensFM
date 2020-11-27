import { GetterTree, MutationTree, ActionTree } from "vuex"
import {applicationUser} from "@/common/types";
import axios from "axios";
import App from "@/App.vue";

class State {
    public includedUsers : Array<applicationUser> = [];
    public loadedUsers : Array<applicationUser> = [];
}

const mutations = <MutationTree<State>>{
    SET_INCLUDED_USERS: (state, users: Array<applicationUser>) => state.includedUsers = users,
    ADD_INCLUDED_USER: (state, user: applicationUser) => state.includedUsers.push(user),
    REMOVE_INCLUDED_USER: (state, user:applicationUser) => state.includedUsers = state.includedUsers.filter(x => x.id !== user.id),
    
    SET_LOADED_USERS: (state, users: Array<applicationUser>) => state.loadedUsers = users
}

const getters = <GetterTree<State, any>>{
    getIncludedUsers: state => state.includedUsers, 
    getLoadedUsers: state => state.loadedUsers, 
}

const actions = <ActionTree<State, any>>{
    loadIncludedUsers(context){
        return axios.get('api/playback/mod/include',{
            baseURL: process.env.VUE_APP_API_BASE_URL,
            withCredentials: true,
            headers: {
                authorization: `Bearer ${context.rootState.oidcStore.access_token}`
            }
        })
            .then((response) => {
                context.commit('SET_INCLUDED_USERS', response.data);
            })
            .catch((err) => console.log(err));
    },
    loadUsers(context){
        return axios.get('api/user/members',{
            baseURL: process.env.VUE_APP_API_BASE_URL,
            withCredentials: true,
            headers: {
                authorization: `Bearer ${context.rootState.oidcStore.access_token}`
            }
        })
            .then(({data}:{data:Array<applicationUser>}) => {
                context.commit('SET_LOADED_USERS', data);
            })
            .catch((err) => console.log(err))
    }
}

const ModModule = {
    namespaced: true,
    state: new State(),
    mutations: mutations,
    getters: getters,
    actions: actions
}

export default ModModule;