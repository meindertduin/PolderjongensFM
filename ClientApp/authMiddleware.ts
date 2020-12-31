
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
                                window.location.href = "https://localhost:5001/api/spotify/account/authenticate"
                                //window.location.href = "https://localhost:5001"
                            }
                        }
                        else {
                            window.location.href = "https://localhost:8085";
                        }
                    }
                    
                    next()
                }
            })
    }
}