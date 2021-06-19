import {ActionContext, ActionTree, GetterTree, Module, MutationTree} from "vuex";
import {User, UserRole} from "@/store/userModule/user";
import axios from "axios";
import {trackDto} from "@/common/types";

class UserState {
    user?: User;
    userRequestedAmount: number = 0;
    userAuthenticated: boolean = false;
    userEmailConfirmed: boolean = false;
    isMod: boolean = false;
}

const getters: GetterTree<UserState, any> = {
    user: (state): User | undefined => state.user,
    userIsMod: (state): boolean => state.isMod,
    userEmailConfirmed: (state): boolean => state.user?.emailConfirmed ?? false,
    userSpotifyAuthenticated: (state): boolean => state.user?.spotifyAuthenticated ?? false,
    userId: (state): string | undefined => state.user?.id,
    getUserRequestedAmount: (state): number => state.userRequestedAmount,
    userAuthenticated: (state): boolean => state.userAuthenticated,
}

const mutations: MutationTree<UserState> = {
    SET_USER: (state, user: User) => state.user = user,
    SET_USER_REQUESTED_AMOUNT: (state, amount: number) => state.userRequestedAmount = amount,
    SET_USER_AUTHENTICATED: (state, authenticated: boolean) => state.userAuthenticated = authenticated,
    SET_IS_MOD: (state, isMod: boolean) => state.isMod = isMod,
}

const actions: ActionTree<UserState, any> = {
    getUser(context: ActionContext<UserState, any>) {
        return axios.get('/api/user/me').then((response) => {
            if (response.status === 200) {
                const user = response.data as User;
                console.log(user);
                let userAuthenticated = false;
                if (user != null) {
                    userAuthenticated = true;
                }
                context.commit('SET_USER_AUTHENTICATED', userAuthenticated);
                context.commit('SET_IS_MOD', user.roles.includes(UserRole.Mod));
                context.commit('SET_USER', user);
            }
        });
    },
    tryCalculateRequestedAmount(context: ActionContext<UserState, any>) {
        const secondaryTracks: trackDto[] = context.rootGetters["playbackModule/getSecondaryQueuedTracks"];
        const userId: string | undefined = context.getters["userId"];

        if (secondaryTracks.length > 0 && userId != null) {
            const userRequestedAmount = secondaryTracks
                .filter(track => {
                    return track.user.id === userId;
                })
                .length;
            context.commit("SET_USER_REQUESTED_AMOUNT", userRequestedAmount);
        } else { // TODO: check if this payload of 0 is still needed
            context.commit("SET_USER_REQUESTED_AMOUNT", 0);
        }
    }
}

const UserModule: Module<UserState, any> = {
    namespaced: true,
    state: new UserState(),
    getters: getters,
    mutations: mutations,
    actions: actions,
};

export default UserModule;

