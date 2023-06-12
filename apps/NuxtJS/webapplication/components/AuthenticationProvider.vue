<script setup>

const providers = ref([
  {
    icon: 'mdi-facebook',
    color: '#4267b2',
    colorInDark: '#4267b2',
  },
  {
    icon: 'mdi-twitter',
    color: '#1da1f2',
    colorInDark: '#1da1f2',
  },
  {
    icon: 'mdi-github',
    color: '#272727',
    colorInDark: '#fff',
  },
  {
    icon: 'mdi-google',
    color: '#db4437',
    colorInDark: '#db4437',
  },
])
</script>

<template>
  <v-row>
    <v-col cols="4">
      <v-btn v-for="link in providers" :key="link.icon" :icon="link.icon" :color="link.color" variant="text" />
    </v-col>
  </v-row>
</template>