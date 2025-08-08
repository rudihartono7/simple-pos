<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <header class="bg-gradient-to-r from-blue-600 to-blue-700 text-white shadow-lg">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center py-4">
          <div class="flex items-center space-x-4">
            <Icon name="heroicons:document-text" class="h-8 w-8" />
            <div>
              <h1 class="text-2xl font-bold">Transaction Management</h1>
              <p class="text-blue-100">Manage and track all transactions</p>
            </div>
          </div>
          <div class="flex items-center space-x-4">
            <NuxtLink to="/pos" class="btn-secondary">
              <Icon name="heroicons:arrow-left" class="h-4 w-4 mr-2" />
              Back to POS
            </NuxtLink>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Search and Filter Section -->
      <div class="bg-white rounded-lg shadow-sm p-6 mb-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Search</label>
            <div class="relative">
              <Icon name="heroicons:magnifying-glass" class="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <input
                v-model="searchTerm"
                type="text"
                placeholder="Transaction number, customer..."
                class="form-input pl-10"
                @input="searchTransactions"
              />
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Status</label>
            <select v-model="statusFilter" @change="filterTransactions" class="form-input">
              <option value="">All Status</option>
              <option value="Pending">Pending</option>
              <option value="Held">Held</option>
              <option value="Completed">Completed</option>
              <option value="Refunded">Refunded</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Date Range</label>
            <input
              v-model="dateFilter"
              type="date"
              class="form-input"
              @change="filterTransactions"
            />
          </div>
          <div class="flex items-end">
            <button @click="refreshTransactions" class="btn-secondary w-full">
              <Icon name="heroicons:arrow-path" class="h-4 w-4 mr-2" />
              Refresh
            </button>
          </div>
        </div>
      </div>

      <!-- Transactions List -->
      <div class="bg-white rounded-lg shadow-sm">
        <div class="px-6 py-4 border-b border-gray-200">
          <h2 class="text-lg font-semibold text-gray-900">Transactions</h2>
        </div>
        
        <div v-if="loading" class="p-8 text-center">
          <Icon name="heroicons:arrow-path" class="h-8 w-8 animate-spin mx-auto text-blue-600" />
          <p class="mt-2 text-gray-600">Loading transactions...</p>
        </div>

        <div v-else-if="filteredTransactions.length === 0" class="p-8 text-center">
          <Icon name="heroicons:document-text" class="h-12 w-12 mx-auto text-gray-400" />
          <p class="mt-2 text-gray-600">No transactions found</p>
        </div>

        <div v-else class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Transaction
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Customer
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Amount
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Status
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Date
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="transaction in filteredTransactions" :key="transaction.id" class="hover:bg-gray-50">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm font-medium text-gray-900">
                    {{ transaction.transactionNumber }}
                  </div>
                  <div class="text-sm text-gray-500">
                    {{ transaction.payments?.[0]?.paymentMethod || 'N/A' }}
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-gray-900">
                    {{ getCustomerName(transaction.customerId) || 'Walk-in Customer' }}
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm font-medium text-gray-900">
                    Rp {{ formatCurrency(transaction.finalAmount) }}
                  </div>
                  <div v-if="transaction.discountAmount > 0" class="text-sm text-green-600">
                    -Rp {{ formatCurrency(transaction.discountAmount) }} discount
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span :class="getStatusBadgeClass(transaction.status)" class="px-2 py-1 text-xs font-semibold rounded-full">
                    {{ transaction.status }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ formatDate(transaction.transactionDate) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2">
                  <button
                    @click="viewTransaction(transaction)"
                    class="text-blue-600 hover:text-blue-900"
                  >
                    <Icon name="heroicons:eye" class="h-4 w-4" />
                  </button>
                  <button
                    v-if="transaction.status === 'Held'"
                    @click="resumeTransaction(transaction)"
                    class="text-green-600 hover:text-green-900"
                  >
                    <Icon name="heroicons:play" class="h-4 w-4" />
                  </button>
                  <button
                    v-if="transaction.status === 'Completed'"
                    @click="printReceipt(transaction.id)"
                    class="text-purple-600 hover:text-purple-900"
                  >
                    <Icon name="heroicons:printer" class="h-4 w-4" />
                  </button>
                  <button
                    v-if="transaction.status === 'Completed'"
                    @click="refundTransaction(transaction)"
                    class="text-red-600 hover:text-red-900"
                  >
                    <Icon name="heroicons:arrow-uturn-left" class="h-4 w-4" />
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </main>

    <!-- Transaction Details Modal -->
    <div v-if="selectedTransaction" class="fixed inset-0 modal-backdrop flex items-center justify-center z-50">
      <div class="bg-white rounded-lg shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="px-6 py-4 border-b border-gray-200 flex justify-between items-center">
          <h3 class="text-lg font-semibold">Transaction Details</h3>
          <button @click="selectedTransaction = null" class="text-gray-400 hover:text-gray-600">
            <Icon name="heroicons:x-mark" class="h-6 w-6" />
          </button>
        </div>
        
        <div class="p-6">
          <div class="grid grid-cols-2 gap-4 mb-6">
            <div>
              <label class="block text-sm font-medium text-gray-700">Transaction Number</label>
              <p class="mt-1 text-sm text-gray-900">{{ selectedTransaction.transactionNumber }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Status</label>
              <span :class="getStatusBadgeClass(selectedTransaction.status)" class="mt-1 inline-block px-2 py-1 text-xs font-semibold rounded-full">
                {{ selectedTransaction.status }}
              </span>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Customer</label>
              <p class="mt-1 text-sm text-gray-900">{{ getCustomerName(selectedTransaction.customerId) || 'Walk-in Customer' }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Payment Method</label>
              <p class="mt-1 text-sm text-gray-900">{{ selectedTransaction.payments?.[0]?.paymentMethod || 'N/A' }}</p>
            </div>
          </div>

          <div v-if="selectedTransaction.transactionItems" class="mb-6">
            <h4 class="text-md font-semibold mb-3">Items</h4>
            <div class="border rounded-lg overflow-hidden">
              <table class="min-w-full divide-y divide-gray-200">
                <thead class="bg-gray-50">
                  <tr>
                    <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Product</th>
                    <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Qty</th>
                    <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Price</th>
                    <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Total</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-gray-200">
                  <tr v-for="item in selectedTransaction.transactionItems" :key="item.productId">
                    <td class="px-4 py-2 text-sm text-gray-900">{{ item.product?.productName || 'Unknown Product' }}</td>
                    <td class="px-4 py-2 text-sm text-gray-900">{{ item.quantity }}</td>
                    <td class="px-4 py-2 text-sm text-gray-900">Rp {{ formatCurrency(item.unitPrice) }}</td>
                    <td class="px-4 py-2 text-sm text-gray-900">Rp {{ formatCurrency(item.unitPrice * item.quantity) }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <div class="border-t pt-4">
            <div class="flex justify-between text-sm">
              <span>Subtotal:</span>
              <span>Rp {{ formatCurrency(selectedTransaction.totalAmount) }}</span>
            </div>
            <div v-if="selectedTransaction.discountAmount > 0" class="flex justify-between text-sm text-green-600">
              <span>Discount:</span>
              <span>-Rp {{ formatCurrency(selectedTransaction.discountAmount) }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span>Tax:</span>
              <span>Rp {{ formatCurrency(selectedTransaction.taxAmount) }}</span>
            </div>
            <div class="flex justify-between text-lg font-semibold border-t pt-2">
              <span>Total:</span>
              <span>Rp {{ formatCurrency(selectedTransaction.finalAmount) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Transaction {
  id: number
  transactionNumber: string
  storeId: number
  customerId?: number | null
  userId: number
  totalAmount: number
  discountAmount: number
  taxAmount: number
  finalAmount: number
  status: string
  paymentMethod: string
  amountReceived?: number
  changeAmount?: number
  transactionDate: string
  createdAt: string
  updatedAt: string
  transactionItems?: TransactionItem[],
  payments?: Payment[],
}

interface Payment{
   id: number
   paymentMethod: string
   receivedAmount: number
   changeAmount: number
   amount: number
   status: string
   paymentDate: string
}

interface TransactionItem {
  productId: number
  unitPrice: number
  quantity: number
  product?: {
    productName: string
    productCode: string
  }
}

interface Customer {
  id: number
  firstName: string
  lastName: string
  phone: string
}

definePageMeta({
  middleware: 'auth'
})

const { user, logout } = useAuth()
const config = useRuntimeConfig()

// Reactive data
const transactions = ref<Transaction[]>([])
const customers = ref<Customer[]>([])
const loading = ref(false)
const searchTerm = ref('')
const statusFilter = ref('')
const dateFilter = ref('')
const selectedTransaction = ref<Transaction | null>(null)

// Computed
const filteredTransactions = computed(() => {
  let filtered = transactions.value

  if (searchTerm.value) {
    const search = searchTerm.value.toLowerCase()
    filtered = filtered.filter(t => 
      t.transactionNumber.toLowerCase().includes(search) ||
      getCustomerName(t.customerId)?.toLowerCase().includes(search)
    )
  }

  if (statusFilter.value) {
    filtered = filtered.filter(t => t.status === statusFilter.value)
  }

  if (dateFilter.value) {
    const filterDate = new Date(dateFilter.value).toDateString()
    filtered = filtered.filter(t => new Date(t.transactionDate).toDateString() === filterDate)
  }

  return filtered.sort((a, b) => new Date(b.transactionDate).getTime() - new Date(a.transactionDate).getTime())
})

// Methods
const handleLogout = async () => {
  await logout()
}

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('id-ID').format(amount)
}

const formatDate = (dateString: string): string => {
  return new Date(dateString).toLocaleDateString('id-ID', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const getStatusBadgeClass = (status: string): string => {
  switch (status) {
    case 'Completed':
      return 'bg-green-100 text-green-800'
    case 'Pending':
      return 'bg-yellow-100 text-yellow-800'
    case 'Held':
      return 'bg-blue-100 text-blue-800'
    case 'Refunded':
      return 'bg-red-100 text-red-800'
    default:
      return 'bg-gray-100 text-gray-800'
  }
}

const getCustomerName = (customerId?: number | null): string | null => {
  if (!customerId) return null
  const customer = customers.value.find(c => c.id === customerId)
  return customer ? `${customer.firstName} ${customer.lastName}` : null
}

const loadTransactions = async () => {
  try {
    loading.value = true
    const { token } = useAuth()

    if (!user.value?.storeId) {
      throw new Error('User store ID not found')
    }

    const response = await $fetch<Transaction[]>(`/api/transaction/by-store/${user.value.storeId}`, {
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })

    transactions.value = response
  } catch (error) {
    console.error('Failed to load transactions:', error)
  } finally {
    loading.value = false
  }
}

const loadCustomers = async () => {
  try {
    const { token } = useAuth()

    const response = await $fetch<Customer[]>('/api/customer', {
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })

    customers.value = response
  } catch (error) {
    console.error('Failed to load customers:', error)
  }
}

const searchTransactions = () => {
  // Filtering is handled by computed property
}

const filterTransactions = () => {
  // Filtering is handled by computed property
}

const refreshTransactions = async () => {
  await loadTransactions()
}

const viewTransaction = async (transaction: Transaction) => {
  try {
    const { token } = useAuth()

    const fullTransaction = await $fetch<Transaction>(`/api/transaction/${transaction.id}`, {
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })

    selectedTransaction.value = fullTransaction
  } catch (error) {
    console.error('Failed to load transaction details:', error)
    alert('Failed to load transaction details')
  }
}

const resumeTransaction = async (transaction: Transaction) => {
  try {
    const { token } = useAuth()

    await $fetch(`/api/transaction/${transaction.id}/resume`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })

    // Navigate to POS with transaction resumed
    await navigateTo('/pos')
  } catch (error) {
    console.error('Failed to resume transaction:', error)
    alert('Failed to resume transaction')
  }
}

const printReceipt = async (transactionId: number) => {
  try {
    const { token } = useAuth()

    const response = await $fetch(`/api/print/receipt/${transactionId}`, {
      method: 'POST',
      body: {
        format: 'PDF'
      },
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })

    if (response && typeof response === 'object' && 'filePath' in response) {
      const printWindow = window.open(`${config.public.apiBase}${response.filePath}`, '_blank')
      if (printWindow) {
        printWindow.focus()
        printWindow.onload = () => {
          printWindow.print()
        }
      }
    }
  } catch (error) {
    console.error('Failed to print receipt:', error)
    alert('Failed to print receipt')
  }
}

const refundTransaction = async (transaction: Transaction) => {
  if (!confirm(`Are you sure you want to refund transaction ${transaction.transactionNumber}?`)) {
    return
  }

  try {
    const { token } = useAuth()

    await $fetch(`/api/transaction/${transaction.id}/refund`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })

    alert('Transaction refunded successfully!')
    await loadTransactions()
  } catch (error) {
    console.error('Failed to refund transaction:', error)
    alert('Failed to refund transaction')
  }
}

// Initialize
onMounted(async () => {
  await Promise.all([
    loadTransactions(),
    loadCustomers()
  ])
})
</script>