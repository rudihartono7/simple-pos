<template>
  <div class="min-h-screen bg-neutral-light">
    <!-- Header -->
    <header class="bg-gradient-to-r from-primary to-primary-dark text-black shadow-lg">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center py-6">
          <div class="flex items-center space-x-4">
            <Icon name="heroicons:arrow-path" class="h-8 w-8" />
            <div>
              <h1 class="text-3xl font-bold">Stock Transfers</h1>
              <p class="text-primary-light text-lg">Manage inventory transfers between warehouses</p>
            </div>
          </div>
          <div class="flex items-center space-x-4">
            <button
              @click="openCreateModal"
              class="bg-white text-primary px-6 py-3 rounded-lg font-semibold hover:bg-primary-light transition-colors flex items-center space-x-2"
            >
              <Icon name="heroicons:plus" class="h-5 w-5" />
              <span>New Stock Transfer</span>
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
      <!-- Filters -->
      <div class="bg-white rounded-lg shadow-sm border border-neutral-medium p-6 mb-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label class="block text-sm font-medium text-neutral-gray mb-2">Status</label>
            <select
              v-model="statusFilter"
              @change="loadStockTransfers"
              class="w-full px-3 py-2 border border-neutral-medium rounded-md focus:ring-primary focus:border-primary"
            >
              <option value="">All Status</option>
              <option value="PENDING">Pending</option>
              <option value="APPROVED">Approved</option>
              <option value="SHIPPED">Shipped</option>
              <option value="RECEIVED">Received</option>
              <option value="COMPLETED">Completed</option>
              <option value="CANCELLED">Cancelled</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-neutral-gray mb-2">From Warehouse</label>
            <select
              v-model="fromWarehouseFilter"
              @change="loadStockTransfers"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-purple-500 focus:border-purple-500"
            >
              <option value="">All Warehouses</option>
              <option v-for="warehouse in warehouses" :key="warehouse.id" :value="warehouse.id">
                {{ warehouse.warehouseName }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-neutral-gray mb-2">To Warehouse</label>
            <select
              v-model="toWarehouseFilter"
              @change="loadStockTransfers"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-purple-500 focus:border-purple-500"
            >
              <option value="">All Warehouses</option>
              <option v-for="warehouse in warehouses" :key="warehouse.id" :value="warehouse.id">
                {{ warehouse.warehouseName }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-neutral-gray mb-2">Date Range</label>
            <input
              v-model="dateFilter"
              type="date"
              @change="loadStockTransfers"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-purple-500 focus:border-purple-500"
            />
          </div>
        </div>
      </div>

      <!-- Stock Transfers List -->
      <div class="bg-white shadow-sm rounded-lg border border-neutral-medium overflow-hidden">
        <div class="px-6 py-4 border-b border-neutral-medium bg-neutral-light">
          <h3 class="text-lg font-semibold text-black">Stock Transfers ({{ stockTransfers.length }})</h3>
        </div>
        
        <div v-if="loading" class="p-8 text-center">
          <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
          <p class="mt-2 text-neutral-gray">Loading stock transfers...</p>
        </div>

        <div v-else-if="stockTransfers.length === 0" class="p-8 text-center">
          <Icon name="heroicons:arrow-path" class="h-12 w-12 text-neutral-gray mx-auto mb-4" />
          <h3 class="text-lg font-medium text-black mb-2">No stock transfers found</h3>
          <p class="text-neutral-gray">Create your first stock transfer to get started.</p>
        </div>

        <div v-else class="divide-y divide-neutral-medium">
          <div v-for="st in stockTransfers" :key="st.id" class="p-6 hover:bg-neutral-light transition-colors">
            <div class="flex items-center justify-between">
              <div class="flex-1">
                <div class="flex items-center space-x-4 mb-2">
                  <h4 class="text-lg font-semibold text-black">{{ st.transferNumber }}</h4>
                  <span :class="getStatusBadgeClass(st.status)" class="px-2 py-1 text-xs font-medium rounded-full">
                    {{ st.status }}
                  </span>
                </div>
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4 text-sm text-neutral-gray">
                  <div>
                    <span class="font-medium">From:</span> {{ st.fromWarehouse?.warehouseName || 'N/A' }}
                  </div>
                  <div>
                    <span class="font-medium">To:</span> {{ st.toWarehouse?.warehouseName || 'N/A' }}
                  </div>
                  <div>
                    <span class="font-medium">Transfer Date:</span> {{ formatDate(st.transferDate) }}
                  </div>
                </div>
                <div class="mt-2 text-sm text-neutral-gray">
                  <span class="font-medium">Total Items:</span> {{ st.items?.length || 0 }}
                </div>
              </div>
              <div class="flex items-center space-x-2">
                <button
                  @click="viewStockTransfer(st)"
                  class="text-primary hover:text-primary-dark p-2 rounded-lg hover:bg-primary-light"
                  title="View Details"
                >
                  <Icon name="heroicons:eye" class="h-5 w-5" />
                </button>
                <button
                  v-if="st.status === 'PENDING'"
                  @click="approveStockTransfer(st.id)"
                  class="text-success hover:text-success-dark p-2 rounded-lg hover:bg-success-light"
                  title="Approve"
                >
                  <Icon name="heroicons:check" class="h-5 w-5" />
                </button>
                <button
                  v-if="st.status === 'APPROVED'"
                  @click="shipStockTransfer(st.id)"
                  class="text-primary hover:text-primary-dark p-2 rounded-lg hover:bg-primary-light"
                  title="Ship"
                >
                  <Icon name="heroicons:truck" class="h-5 w-5" />
                </button>
                <button
                  v-if="st.status === 'SHIPPED'"
                  @click="receiveStockTransfer(st.id)"
                  class="text-primary hover:text-primary-dark p-2 rounded-lg hover:bg-primary-light"
                  title="Receive"
                >
                  <Icon name="heroicons:inbox-arrow-down" class="h-5 w-5" />
                </button>
                <button
                  v-if="st.status === 'RECEIVED'"
                  @click="completeStockTransfer(st.id)"
                  class="text-success hover:text-success-dark p-2 rounded-lg hover:bg-success-light"
                  title="Complete"
                >
                  <Icon name="heroicons:check-circle" class="h-5 w-5" />
                </button>
                <button
                  v-if="['PENDING', 'APPROVED'].includes(st.status)"
                  @click="cancelStockTransfer(st.id)"
                  class="text-danger hover:text-danger-dark p-2 rounded-lg hover:bg-danger-light"
                  title="Cancel"
                >
                  <Icon name="heroicons:x-mark" class="h-5 w-5" />
                </button>
                <button
                  @click="editStockTransfer(st)"
                  class="text-neutral-gray hover:text-black p-2 rounded-lg hover:bg-neutral-light"
                  title="Edit"
                >
                  <Icon name="heroicons:pencil" class="h-5 w-5" />
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Create/Edit Modal -->
    <div v-if="showModal" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-full max-w-4xl shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-black mb-4">
            {{ isEditing ? 'Edit Stock Transfer' : 'Create Stock Transfer' }}
          </h3>
          
          <form @submit.prevent="saveStockTransfer" class="space-y-6">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">From Warehouse *</label>
                <select
                  v-model="form.fromWarehouseId"
                  required
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-purple-500 focus:border-purple-500"
                >
                  <option value="">Select source warehouse</option>
                  <option v-for="warehouse in warehouses" :key="warehouse.id" :value="warehouse.id">
                    {{ warehouse.warehouseName }}
                  </option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">To Warehouse *</label>
                <select
                  v-model="form.toWarehouseId"
                  required
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-purple-500 focus:border-purple-500"
                >
                  <option value="">Select destination warehouse</option>
                  <option v-for="warehouse in warehouses" :key="warehouse.id" :value="warehouse.id">
                    {{ warehouse.warehouseName }}
                  </option>
                </select>
              </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Transfer Date *</label>
                <input
                  v-model="form.transferDate"
                  type="date"
                  required
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-purple-500 focus:border-purple-500"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Expected Date</label>
                <input
                  v-model="form.expectedDate"
                  type="date"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-purple-500 focus:border-purple-500"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Notes</label>
              <textarea
                v-model="form.notes"
                rows="3"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-purple-500 focus:border-purple-500"
                placeholder="Additional notes or instructions"
              ></textarea>
            </div>

            <!-- Stock Transfer Items -->
            <div>
              <div class="flex items-center justify-between mb-4">
                <h4 class="text-md font-medium text-gray-900">Items</h4>
                <button
                  type="button"
                  @click="addItem"
                  class="text-purple-600 hover:text-purple-800 text-sm font-medium"
                >
                  + Add Item
                </button>
              </div>
              
              <div class="space-y-3">
                <div v-for="(item, index) in form.items" :key="index" class="flex items-center space-x-3 p-3 border border-gray-200 rounded-lg">
                  <select
                    v-model="item.productId"
                    required
                    class="flex-1 px-3 py-2 border border-gray-300 rounded-md focus:ring-purple-500 focus:border-purple-500"
                  >
                    <option value="">Select product</option>
                    <option v-for="product in products" :key="product.id" :value="product.id">
                      {{ product.productName }} ({{ product.productCode }})
                    </option>
                  </select>
                  <input
                    v-model.number="item.quantityTransferred"
                    type="number"
                    min="1"
                    step="0.01"
                    placeholder="Quantity"
                    required
                    class="w-32 px-3 py-2 border border-gray-300 rounded-md focus:ring-purple-500 focus:border-purple-500"
                  />
                  <button
                    type="button"
                    @click="removeItem(index)"
                    class="text-red-600 hover:text-red-800 p-2"
                  >
                    <Icon name="heroicons:trash" class="h-4 w-4" />
                  </button>
                </div>
              </div>
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
                class="px-4 py-2 text-sm font-medium text-white bg-purple-600 hover:bg-purple-700 rounded-md disabled:opacity-50"
              >
                {{ saving ? 'Saving...' : (isEditing ? 'Update' : 'Create') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- View Modal -->
    <div v-if="showViewModal" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-full max-w-4xl shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-6">
            <h3 class="text-lg font-medium text-gray-900">Stock Transfer Details</h3>
            <button @click="closeViewModal" class="text-gray-400 hover:text-gray-600">
              <Icon name="heroicons:x-mark" class="h-6 w-6" />
            </button>
          </div>
          
          <div v-if="selectedST" class="space-y-6">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <h4 class="font-medium text-gray-900 mb-2">Transfer Information</h4>
                <div class="space-y-2 text-sm">
                  <div><span class="font-medium">Transfer Number:</span> {{ selectedST.transferNumber }}</div>
                  <div><span class="font-medium">Status:</span> 
                    <span :class="getStatusBadgeClass(selectedST.status)" class="px-2 py-1 text-xs font-medium rounded-full ml-2">
                      {{ selectedST.status }}
                    </span>
                  </div>
                  <div><span class="font-medium">Transfer Date:</span> {{ formatDate(selectedST.transferDate) }}</div>
                  <div><span class="font-medium">Expected Date:</span> {{ formatDate(selectedST.expectedDate) }}</div>
                </div>
              </div>
              <div>
                <h4 class="font-medium text-gray-900 mb-2">Warehouse Information</h4>
                <div class="space-y-2 text-sm">
                  <div><span class="font-medium">From:</span> {{ selectedST.fromWarehouse?.warehouseName || 'N/A' }}</div>
                  <div><span class="font-medium">To:</span> {{ selectedST.toWarehouse?.warehouseName || 'N/A' }}</div>
                </div>
              </div>
            </div>

            <div>
              <h4 class="font-medium text-gray-900 mb-4">Items</h4>
              <div class="overflow-x-auto">
                <table class="min-w-full divide-y divide-gray-200">
                  <thead class="bg-gray-50">
                    <tr>
                      <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Product</th>
                      <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Quantity</th>
                      <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                    </tr>
                  </thead>
                  <tbody class="bg-white divide-y divide-gray-200">
                    <tr v-for="item in selectedST.items" :key="item.id">
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {{ item.product?.productName || 'N/A' }}
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {{ item.quantityTransferred }}
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        <span class="px-2 py-1 text-xs font-medium rounded-full bg-blue-100 text-blue-800">
                          Transferred
                        </span>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>

            <div v-if="selectedST.notes">
              <h4 class="font-medium text-gray-900 mb-2">Notes</h4>
              <p class="text-sm text-gray-600 bg-gray-50 p-3 rounded-lg">{{ selectedST.notes }}</p>
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

const { showError, showWarning } = useAlert()

interface Warehouse {
  id: number
  warehouseName: string
  warehouseCode: string
}

interface Product {
  id: number
  productName: string
  productCode: string
}

interface StockTransferItem {
  id?: number
  productId: number
  product?: Product
  quantityTransferred: number
}

interface StockTransfer {
  id: number
  transferNumber: string
  fromWarehouseId: number
  fromWarehouse?: Warehouse
  toWarehouseId: number
  toWarehouse?: Warehouse
  transferDate: string
  expectedDate?: string
  status: string
  notes?: string
  items: StockTransferItem[]
}

const { user } = useAuth()
const config = useRuntimeConfig()

// Reactive data
const loading = ref(false)
const saving = ref(false)
const stockTransfers = ref<StockTransfer[]>([])
const warehouses = ref<Warehouse[]>([])
const products = ref<Product[]>([])
const showModal = ref(false)
const showViewModal = ref(false)
const isEditing = ref(false)
const selectedST = ref<StockTransfer | null>(null)

// Filters
const statusFilter = ref('')
const fromWarehouseFilter = ref('')
const toWarehouseFilter = ref('')
const dateFilter = ref('')

// Form data
const form = ref({
  id: 0,
  fromWarehouseId: '',
  toWarehouseId: '',
  transferDate: '',
  expectedDate: '',
  notes: '',
  items: [] as StockTransferItem[]
})

// Methods
const loadStockTransfers = async () => {
  loading.value = true
  try {
    const token = useCookie('auth-token')
    let url = `${config.public.apiBase}/api/StockTransfer`
    
    const params = new URLSearchParams()
    if (statusFilter.value) params.append('status', statusFilter.value)
    if (fromWarehouseFilter.value) params.append('fromWarehouseId', fromWarehouseFilter.value.toString())
    if (toWarehouseFilter.value) params.append('toWarehouseId', toWarehouseFilter.value.toString())
    if (dateFilter.value) params.append('date', dateFilter.value)
    
    if (params.toString()) {
      url += '?' + params.toString()
    }
    
    const response = await $fetch<StockTransfer[]>(url, {
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    
    stockTransfers.value = response
  } catch (error) {
    console.error('Failed to load stock transfers:', error)
  } finally {
    loading.value = false
  }
}

const loadWarehouses = async () => {
  try {
    const token = useCookie('auth-token')
    const response = await $fetch<Warehouse[]>(`${config.public.apiBase}/api/Warehouse`, {
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    warehouses.value = response
  } catch (error) {
    console.error('Failed to load warehouses:', error)
  }
}

const loadProducts = async () => {
  try {
    const token = useCookie('auth-token')
    const response = await $fetch<Product[]>(`${config.public.apiBase}/api/Product`, {
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    products.value = response
  } catch (error) {
    console.error('Failed to load products:', error)
  }
}

const openCreateModal = () => {
  isEditing.value = false
  form.value = {
    id: 0,
    fromWarehouseId: '',
    toWarehouseId: '',
    transferDate: new Date().toISOString().split('T')[0],
    expectedDate: '',
    notes: '',
    items: []
  }
  showModal.value = true
}

const editStockTransfer = (st: StockTransfer) => {
  isEditing.value = true
  form.value = {
    id: st.id,
    fromWarehouseId: st.fromWarehouseId?.toString() || '',
    toWarehouseId: st.toWarehouseId?.toString() || '',
    transferDate: st.transferDate ? st.transferDate.split('T')[0] : '',
    expectedDate: st.expectedDate ? st.expectedDate.split('T')[0] : '',
    notes: st.notes || '',
    items: st.items?.map(item => ({
      id: item.id,
      productId: item.productId,
      quantityTransferred: item.quantityTransferred
    })) || []
  }
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
}

const viewStockTransfer = (st: StockTransfer) => {
  selectedST.value = st
  showViewModal.value = true
}

const closeViewModal = () => {
  showViewModal.value = false
  selectedST.value = null
}

const addItem = () => {
  form.value.items.push({
    productId: 0,
    quantityTransferred: 1
  })
}

const removeItem = (index: number) => {
  form.value.items.splice(index, 1)
}

const saveStockTransfer = async () => {
  if (!form.value.fromWarehouseId || !form.value.toWarehouseId || form.value.items.length === 0) {
    showWarning('Please select both warehouses and add at least one item')
    return
  }

  if (form.value.fromWarehouseId === form.value.toWarehouseId) {
    showWarning('Source and destination warehouses must be different')
    return
  }

  saving.value = true
  try {
    const token = useCookie('auth-token')
    const payload = {
      fromWarehouseId: parseInt(form.value.fromWarehouseId),
      toWarehouseId: parseInt(form.value.toWarehouseId),
      transferDate: form.value.transferDate,
      expectedDate: form.value.expectedDate || null,
      notes: form.value.notes,
      items: form.value.items.map(item => ({
        productId: Number(item.productId),
        quantityTransferred: item.quantityTransferred
      }))
    }

    if (isEditing.value) {
      await $fetch(`${config.public.apiBase}/api/StockTransfer/${form.value.id}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token.value || ''}`,
          'Content-Type': 'application/json'
        },
        body: { ...payload, id: form.value.id }
      })
    } else {
      await $fetch(`${config.public.apiBase}/api/StockTransfer`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token.value || ''}`,
          'Content-Type': 'application/json'
        },
        body: payload
      })
    }

    closeModal()
    await loadStockTransfers()
  } catch (error) {
    console.error('Failed to save stock transfer:', error)
    showError('Failed to save stock transfer')
  } finally {
    saving.value = false
  }
}

const approveStockTransfer = async (id: number) => {
  if (!confirm('Are you sure you want to approve this stock transfer?')) return

  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/StockTransfer/${id}/approve`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    await loadStockTransfers()
  } catch (error) {
    console.error('Failed to approve stock transfer:', error)
    showError('Failed to approve stock transfer')
  }
}

const shipStockTransfer = async (id: number) => {
  if (!confirm('Are you sure you want to ship this stock transfer?')) return

  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/StockTransfer/${id}/ship`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    await loadStockTransfers()
  } catch (error) {
    console.error('Failed to ship stock transfer:', error)
    showError('Failed to ship stock transfer')
  }
}

const receiveStockTransfer = async (id: number) => {
  if (!confirm('Are you sure you want to receive this stock transfer?')) return

  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/StockTransfer/${id}/receive`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    await loadStockTransfers()
  } catch (error) {
    console.error('Failed to receive stock transfer:', error)
    showError('Failed to receive stock transfer')
  }
}

const completeStockTransfer = async (id: number) => {
  if (!confirm('Are you sure you want to complete this stock transfer?')) return

  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/StockTransfer/${id}/complete`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    await loadStockTransfers()
  } catch (error) {
    console.error('Failed to complete stock transfer:', error)
    showError('Failed to complete stock transfer')
  }
}

const cancelStockTransfer = async (id: number) => {
  if (!confirm('Are you sure you want to cancel this stock transfer?')) return

  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/StockTransfer/${id}/cancel`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    await loadStockTransfers()
  } catch (error) {
    console.error('Failed to cancel stock transfer:', error)
    showError('Failed to cancel stock transfer')
  }
}

const getStatusBadgeClass = (status: string) => {
  const classes = {
    'PENDING': 'bg-yellow-100 text-yellow-800',
    'APPROVED': 'bg-blue-100 text-blue-800',
    'SHIPPED': 'bg-indigo-100 text-indigo-800',
    'RECEIVED': 'bg-purple-100 text-purple-800',
    'COMPLETED': 'bg-green-100 text-green-800',
    'CANCELLED': 'bg-red-100 text-red-800'
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const formatDate = (dateString: string | undefined) => {
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

// Load data on mount
onMounted(async () => {
  await Promise.all([
    loadStockTransfers(),
    loadWarehouses(),
    loadProducts()
  ])
})
</script>