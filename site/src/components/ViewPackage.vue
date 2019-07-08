<template>
  <div><h2>{{ id }} - {{ version }}</h2></div>
</template>

<script>
export default {
  props: ['id','version'],
  mounted: function (){
    console.log(`mounted: ${this.id}/${this.version}.json`)

    let packageApiDetails = this.$packageApi.get(`${this.id}/${this.version}.json`)
      .then(response => resonse.data);
     
    console.log("packageApiDetails");
  }
};
</script>
