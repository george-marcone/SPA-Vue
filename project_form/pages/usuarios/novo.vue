<template>
  <section class="stack">
    <div class="page-heading">
      <p class="eyebrow">Usuarios</p>
      <h1>Novo usuario</h1>
    </div>

    <form class="form-panel" @submit.prevent="salvar">
      <div class="form-grid two-columns">
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
            <option v-for="perfil in usuarios.perfis" :key="perfil.idPerfil" :value="perfil.idPerfil">
              {{ perfil.descricaoPerfil }}
            </option>
          </select>
        </label>

        <label>
          <span>Senha</span>
          <input v-model="form.senha" type="password" required minlength="8" autocomplete="new-password" />
        </label>

        <label>
          <span>Confirmar senha</span>
          <input v-model="confirmacaoSenha" type="password" required minlength="8" autocomplete="new-password" />
        </label>
      </div>

      <p v-if="mensagem" class="alert alert-success">{{ mensagem }}</p>
      <p v-if="erro" class="alert alert-error">{{ erro }}</p>

      <div class="form-actions">
        <NuxtLink class="btn btn-secondary" to="/">Cancelar</NuxtLink>
        <button class="btn btn-primary" type="submit" :disabled="usuarios.loading">
          {{ usuarios.loading ? 'Salvando...' : 'Salvar usuario' }}
        </button>
      </div>
    </form>
  </section>
</template>

<script setup lang="ts">
import { normalizeApiError } from '~/utils/api-client'

definePageMeta({
  roles: ['Administrador']
})

const usuarios = useUsuariosStore()
const mensagem = ref('')
const erro = ref('')
const confirmacaoSenha = ref('')
const form = reactive({
  nome: '',
  email: '',
  telefone: '',
  senha: '',
  idPerfil: 0
})

onMounted(() => {
  usuarios.fetchPerfis().catch((err) => {
    erro.value = normalizeApiError(err)
  })
})

async function salvar() {
  mensagem.value = ''
  erro.value = ''

  if (form.senha !== confirmacaoSenha.value) {
    erro.value = 'As senhas nao conferem.'
    return
  }

  try {
    const created = await usuarios.createUsuario({ ...form })
    mensagem.value = `Usuario ${created.nome} cadastrado.`
    form.nome = ''
    form.email = ''
    form.telefone = ''
    form.senha = ''
    form.idPerfil = 0
    confirmacaoSenha.value = ''
  } catch (err) {
    erro.value = normalizeApiError(err)
  }
}
</script>
