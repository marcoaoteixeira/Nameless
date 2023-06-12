export default {
  baseUrl: '',
  locales: [
    {
      code: 'pt_BR',
      iso: 'pt-BR',
      file: 'pt-br.json',
      dir: 'ltr',
      name: 'Português do Brasil',
    },
    {
      code: 'en_US',
      iso: 'en-US',
      file: 'en-us.js',
      dir: 'ltr',
      name: 'United States - English'
    }
  ],
  defaultDirection: 'ltr',
  defaultLocale: 'pt_BR',
  sortRoutes: true,
  strategy: 'prefix_except_default',
  lazy: {
    skipNuxtState: true
  },
  langDir: 'localization',
  detectBrowserLanguage: {
    alwaysRedirect: false,
    fallbackLocale: '',
    redirectOn: 'root',
    useCookie: true,
    cookieAge: 365,
    cookieCrossOrigin: false,
    cookieDomain: null,
    cookieKey: 'i18n_redirected',
    cookieSecure: false,
  },
  rootRedirect: null,
  differentDomains: false,
  pages: {},
  seo: true,
  vuex: {
    moduleName: 'i18n',
    syncRouteParams: true
  },
  vueI18n: {
    legacy: false,
    fallbackLocale: 'pt_BR'
  },
  vueI18nLoader: false,
  onBeforeLanguageSwitch: (oldLocale: any, newLocale: any, isInitialSetup: any, context: any): void => {},
  onLanguageSwitched: (oldLocale: any, newLocale: any): void => {},
  skipSettingLocaleOnNavigate: false,
  defaultLocaleRouteNameSuffix: 'default',
  routesNameSeparator: '___',
}