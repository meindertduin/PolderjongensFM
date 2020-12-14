import { GetterTree, MutationTree, ActionTree } from "vuex"
import {playerUpdateInfo, trackDto, playbackSettings} from "@/common/types"
import {HubConnection} from "@microsoft/signalr";

class State {
    public playbackInfo: playerUpdateInfo | null = null;
    public radioConnection: HubConnection | null = null;
    public PlayerTimerOverLay : boolean = false
    
    public isPlaying: boolean = false;
    public isConnected: boolean = false;
    
    public currentSongInfo: trackDto | null = null
    public fillerQueuedTracks : Array<trackDto> = [];
    public playbackSettings : playbackSettings | null = null;
    public priorityQueuedTracks: Array<trackDto> = [];
    public currentTrackStartingTime : string | null = null;
    public secondaryQueuedTracks : Array<trackDto> = [];
}

const mutations = <MutationTree<State>>{
    SET_PLAYBACK_INFO: (state, playerUpdateInfo:playerUpdateInfo) => {
        state.currentSongInfo = playerUpdateInfo.currentPlayingTrack;
        state.fillerQueuedTracks = playerUpdateInfo.fillerQueuedTracks;
        state.priorityQueuedTracks = playerUpdateInfo.priorityQueuedTracks;
        state.playbackSettings = playerUpdateInfo.playbackSettings;
        state.currentTrackStartingTime = playerUpdateInfo.startingTime;
        state.secondaryQueuedTracks = playerUpdateInfo.secondaryQueuedTracks;
    },
    SET_RADIO_CONNECTION: (state, radioConnection:HubConnection) => state.radioConnection = radioConnection,
    TOGGLE_PLAYER_TIMER_OVERLAY: state => state.PlayerTimerOverLay = ! state.PlayerTimerOverLay,
    
    SET_PLAYBACK_PLAYING_STATUS: (state, isPlaying:boolean) => state.isPlaying = isPlaying, 
    SET_CONNECTED_STATUS: (state, isConnected:boolean) => state.isConnected = isConnected,
}

const getters = <GetterTree<State, any>>{
    getRadioConnection: state => state.radioConnection,
    getPlayerTimerOverlayActive: state => state.PlayerTimerOverLay,
    getPlayingStatus: state => state.isPlaying,
    getConnectedState: state => state.isConnected,

    getPlaybackInfo: state => state.playbackInfo,
    getCurrentTrack: state => state.currentSongInfo,
    getCurrentTrackStartingTime: state => state.currentTrackStartingTime,
    getPriorityQueuedTracks: state => state.priorityQueuedTracks,
    getFillerQueuedTracks: state => state.fillerQueuedTracks,
    getSecondaryQueuedTracks: state => state.secondaryQueuedTracks,
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