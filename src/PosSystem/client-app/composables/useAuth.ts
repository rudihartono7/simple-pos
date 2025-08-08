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

// Global state to ensure single instance
const globalUser = ref<User | null>(null)
const globalToken = useCookie<string | null>('auth-token', {
  default: () => null,
  secure: false,
  sameSite: 'lax'
})
const globalAuthLoading = ref(false)
const globalAuthInitialized = ref(false)

// Initialize user data immediately when token exists
const initializeAuth = async () => {
  if (process.client && globalToken.value && !globalUser.value && !globalAuthInitialized.value) {
    globalAuthLoading.value = true
    try {
      const config = useRuntimeConfig()
      const userData = await $fetch<User>('/api/auth/current-user', {
        headers: {
          Authorization: `Bearer ${globalToken.value}`
        },
        baseURL: config.public.apiBase
      })
      
      globalUser.value = userData
      console.log('Auth initialized with user:', userData)
    } catch (error) {
      console.error('Failed to initialize auth:', error)
      globalToken.value = null
      globalUser.value = null
    } finally {
      globalAuthLoading.value = false
      globalAuthInitialized.value = true
    }
  } else if (process.client && !globalToken.value) {
    globalAuthInitialized.value = true
  }
}

// Initialize immediately
if (process.client) {
  initializeAuth()
}

export const useAuth = () => {
  const config = useRuntimeConfig()

  const isAuthenticated = computed(() => {
    const hasToken = !!globalToken.value
    const hasUser = !!globalUser.value
    const result = hasToken && hasUser
    console.log('isAuthenticated computed - token:', hasToken, 'user:', hasUser, 'result:', result)
    return result
  })

  const isLoading = computed(() => globalAuthLoading.value)
  const isInitialized = computed(() => globalAuthInitialized.value)

  const login = async (credentials: LoginCredentials): Promise<boolean> => {
    try {
      const response = await $fetch<LoginResponse>('/api/auth/login', {
        method: 'POST',
        body: credentials,
        baseURL: config.public.apiBase
      })

      console.log('Login response:', response)

      globalToken.value = response.token as string | null
      globalUser.value = response.user
      globalAuthInitialized.value = true

      console.log('Login successful, token:', globalToken.value, 'user:', globalUser.value)
      console.log('isAuthenticated after login:', !!globalToken.value && !!globalUser.value)
      
      return true
    } catch (error) {
      console.error('Login failed:', error)
      return false
    }
  }

  const logout = async () => {
    try {
      if (globalToken.value) {
        await $fetch('/api/auth/logout', {
          method: 'POST',
          headers: {
            Authorization: `Bearer ${globalToken.value}`
          },
          baseURL: config.public.apiBase
        })
      }
    } catch (error) {
      console.error('Logout error:', error)
    } finally {
      globalToken.value = null
      globalUser.value = null
      await navigateTo('/login')
    }
  }

  const getCurrentUser = async () => {
    if (!globalToken.value) return null

    try {
      const userData = await $fetch<User>('/api/auth/current-user', {
        headers: {
          Authorization: `Bearer ${globalToken.value}`
        },
        baseURL: config.public.apiBase
      })
      
      globalUser.value = userData
      return userData
    } catch (error) {
      console.error('Failed to get current user:', error)
      globalToken.value = null
      globalUser.value = null
      return null
    }
  }

  return {
    user: readonly(globalUser),
    token: readonly(globalToken),
    isAuthenticated,
    isLoading,
    isInitialized,
    login,
    logout,
    getCurrentUser
  }
}