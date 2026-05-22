<template>
  <section class="grid gap-6">
    <div>
      <p class="eyebrow">Diretoria</p>
      <h1 class="m-0 text-3xl font-extrabold text-slate-900">Novo integrante</h1>
    </div>

    <form class="grid gap-5 rounded-lg border border-slate-200 bg-white p-5" @submit.prevent="salvar">
      <div class="grid gap-4 md:grid-cols-2">
        <label>
          <span>Nome</span>
          <input v-model.trim="form.nome" type="text" required maxlength="100" />
        </label>

        <label>
          <span>Usuario vinculado</span>
          <select v-model="idUsuario">
            <option value="">Sem vinculo</option>
            <option v-for="usuario in usuarios" :key="usuario.idUsuario" :value="usuario.idUsuario">
              {{ usuario.nome }} - {{ usuario.descricaoPerfil }}
            </option>
          </select>
        </label>
      </div>

      <p v-if="erro" class="alert alert-error">{{ erro }}</p>

      <div class="flex flex-wrap justify-end gap-2">
        <NuxtLink class="rounded-md border border-slate-200 px-4 py-2 text-sm font-bold no-underline hover:bg-slate-100" to="/diretoria">
          Cancelar
        </NuxtLink>
        <button class="rounded-md bg-[#147f72] px-4 py-2 text-sm font-bold text-white hover:bg-[#0f6c61]" type="submit" :disabled="salvando">
          {{ salvando ? 'Salvando...' : 'Salvar integrante' }}
        </button>
      </div>
    </form>
  </section>
</template>

<script setup lang="ts">
import type { Diretoria, DiretoriaCreate, UsuarioSummary } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

definePageMeta({
  roles: ['Administrador', 'Contribuinte']
})

const { $api } = useNuxtApp()
const usuarios = ref<UsuarioSummary[]>([])
const salvando = ref(false)
const erro = ref('')
const idUsuario = ref<number | ''>('')
const form = reactive<DiretoriaCreate>({
  nome: '',
  idUsuario: null
})

onMounted(async () => {
  try {
    usuarios.value = await $api<UsuarioSummary[]>('/usuarios')
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
})

async function salvar() {
  salvando.value = true
  erro.value = ''

  try {
    const created = await $api<Diretoria>('/diretoria', {
      method: 'POST',
      body: {
        nome: form.nome,
        idUsuario: idUsuario.value === '' ? null : Number(idUsuario.value)
      }
    })
    await navigateTo(`/diretoria/${created.id}`)
  } catch (err) {
    erro.value = normalizeApiError(err)
  } finally {
    salvando.value = false
  }
}
</script>
