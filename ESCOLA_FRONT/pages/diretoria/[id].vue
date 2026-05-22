<template>
  <section class="grid gap-6">
    <div class="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
      <div>
        <p class="eyebrow">Diretoria</p>
        <h1 class="m-0 text-3xl font-extrabold text-slate-900">
          {{ editando ? 'Editar integrante' : 'Visualizar integrante' }}
        </h1>
      </div>
      <div class="flex flex-wrap gap-2">
        <NuxtLink class="rounded-md border border-slate-200 px-4 py-2 text-sm font-bold no-underline hover:bg-slate-100" to="/diretoria">
          Voltar
        </NuxtLink>
        <button
          v-if="auth.canWrite && !editando"
          class="rounded-md bg-blue-600 px-4 py-2 text-sm font-bold text-white hover:bg-blue-700"
          type="button"
          @click="editando = true"
        >
          Editar
        </button>
        <button
          v-if="auth.isAdmin"
          class="rounded-md border border-red-200 px-4 py-2 text-sm font-bold text-red-700 hover:bg-red-50"
          type="button"
          @click="excluir"
        >
          Excluir
        </button>
      </div>
    </div>

    <p v-if="mensagem" class="alert alert-success">{{ mensagem }}</p>
    <p v-if="erro" class="alert alert-error">{{ erro }}</p>

    <form class="grid gap-5 rounded-lg border border-slate-200 bg-white p-5" @submit.prevent="salvar">
      <div class="grid gap-4 md:grid-cols-2">
        <label>
          <span>Nome</span>
          <input v-model.trim="form.nome" type="text" required maxlength="100" :disabled="!editando" />
        </label>

        <label>
          <span>Usuario vinculado</span>
          <select v-model="idUsuario" :disabled="!editando">
            <option value="">Sem vinculo</option>
            <option v-if="usuarioAtualAusente" :value="integrante?.idUsuario">
              {{ integrante?.usuario?.nome }} - {{ integrante?.usuario?.descricaoPerfil }}
            </option>
            <option v-for="usuario in usuarios" :key="usuario.idUsuario" :value="usuario.idUsuario">
              {{ usuario.nome }} - {{ usuario.descricaoPerfil }}
            </option>
          </select>
        </label>
      </div>

      <div v-if="editando" class="flex flex-wrap justify-end gap-2">
        <button class="rounded-md border border-slate-200 px-4 py-2 text-sm font-bold hover:bg-slate-100" type="button" @click="cancelar">
          Cancelar
        </button>
        <button class="rounded-md bg-blue-600 px-4 py-2 text-sm font-bold text-white hover:bg-blue-700" type="submit" :disabled="salvando">
          {{ salvando ? 'Salvando...' : 'Salvar alteracoes' }}
        </button>
      </div>
    </form>
  </section>
</template>

<script setup lang="ts">
import type { Diretoria, DiretoriaUpdate, UsuarioSummary } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

const { $api } = useNuxtApp()
const route = useRoute()
const auth = useAuthStore()
const integrante = ref<Diretoria | null>(null)
const usuarios = ref<UsuarioSummary[]>([])
const editando = ref(false)
const salvando = ref(false)
const erro = ref('')
const mensagem = ref('')
const idUsuario = ref<number | ''>('')
const form = reactive<DiretoriaUpdate>({
  nome: '',
  idUsuario: null
})

const diretoriaId = computed(() => Number(route.params.id))
const usuarioAtualAusente = computed(() => {
  const idUsuarioAtual = integrante.value?.idUsuario

  return Boolean(
    idUsuarioAtual &&
    integrante.value?.usuario &&
    !usuarios.value.some((usuario) => usuario.idUsuario === idUsuarioAtual)
  )
})

onMounted(async () => {
  await carregar()

  if (auth.canWrite) {
    try {
      usuarios.value = await $api<UsuarioSummary[]>('/usuarios')
    } catch (err) {
      erro.value = normalizeApiError(err)
    }
  }
})

async function carregar() {
  erro.value = ''

  try {
    integrante.value = await $api<Diretoria>(`/diretoria/${diretoriaId.value}`)
    preencherForm(integrante.value)
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}

function preencherForm(value: Diretoria) {
  form.nome = value.nome
  form.idUsuario = value.idUsuario ?? null
  idUsuario.value = value.idUsuario ?? ''
}

function cancelar() {
  if (integrante.value) {
    preencherForm(integrante.value)
  }

  editando.value = false
}

async function salvar() {
  salvando.value = true
  erro.value = ''
  mensagem.value = ''

  try {
    integrante.value = await $api<Diretoria>(`/diretoria/${diretoriaId.value}`, {
      method: 'PUT',
      body: {
        nome: form.nome,
        idUsuario: idUsuario.value === '' ? null : Number(idUsuario.value)
      }
    })
    preencherForm(integrante.value)
    mensagem.value = 'Integrante atualizado.'
    editando.value = false
  } catch (err) {
    erro.value = normalizeApiError(err)
  } finally {
    salvando.value = false
  }
}

async function excluir() {
  if (!integrante.value || !confirm(`Excluir ${integrante.value.nome} da diretoria?`)) {
    return
  }

  erro.value = ''

  try {
    await $api(`/diretoria/${diretoriaId.value}`, { method: 'DELETE' })
    await navigateTo('/diretoria')
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}
</script>
