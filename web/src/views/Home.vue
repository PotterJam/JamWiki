<template>
  <div class="home">
    <WikiHome msg="Welcome to Jam Wiki"/>
    <GoogleAuth/>
    <GetWiki/>
  </div>
</template>

<script>
// @ is an alias to /src
import WikiHome from '@/components/WikiHome.vue'
import GetWiki from '@/components/GetWiki.vue'
import GoogleAuth from '@/components/GoogleAuth.vue'

export default {
  name: 'home',
  components: {
    WikiHome,
    GetWiki,
    GoogleAuth
  }
}
</script>

<style scoped>

</style>