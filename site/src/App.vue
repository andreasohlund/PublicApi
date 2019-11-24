<template>
  <v-app id="inspire">
    <v-app-bar app>
      <v-toolbar-title v-on:click="home()">Public API</v-toolbar-title>
    </v-app-bar>
    <v-content>
      <v-container fluid>
        <router-view></router-view>
      </v-container>
    </v-content>
    <v-footer app fixed>
      <span>
        &copy;PublicAPI.io 2019
        <a
          href="https://github.com/andreasohlund/PublicApi/issues/new"
          target="_blank"
        >Get in touch</a>
      </span>
    </v-footer>
  </v-app>
</template>

<script>
export default {
  name: "App",
  components: {},
  methods: {
    home() {
      this.$router.push({ name: "home" });
    }
  },
  data() {
    return {
      drawer: false
    };
  }
};
</script>
