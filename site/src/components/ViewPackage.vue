<template>
  <v-container fluid>
    <v-row dense>
      <v-col>
        <package-overview v-if="version" v-bind:id="id" v-bind:version="version" />
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <package-api
          v-if="packageDetails"
          v-bind:packageDetails="packageDetails"
          v-bind:schemaVersion="schemaVersion"
        />
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
      packageDetails: null,
      schemaVersion: null
    };
  },
  mounted: function() {
    if (!this.version) {
      this.$nugetPackageContent
        .get(`/${this.id.toLowerCase()}/index.json`)
        .then(response => {
          let versions = response.data.versions;
          let version = versions[versions.length - 1];

          this.loadPackageApi(this.id, version);
          this.$router.push({
            name: "view-package-version",
            params: { id: this.id, version }
          });
        });
      return;
    }
    this.loadPackageApi(this.id, this.version);
  },
  methods: {
    loadPackageApi(id, version) {
      let url = `/packages/${id.toLowerCase()}/${version}`;
      this.$storage
        .get(url)
        .then(response => {
          this.schemaVersion = response.headers["x-ms-meta-schemaversion"];
          this.packageDetails = response.data;
        })
        .catch(error => {
          if (error.response && error.response.status == 404) {
            this.$api.get(url).then(response => {
              this.schemaVersion = response.headers["x-ms-meta-schemaversion"];
              this.packageDetails = response.data;
            });

            return;
          }

          throw error;
        });
    }
  }
};
</script>
