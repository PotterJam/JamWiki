<template>
  <div id="wiki-input-wrapper">
    <b-form-input id="wiki-input" v-model="wikiName" placeholder="Enter wiki name"></b-form-input>
    <b-button id="get-wiki-but" variant="primary">Get wiki</b-button>
  </div>
</template>

<script>
export default {
  name: 'WikiHome',
  props: {
    msg: String
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
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
