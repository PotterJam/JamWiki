<template>
  <div class="home">
    <WikiHome msg="Welcome to Jam Wiki"/>
    <b-button variant="primary">Get wiki</b-button>
    <b-form-input v-model="wikiName" placeholder="Enter wiki name"></b-form-input>
  </div>
</template>

<script>
// @ is an alias to /src
import WikiHome from '@/components/WikiHome.vue'

export default {
  name: 'home',
  components: {
    WikiHome
  }
}
</script>
