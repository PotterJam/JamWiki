<template>
  <div class="home">
    <WikiHome msg="Welcome to Jam Wiki"/>
    <GetWiki/>
  </div>
</template>

<script>
// @ is an alias to /src
import WikiHome from '@/components/WikiHome.vue'
import GetWiki from '@/components/GetWiki.vue'

export default {
  name: 'home',
  components: {
    WikiHome,
    GetWiki
  }
}
</script>

<style scoped>

</style>