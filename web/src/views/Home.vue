<template>
  <div class="home">
    <v-expand-transition>
      <v-card
        flat
        v-show="wikiBody === null"
        class="mx-auto"
      >
        <WikiHome msg="Welcome to Jam Wiki"/>
      </v-card>
    </v-expand-transition>
    <div class="mx-auto" id="wiki-input-wrapper">
      <v-autocomplete
        class="pr-3"
        v-model="wikiName"
        autocomplete="off"
        :items="wikiNames"
        :loading="isLoading"
        hide-selected
        hide-no-data
        color="secondary"
        label="Wikis"
        placeholder="Start typing to Search"
        prepend-icon="mdi-magnify"
      >
      </v-autocomplete>
      <v-btn text icon color="primary" :loading= "isSavingWiki" v-on:click="updateWiki" id="wiki-but"><v-icon>mdi-content-save</v-icon></v-btn>

      <v-dialog v-model="addWikiDialog" max-width="300">
        <template v-slot:activator="{ on }">
          <v-btn text icon color="primary" v-on="on" id="wiki-but"><v-icon>mdi-plus</v-icon></v-btn>
        </template>
        <v-card class="pl-5 pr-5">
          <v-card-title class="headline pb-4">Create a new wiki</v-card-title>
          <v-text-field
            v-model="createWikiModalInput"
            label="Wiki name"
            solo
          ></v-text-field>
          <v-combobox
            min-width="300"
            v-model="createWikiModalTags"
            :items="[]"
            label="Add wiki tags"
            multiple
            chips
            append-icon=""
            solo
          ></v-combobox>
          <v-card-actions id="addWikiButModal">
            <v-btn center color="green darken-1" text v-on:click="addWiki">Add wiki</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>

      <v-dialog v-model="areYouSureDeleteDialog" max-width="235">
        <template v-slot:activator="{ on }">
          <v-btn text icon color="primary" v-on="on" id="wiki-but"><v-icon>mdi-delete</v-icon></v-btn>
        </template>
        <v-card class="pl-5 pr-5">
          <v-card-title class="headline pb-1">Are you sure?</v-card-title>
          <v-card-actions class="d-flex justify-center" id="deleteWikiButModal">
            <v-btn color="green darken-1" text v-on:click="deleteWiki">Yes</v-btn>
            <v-btn color="red darken-1" text v-on:click="areYouSureDeleteDialog = false">No</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </div>

    <v-expand-transition>
      <div v-show="wikiBody !== null" class="mx-auto" id="wiki-body-wrapper">
        <div id="wiki-editor" class="pb-4">
          <WikiEditor :body.sync="wikiBody" />
        </div>

        <v-combobox
            min-width="300"
            v-model="wikiTags"
            :items="[]"
            label="Add wiki tags"
            multiple
            chips
            append-icon=""
            solo
          ></v-combobox>
      </div>
    </v-expand-transition>
  </div>
</template>

<script>
import WikiHome from '@/components/WikiHome'
import WikiEditor from '@/components/WikiEditor'

export default {
  name: 'home',
  components: {
    WikiHome,
    WikiEditor
  },
  data () {
    return {
      wikiName: null,
      wikiBody: null,
      wikiTags: [],
      wikiNames: [],
      isLoading: false,
      search: null,
      colors: ['green', 'purple', 'indigo', 'cyan', 'teal', 'orange'],
      editing: null,
      index: -1,
      menu: false,
      isSavingWiki: false,
      currentWikiName: null,
      addWikiDialog: false,
      areYouSureDeleteDialog: false,
      createWikiModalInput: null,
      createWikiModalTags: []
    }
  },
  created() {
    if (this.isLoading)
      return
    this.isLoading = true

    this.axios
      .get(`/api/wiki/names`)
      .then(response => this.wikiNames = response.data)
      .catch(err => console.log(err))
      .finally(() => this.isLoading = false)
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
      const self = this;
      self.isSavingWiki = true

      self.axios
          .post('/api/wiki/update', {
            name: self.currentWikiName,
            body: self.wikiBody,
            tags: self.wikiTags
          }).finally(() => setTimeout(function() {
            self.isSavingWiki = false
          }, 250)); // this adds a delay so the loading works
    },
    addWiki() {
      if (this.createWikiModalInput == null 
        || this.createWikiModalInput.length < 1
        || this.wikiNames.includes(this.createWikiModalInput))
        return;

      const wikiNameBeingAdded = this.createWikiModalInput;

      this.axios
        .post('/api/wiki', {
          name: wikiNameBeingAdded,
          body: '',
          tags: this.createWikiModalTags
        }).then(() => {
          this.wikiBody = 'Wiki added'
          this.currentWikiName = wikiNameBeingAdded;
          this.wikiNames.push(wikiNameBeingAdded);
          this.addWikiDialog = false;
          this.wikiName = this.createWikiModalInput;
          this.createWikiModalInput = null;
          this.wikiTags = this.createWikiModalTags;
          this.createWikiModalTags = [];
        });
    },
    deleteWiki() {
      const wiki = this.currentWikiName;
      this.axios
          .delete('/api/wiki', {
            data: { name: wiki }
          }).then(() => {
            this.wikiNames = this.wikiNames.filter(x => x != wiki);

            // TODO: add a 'deleted wiki' notification of sorts
            this.wikiBody = null
            this.currentWikiName = null;
            this.editing = null;
            this.areYouSureDeleteDialog = false
          })
    }
  },
  watch: {
    wikiName(newWikiName, prevWikiName) {
      if (newWikiName === prevWikiName) return
      if (newWikiName == null || newWikiName.length < 1) return

      if (this.wikiNames.includes(newWikiName)) {
        this.axios.get(`/api/wiki?name=${newWikiName}`)
        .then(response => {
          this.wikiBody = response.data.body;
          this.currentWikiName = response.data.name;
          this.wikiTags = response.data.tags;
        });
      }
    }
  }
}
</script>

<style scoped>

#wiki-body-wrapper {
  padding: 1em;
  margin-bottom: 1em;
  width: 43em;
}

#wiki-editor {
  text-align: left;
}

#wiki-input-wrapper {
    display: flex;
    justify-content: space-around;
    align-items: center;
    align-content: space-around;
    flex-direction: row;
    width: 30em;
}

#wiki-but {
  margin: 0.2em;
}

#addWikiButModal {
  justify-content: center;
}

</style>