import { GetterTree, MutationTree, ActionTree } from "vuex"
import {playerUpdateInfo} from "@/common/types"
import {HubConnection} from "@microsoft/signalr";

class State {
    public playbackInfo: playerUpdateInfo | null = null;
    public radioConnection: HubConnection | null = null;
    public PlayerTimerOverLay : boolean = false
    
    public isPlaying: boolean = false;
}

const mutations = <MutationTree<State>>{
    SET_PLAYBACK_INFO: (state, playerUpdateInfo:playerUpdateInfo) => state.playbackInfo = playerUpdateInfo,
    SET_RADIO_CONNECTION: (state, radioConnection:HubConnection) => state.radioConnection = radioConnection,
    TOGGLE_PLAYER_TIMER_OVERLAY: state => state.PlayerTimerOverLay = ! state.PlayerTimerOverLay,
    
    SET_PLAYBACK_PLAYING_STATUS: (state, isPlaying:boolean) => state.isPlaying = isPlaying, 
}

const getters = <GetterTree<State, any>>{
    getPlaybackInfo: state => state.playbackInfo,
    getRadioConnection: state => state.radioConnection,
    getPlayerTimerOverlayActive: state => state.PlayerTimerOverLay,
    getPlayingStatus: state => state.isPlaying,
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