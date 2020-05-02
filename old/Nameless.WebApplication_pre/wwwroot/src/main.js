// Styles
import '../node_modules/admin-lte/dist/css/adminlte.css'

// Scripts
import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'
import $ from 'jquery'
import jQuery from 'jquery'
import bootstrap from 'bootstrap'
import adminlte from 'admin-lte'

Vue.config.productionTip = false

Vue.use($)
Vue.use(jQuery)
Vue.use(bootstrap)
Vue.use(adminlte)

new Vue({
  router,
  store,
  render: handler => handler(App)
}).$mount('#app')
