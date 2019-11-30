<template>
  <div>
    <div v-if="this.$store.getters.isAuthenticated" class="account">
      <h1>Account</h1>
      <h3>your name</h3>
      <h3>email</h3>
      <v-btn
          color="primary"
          @click="handleClickSignOut"
          :disabled="!isInit"
        >sign out</v-btn>
    </div>
    <div id="spinny">
      <v-progress-circular
        :size="80"
        :width="7"
        color="primary"
        indeterminate
        v-if="!this.$store.getters.isAuthenticated"
      ></v-progress-circular>
    </div>
  </div>
</template>

<script>
import { AUTH_LOGOUT } from '../store/actions/auth'

/* eslint-disable */
export default {
  name: 'Account',
  data() {
    return {
      isInit: false
    };
  },
  methods: {
    handleClickSignOut() {
       this.$gAuth.signOut()
        .then(() => this.$store.dispatch(AUTH_LOGOUT))
            .then(() => this.$router.go('/login'));
    }
  },
  created() {
    let that = this;
    let checkGauthLoad = setInterval(function() {
      that.isInit = that.$gAuth.isInit;
      if (that.isInit) clearInterval(checkGauthLoad);
    }, 1000);
  }
};
</script>

<style scoped>
  #spinny {
    display: flex;
    justify-content: center;

    /* padding hack to center spinny - needed because it's during routing so elements randomly dissapear */
    padding-top: 45vh;
  }
</style>