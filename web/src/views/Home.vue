<template>
  <div class="home">
    <v-expand-transition>
      <v-card flat v-show="!isEditing" class="mx-auto">
        <WikiHome msg="Welcome to Jam Wiki" />
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
      />

      <v-dialog v-model="createWikiDialog" max-width="300">
        <template v-slot:activator="{ on }">
          <v-btn text icon color="primary" v-on="on" id="wiki-but"
            ><v-icon>mdi-plus</v-icon></v-btn
          >
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
          />
          <v-card-actions id="addWikiButModal">
            <v-btn 
              center
              color="green darken-1"
              text
              v-on:click="addWiki"
            >Add wiki</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </div>

    <v-expand-transition>
      <div v-show="isEditing" class="mx-auto" id="wiki-body-wrapper">
        <WikiPane
          v-bind:wikiName="currentWikiName"
          v-on:deleteWiki="deleteWikiFromSearch"
        />
      </div>
    </v-expand-transition>
  </div>
</template>

<script>
import WikiHome from "@/components/WikiHome";
import WikiPane from "@/components/WikiPane";

export default {
  name: "home",
  components: {
    WikiHome,
    WikiPane
  },
  data() {
    return {
      wikiName: null,
      currentWikiName: null,
      wikiNames: [],
      isLoading: false,
      isEditing: false,

      createWikiDialog: false,
      createWikiModalInput: null,
      createWikiModalTags: []
    };
  },
  methods: {
    deleteWikiFromSearch(wiki) {
      this.wikiName = null;
      this.currentWikiName = null;
      this.wikiNames = this.wikiNames.filter(x => x != wiki);
      this.isEditing = false;
    },
    addWiki() {
      if (
        this.createWikiModalInput == null ||
        this.createWikiModalInput.length < 1 ||
        this.wikiNames.includes(this.createWikiModalInput)
      ) {
        return;
      }

      const wikiNameBeingAdded = this.createWikiModalInput;

      this.axios
        .post("/api/wiki", {
          name: wikiNameBeingAdded,
          body: "",
          tags: this.createWikiModalTags
        })
        .then(() => {
          this.wikiName = wikiNameBeingAdded;
          this.createWikiDialog = false;

          this.wikiNames.push(wikiNameBeingAdded);
          this.isEditing = true;

          this.createWikiModalInput = null;
          this.createWikiModalTags = [];
        });
    }
  },
  created() {
    if (this.isLoading) return;

    this.isLoading = true;

    this.axios
      .get(`/api/wiki/names`)
      .then(response => (this.wikiNames = response.data))
      .catch(err => console.log(err))
      .finally(() => (this.isLoading = false));
  },
  watch: {
    wikiName(newWikiName, prevWikiName) {
      if (newWikiName === prevWikiName) return;
      if (newWikiName == null || newWikiName.length < 1) return;

      if (this.wikiNames.includes(newWikiName)) {
        this.currentWikiName = newWikiName;
        this.isEditing = true;
      }
    }
  }
};
</script>

<style scoped>
#wiki-body-wrapper {
  padding: 1em;
  margin-bottom: 1em;
  width: 43em;
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
