<template>
  <section class="grid gap-6">
    <div class="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
      <div>
        <p class="eyebrow">Alunos</p>
        <h1 class="m-0 text-3xl font-extrabold text-slate-900">
          {{ editando ? 'Editar aluno' : 'Visualizar aluno' }}
        </h1>
      </div>
      <div class="flex flex-wrap gap-2">
        <NuxtLink class="rounded-md border border-slate-200 px-4 py-2 text-sm font-bold no-underline hover:bg-slate-100" to="/alunos">
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
          <span>Sobrenome</span>
          <input v-model.trim="form.sobrenome" type="text" required maxlength="100" :disabled="!editando" />
        </label>

        <label>
          <span>Data de nascimento</span>
          <input v-model.trim="form.dataNasc" type="text" required placeholder="dd/MM/aaaa" maxlength="10" :disabled="!editando" />
        </label>

        <label>
          <span>Professor</span>
          <select v-model.number="form.professorId" required :disabled="!editando">
            <option disabled :value="0">Selecione</option>
            <option v-for="professor in professores" :key="professor.id" :value="professor.id">
              {{ professor.nome }}
            </option>
          </select>
        </label>

        <label>
          <span>Usuario vinculado</span>
          <select v-model="idUsuario" :disabled="!editando">
            <option value="">Sem vinculo</option>
            <option v-if="usuarioAtualAusente" :value="aluno?.idUsuario">
              {{ aluno?.usuario?.nome }} - {{ aluno?.usuario?.descricaoPerfil }}
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
import type { Aluno, AlunoUpdate, Professor, UsuarioSummary } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

const { $api } = useNuxtApp()
const route = useRoute()
const auth = useAuthStore()
const aluno = ref<Aluno | null>(null)
const professores = ref<Professor[]>([])
const usuarios = ref<UsuarioSummary[]>([])
const editando = ref(false)
const salvando = ref(false)
const erro = ref('')
const mensagem = ref('')
const idUsuario = ref<number | ''>('')
const form = reactive<AlunoUpdate>({
  nome: '',
  sobrenome: '',
  dataNasc: '',
  professorId: 0,
  idUsuario: null
})

const alunoId = computed(() => Number(route.params.id))
const usuarioAtualAusente = computed(() => {
  const idUsuarioAtual = aluno.value?.idUsuario

  return Boolean(
    idUsuarioAtual &&
    aluno.value?.usuario &&
    !usuarios.value.some((usuario) => usuario.idUsuario === idUsuarioAtual)
  )
})

onMounted(async () => {
  await carregar()

  try {
    const [professoresResponse, usuariosResponse] = await Promise.all([
      $api<Professor[]>('/professor'),
      auth.canWrite ? $api<UsuarioSummary[]>('/usuarios') : Promise.resolve<UsuarioSummary[]>([])
    ])
    professores.value = professoresResponse
    usuarios.value = usuariosResponse
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
})

async function carregar() {
  erro.value = ''

  try {
    aluno.value = await $api<Aluno>(`/aluno/${alunoId.value}`)
    preencherForm(aluno.value)
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}

function preencherForm(value: Aluno) {
  form.nome = value.nome
  form.sobrenome = value.sobrenome
  form.dataNasc = value.dataNasc
  form.professorId = value.professorId
  form.idUsuario = value.idUsuario ?? null
  idUsuario.value = value.idUsuario ?? ''
}

function cancelar() {
  if (aluno.value) {
    preencherForm(aluno.value)
  }

  editando.value = false
}

async function salvar() {
  salvando.value = true
  erro.value = ''
  mensagem.value = ''

  try {
    aluno.value = await $api<Aluno>(`/aluno/${alunoId.value}`, {
      method: 'PUT',
      body: {
        ...form,
        idUsuario: idUsuario.value === '' ? null : Number(idUsuario.value)
      }
    })
    preencherForm(aluno.value)
    mensagem.value = 'Aluno atualizado.'
    editando.value = false
  } catch (err) {
    erro.value = normalizeApiError(err)
  } finally {
    salvando.value = false
  }
}

async function excluir() {
  if (!aluno.value || !confirm(`Excluir o aluno ${aluno.value.nome} ${aluno.value.sobrenome}?`)) {
    return
  }

  erro.value = ''

  try {
    await $api(`/aluno/${alunoId.value}`, { method: 'DELETE' })
    await navigateTo('/alunos')
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}
</script>
