<template>
  <section class="grid gap-6">
    <div class="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
      <div>
        <p class="eyebrow">Usuarios</p>
        <h1 class="m-0 text-3xl font-extrabold text-slate-900">
          {{ editando ? 'Editar usuario' : 'Visualizar usuario' }}
        </h1>
      </div>
      <div class="flex flex-wrap gap-2">
        <NuxtLink class="rounded-md border border-slate-200 px-4 py-2 text-sm font-bold no-underline hover:bg-slate-100" to="/usuarios">
          Voltar
        </NuxtLink>
        <button
          v-if="auth.isAdmin && !editando"
          class="rounded-md bg-[#147f72] px-4 py-2 text-sm font-bold text-white hover:bg-[#0f6c61]"
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
          <span>Email</span>
          <input v-model.trim="form.email" type="email" required maxlength="150" :disabled="!editando" />
        </label>

        <label>
          <span>Telefone</span>
          <input v-model.trim="form.telefone" type="tel" required maxlength="20" :disabled="!editando" />
        </label>

        <label>
          <span>Perfil</span>
          <select v-model.number="form.idPerfil" required :disabled="!editando">
            <option v-if="!perfis.length" :value="form.idPerfil">{{ usuario?.descricaoPerfil || 'Perfil atual' }}</option>
            <option v-for="perfil in perfis" :key="perfil.idPerfil" :value="perfil.idPerfil">
              {{ perfil.descricaoPerfil }}
            </option>
          </select>
        </label>
      </div>

      <div v-if="editando" class="flex flex-wrap justify-end gap-2">
        <button class="rounded-md border border-slate-200 px-4 py-2 text-sm font-bold hover:bg-slate-100" type="button" @click="cancelar">
          Cancelar
        </button>
        <button class="rounded-md bg-[#147f72] px-4 py-2 text-sm font-bold text-white hover:bg-[#0f6c61]" type="submit" :disabled="salvando">
          {{ salvando ? 'Salvando...' : 'Salvar alteracoes' }}
        </button>
      </div>
    </form>
  </section>
</template>

<script setup lang="ts">
import type { Perfil, UsuarioSummary, UsuarioUpdate } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'
import { DUPLICATE_USER_EMAIL_MESSAGE, isDuplicateUserEmail } from '~/utils/usuario-validation'

definePageMeta({
  roles: ['Administrador', 'Contribuinte']
})

const { $api } = useNuxtApp()
const route = useRoute()
const auth = useAuthStore()
const usuario = ref<UsuarioSummary | null>(null)
const perfis = ref<Perfil[]>([])
const usuarios = ref<UsuarioSummary[]>([])
const editando = ref(false)
const salvando = ref(false)
const erro = ref('')
const mensagem = ref('')
const form = reactive<UsuarioUpdate>({
  nome: '',
  email: '',
  telefone: '',
  idPerfil: 0
})

const usuarioId = computed(() => Number(route.params.id))

onMounted(async () => {
  await carregar()

  if (auth.isAdmin) {
    try {
      const [perfisResponse, usuariosResponse] = await Promise.all([
        $api<Perfil[]>('/usuarios/perfis'),
        $api<UsuarioSummary[]>('/usuarios')
      ])
      perfis.value = perfisResponse
      usuarios.value = usuariosResponse
    } catch (err) {
      erro.value = normalizeApiError(err)
    }
  }
})

async function carregar() {
  erro.value = ''

  try {
    usuario.value = await $api<UsuarioSummary>(`/usuarios/${usuarioId.value}`)
    preencherForm(usuario.value)
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}

function preencherForm(value: UsuarioSummary) {
  form.nome = value.nome
  form.email = value.email
  form.telefone = value.telefone
  form.idPerfil = value.idPerfil
}

function cancelar() {
  if (usuario.value) {
    preencherForm(usuario.value)
  }

  editando.value = false
}

async function salvar() {
  erro.value = ''
  mensagem.value = ''

  if (isDuplicateUserEmail(usuarios.value, form.email, usuarioId.value)) {
    erro.value = DUPLICATE_USER_EMAIL_MESSAGE
    return
  }

  salvando.value = true

  try {
    usuario.value = await $api<UsuarioSummary>(`/usuarios/${usuarioId.value}`, {
      method: 'PUT',
      body: { ...form }
    })
    preencherForm(usuario.value)
    mensagem.value = 'Usuario atualizado.'
    editando.value = false
  } catch (err) {
    erro.value = normalizeApiError(err)
  } finally {
    salvando.value = false
  }
}

async function excluir() {
  if (!usuario.value || !confirm(`Excluir o usuario ${usuario.value.nome}?`)) {
    return
  }

  erro.value = ''

  try {
    await $api(`/usuarios/${usuarioId.value}`, { method: 'DELETE' })
    await navigateTo('/usuarios')
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}
</script>
