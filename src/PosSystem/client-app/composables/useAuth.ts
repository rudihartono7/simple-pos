interface User {
  id: number
  userName: string
  email: string
  firstName: string
  lastName: string
  role: string
  storeId: number
  isActive: boolean
}

interface LoginCredentials {
  email: string
  password: string
}

interface LoginResponse {
  token: string
  refreshToken: string
  expiration: string
  user: User
}

export const useAuth = () => {
  const config = useRuntimeConfig()
  const user = ref<User | null>(null)
  const token = useCookie<string | null>('auth-token', {
    default: () => null,
    secure: false, // Changed to false for development
    sameSite: 'lax' // Changed to lax for better compatibility
  })

  const isAuthenticated = computed(() => {
    const hasToken = !!token.value
    const hasUser = !!user.value
    const result = hasToken && hasUser
    console.log('isAuthenticated computed - token:', hasToken, 'user:', hasUser, 'result:', result)
    return result
  })

  const login = async (credentials: LoginCredentials): Promise<boolean> => {
    try {
      const response = await $fetch<LoginResponse>('/api/auth/login', {
        method: 'POST',
        body: credentials,
        baseURL: config.public.apiBase
      })


      console.log('Login response:', response)

      token.value = response.token as string | null
      user.value = response.user

      console.log('value of user:', user.value)

      console.log('Login successful, token:', token.value, 'user:', user.value)
      console.log('isAuthenticated after login:', !!token.value && !!user.value)
      
      return true
    } catch (error) {
      console.error('Login failed:', error)
      return false
    }
  }

  const logout = async () => {
    try {
      if (token.value) {
        await $fetch('/api/auth/logout', {
          method: 'POST',
          headers: {
            Authorization: `Bearer ${token.value}`
          },
          baseURL: config.public.apiBase
        })
      }
    } catch (error) {
      console.error('Logout error:', error)
    } finally {
      token.value = null
      user.value = null
      await navigateTo('/login')
    }
  }

  const getCurrentUser = async () => {
    if (!token.value) return null

    try {
      const userData = await $fetch<User>('/api/auth/current-user', {
        headers: {
          Authorization: `Bearer ${token.value}`
        },
        baseURL: config.public.apiBase
      })
      
      user.value = userData
      return userData
    } catch (error) {
      console.error('Failed to get current user:', error)
      token.value = null
      user.value = null
      return null
    }
  }

  // Initialize user on composable creation
  onMounted(async () => {
    console.log('useAuth onMounted - token:', token.value, 'user:', user.value)
    if (token.value && !user.value) {
      console.log('Token exists but no user, fetching current user...')
      await getCurrentUser()
      console.log('After getCurrentUser - user:', user.value, 'isAuthenticated:', !!token.value && !!user.value)
    }
  })

  return {
    user: readonly(user),
    token: readonly(token),
    isAuthenticated,
    login,
    logout,
    getCurrentUser
  }
}