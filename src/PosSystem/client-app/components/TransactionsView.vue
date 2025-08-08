<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-gray-900">Transactions</h2>
      <div class="flex space-x-3">
        <button @click="refreshTransactions" class="btn-secondary">
          <Icon name="heroicons:arrow-path" class="h-4 w-4 mr-2" />
          Refresh
        </button>
      </div>
    </div>

    <!-- Search and Filter -->
    <div class="bg-white rounded-lg shadow-sm p-4">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Search</label>
          <input
            v-model="searchTerm"
            type="text"
            placeholder="Transaction number, customer..."
            class="form-input"
            @input="searchTransactions"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Status</label>
          <select v-model="statusFilter" class="form-select" @change="filterTransactions">
            <option value="">All Status</option>
            <option value="Completed">Completed</option>
            <option value="Pending">Pending</option>
            <option value="Held">Held</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">From Date</label>
          <input
              v-model="fromDate"
              type="date"
              class="form-input"
              @change="loadTransactions"
            />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">To Date</label>
          <input
              v-model="toDate"
              type="date"
              class="form-input"
              @change="loadTransactions"
            />
        </div>
      </div>
    </div>

    <!-- Transactions List -->
    <div class="bg-white rounded-lg shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
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
                <div class="text-sm font-medium text-gray-900">{{ transaction.transactionNumber }}</div>
                <div class="text-sm text-gray-500">{{ transaction.paymentMethod || 'N/A' }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900">
                  {{ transaction.customer ? `${transaction.customer.firstName} ${transaction.customer.lastName}` : 'Walk-in' }}
                </div>
                <div class="text-sm text-gray-500">
                  {{ transaction.customer?.phone || '' }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-gray-900">{{ formatCurrency(transaction.subTotal) }}</div>
                <div class="text-sm text-gray-500">
                  Discount: {{ formatCurrency(transaction.discountAmount) }}
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
                  View
                </button>
                <button
                  v-if="transaction.status === 'Completed'"
                  @click="printReceipt(transaction.id)"
                  class="text-green-600 hover:text-green-900"
                >
                  Print
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Transaction Details Modal -->
    <div v-if="selectedTransaction" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-11/12 md:w-3/4 lg:w-1/2 shadow-lg rounded-md bg-white">
        <div class="flex justify-between items-center mb-4">
          <h3 class="text-lg font-bold text-gray-900">Transaction Details</h3>
          <button @click="selectedTransaction = null" class="text-gray-400 hover:text-gray-600">
            <Icon name="heroicons:x-mark" class="h-6 w-6" />
          </button>
        </div>
        
        <div class="space-y-4">
          <!-- Transaction Info -->
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700">Transaction Number</label>
              <p class="text-sm text-gray-900">{{ selectedTransaction.transactionNumber }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Status</label>
              <span :class="getStatusBadgeClass(selectedTransaction.status)" class="px-2 py-1 text-xs font-semibold rounded-full">
                {{ selectedTransaction.status }}
              </span>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Customer</label>
              <p class="text-sm text-gray-900">
                {{ selectedTransaction.customer ? `${selectedTransaction.customer.firstName} ${selectedTransaction.customer.lastName}` : 'Walk-in' }}
              </p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Payment Method</label>
              <p class="text-sm text-gray-900">{{ selectedTransaction.paymentMethod || 'N/A' }}</p>
            </div>
          </div>

          <!-- Items -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Items</label>
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
                  <tr v-for="item in selectedTransaction.items || selectedTransaction.transactionItems" :key="item.id">
                    <td class="px-4 py-2 text-sm text-gray-900">
                      {{ item.product?.productName || 'Unknown Product' }}
                    </td>
                    <td class="px-4 py-2 text-sm text-gray-900">{{ item.quantity }}</td>
                    <td class="px-4 py-2 text-sm text-gray-900">{{ formatCurrency(item.unitPrice) }}</td>
                    <td class="px-4 py-2 text-sm text-gray-900">{{ formatCurrency(item.unitPrice * item.quantity) }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- Totals -->
          <div class="border-t pt-4">
            <div class="flex justify-between text-sm">
              <span>Subtotal:</span>
              <span>{{ formatCurrency(selectedTransaction.subTotal) }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span>Discount:</span>
              <span>-{{ formatCurrency(selectedTransaction.discountAmount) }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span>Tax:</span>
              <span>{{ formatCurrency(selectedTransaction.taxAmount) }}</span>
            </div>
            <div class="flex justify-between text-lg font-bold border-t pt-2">
              <span>Total:</span>
              <span>{{ formatCurrency(selectedTransaction.totalAmount) }}</span>
            </div>
          </div>

          <!-- Modal Action Buttons -->
          <div class="flex justify-end space-x-3 pt-4 border-t">
            <button
              @click="selectedTransaction = null"
              class="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 border border-gray-300 rounded-md hover:bg-gray-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"
            >
              Close
            </button>
            <button
              v-if="selectedTransaction.status === 'Completed'"
              @click="printReceipt(selectedTransaction.id)"
              class="px-4 py-2 text-sm font-medium text-white bg-green-600 border border-transparent rounded-md hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
            >
              <Icon name="heroicons:printer" class="h-4 w-4 mr-2 inline" />
              Print Receipt
            </button>
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
  subTotal: number,
  discountAmount: number
  taxAmount: number
  finalAmount: number
  status: string
  paymentMethod?: string
  amountReceived?: number
  changeAmount?: number
  transactionDate: string
  createdAt: string
  updatedAt: string
  customer?: {
    id: number
    firstName: string
    lastName: string
    phone: string
  }
  items?: TransactionItem[]
  transactionItems?: TransactionItem[]
}

interface TransactionItem {
  id?: number
  productId: number
  unitPrice: number
  quantity: number
  product?: {
    id: number
    productName: string
    productCode: string
  }
}

const { user } = useAuth()
const config = useRuntimeConfig()

// Reactive data
const transactions = ref<Transaction[]>([])
const filteredTransactions = ref<Transaction[]>([])
const selectedTransaction = ref<Transaction | null>(null)
const searchTerm = ref('')
const statusFilter = ref('')
const fromDate = ref('')
const toDate = ref('')

// Methods
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
    case 'Cancelled':
      return 'bg-red-100 text-red-800'
    default:
      return 'bg-gray-100 text-gray-800'
  }
}

const loadTransactions = async () => {
  try {
    const { token } = useAuth()
    
    if (!user.value?.storeId) {
      return
    }
    
    // Set default date range if not already set
    if (!fromDate.value || !toDate.value) {
      const today = new Date()
      const thirtyDaysAgo = new Date(today)
      thirtyDaysAgo.setDate(today.getDate() - 30)
      
      if (!fromDate.value) {
        fromDate.value = thirtyDaysAgo.toISOString().split('T')[0]
      }
      if (!toDate.value) {
        toDate.value = today.toISOString().split('T')[0]
      }
    }
    
    const params = new URLSearchParams({
      startDate: fromDate.value,
      endDate: toDate.value,
      storeId: user.value.storeId.toString()
    })
    
    const response = await $fetch<Transaction[]>(`/api/transaction/by-date-range?${params.toString()}`, {
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })
    
    transactions.value = response
    filteredTransactions.value = response

    console.log('Loaded transactions:', response);
  } catch (error) {
    console.error('Failed to load transactions:', error)
  }
}

const refreshTransactions = async () => {
  // Always reload with current date range
  await loadTransactions()
}

const searchTransactions = () => {
  filterTransactions()
}

const filterTransactions = () => {
  let filtered = transactions.value

  // Filter by search term
  if (searchTerm.value.trim()) {
    const term = searchTerm.value.toLowerCase()
    filtered = filtered.filter(transaction =>
      transaction.transactionNumber.toLowerCase().includes(term) ||
      (transaction.customer && 
        (`${transaction.customer.firstName} ${transaction.customer.lastName}`.toLowerCase().includes(term) ||
         transaction.customer.phone.includes(term)))
    )
  }

  // Filter by status
  if (statusFilter.value) {
    filtered = filtered.filter(transaction => transaction.status === statusFilter.value)
  }

  // Note: Date filtering is now handled by the API call in loadTransactions()
  // The fromDate and toDate filters are used to reload data from the server

  filteredTransactions.value = filtered
}

const viewTransaction = async (transaction: Transaction) => {
  try {
    const { token } = useAuth()
    
    // Get full transaction details with items
    const fullTransaction = await $fetch<Transaction>(`/api/transaction/${transaction.id}`, {
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })
    
    selectedTransaction.value = fullTransaction
  } catch (error) {
    console.error('Failed to load transaction details:', error)
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

// Initialize
onMounted(async () => {
  await loadTransactions()
})
</script>