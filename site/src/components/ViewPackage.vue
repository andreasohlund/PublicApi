<template>
  <v-container fluid>
    <v-row dense>
      <v-col>
        <package-overview v-bind:id="id" v-bind:version="version" />
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <package-api v-if="packageDetails" v-bind:packageDetails="packageDetails" />
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import PackageOverview from "./PackageOverview";
import PackageApi from "./PackageApi";

export default {
  components: {
    PackageOverview,
    PackageApi
  },
  props: {
    id: String,
    version: String
  },
  data: () => {
    return {
      packageDetails: null
    };
  },
  mounted: function() {
    let url = `/packages/${this.id.toLowerCase()}/${this.version}`;
    this.$storage
      .get(url)
      .then(response => {
        //TODO: x-ms-meta-schemaversion
        this.packageDetails = response.data;
      })
      .catch(error => {
        if (error.response && error.response.status == 404) {
          this.$api
            .get(url)
            .then(response => (this.packageDetails = response.data));

          return;
        }

        throw error;
      });
  }
};
</script>
