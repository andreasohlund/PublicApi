import Vue from 'vue'
import VueRouter from 'vue-router'
import './plugins/vuetify'
import App from './App.vue'
import PackageSelect from "./components/PackageSelect"
import ViewPackage from "./components/ViewPackage"
import PageNotFound from "./components/PageNotFound"

Vue.config.productionTip = false
Vue.use(VueRouter)


const routes = [
  { path: '/', component: PackageSelect },
  { path: '/package/:id', component: ViewPackage, name: 'package', props: true },
  
  //need to be last
  { path: '*', component: PageNotFound }
]

const router = new VueRouter({
  routes // short for `routes: routes`
})


new Vue(
{
  router,
  render: h => h(App)
}).$mount('#app')
