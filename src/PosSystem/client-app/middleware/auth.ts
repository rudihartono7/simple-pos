export default defineNuxtRouteMiddleware(async (to, from) => {
  // Skip on server-side to prevent hydration issues
  if (process.server) {
    return
  }

  // Wait for next tick to ensure auth state is initialized
  await nextTick()

  const { isAuthenticated, isInitialized } = useAuth()

  // Wait for auth to be initialized
  if (!isInitialized.value) {
    await new Promise(resolve => {
      const unwatch = watch(isInitialized, (initialized) => {
        if (initialized) {
          unwatch()
          resolve(true)
        }
      }, { immediate: true })
    })
  }

  if (!isAuthenticated.value) {
    console.log('Not authenticated, redirecting to /login')
    return navigateTo('/login')
  }
})