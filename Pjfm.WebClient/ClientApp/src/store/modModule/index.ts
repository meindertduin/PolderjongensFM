import Axios from "axios";

import { GetterTree, MutationTree, ActionTree } from "vuex"
import {applicationUser} from "@/common/types";
import axios from "axios";
import App from "@/App.vue";

class State {
    public includedUsers : Array<applicationUser> = [];
}

const mutations = <MutationTree<State>>{
    SET_INCLUDED_USERS: (state, users: Array<applicationUser>) => state.includedUsers = users,
    ADD_INCLUDED_USER: (state, user: applicationUser) => state.includedUsers.push(user),
    REMOVE_INCLUDED_USER: (state, user:applicationUser) => state.includedUsers = state.includedUsers.filter(x => x.id !== user.id),
}

const getters = <GetterTree<State, any>>{
    getIncludedUsers: state => state.includedUsers, 
}

const actions = <ActionTree<State, any>>{
    loadIncludedUsers({commit}){
        return axios.get('api/playback/mod/include')
            .then((response) => {
                this.includedUsers = response.data;
                console.log(response)
            })
            .catch((err) => console.log(err));
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