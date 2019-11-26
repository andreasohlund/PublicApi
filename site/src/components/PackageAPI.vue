<template>
  <v-card>
    <v-card-title class="headline lighten-3">Public types</v-card-title>
    <v-card-text>
      <v-tabs center-active dark>
        <v-tab v-for="tfm in packageDetails.TargetFrameworks" v-bind:key="tfm.Name">{{tfm.Name}}</v-tab>
        <v-tab-item v-for="tfm in packageDetails.TargetFrameworks" v-bind:key="tfm.Name">
          <v-card v-for="assembly in tfm.Assemblies" v-bind:key="tfm.Name + '-' + assembly.Name">
            <v-expansion-panels multiple>
              <v-expansion-panel
                v-for="type in assembly.PublicTypes"
                v-bind:key="tfm.Name + '-'+ assembly.Name + '-' + type.Namespace +'-' + type.Name"
              >
                <v-expansion-panel-header>
                  <div>
                    <span class="font-italic font-weight-light">{{type.Namespace}}.</span>
                    <span class="font-weight-bold">{{type.Name}}</span>
                  </div>
                </v-expansion-panel-header>
                <v-expansion-panel-content>Coming soon</v-expansion-panel-content>
              </v-expansion-panel>
            </v-expansion-panels>
          </v-card>
        </v-tab-item>
      </v-tabs>
    </v-card-text>
  </v-card>
</template>

<script>
export default {
  props: ["packageDetails"],
  data: () => {
    return {
      namespaces: null,
      allTypes: null
    };
  },
  methods: {
    getAllTypes(tfm) {
      return tfm.Assemblies[0].PublicTypes;
    }
  }
};
</script>
