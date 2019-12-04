<template>
  <v-card>
    <v-card-title class="headline lighten-3">
      Types
      <v-spacer></v-spacer>
      <v-chip outlined color="primary" v-for="tfm in this.targetFrameworks" v-bind:key="tfm">{{tfm}}</v-chip>
    </v-card-title>
    <v-card-text>
      <v-expansion-panels multiple>
        <v-expansion-panel v-for="type in this.allTypes" v-bind:key="type.Id">
          <v-expansion-panel-header>
            <span>
              <v-badge v-model="type.HasWarnings" color="warning lighten-2">
                <template v-slot:badge>
                  <span>!</span>
                </template>
                <v-icon v-if="type.IsInterface">mdi-alpha-i-box-outline</v-icon>
                <v-icon v-if="type.IsClass">mdi-alpha-c-box-outline</v-icon>
                <v-icon v-if="type.IsEnum">mdi-alpha-e-box-outline</v-icon>
                <span class="font-weight-bold">{{type.Name}}</span>
              </v-badge>
            </span>
          </v-expansion-panel-header>
          <v-expansion-panel-content>
            <show-class v-if="type.IsClass" v-bind:type="type"></show-class>
            <show-enum v-if="type.IsEnum" v-bind:type="type"></show-enum>
          </v-expansion-panel-content>
        </v-expansion-panel>
      </v-expansion-panels>
    </v-card-text>
  </v-card>
</template>

<script>
import ShowClass from "./ShowClass.vue";
import ShowEnum from "./ShowEnum.vue";

export default {
  components: {
    ShowClass,
    ShowEnum
  },
  props: ["packageDetails", "schemaVersion"],
  data: () => {
    return {
      namespaces: null,
      allTypes: null,
      targetFrameworks: null,
      show: false
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
            type["HasWarnings"] = false;

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

    for (var key in types) {
      let type = types[key];

      if (this.schemaVersion < "0.3.0") {
        this.applyEnumAndClassFlag(type);
      }
    }

    this.allTypes = Object.values(types).sort(function(a, b) {
      var nameA = a.Name;
      var nameB = b.Name;
      if (nameA < nameB) {
        return -1;
      }
      if (nameA > nameB) {
        return 1;
      }

      return 0;
    });
    this.targetFrameworks = tfms;
    this.namespaces = namespaces;
  },
  methods: {
    applyEnumAndClassFlag(type) {
      let isClass = !type.IsInterface;

      if (type.Fields.some(field => field.Name == "value__")) {
        type["IsEnum"] = true;
        isClass = false;
      }
      type["IsClass"] = isClass;
    }
  }
};
</script>
