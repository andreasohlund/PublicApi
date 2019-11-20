<template>
  <v-tabs center-active dark>
    <v-tab v-for="tfm in packageDetails.TargetFrameworks" v-bind:key="tfm.Name">{{tfm.Name}}</v-tab>
    <v-tab-item v-for="tfm in packageDetails.TargetFrameworks" v-bind:key="tfm.Name">
      <v-card v-for="assembly in tfm.Assemblies" v-bind:key="tfm.Name + '-' + assembly.Name">
        <v-card>
          <v-card
            v-for="type in assembly.PublicTypes"
            v-bind:key="tfm.Name + '-'+ assembly.Name + '-' + type.Namespace +'-' + type.Name"
          >{{type.Namespace}}.{{type.Name}}</v-card>
        </v-card>
      </v-card>
    </v-tab-item>
  </v-tabs>
</template>

<script>
export default {
  props: ["packageDetails"],
  mounted: function() {}
};
</script>
