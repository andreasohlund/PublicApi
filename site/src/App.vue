<template>
  <v-app>
    <v-toolbar app>
      <v-toolbar-title class="headline text-uppercase">
        <span>PublicAPI</span>
        <span class="font-weight-light">.IO</span>
      </v-toolbar-title>
    </v-toolbar>

    <v-content>
      <PackageSearch/>
    </v-content>
  </v-app>
</template>

<script>
import PackageSearch from './components/PackageSearch'

export default {
  name: 'App',
  components: {
    HelloWorld,
    PackageSearch
  },
  data () {
    return {
      //
    }
  }
}
</script>
