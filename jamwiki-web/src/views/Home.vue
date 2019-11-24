<template>
  <div class="home">
    <WikiHome msg="Welcome to Jam Wiki"/>
    <div id="wiki-input-wrapper">
      <div id="wiki-input">
        <b-form-input v-model="wikiName" placeholder="Enter wiki name"></b-form-input>
        <b-button id="get-wiki-but" variant="primary">Get wiki</b-button>
      </div>
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
}

#wiki-input {
  align-items: center;
  display: flex;
  padding: 1em;
  width: 20%;
  min-width: 200px;
}

#get-wiki-but {
  margin: 1em;
  min-width: 5.5em;
}

</style>