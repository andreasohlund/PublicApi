<template>
  <v-card>
    <v-card-title class="headline lighten-3">Public types</v-card-title>
    <v-card-text>
      <v-expansion-panels multiple>
        <v-expansion-panel v-for="type in this.allTypes" v-bind:key="type.Id">
          <v-expansion-panel-header>
            <div>
              <span class="font-italic font-weight-light">{{type.Namespace}}.</span>
              <span class="font-weight-bold">{{type.Name}}</span>
            </div>
          </v-expansion-panel-header>
          <v-expansion-panel-content>
            <v-chip
              v-for="availability in type.AvailableIn"
              v-bind:key="availability.Framework"
            >{{availability.Framework}}</v-chip>
          </v-expansion-panel-content>
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
      allTypes: null
    };
  },
  mounted: function() {
    let types = {};
    let tfms = new Set();

    this.packageDetails.TargetFrameworks.forEach(tfm => {
      tfms.add(tfm.Name);

      tfm.Assemblies.forEach(assembly => {
        assembly.PublicTypes.forEach(type => {
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
  }
};
</script>
