<template>
  <v-card>
    <v-list-item three-line>
      <v-list-item-content>
        <div class="overline mb-4">version {{version}}</div>
        <v-list-item-title class="headline mb-1">{{id}}</v-list-item-title>
        <v-list-item-subtitle>Greyhound divisely hello coldly fonwderfully</v-list-item-subtitle>
      </v-list-item-content>

      <v-list-item-avatar tile size="80" color="grey"></v-list-item-avatar>
    </v-list-item>

    <!-- <v-card-actions>
      <v-btn text>Browse on NuGet</v-btn>
      <v-btn text>Button</v-btn>
    </v-card-actions>-->
  </v-card>
</template>

<script>
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
    let url = `/packages/${this.id.toLowerCase()}/${this.version}`;
    this.$storage
      .get(url)
      .then(response => {
        //TODO: x-ms-meta-schemaversion
        this.nugetDetails = response.data;
      })
      .catch(error => {
        if (error.response && error.response.status == 404) {
          this.$api
            .get(url)
            .then(response => (this.nugetDetails = response.data));

          return;
        }

        throw error;
      });
  }
};
</script>
