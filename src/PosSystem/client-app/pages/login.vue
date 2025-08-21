<template>
  <div class="min-h-screen flex">
    <!-- Left Side - Welcome Section -->
    <div class="flex-1 bg-gradient-to-br from-orange-600 via-orange-700 to-white-600 flex items-center justify-center p-12 relative overflow-hidden">
      <!-- Decorative Elements -->
      <div class="absolute inset-0">
        <!-- Geometric shapes -->
        <div class="absolute top-20 left-20 w-32 h-8 bg-gradient-to-r from-orange-400 to-white-400 rounded-full transform rotate-45 opacity-80"></div>
        <div class="absolute top-40 left-40 w-24 h-6 bg-gradient-to-r from-yellow-400 to-orange-400 rounded-full transform rotate-12 opacity-70"></div>
        <div class="absolute top-60 left-16 w-20 h-5 bg-gradient-to-r from-pink-400 to-red-400 rounded-full transform -rotate-12 opacity-60"></div>
        <div class="absolute bottom-40 left-32 w-28 h-7 bg-gradient-to-r from-orange-400 to-yellow-400 rounded-full transform rotate-30 opacity-75"></div>
        <div class="absolute bottom-60 left-60 w-16 h-4 bg-gradient-to-r from-red-400 to-pink-400 rounded-full transform -rotate-45 opacity-65"></div>
        
        <!-- Additional decorative elements -->
        <div class="absolute top-32 right-40 w-36 h-9 bg-gradient-to-r from-yellow-400 to-orange-500 rounded-full transform rotate-60 opacity-70"></div>
        <div class="absolute top-80 right-20 w-22 h-6 bg-gradient-to-r from-orange-400 to-red-400 rounded-full transform -rotate-30 opacity-60"></div>
        <div class="absolute bottom-32 right-48 w-26 h-7 bg-gradient-to-r from-pink-400 to-purple-400 rounded-full transform rotate-15 opacity-65"></div>
      </div>
      
      <!-- Welcome Content -->
      <div class="relative z-10 text-white max-w-lg">
        <h1 class="text-5xl font-bold mb-6 leading-tight">
          Welcome to POS System
        </h1>
        <p class="text-xl text-purple-100 leading-relaxed">
          Streamline your business operations with our comprehensive point-of-sale solution. Manage inventory, track sales, and grow your business efficiently.
        </p>
      </div>
    </div>
    
    <!-- Right Side - Login Form -->
    <div class="flex-1 bg-gray-50 flex items-center justify-center p-12">
      <div class="w-full max-w-md">
        <!-- Logo and Header -->
        <div class="text-center mb-8">
          <div class="mx-auto w-16 h-16 bg-white shadow-lg rounded-full flex items-center justify-center mb-4">
            <img src="/images/logo.png" alt="Logo" class="h-8 w-8 object-contain">
          </div>
          <h2 class="text-2xl font-bold text-gray-900 mb-2">
            USER LOGIN
          </h2>
        </div>
        
        <!-- Login Form -->
        <div class="bg-white rounded-2xl shadow-xl p-8">
          <form class="space-y-6" @submit.prevent="handleLogin">
            <div>
              <input
                id="email"
                v-model="credentials.email"
                name="email"
                type="email"
                autocomplete="email"
                required
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent transition-all duration-200 text-gray-900 placeholder-gray-500"
                placeholder="Email address"
              />
            </div>
            
            <div>
              <input
                id="password"
                v-model="credentials.password"
                name="password"
                type="password"
                autocomplete="current-password"
                required
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent transition-all duration-200 text-gray-900 placeholder-gray-500"
                placeholder="Password"
              />
            </div>
            
            <div class="flex items-center justify-between">
              <label class="flex items-center">
                <input type="checkbox" class="rounded border-gray-300 text-purple-600 focus:ring-purple-500">
                <span class="ml-2 text-sm text-gray-600">Remember me</span>
              </label>
              <a href="#" class="text-sm text-purple-600 hover:text-purple-800 transition-colors">
                Forgot password?
              </a>
            </div>

            <div v-if="error" class="bg-red-50 border border-red-200 rounded-lg p-3">
              <div class="flex">
                <div class="flex-shrink-0">
                  <svg class="h-5 w-5 text-red-400" viewBox="0 0 20 20" fill="currentColor">
                    <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
                  </svg>
                </div>
                <div class="ml-3">
                  <p class="text-sm text-red-600">{{ error }}</p>
                </div>
              </div>
            </div>

            <div>
              <button
                type="submit"
                :disabled="loading"
                class="w-full bg-gradient-to-r from-orange-600 to-white-600 text-white py-3 px-4 rounded-lg font-semibold hover:from-purple-700 hover:to-pink-700 focus:outline-none focus:ring-2 focus:ring-purple-500 focus:ring-offset-2 transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed flex justify-center items-center"
              >
                <svg v-if="loading" class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                <span v-if="loading">Signing in...</span>
                <span v-else>LOGIN</span>
              </button>
            </div>
            
            <div class="text-center">
              <p class="text-xs text-gray-500">
                Default: admin@possystem.com / Admin123!
              </p>
            </div>
          </form>
        </div>
      </div>
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