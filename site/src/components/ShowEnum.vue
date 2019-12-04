<template>
  <div>
    enum{
    <v-list-item v-for="field in type.Fields" v-bind:key="field.Name">
      <v-list-item-content>
        <v-list-item-title>{{field.Name}}</v-list-item-title>
      </v-list-item-content>
    </v-list-item>}
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
