<template>
  <div class="home">
    <WikiHome msg="Welcome to Jam Wiki"/>
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
