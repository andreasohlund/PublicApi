<template>
  <div>
    <v-card>
      <v-card-title class="headline lighten-3">{{ id }} - {{ version }}</v-card-title>
      <v-card-text>This is the overview</v-card-text>
    </v-card>
    <v-card>
      <v-card-title class="headline lighten-3">Target frameworks</v-card-title>
      <view-api v-if="packageDetails" v-bind:packageDetails="packageDetails" />
    </v-card>
  </div>
</template>

<script>
export default {
  data: () => {
    return {
      packageDetails: null
    };
  },
  props: ["id", "version"],
  mounted: function() {
    let url = `/packages/${this.id.toLowerCase()}/${this.version}`;

    this.$storage
      .get(url)
      .then(response => (this.packageDetails = response.data))
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
