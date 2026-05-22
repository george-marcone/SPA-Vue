<template>
  <section class="grid gap-6">
    <div>
      <p class="eyebrow">Usuarios</p>
      <h1 class="m-0 text-3xl font-extrabold text-slate-900">Novo usuario</h1>
    </div>

    <form class="grid gap-5 rounded-lg border border-slate-200 bg-white p-5" @submit.prevent="salvar">
      <div class="grid gap-4 md:grid-cols-2">
        <label>
          <span>Nome</span>
          <input v-model.trim="form.nome" type="text" required maxlength="100" />
        </label>

        <label>
          <span>Email</span>
          <input v-model.trim="form.email" type="email" required maxlength="150" />
        </label>

        <label>
          <span>Telefone</span>
          <input v-model.trim="form.telefone" type="tel" required maxlength="20" />
        </label>

        <label>
          <span>Perfil</span>
          <select v-model.number="form.idPerfil" required>
            <option disabled :value="0">Selecione</option>
            <option v-for="perfil in perfis" :key="perfil.idPerfil" :value="perfil.idPerfil">
              {{ perfil.descricaoPerfil }}
            </option>
          </select>
        </label>

        <div class="rounded-md border border-[#d7e8ff] bg-[#eff6ff] p-4 text-sm font-semibold text-[#24446d] md:col-span-2">
          A senha inicial sera definida automaticamente como <strong>Senha@252525</strong>.
        </div>
      </div>

      <p v-if="erro" class="alert alert-error">{{ erro }}</p>

      <div class="flex flex-wrap justify-end gap-2">
        <NuxtLink class="rounded-md border border-slate-200 px-4 py-2 text-sm font-bold no-underline hover:bg-slate-100" to="/usuarios">
          Cancelar
        </NuxtLink>
        <button class="rounded-md bg-[#147f72] px-4 py-2 text-sm font-bold text-white hover:bg-[#0f6c61]" type="submit" :disabled="salvando">
          {{ salvando ? 'Salvando...' : 'Salvar usuario' }}
        </button>
      </div>
    </form>
  </section>
</template>

<script setup lang="ts">
import type { Perfil, UsuarioCreate, UsuarioSummary } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

definePageMeta({
  roles: ['Administrador']
})

const { $api } = useNuxtApp()
const perfis = ref<Perfil[]>([])
const salvando = ref(false)
const erro = ref('')
const form = reactive<UsuarioCreate>({
  nome: '',
  email: '',
  telefone: '',
  idPerfil: 0
})

onMounted(async () => {
  try {
    perfis.value = await $api<Perfil[]>('/usuarios/perfis')
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
})

async function salvar() {
  salvando.value = true
  erro.value = ''

  try {
    const created = await $api<UsuarioSummary>('/usuarios', {
      method: 'POST',
      body: { ...form }
    })
    await navigateTo(`/usuarios/${created.idUsuario}`)
  } catch (err) {
    erro.value = normalizeApiError(err)
  } finally {
    salvando.value = false
  }
}
</script>
