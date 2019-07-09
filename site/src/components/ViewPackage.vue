<template>
  <div>
          <v-card v-if="needsindexing">
    <v-card-title class="headline lighten-3">{{ id }} - {{ version }} not indexed yet</v-card-title>
    <v-card-text >Please wait while we index the package</v-card-text>
  </v-card>
    
    </div>
</template>

<script>
export default {
   data: () => {
    return {
      needsindexing: false
    };
  },
  props: ['id','version'],
  mounted: function (){
    console.log(`mounted: ${this.id}/${this.version}.json`)

    let packageApiDetails = this.$packageApi.get(`${this.id}/${this.version}.json`)
      .then(response => resonse.data)
      .catch(error =>{
        if(error.response && error.response.status == 404)
        {
          this.needsindexing = true;
          return;
        }

        throw error;
      });
  }
};
</script>
