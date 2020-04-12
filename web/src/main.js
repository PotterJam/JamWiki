import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import BootstrapVue from "bootstrap-vue";
import axios from "axios";
import vuetify from "./plugins/vuetify";
import store from "./store";
import GAuth from "vue-google-oauth2";

const gauthOption = {
  clientId: "357451785008-mvk7i4sla4qdq9nh8u5uml09g5gdqt26.apps.googleusercontent.com",
  scope: "profile email",
  prompt: "select_account"
};

Vue.use(GAuth, gauthOption);

Vue.prototype.axios = axios;

Vue.use(BootstrapVue);
Vue.config.productionTip = false;

new Vue({
  router,
  vuetify,
  store,
  render: h => h(App)
}).$mount("#app");
