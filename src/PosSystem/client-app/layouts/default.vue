<template>
  <div class="min-h-screen bg-neutral-light flex">
    <!-- Sidebar -->
    <div class="fixed inset-y-0 left-0 z-50 w-64 bg-white transform transition-transform duration-300 ease-in-out lg:translate-x-0 lg:static lg:inset-0" :class="sidebarOpen ? 'translate-x-0' : '-translate-x-full'">
      <div class="flex items-center justify-center h-16 px-4 bg-neutral-white">
        <div class="flex items-center space-x-2">
          <div class="w-8 h-8 lg:w-10 lg:h-10 bg-white shadow rounded-lg flex items-center justify-center">
            <!-- add image as logo from public/images/logo.png -->
            <img src="/images/logo.png" alt="Logo" class="w-8 h-6 lg:w-10 lg:h-8">
            <!-- <svg class="w-5 h-5 text-white" fill="currentColor" viewBox="0 0 20 20">
              <path d="M3 4a1 1 0 011-1h12a1 1 0 011 1v2a1 1 0 01-1 1H4a1 1 0 01-1-1V4zM3 10a1 1 0 011-1h6a1 1 0 011 1v6a1 1 0 01-1 1H4a1 1 0 01-1-1v-6zM14 9a1 1 0 00-1 1v6a1 1 0 001 1h2a1 1 0 001-1v-6a1 1 0 00-1-1h-2z"/>
            </svg> -->
          </div>
          <span class="text-black font-semibold text-base lg:text-lg">POS System</span>
        </div>
      </div>
      
      <nav class="mt-4 lg:mt-8 px-3 lg:px-4 overflow-y-auto max-h-[calc(100vh-8rem)]">
        <div class="space-y-1 lg:space-y-2">
          <NuxtLink
            to="/dashboard"
            class="flex items-center space-x-2 lg:space-x-3 px-2 lg:px-3 py-2 rounded-lg text-black hover:text-black hover:bg-neutral-gray transition-colors duration-200"
            :class="{ 'bg-primary text-black shadow-lg': $route.path === '/dashboard' }"
          >
            <svg class="w-4 h-4 lg:w-5 lg:h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2H5a2 2 0 00-2-2z"/>
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 5a2 2 0 012-2h4a2 2 0 012 2v6H8V5z"/>
            </svg>
            <span class="text-sm lg:text-base">Dashboard</span>
          </NuxtLink>
          
          <NuxtLink
            to="/pos"
            class="flex items-center space-x-2 lg:space-x-3 px-2 lg:px-3 py-2 rounded-lg text-black hover:text-black hover:bg-neutral-gray transition-colors duration-200"
            :class="{ 'bg-primary text-black shadow-lg': $route.path === '/pos' }"
          >
            <svg class="w-4 h-4 lg:w-5 lg:h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z"/>
            </svg>
            <span class="text-sm lg:text-base">POS Terminal</span>
          </NuxtLink>
          
          <NuxtLink
            to="/products"
            class="flex items-center space-x-2 lg:space-x-3 px-2 lg:px-3 py-2 rounded-lg text-black hover:text-black hover:bg-neutral-gray transition-colors duration-200"
            :class="{ 'bg-primary text-black shadow-lg': $route.path === '/products' }"
          >
            <svg class="w-4 h-4 lg:w-5 lg:h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4"/>
            </svg>
            <span class="text-sm lg:text-base">Products</span>
          </NuxtLink>
          
          <NuxtLink
            to="/categories"
            class="flex items-center space-x-2 lg:space-x-3 px-2 lg:px-3 py-2 rounded-lg text-black hover:text-black hover:bg-neutral-gray transition-colors duration-200"
            :class="{ 'bg-primary text-black shadow-lg': $route.path === '/categories' }"
          >
            <svg class="w-4 h-4 lg:w-5 lg:h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"/>
            </svg>
            <span class="text-sm lg:text-base">Categories</span>
          </NuxtLink>
          
          <NuxtLink
            to="/customers"
            class="flex items-center space-x-2 lg:space-x-3 px-2 lg:px-3 py-2 rounded-lg text-black hover:text-black hover:bg-neutral-gray transition-colors duration-200"
            :class="{ 'bg-primary text-black shadow-lg': $route.path === '/customers' }"
          >
            <svg class="w-4 h-4 lg:w-5 lg:h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5 0a4 4 0 11-8 0 4 4 0 018 0z"/>
            </svg>
            <span class="text-sm lg:text-base">Customers</span>
          </NuxtLink>
          
          <NuxtLink
            to="/users"
            class="flex items-center space-x-2 lg:space-x-3 px-2 lg:px-3 py-2 rounded-lg text-black hover:text-black hover:bg-neutral-gray transition-colors duration-200"
            :class="{ 'bg-primary text-black shadow-lg': $route.path === '/users' }"
          >
            <svg class="w-4 h-4 lg:w-5 lg:h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"/>
            </svg>
            <span class="text-sm lg:text-base">Users</span>
          </NuxtLink>
          
          <NuxtLink
            to="/stores"
            class="flex items-center space-x-2 lg:space-x-3 px-2 lg:px-3 py-2 rounded-lg text-black hover:text-black hover:bg-neutral-gray transition-colors duration-200"
            :class="{ 'bg-primary text-black shadow-lg': $route.path === '/stores' }"
          >
            <svg class="w-4 h-4 lg:w-5 lg:h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"/>
            </svg>
            <span class="text-sm lg:text-base">Stores</span>
          </NuxtLink>
          
          <!-- Inventory Management Section -->
          <div class="mt-4 lg:mt-6 mb-2">
            <h3 class="px-2 lg:px-3 text-xs font-semibold text-neutral-medium uppercase tracking-wider">Inventory</h3>
          </div>
          
          <NuxtLink
            to="/stock"
            class="flex items-center space-x-2 lg:space-x-3 px-2 lg:px-3 py-2 rounded-lg text-black hover:text-black hover:bg-neutral-gray transition-colors duration-200"
            :class="{ 'bg-primary text-black shadow-lg': $route.path === '/stock' }"
          >
            <svg class="w-4 h-4 lg:w-5 lg:h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
            </svg>
            <span class="text-sm lg:text-base">Stock Movement</span>
          </NuxtLink>
          
          <NuxtLink
            to="/purchase-orders"
            class="flex items-center space-x-2 lg:space-x-3 px-2 lg:px-3 py-2 rounded-lg text-black hover:text-black hover:bg-neutral-gray transition-colors duration-200"
            :class="{ 'bg-primary text-black shadow-lg': $route.path === '/purchase-orders' }"
          >
            <svg class="w-4 h-4 lg:w-5 lg:h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
            </svg>
            <span class="text-sm lg:text-base">Purchase Orders</span>
          </NuxtLink>
          
          <NuxtLink
            to="/stock-receiving"
            class="flex items-center space-x-2 lg:space-x-3 px-2 lg:px-3 py-2 rounded-lg text-black hover:text-black hover:bg-neutral-gray transition-colors duration-200"
            :class="{ 'bg-primary text-black shadow-lg': $route.path === '/stock-receiving' }"
          >
            <svg class="w-4 h-4 lg:w-5 lg:h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4"/>
            </svg>
            <span class="text-sm lg:text-base">Stock Receiving</span>
          </NuxtLink>
          
          <NuxtLink
            to="/stock-transfers"
            class="flex items-center space-x-2 lg:space-x-3 px-2 lg:px-3 py-2 rounded-lg text-black hover:text-black hover:bg-neutral-gray transition-colors duration-200"
            :class="{ 'bg-primary text-black shadow-lg': $route.path === '/stock-transfers' }"
          >
            <svg class="w-4 h-4 lg:w-5 lg:h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4"/>
            </svg>
            <span class="text-sm lg:text-base">Stock Transfers</span>
          </NuxtLink>
          
          <NuxtLink
            to="/warehouses"
            class="flex items-center space-x-2 lg:space-x-3 px-2 lg:px-3 py-2 rounded-lg text-black hover:text-black hover:bg-neutral-gray transition-colors duration-200"
            :class="{ 'bg-primary text-black shadow-lg': $route.path === '/warehouses' }"
          >
            <svg class="w-4 h-4 lg:w-5 lg:h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"/>
            </svg>
            <span class="text-sm lg:text-base">Warehouses</span>
          </NuxtLink>
          
          <NuxtLink
            to="/suppliers"
            class="flex items-center space-x-2 lg:space-x-3 px-2 lg:px-3 py-2 rounded-lg text-black hover:text-black hover:bg-neutral-gray transition-colors duration-200"
            :class="{ 'bg-primary text-black shadow-lg': $route.path === '/suppliers' }"
          >
            <svg class="w-4 h-4 lg:w-5 lg:h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 14v3m4-3v3m4-3v3M3 21h18M3 10h18M3 7l9-4 9 4M4 10h16v11H4V10z"/>
            </svg>
            <span class="text-sm lg:text-base">Suppliers</span>
          </NuxtLink>
          
          <!-- Other Sections -->
          <div class="mt-4 lg:mt-6 mb-2">
            <h3 class="px-2 lg:px-3 text-xs font-semibold text-neutral-medium uppercase tracking-wider">Other</h3>
          </div>
          
          <NuxtLink
            to="/promotions"
            class="flex items-center space-x-2 lg:space-x-3 px-2 lg:px-3 py-2 rounded-lg text-black hover:text-black hover:bg-neutral-gray transition-colors duration-200"
            :class="{ 'bg-primary text-black shadow-lg': $route.path === '/promotions' }"
          >
            <svg class="w-4 h-4 lg:w-5 lg:h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z"/>
            </svg>
            <span class="text-sm lg:text-base">Promotions</span>
          </NuxtLink>
          
          <NuxtLink
            to="/reports"
            class="flex items-center space-x-2 lg:space-x-3 px-2 lg:px-3 py-2 rounded-lg text-black hover:text-black hover:bg-neutral-gray transition-colors duration-200"
            :class="{ 'bg-primary text-black shadow-lg': $route.path === '/reports' }"
          >
            <svg class="w-4 h-4 lg:w-5 lg:h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
            </svg>
            <span class="text-sm lg:text-base">Reports</span>
          </NuxtLink>
        </div>
      </nav>
    </div>

    <!-- Main Content -->
    <div class="flex-1 lg:ml-0">
      <!-- Top Header -->
      <header class="bg-white shadow-sm border-b border-neutral-medium">
        <div class="flex items-center justify-between px-4 lg:px-6 py-3 lg:py-4">
          <div class="flex items-center space-x-2 lg:space-x-4 min-w-0 flex-1">
            <button
              @click="sidebarOpen = !sidebarOpen"
              class="lg:hidden p-2 rounded-md text-neutral-gray hover:text-black hover:bg-neutral-light flex-shrink-0"
            >
              <svg class="w-5 h-5 lg:w-6 lg:h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"/>
              </svg>
            </button>
            <div class="min-w-0 flex-1">
              <h1 class="text-lg lg:text-xl font-semibold text-black truncate">{{ pageTitle }}</h1>
              <p v-if="pageSubtitle" class="text-xs lg:text-sm text-neutral-gray truncate hidden sm:block">{{ pageSubtitle }}</p>
            </div>
          </div>
          
          <div class="flex items-center space-x-2 lg:space-x-4 flex-shrink-0">
            <!-- User Menu -->
            <div class="relative" v-if="user">
              <button
                @click="userMenuOpen = !userMenuOpen"
                class="flex items-center space-x-2 lg:space-x-3 p-1 lg:p-2 rounded-lg hover:bg-neutral-light"
              >
                <div class="w-7 h-7 lg:w-8 lg:h-8 bg-neutral-medium rounded-full flex items-center justify-center flex-shrink-0">
                  <span class="text-xs lg:text-sm font-medium text-neutral-gray">
                    {{ user.firstName?.charAt(0) }}{{ user.lastName?.charAt(0) }}
                  </span>
                </div>
                <div class="text-left hidden sm:block">
                  <p class="text-sm font-medium text-black truncate max-w-[120px] lg:max-w-none">{{ user.firstName }} {{ user.lastName }}</p>
                  <p class="text-xs text-neutral-gray truncate">{{ user.role }}</p>
                </div>
                <svg class="w-3 h-3 lg:w-4 lg:h-4 text-neutral-gray hidden sm:block" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
                </svg>
              </button>
              
              <!-- User Dropdown -->
              <div
                v-if="userMenuOpen"
                class="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg py-1 z-50"
                @click.away="userMenuOpen = false"
              >
                <a href="#" class="block px-4 py-2 text-sm text-black hover:bg-neutral-light">Profile</a>
                <a href="#" class="block px-4 py-2 text-sm text-black hover:bg-neutral-light">Settings</a>
                <hr class="my-1">
                <button
                  @click="handleLogout"
                  class="block w-full text-left px-4 py-2 text-sm text-black hover:bg-neutral-light"
                >
                  Sign out
                </button>
              </div>
            </div>
          </div>
        </div>
      </header>

      <!-- Page Content -->
      <main class="flex-1 p-4 lg:p-6">
        <slot />
      </main>
    </div>

    <!-- Mobile sidebar overlay -->
    <div
      v-if="sidebarOpen"
      class="fixed inset-0 z-40 lg:hidden"
      @click="sidebarOpen = false"
    >
      <div class="absolute inset-0 bg-neutral-dark opacity-75"></div>
    </div>
    
    <!-- Custom Alert Component -->
    <CustomAlert 
      :is-visible="alertState.isVisible"
      :type="alertState.type"
      :title="alertState.title"
      :message="alertState.message"
      :confirm-text="alertState.confirmText"
      :cancel-text="alertState.cancelText"
      :show-cancel="alertState.showCancel"
      @confirm="alertState.onConfirm"
      @cancel="alertState.onCancel"
    />
  </div>
