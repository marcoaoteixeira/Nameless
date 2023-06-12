<template>
  <v-app id="blank-layout">
    <v-main>
      <v-container>
        <slot />
      </v-container>
    </v-main>
    <v-footer app bottom fixed class="pa-0">
      <v-card width="100%" color="indigo" class="pa-5 ma-0 rounded-0">
        &copy; {{ new Date().getFullYear() }} Nameless
      </v-card>
    </v-footer>
  </v-app>
</template>