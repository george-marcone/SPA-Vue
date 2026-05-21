export default defineNuxtRouteMiddleware((to) => {
  const auth = useAuthStore()
  auth.loadFromStorage()

  if (to.meta.public) {
    if (auth.isAuthenticated) {
      return navigateTo('/')
    }

    return
  }

  if (!auth.isAuthenticated) {
    return navigateTo('/login')
  }

  const roles = to.meta.roles as string[] | undefined
  if (roles?.length && auth.usuario && !roles.includes(auth.usuario.descricaoPerfil)) {
    return navigateTo('/')
  }
})
