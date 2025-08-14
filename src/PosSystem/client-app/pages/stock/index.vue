<template>
  <div class="min-h-screen bg-neutral-light">
    <!-- Main Content -->
    <main class="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
      <div class="px-4 py-6 sm:px-0">
        <!-- Header -->
        <div class="mb-8">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-3xl font-bold text-black flex items-center">
                <svg class="h-8 w-8 text-primary mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4"></path>
                </svg>
                Stock Movement
              </h1>
              <p class="text-neutral-gray mt-2">Track and manage inventory stock movements with real-time insights</p>
            </div>
            <div class="hidden md:flex items-center space-x-4">
              <div class="bg-primary-light px-4 py-2 rounded-lg">
                <div class="text-sm text-primary font-medium">Total Movements</div>
                <div class="text-2xl font-bold text-primary-dark">{{ movements.length }}</div>
              </div>
            </div>
          </div>
        </div>

        <!-- Filters Card -->
        <div class="bg-white rounded-xl shadow-sm border border-neutral-medium p-6 mb-6">
          <div class="flex items-center mb-4">
            <svg class="h-5 w-5 text-neutral-gray mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 2v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z"></path>
            </svg>
            <h3 class="text-lg font-semibold text-black">Filters & Search</h3>
          </div>
          
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-4">
            <div>
              <label class="block text-sm font-medium text-neutral-gray mb-2">Start Date</label>
              <input
                v-model="startDate"
                type="date"
                class="w-full px-4 py-2 border border-neutral-medium rounded-lg focus:ring-2 focus:ring-primary focus:border-primary transition-colors"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-neutral-gray mb-2">End Date</label>
              <input
                v-model="endDate"
                type="date"
                class="w-full px-4 py-2 border border-neutral-medium rounded-lg focus:ring-2 focus:ring-primary focus:border-primary transition-colors"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-neutral-gray mb-2">Movement Type</label>
              <select
              v-model="movementTypeFilter"
              @change="loadMovements"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            >
              <option value="">All Types</option>
              <option :value="MOVEMENT_TYPES.IN">Stock In</option>
              <option :value="MOVEMENT_TYPES.OUT">Stock Out</option>
              <option :value="MOVEMENT_TYPES.ADJUSTMENT">Adjustment</option>
              <option :value="MOVEMENT_TYPES.TRANSFER">Transfer</option>
              <option :value="MOVEMENT_TYPES.RETURN">Return</option>
              <option :value="MOVEMENT_TYPES.RESERVE">Reserve</option>
              <option :value="MOVEMENT_TYPES.UNRESERVE">Unreserve</option>
            </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-neutral-gray mb-2">Product ID</label>
              <input
                v-model="productIdFilter"
                type="number"
                placeholder="Enter Product ID"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              />
            </div>
          </div>
          
          <div class="flex flex-wrap gap-3">
            <button
              @click="loadMovements"
              :disabled="loading"
              class="inline-flex items-center px-4 py-2 bg-primary hover:bg-primary-dark text-black font-medium rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <svg class="h-4 w-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 2v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z"></path>
              </svg>
              Apply Filters
            </button>
            <button
              @click="loadSummary"
              :disabled="loading"
              class="inline-flex items-center px-4 py-2 bg-success hover:bg-success-dark text-white font-medium rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <svg class="h-4 w-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
              </svg>
              View Summary
            </button>
            <button
              @click="openRecordModal"
              class="inline-flex items-center px-4 py-2 bg-indigo-600 hover:bg-indigo-700 text-white font-medium rounded-lg transition-colors"
            >
              <svg class="h-4 w-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
              </svg>
              Record Movement
            </button>
          </div>
        </div>

        <!-- Summary Cards -->
        <div v-if="summary" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          <div class="bg-gradient-to-br from-blue-50 to-blue-100 p-6 rounded-xl border border-blue-200 hover:shadow-lg transition-shadow">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm font-medium text-blue-600 mb-1">Total Movements</p>
                <p class="text-3xl font-bold text-blue-700">{{ summary.totalMovements }}</p>
                <p class="text-xs text-blue-500 mt-1">All time records</p>
              </div>
              <div class="bg-blue-600 p-3 rounded-lg">
                <svg class="h-6 w-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
                </svg>
              </div>
            </div>
          </div>
          
          <div v-for="item in summary.summary" :key="item.movementType" class="bg-white p-6 rounded-xl shadow-sm border border-gray-200 hover:shadow-lg transition-shadow">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm font-medium text-gray-600 mb-1">{{ getMovementTypeName(item.movementType) }}</p>
                <p class="text-2xl font-bold text-gray-900">{{ item.totalMovements }}</p>
                <p class="text-xs text-gray-500 mt-1">Qty: {{ item.totalQuantity }}</p>
              </div>
              <div :class="getMovementTypeColor(item.movementType)" class="p-3 rounded-lg">
                <span class="text-white text-lg font-bold">{{ item.movementType.charAt(0) }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Loading State -->
        <div v-if="loading" class="text-center py-8">
          <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
          <p class="mt-2 text-gray-600">Loading stock movements...</p>
        </div>

        <!-- Stock Movements Table -->
        <div v-else class="bg-white shadow-sm rounded-xl border border-gray-200 overflow-hidden">
          <div class="px-6 py-5 border-b border-gray-200 bg-gray-50">
            <div class="flex items-center justify-between">
              <div>
                <h3 class="text-lg font-semibold text-gray-900 flex items-center">
                  <svg class="h-5 w-5 text-gray-500 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"></path>
                  </svg>
                  Stock Movements
                </h3>
                <p class="mt-1 text-sm text-gray-600">
                  {{ movements.length }} movement(s) found
                  <span v-if="startDate && endDate">
                    from {{ formatDate(startDate) }} to {{ formatDate(endDate) }}
                  </span>
                </p>
              </div>
              <div class="flex items-center space-x-2">
                <span class="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                  {{ movements.length }} Records
                </span>
              </div>
            </div>
          </div>
          
          <div class="divide-y divide-gray-200">
            <div v-for="movement in movements" :key="movement.id" class="px-6 py-4 hover:bg-gray-50 transition-colors">
              <div class="flex items-center justify-between">
                <div class="flex items-center space-x-4">
                  <div class="flex-shrink-0">
                    <div :class="getMovementTypeColor(movement.movementType)" class="h-12 w-12 rounded-xl flex items-center justify-center shadow-sm">
                      <span class="text-white text-sm font-bold">{{ movement.movementType.charAt(0) }}</span>
                    </div>
                  </div>
                  <div class="flex-1 min-w-0">
                    <div class="flex items-center space-x-3 mb-1">
                      <p class="text-sm font-semibold text-gray-900 truncate">
                        {{ movement.productName || `Product ID: ${movement.productId}` }}
                      </p>
                      <span :class="getMovementTypeBadgeColor(movement.movementType)" class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium">
                        {{ getMovementTypeName(movement.movementType) }}
                      </span>
                    </div>
                    <div class="flex items-center space-x-4 text-sm text-gray-500">
                      <span class="flex items-center">
                        <svg class="h-4 w-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z"></path>
                        </svg>
                        Qty: {{ movement.quantity > 0 ? '+' : '' }}{{ movement.quantity }}
                      </span>
                      <span class="flex items-center">
                        <svg class="h-4 w-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                        </svg>
                        {{ formatDate(movement.movementDate) }}
                      </span>
                      <span v-if="movement.userName" class="flex items-center">
                        <svg class="h-4 w-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"></path>
                        </svg>
                        {{ movement.userName }}
                      </span>
                    </div>
                    <div v-if="movement.notes" class="mt-2 text-sm text-gray-600 italic bg-gray-50 px-3 py-2 rounded-lg">
                      <svg class="h-4 w-4 inline mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 8h10M7 12h4m1 8l-4-4H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-3l-4 4z"></path>
                      </svg>
                      {{ movement.notes }}
                    </div>
                  </div>
                </div>
                <div class="text-right flex-shrink-0">
                  <div class="text-lg font-bold mb-1" :class="movement.quantity > 0 ? 'text-green-600' : 'text-red-600'">
                    {{ movement.quantity > 0 ? '+' : '' }}{{ movement.quantity }}
                  </div>
                  <div class="text-xs text-gray-500 bg-gray-100 px-2 py-1 rounded">
                    ID: {{ movement.id }}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-if="!loading && movements.length === 0" class="text-center py-12">
          <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
          </svg>
          <h3 class="mt-2 text-sm font-medium text-gray-900">No stock movements found</h3>
          <p class="mt-1 text-sm text-gray-500">No stock movements match your current filters.</p>
        </div>
      </div>
    </main>

    <!-- Record Movement Modal -->
    <div v-if="showRecordModal" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-full max-w-lg shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Record Stock Movement</h3>
          
          <form @submit.prevent="recordMovement" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Product</label>
              <select
                v-model="movementForm.productId"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Select product</option>
                <option v-for="product in products" :key="product.id" :value="product.id">
                  {{ product.productName }} ({{ product.productCode }})
                </option>
              </select>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Movement Type</label>
              <select
                v-model="movementForm.movementType"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
              >
              <option value="">Select Movement Type</option>
              <option :value="MOVEMENT_TYPES.IN">Stock In</option>
              <option :value="MOVEMENT_TYPES.OUT">Stock Out</option>
              <option :value="MOVEMENT_TYPES.ADJUSTMENT">Adjustment</option>
              <option :value="MOVEMENT_TYPES.TRANSFER">Transfer</option>
              <option :value="MOVEMENT_TYPES.RETURN">Return</option>
            </select>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Quantity</label>
              <input
                v-model.number="movementForm.quantity"
                type="number"
                step="0.01"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                placeholder="Enter quantity (positive for in, negative for out)"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Notes</label>
              <textarea
                v-model="movementForm.notes"
                rows="3"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                placeholder="Optional notes about this movement"
              ></textarea>
            </div>
            
            <div class="flex justify-end space-x-3 pt-4">
              <button
                type="button"
                @click="closeRecordModal"
                class="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-200 hover:bg-gray-300 rounded-md"
              >
                Cancel
              </button>
              <button
                type="submit"
                :disabled="recording"
                class="px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-md disabled:opacity-50"
              >
                {{ recording ? 'Recording...' : 'Record Movement' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  middleware: 'auth'
})

// Movement Types Constants (matching MovementTypes.cs)
const MOVEMENT_TYPES = {
  IN: 'IN',
  OUT: 'OUT',
  ADJUSTMENT: 'ADJUSTMENT',
  TRANSFER: 'TRANSFER',
  RETURN: 'RETURN',
  RESERVE: 'RESERVE',
  UNRESERVE: 'UNRESERVE'
} as const

const { showError } = useAlert()

interface StockMovement {
  id: number
  productId: number
  productName?: string
  movementType: string
  quantity: number
  movementDate: string
  notes?: string
  userName?: string
}

interface MovementSummary {
  totalMovements: number
  summary: {
    movementType: string
    totalMovements: number
    totalQuantity: number
  }[]
}

interface Product {
  id: number
  productName: string
  productCode: string
}

interface MovementForm {
  productId: number | null
  movementType: string
  quantity: number | null
  notes: string
}

const { user, logout } = useAuth()
const config = useRuntimeConfig()

// Reactive data
const loading = ref(false)
const recording = ref(false)
const movements = ref<StockMovement[]>([])
const products = ref<Product[]>([])
const summary = ref<MovementSummary | null>(null)
const showRecordModal = ref(false)
const startDate = ref('')
const endDate = ref('')
const movementTypeFilter = ref('')
const productIdFilter = ref<number | null>(null)

const movementForm = ref<MovementForm>({
  productId: null,
  movementType: '',
  quantity: null,
  notes: ''
})

// Methods
const handleLogout = async () => {
  await logout()
}

const loadMovements = async () => {
  loading.value = true
  try {
    const token = useCookie('auth-token')
    const params = new URLSearchParams()
    
    if (startDate.value) params.append('startDate', startDate.value)
    if (endDate.value) params.append('endDate', endDate.value)
    if (movementTypeFilter.value) params.append('movementType', movementTypeFilter.value)
    if (productIdFilter.value) params.append('productId', productIdFilter.value.toString())
    
    const queryString = params.toString()
    const url = `${config.public.apiBase}/api/StockMovement${queryString ? '?' + queryString : ''}`
    
    const response = await $fetch<StockMovement[]>(url, {
      headers: {
        'Authorization': `Bearer ${token.value ?? ''}`
      }
    })
    
    movements.value = response
  } catch (error) {
    console.error('Failed to load stock movements:', error)
  } finally {
    loading.value = false
  }
}

const loadSummary = async () => {
  loading.value = true
  try {
    const token = useCookie('auth-token')
    const params = new URLSearchParams()
    
    if (startDate.value) params.append('startDate', startDate.value)
    if (endDate.value) params.append('endDate', endDate.value)
    
    const queryString = params.toString()
    const url = `${config.public.apiBase}/api/StockMovement/summary${queryString ? '?' + queryString : ''}`
    
    const response = await $fetch<MovementSummary>(url, {
      headers: {
        'Authorization': `Bearer ${token.value ?? ''}`
      }
    })
    
    summary.value = response
  } catch (error) {
    console.error('Failed to load movement summary:', error)
  } finally {
    loading.value = false
  }
}

const loadProducts = async () => {
  try {
    const token = useCookie('auth-token')
    const response = await $fetch<Product[]>(`${config.public.apiBase}/api/Product`, {
      headers: {
        'Authorization': `Bearer ${token.value ?? ''}`
      }
    })
    products.value = response
  } catch (error) {
    console.error('Failed to load products:', error)
  }
}

const openRecordModal = () => {
  movementForm.value = {
    productId: null,
    movementType: '',
    quantity: null,
    notes: ''
  }
  showRecordModal.value = true
}

const closeRecordModal = () => {
  showRecordModal.value = false
}

const recordMovement = async () => {
  if (!movementForm.value.productId || !movementForm.value.movementType || !movementForm.value.quantity) {
    return
  }
  
  recording.value = true
  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/StockMovement`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token.value ?? ''}`,
        'Content-Type': 'application/json'
      },
      body: {
        productId: movementForm.value.productId,
        movementType: movementForm.value.movementType,
        quantity: movementForm.value.quantity,
        notes: movementForm.value.notes
      }
    })
    
    closeRecordModal()
    await loadMovements()
    if (summary.value) {
      await loadSummary()
    }
  } catch (error) {
    console.error('Failed to record movement:', error)
    showError('Failed to record stock movement')
  } finally {
    recording.value = false
  }
}

