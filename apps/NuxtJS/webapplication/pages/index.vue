<template>
  <div>
    <h1>Test</h1>
    <p>{{ $t('main', { application: $hostEnvironment.applicationName }) }}</p>

    <AuthenticationProvider />
  </div>
</template>