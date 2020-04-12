<template>
  <div>
    <div id="wiki-editor" class="pb-4">
      <WikiEditor :body.sync="wikiBody" />

      <v-btn
        text
        icon
        color="primary"
        :loading="isSavingWiki"
        v-on:click="updateWiki"
        id="wiki-but"
        ><v-icon>mdi-content-save</v-icon></v-btn
      >

      <v-dialog v-model="areYouSureDeleteDialog" max-width="235">
        <template v-slot:activator="{ on }">
          <v-btn text icon color="primary" v-on="on" id="wiki-but">
              <v-icon>mdi-delete</v-icon>
          </v-btn>
        </template>
        <v-card class="pl-5 pr-5">
          <v-card-title class="headline pb-1">Are you sure?</v-card-title>
          <v-card-actions class="d-flex justify-center" id="deleteWikiButModal">
            <v-btn color="green darken-1"
                text
                v-on:click="deleteWiki"
            >Yes</v-btn>
            <v-btn
              color="red darken-1"
              text
              v-on:click="areYouSureDeleteDialog = false"
            >No</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
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
  </div>
</template>

<script>
import WikiEditor from "@/components/WikiEditor";

export default {
  name: "WikiPane",
  props: {
    wikiName: String
  },
  components: {
    WikiEditor
  },
  data() {
    return {
      wikiBody: null,
      wikiTags: [],
      search: null,
      colors: ["green", "purple", "indigo", "cyan", "teal", "orange"],
      index: -1,
      menu: false,
      isSavingWiki: false,
      currentWikiName: null,
      areYouSureDeleteDialog: false
    };
  },
  methods: {
    updateWiki() {
      const self = this;
      self.isSavingWiki = true;

      self.axios
        .post("/api/wiki/update", {
          name: self.currentWikiName,
          body: self.wikiBody,
          tags: self.wikiTags
        })
        .finally(() =>
          setTimeout(function() {
            self.isSavingWiki = false;
          }, 250)
        ); // this adds a delay so the loading works
    },

    deleteWiki() {
      const wiki = this.currentWikiName;
      this.axios
        .delete("/api/wiki", {
          data: { name: wiki }
        })
        .then(() => {
          this.$emit("deleteWiki", wiki);

          // TODO: add a 'deleted wiki' notification of sorts
          this.wikiBody = null;
          this.currentWikiName = null;
          this.areYouSureDeleteDialog = false;
        });
    }
  },
  watch: {
    wikiName(newWikiName, _) {
      this.axios.get(`/api/wiki?name=${newWikiName}`).then(response => {
        this.wikiBody = response.data.body;
        this.currentWikiName = response.data.name;
        this.wikiTags = response.data.tags;
      });
    }
  }
};
</script>