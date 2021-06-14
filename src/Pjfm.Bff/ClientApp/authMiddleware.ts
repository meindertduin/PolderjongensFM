
export default (store:any, vuexNamespace:any) => {
    return (to:any, from:any, next:any) => {
        store.dispatch((vuexNamespace ? vuexNamespace + '/' : '') + 'oidcCheckAccess', to)
            .then((hasAccess:boolean) => {
                if (hasAccess) {
                    if (to.meta.requiresSpotAuth){
                        if (store.getters["profileModule/userProfile"] !== null){
                            console.log(store.getters["profileModule/isSpotifyAuthenticated"])

                            if (store.getters["profileModule/isSpotifyAuthenticated"]){
                                next()
                            }
                            else{
                                window.location.href = process.env.VUE_APP_API_BASE_URL + "/api/spotify/account/authenticate"
                            }
                        }
                        else {
                            if (process.env.VUE_APP_BASE_URL)
                                window.location.href = process.env.VUE_APP_BASE_URL;
                        }
                    }
                    
                    next()
                }
            })
    }
}