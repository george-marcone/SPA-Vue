<template>
  <section class="stack">
    <div class="page-heading">
      <p class="eyebrow">Painel</p>
      <h1>Cadastros da escola</h1>
    </div>

    <div class="summary-strip">
      <div>
        <span>Usuario</span>
        <strong>{{ auth.usuario?.nome }}</strong>
      </div>
      <div>
        <span>Perfil</span>
        <strong>{{ auth.perfil }}</strong>
      </div>
    </div>

    <div class="action-grid">
      <NuxtLink class="action-link" to="/diretoria">
        <span>Diretoria</span>
        <strong>Gerenciar diretoria</strong>
      </NuxtLink>
      <NuxtLink class="action-link" to="/professores">
        <span>Professores</span>
        <strong>Gerenciar professores</strong>
      </NuxtLink>
      <NuxtLink class="action-link" to="/alunos">
        <span>Alunos</span>
        <strong>Gerenciar alunos</strong>
      </NuxtLink>
      <NuxtLink v-if="auth.isAdmin" class="action-link" to="/usuarios">
        <span>Usuarios</span>
        <strong>Gerenciar usuarios</strong>
      </NuxtLink>
    </div>
  </section>
</template>

<script setup lang="ts">
const auth = useAuthStore()
</script>
