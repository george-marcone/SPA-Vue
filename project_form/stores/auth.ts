import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { useNuxtApp } from '#app'
import type { AuthResponse, LoginCredentials, UsuarioSummary } from '~/types/api'
import { normalizeApiError } from '~/utils/api-client'

const STORAGE_KEY = 'form-escola-auth'

interface StoredSession {
  token: string
  expiraEm: string
  usuario: UsuarioSummary
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(null)
  const expiraEm = ref<string | null>(null)
  const usuario = ref<UsuarioSummary | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => Boolean(token.value))
  const perfil = computed(() => usuario.value?.descricaoPerfil ?? '')
  const isAdmin = computed(() => perfil.value === 'Administrador')
  const canWrite = computed(() => ['Administrador', 'Contribuinte'].includes(perfil.value))

  async function login(credentials: LoginCredentials) {
    loading.value = true
    error.value = null

    try {
      const { $api } = useNuxtApp()
      const response = await $api<AuthResponse>('/auth/login', {
        method: 'POST',
        body: credentials
      })

      setSession(response)
      return response
    } catch (err) {
      error.value = normalizeApiError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchMe() {
    if (!token.value) {
      return null
    }

    const { $api } = useNuxtApp()
    usuario.value = await $api<UsuarioSummary>('/auth/me')
    persist()
    return usuario.value
  }

  function setSession(session: AuthResponse | StoredSession) {
    token.value = session.token
    expiraEm.value = session.expiraEm
    usuario.value = session.usuario
    persist()
  }

  function logout() {
    token.value = null
    expiraEm.value = null
    usuario.value = null
    error.value = null

    if (canUseStorage()) {
      localStorage.removeItem(STORAGE_KEY)
    }
  }

  function loadFromStorage() {
    if (token.value || !canUseStorage()) {
      return
    }

    const rawSession = localStorage.getItem(STORAGE_KEY)
    if (!rawSession) {
      return
    }

    try {
      const session = JSON.parse(rawSession) as StoredSession
      if (!session.token || !session.usuario) {
        logout()
        return
      }

      setSession(session)
    } catch {
      logout()
    }
  }

  function persist() {
    if (!canUseStorage() || !token.value || !usuario.value || !expiraEm.value) {
      return
    }

    localStorage.setItem(
      STORAGE_KEY,
      JSON.stringify({
        token: token.value,
        expiraEm: expiraEm.value,
        usuario: usuario.value
      })
    )
  }

  return {
    token,
    expiraEm,
    usuario,
    loading,
    error,
    isAuthenticated,
    perfil,
    isAdmin,
    canWrite,
    login,
    fetchMe,
    setSession,
    logout,
    loadFromStorage
  }
})

function canUseStorage() {
  return typeof localStorage !== 'undefined'
}
