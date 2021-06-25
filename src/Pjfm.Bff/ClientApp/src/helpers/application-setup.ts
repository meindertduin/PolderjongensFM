import App from "@/App.vue";
import {userPlaybackInfo, userPlaybackSettings, userSettings} from "@/common/types";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";


export const setupApplication = (app: App): void => {
    setSockerConnections(app);
    setUserPreferences(app);
    app.$store.dispatch('userModule/getUser');
}

const setSockerConnections = (app: App): void => {
    let radioConnection: HubConnection | null;

    radioConnection = new HubConnectionBuilder()
        .withUrl(`${process.env.VUE_APP_API_BASE_URL}/api/radio`)
        .build();

    radioConnection.start()
        .then(() => {
        });

    radioConnection.on("ReceivePlaybackInfo", (playbackInfo: userPlaybackInfo) => {
        app.$store.commit('playbackModule/SET_PLAYBACK_INFO', playbackInfo);
        app.$store.dispatch('userModule/tryCalculateRequestedAmount');
    });

    radioConnection.on("IsConnected", (connected: boolean) => {
        app.$store.commit('playbackModule/SET_CONNECTED_STATUS', connected);
    });

    radioConnection.on("SubscribeTime", ((minutes: number) => {
        app.$store.commit('playbackModule/SET_SUBSCRIBE_TIME', minutes);
    }))

    radioConnection.on("ListenersCountUpdate", ((amount: number) => {
        app.$store.commit('playbackModule/SET_LISTENERS_COUNT', amount);
    }));

    radioConnection.on("ReceivePlayingStatus", (isPlaying: boolean) => {
        app.$store.commit("playbackModule/SET_PLAYBACK_PLAYING_STATUS", isPlaying);
    });

    radioConnection.on("PlaybackSettings", (playbackSettings: userPlaybackSettings) => {
        app.$store.commit("playbackModule/SET_PLAYBACK_SETTINGS", playbackSettings);
        app.$store.dispatch("userModule/tryCalculateRequestedAmount");
    });
    
    app.$store.commit('playbackModule/SET_RADIO_CONNECTION', radioConnection);
}

const setUserPreferences = (app: App): void => {
    const userSettings = app.$store.getters['userSettingsModule/loadUserSettings'] as userSettings;
    // @ts-ignore
    app.$vuetify.theme.dark = userSettings.darkMode;
}