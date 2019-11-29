<template>
  <div id="loginCard">
    <v-card
      class="mx-auto"
      width="200px"
      height="230px"
      outlined
    >
      <div id="loginCard">
        <h1>JamWiki</h1>
        <img id="jamlogo" alt="Jam Logo" src="../assets/jam-logo.png">
        <div id="buttons">
        <v-btn
          color="primary"
          @click="handleClickSignIn"
          v-if="!this.$store.getters.isAuthenticated"
          :disabled="!isInit"
        >sign in</v-btn>
        <v-btn
          color="primary"
          @click="handleClickSignOut"
          v-if="this.$store.getters.isAuthenticated"
          :disabled="!isInit"
        >sign out</v-btn>
        </div>
      </div>
    </v-card>
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
        .then(googleUser => this.$store.dispatch(AUTH_REQUEST, googleUser.getAuthResponse().id_token))
        .then(() => this.$router.go('/'))
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
      if (that.isInit) clearInterval(checkGauthLoad);
    }, 1000);
  }
};
</script>

<style scoped>
  #loginCard {
    display: flex;
    justify-content: space-around;
    align-content: space-around;
    align-items: center;
    flex-direction: column;
    height: 100%;
  }

  #buttons {
    padding-bottom: 0.5em;
  }
</style>