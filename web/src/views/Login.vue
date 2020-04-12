<template>
  <div id="loginCard">
    <v-card class="mx-auto" width="200px" height="230px" outlined>
      <div id="loginCard">
        <h1>JamWiki</h1>
        <img id="jamlogo" alt="Jam Logo" src="../assets/jam-logo.png" />
        <div id="buttons">
          <v-btn
            color="primary"
            @click="handleClickSignIn"
            v-if="!this.$store.getters.isAuthenticated"
            :loading="this.isLoading"
            :disabled="!isInit"
          >
            <v-icon left>mdi-google</v-icon>
            sign in
          </v-btn>
        </div>
      </div>
    </v-card>
  </div>
</template>

<script>
import { AUTH_REQUEST } from "../store/actions/auth";

/* eslint-disable */
export default {
  name: "Login",
  data() {
    return {
      isInit: false,
      isLoading: false
    };
  },
  methods: {
    async handleClickSignIn() {
      try {
        const googleUser = await this.$gAuth.signIn();
        this.isLoading = true;
        await this.$store.dispatch(
          AUTH_REQUEST,
          googleUser.getAuthResponse().id_token
        );
        this.$router.go("/");
      } catch (ex) {
        this.isLoading = false;
        console.log(ex);
      }
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
