// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  typescript: {
    strict: true
  },

  css: [
    'vuetify/lib/styles/main.sass',
    '@mdi/font/css/materialdesignicons.min.css'
  ],

  build: {
    transpile: ['vuetify'],
  },

  vite: {
    define: {
      'process.env.DEBUG': false,
    },
  },

  modules: [
    // https://nuxt.com/modules/i18n
    '@nuxtjs/i18n',

    // // https://axios.nuxtjs.org
    // '@nuxtjs/axios',

    // // https://auth.nuxtjs.org
    // '@nuxtjs/auth-next'
  ],

  i18n: {
    strategy: 'prefix_except_default',
    defaultLocale: 'pt_BR',
    locales: [
      {
        code: 'pt_BR',
        file: 'pt-BR.json'
      },
      {
        code: 'en_US',
        file: 'en-US.json'
      }
    ],
    lazy: true,
    langDir: 'lang',
    vueI18n: {
      legacy: false
    }
  },
})
