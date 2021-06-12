import { GetterTree, MutationTree, ActionTree } from "vuex"
import {alertInfo} from "@/common/types";

class State {
    public id: number = 0;
    public type: string | null = null;
    public message: string | null = null
}

const mutations = <MutationTree<State>>{
    SET_ALERT: (state, alert: alertInfo | null) => {
        state.id++;
        
        if(alert != null) {
            state.type = alert.type;
            state.message = alert.message;
        }else{
            state.type = null;
            state.message = null;
        }
    },
}

const getters = <GetterTree<State, any>>{
    alertId: state => state.id,
    alertType: state => state.type,
    alertMessage: state => state.message,
}

const actions = <ActionTree<State, any>>{

}

const AlertModule = {
    namespaced: true,
    state: new State(),
    mutations: mutations,
    getters: getters,
}

export default AlertModule;