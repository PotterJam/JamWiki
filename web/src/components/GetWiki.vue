<template>
  <div>
    <div id="wiki-input-wrapper">
      <b-form-input id="wiki-input" v-model="wikiName" placeholder="Enter wiki name..."></b-form-input>
      <v-btn text icon color="primary" v-on:click="getWiki" id="wiki-but"><v-icon>mdi-alpha-b-circle</v-icon></v-btn>
      <v-btn text icon color="primary" v-on:click="addWiki" id="wiki-but"><v-icon>mdi-plus</v-icon></v-btn>
      <v-btn text icon color="primary" v-on:click="deleteWiki" id="wiki-but"><v-icon>mdi-delete</v-icon></v-btn>
    </div>
    <div id="wiki-body-wrapper">
      <v-textarea solo v-if="wikiBody !== null" label="Wiki body goes here..." rows="8" v-model="wikiBody"></v-textarea>
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
  display: block;
  margin: auto;
  padding: 1em;
  width: 40em;
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
}
</style>
