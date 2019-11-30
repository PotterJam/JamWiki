<template>
  <div class="home">
    <v-expand-transition>
      <v-card
        color="#fafafa"
        flat
        v-show="wikiBody === null"
        class="mx-auto"
      >
        <WikiHome msg="Welcome to Jam Wiki"/>
      </v-card>
    </v-expand-transition>
    <div id="wiki-input-wrapper">
      <v-combobox
        id="wiki-input"
        v-model="wikiName"
        autocomplete="off"
        :items="wikiNames"
        :loading="isLoading"
        :search-input.sync="search"
        hide-selected
        color="secondary"
        label="Wikis"
        placeholder="Start typing to Search"
        prepend-icon="mdi-magnify"
      >
        <template v-slot:no-data>
        <v-list-item>
          <v-list-item-content>
            <v-list-item-title>
              No results matching "<strong>{{ search }}</strong>". Press <kbd>enter</kbd> to create a new one
            </v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </template>
      </v-combobox>
      <v-btn text icon color="primary" v-on:click="updateWiki" id="wiki-but"><v-icon>mdi-content-save</v-icon></v-btn>
      <v-btn text icon color="primary" v-on:click="addWiki" id="wiki-but"><v-icon>mdi-plus</v-icon></v-btn>
      <v-btn text icon color="primary" v-on:click="deleteWiki" id="wiki-but"><v-icon>mdi-delete</v-icon></v-btn>
    </div>
    <div id="wiki-body-wrapper">
      <v-textarea solo v-if="wikiBody !== null" label="Wiki body goes here..." rows="30" v-model="wikiBody"></v-textarea>
    </div>
  </div>
</template>

<script>
import WikiHome from '@/components/WikiHome'

export default {
  name: 'home',
  components: {
    WikiHome
  },
  data () {
    return {
      wikiName: null,
      wikiBody: null,
      wikiNames: [],
      isLoading: false,
      search: null,
      colors: ['green', 'purple', 'indigo', 'cyan', 'teal', 'orange'],
      editing: null,
      index: -1,
      menu: false
    }
  },
  methods: {
    edit(index, item) {
      if (!this.editing) {
        this.editing = item
        this.index = index
      } else {
        this.editing = null
        this.index = -1
      }
    },
    updateWiki() {
      this.axios
          .post('http://localhost:5000/api/wiki/update', {
            name: this.wikiName,
            body: this.wikiBody,
            tags: 'tags'
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
          }).then(() => {
            this.wikiBody = 'Wiki deleted';
            this.wikiNames = this.wikiNames.filter(x => x != this.wikiName);
          })
    }
  },
  watch: {
    wikiName(newWikiName, prevWikiName) {
      if (newWikiName === prevWikiName) return

      if (!this.wikiNames.includes(newWikiName)) {
        this.axios
            .post('http://localhost:5000/api/wiki', {
              name: newWikiName,
              body: '',
              tags: 'tags'
            }).then(() => {
              this.wikiBody = 'Wiki added'
              this.wikiNames.push(newWikiName);
            });
      } else {
        this.axios.get(`http://localhost:5000/api/wiki?name=${newWikiName}`)
                  .then(response => {
                    this.wikiBody = response.data.body;
                  });
      }
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
          console.log(err)
        })
        .finally(() => this.isLoading = false)
    },
  }
}
</script>

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
  width: 30em;
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