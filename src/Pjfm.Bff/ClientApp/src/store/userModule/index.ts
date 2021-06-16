import {ActionContext, ActionTree, GetterTree, Module, MutationTree} from "vuex";
import {User, UserRole} from "@/store/userModule/user";
import axios from "axios";

class UserState {
    user?: User;
}

const getters: GetterTree<UserState, any> = {
    user: (state): User | undefined => state.user,
    userISMod: (state): boolean => state.user?.roles.includes(UserRole.Mod) ?? false,
    userEmailConfirmed: (state): boolean => state.user?.emailConfirmed ?? false,
    userSpotifyAuthenticated: (state): boolean => state.user?.spotifyAuthenticated ?? false,
    userId: (state): string | undefined => state.user?.id,
}

const mutations: MutationTree<UserState> = {
    SET_USER: (state, user: User) => state.user = user,
}

const actions: ActionTree<UserState, any> = {
    getUser(context: ActionContext<UserState, any>) {
        return axios.get('/api/user/me').then(({data}) => {
            const user = data as User;
            context.commit('SET_USER', user);
        });
    }
}

const UserModule: Module<UserState, any> = {
    namespaced: true,
    state: new UserState(),
    actions: actions,
};

export default UserModule;

