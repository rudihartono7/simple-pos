<template>
  <div class="min-h-screen bg-gray-100">
    <!-- Navigation -->
    <nav class="bg-white shadow-sm">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex items-center">
            <NuxtLink to="/dashboard" class="text-xl font-semibold text-gray-900">POS System</NuxtLink>
          </div>
          <div class="flex items-center space-x-4">
            <span class="text-sm text-gray-700">
              Welcome, {{ user?.firstName }} {{ user?.lastName }}
            </span>
            <button
              @click="handleLogout"
              class="bg-red-600 hover:bg-red-700 text-white px-3 py-2 rounded-md text-sm font-medium"
            >
              Logout
            </button>
          </div>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main class="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
      <div class="px-4 py-6 sm:px-0">
        <!-- Header -->
        <div class="mb-6">
          <h1 class="text-2xl font-bold text-gray-900">Print Management</h1>
          <p class="text-gray-600">Generate and manage receipts and bills</p>
        </div>

        <!-- Print Options -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
          <!-- Single Receipt -->
          <div class="bg-white p-6 rounded-lg shadow-md">
            <div class="flex items-center mb-4">
              <svg class="h-8 w-8 text-blue-600 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
              </svg>
              <h3 class="text-lg font-medium text-gray-900">Single Receipt</h3>
            </div>
            <p class="text-sm text-gray-600 mb-4">Generate receipt for a specific transaction</p>
            <div class="space-y-3">
              <input
                v-model="singleTransactionId"
                type="number"
                placeholder="Transaction ID"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
              />
              <div class="flex space-x-2">
                <button
                  @click="generateSingleReceipt('pdf')"
                  :disabled="!singleTransactionId || loading"
                  class="flex-1 bg-blue-600 hover:bg-blue-700 text-white px-3 py-2 rounded-md text-sm font-medium disabled:opacity-50"
                >
                  PDF
                </button>
                <button
                  @click="generateSingleReceipt('html')"
                  :disabled="!singleTransactionId || loading"
                  class="flex-1 bg-green-600 hover:bg-green-700 text-white px-3 py-2 rounded-md text-sm font-medium disabled:opacity-50"
                >
                  HTML
                </button>
              </div>
            </div>
          </div>

          <!-- Single Bill -->
          <div class="bg-white p-6 rounded-lg shadow-md">
            <div class="flex items-center mb-4">
              <svg class="h-8 w-8 text-purple-600 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
              </svg>
              <h3 class="text-lg font-medium text-gray-900">Single Bill</h3>
            </div>
            <p class="text-sm text-gray-600 mb-4">Generate bill for a specific transaction</p>
            <div class="space-y-3">
              <input
                v-model="singleBillTransactionId"
                type="number"
                placeholder="Transaction ID"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
              />
              <div class="flex space-x-2">
                <button
                  @click="generateSingleBill('pdf')"
                  :disabled="!singleBillTransactionId || loading"
                  class="flex-1 bg-purple-600 hover:bg-purple-700 text-white px-3 py-2 rounded-md text-sm font-medium disabled:opacity-50"
                >
                  PDF
                </button>
                <button
                  @click="generateSingleBill('html')"
                  :disabled="!singleBillTransactionId || loading"
                  class="flex-1 bg-indigo-600 hover:bg-indigo-700 text-white px-3 py-2 rounded-md text-sm font-medium disabled:opacity-50"
                >
                  HTML
                </button>
              </div>
            </div>
          </div>

          <!-- Multiple Receipts -->
          <div class="bg-white p-6 rounded-lg shadow-md">
            <div class="flex items-center mb-4">
              <svg class="h-8 w-8 text-orange-600 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7v8a2 2 0 002 2h6M8 7V5a2 2 0 012-2h4.586a1 1 0 01.707.293l4.414 4.414a1 1 0 01.293.707V15a2 2 0 01-2 2v0a2 2 0 01-2-2V9a2 2 0 00-2-2H8z"></path>
              </svg>
              <h3 class="text-lg font-medium text-gray-900">Multiple Receipts</h3>
            </div>
            <p class="text-sm text-gray-600 mb-4">Generate receipts for multiple transactions</p>
            <div class="space-y-3">
              <textarea
                v-model="multipleTransactionIds"
                placeholder="Transaction IDs (comma separated)"
                rows="3"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
              ></textarea>
              <button
                @click="generateMultipleReceipts"
                :disabled="!multipleTransactionIds || loading"
                class="w-full bg-orange-600 hover:bg-orange-700 text-white px-3 py-2 rounded-md text-sm font-medium disabled:opacity-50"
              >
                Generate PDF
              </button>
            </div>
          </div>
        </div>

        <!-- Recent Transactions -->
        <div class="bg-white shadow overflow-hidden sm:rounded-md mb-6">
          <div class="px-4 py-5 sm:px-6 border-b border-gray-200">
            <div class="flex justify-between items-center">
              <div>
                <h3 class="text-lg leading-6 font-medium text-gray-900">Recent Transactions</h3>
                <p class="mt-1 max-w-2xl text-sm text-gray-500">Quick access to recent transactions for printing</p>
              </div>
              <button
                @click="loadRecentTransactions"
                :disabled="loading"
                class="bg-gray-600 hover:bg-gray-700 text-white px-4 py-2 rounded-md text-sm font-medium disabled:opacity-50"
              >
                Refresh
              </button>
            </div>
          </div>
          
          <!-- Loading State -->
          <div v-if="loading" class="text-center py-8">
            <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
            <p class="mt-2 text-gray-600">Loading transactions...</p>
          </div>

          <!-- Transactions List -->
          <ul v-else class="divide-y divide-gray-200">
            <li v-for="transaction in recentTransactions" :key="transaction.id" class="px-6 py-4">
              <div class="flex items-center justify-between">
                <div class="flex items-center">
                  <div class="flex-shrink-0">
                    <div class="h-10 w-10 rounded-full bg-blue-100 flex items-center justify-center">
                      <svg class="h-5 w-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
                      </svg>
                    </div>
                  </div>
                  <div class="ml-4">
                    <div class="text-sm font-medium text-gray-900">
                      Transaction #{{ transaction.transactionNumber }}
                    </div>
                    <div class="text-sm text-gray-500">
                      {{ formatDate(transaction.transactionDate) }} • 
                      {{ formatCurrency(transaction.totalAmount) }}
                      <span v-if="transaction.customerName"> • {{ transaction.customerName }}</span>
                    </div>
                  </div>
                </div>
                <div class="flex items-center space-x-2">
                  <button
                    @click="generateSingleReceiptFromTransaction(transaction.id, 'pdf')"
                    class="bg-blue-600 hover:bg-blue-700 text-white px-3 py-1 rounded text-sm"
                  >
                    Receipt PDF
                  </button>
                  <button
                    @click="generateSingleReceiptFromTransaction(transaction.id, 'html')"
                    class="bg-green-600 hover:bg-green-700 text-white px-3 py-1 rounded text-sm"
                  >
                    Receipt HTML
                  </button>
                  <button
                    @click="generateSingleBillFromTransaction(transaction.id, 'pdf')"
                    class="bg-purple-600 hover:bg-purple-700 text-white px-3 py-1 rounded text-sm"
                  >
                    Bill PDF
                  </button>
                </div>
              </div>
            </li>
          </ul>

          <!-- Empty State -->
          <div v-if="!loading && recentTransactions.length === 0" class="text-center py-12">
            <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
            </svg>
            <h3 class="mt-2 text-sm font-medium text-gray-900">No transactions found</h3>
            <p class="mt-1 text-sm text-gray-500">No recent transactions available for printing.</p>
          </div>
        </div>

        <!-- Print Preview Modal -->
        <div v-if="showPreviewModal" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
          <div class="relative top-10 mx-auto p-5 border w-full max-w-4xl shadow-lg rounded-md bg-white">
            <div class="mt-3">
              <div class="flex justify-between items-center mb-4">
                <h3 class="text-lg font-medium text-gray-900">Print Preview</h3>
                <button
                  @click="closePreviewModal"
                  class="text-gray-400 hover:text-gray-600"
                >
                  <svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                  </svg>
                </button>
              </div>
              
              <div class="border rounded-lg p-4 bg-gray-50 max-h-96 overflow-auto">
                <div v-html="previewContent"></div>
              </div>
              
              <div class="flex justify-end space-x-3 mt-4">
                <button
                  @click="closePreviewModal"
                  class="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-200 hover:bg-gray-300 rounded-md"
                >
                  Close
                </button>
                <button
                  @click="printPreview"
                  class="px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-md"
                >
                  Print
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  middleware: 'auth'
})

