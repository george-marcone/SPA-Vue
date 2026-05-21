<template>
  <section class="login-panel">
    <div>
      <p class="eyebrow">Acesso seguro</p>
      <h1>Form Escola</h1>
    </div>

    <form class="form-grid" @submit.prevent="entrar">
      <label>
        <span>Email</span>
        <input v-model.trim="form.email" type="email" autocomplete="email" required />
      </label>

      <label>
        <span>Senha</span>
        <input v-model="form.senha" type="password" autocomplete="current-password" required />
      </label>

      <p v-if="auth.error" class="alert alert-error">{{ auth.error }}</p>

      <button class="btn btn-primary" type="submit" :disabled="auth.loading">
        {{ auth.loading ? 'Entrando...' : 'Entrar' }}
      </button>
    </form>
  </section>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'auth',
  public: true
})

const auth = useAuthStore()
const form = reactive({
  email: '',
  senha: ''
})

async function entrar() {
  await auth.login(form)
  await navigateTo('/')
}
</script>
