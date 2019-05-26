<template>
  <v-autocomplete
    v-model="model"
    :items="packages"
    label="Select NuGet package"
  >
  </v-autocomplete>
</template>

<script>
export default {
  data: () => ({})
};
</script>
