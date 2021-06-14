import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import vuetify from './plugins/vuetify';
import axios from './plugins/axios'
import initInterceptor from "@/helpers/axiosInterceptor";

Vue.config.productionTip = false;
Vue.prototype.$axios = axios;

initInterceptor();
axios.defaults.baseURL = process.env.VUE_APP_API_BASE_URL;

new Vue({
  router,
  store,
  // @ts-ignore
  vuetify,
  render: h => h(App)
}).$mount('#app')
