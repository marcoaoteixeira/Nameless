<template>
  <v-layout align-center justify-center>
    <v-container class="w-50">
      <v-card class="elevation-12">
        <v-toolbar color="primary">
          <v-toolbar-title>{{ $t('auth.form.title') }}</v-toolbar-title>
        </v-toolbar>

        <v-card-text>
          <v-form ref="form" @submit.prevent="submit">

            <v-text-field
              v-model="state.email"
              :append-icon="'mdi-email-outline'"
              :label="$t('auth.form.field.email.title')"
              :error-messages="v$.email.$errors.map(e => e.$message)"
              name="email-input"
              @input="v$.email.$touch()"
              @blur="v$.email.$touch()">
            </v-text-field>

            <v-text-field
              v-model="state.password"
              :type="state.showPassword ? 'text' : 'password'"
              :append-icon="state.showPassword ? 'mdi-eye' : 'mdi-eye-off'"
              :label="$t('auth.form.field.password.title')"
              :error-messages="v$.password.$errors.map(e => e.$message)"
              name="password-input"
              @input="v$.password.$touch()"
              @blur="v$.password.$touch()"
              @click:append="state.showPassword = !state.showPassword">
            </v-text-field>

            <v-checkbox
              v-model="state.rememberMe"
              :label="$t('auth.form.field.rememberMe.title')">
            </v-checkbox>

          </v-form>
        </v-card-text>

        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn type="submit" @click="v$.$validate">{{ $t('auth.form.button.submit.title') }}</v-btn>
        </v-card-actions>
      </v-card>
    </v-container>

  </v-layout>
</template>

<script>
import { reactive } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { required, email } from '@vuelidate/validators'

export default {
 
  setup() {
    definePageMeta({
      layout: 'blank'
    })

    const initialState = {
      email: '',
      password: '',
      showPassword: false,
      rememberMe: false,
    }

    const state = reactive({
      ...initialState
    })

    const rules = {
      email: { required, email },
      password: { required }
    }

    const v$ = useVuelidate(rules, state)

    return { state, v$ }
  }
}
</script>