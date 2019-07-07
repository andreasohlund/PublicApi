import Vue from 'vue'
import VueRouter from 'vue-router'
import './plugins/vuetify'
import App from './App.vue'
import PackageSelect from "./components/PackageSelect"

Vue.config.productionTip = false
Vue.use(VueRouter)



const routes = [
  { path: '/', component: PackageSelect }
]

const router = new VueRouter({
  routes // short for `routes: routes`
})


new Vue(
{
  router,
  render: h => h(App)
}).$mount('#app')
