import { GetterTree, MutationTree, ActionTree } from "vuex"

class State {
    public error: boolean = false;
    public message: string | null = null
}

const mutations = <MutationTree<State>>{
    ERROR_OFF: state => state.error = false,
    ERROR_ON: state => state.error = true,
    SET_ERROR_MESSAGE: (state, message: string) => {
        state.message = message;
    },
}

const getters = <GetterTree<State, any>>{
    showError: state => state.error,
    errorMessage: state => state.message,
}

const actions = <ActionTree<State, any>>{

}

const ErrorModule = {
    namespaced: true,
    state: new State(),
    mutations: mutations,
    getters: getters,
}

export default ErrorModule;