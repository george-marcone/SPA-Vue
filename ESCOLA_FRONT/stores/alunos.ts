import { ref } from 'vue'
import { defineStore } from 'pinia'
import { useNuxtApp } from '#app'
import type { Aluno, AlunoCreate, AlunoUpdate } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

export const useAlunosStore = defineStore('alunos', () => {
  const alunos = ref<Aluno[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchAlunos() {
    loading.value = true
    error.value = null

    try {
      const { $api } = useNuxtApp()
      alunos.value = await $api<Aluno[]>('/aluno')
      return alunos.value
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function createAluno(payload: AlunoCreate) {
    loading.value = true
    error.value = null

    try {
      const { $api } = useNuxtApp()
      const created = await $api<Aluno>('/aluno', {
        method: 'POST',
        body: payload
      })

      alunos.value = [created, ...alunos.value]
      return created
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateAluno(id: number, payload: AlunoUpdate) {
    loading.value = true
    error.value = null

    try {
      const { $api } = useNuxtApp()
      const updated = await $api<Aluno>(`/aluno/${id}`, {
        method: 'PUT',
        body: payload
      })

      alunos.value = alunos.value.map((aluno) => aluno.id === id ? updated : aluno)
      return updated
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function deleteAluno(id: number) {
    loading.value = true
    error.value = null

    try {
      const { $api } = useNuxtApp()
      await $api<void>(`/aluno/${id}`, {
        method: 'DELETE'
      })

      alunos.value = alunos.value.filter((aluno) => aluno.id !== id)
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  return {
    alunos,
    loading,
    error,
    fetchAlunos,
    createAluno,
    updateAluno,
    deleteAluno
  }
})
