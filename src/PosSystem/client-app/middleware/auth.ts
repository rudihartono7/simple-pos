export default defineNuxtRouteMiddleware((to) => {
  const { isAuthenticated, token, user } = useAuth()
  
  console.log('Auth middleware - isAuthenticated:', isAuthenticated.value, 'route:', to.path)
  console.log('Auth middleware - token exists:', !!token.value, 'user exists:', !!user.value)
  console.log('Auth middleware - user:', user.value);
  
  // if (!isAuthenticated.value) {
  //   console.log('Not authenticated, redirecting to login')
  //   return navigateTo('/login')
  // }
})