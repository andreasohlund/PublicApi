<template>
  <v-card>
    <v-card-text>
      <v-autocomplete
        v-model="model"
        :items="items"
        :loading="loading"
        no-filter
        clearable
        return-object
        hide-no-data
        label="Select NuGet package"
        :search-input.sync="search"
      ></v-autocomplete>
    </v-card-text>
  </v-card>
</template>

<script>
import _ from "lodash";
import axios from "axios";

export default {
  data: () => {
    return {
      model: null,
      search: null,
      loading: false,
      apiSource: null,
      items: []
    };
  },
  watch: {
    search(query) {
      if (query && (!this.select || this.select.text !== query)) {
        this.querySearch(query);
      }
    },
    model(packageId) {
      this.$router.push({ name: "view-package", params: { id: packageId } });
    }
  },
  methods: {
    querySearch: _.debounce(function(query) {
      this.loading = true;
      this.apiQuery(query)
        .then(response => {
          this.items = response.data.data;
        })
        .finally(() => {
          this.loading = false;
        });
    }, 270),
    apiQuery(query) {
      if (this.apiSource) {
        this.apiSource.cancel();
      }
      this.apiSource = axios.CancelToken.source();
      return axios.get(
        `https://api-v2v3search-0.nuget.org/autocomplete?q=${query}&prerelease=true&semVerLevel=2.0.0`,
        { cancelToken: this.apiSource.token }
      );
    }
  }
};
</script>
