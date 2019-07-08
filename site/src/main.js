import Vue from 'vue'
import VueRouter from 'vue-router'
import axios from "axios";
import './plugins/vuetify'
import App from './App.vue'


import PackageSelect from "./components/PackageSelect"
import SelectPackageVersion from "./components/SelectPackageVersion"
import ViewPackage from "./components/ViewPackage"
import PageNotFound from "./components/PageNotFound"

Vue.config.productionTip = false
Vue.use(VueRouter)

Vue.use({
    install (Vue) {
    Vue.prototype.$packageApi = axios.create({
      baseURL: 'https://publicapi.blob.core.windows.net/packages'
    })
  }
})

Vue.use({
  install (Vue) {
  Vue.prototype.$nugetPackageContent = axios.create({
    baseURL: 'https://api.nuget.org/v3-flatcontainer'
  })
}
})


const routes = [
  { path: '/', component: PackageSelect },
  { path: '/package/:id', component: SelectPackageVersion, name: 'selectversion', props: true },
  { path: '/package/:id/:version', component: ViewPackage, name: 'package', props: true },
  
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
