import Vue from 'vue'
import VueRouter from 'vue-router'
import axios from "axios";
import App from './App.vue'
import PackageSelect from "./components/PackageSelect"
import ViewPackage from "./components/ViewPackage"
import PageNotFound from "./components/PageNotFound"
import vuetify from './plugins/vuetify';

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

Vue.use({
  install(Vue) {
    Vue.prototype.$nugetPackageMetadata = axios.create({
      baseURL: 'https://api.nuget.org/v3/registration3'
    })
  }
})

const routes = [
  { path: '/', component: PackageSelect, name: 'home' },
  { path: '/packages/:id', component: ViewPackage, name: 'view-package', props: true },
  { path: '/packages/:id/:version', component: ViewPackage, name: 'view-package-version', props: true },

  //need to be last
  { path: '*', component: PageNotFound }
]

const router = new VueRouter({
  mode: 'history',
  routes
})

Vue.config.productionTip = false
Vue.use(VueRouter)
new Vue(
  {
    router,
    vuetify,
    render: h => h(App)
  }).$mount('#app')
