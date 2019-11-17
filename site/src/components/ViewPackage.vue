<template>
  <div>
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
      packageDetails: null
    };
  },
  props: ["id", "version"],
  mounted: function() {
    this.$storage
      .get(`${this.id.toLowerCase()}/${this.version}`)
      .then(response => (this.packageDetails = response.data))
      .catch(error => {
        if (error.response && error.response.status == 404) {
          this.$api
            .get(`packages/${this.id.toLowerCase()}/${this.version}`)
            .then(response => (this.packageDetails = response.data));

          return;
        }

        throw error;
      });
  }
};
</script>