interface Transaction {
  id: number
  transactionNumber: string
  transactionDate: string
  totalAmount: number
  customerName?: string
}

const { user, logout } = useAuth()
const config = useRuntimeConfig()

// Reactive data
const loading = ref(false)
const singleTransactionId = ref<number | null>(null)
const singleBillTransactionId = ref<number | null>(null)
const multipleTransactionIds = ref('')
const recentTransactions = ref<Transaction[]>([])
const showPreviewModal = ref(false)
const previewContent = ref('')

// Methods
const handleLogout = async () => {
  await logout()
}

const generateSingleReceipt = async (format: 'pdf' | 'html') => {
  if (!singleTransactionId.value) return
  
  loading.value = true
  try {
    const token = useCookie('auth-token')
    
    if (format === 'pdf') {
      const response = await fetch(`${config.public.apiBase}/api/Print/receipt/${singleTransactionId.value}`, {
        headers: {
          'Authorization': `Bearer ${token.value || ''}`
        }
      })
      
      if (response.ok) {
        const blob = await response.blob()
        const url = window.URL.createObjectURL(blob)
        const a = document.createElement('a')
        a.href = url
        a.download = `receipt_${singleTransactionId.value}.pdf`
        document.body.appendChild(a)
        a.click()
        window.URL.revokeObjectURL(url)
        document.body.removeChild(a)
      } else {
        throw new Error('Failed to generate receipt')
      }
    } else {
      const html = await $fetch<string>(`${config.public.apiBase}/api/Print/receipt/${singleTransactionId.value}/html`, {
        headers: {
          'Authorization': `Bearer ${token.value || ''}`
        }
      })
      
      previewContent.value = html
      showPreviewModal.value = true
    }
  } catch (error) {
    console.error('Failed to generate receipt:', error)
    alert('Failed to generate receipt')
  } finally {
    loading.value = false
  }
}

