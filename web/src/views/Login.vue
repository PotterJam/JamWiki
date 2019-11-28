<template>
  <div id="gauth">
    <div>
    <v-row>
      <v-btn
        color="primary"
        @click="handleClickSignIn"
        v-if="!isSignIn"
        :disabled="!isInit"
      >sign in</v-btn>
      <v-btn
        color="primary"
        @click="handleClickSignOut"
        v-if="isSignIn"
        :disabled="!isInit"
      >sign out</v-btn>
    </v-row>
    <v-row>
      <p>isInit: {{isInit}}</p>
      <p>isSignIn: {{isSignIn}}</p>
    </v-row>
    </div>
  </div>
</template>

<script>
import { AUTH_REQUEST, AUTH_LOGOUT } from '../store/actions/auth'

/* eslint-disable */
export default {
  name: "GoogleAuth",
  data() {
    return {
      isInit: false,
      isSignIn: false
    };
  },
  methods: {
    handleClickSignIn() {
      this.$gAuth.signIn()
        .then(googleUser =>
          this.$store.dispatch(AUTH_REQUEST, googleUser.getAuthResponse().id_token))
            .then(() => this.$router.push('/'))
    },
    handleClickSignOut() {
       this.$gAuth.signOut()
        .then(() =>
          this.$store.dispatch(AUTH_LOGOUT))
            .then(() => this.$router.push('/login'));
    }
  },
  created() {
    let that = this;
    let checkGauthLoad = setInterval(function() {
      that.isInit = that.$gAuth.isInit;
      that.isSignIn = that.$gAuth.isAuthorized;
      if (that.isInit) clearInterval(checkGauthLoad);
    }, 1000);
  }
};
</script>

<style scoped>
  #gauth {
    display: flex;
    justify-content: center;
  }
</style>