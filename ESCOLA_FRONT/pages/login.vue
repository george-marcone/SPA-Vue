<template>
  <section class="w-[min(420px,100%)] rounded-lg border border-[#d4dee9] bg-white p-8 shadow-[0_22px_55px_rgba(14,30,53,0.12)]">
    <div>
      <p class="m-0 text-xs font-extrabold uppercase text-[#d64200]">GM Tech Solutions</p>
      <h1 class="mb-2 mt-2 text-4xl font-normal leading-tight text-[#071d3b]">Escola High Tech</h1>
      <p class="m-0 text-sm font-bold uppercase text-[#147f72]">Acesso seguro</p>
    </div>

    <form v-if="!mostrarAlteracaoSenha" class="mt-8 grid gap-5" @submit.prevent="entrar">
      <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
        <span>Email</span>
        <input v-model.trim="form.email" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="email" autocomplete="email" required />
      </label>

      <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
        <span>Senha</span>
        <input v-model="form.senha" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="password" autocomplete="current-password" required />
      </label>

      <p v-if="auth.error" class="alert alert-error">{{ auth.error }}</p>

      <button class="inline-flex min-h-12 items-center justify-center rounded-md bg-[#147f72] px-4 text-sm font-extrabold text-white transition hover:bg-[#0f6c61] disabled:cursor-wait disabled:opacity-70" type="submit" :disabled="auth.loading">
        {{ auth.loading ? 'Entrando...' : 'Entrar' }}
      </button>
    </form>

    <form v-else class="mt-8 grid gap-5" @submit.prevent="alterarSenha">
      <p class="alert alert-warning">
        Sua conta ainda usa a senha padrao. Defina uma nova senha para continuar.
      </p>

      <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
        <span>Nova senha</span>
        <input v-model="senhaForm.novaSenha" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="password" autocomplete="new-password" required />
      </label>

      <PasswordStrengthMeter :password="senhaForm.novaSenha" />

      <label class="grid gap-2 text-sm font-extrabold text-[#071d3b]">
        <span>Confirmar nova senha</span>
        <input v-model="senhaForm.confirmacaoSenha" class="min-h-11 rounded-md border border-[#ccd8e5] px-3 text-[#071d3b] outline-none focus:border-[#147f72] focus:ring-4 focus:ring-[#147f72]/10" type="password" autocomplete="new-password" required />
      </label>

      <p v-if="erroAlteracao" class="alert alert-error">{{ erroAlteracao }}</p>

      <button class="inline-flex min-h-12 items-center justify-center rounded-md bg-[#147f72] px-4 text-sm font-extrabold text-white transition hover:bg-[#0f6c61] disabled:cursor-wait disabled:opacity-70" type="submit" :disabled="auth.loading">
        {{ auth.loading ? 'Alterando...' : 'Alterar senha e continuar' }}
      </button>
    </form>
  </section>
</template>

<script setup lang="ts">
import { normalizeApiError } from '~/utils/api-client'

definePageMeta({
  layout: 'auth',
  public: true
})

const auth = useAuthStore()
const form = reactive({
  email: '',
  senha: ''
})
const senhaForm = reactive({
  novaSenha: '',
  confirmacaoSenha: ''
})
const mostrarAlteracaoSenha = ref(false)
const erroAlteracao = ref('')

async function entrar() {
  const response = await auth.login(form)
  if (response.deveAlterarSenhaPadrao) {
    mostrarAlteracaoSenha.value = true
    return
  }

  await navigateTo('/')
}

async function alterarSenha() {
  erroAlteracao.value = ''

  try {
    await auth.alterarSenha({
      senhaAtual: form.senha,
      novaSenha: senhaForm.novaSenha,
      confirmacaoSenha: senhaForm.confirmacaoSenha
    })
  } catch (err) {
    erroAlteracao.value = normalizeApiError(err)
    return
  }

  await navigateTo('/')
}
</script>