const generateSingleBill = async (format: 'pdf' | 'html') => {
  if (!singleBillTransactionId.value) return
  
  loading.value = true
  try {
    const token = useCookie('auth-token')
    
    if (format === 'pdf') {
      const response = await fetch(`${config.public.apiBase}/api/Print/bill/${singleBillTransactionId.value}`, {
        headers: {
          'Authorization': `Bearer ${token.value || ''}`
        }
      })
      
      if (response.ok) {
        const blob = await response.blob()
        const url = window.URL.createObjectURL(blob)
        const a = document.createElement('a')
        a.href = url
        a.download = `bill_${singleBillTransactionId.value}.pdf`
        document.body.appendChild(a)
        a.click()
        window.URL.revokeObjectURL(url)
        document.body.removeChild(a)
      } else {
        throw new Error('Failed to generate bill')
      }
    } else {
      const html = await $fetch<string>(`${config.public.apiBase}/api/Print/bill/${singleBillTransactionId.value}/html`, {
        headers: {
          'Authorization': `Bearer ${token.value || ''}`
        }
      })
      
      previewContent.value = html
      showPreviewModal.value = true
    }
  } catch (error) {
    console.error('Failed to generate bill:', error)
    alert('Failed to generate bill')
  } finally {
    loading.value = false
  }
}

const generateMultipleReceipts = async () => {
  if (!multipleTransactionIds.value) return
  
  loading.value = true
  try {
    const token = useCookie('auth-token')
    const ids = multipleTransactionIds.value.split(',').map(id => parseInt(id.trim())).filter(id => !isNaN(id))
    
    if (ids.length === 0) {
      alert('Please enter valid transaction IDs')
      return
    }
    
    const response = await fetch(`${config.public.apiBase}/api/Print/receipts/multiple`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(ids)
    })
    
    if (response.ok) {
      const blob = await response.blob()
      const url = window.URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = `receipts_${new Date().toISOString().slice(0, 10)}.pdf`
      document.body.appendChild(a)
      a.click()
      window.URL.revokeObjectURL(url)
      document.body.removeChild(a)
    } else {
      throw new Error('Failed to generate receipts')
    }
  } catch (error) {
    console.error('Failed to generate multiple receipts:', error)
    alert('Failed to generate multiple receipts')
  } finally {
    loading.value = false
  }
}

const generateSingleReceiptFromTransaction = async (transactionId: number, format: 'pdf' | 'html') => {
  singleTransactionId.value = transactionId
  await generateSingleReceipt(format)
}

const generateSingleBillFromTransaction = async (transactionId: number, format: 'pdf' | 'html') => {
  singleBillTransactionId.value = transactionId
  await generateSingleBill(format)
}

const loadRecentTransactions = async () => {
  loading.value = true
  try {
    const token = useCookie('auth-token')
    const response = await $fetch<Transaction[]>(`${config.public.apiBase}/api/Transaction`, {
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    
    // Get the most recent 10 transactions
    recentTransactions.value = response.slice(0, 10)
  } catch (error) {
    console.error('Failed to load recent transactions:', error)
  } finally {
    loading.value = false
  }
}

const closePreviewModal = () => {
  showPreviewModal.value = false
  previewContent.value = ''
}

const printPreview = () => {
  const printWindow = window.open('', '_blank')
  if (printWindow) {
    printWindow.document.write(previewContent.value)
    printWindow.document.close()
    printWindow.print()
  }
  closePreviewModal()
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

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('id-ID', {
    style: 'currency',
    currency: 'IDR'
  }).format(amount)
}

// Load data on mount
onMounted(async () => {
  await loadRecentTransactions()
})
</script>