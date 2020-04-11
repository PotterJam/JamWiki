import Vue from 'vue';
import Vuetify from 'vuetify/lib';
import { TiptapVuetifyPlugin } from 'tiptap-vuetify'
import 'tiptap-vuetify/dist/main.css'
import 'vuetify/dist/vuetify.min.css'
import 'material-design-icons-iconfont/dist/material-design-icons.css'

Vue.use(Vuetify);
// Vuetify Object (as described in the Vuetify 2 documentation)
const vuetify = new Vuetify({
    theme: {
        themes: {
            light: {
                primary: '#c8377a',
                secondary: '#377ac8'
            },
        },
    }
})

Vue.use(TiptapVuetifyPlugin, {
  // the next line is important! You need to provide the Vuetify Object to this place.
  vuetify, // same as "vuetify: vuetify"
  // optional, default to 'md' (default vuetify icons before v2.0.0)
  iconsGroup: 'md'
})

export default vuetify