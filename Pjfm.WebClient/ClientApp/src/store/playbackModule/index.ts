import { GetterTree, MutationTree, ActionTree } from "vuex"
import {playerUpdateInfo} from "@/common/types"

class State {
    public playbackInfo: playerUpdateInfo | null = null;  
}

const mutations = <MutationTree<State>>{
    SET_PLAYBACK_INFO: (state, playerUpdateInfo:playerUpdateInfo) => state.playbackInfo = playerUpdateInfo,
}

const getters = <GetterTree<State, any>>{
    getPlaybackInfo: state => state.playbackInfo,
}

const actions = <ActionTree<State, any>>{
}

const PlaybackModule = {
    namespaced: true,
    state: new State(),
    mutations: mutations,
    getters: getters,
    actions: actions
}

export default PlaybackModule;