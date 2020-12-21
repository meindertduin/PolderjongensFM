import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import { vuexOidcCreateRouterMiddleware } from 'vuex-oidc'
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
      isPublic: false,
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
      isPublic: false,
    }
  },
  {
    path: '/signin-oidc-error',
    name: 'oidcCallbackError',
    component: OidcCallbackError,
    meta: {
      isPublic: false,
    }
  },
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: routes,
})

router.beforeEach(authMiddleware(store, "oidcStore"));

// router.beforeEach((to, from, next) => {
//   if (to.meta.middleware) {
//     // checks if its an array or not
//     const middleware = Array.isArray(to.meta.middleware)
//         ? to.meta.middleware
//         : [to.meta.middleware];
//
//     const context = {
//       from,
//       next,
//       router,
//       to,
//     };
//     const nextMiddleware = nextFactory(context, middleware, 1);
//
//     return middleware[0]({ ...context, next: nextMiddleware });
//   }
//
//   return next();
// });
//
//
// function nextFactory(context, middleware, index) {
//   const subsequentMiddleware = middleware[index];
//   // If no subsequent Middleware exists,
//   // the default `next()` callback is returned.
//   if (!subsequentMiddleware) return context.next;
//
//   return (...parameters) => {
//     // Run the default Vue Router `next()` callback first.
//     context.next(...parameters);
//     // Than run the subsequent Middleware with a new
//     // `nextMiddleware()` callback.
//     const nextMiddleware = nextFactory(context, middleware, index + 1);
//     subsequentMiddleware({ ...context, next: nextMiddleware });
//   };
// }

export default router
