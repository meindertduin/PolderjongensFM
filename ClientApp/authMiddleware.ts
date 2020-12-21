
import axios from 'axios'

export default (store:any, vuexNamespace:any) => {
    return (to, from, next) => {
        store.dispatch((vuexNamespace ? vuexNamespace + '/' : '') + 'oidcCheckAccess', to)
            .then((hasAccess:boolean) => {
                if (hasAccess) {
                    axios.get(`https://localhost:5001/api/spotify/account/me`, {
                        maxRedirects: 0
                    })
                    .then((response) => {
                        next();
                    })
                    .catch((err) => {
                        console.log("super error: ");
                        console.log(err);
                        window.location.href = 'http://jordaan.depolderjongens.nl/api/spotify/authorize'
                    })
                }
            })
    }
}