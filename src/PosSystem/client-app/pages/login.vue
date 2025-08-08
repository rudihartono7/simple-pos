<template>
  <div class="max-w-md w-full space-y-8">
    <div class="text-center">
      <div class="mx-auto h-12 w-12 bg-indigo-600 rounded-full flex items-center justify-center mb-4">
        <svg class="h-6 w-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
        </svg>
      </div>
      <h2 class="text-3xl font-extrabold text-gray-900">
        Sign in to POS System
      </h2>
      <p class="mt-2 text-sm text-gray-600">
        Enter your credentials to access the system
      </p>
    </div>
    
    <div class="bg-white py-8 px-6 shadow-xl rounded-lg">
      <form class="space-y-6" @submit.prevent="handleLogin">
        <div>
          <label for="email" class="block text-sm font-medium text-gray-700 mb-2">
            Email address
          </label>
          <input
            id="email"
            v-model="credentials.email"
            name="email"
            type="email"
            autocomplete="email"
            required
            class="form-input"
            placeholder="Enter your email"
          />
        </div>
        
        <div>
          <label for="password" class="block text-sm font-medium text-gray-700 mb-2">
            Password
          </label>
          <input
            id="password"
            v-model="credentials.password"
            name="password"
            type="password"
            autocomplete="current-password"
            required
            class="form-input"
            placeholder="Enter your password"
          />
        </div>

        <div v-if="error" class="bg-red-50 border border-red-200 rounded-md p-3">
          <div class="flex">
            <div class="flex-shrink-0">
              <svg class="h-5 w-5 text-red-400" viewBox="0 0 20 20" fill="currentColor">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
              </svg>
            </div>
            <div class="ml-3">
              <p class="text-sm text-red-800">{{ error }}</p>
            </div>
          </div>
        </div>

        <div>
          <button
            type="submit"
            :disabled="loading"
            class="btn-primary w-full flex justify-center items-center"
          >
            <svg v-if="loading" class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            <span v-if="loading">Signing in...</span>
            <span v-else>Sign in</span>
          </button>
        </div>
        
        <div class="text-center">
          <p class="text-xs text-gray-500">
            Default credentials: admin@possystem.com / Admin123!
          </p>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'auth'
})

const { login, isAuthenticated, isInitialized } = useAuth()

const credentials = ref({
  email: 'admin@possystem.com',
  password: 'Admin123!'
})

const loading = ref(false)
const error = ref('')

// Redirect if already authenticated - only on client side
onMounted(async () => {
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
  
  if (isAuthenticated.value) {
    console.log('Already authenticated, redirecting to /dashboard')
    navigateTo('/dashboard', { replace: true })
  }
})

const handleLogin = async () => {
  loading.value = true
  error.value = ''
  
  try {
    const success = await login(credentials.value)
    if (success) {
      console.log('Login successful, redirecting to dashboard...')
      
      // Small delay to ensure state is updated
      await new Promise(resolve => setTimeout(resolve, 100))
      
      await navigateTo('/dashboard', { replace: true })
    } else {
      error.value = 'Invalid email or password. Please check your credentials and try again.'
    }
  } catch (err) {
    console.error('Login error:', err)
    error.value = 'Login failed. Please check your connection and try again.'
  } finally {
    loading.value = false
  }
}
</script>