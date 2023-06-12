import { Form, Field, ErrorMessage } from 'vee-validate'

export default defineNuxtPlugin(app => {
  app.vueApp.component('VeeForm', Form)
  app.vueApp.component('VeeField', Field)
  app.vueApp.component('VeeErrorMessage', ErrorMessage)

  // let translate = (value: string): string => {
  //   return app.i18n.t(value).toString()
  // };
})