import HostEnvironment from 'types/HostEnvironment'

export default defineNuxtPlugin(app => {
  const config = useRuntimeConfig()

  let hostEnvironment: HostEnvironment = {
    applicationName: config.public.application.name || 'Web Application',
    applicationLicense: config.public.application.license || `© ${new Date().getFullYear()}`,
    applicationOwner: config.public.application.owner || 'Nameless',
  }

  return {
    provide: {
      hostEnvironment
    }
  }
})