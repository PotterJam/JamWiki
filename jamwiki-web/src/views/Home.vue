<template>
  <div class="home">
    <WikiHome msg="Welcome to Jam Wiki"/>
    <div id="wiki-input-wrapper">
      <b-form-input id="wiki-input" v-model="wikiName" placeholder="Enter wiki name"></b-form-input>
      <b-button id="get-wiki-but" variant="primary">Get wiki</b-button>
    </div>
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

<style scoped>

#wiki-input-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 1em;
}

#wiki-input {
  display: flex;
  margin-right: 1em; 
  width: 11em;
}

#get-wiki-but {
  width: 5.5em;
}

</style>