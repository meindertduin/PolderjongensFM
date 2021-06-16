import {ActionContext, ActionTree, GetterTree, Module, MutationTree} from "vuex";
import {User} from "@/store/userModule/user";
import axios from "axios";

class UserState {
    user?: User;
}

const getters: GetterTree<UserState, any> = {
    user: (state) => state.user,
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

