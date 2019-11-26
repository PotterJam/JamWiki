<template>
  <div class="hello">
    <v-row>
      <v-btn
        color="primary"
        @click="handleClickLogin"
        :disabled="!isInit"
      >get authCode</v-btn>
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
      <p>isInit: {{isInit}}</p>
      <p>isSignIn: {{isSignIn}}</p>
    </v-row>
  </div>
</template>

<script>
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
    handleClickLogin() {
      this.$gAuth
        .getAuthCode()
        .then(authCode => {
          //on success
          console.log("authCode", authCode);
        })
        .catch(error => {
          //on fail do something
        });
    },
    handleClickSignIn() {
      this.$gAuth
        .signIn()
        .then(GoogleUser => {
          //on success do something
          console.log("GoogleUser", GoogleUser);
          console.log("getId", GoogleUser.getId());
          console.log("getBasicProfile", GoogleUser.getBasicProfile());
          console.log("getAuthResponse", GoogleUser.getAuthResponse());
          console.log(
            "getAuthResponse",
            this.$gAuth.GoogleAuth.currentUser.get().getAuthResponse()
          );
          this.isSignIn = this.$gAuth.isAuthorized;
        })
        .catch(error => {
          //on fail do something
        });
    },
    handleClickSignOut() {
      this.$gAuth
        .signOut()
        .then(() => {
          //on success do something
          this.isSignIn = this.$gAuth.isAuthorized;
        })
        .catch(error => {
          //on fail do something
        });
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
h3 {
  margin: 40px 0 0;
}
</style>