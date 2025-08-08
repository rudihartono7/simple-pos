<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <header class="bg-gradient-to-r from-blue-600 to-blue-700 text-white shadow-lg">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center py-6">
          <div class="flex items-center space-x-4">
            <Icon name="heroicons:document-text" class="h-8 w-8" />
            <div>
              <h1 class="text-3xl font-bold">Purchase Orders</h1>
              <p class="text-blue-100 text-lg">Manage purchase orders and supplier relationships</p>
            </div>
          </div>
          <div class="flex items-center space-x-4">
            <button
              @click="openCreateModal"
              class="bg-white text-blue-600 px-6 py-3 rounded-lg font-semibold hover:bg-blue-50 transition-colors flex items-center space-x-2"
            >
              <Icon name="heroicons:plus" class="h-5 w-5" />
              <span>New Purchase Order</span>
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
              @change="loadPurchaseOrders"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="">All Status</option>
              <option value="PENDING">Pending</option>
              <option value="APPROVED">Approved</option>
              <option value="RECEIVED">Received</option>
              <option value="CANCELLED">Cancelled</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Supplier</label>
            <select
              v-model="supplierFilter"
              @change="loadPurchaseOrders"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="">All Suppliers</option>
              <option v-for="supplier in suppliers" :key="supplier.id" :value="supplier.id">
                {{ supplier.supplierName }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Start Date</label>
            <input
              v-model="startDate"
              type="date"
              @change="loadPurchaseOrders"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">End Date</label>
            <input
              v-model="endDate"
              type="date"
              @change="loadPurchaseOrders"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
            />
          </div>
        </div>
      </div>

      <!-- Purchase Orders List -->
      <div class="bg-white shadow-sm rounded-lg border border-gray-200 overflow-hidden">
        <div class="px-6 py-4 border-b border-gray-200 bg-gray-50">
          <h3 class="text-lg font-semibold text-gray-900">Purchase Orders ({{ purchaseOrders.length }})</h3>
        </div>
        
        <div v-if="loading" class="p-8 text-center">
          <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
          <p class="mt-2 text-gray-600">Loading purchase orders...</p>
        </div>

        <div v-else-if="purchaseOrders.length === 0" class="p-8 text-center">
          <Icon name="heroicons:document-text" class="h-12 w-12 text-gray-400 mx-auto mb-4" />
          <h3 class="text-lg font-medium text-gray-900 mb-2">No purchase orders found</h3>
          <p class="text-gray-500">Create your first purchase order to get started.</p>
        </div>

        <div v-else class="divide-y divide-gray-200">
          <div v-for="po in purchaseOrders" :key="po.id" class="p-6 hover:bg-gray-50 transition-colors">
            <div class="flex items-center justify-between">
              <div class="flex-1">
                <div class="flex items-center space-x-4 mb-2">
                  <h4 class="text-lg font-semibold text-gray-900">{{ po.orderNumber }}</h4>
                  <span :class="getStatusBadgeClass(po.status)" class="px-2 py-1 text-xs font-medium rounded-full">
                    {{ po.status }}
                  </span>
                </div>
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4 text-sm text-gray-600">
                  <div>
                    <span class="font-medium">Supplier:</span> {{ po.supplier?.supplierName || 'N/A' }}
                  </div>
                  <div>
                    <span class="font-medium">Order Date:</span> {{ formatDate(po.orderDate) }}
                  </div>
                  <div>
                    <span class="font-medium">Expected Date:</span> {{ formatDate(po.expectedDeliveryDate) }}
                  </div>
                </div>
                <div class="mt-2 text-sm text-gray-600">
                  <span class="font-medium">Total Amount:</span> 
                  <span class="text-lg font-bold text-green-600">${{ po.totalAmount?.toFixed(2) || '0.00' }}</span>
                </div>
              </div>
              <div class="flex items-center space-x-2">
                <button
                  @click="viewPurchaseOrder(po)"
                  class="text-blue-600 hover:text-blue-800 p-2 rounded-lg hover:bg-blue-50"
                  title="View Details"
                >
                  <Icon name="heroicons:eye" class="h-5 w-5" />
                </button>
                <button
                  v-if="po.status === 'PENDING'"
                  @click="approvePurchaseOrder(po.id)"
                  class="text-green-600 hover:text-green-800 p-2 rounded-lg hover:bg-green-50"
                  title="Approve"
                >
                  <Icon name="heroicons:check" class="h-5 w-5" />
                </button>
                <button
                  v-if="po.status === 'PENDING'"
                  @click="cancelPurchaseOrder(po.id)"
                  class="text-red-600 hover:text-red-800 p-2 rounded-lg hover:bg-red-50"
                  title="Cancel"
                >
                  <Icon name="heroicons:x-mark" class="h-5 w-5" />
                </button>
                <button
                  @click="editPurchaseOrder(po)"
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
            {{ isEditing ? 'Edit Purchase Order' : 'Create Purchase Order' }}
          </h3>
          
          <form @submit.prevent="savePurchaseOrder" class="space-y-6">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Supplier *</label>
                <select
                  v-model="form.supplierId"
                  required
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                >
                  <option value="">Select supplier</option>
                  <option v-for="supplier in suppliers" :key="supplier.id" :value="supplier.id">
                    {{ supplier.supplierName }}
                  </option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Expected Delivery Date</label>
                <input
                  v-model="form.expectedDeliveryDate"
                  type="date"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Notes</label>
              <textarea
                v-model="form.notes"
                rows="3"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                placeholder="Additional notes or instructions"
              ></textarea>
            </div>

            <!-- Purchase Order Items -->
            <div>
              <div class="flex items-center justify-between mb-4">
                <h4 class="text-md font-medium text-gray-900">Items</h4>
                <button
                  type="button"
                  @click="addItem"
                  class="text-blue-600 hover:text-blue-800 text-sm font-medium"
                >
                  + Add Item
                </button>
              </div>
              
              <div class="space-y-3">
                <div v-for="(item, index) in form.items" :key="index" class="flex items-center space-x-3 p-3 border border-gray-200 rounded-lg">
                  <select
                    v-model="item.productId"
                    required
                    class="flex-1 px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                  >
                    <option value="">Select product</option>
                    <option v-for="product in products" :key="product.id" :value="product.id">
                      {{ product.productName }} ({{ product.productCode }})
                    </option>
                  </select>
                  <input
                    v-model.number="item.quantityOrdered"
                    type="number"
                    min="1"
                    step="0.01"
                    placeholder="Qty"
                    required
                    class="w-24 px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                  />
                  <input
                    v-model.number="item.unitCost"
                    type="number"
                    min="0"
                    step="0.01"
                    placeholder="Unit Cost"
                    required
                    class="w-32 px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
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
                class="px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-md disabled:opacity-50"
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
            <h3 class="text-lg font-medium text-gray-900">Purchase Order Details</h3>
            <button @click="closeViewModal" class="text-gray-400 hover:text-gray-600">
              <Icon name="heroicons:x-mark" class="h-6 w-6" />
            </button>
          </div>
          
          <div v-if="selectedPO" class="space-y-6">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <h4 class="font-medium text-gray-900 mb-2">Order Information</h4>
                <div class="space-y-2 text-sm">
                  <div><span class="font-medium">Order Number:</span> {{ selectedPO.orderNumber }}</div>
                  <div><span class="font-medium">Status:</span> 
                    <span :class="getStatusBadgeClass(selectedPO.status)" class="px-2 py-1 text-xs font-medium rounded-full ml-2">
                      {{ selectedPO.status }}
                    </span>
                  </div>
                  <div><span class="font-medium">Order Date:</span> {{ formatDate(selectedPO.orderDate) }}</div>
                  <div><span class="font-medium">Expected Delivery:</span> {{ formatDate(selectedPO.expectedDeliveryDate) }}</div>
                </div>
              </div>
              <div>
                <h4 class="font-medium text-gray-900 mb-2">Supplier Information</h4>
                <div class="space-y-2 text-sm">
                  <div><span class="font-medium">Name:</span> {{ selectedPO.supplier?.supplierName || 'N/A' }}</div>
                  <div><span class="font-medium">Contact:</span> {{ selectedPO.supplier?.contactPerson || 'N/A' }}</div>
                  <div><span class="font-medium">Phone:</span> {{ selectedPO.supplier?.phone || 'N/A' }}</div>
                  <div><span class="font-medium">Email:</span> {{ selectedPO.supplier?.email || 'N/A' }}</div>
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
                      <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Qty Ordered</th>
                      <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Unit Cost</th>
                      <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Total</th>
                    </tr>
                  </thead>
                  <tbody class="bg-white divide-y divide-gray-200">
                    <tr v-for="item in selectedPO.items" :key="item.id">
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {{ item.product?.productName || 'N/A' }}
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {{ item.quantityOrdered }}
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        ${{ item.unitCost?.toFixed(2) || '0.00' }}
                      </td>
                      <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        ${{ ((item.quantityOrdered || 0) * (item.unitCost || 0)).toFixed(2) }}
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
              <div class="mt-4 text-right">
                <div class="text-lg font-bold">
                  Total: ${{ selectedPO.totalAmount?.toFixed(2) || '0.00' }}
                </div>
              </div>
            </div>

            <div v-if="selectedPO.notes">
              <h4 class="font-medium text-gray-900 mb-2">Notes</h4>
              <p class="text-sm text-gray-600 bg-gray-50 p-3 rounded-lg">{{ selectedPO.notes }}</p>
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

interface PurchaseOrderItem {
  id?: number
  productId: number
  product?: Product
  quantityOrdered: number
  unitCost: number
}

interface PurchaseOrder {
  id: number
  orderNumber: string
  supplierId: number
  supplier?: Supplier
  orderDate: string
  expectedDeliveryDate?: string
  status: string
  totalAmount?: number
  notes?: string
  items: PurchaseOrderItem[]
}

const { user } = useAuth()
const config = useRuntimeConfig()

// Reactive data
const loading = ref(false)
const saving = ref(false)
const purchaseOrders = ref<PurchaseOrder[]>([])
const suppliers = ref<Supplier[]>([])
const products = ref<Product[]>([])
const showModal = ref(false)
const showViewModal = ref(false)
const isEditing = ref(false)
const selectedPO = ref<PurchaseOrder | null>(null)

// Filters
const statusFilter = ref('')
const supplierFilter = ref('')
const startDate = ref('')
const endDate = ref('')

// Form data
const form = ref({
  id: 0,
  supplierId: '',
  expectedDeliveryDate: '',
  notes: '',
  items: [] as PurchaseOrderItem[]
})

// Methods
const loadPurchaseOrders = async () => {
  loading.value = true
  try {
    const token = useCookie('auth-token')
    let url = `${config.public.apiBase}/api/PurchaseOrder`
    
    const params = new URLSearchParams()
    if (statusFilter.value) params.append('status', statusFilter.value)
    if (supplierFilter.value) params.append('supplierId', supplierFilter.value)
    if (startDate.value) params.append('startDate', startDate.value)
    if (endDate.value) params.append('endDate', endDate.value)
    
    if (params.toString()) {
      url += '?' + params.toString()
    }
    
    const response = await $fetch<PurchaseOrder[]>(url, {
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    
    purchaseOrders.value = response
  } catch (error) {
    console.error('Failed to load purchase orders:', error)
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

const openCreateModal = () => {
  isEditing.value = false
  form.value = {
    id: 0,
    supplierId: '',
    expectedDeliveryDate: '',
    notes: '',
    items: []
  }
  showModal.value = true
}

const editPurchaseOrder = (po: PurchaseOrder) => {
  isEditing.value = true
  form.value = {
    id: po.id,
    supplierId: po.supplierId.toString(),
    expectedDeliveryDate: po.expectedDeliveryDate ? po.expectedDeliveryDate.split('T')[0] : '',
    notes: po.notes || '',
    items: po.items.map(item => ({
      id: item.id,
      productId: item.productId,
      quantityOrdered: item.quantityOrdered,
      unitCost: item.unitCost
    }))
  }
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
}

const viewPurchaseOrder = (po: PurchaseOrder) => {
  selectedPO.value = po
  showViewModal.value = true
}

const closeViewModal = () => {
  showViewModal.value = false
  selectedPO.value = null
}

const addItem = () => {
  form.value.items.push({
    productId: 0,
    quantityOrdered: 1,
    unitCost: 0
  })
}

const removeItem = (index: number) => {
  form.value.items.splice(index, 1)
}

const savePurchaseOrder = async () => {
  if (!form.value.supplierId || form.value.items.length === 0) {
    alert('Please select a supplier and add at least one item')
    return
  }

  saving.value = true
  try {
    const token = useCookie('auth-token')
    const payload = {
      supplierId: parseInt(form.value.supplierId),
      expectedDeliveryDate: form.value.expectedDeliveryDate || null,
      notes: form.value.notes,
      items: form.value.items.map(item => ({
        productId: item.productId,
        quantityOrdered: item.quantityOrdered,
        unitCost: item.unitCost
      }))
    }

    if (isEditing.value) {
      await $fetch(`${config.public.apiBase}/api/PurchaseOrder/${form.value.id}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token.value || ''}`,
          'Content-Type': 'application/json'
        },
        body: { ...payload, id: form.value.id }
      })
    } else {
      await $fetch(`${config.public.apiBase}/api/PurchaseOrder`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token.value || ''}`,
          'Content-Type': 'application/json'
        },
        body: payload
      })
    }

    closeModal()
    await loadPurchaseOrders()
  } catch (error) {
    console.error('Failed to save purchase order:', error)
    alert('Failed to save purchase order')
  } finally {
    saving.value = false
  }
}

const approvePurchaseOrder = async (id: number) => {
  if (!confirm('Are you sure you want to approve this purchase order?')) return

  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/PurchaseOrder/${id}/approve`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    await loadPurchaseOrders()
  } catch (error) {
    console.error('Failed to approve purchase order:', error)
    alert('Failed to approve purchase order')
  }
}

const cancelPurchaseOrder = async (id: number) => {
  if (!confirm('Are you sure you want to cancel this purchase order?')) return

  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/PurchaseOrder/${id}/cancel`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    await loadPurchaseOrders()
  } catch (error) {
    console.error('Failed to cancel purchase order:', error)
    alert('Failed to cancel purchase order')
  }
}

const getStatusBadgeClass = (status: string) => {
  const classes = {
    'PENDING': 'bg-yellow-100 text-yellow-800',
    'APPROVED': 'bg-blue-100 text-blue-800',
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
    loadPurchaseOrders(),
    loadSuppliers(),
    loadProducts()
  ])
})
</script>