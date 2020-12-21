


export default (store:any, vuexNamespace:any) => {
    return (to, from, next) => {
        store.dispatch((vuexNamespace ? vuexNamespace + '/' : '') + 'oidcCheckAccess', to)
            .then((hasAccess:boolean) => {
                if (hasAccess) {
                    const isSpotifyAuthenticated = store.getters["profileModule/isSpotifyAuthenticated"];
                    if (isSpotifyAuthenticated){
                        next()
                    } else{
                        window.location.href = "https://localhost:5001/api/spotify/account/authenticate"
                    }
                }
            })
    }
}