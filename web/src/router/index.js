import Vue from 'vue'
import VueRouter from 'vue-router'
import store from '../store' // your vuex store

Vue.use(VueRouter)

const ifNotAuthenticated = (to, from, next) => {
  if (!store.getters.isAuthenticated) {
    next()
    return
  }
  next('/')
}

const ifAuthenticated = (to, from, next) => {
  if (store.getters.isAuthenticated) {
    next()
    return
  }
  next('/login')
}

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('../views/Login.vue'),
    beforeEnter: ifNotAuthenticated
  },
  {
    path: '/',
    name: 'home',
    component: () => import('../views/Home.vue'),
    beforeEnter: ifAuthenticated
  },
  {
    path: '/account',
    name: 'Account',
    component: () => import('../views/Account.vue'),
    beforeEnter: ifAuthenticated
  },
  {
    path: '/wikiswithtag/:tagName',
    name: 'WikisWithTag',
    component: () => import('../views/WikisWithTag.vue'),
    beforeEnter: ifAuthenticated
  },
  {
    path: '/tags',
    name: 'WikiTags',
    component: () => import('../views/WikiTags.vue'),
    beforeEnter: ifAuthenticated
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

export default router
