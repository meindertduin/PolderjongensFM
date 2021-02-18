import {ActionTree, GetterTree, MutationTree} from "vuex"
import {User} from "oidc-client";



class State {
    public accessToken: string | null = null;
    public idToken: string | null = null;
    public refreshToken: string | null = null;
    public user: User | null = null;
    public scopes: string | null = null;
    public isChecked: boolean = false;
    public eventsAreBound: boolean = false;
    public error: any = null;
}

const isAuthenticated = (state: State) => {
    if (state){
        return true;
    }
    
    return false;
}


const mutations = <MutationTree<State>> {
    
}

const getters = <GetterTree<State, any>> {
    oidcIsAuthenticated: state => isAuthenticated(state),
    oidcUser: state => state.user,
    oidcAccessToken: state => state.accessToken,
}

const actions = <ActionTree<State, any>> {
    
}

const OidcModule = {
    namespaced: true,
    state: new State(),
    mutations: mutations,
    getters: getters,
    actions: actions,
}

export default OidcModule;