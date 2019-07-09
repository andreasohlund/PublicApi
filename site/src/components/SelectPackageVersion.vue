<template>
  <div>
    <v-card>
      <v-card-title class="headline lighten-3">{{ id }}</v-card-title>
      <v-card-text>
        <v-autocomplete
          v-model="model"
          :items="versions"
          clearable
          hide-no-data
          label="Select Version"
        ></v-autocomplete>
      </v-card-text>
      <v-divider></v-divider>
      <v-expand-transition>
        <v-btn v-if="model" color="green darken-3" v-on:click="showPackage">Show</v-btn>
      </v-expand-transition>
    </v-card>
  </div>
</template>

<script>
export default {
  data: () => {
    return {
      model: null,
      versions: []
    };
  },
  props: ["id"],
  mounted: function() {
    this.$nugetPackageContent
      .get(`${this.id.toLowerCase()}/index.json`)
      .then(response => {
        this.versions = response.data.versions
          .filter(v => !v.includes("-"))
          .reverse();
      });
  },
  methods: {
    showPackage() {
      let id = this.id;
      let version = this.model;
      this.$router.push({ name: "viewpackage", params: { id, version } });
    }
  }
};
</script>
