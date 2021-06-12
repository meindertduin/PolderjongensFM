import axios from '@/plugins/axios';

import { GetterTree, MutationTree, ActionTree } from "vuex"
import {applicationUser, identityProfile, trackDto} from "@/common/types";

class State {
    public userProfile: identityProfile | null = null;
    public playlistDialog : boolean = false
    public isMod : boolean = false;
    public isSpotifyAuthenticated: boolean = false;

    public userRequestedTracksAmount: number = 0;
}

const mutations = <MutationTree<State>>{
    SET_USER_PROFILE: (state, profile:identityProfile) => state.userProfile = profile,
    TOGGLE_PLAYLIST_DIALOG: state => state.playlistDialog = !state.playlistDialog,
    SET_PLAYLIST_DIALOG: (state, active:boolean) => state.playlistDialog = active,
    SET_USER_REQUESTED_AMOUNT: (state, amount:number) => state.userRequestedTracksAmount = amount,
}

const getters = <GetterTree<State, any>>{
    userProfile: (state) => state.userProfile? state.userProfile.userProfile : null,
    isMod: state => state.userProfile? state.userProfile.isMod: false,
    isSpotifyAuthenticated: state => state.userProfile? state.userProfile.isSpotifyAuthenticated: false,
    emailConfirmed: state => state.userProfile? state.userProfile.emailConfirmed : true,
    isPlaylistDialogActive: state => state.playlistDialog,
    userId: state => state.userProfile? state.userProfile.userProfile.id : null,
    userRequestedAmount: state => state.userRequestedTracksAmount,
}

const actions = <ActionTree<State, any>>{
    getUserProfile(context){
        return axios.get('api/auth/profile')
            .then(({data}) => {
                const profile:identityProfile = data.data;
                context.commit('SET_USER_PROFILE', profile)
                context.dispatch("tryCalculateRequestedAmount")
                    .catch((err) => console.log(err));
            })
            .catch(err => console.log(err));
    },
    tryCalculateRequestedAmount(context):void{
        const secondaryTracks: Array<trackDto> = context.rootGetters["playbackModule/getSecondaryQueuedTracks"];
        const userId = context.getters["userId"]
        
        if (secondaryTracks.length > 0 && userId !== null){
            const userRequestedAmount = secondaryTracks
                .filter(track => {
                    return track.user.id === userId;
                })
                .length;
            context.commit("SET_USER_REQUESTED_AMOUNT", userRequestedAmount);
        }
        else {
            context.commit("SET_USER_REQUESTED_AMOUNT", 0);
        }
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