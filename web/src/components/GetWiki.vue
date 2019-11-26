<template>
  <div>
    <div id="wiki-input-wrapper">
      <b-form-input id="wiki-input" v-model="wikiName" placeholder="Enter wiki name..."></b-form-input>
      <b-button v-on:click="getWiki" id="wiki-but" variant="primary">Get</b-button>
      <b-button v-on:click="addWiki" id="wiki-but" variant="primary">Add</b-button>
      <b-button v-on:click="deleteWiki" id="wiki-but" variant="primary">Delete</b-button>
    </div>
    <div id="wiki-body-wrapper">
      <b-form-textarea v-if="wikiBody !== null" id="wiki-body" placeholder="Wiki body goes here..." rows="8" v-model="wikiBody"></b-form-textarea>
    </div>
  </div>
</template>

<script>
export default {
  name: 'GetWiki',
  data () {
    return {
      wikiName: '',
      wikiBody: ''
    }
  },
  methods: {
    getWiki() {
      this.axios
          .get(`http://localhost:5000/api/wiki?name=${this.wikiName}`)
          .then(response => {
            this.wikiName = response.data.name;
            this.wikiBody = response.data.body;
          });
    },
    addWiki() {
      this.axios
          .post('http://localhost:5000/api/wiki', {
            name: this.wikiName,
            body: this.wikiBody,
            tags: 'tags'
          }).then(() => this.wikiBody = 'Wiki added');
    },
    deleteWiki() {
      this.axios
          .delete('http://localhost:5000/api/wiki', {
            data: { name: this.wikiName }
          }).then(() => this.wikiBody = 'Wiki deleted')
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

#wiki-body-wrapper {
  display: flex;
  justify-content: center;
  padding: 1em;
}

#wiki-body {
  width: 27.2em;
}

#wiki-input-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  padding-top: 1em;
}

#wiki-input {
  display: flex;
  margin: 0.2em;
  width: 11em;
}

#wiki-but {
  margin: 0.2em;
  width: 5em;
}
</style>
