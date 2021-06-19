import axios from "axios";

import {GetterTree, MutationTree, ActionTree, Module} from "vuex"
import {applicationUser, identityProfile, trackDto} from "@/common/types";

class ProfileState {
    playlistDialog: boolean = false
}

const mutations = <MutationTree<ProfileState>>{
    TOGGLE_PLAYLIST_DIALOG: state => state.playlistDialog = !state.playlistDialog,
    SET_PLAYLIST_DIALOG: (state, active: boolean) => state.playlistDialog = active,
}

const getters = <GetterTree<ProfileState, any>>{
    isPlaylistDialogActive: state => state.playlistDialog,
}


const ProfileModule: Module<ProfileState, any> = {
    namespaced: true,
    state: new ProfileState(),
    mutations: mutations,
    getters: getters,
}

export default ProfileModule;