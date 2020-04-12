<template>
  <div>
    <div v-if="this.$store.getters.isAuthenticated" class="account">
      <h1 class="pb-5">Account</h1>
      <v-card
        id="accountCard"
        class="mx-auto pa-2"
        width="300px"
        height="120px"
        outlined
      >
        <h3>
          <v-icon color="secondary">mdi-email</v-icon> {{ this.userEmail }}
        </h3>
        <v-btn
          large
          id="signoutBut"
          color="primary"
          @click="handleClickSignOut"
          :disabled="!isInit"
        >
          <v-icon left>mdi-google</v-icon>
          sign out
        </v-btn>
      </v-card>
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
import { AUTH_LOGOUT } from "../store/actions/auth";

/* eslint-disable */
export default {
  name: "Account",
  data() {
    return {
      isInit: false,
      userEmail: ""
    };
  },
  methods: {
    async handleClickSignOut() {
      await this.$gAuth.signOut();
      await this.$store.dispatch(AUTH_LOGOUT);
      this.$router.go("/login");
    }
  },
  created() {
    let that = this;
    let checkGauthLoad = setInterval(function() {
      that.isInit = that.$gAuth.isInit;
      if (that.isInit) {
        clearInterval(checkGauthLoad);
        that.userEmail = that.$gAuth.GoogleAuth.currentUser.get().Qt.zu;
      }
    }, 1000);
  }
};
</script>

<style scoped>
#accountCard {
  display: flex;
  justify-content: space-around;
  align-content: space-around;
  flex-direction: column;
  height: 100%;
}

#spinny {
  display: flex;
  justify-content: center;

  /* padding hack to center spinny - needed because it's during routing so elements randomly dissapear */
  padding-top: 45vh;
}
</style>
