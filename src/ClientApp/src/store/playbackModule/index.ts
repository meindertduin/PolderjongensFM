import {ActionTree, GetterTree, MutationTree} from "vuex"
import {
    trackDto,
    userPlaybackInfo,
    playbackState,
    userPlaybackSettings
} from "@/common/types"
import {HubConnection} from "@microsoft/signalr";
import axios, {AxiosResponse} from "axios";

class State {
    public playbackInfo: userPlaybackInfo | null = null;
    public radioConnection: HubConnection | null = null;
    public PlayerTimerOverLay : boolean = false
    
    public listenersCount : number = 0;
    
    public isPlaying: boolean = false;
    public isConnected: boolean = false;
    public subscribeEndDate: any | null = null;
    
    public currentSongInfo: trackDto | null = null
    public fillerQueuedTracks : Array<trackDto> = [];
    public priorityQueuedTracks: Array<trackDto> = [];
    public currentTrackStartingTime : string | null = null;
    public secondaryQueuedTracks : Array<trackDto> = [];
    
    public playbackState: playbackState | null = null;
    public maxRequestsPerUser: number | null = null;

}

const mutations = <MutationTree<State>>{
    SET_PLAYBACK_INFO: (state, playerUpdateInfo:userPlaybackInfo) => {
        state.playbackInfo = playerUpdateInfo;
        // playback tracks info
        state.currentSongInfo = playerUpdateInfo.currentPlayingTrack;
        state.fillerQueuedTracks = playerUpdateInfo.fillerQueuedTracks;
        state.priorityQueuedTracks = playerUpdateInfo.priorityQueuedTracks;
        state.currentTrackStartingTime = playerUpdateInfo.startingTime;
        state.secondaryQueuedTracks = playerUpdateInfo.secondaryQueuedTracks;
    },
    
    SET_PLAYBACK_SETTINGS: (state, playbackSettings:userPlaybackSettings) => {
        state.playbackState = playbackSettings.playbackState;
        state.isPlaying = playbackSettings.isPlaying;
        state.maxRequestsPerUser = playbackSettings.maxRequestsPerUser;
    },
    
    SET_LISTENERS_COUNT: (state, amount: number) => state.listenersCount = amount,
    
    SET_RADIO_CONNECTION: (state, radioConnection:HubConnection) => state.radioConnection = radioConnection,
    TOGGLE_PLAYER_TIMER_OVERLAY: state => state.PlayerTimerOverLay = ! state.PlayerTimerOverLay,
    
    SET_PLAYBACK_PLAYING_STATUS: (state, isPlaying:boolean) => state.isPlaying = isPlaying, 
    SET_CONNECTED_STATUS: (state, isConnected:boolean) => state.isConnected = isConnected,
    
    SET_SUBSCRIBE_TIME: (state, minutes:number) => {
        let subscribeEndDate = new Date();
        subscribeEndDate.setMinutes(subscribeEndDate.getMinutes() + minutes);
        subscribeEndDate.setTime(subscribeEndDate.getTime() - subscribeEndDate.getTimezoneOffset()*60*1000)
        
        state.subscribeEndDate = subscribeEndDate;
    },


}

const getters = <GetterTree<State, any>>{
    getRadioConnection: state => state.radioConnection,
    getPlayerTimerOverlayActive: state => state.PlayerTimerOverLay,
    getPlayingStatus: state => state.isPlaying,
    getConnectedState: state => state.isConnected,
    getSubscribeEndDate: state => state.subscribeEndDate,
    
    
    getPlaybackInfo: state => state.playbackInfo,
    getCurrentTrack: state => state.currentSongInfo,
    getCurrentTrackStartingTime: state => state.currentTrackStartingTime,
    getPriorityQueuedTracks: state => state.priorityQueuedTracks,
    getFillerQueuedTracks: state => state.fillerQueuedTracks,
    getSecondaryQueuedTracks: state => state.secondaryQueuedTracks,

    // playback settings
    getPlaybackState: state => state.playbackState,
    getMaxRequestsPerUser: state => state.maxRequestsPerUser,
    getListenersCount: state => state.listenersCount,
}

const actions = <ActionTree<State, any>>{
    requestTrack(context, request: trackRequest): Promise<AxiosResponse> {
        const isMod: boolean = context.rootGetters["profileModule/isMod"]
        
        let route:string = ""
        
        if (isMod &&  context.rootGetters['userSettingsModule/getMakeRequestAsMod']){
            route = `/api/playback/mod/request/${request.trackId}`;
        } else{
            route = `api/playback/request/${request.trackId}`;
        }
        if (request.message !== undefined){
            route += `?message=${request.message}`
        }
        
        return axios.put(route, null)
    }
}

const PlaybackModule = {
    namespaced: true,
    state: new State(),
    mutations: mutations,
    getters: getters,
    actions: actions
}

export default PlaybackModule;

export interface trackRequest {
    trackId: string,
    message: string | undefined,
}