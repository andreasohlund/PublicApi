<template>
  <v-card v-if="nugetDetails">
    <v-list-item three-line>
      <v-list-item-content>
        <div class="overline mb-4">version {{version}}</div>
        <v-list-item-title class="headline mb-1">{{id}}</v-list-item-title>
        <v-list-item-subtitle>{{nugetDetails.description}}</v-list-item-subtitle>
      </v-list-item-content>

      <v-list-item-avatar tile size="64">
        <img v-bind:src="nugetDetails.iconUrl" v-bind:alt="nugetDetails.authors" />
      </v-list-item-avatar>
    </v-list-item>
  </v-card>
</template>

<script>
import axios from "axios";

export default {
  data: () => {
    return {
      nugetDetails: null
    };
  },
  props: {
    id: String,
    version: String
  },
  mounted: function() {
    let url = `/${this.id.toLowerCase()}/${this.version}.json`;
    this.$nugetPackageMetadata.get(url).then(response => {
      let catalogEntry = response.data.catalogEntry;

      axios
        .get(catalogEntry)
        .then(response => (this.nugetDetails = response.data));
    });
  }
};
</script>
