<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Main Content -->
    <main class="max-w-7xl mx-auto py-4 sm:py-6 lg:py-8 px-3 sm:px-4 lg:px-8">
      <!-- Welcome Section -->
      <div class="mb-6 sm:mb-8">
        <div class="bg-gradient-to-r from-blue-600 to-blue-700 rounded-lg shadow-lg p-4 sm:p-6 lg:p-8 text-white">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-xl sm:text-2xl lg:text-3xl font-bold mb-1 sm:mb-2">Welcome Back!</h1>
              <p class="text-blue-100 text-sm sm:text-base lg:text-lg">
                Good to see you, {{ user?.firstName }}. Your role: <span class="font-semibold">{{ user?.role }}</span>
              </p>
            </div>
            <div class="hidden sm:block">
              <svg class="w-10 h-10 sm:w-12 sm:h-12 lg:w-16 lg:h-16 text-blue-200" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
              </svg>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Quick Stats -->
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-3 sm:gap-4 lg:gap-6 mb-6 sm:mb-8">
        <div class="bg-white rounded-lg shadow-sm p-4 sm:p-5 lg:p-6 border border-gray-200">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <div class="w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 bg-blue-100 rounded-lg flex items-center justify-center">
                <svg class="w-3 h-3 sm:w-4 sm:h-4 lg:w-5 lg:h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4"></path>
                </svg>
              </div>
            </div>
            <div class="ml-3 sm:ml-4">
              <h3 class="text-xs sm:text-sm font-medium text-gray-500">Total Products</h3>
              <p class="text-lg sm:text-xl lg:text-2xl font-bold text-gray-900">{{ stats.products }}</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white rounded-lg shadow-sm p-4 sm:p-5 lg:p-6 border border-gray-200">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <div class="w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 bg-green-100 rounded-lg flex items-center justify-center">
                <svg class="w-3 h-3 sm:w-4 sm:h-4 lg:w-5 lg:h-5 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z"></path>
                </svg>
              </div>
            </div>
            <div class="ml-3 sm:ml-4">
              <h3 class="text-xs sm:text-sm font-medium text-gray-500">Transactions</h3>
              <p class="text-lg sm:text-xl lg:text-2xl font-bold text-gray-900">{{ stats.transactions }}</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white rounded-lg shadow-sm p-4 sm:p-5 lg:p-6 border border-gray-200">
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <div class="w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 bg-purple-100 rounded-lg flex items-center justify-center">
                <svg class="w-3 h-3 sm:w-4 sm:h-4 lg:w-5 lg:h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z"></path>
                </svg>
              </div>
            </div>
            <div class="ml-3 sm:ml-4">
              <h3 class="text-xs sm:text-sm font-medium text-gray-500">Customers</h3>
              <p class="text-lg sm:text-xl lg:text-2xl font-bold text-gray-900">{{ stats.customers }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Navigation Cards -->
      <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-4 sm:p-5 lg:p-6">
        <h2 class="text-base sm:text-lg font-semibold text-gray-900 mb-4 sm:mb-6">Quick Access</h2>
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-3 sm:gap-4">
          <NuxtLink
            to="/products"
            class="group bg-white border border-gray-200 p-4 sm:p-5 lg:p-6 rounded-lg shadow-sm hover:shadow-md hover:border-blue-300 transition-all duration-200"
          >
            <div class="flex items-center mb-2 sm:mb-3">
              <div class="w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 bg-blue-100 rounded-lg flex items-center justify-center group-hover:bg-blue-200 transition-colors">
                <svg class="w-3 h-3 sm:w-4 sm:h-4 lg:w-5 lg:h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4"></path>
                </svg>
              </div>
            </div>
            <h3 class="text-sm sm:text-base lg:text-lg font-medium text-gray-900 mb-1 sm:mb-2">Products</h3>
            <p class="text-xs sm:text-sm text-gray-600">Manage inventory and product catalog</p>
          </NuxtLink>
          
          <NuxtLink
            to="/categories"
            class="group bg-white border border-gray-200 p-4 sm:p-5 lg:p-6 rounded-lg shadow-sm hover:shadow-md hover:border-green-300 transition-all duration-200"
          >
            <div class="flex items-center mb-2 sm:mb-3">
              <div class="w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 bg-green-100 rounded-lg flex items-center justify-center group-hover:bg-green-200 transition-colors">
                <svg class="w-3 h-3 sm:w-4 sm:h-4 lg:w-5 lg:h-5 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"></path>
                </svg>
              </div>
            </div>
            <h3 class="text-sm sm:text-base lg:text-lg font-medium text-gray-900 mb-1 sm:mb-2">Categories</h3>
            <p class="text-xs sm:text-sm text-gray-600">Manage product categories and classifications</p>
          </NuxtLink>
          
          <NuxtLink
            to="/pos"
            class="group bg-gradient-to-r from-blue-600 to-blue-700 text-white p-4 sm:p-5 lg:p-6 rounded-lg shadow-sm hover:shadow-md hover:from-blue-700 hover:to-blue-800 transition-all duration-200"
          >
            <div class="flex items-center mb-2 sm:mb-3">
              <div class="w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 bg-white bg-opacity-20 rounded-lg flex items-center justify-center group-hover:bg-opacity-30 transition-colors">
                <svg class="w-3 h-3 sm:w-4 sm:h-4 lg:w-5 lg:h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2 2v14a2 2 0 002 2z"></path>
                </svg>
              </div>
            </div>
            <h3 class="text-sm sm:text-base lg:text-lg font-medium mb-1 sm:mb-2">Point of Sale</h3>
            <p class="text-xs sm:text-sm opacity-90">Process sales transactions</p>
          </NuxtLink>
          
          <NuxtLink
            to="/customers"
            class="group bg-white border border-gray-200 p-4 sm:p-5 lg:p-6 rounded-lg shadow-sm hover:shadow-md hover:border-purple-300 transition-all duration-200"
          >
            <div class="flex items-center mb-2 sm:mb-3">
              <div class="w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 bg-purple-100 rounded-lg flex items-center justify-center group-hover:bg-purple-200 transition-colors">
                <svg class="w-3 h-3 sm:w-4 sm:h-4 lg:w-5 lg:h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z"></path>
                </svg>
              </div>
            </div>
            <h3 class="text-sm sm:text-base lg:text-lg font-medium text-gray-900 mb-1 sm:mb-2">Customers</h3>
            <p class="text-xs sm:text-sm text-gray-600">Manage customer information and profiles</p>
          </NuxtLink>
          
          <NuxtLink
            to="/reports"
            class="group bg-white border border-gray-200 p-4 sm:p-5 lg:p-6 rounded-lg shadow-sm hover:shadow-md hover:border-indigo-300 transition-all duration-200"
          >
            <div class="flex items-center mb-2 sm:mb-3">
              <div class="w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 bg-indigo-100 rounded-lg flex items-center justify-center group-hover:bg-indigo-200 transition-colors">
                <svg class="w-3 h-3 sm:w-4 sm:h-4 lg:w-5 lg:h-5 text-indigo-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
                </svg>
              </div>
            </div>
            <h3 class="text-sm sm:text-base lg:text-lg font-medium text-gray-900 mb-1 sm:mb-2">Reports</h3>
            <p class="text-xs sm:text-sm text-gray-600">View sales reports and analytics</p>
          </NuxtLink>
          
          <NuxtLink
            to="/stores"
            class="group bg-white border border-gray-200 p-4 sm:p-5 lg:p-6 rounded-lg shadow-sm hover:shadow-md hover:border-orange-300 transition-all duration-200"
          >
            <div class="flex items-center mb-2 sm:mb-3">
              <div class="w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 bg-orange-100 rounded-lg flex items-center justify-center group-hover:bg-orange-200 transition-colors">
                <svg class="w-3 h-3 sm:w-4 sm:h-4 lg:w-5 lg:h-5 text-orange-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"></path>
                </svg>
              </div>
            </div>
            <h3 class="text-sm sm:text-base lg:text-lg font-medium text-gray-900 mb-1 sm:mb-2">Stores</h3>
            <p class="text-xs sm:text-sm text-gray-600">Manage store locations and information</p>
          </NuxtLink>
          
          <NuxtLink
            to="/promotions"
            class="group bg-white border border-gray-200 p-4 sm:p-5 lg:p-6 rounded-lg shadow-sm hover:shadow-md hover:border-pink-300 transition-all duration-200"
          >
            <div class="flex items-center mb-2 sm:mb-3">
              <div class="w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 bg-pink-100 rounded-lg flex items-center justify-center group-hover:bg-pink-200 transition-colors">
                <svg class="w-3 h-3 sm:w-4 sm:h-4 lg:w-5 lg:h-5 text-pink-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z"></path>
                </svg>
              </div>
            </div>
            <h3 class="text-sm sm:text-base lg:text-lg font-medium text-gray-900 mb-1 sm:mb-2">Promotions</h3>
            <p class="text-xs sm:text-sm text-gray-600">Manage discounts and promotional offers</p>
          </NuxtLink>
          
          <NuxtLink
            to="/users"
            class="group bg-white border border-gray-200 p-4 sm:p-5 lg:p-6 rounded-lg shadow-sm hover:shadow-md hover:border-gray-400 transition-all duration-200"
          >
            <div class="flex items-center mb-2 sm:mb-3">
              <div class="w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 bg-gray-100 rounded-lg flex items-center justify-center group-hover:bg-gray-200 transition-colors">
                <svg class="w-3 h-3 sm:w-4 sm:h-4 lg:w-5 lg:h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z"></path>
                </svg>
              </div>
            </div>
            <h3 class="text-sm sm:text-base lg:text-lg font-medium text-gray-900 mb-1 sm:mb-2">Users</h3>
            <p class="text-xs sm:text-sm text-gray-600">Manage system users and permissions</p>
          </NuxtLink>
          
          <NuxtLink
            to="/print"
            class="group bg-white border border-gray-200 p-4 sm:p-5 lg:p-6 rounded-lg shadow-sm hover:shadow-md hover:border-teal-300 transition-all duration-200"
          >
            <div class="flex items-center mb-2 sm:mb-3">
              <div class="w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 bg-teal-100 rounded-lg flex items-center justify-center group-hover:bg-teal-200 transition-colors">
                <svg class="w-3 h-3 sm:w-4 sm:h-4 lg:w-5 lg:h-5 text-teal-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 17h2a2 2 0 002-2v-4a2 2 0 00-2-2H5a2 2 0 00-2 2v4a2 2 0 002 2h2m2 4h6a2 2 0 002-2v-4a2 2 0 00-2-2H9a2 2 0 00-2 2v4a2 2 0 002 2zm8-12V5a2 2 0 00-2-2H9a2 2 0 00-2 2v4h10z"></path>
                </svg>
              </div>
            </div>
            <h3 class="text-sm sm:text-base lg:text-lg font-medium text-gray-900 mb-1 sm:mb-2">Print Receipts</h3>
            <p class="text-xs sm:text-sm text-gray-600">Generate and print transaction receipts</p>
          </NuxtLink>
          
          <NuxtLink
            to="/stock"
            class="group bg-white border border-gray-200 p-4 sm:p-5 lg:p-6 rounded-lg shadow-sm hover:shadow-md hover:border-yellow-300 transition-all duration-200"
          >
            <div class="flex items-center mb-2 sm:mb-3">
              <div class="w-6 h-6 sm:w-7 sm:h-7 lg:w-8 lg:h-8 bg-yellow-100 rounded-lg flex items-center justify-center group-hover:bg-yellow-200 transition-colors">
                <svg class="w-3 h-3 sm:w-4 sm:h-4 lg:w-5 lg:h-5 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 12l3-3 3 3 4-4M8 21l4-4 4 4M3 4h18M4 4h16v12a1 1 0 01-1 1H5a1 1 0 01-1-1V4z"></path>
                </svg>
              </div>
            </div>
            <h3 class="text-sm sm:text-base lg:text-lg font-medium text-gray-900 mb-1 sm:mb-2">Stock Movement</h3>
            <p class="text-xs sm:text-sm text-gray-600">Track and manage inventory stock movements</p>
          </NuxtLink>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  middleware: 'auth'
})

const { user, logout } = useAuth()

const stats = ref({
  products: 0,
  transactions: 0,
  customers: 0
})

const handleLogout = async () => {
  await logout()
}

// Load dashboard stats
onMounted(async () => {
  try {
    const config = useRuntimeConfig()
    const { token } = useAuth()
    
    if (user.value || token.value) {

      console.log(token.value)
      // Fetch actual stats from the API
      const response = await $fetch<{products: number, transactions: number, customers: number}>('/api/dashboard/stats', {
        baseURL: config.public.apiBase,
        headers: {
          'Authorization': `Bearer ${token.value}`
        }
      })
      
      console.log(response)
      
      stats.value = {
        products: response.products,
        transactions: response.transactions,
        customers: response.customers
      }
    }
  } catch (error) {
    console.error('Failed to load dashboard stats:', error)
    // Fallback to default values if API fails
    stats.value = {
      products: 0,
      transactions: 0,
      customers: 0
    }
  }
})
</script>