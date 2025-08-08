<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Main Content -->
    <main class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
      <!-- Page Header -->
      <div class="mb-8">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-3xl font-bold text-gray-900">Reports & Analytics</h1>
            <p class="mt-2 text-gray-600">View sales reports and business analytics to track your performance</p>
          </div>
          <div class="flex items-center space-x-3">
            <button
              @click="refreshData"
              :disabled="loading"
              class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50"
            >
              <svg class="w-4 h-4 mr-2" :class="{ 'animate-spin': loading }" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"></path>
              </svg>
              Refresh
            </button>
          </div>
        </div>
      </div>

      <!-- Report Type Selection -->
      <div class="mb-8">
        <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <h2 class="text-lg font-semibold text-gray-900 mb-4">Report Type</h2>
          <div class="grid grid-cols-2 md:grid-cols-4 gap-3">
            <button
              v-for="type in reportTypes"
              :key="type.key"
              @click="selectedReportType = type.key"
              :class="[
                'flex items-center justify-center px-4 py-3 rounded-lg text-sm font-medium transition-all duration-200',
                selectedReportType === type.key
                  ? 'bg-blue-600 text-white shadow-md transform scale-105'
                  : 'bg-gray-50 text-gray-700 hover:bg-gray-100 border border-gray-200 hover:border-gray-300'
              ]"
            >
              <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
              </svg>
              {{ type.name }}
            </button>
          </div>
        </div>
      </div>

      <!-- Date Range and Filters -->
      <div class="bg-white rounded-lg shadow-sm p-6 mb-6">
        <h2 class="text-lg font-medium text-gray-900 mb-4">Report Parameters</h2>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div v-if="selectedReportType !== 'dashboard-metrics'">
            <label class="block text-sm font-medium text-gray-700 mb-1">
              {{ selectedReportType === 'daily-sales' ? 'Date' : 'Start Date' }}
            </label>
            <input
              v-model="startDate"
              type="date"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          <div v-if="selectedReportType !== 'daily-sales' && selectedReportType !== 'dashboard-metrics'">
            <label class="block text-sm font-medium text-gray-700 mb-1">
              End Date
            </label>
            <input
              v-model="endDate"
              type="date"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
          <div v-if="selectedReportType === 'top-selling-products'">
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Top Count
            </label>
            <input
              v-model.number="topCount"
              type="number"
              min="1"
              max="100"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
        </div>
        <div class="mt-4 flex justify-end">
          <button
            @click="generateReport"
            :disabled="loading"
            class="btn-primary"
          >
            {{ loading ? 'Generating...' : 'Generate Report' }}
          </button>
        </div>
      </div>

      <!-- Report Results -->
      <div v-if="reportData" class="bg-white rounded-lg shadow-sm">
        <div class="px-6 py-4 border-b border-gray-200">
          <h2 class="text-lg font-medium text-gray-900">
            {{ getReportTitle() }}
          </h2>
          <p class="text-sm text-gray-600">
            Generated on {{ new Date().toLocaleString() }}
          </p>
        </div>

        <!-- Dashboard Metrics -->
        <div v-if="selectedReportType === 'dashboard-metrics'" class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
            <div class="bg-blue-50 p-4 rounded-lg">
              <h3 class="text-sm font-medium text-blue-800">Today's Sales</h3>
              <p class="text-2xl font-bold text-blue-900">
                {{ formatCurrency(reportData.TodaySales?.totalAmount || 0) }}
              </p>
              <p class="text-sm text-blue-600">
                {{ reportData.TodaySales?.transactionCount || 0 }} transactions
              </p>
            </div>
            <div class="bg-green-50 p-4 rounded-lg">
              <h3 class="text-sm font-medium text-green-800">Month Sales</h3>
              <p class="text-2xl font-bold text-green-900">
                {{ formatCurrency(reportData.MonthSales?.totalAmount || 0) }}
              </p>
              <p class="text-sm text-green-600">
                {{ reportData.MonthSales?.transactionCount || 0 }} transactions
              </p>
            </div>
            <div class="bg-purple-50 p-4 rounded-lg">
              <h3 class="text-sm font-medium text-purple-800">Top Products</h3>
              <p class="text-2xl font-bold text-purple-900">
                {{ reportData.TopProducts?.length || 0 }}
              </p>
              <p class="text-sm text-purple-600">products sold</p>
            </div>
          </div>
          
          <div v-if="reportData.TopProducts?.length > 0">
            <h3 class="text-lg font-medium text-gray-900 mb-4">Top Selling Products</h3>
            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-gray-200">
                <thead class="bg-gray-50">
                  <tr>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Product
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Quantity Sold
                    </th>
                    <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Revenue
                    </th>
                  </tr>
                </thead>
                <tbody class="bg-white divide-y divide-gray-200">
                  <tr v-for="product in reportData.TopProducts" :key="product.productId">
                    <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                      {{ product.productName }}
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {{ product.quantitySold }}
                    </td>
                    <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {{ formatCurrency(product.totalRevenue) }}
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- Daily Sales Report -->
        <div v-else-if="selectedReportType === 'daily-sales'" class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
            <div class="bg-blue-50 p-4 rounded-lg">
              <h3 class="text-sm font-medium text-blue-800">Total Sales</h3>
              <p class="text-xl font-bold text-blue-900">
                {{ formatCurrency(reportData.totalAmount || 0) }}
              </p>
            </div>
            <div class="bg-green-50 p-4 rounded-lg">
              <h3 class="text-sm font-medium text-green-800">Transactions</h3>
              <p class="text-xl font-bold text-green-900">
                {{ reportData.transactionCount || 0 }}
              </p>
            </div>
            <div class="bg-purple-50 p-4 rounded-lg">
              <h3 class="text-sm font-medium text-purple-800">Avg. Transaction</h3>
              <p class="text-xl font-bold text-purple-900">
                {{ formatCurrency(reportData.averageTransactionAmount || 0) }}
              </p>
            </div>
            <div class="bg-yellow-50 p-4 rounded-lg">
              <h3 class="text-sm font-medium text-yellow-800">Items Sold</h3>
              <p class="text-xl font-bold text-yellow-900">
                {{ reportData.totalItemsSold || 0 }}
              </p>
            </div>
          </div>
        </div>

        <!-- Product Sales Report -->
        <div v-else-if="selectedReportType === 'product-sales' && Array.isArray(reportData)" class="p-6">
          <div class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Product
                  </th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Quantity Sold
                  </th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Revenue
                  </th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Avg. Price
                  </th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                <tr v-for="product in reportData" :key="product.productId">
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {{ product.productName }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {{ product.quantitySold }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {{ formatCurrency(product.totalRevenue) }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {{ formatCurrency(product.averagePrice) }}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Cashier Performance Report -->
        <div v-else-if="selectedReportType === 'cashier-performance' && Array.isArray(reportData)" class="p-6">
          <div class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Cashier
                  </th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Transactions
                  </th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Total Sales
                  </th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Avg. Transaction
                  </th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                <tr v-for="cashier in reportData" :key="cashier.userId">
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {{ cashier.userName }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {{ cashier.transactionCount }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {{ formatCurrency(cashier.totalSales) }}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {{ formatCurrency(cashier.averageTransactionAmount) }}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Generic Table for other reports -->
        <div v-else-if="Array.isArray(reportData)" class="p-6">
          <div class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th
                    v-for="(value, key) in reportData[0]"
                    :key="key"
                    class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                  >
                    {{ formatColumnName(key) }}
                  </th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                <tr v-for="(row, index) in reportData" :key="index">
                  <td
                    v-for="(value, key) in row"
                    :key="key"
                    class="px-6 py-4 whitespace-nowrap text-sm text-gray-500"
                  >
                    {{ formatCellValue(key, value) }}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Single Object Report -->
        <div v-else-if="typeof reportData === 'object'" class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div
              v-for="(value, key) in reportData"
              :key="key"
              class="bg-gray-50 p-4 rounded-lg"
            >
              <h3 class="text-sm font-medium text-gray-700">{{ formatColumnName(key) }}</h3>
              <p class="text-lg font-semibold text-gray-900">
                {{ formatCellValue(key, value) }}
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- No Data Message -->
      <div v-else-if="!loading && reportGenerated" class="bg-white rounded-lg shadow-sm p-8 text-center">
        <div class="text-gray-500">No data available for the selected parameters</div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  middleware: 'auth'
})

const { user, logout } = useAuth()
const config = useRuntimeConfig()

// Reactive data
const selectedReportType = ref('dashboard-metrics')
const startDate = ref(new Date().toISOString().split('T')[0])
const endDate = ref(new Date().toISOString().split('T')[0])
const topCount = ref(10)
const loading = ref(false)
const reportData = ref<any>(null)
const reportGenerated = ref(false)

// Report types configuration
const reportTypes = [
  {
    key: 'dashboard-metrics',
    name: 'Dashboard Metrics',
    description: 'Overview of today\'s and monthly performance'
  },
  {
    key: 'daily-sales',
    name: 'Daily Sales',
    description: 'Sales summary for a specific date'
  },
  {
    key: 'product-sales',
    name: 'Product Sales',
    description: 'Sales performance by product'
  },
  {
    key: 'cashier-performance',
    name: 'Cashier Performance',
    description: 'Performance metrics by cashier'
  },
  {
    key: 'top-selling-products',
    name: 'Top Selling Products',
    description: 'Best performing products by sales volume'
  },
  {
    key: 'profit-margin',
    name: 'Profit Margin',
    description: 'Profit analysis and margins'
  },
  {
    key: 'stock-movement',
    name: 'Stock Movement',
    description: 'Inventory movement and changes'
  },
  {
    key: 'sales-summary',
    name: 'Sales Summary',
    description: 'Comprehensive sales overview'
  }
]

// Methods
const handleLogout = async () => {
  await logout()
}

const refreshData = async () => {
  await generateReport()
}

const generateReport = async () => {
  try {
    loading.value = true
    reportData.value = null
    
    let url = `/api/report/${selectedReportType.value}`
    const params = new URLSearchParams()
    
    if (user.value?.storeId) {
      params.append('storeId', user.value.storeId.toString())
    }
    
    if (selectedReportType.value === 'daily-sales') {
      if (startDate.value) params.append('date', startDate.value)
    } else if (selectedReportType.value !== 'dashboard-metrics') {
      if (startDate.value) params.append('startDate', startDate.value)
      if (endDate.value) params.append('endDate', endDate.value)
      
      if (selectedReportType.value === 'top-selling-products') {
        params.append('topCount', topCount.value.toString())
      }
    }
    
    if (params.toString()) {
      url += `?${params.toString()}`
    }
    
    const token = useCookie('auth-token')
    const response = await $fetch(url, {
      headers: {
        Authorization: `Bearer ${token.value || ''}`
      },
      baseURL: config.public.apiBase
    })
    
    reportData.value = response
    reportGenerated.value = true
  } catch (error) {
    console.error('Failed to generate report:', error)
    alert('Failed to generate report')
    reportGenerated.value = true
  } finally {
    loading.value = false
  }
}

const getReportTitle = (): string => {
  const reportType = reportTypes.find(r => r.key === selectedReportType.value)
  return reportType?.name || 'Report'
}

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('id-ID', {
    style: 'currency',
    currency: 'IDR'
  }).format(amount)
}

const formatColumnName = (key: string): string => {
  return key.replace(/([A-Z])/g, ' $1')
    .replace(/^./, str => str.toUpperCase())
    .trim()
}

const formatCellValue = (key: string, value: any): string => {
  if (value === null || value === undefined) {
    return '-'
  }
  
  // Format currency fields
  if (key.toLowerCase().includes('amount') || 
      key.toLowerCase().includes('price') || 
      key.toLowerCase().includes('revenue') ||
      key.toLowerCase().includes('sales')) {
    return formatCurrency(Number(value))
  }
  
  // Format dates
  if (key.toLowerCase().includes('date') && typeof value === 'string') {
    return new Date(value).toLocaleDateString()
  }
  
  return String(value)
}

// Generate dashboard metrics on mount
onMounted(() => {
  generateReport()
})
</script>