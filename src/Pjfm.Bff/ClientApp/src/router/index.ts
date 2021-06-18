import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import store from '@/store'
import OidcCallback from "@/views/OidcCallback.vue";
import OidcCallbackError from "@/views/OidcCallbackError.vue";

import Search from "@/views/Search.vue";
import AppView from "@/views/AppView.vue";
import authMiddleware from "../../authMiddleware";

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
      requiresSpotAuth: true,
    }
  },
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: routes,
})

export default router
