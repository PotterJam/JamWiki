<template>
  <div class="hello">
    <h1>{{ msg }}</h1>

    <div id="logo">
      <img alt="Jam Logo" src="../assets/jam-logo.png" />
    </div>
  </div>
</template>

<script>
export default {
  name: "WikiHome",
  props: {
    msg: String
  }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: circle;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}

#logo {
  padding: 1em;
}
</style>
