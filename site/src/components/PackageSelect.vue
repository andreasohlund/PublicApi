<template>
  <v-autocomplete
    v-model="package"
    :items="items"
    :loading="loading"
    no-filter
    clearable
    return-object
    label="Select NuGet package"
    :search-input.sync="search"
  ></v-autocomplete>
</template>

<script>
import _ from "lodash";
import axios from "axios";

export default {
  data: () => {
    return {
      package: null,
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
    package(id) {
       console.log(id)
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
      // if (this.apiSource) {
      //   this.apiSource.cancel();
      // }
      // this.apiSource = Axios.CancelToken.source();
      return axios.get(
        `https://api-v2v3search-0.nuget.org/autocomplete?q=${query}&prerelease=true`
      );
    }
  }
};
</script>
