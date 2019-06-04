<template>
  <v-card>
    <v-card-title class="headline lighten-3">Explore the public API of your NuGet dependency</v-card-title>
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
    <v-divider></v-divider>
    <v-expand-transition>
      <v-card-text v-if="model" class="lighten-3">{{model}}</v-card-text>
    </v-expand-transition>
    <v-card-actions v-if="model">
      <v-spacer></v-spacer>
      <v-btn color="green darken-3" @click>Show</v-btn>
    </v-card-actions>
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
    package(id) {
      console.log(id);
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
        `https://api-v2v3search-0.nuget.org/autocomplete?q=${query}&prerelease=true`,
        { cancelToken: this.apiSource.token }
      );
    }
  }
};
</script>
