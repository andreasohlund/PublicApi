import Vue from 'vue'
import VueRouter from 'vue-router'
import axios from "axios";
import './plugins/vuetify'
import App from './App.vue'


import PackageSelect from "./components/PackageSelect"
import SelectPackageVersion from "./components/SelectPackageVersion"
import ViewPackage from "./components/ViewPackage"
import PageNotFound from "./components/PageNotFound"
import PackageAPI from "./components/PackageAPI"

Vue.config.productionTip = false
Vue.use(VueRouter)

// Register the constructor with id: my-component
Vue.component('view-api', PackageAPI)

Vue.use({
  install(Vue) {
    Vue.prototype.$storage = axios.create({
      baseURL: 'https://storage.publicapi.io'
    })
  }
})

Vue.use({
  install(Vue) {
    Vue.prototype.$api = axios.create({
      baseURL: 'https://api.publicapi.io'
    })
  }
})

Vue.use({
  install(Vue) {
    Vue.prototype.$nugetPackageContent = axios.create({
      baseURL: 'https://api.nuget.org/v3-flatcontainer'
    })
  }
})


const routes = [
  { path: '/', component: PackageSelect },
  { path: '/packages/:id', component: SelectPackageVersion, name: 'selectversion', props: true },
  { path: '/packages/:id/:version', component: ViewPackage, name: 'viewpackage', props: true },

  //need to be last
  { path: '*', component: PageNotFound }
]

const router = new VueRouter({
  mode: 'history',
  routes
})


new Vue(
  {
    router,
    render: h => h(App)
  }).$mount('#app')
