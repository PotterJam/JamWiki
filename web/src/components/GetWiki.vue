<template>
  <div>
    <div id="wiki-input-wrapper">
      <v-autocomplete
        id="wiki-input"
        v-model="wikiName"
        autocomplete="off"
        :items="wikiNames"
        :loading="isLoading"
        :search-input.sync="search"
        hide-no-data
        hide-selected
        color="secondary"
        item-text="Description"
        item-value="API"
        label="Wikis"
        placeholder="Start typing to Search"
        prepend-icon="mdi-magnify"
        return-object
      ></v-autocomplete>
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
      wikiName: null,
      wikiBody: null,
      wikiNames: [],
      isLoading: false,
      search: null
    }
  },
  methods: {
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
  },
  watch: {
    wikiName(newWikiName) {
      this.axios.get(`http://localhost:5000/api/wiki?name=${newWikiName}`)
                .then(response => {
                  this.wikiBody = response.data.body;
                });
    },
    search () {
      // Items have already been loaded
      if (this.wikiNames.length > 0) return
      // Items have already been requested
      if (this.isLoading) return
      this.isLoading = true
      // Lazily load input items
      this.axios
        .get(`http://localhost:5000/api/wiki/names`)
        .then(response => this.wikiNames = response.data)
        .catch(err => {
          // 
          console.log(err)
        })
        .finally(() => (this.isLoading = false))
    },
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
  display: block;
  margin: auto;
  padding-top: 1em;
  width: 20em;
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
