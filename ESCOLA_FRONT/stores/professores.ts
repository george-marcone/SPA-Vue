import { ref } from 'vue'
import { defineStore } from 'pinia'
import { useNuxtApp } from '#app'
import type { Professor, ProfessorCreate, ProfessorUpdate } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

export const useProfessoresStore = defineStore('professores', () => {
  const professores = ref<Professor[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchProfessores() {
    loading.value = true
    error.value = null

    try {
      const { $api } = useNuxtApp()
      professores.value = await $api<Professor[]>('/professor')
      return professores.value
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function createProfessor(payload: ProfessorCreate) {
    loading.value = true
    error.value = null

    try {
      const { $api } = useNuxtApp()
      const created = await $api<Professor>('/professor', {
        method: 'POST',
        body: payload
      })

      professores.value = [created, ...professores.value]
      return created
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateProfessor(id: number, payload: ProfessorUpdate) {
    loading.value = true
    error.value = null

    try {
      const { $api } = useNuxtApp()
      const updated = await $api<Professor>(`/professor/${id}`, {
        method: 'PUT',
        body: payload
      })

      professores.value = professores.value.map((professor) =>
        professor.id === id ? updated : professor
      )
      return updated
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function deleteProfessor(id: number) {
    loading.value = true
    error.value = null

    try {
      const { $api } = useNuxtApp()
      await $api<void>(`/professor/${id}`, {
        method: 'DELETE'
      })

      professores.value = professores.value.filter((professor) => professor.id !== id)
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  return {
    professores,
    loading,
    error,
    fetchProfessores,
    createProfessor,
    updateProfessor,
    deleteProfessor
  }
})