const getMovementTypeColor = (type: string) => {
  const colors = {
    [MOVEMENT_TYPES.IN]: 'bg-green-600',
    [MOVEMENT_TYPES.OUT]: 'bg-red-600',
    [MOVEMENT_TYPES.ADJUSTMENT]: 'bg-yellow-600',
    [MOVEMENT_TYPES.TRANSFER]: 'bg-blue-600',
    [MOVEMENT_TYPES.RETURN]: 'bg-purple-600'
  }
  return colors[type as keyof typeof colors] || 'bg-gray-600'
}

const getMovementTypeBadgeColor = (type: string) => {
  const colors = {
    [MOVEMENT_TYPES.IN]: 'bg-green-100 text-green-800',
    [MOVEMENT_TYPES.OUT]: 'bg-red-100 text-red-800',
    [MOVEMENT_TYPES.ADJUSTMENT]: 'bg-yellow-100 text-yellow-800',
    [MOVEMENT_TYPES.TRANSFER]: 'bg-blue-100 text-blue-800',
    [MOVEMENT_TYPES.RETURN]: 'bg-purple-100 text-purple-800'
  }
  return colors[type as keyof typeof colors] || 'bg-gray-100 text-gray-800'
}

const getMovementTypeName = (type: string) => {
  const names = {
    [MOVEMENT_TYPES.IN]: 'Stock In',
    [MOVEMENT_TYPES.OUT]: 'Stock Out',
    [MOVEMENT_TYPES.ADJUSTMENT]: 'Adjustment',
    [MOVEMENT_TYPES.TRANSFER]: 'Transfer',
    [MOVEMENT_TYPES.RETURN]: 'Return'
  }
  return names[type as keyof typeof names] || type
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// Set default date range (last 30 days)
const setDefaultDateRange = () => {
  const today = new Date()
  const thirtyDaysAgo = new Date(today.getTime() - (30 * 24 * 60 * 60 * 1000))
  
  endDate.value = today.toISOString().split('T')[0] || ''
  startDate.value = thirtyDaysAgo.toISOString().split('T')[0] || ''
}

// Load data on mount
onMounted(async () => {
  setDefaultDateRange()
  await Promise.all([loadMovements(), loadProducts(), loadSummary()])
})
</script>