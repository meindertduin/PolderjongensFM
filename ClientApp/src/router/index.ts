import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import { vuexOidcCreateRouterMiddleware } from 'vuex-oidc'
import store from '@/store'
import vuetify from "@/plugins/vuetify";
import OidcCallback from "@/views/OidcCallback.vue";
import OidcCallbackError from "@/views/OidcCallbackError.vue";

import {defaultSettings} from "@/common/objects";
import {userSettings} from "@/common/types";
import Search from "@/views/Search.vue";
import DjView from "@/views/DjView.vue";
import AppView from "@/views/AppView.vue";
import Landing from "@/views/Landing.vue";

Vue.use(VueRouter)

const routes: Array<RouteConfig> = [
  {
    path: '/',
    name: 'App',
    component: AppView,
    meta: {
      isPublic: true,
    }
  },
  {
    path: '/search',
    name: 'Search',
    component: Search,
    meta: {
      isPublic: false,
    }
  },
  {
    path: '/Test',
    name: 'Test',
    component: () => import(/* webpackChunkName: "Test" */ '../views/Test.vue')
  },
  {
    path: '/oidc-callback',
    name: 'OidcCallback',
    component: OidcCallback,
    meta: {
      isPublic: true,
    }
  },
  {
    path: '/signin-oidc-error',
    name: 'oidcCallbackError',
    component: OidcCallbackError,
    meta: {
      isPublic: true,
    }
  },
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: routes,
})
router.beforeEach(vuexOidcCreateRouterMiddleware(store, 'oidcStore'))
export default router
