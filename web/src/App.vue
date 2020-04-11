<template>
  <v-app>
    <div id="header">
      <div id="nav" v-if="this.$store.getters.isAuthenticated">
        <router-link to="/">Wikis</router-link> |
        <router-link to="/account">Account</router-link>
        <!-- Removing tags tab until finished -->
        <!--<router-link to="/tags">Tags</router-link> -->
      </div>
    </div>
    <router-view/>
  </v-app>
</template>

<script>
import { AUTH_LOGOUT } from './store/actions/auth'

export default {
  created: function () {
    this.axios.interceptors.response.use(undefined, function (err) {
      // eslint-disable-next-line
      return new Promise(function (resolve, reject) {
        if (err.status === 401 && err.config && !err.config.__isRetryRequest) {
        // if you ever get an unauthorized, logout the user
          this.$store.dispatch(AUTH_LOGOUT)
        // you can also redirect to /login if needed !
        }
        throw err;
      });
    });

    this.axios.interceptors.request.use(function (config) {
      const token = localStorage.getItem('user-token')
      if (token) {
        config.headers.Authorization = token;
      }

      return config;
    });
  }
}
</script>

<style>
  body {
    margin: 0;
  }

  #app {
    font-family: 'Avenir', Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    text-align: center;
    color: #2c3e50;
  }

  #header {
    display: flex;
    justify-content: center;
  }

  #nav {
    padding: 30px;
  }

  #nav a {
    font-weight: bold;
    color: #2c3e50;
  }

  #nav a.router-link-exact-active {
    color: #c8377a;
  }
</style>
