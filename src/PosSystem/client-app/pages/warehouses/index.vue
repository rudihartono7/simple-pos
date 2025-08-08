<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <header class="bg-gradient-to-r from-indigo-600 to-indigo-700 text-white shadow-lg">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center py-6">
          <div class="flex items-center space-x-4">
            <Icon name="heroicons:building-storefront" class="h-8 w-8" />
            <div>
              <h1 class="text-3xl font-bold">Warehouses</h1>
              <p class="text-indigo-100 text-lg">Manage warehouse locations and inventory</p>
            </div>
          </div>
          <div class="flex items-center space-x-4">
            <button
              @click="openCreateModal"
              class="bg-white text-indigo-600 px-6 py-3 rounded-lg font-semibold hover:bg-indigo-50 transition-colors flex items-center space-x-2"
            >
              <Icon name="heroicons:plus" class="h-5 w-5" />
              <span>New Warehouse</span>
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
      <!-- Search and Filters -->
      <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-6">
        <div class="flex flex-col md:flex-row md:items-center md:justify-between space-y-4 md:space-y-0">
          <div class="flex-1 max-w-lg">
            <div class="relative">
              <Icon name="heroicons:magnifying-glass" class="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-5 w-5" />
              <input
                v-model="searchQuery"
                @input="searchWarehouses"
                type="text"
                placeholder="Search warehouses..."
                class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-indigo-500 focus:border-indigo-500"
              />
            </div>
          </div>
          <div class="flex items-center space-x-4">
            <label class="flex items-center">
              <input
                v-model="showActiveOnly"
                @change="filterWarehouses"
                type="checkbox"
                class="rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
              />
              <span class="ml-2 text-sm text-gray-700">Active only</span>
            </label>
            <button
              @click="refreshWarehouses"
              class="text-indigo-600 hover:text-indigo-800 p-2 rounded-lg hover:bg-indigo-50"
              title="Refresh"
            >
              <Icon name="heroicons:arrow-path" class="h-5 w-5" />
            </button>
          </div>
        </div>
      </div>

      <!-- Warehouses Grid -->
      <div v-if="loading" class="text-center py-12">
        <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600"></div>
        <p class="mt-2 text-gray-600">Loading warehouses...</p>
      </div>

      <div v-else-if="filteredWarehouses.length === 0" class="text-center py-12">
        <Icon name="heroicons:building-storefront" class="h-12 w-12 text-gray-400 mx-auto mb-4" />
        <h3 class="text-lg font-medium text-gray-900 mb-2">No warehouses found</h3>
        <p class="text-gray-500 mb-4">
          {{ searchQuery ? 'Try adjusting your search criteria.' : 'Create your first warehouse to get started.' }}
        </p>
        <button
          v-if="!searchQuery"
          @click="openCreateModal"
          class="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 transition-colors"
        >
          Create Warehouse
        </button>
      </div>

      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="warehouse in filteredWarehouses"
          :key="warehouse.id"
          class="bg-white rounded-lg shadow-sm border border-gray-200 hover:shadow-md transition-shadow"
        >
          <div class="p-6">
            <div class="flex items-start justify-between mb-4">
              <div class="flex-1">
                <h3 class="text-lg font-semibold text-gray-900 mb-1">{{ warehouse.warehouseName }}</h3>
                <p class="text-sm text-gray-600 mb-2">Code: {{ warehouse.warehouseCode }}</p>
                <div class="flex items-center space-x-2">
                  <span
                    :class="warehouse.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'"
                    class="px-2 py-1 text-xs font-medium rounded-full"
                  >
                    {{ warehouse.isActive ? 'Active' : 'Inactive' }}
                  </span>
                </div>
              </div>
              <div class="flex items-center space-x-1">
                <button
                  @click="viewWarehouseStock(warehouse)"
                  class="text-indigo-600 hover:text-indigo-800 p-2 rounded-lg hover:bg-indigo-50"
                  title="View Stock"
                >
                  <Icon name="heroicons:cube" class="h-4 w-4" />
                </button>
                <button
                  @click="editWarehouse(warehouse)"
                  class="text-gray-600 hover:text-gray-800 p-2 rounded-lg hover:bg-gray-50"
                  title="Edit"
                >
                  <Icon name="heroicons:pencil" class="h-4 w-4" />
                </button>
                <button
                  @click="deleteWarehouse(warehouse.id)"
                  class="text-red-600 hover:text-red-800 p-2 rounded-lg hover:bg-red-50"
                  title="Delete"
                >
                  <Icon name="heroicons:trash" class="h-4 w-4" />
                </button>
              </div>
            </div>
            
            <div class="space-y-2 text-sm text-gray-600">
              <div v-if="warehouse.address">
                <Icon name="heroicons:map-pin" class="inline h-4 w-4 mr-1" />
                {{ warehouse.address }}
              </div>
              <div v-if="warehouse.phone">
                <Icon name="heroicons:phone" class="inline h-4 w-4 mr-1" />
                {{ warehouse.phone }}
              </div>
              <div v-if="warehouse.email">
                <Icon name="heroicons:envelope" class="inline h-4 w-4 mr-1" />
                {{ warehouse.email }}
              </div>
            </div>

            <div v-if="warehouse.description" class="mt-3 text-sm text-gray-600">
              {{ warehouse.description }}
            </div>

            <!-- Stock Summary -->
            <div class="mt-4 pt-4 border-t border-gray-200">
              <div class="flex items-center justify-between text-sm">
                <span class="text-gray-600">Total Stock Items:</span>
                <span class="font-medium text-gray-900">{{ warehouse.stockCount || 0 }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Create/Edit Modal -->
    <div v-if="showModal" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-full max-w-2xl shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 mb-4">
            {{ isEditing ? 'Edit Warehouse' : 'Create New Warehouse' }}
          </h3>
          
          <form @submit.prevent="saveWarehouse" class="space-y-4">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Warehouse Name *</label>
                <input
                  v-model="form.warehouseName"
                  type="text"
                  required
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-indigo-500 focus:border-indigo-500"
                  placeholder="Enter warehouse name"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Warehouse Code *</label>
                <input
                  v-model="form.warehouseCode"
                  type="text"
                  required
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-indigo-500 focus:border-indigo-500"
                  placeholder="Enter warehouse code"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Address</label>
              <textarea
                v-model="form.address"
                rows="3"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-indigo-500 focus:border-indigo-500"
                placeholder="Enter warehouse address"
              ></textarea>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Phone</label>
                <input
                  v-model="form.phone"
                  type="tel"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-indigo-500 focus:border-indigo-500"
                  placeholder="Enter phone number"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Email</label>
                <input
                  v-model="form.email"
                  type="email"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-indigo-500 focus:border-indigo-500"
                  placeholder="Enter email address"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Description</label>
              <textarea
                v-model="form.description"
                rows="3"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-indigo-500 focus:border-indigo-500"
                placeholder="Enter warehouse description"
              ></textarea>
            </div>

            <div class="flex items-center">
              <input
                v-model="form.isActive"
                type="checkbox"
                class="rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
              />
              <label class="ml-2 text-sm text-gray-700">Active</label>
            </div>
            
            <div class="flex justify-end space-x-3 pt-4 border-t">
              <button
                type="button"
                @click="closeModal"
                class="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-200 hover:bg-gray-300 rounded-md"
              >
                Cancel
              </button>
              <button
                type="submit"
                :disabled="saving"
                class="px-4 py-2 text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 rounded-md disabled:opacity-50"
              >
                {{ saving ? 'Saving...' : (isEditing ? 'Update' : 'Create') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Stock View Modal -->
    <div v-if="showStockModal" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-full max-w-6xl shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-6">
            <h3 class="text-lg font-medium text-gray-900">
              Stock Levels - {{ selectedWarehouse?.warehouseName }}
            </h3>
            <button @click="closeStockModal" class="text-gray-400 hover:text-gray-600">
              <Icon name="heroicons:x-mark" class="h-6 w-6" />
            </button>
          </div>
          
          <div v-if="loadingStock" class="text-center py-8">
            <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600"></div>
            <p class="mt-2 text-gray-600">Loading stock levels...</p>
          </div>

          <div v-else-if="warehouseStock.length === 0" class="text-center py-8">
            <Icon name="heroicons:cube" class="h-12 w-12 text-gray-400 mx-auto mb-4" />
            <h3 class="text-lg font-medium text-gray-900 mb-2">No stock found</h3>
            <p class="text-gray-500">This warehouse doesn't have any stock items yet.</p>
          </div>

          <div v-else>
            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-gray-200">
                <thead class="bg-gray-50">
                  <tr>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Product</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Code</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Quantity</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Min Level</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Max Level</th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                  </tr>
                </thead>
                <tbody class="bg-white divide-y divide-gray-200">
                  <tr v-for="stock in warehouseStock" :key="stock.id">
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {{ stock.product?.productName || 'N/A' }}
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                      {{ stock.product?.productCode || 'N/A' }}
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 font-medium">
                      {{ stock.quantity }}
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                      {{ stock.minLevel || 'N/A' }}
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                      {{ stock.maxLevel || 'N/A' }}
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap">
                      <span :class="getStockStatusClass(stock)" class="px-2 py-1 text-xs font-medium rounded-full">
                        {{ getStockStatus(stock) }}
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  middleware: 'auth'
})

interface Product {
  id: number
  productName: string
  productCode: string
}

interface WarehouseStock {
  id: number
  warehouseId: number
  productId: number
  product?: Product
  quantity: number
  minLevel?: number
  maxLevel?: number
}

interface Warehouse {
  id: number
  warehouseName: string
  warehouseCode: string
  address?: string
  phone?: string
  email?: string
  description?: string
  isActive: boolean
  stockCount?: number
}

const { user } = useAuth()
const config = useRuntimeConfig()

// Reactive data
const loading = ref(false)
const saving = ref(false)
const loadingStock = ref(false)
const warehouses = ref<Warehouse[]>([])
const filteredWarehouses = ref<Warehouse[]>([])
const warehouseStock = ref<WarehouseStock[]>([])
const showModal = ref(false)
const showStockModal = ref(false)
const isEditing = ref(false)
const selectedWarehouse = ref<Warehouse | null>(null)

// Search and filters
const searchQuery = ref('')
const showActiveOnly = ref(true)

// Form data
const form = ref({
  id: 0,
  warehouseName: '',
  warehouseCode: '',
  address: '',
  phone: '',
  email: '',
  description: '',
  isActive: true
})

// Methods
const loadWarehouses = async () => {
  loading.value = true
  try {
    const token = useCookie('auth-token')
    const response = await $fetch<Warehouse[]>(`${config.public.apiBase}/api/Warehouse`, {
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    warehouses.value = response
    filterWarehouses()
  } catch (error) {
    console.error('Failed to load warehouses:', error)
  } finally {
    loading.value = false
  }
}

const searchWarehouses = () => {
  filterWarehouses()
}

const filterWarehouses = () => {
  let filtered = warehouses.value

  if (showActiveOnly.value) {
    filtered = filtered.filter(w => w.isActive)
  }

  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(w =>
      w.warehouseName.toLowerCase().includes(query) ||
      w.warehouseCode.toLowerCase().includes(query) ||
      (w.address && w.address.toLowerCase().includes(query))
    )
  }

  filteredWarehouses.value = filtered
}

const refreshWarehouses = () => {
  loadWarehouses()
}

const openCreateModal = () => {
  isEditing.value = false
  form.value = {
    id: 0,
    warehouseName: '',
    warehouseCode: '',
    address: '',
    phone: '',
    email: '',
    description: '',
    isActive: true
  }
  showModal.value = true
}

const editWarehouse = (warehouse: Warehouse) => {
  isEditing.value = true
  form.value = {
    id: warehouse.id,
    warehouseName: warehouse.warehouseName,
    warehouseCode: warehouse.warehouseCode,
    address: warehouse.address || '',
    phone: warehouse.phone || '',
    email: warehouse.email || '',
    description: warehouse.description || '',
    isActive: warehouse.isActive
  }
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
}

const saveWarehouse = async () => {
  if (!form.value.warehouseName.trim() || !form.value.warehouseCode.trim()) {
    alert('Please fill in all required fields')
    return
  }

  saving.value = true
  try {
    const token = useCookie('auth-token')
    const payload = {
      warehouseName: form.value.warehouseName.trim(),
      warehouseCode: form.value.warehouseCode.trim(),
      address: form.value.address.trim() || null,
      phone: form.value.phone.trim() || null,
      email: form.value.email.trim() || null,
      description: form.value.description.trim() || null,
      isActive: form.value.isActive
    }

    if (isEditing.value) {
      await $fetch(`${config.public.apiBase}/api/Warehouse/${form.value.id}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token.value || ''}`,
          'Content-Type': 'application/json'
        },
        body: { ...payload, id: form.value.id }
      })
    } else {
      await $fetch(`${config.public.apiBase}/api/Warehouse`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token.value || ''}`,
          'Content-Type': 'application/json'
        },
        body: payload
      })
    }

    closeModal()
    await loadWarehouses()
  } catch (error) {
    console.error('Failed to save warehouse:', error)
    alert('Failed to save warehouse')
  } finally {
    saving.value = false
  }
}

const deleteWarehouse = async (id: number) => {
  if (!confirm('Are you sure you want to delete this warehouse? This action cannot be undone.')) return

  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/Warehouse/${id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    await loadWarehouses()
  } catch (error) {
    console.error('Failed to delete warehouse:', error)
    alert('Failed to delete warehouse. It may have existing stock or transactions.')
  }
}

const viewWarehouseStock = async (warehouse: Warehouse) => {
  selectedWarehouse.value = warehouse
  showStockModal.value = true
  loadingStock.value = true

  try {
    const token = useCookie('auth-token')
    const response = await $fetch<WarehouseStock[]>(`${config.public.apiBase}/api/Warehouse/${warehouse.id}/stock`, {
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    warehouseStock.value = response
  } catch (error) {
    console.error('Failed to load warehouse stock:', error)
    warehouseStock.value = []
  } finally {
    loadingStock.value = false
  }
}

const closeStockModal = () => {
  showStockModal.value = false
  selectedWarehouse.value = null
  warehouseStock.value = []
}

const getStockStatus = (stock: WarehouseStock) => {
  if (stock.minLevel && stock.quantity <= stock.minLevel) {
    return 'Low Stock'
  }
  if (stock.maxLevel && stock.quantity >= stock.maxLevel) {
    return 'Overstock'
  }
  return 'Normal'
}

const getStockStatusClass = (stock: WarehouseStock) => {
  const status = getStockStatus(stock)
  const classes = {
    'Low Stock': 'bg-red-100 text-red-800',
    'Overstock': 'bg-yellow-100 text-yellow-800',
    'Normal': 'bg-green-100 text-green-800'
  }
  return classes[status as keyof typeof classes]
}

// Watch for search and filter changes
watch([searchQuery, showActiveOnly], () => {
  filterWarehouses()
})

// Load data on mount
onMounted(() => {
  loadWarehouses()
})
</script>