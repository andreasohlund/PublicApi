<template>
  <div>
    <v-expansion-panels multiple>
      <v-expansion-panel>
        <v-expansion-panel-header>Fields</v-expansion-panel-header>
        <v-expansion-panel-content>
          <v-list-item v-for="field in type.Fields" v-bind:key="field.Name">
            <v-list-item-content>
              <v-list-item-title>
                <span>{{field.Type}}</span>
                {{field.Name}}
              </v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </v-expansion-panel-content>
      </v-expansion-panel>
    </v-expansion-panels>
  </div>
</template>

<script>
export default {
  props: ["type"],
  data: () => {
    return {};
  }
};
</script>
