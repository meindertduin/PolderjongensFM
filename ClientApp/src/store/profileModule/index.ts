import axios from '@/plugins/axios';

import { GetterTree, MutationTree, ActionTree } from "vuex"
import {applicationUser, identityProfile} from "@/common/types";

class State {
    public userProfile: identityProfile | null = null;
    public playlistDialog : boolean = false
    public isMod : boolean = false;
    public isSpotifyAuthenticated: boolean = false;
}

const mutations = <MutationTree<State>>{
    SET_USER_PROFILE: (state, profile:identityProfile) => state.userProfile = profile,
    TOGGLE_PLAYLIST_DIALOG: state => state.playlistDialog = !state.playlistDialog,
}

const getters = <GetterTree<State, any>>{
    userProfile: (state) => state.userProfile? state.userProfile.userProfile : null,
    isMod: state => state.userProfile? state.userProfile.isMod: false,
    isSpotifyAuthenticated: state => state.userProfile? state.userProfile.isSpotifyAuthenticated: false,
    isPlaylistDialogActive: state => state.playlistDialog,
    userId: state => state.userProfile? state.userProfile.userProfile.id : null,
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
                console.log(data);
                context.commit('SET_USER_PROFILE', data.data)
            })
            .catch(err => console.log(err));
    },
}

const ProfileModule = {
    namespaced: true,
    state: new State(),
    mutations: mutations,
    getters: getters,
    actions: actions
}

export default ProfileModule;