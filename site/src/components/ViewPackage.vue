<template>
  <div>
    <v-card v-if="needsindexing">
      <v-card-title class="headline lighten-3">{{ id }} - {{ version }} not indexed yet</v-card-title>
      <v-card-text>Please wait while we index the package</v-card-text>
    </v-card>

    <v-card v-if="packageDetails">
      <v-card-title class="headline lighten-3">{{ id }} - {{ version }}</v-card-title>
      <v-card-text>{{ packageDetails }}</v-card-text>
    </v-card>
  </div>
</template>

<script>
export default {
  data: () => {
    return {
      needsindexing: false,
      packageDetails: null
    };
  },
  props: ["id", "version"],
  mounted: function() {
    this.$packageApi
      .get(`${this.id}/${this.version}.json`)
      .then(response => (this.packageDetails = response.data))
      .catch(error => {
        if (error.response && error.response.status == 404) {
          this.needsindexing = true;
          return;
        }

        throw error;
      });
  }
};
</script>
