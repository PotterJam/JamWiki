<template>
  <div class="home">
    <WikiHome msg="This is an initial screen"/>
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
