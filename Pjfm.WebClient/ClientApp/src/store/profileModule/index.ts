import Axios from "axios";

import { GetterTree, MutationTree, ActionTree } from "vuex"

class State {
    public userProfile: profileModel | null = null; 
}

const mutations = <MutationTree<State>>{
    SET_USER_PROFILE: (state, profile:profileModel) => state.userProfile = profile,
}

const getters = <GetterTree<State, any>>{
    userProfile: (state) => state.userProfile,
}

const actions = <ActionTree<State, any>>{
    getUserProfile({commit}){
        return Axios.get('api/auth/profile')
            .then(({data}) => commit('SET_USER_PROFILE', data.data))
            .catch(err => console.log(err));
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