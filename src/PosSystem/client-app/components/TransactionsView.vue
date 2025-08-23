<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex justify-between items-center">
      <h2 class="text-2xl font-bold text-black">Transactions</h2>
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
          <label class="block text-sm font-medium text-black mb-1">Search</label>
          <input
            v-model="searchTerm"
            type="text"
            placeholder="Transaction number, customer..."
            class="form-input"
            @input="searchTransactions"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-black mb-1">Status</label>
          <select v-model="statusFilter" class="form-select" @change="filterTransactions">
            <option value="">All Status</option>
            <option value="Completed">Completed</option>
            <option value="Pending">Pending</option>
            <option value="Hold">Held</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-black mb-1">From Date</label>
          <input
              v-model="fromDate"
              type="date"
              class="form-input"
              @change="loadTransactions"
            />
        </div>
        <div>
          <label class="block text-sm font-medium text-black mb-1">To Date</label>
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
        <table class="min-w-full divide-y divide-neutral-medium">
          <thead class="bg-neutral-light">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-neutral-gray uppercase tracking-wider">
                Transaction
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-neutral-gray uppercase tracking-wider">
                Customer
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-neutral-gray uppercase tracking-wider">
                Amount
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-neutral-gray uppercase tracking-wider">
                Status
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-neutral-gray uppercase tracking-wider">
                Date
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-neutral-gray uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-neutral-medium">
            <tr v-for="transaction in filteredTransactions" :key="transaction.id" class="hover:bg-neutral-light">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-black">{{ transaction.transactionNumber }}</div>
                <div class="text-sm text-neutral-gray">{{ transaction.payments?.map(p => p.paymentMethod).join(', ') || 'N/A' }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-black">
                  {{ transaction.customer ? `${transaction.customer.firstName} ${transaction.customer.lastName}` : 'Walk-in' }}
                </div>
                <div class="text-sm text-neutral-gray">
                  {{ transaction.customer?.phone || '' }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-black">{{ formatCurrency(transaction.subTotal) }}</div>
                <div class="text-sm text-neutral-gray">
                  Discount: {{ formatCurrency(transaction.discountAmount) }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="getStatusBadgeClass(transaction.status)" class="px-2 py-1 text-xs font-semibold rounded-full">
                  {{ transaction.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-black">
                {{ formatDate(transaction.transactionDate) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium space-x-2">
                <button
                  @click="viewTransaction(transaction)"
                  class="text-primary hover:text-black"
                >
                  View
                </button>
                <button
                  v-if="transaction.status === 'Hold'"
                  @click="resumeTransaction(transaction.id)"
                  class="text-primary hover:text-black"
                >
                  Resume
                </button>
                <button
                  v-if="transaction.status === 'Completed'"
                  @click="printReceipt(transaction.id)"
                  class="text-primary hover:text-black"
                >
                  Print
                </button>
                <button
                  v-if="transaction.status === 'Completed'"
                  @click="showRefundModal(transaction)"
                  class="text-danger hover:text-black"
                >
                  Refund
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
          <h3 class="text-lg font-bold text-black">Transaction Details</h3>
          <button @click="selectedTransaction = null" class="text-neutral-gray hover:text-black">
            <Icon name="heroicons:x-mark" class="h-6 w-6" />
          </button>
        </div>
        
        <div class="space-y-4">
          <!-- Transaction Info -->
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-black">Transaction Number</label>
              <p class="text-sm text-black">{{ selectedTransaction.transactionNumber }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-black">Status</label>
              <span :class="getStatusBadgeClass(selectedTransaction.status)" class="px-2 py-1 text-xs font-semibold rounded-full">
                {{ selectedTransaction.status }}
              </span>
            </div>
            <div>
              <label class="block text-sm font-medium text-black">Customer</label>
              <p class="text-sm text-black">
                {{ selectedTransaction.customer ? `${selectedTransaction.customer.firstName} ${selectedTransaction.customer.lastName}` : 'Walk-in' }}
              </p>
            </div>
            <div>
              <label class="block text-sm font-medium text-black">Payment Method</label>
              <p class="text-sm text-black">{{ selectedTransaction.paymentMethod || 'N/A' }}</p>
            </div>
          </div>

          <!-- Items -->
          <div>
            <label class="block text-sm font-medium text-black mb-2">Items</label>
            <div class="border border-neutral-medium rounded-lg overflow-hidden">
              <table class="min-w-full divide-y divide-neutral-medium">
                <thead class="bg-neutral-light">
                  <tr>
                    <th class="px-4 py-2 text-left text-xs font-medium text-neutral-gray uppercase">Product</th>
                    <th class="px-4 py-2 text-left text-xs font-medium text-neutral-gray uppercase">Qty</th>
                    <th class="px-4 py-2 text-left text-xs font-medium text-neutral-gray uppercase">Price</th>
                    <th class="px-4 py-2 text-left text-xs font-medium text-neutral-gray uppercase">Total</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-neutral-medium">
                  <tr v-for="item in selectedTransaction.items || selectedTransaction.transactionItems" :key="item.id">
                    <td class="px-4 py-2 text-sm text-black">
                      {{ item.product?.productName || 'Unknown Product' }}
                    </td>
                    <td class="px-4 py-2 text-sm text-black">{{ item.quantity }}</td>
                    <td class="px-4 py-2 text-sm text-black">{{ formatCurrency(item.unitPrice) }}</td>
                    <td class="px-4 py-2 text-sm text-black">{{ formatCurrency(item.unitPrice * item.quantity) }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- Totals -->
          <div class="border-t border-neutral-medium pt-4">
            <div class="flex justify-between text-sm text-black">
              <span>Subtotal:</span>
              <span>{{ formatCurrency(selectedTransaction.subTotal) }}</span>
            </div>
            <div class="flex justify-between text-sm text-black">
              <span>Discount:</span>
              <span>-{{ formatCurrency(selectedTransaction.discountAmount) }}</span>
            </div>
            <div class="flex justify-between text-sm text-black">
              <span>Tax:</span>
              <span>{{ formatCurrency(selectedTransaction.taxAmount) }}</span>
            </div>
            <div class="flex justify-between text-lg font-bold text-black border-t border-neutral-medium pt-2">
              <span>Total:</span>
              <span>{{ formatCurrency(selectedTransaction.totalAmount) }}</span>
            </div>
          </div>

          <!-- Modal Action Buttons -->
          <div class="flex justify-end space-x-3 pt-4 border-t border-neutral-medium">
            <button
              @click="selectedTransaction = null"
              class="px-4 py-2 text-sm font-medium text-black bg-neutral-light border border-neutral-medium rounded-md hover:bg-neutral-medium focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary"
            >
              Close
            </button>
            <button
              v-if="selectedTransaction.status === 'Hold'"
              @click="resumeTransaction(selectedTransaction.id)"
              class="px-4 py-2 text-sm font-medium text-black bg-primary border border-transparent rounded-md hover:bg-primary focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary"
            >
              <Icon name="heroicons:play" class="h-4 w-4 mr-2 inline" />
              Resume
            </button>
            <button
              v-if="selectedTransaction.status === 'Completed'"
              @click="printReceipt(selectedTransaction.id)"
              class="px-4 py-2 text-sm font-medium text-black bg-primary border border-transparent rounded-md hover:bg-primary focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary"
            >
              <Icon name="heroicons:printer" class="h-4 w-4 mr-2 inline" />
              Print Receipt
            </button>
            <button
              v-if="selectedTransaction.status === 'Completed'"
              @click="showRefundModal(selectedTransaction)"
              class="px-4 py-2 text-sm font-medium text-white bg-danger border border-transparent rounded-md hover:bg-danger focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-danger"
            >
              <Icon name="heroicons:arrow-uturn-left" class="h-4 w-4 mr-2 inline" />
              Refund
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Refund Modal -->
    <div v-if="showRefund" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-11/12 md:w-1/2 lg:w-1/3 shadow-lg rounded-md bg-white">
        <div class="flex justify-between items-center mb-4">
          <h3 class="text-lg font-bold text-black">Refund Transaction</h3>
          <button @click="closeRefundModal" class="text-neutral-gray hover:text-black">
            <Icon name="heroicons:x-mark" class="h-6 w-6" />
          </button>
        </div>
        
        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-black mb-1">Transaction Number</label>
            <p class="text-sm text-black font-medium">{{ refundTransaction?.transactionNumber }}</p>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-black mb-1">Total Amount</label>
            <p class="text-sm text-black font-medium">{{ formatCurrency(refundTransaction?.totalAmount || 0) }}</p>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-black mb-1">Refund Reason *</label>
            <textarea
              v-model="refundReason"
              rows="3"
              class="form-input w-full"
              placeholder="Please provide a reason for the refund..."
              required
            ></textarea>
          </div>
          
          <div class="flex justify-end space-x-3 pt-4 border-t border-neutral-medium">
            <button
              @click="closeRefundModal"
              class="px-4 py-2 text-sm font-medium text-black bg-neutral-light border border-neutral-medium rounded-md hover:bg-neutral-medium focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary"
            >
              Cancel
            </button>
            <button
              @click="processRefund"
              :disabled="!refundReason.trim() || isProcessingRefund"
              class="px-4 py-2 text-sm font-medium text-white bg-danger border border-transparent rounded-md hover:bg-danger focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-danger disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <Icon v-if="isProcessingRefund" name="heroicons:arrow-path" class="h-4 w-4 mr-2 inline animate-spin" />
              {{ isProcessingRefund ? 'Processing...' : 'Process Refund' }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Receipt Print Component -->
    <ReceiptPrint
      v-if="printingTransaction"
      ref="receiptPrintRef"
      :transaction="printingTransaction"
      :store-info="printingTransaction.store"
      :user-info="printingTransaction.user"
      @close="onReceiptClosed"
      @printed="onReceiptPrinted"
    />
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
  paymentMethod?: string | undefined,
  payments?: Payment[],
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

interface Payment {
  id?: number
  transactionId: number
  paymentMethod: string
  amount: number
  paymentDate: string,
  status: string
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
const showRefund = ref(false)
const refundTransaction = ref<Transaction | null>(null)
const refundReason = ref('')
const isProcessingRefund = ref(false)

// Print functionality
const printingTransaction = ref<Transaction | null>(null)
const receiptPrintRef = ref()
const storeInfo = ref({
  name: 'Your Store Name',
  address: '123 Main Street, City, State 12345',
  phone: '+1 (555) 123-4567',
  email: 'info@yourstore.com'
})

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
    case 'Hold':
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
        fromDate.value = thirtyDaysAgo.toISOString().split('T')[0] || ''
      }
      if (!toDate.value) {
        toDate.value = today.toISOString().split('T')[0] || ''
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
    console.log('Loaded transaction details:', fullTransaction)
    //get paymentMethod from payments list property paymentMethod
    if (fullTransaction.payments) {
      fullTransaction.paymentMethod = fullTransaction.payments.map(p => p.paymentMethod).join(', ')
    }
    selectedTransaction.value.paymentMethod = fullTransaction.paymentMethod;
  } catch (error) {
    console.error('Failed to load transaction details:', error)
  }
}

const printReceipt = async (transactionId: number) => {
  console.log("print clicked")
  try {
    // Find the transaction
    const transaction = transactions.value.find(t => t.id === transactionId)
    if (!transaction) {
      alert('Transaction not found')
      return
    }

    // Load full transaction details if needed
    await viewTransaction(transaction)
    
    if (selectedTransaction.value) {
      // Ensure paymentMethod is set
      if (selectedTransaction.value.payments && !selectedTransaction.value.paymentMethod) {
        selectedTransaction.value.paymentMethod = selectedTransaction.value.payments.map(p => p.paymentMethod).join(', ')
      }
      printingTransaction.value = selectedTransaction.value
      // Open print preview
      if (receiptPrintRef.value) {
        receiptPrintRef.value.openPreview()
      }
    }
  } catch (error) {
    console.error('Failed to prepare receipt for printing:', error)
    alert('Failed to prepare receipt for printing')
  }
}

const resumeTransaction = async (transactionId: number) => {
  try {
    const { token } = useAuth()
    
    // Get the full transaction details with items
    const fullTransaction = await $fetch<Transaction>(`/api/transaction/${transactionId}`, {
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })
    
    // Resume the transaction on the server
    const response = await $fetch<Transaction>(`/api/transaction/${transactionId}/resume`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })
    
    // Update the transaction in the list
    const index = transactions.value.findIndex(t => t.id === transactionId)
    if (index !== -1) {
      transactions.value[index] = response
    }
    
    // Update filtered transactions
    filterTransactions()
    
    // Update selected transaction if it's the same one
    if (selectedTransaction.value?.id === transactionId) {
      selectedTransaction.value = response
    }
    
    // Store transaction data for POS to load
    const transactionData = {
      transaction: fullTransaction,
      shouldResume: true
    }
    
    // Store in sessionStorage so POS can pick it up
    sessionStorage.setItem('resumeTransaction', JSON.stringify(transactionData))
    
    // Navigate to POS page
    await navigateTo('/pos')
    
    alert('Transaction resumed successfully!')
  } catch (error) {
    console.error('Failed to resume transaction:', error)
    alert('Failed to resume transaction')
  }
}

const showRefundModal = (transaction: Transaction) => {
  refundTransaction.value = transaction
  refundReason.value = ''
  showRefund.value = true
}

const closeRefundModal = () => {
  showRefund.value = false
  refundTransaction.value = null
  refundReason.value = ''
  isProcessingRefund.value = false
}

const processRefund = async () => {
  if (!refundTransaction.value || !refundReason.value.trim()) {
    return
  }
  
  isProcessingRefund.value = true
  
  try {
    const { token } = useAuth()
    
    const response = await $fetch<Transaction>(`/api/transaction/${refundTransaction.value.id}/refund`, {
      method: 'POST',
      body: {
        reason: refundReason.value.trim()
      },
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })
    
    // Update the transaction in the list
    const index = transactions.value.findIndex(t => t.id === refundTransaction.value!.id)
    if (index !== -1) {
      transactions.value[index] = response
    }
    
    // Update filtered transactions
    filterTransactions()
    
    // Update selected transaction if it's the same one
    if (selectedTransaction.value?.id === refundTransaction.value.id) {
      selectedTransaction.value = response
    }
    
    closeRefundModal()
    alert('Transaction refunded successfully!')
  } catch (error) {
    console.error('Failed to process refund:', error)
    alert('Failed to process refund')
  } finally {
    isProcessingRefund.value = false
  }
}

// Print event handlers
const onReceiptPrinted = () => {
  alert('Receipt printed successfully!')
  printingTransaction.value = null
}

const onReceiptClosed = () => {
  printingTransaction.value = null
}

// Initialize
onMounted(async () => {
  await loadTransactions()
})
</script>