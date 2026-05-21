import { createPinia, setActivePinia } from 'pinia'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { useAuthStore } from '~/stores/auth'
import type { AuthResponse } from '~/types/api'

const { apiMock } = vi.hoisted(() => ({
  apiMock: vi.fn()
}))

vi.mock('#app', () => ({
  useNuxtApp: () => ({
    $api: apiMock
  })
}))

describe('auth store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    apiMock.mockReset()
  })

  it('stores JWT session after login', async () => {
    const response: AuthResponse = {
      token: 'jwt-token',
      expiraEm: '2026-05-21T22:00:00Z',
      usuario: {
        idUsuario: 1,
        nome: 'Administrador Sistema',
        email: 'admin@escola.com',
        telefone: '11999990001',
        idPerfil: 1,
        descricaoPerfil: 'Administrador'
      }
    }
    apiMock.mockResolvedValue(response)

    const auth = useAuthStore()
    await auth.login({ email: 'admin@escola.com', senha: 'Senha@123' })

    expect(apiMock).toHaveBeenCalledWith('/auth/login', {
      method: 'POST',
      body: { email: 'admin@escola.com', senha: 'Senha@123' }
    })
    expect(auth.token).toBe('jwt-token')
    expect(auth.isAuthenticated).toBe(true)
    expect(auth.isAdmin).toBe(true)
    expect(localStorage.getItem('form-escola-auth')).toContain('jwt-token')
  })

  it('clears session on logout', () => {
    const auth = useAuthStore()
    auth.setSession({
      token: 'jwt-token',
      expiraEm: '2026-05-21T22:00:00Z',
      usuario: {
        idUsuario: 1,
        nome: 'Administrador Sistema',
        email: 'admin@escola.com',
        telefone: '11999990001',
        idPerfil: 1,
        descricaoPerfil: 'Administrador'
      }
    })

    auth.logout()

    expect(auth.token).toBeNull()
    expect(auth.usuario).toBeNull()
    expect(auth.isAuthenticated).toBe(false)
    expect(localStorage.getItem('form-escola-auth')).toBeNull()
  })
})
