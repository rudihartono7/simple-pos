<template>
  <div class="p-8">
    <h1 class="text-2xl font-bold mb-4">Authentication Test Page</h1>
    
    <div class="space-y-4">
      <div class="bg-gray-100 p-4 rounded">
        <h2 class="font-semibold">Auth State:</h2>
        <p>Is Authenticated: {{ isAuthenticated }}</p>
        <p>Is Loading: {{ isLoading }}</p>
        <p>Is Initialized: {{ isInitialized }}</p>
        <p>Has Token: {{ !!token }}</p>
        <p>Has User: {{ !!user }}</p>
      </div>
      
      <div class="bg-gray-100 p-4 rounded">
        <h2 class="font-semibold">User Info:</h2>
        <pre>{{ JSON.stringify(user, null, 2) }}</pre>
      </div>
      
      <div class="space-x-4">
        <button @click="testLogin" class="bg-blue-500 text-white px-4 py-2 rounded">
          Test Login
        </button>
        <button @click="testLogout" class="bg-red-500 text-white px-4 py-2 rounded">
          Test Logout
        </button>
        <button @click="refreshAuth" class="bg-green-500 text-white px-4 py-2 rounded">
          Refresh Auth
        </button>
      </div>
      
      <div class="space-x-4">
        <NuxtLink to="/login" class="bg-gray-500 text-white px-4 py-2 rounded inline-block">
          Go to Login
        </NuxtLink>
        <NuxtLink to="/dashboard" class="bg-blue-500 text-white px-4 py-2 rounded inline-block">
          Go to Dashboard
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const { user, token, isAuthenticated, isLoading, isInitialized, login, logout, getCurrentUser } = useAuth()

const testLogin = async () => {
  const success = await login({
    email: 'admin@possystem.com',
    password: 'Admin123!'
  })
  console.log('Test login result:', success)
}

const testLogout = async () => {
  await logout()
}

const refreshAuth = async () => {
  await getCurrentUser()
}
</script>