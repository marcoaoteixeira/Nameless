<template>
  <v-app id="blank-layout">
    <v-app-bar app>
      <v-toolbar-title>{{ $hostEnvironment.applicationName }}</v-toolbar-title>
    </v-app-bar>
    <v-main>
      <v-container>
        <slot />
      </v-container>
    </v-main>
    <v-footer app bottom fixed padless>
      <v-card class="rounded-0" width="100%" color="secondary">
        <v-card-text>
          <span class="white--text">{{ $hostEnvironment.applicationLicense }} <strong>{{ $hostEnvironment.applicationOwner }}</strong></span>
        </v-card-text>
      </v-card>
    </v-footer>
  </v-app>
</template>