<template>
  <v-app id="inspire" dark>
    <v-navigation-drawer
      clipped
      fixed
      v-model="drawer"
      app
    >
      <v-list dense>
        <v-list-tile @click="">
          <v-list-tile-action>
            <v-icon>dashboard</v-icon>
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title>Dashboard</v-list-tile-title>
          </v-list-tile-content>
        </v-list-tile>
      </v-list>
    </v-navigation-drawer>
    <v-toolbar app fixed clipped-left>
      <v-toolbar-side-icon @click.stop="drawer = !drawer"></v-toolbar-side-icon>
      <v-toolbar-title>Public API</v-toolbar-title>
    </v-toolbar>
    <v-content>
      <v-container fluid>
        <v-layout align-center justify-center row fill-height>
          <v-flex>
            <h2>Explore the public api of your package dependencies</h2>
            <PackageSelect/>
          </v-flex>
        </v-layout>
      </v-container>
    </v-content>
    <v-footer app fixed>
      <span>&copy;PublicAPI.io 2019</span>
    </v-footer>
  </v-app>
</template>

<script>
import PackageSelect from "./components/PackageSelect";

export default {
  name: "App",
  components: {
    PackageSelect
  },
  data() {
    return {
      drawer: false
    };
  }
};
</script>
