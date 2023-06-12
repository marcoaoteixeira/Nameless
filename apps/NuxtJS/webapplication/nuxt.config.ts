import i18n from "./configs/i18n"

const environment = {
  isDevelopment: process.env.NODE_ENV === 'development',
  client: {
    scheme: process.env.HTTP_SCHEME || 'https',
    host: process.env.HOST || 'localhost',
    port: process.env.PORT || '4430',
    baseUrl: `${process.env.HTTP_SCHEME || 'https'}://${process.env.HOST || 'localhost'}:${process.env.PORT || '4430'}`
  },
  api: {
    scheme: process.env.API_HTTP_SCHEME || 'https',
    host: process.env.API_HOST || 'localhost',
    port: process.env.API_PORT || '44300',
    baseUrl: `${process.env.API_HTTP_SCHEME || 'https'}://${process.env.API_HOST || 'localhost'}:${process.env.API_PORT || '44300'}/api`
  },
  application: {
    name: process.env.APPLICATION_NAME || 'Web Application',
    license: process.env.APPLICATION_LICENSE || `© ${new Date().getFullYear()}`,
    owner: process.env.APPLICATION_OWNER || 'Nameless'
  }
}

export default defineNuxtConfig({
  css: [
    'vuetify/lib/styles/main.sass',
    '@mdi/font/css/materialdesignicons.min.css'
  ],

  build: {
    transpile: [
      'vuetify'
    ]
  },

  modules: [
    // https://nuxt.com/modules/i18n
    ['@nuxtjs/i18n', i18n]
  ],

  devServer: {
    host: environment.client.host,
    port: parseInt(environment.client.port)
  },

  runtimeConfig: {
    api: {
      ...environment.api,
    },

    public: {
      application: {
        ...environment.application
      }
    }
  },
})
