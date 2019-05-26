<template>
  <v-autocomplete 
  v-model="model" 
  :items="packages" 
  label="Select NuGet package"
  :search-input.sync="search"
  ></v-autocomplete>
</template>

<script>
export default {
  data: () => {
    return {
      model: null,
      search: null,
      isLoading: false,
      packages: []
    };
  },
   watch: {
    search (val) {
          console.log(val);
      
      // Items have already been loaded
      //if (this.items.length > 0) return

      // Items have already been requested
      if (this.isLoading) return

      this.isLoading = true
    }
  }
};
</script>
