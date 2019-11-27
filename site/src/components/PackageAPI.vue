<template>
  <v-card>
    <v-card-title class="headline lighten-3">
      Public types
      <v-spacer></v-spacer>
      <v-chip outlined color="primary" v-for="tfm in this.targetFrameworks" v-bind:key="tfm">{{tfm}}</v-chip>
    </v-card-title>
    <v-card-text>
      <v-expansion-panels multiple>
        <v-expansion-panel v-for="type in this.allTypes" v-bind:key="type.Id">
          <v-expansion-panel-header>
            <div>
              <span class="font-italic font-weight-light">{{type.Namespace}}.</span>
              <span class="font-weight-bold">{{type.Name}}</span>
            </div>
          </v-expansion-panel-header>
          <v-expansion-panel-content>TODO</v-expansion-panel-content>
        </v-expansion-panel>
      </v-expansion-panels>
    </v-card-text>
  </v-card>
</template>

<script>
export default {
  props: ["packageDetails"],
  data: () => {
    return {
      namespaces: null,
      allTypes: null,
      targetFrameworks: null
    };
  },
  mounted: function() {
    let types = {};
    let tfms = new Set();

    let namespaces = new Set();

    this.packageDetails.TargetFrameworks.forEach(tfm => {
      tfms.add(tfm.Name);

      tfm.Assemblies.forEach(assembly => {
        assembly.PublicTypes.forEach(type => {
          namespaces.add(type.Namespace);

          let id = `${assembly.Name}-${type.Namespace}.${type.Name}`;

          let existingType = types[id];

          if (!existingType) {
            type["Id"] = id;
            type["AvailableIn"] = new Set();
            types[id] = type;

            existingType = types[id];
          }

          existingType.AvailableIn.add({
            Framework: tfm.Name,
            Assembly: assembly.Name
          });
          //TODO: Deal with availability for fields, props and methods
        });
      });
    });

    // window.console.log(tfms);
    // window.console.log(types);
    this.allTypes = types;
    this.targetFrameworks = tfms;
    this.namespaces = namespaces;
  }
};
</script>