</template>

<script setup lang="ts">
const { user, logout } = useAuth()
const route = useRoute()
const { alertState } = useAlert()

const sidebarOpen = ref(false)
const userMenuOpen = ref(false)

const pageTitle = computed(() => {
  const titles: Record<string, string> = {
    '/dashboard': 'Dashboard',
    '/pos': 'POS Terminal',
    '/products': 'Products',
    '/categories': 'Categories',
    '/customers': 'Customers',
    '/users': 'User Management',
    '/stores': 'Stores',
    '/stock': 'Stock Movement',
    '/purchase-orders': 'Purchase Orders',
    '/stock-receiving': 'Stock Receiving',
    '/stock-transfers': 'Stock Transfers',
    '/warehouses': 'Warehouses',
    '/suppliers': 'Suppliers',
    '/promotions': 'Promotions',
    '/reports': 'Reports'
  }
  return titles[route.path] || 'POS System'
})

const pageSubtitle = computed(() => {
  const subtitles: Record<string, string> = {
    '/dashboard': 'Overview of your business performance',
    '/pos': 'Process sales and transactions',
    '/products': 'Manage your product catalog',
    '/categories': 'Organize your product categories',
    '/customers': 'Manage customer information',
    '/users': 'Manage system users and permissions',
    '/stores': 'Manage store locations',
    '/stock': 'Monitor and track inventory movements',
    '/purchase-orders': 'Manage purchase orders and procurement',
    '/stock-receiving': 'Process incoming stock deliveries',
    '/stock-transfers': 'Transfer stock between warehouses',
    '/warehouses': 'Manage warehouse locations and inventory',
    '/suppliers': 'Manage supplier information and contacts',
    '/promotions': 'Create and manage promotional offers',
    '/reports': 'View business analytics and reports'
  }
  return subtitles[route.path] || ''
})

const handleLogout = async () => {
  await logout()
}

// Close dropdowns when clicking outside
onMounted(() => {
  document.addEventListener('click', (e) => {
    const target = e.target as Element
    if (!target?.closest('.relative')) {
      userMenuOpen.value = false
    }
  })
})
</script>

<style scoped>
/* Navigation styles are now inline for better specificity */
</style>