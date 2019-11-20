<template>
  <v-app id="inspire">
    <v-app-bar app>
      <v-toolbar-title>Public API</v-toolbar-title>
    </v-app-bar>
    <v-content>
      <v-container fluid>
        <router-view></router-view>
      </v-container>
    </v-content>
    <v-footer app fixed>
      <span>&copy;PublicAPI.io 2019</span>
    </v-footer>
  </v-app>
</template>

<script>
export default {
  name: "App",
  components: {},
  data() {
    return {
      drawer: false
    };
  }
};
</script>
