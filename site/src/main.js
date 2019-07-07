import Vue from 'vue'
import VueRouter from 'vue-router'
import './plugins/vuetify'
import App from './App.vue'
import PackageSelect from "./components/PackageSelect"
import PageNotFound from "./components/PageNotFound"

Vue.config.productionTip = false
Vue.use(VueRouter)


const routes = [
  { path: '/', component: PackageSelect },
  
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
