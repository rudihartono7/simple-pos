<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <header class="bg-gradient-to-r from-green-600 to-green-700 text-white shadow-lg">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center py-6">
          <div class="flex items-center space-x-4">
            <Icon name="heroicons:truck" class="h-8 w-8" />
            <div>
              <h1 class="text-3xl font-bold">Stock Receiving</h1>
              <p class="text-green-100 text-lg">Manage incoming stock and deliveries</p>
            </div>
          </div>
          <div class="flex items-center space-x-4">
            <button
              @click="openCreateModal"
              class="bg-white text-green-600 px-6 py-3 rounded-lg font-semibold hover:bg-green-50 transition-colors flex items-center space-x-2"
            >
              <Icon name="heroicons:plus" class="h-5 w-5" />
              <span>New Stock Receiving</span>
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
      <!-- Filters -->
      <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Status</label>
            <select
              v-model="statusFilter"
              @change="loadStockReceivings"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-green-500 focus:border-green-500"
            >
              <option value="">All Status</option>
              <option value="PENDING">Pending</option>
              <option value="RECEIVED">Received</option>
              <option value="CANCELLED">Cancelled</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Supplier</label>
            <select
              v-model="supplierFilter"
              @change="loadStockReceivings"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-green-500 focus:border-green-500"
            >
              <option value="">All Suppliers</option>
              <option v-for="supplier in suppliers" :key="supplier.id" :value="supplier.id">
                {{ supplier.supplierName }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Purchase Order</label>
            <select
              v-model="purchaseOrderFilter"
              @change="loadStockReceivings"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-green-500 focus:border-green-500"
            >
              <option value="">All Purchase Orders</option>
              <option v-for="po in purchaseOrders" :key="po.id" :value="po.id">
                {{ po.orderNumber }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Show Pending Only</label>
            <label class="flex items-center">
              <input
                v-model="pendingOnly"
                @change="loadStockReceivings"
                type="checkbox"
                class="rounded border-gray-300 text-green-600 focus:ring-green-500"
              />
              <span class="ml-2 text-sm text-gray-700">Pending only</span>
            </label>
          </div>
        </div>
      </div>

      <!-- Stock Receivings List -->
      <div class="bg-white shadow-sm rounded-lg border border-gray-200 overflow-hidden">
        <div class="px-6 py-4 border-b border-gray-200 bg-gray-50">
          <h3 class="text-lg font-semibold text-gray-900">Stock Receivings ({{ stockReceivings.length }})</h3>
        </div>
        
        <div v-if="loading" class="p-8 text-center">
          <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-green-600"></div>
          <p class="mt-2 text-gray-600">Loading stock receivings...</p>
        </div>

        <div v-else-if="stockReceivings.length === 0" class="p-8 text-center">
          <Icon name="heroicons:truck" class="h-12 w-12 text-gray-400 mx-auto mb-4" />
          <h3 class="text-lg font-medium text-gray-900 mb-2">No stock receivings found</h3>
          <p class="text-gray-500">Create your first stock receiving to get started.</p>
        </div>

        <div v-else class="divide-y divide-gray-200">
          <div v-for="sr in stockReceivings" :key="sr.id" class="p-6 hover:bg-gray-50 transition-colors">
            <div class="flex items-center justify-between">
              <div class="flex-1">
                <div class="flex items-center space-x-4 mb-2">
                  <h4 class="text-lg font-semibold text-gray-900">{{ sr.receivingNumber }}</h4>
                  <span :class="getStatusBadgeClass(sr.status)" class="px-2 py-1 text-xs font-medium rounded-full">
                    {{ sr.status }}
                  </span>
                </div>
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4 text-sm text-gray-600">
                  <div>
                    <span class="font-medium">Supplier:</span> {{ sr.supplier?.supplierName || 'N/A' }}
                  </div>
                  <div>
                    <span class="font-medium">Purchase Order:</span> {{ sr.purchaseOrder?.orderNumber || 'N/A' }}
                  </div>
                  <div>
                    <span class="font-medium">Received Date:</span> {{ formatDate(sr.receivedDate) }}
                  </div>
                </div>
                <div class="mt-2 text-sm text-gray-600">
                  <span class="font-medium">Total Items:</span> {{ sr.items?.length || 0 }}
                </div>
              </div>
              <div class="flex items-center space-x-2">
                <button
                  @click="viewStockReceiving(sr)"
                  class="text-green-600 hover:text-green-800 p-2 rounded-lg hover:bg-green-50"
                  title="View Details"
                >
                  <Icon name="heroicons:eye" class="h-5 w-5" />
                </button>
                <button
                  v-if="sr.status === 'PENDING'"
                  @click="processStockReceiving(sr.id)"
                  class="text-blue-600 hover:text-blue-800 p-2 rounded-lg hover:bg-blue-50"
                  title="Process"
                >
                  <Icon name="heroicons:check" class="h-5 w-5" />
                </button>
                <button
                  @click="editStockReceiving(sr)"
                  class="text-gray-600 hover:text-gray-800 p-2 rounded-lg hover:bg-gray-50"
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
          <h3 class="text-lg font-medium text-gray-900 mb-4">
            {{ isEditing ? 'Edit Stock Receiving' : 'Create Stock Receiving' }}
          </h3>
          
          <form @submit.prevent="saveStockReceiving" class="space-y-6">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Supplier *</label>
                <select
                  v-model="form.supplierId"
                  required
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-green-500 focus:border-green-500"
                >
                  <option value="">Select supplier</option>
                  <option v-for="supplier in suppliers" :key="supplier.id" :value="supplier.id">
                    {{ supplier.supplierName }}
                  </option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Purchase Order</label>
                <select
                  v-model="form.purchaseOrderId"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-green-500 focus:border-green-500"
                >
                  <option value="">Select purchase order (optional)</option>
                  <option v-for="po in purchaseOrders" :key="po.id" :value="po.id">
                    {{ po.orderNumber }}
                  </option>
                </select>
              </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Received Date *</label>
                <input
                  v-model="form.receivedDate"
                  type="date"
                  required
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-green-500 focus:border-green-500"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Invoice Number</label>
                <input
                  v-model="form.invoiceNumber"
                  type="text"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-green-500 focus:border-green-500"
                  placeholder="Supplier invoice number"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Notes</label>
              <textarea
                v-model="form.notes"
                rows="3"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-green-500 focus:border-green-500"
                placeholder="Additional notes"
              ></textarea>
            </div>

            <!-- Stock Receiving Items -->
            <div>
              <div class="flex items-center justify-between mb-4">
                <h4 class="text-md font-medium text-gray-900">Items</h4>
                <button
                  type="button"
                  @click="addItem"
                  class="text-green-600 hover:text-green-800 text-sm font-medium"
                >
                  + Add Item
                </button>
              </div>
              
              <div class="space-y-3">
                <div v-for="(item, index) in form.items" :key="index" class="flex items-center space-x-3 p-3 border border-gray-200 rounded-lg">
                  <select
                    v-model="item.productId"
                    required
                    class="flex-1 px-3 py-2 border border-gray-300 rounded-md focus:ring-green-500 focus:border-green-500"
                  >
                    <option value="">Select product</option>
                    <option v-for="product in products" :key="product.id" :value="product.id">
                      {{ product.productName }} ({{ product.productCode }})
                    </option>
                  </select>
                  <input
                    v-model.number="item.quantityReceived"
                    type="number"
                    min="0"
                    step="0.01"
                    placeholder="Qty Received"
                    required
                    class="w-32 px-3 py-2 border border-gray-300 rounded-md focus:ring-green-500 focus:border-green-500"
                  />
                  <input
                    v-model.number="item.unitCost"
                    type="number"
                    min="0"
                    step="0.01"
                    placeholder="Unit Cost"
                    class="w-32 px-3 py-2 border border-gray-300 rounded-md focus:ring-green-500 focus:border-green-500"
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
                class="px-4 py-2 text-sm font-medium text-white bg-green-600 hover:bg-green-700 rounded-md disabled:opacity-50"
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
            <h3 class="text-lg font-medium text-gray-900">Stock Receiving Details</h3>
            <button @click="closeViewModal" class="text-gray-400 hover:text-gray-600">
              <Icon name="heroicons:x-mark" class="h-6 w-6" />
            </button>
          </div>
          
          <div v-if="selectedSR" class="space-y-6">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <h4 class="font-medium text-gray-900 mb-2">Receiving Information</h4>
                <div class="space-y-2 text-sm">
                  <div><span class="font-medium">Receiving Number:</span> {{ selectedSR.receivingNumber }}</div>
                  <div><span class="font-medium">Status:</span> 
                    <span :class="getStatusBadgeClass(selectedSR.status)" class="px-2 py-1 text-xs font-medium rounded-full ml-2">
                      {{ selectedSR.status }}
                    </span>
                  </div>
                  <div><span class="font-medium">Received Date:</span> {{ formatDate(selectedSR.receivedDate) }}</div>
                  <div><span class="font-medium">Invoice Number:</span> {{ selectedSR.invoiceNumber || 'N/A' }}</div>
                </div>
              </div>
              <div>
                <h4 class="font-medium text-gray-900 mb-2">Supplier Information</h4>
                <div class="space-y-2 text-sm">
                  <div><span class="font-medium">Name:</span> {{ selectedSR.supplier?.supplierName || 'N/A' }}</div>
                  <div><span class="font-medium">Contact:</span> {{ selectedSR.supplier?.contactPerson || 'N/A' }}</div>
                  <div><span class="font-medium">Phone:</span> {{ selectedSR.supplier?.phone || 'N/A' }}</div>
                  <div><span class="font-medium">Email:</span> {{ selectedSR.supplier?.email || 'N/A' }}</div>
                </div>
              </div>
            </div>

            <div v-if="selectedSR.purchaseOrder">
              <h4 class="font-medium text-gray-900 mb-2">Purchase Order</h4>
              <div class="text-sm text-gray-600">
                <span class="font-medium">Order Number:</span> {{ selectedSR.purchaseOrder.orderNumber }}
              </div>
            </div>

            <div>
              <h4 class="font-medium text-gray-900 mb-4">Items</h4>
              <div class="overflow-x-auto">
                <table class="min-w-full divide-y divide-gray-200">
                  <thead class="bg-gray-50">
                    <tr>
                      <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Product</th>
                      <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Qty Received</th>
                      <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Unit Cost</th>
                      <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Total</th>
                    </tr>
                  </thead>
                  <tbody class="bg-white divide-y divide-gray-200">
                    <tr v-for="item in selectedSR.items" :key="item.id">
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {{ item.product?.productName || 'N/A' }}
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {{ item.quantityReceived }}
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        ${{ item.unitCost?.toFixed(2) || '0.00' }}
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        ${{ ((item.quantityReceived || 0) * (item.unitCost || 0)).toFixed(2) }}
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>

            <div v-if="selectedSR.notes">
              <h4 class="font-medium text-gray-900 mb-2">Notes</h4>
              <p class="text-sm text-gray-600 bg-gray-50 p-3 rounded-lg">{{ selectedSR.notes }}</p>
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

interface Supplier {
  id: number
  supplierName: string
  contactPerson?: string
  phone?: string
  email?: string
}

interface Product {
  id: number
  productName: string
  productCode: string
}

interface PurchaseOrder {
  id: number
  orderNumber: string
}

interface StockReceivingItem {
  id?: number
  productId: number
  product?: Product
  quantityReceived: number
  unitCost?: number
}

interface StockReceiving {
  id: number
  receivingNumber: string
  supplierId: number
  supplier?: Supplier
  purchaseOrderId?: number
  purchaseOrder?: PurchaseOrder
  receivedDate: string
  invoiceNumber?: string
  status: string
  notes?: string
  items: StockReceivingItem[]
}

const { user } = useAuth()
const config = useRuntimeConfig()

// Reactive data
const loading = ref(false)
const saving = ref(false)
const stockReceivings = ref<StockReceiving[]>([])
const suppliers = ref<Supplier[]>([])
const products = ref<Product[]>([])
const purchaseOrders = ref<PurchaseOrder[]>([])
const showModal = ref(false)
const showViewModal = ref(false)
const isEditing = ref(false)
const selectedSR = ref<StockReceiving | null>(null)

// Filters
const statusFilter = ref('')
const supplierFilter = ref('')
const purchaseOrderFilter = ref('')
const pendingOnly = ref(false)

// Form data
const form = ref({
  id: 0,
  supplierId: '',
  purchaseOrderId: '',
  receivedDate: '',
  invoiceNumber: '',
  notes: '',
  items: [] as StockReceivingItem[]
})

// Methods
const loadStockReceivings = async () => {
  loading.value = true
  try {
    const token = useCookie('auth-token')
    let url = `${config.public.apiBase}/api/StockReceiving`
    
    const params = new URLSearchParams()
    if (statusFilter.value) params.append('status', statusFilter.value)
    if (supplierFilter.value) params.append('supplierId', supplierFilter.value)
    if (purchaseOrderFilter.value) params.append('purchaseOrderId', purchaseOrderFilter.value)
    if (pendingOnly.value) params.append('pending', 'true')
    
    if (params.toString()) {
      url += '?' + params.toString()
    }
    
    const response = await $fetch<StockReceiving[]>(url, {
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    
    stockReceivings.value = response
  } catch (error) {
    console.error('Failed to load stock receivings:', error)
  } finally {
    loading.value = false
  }
}

const loadSuppliers = async () => {
  try {
    const token = useCookie('auth-token')
    const response = await $fetch<Supplier[]>(`${config.public.apiBase}/api/Supplier`, {
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    suppliers.value = response
  } catch (error) {
    console.error('Failed to load suppliers:', error)
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

const loadPurchaseOrders = async () => {
  try {
    const token = useCookie('auth-token')
    const response = await $fetch<PurchaseOrder[]>(`${config.public.apiBase}/api/PurchaseOrder`, {
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    purchaseOrders.value = response
  } catch (error) {
    console.error('Failed to load purchase orders:', error)
  }
}

const openCreateModal = () => {
  isEditing.value = false
  form.value = {
    id: 0,
    supplierId: '',
    purchaseOrderId: '',
    receivedDate: new Date().toISOString().split('T')[0],
    invoiceNumber: '',
    notes: '',
    items: []
  }
  showModal.value = true
}

const editStockReceiving = (sr: StockReceiving) => {
  isEditing.value = true
  form.value = {
    id: sr.id,
    supplierId: sr.supplierId.toString(),
    purchaseOrderId: sr.purchaseOrderId?.toString() || '',
    receivedDate: sr.receivedDate ? sr.receivedDate.split('T')[0] : '',
    invoiceNumber: sr.invoiceNumber || '',
    notes: sr.notes || '',
    items: sr.items.map(item => ({
      id: item.id,
      productId: item.productId,
      quantityReceived: item.quantityReceived,
      unitCost: item.unitCost || 0
    }))
  }
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
}

const viewStockReceiving = (sr: StockReceiving) => {
  selectedSR.value = sr
  showViewModal.value = true
}

const closeViewModal = () => {
  showViewModal.value = false
  selectedSR.value = null
}

const addItem = () => {
  form.value.items.push({
    productId: 0,
    quantityReceived: 0,
    unitCost: 0
  })
}

const removeItem = (index: number) => {
  form.value.items.splice(index, 1)
}

const saveStockReceiving = async () => {
  if (!form.value.supplierId || form.value.items.length === 0) {
    showWarning('Please select a supplier and add at least one item')
    return
  }

  saving.value = true
  try {
    const token = useCookie('auth-token')
    const payload = {
      supplierId: Number(form.value.supplierId),
      purchaseOrderId: form.value.purchaseOrderId ? Number(form.value.purchaseOrderId) : null,
      receivedDate: form.value.receivedDate,
      invoiceNumber: form.value.invoiceNumber || null,
      notes: form.value.notes,
      items: form.value.items.map(item => ({
        productId: Number(item.productId),
        quantityReceived: Number(item.quantityReceived),
        unitCost: Number(item.unitCost) || 0
      }))
    }

    if (isEditing.value) {
      await $fetch(`${config.public.apiBase}/api/StockReceiving/${form.value.id}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token.value || ''}`,
          'Content-Type': 'application/json'
        },
        body: { ...payload, id: form.value.id }
      })
    } else {
      await $fetch(`${config.public.apiBase}/api/StockReceiving`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token.value || ''}`,
          'Content-Type': 'application/json'
        },
        body: payload
      })
    }

    closeModal()
    await loadStockReceivings()
  } catch (error) {
    console.error('Failed to save stock receiving:', error)
    showError('Failed to save stock receiving')
  } finally {
    saving.value = false
  }
}

const processStockReceiving = async (id: number) => {
  if (!confirm('Are you sure you want to process this stock receiving? This will update inventory levels.')) return

  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/StockReceiving/${id}/process`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    await loadStockReceivings()
  } catch (error) {
    console.error('Failed to process stock receiving:', error)
    showError('Failed to process stock receiving')
  }
}

const getStatusBadgeClass = (status: string) => {
  const classes = {
    'PENDING': 'bg-yellow-100 text-yellow-800',
    'RECEIVED': 'bg-green-100 text-green-800',
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
    loadStockReceivings(),
    loadSuppliers(),
    loadProducts(),
    loadPurchaseOrders()
  ])
})
</script>