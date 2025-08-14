<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Compact Header -->
    <header class="bg-gradient-to-r from-blue-600 to-blue-700 text-white shadow-lg">
      <div class="max-w-7xl mx-auto px-3 sm:px-4 lg:px-8">
        <div class="flex justify-between items-center py-2 sm:py-3">
          <div class="flex items-center space-x-2 sm:space-x-3">
            <Icon name="heroicons:shopping-cart" class="h-5 w-5 sm:h-6 sm:w-6" />
            <div>
              <h1 class="text-lg sm:text-xl font-bold">POS System</h1>
            </div>
          </div>
        </div>
      </div>
    </header>

    <!-- Tab Navigation -->
    <div class="bg-white border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-3 sm:px-4 lg:px-8">
        <nav class="flex space-x-4 sm:space-x-8">
          <button
            @click="activeTab = 'pos'"
            :class="[
              'py-3 sm:py-4 px-1 border-b-2 font-medium text-xs sm:text-sm',
              activeTab === 'pos'
                ? 'border-blue-500 text-blue-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            ]"
          >
            <Icon name="heroicons:shopping-cart" class="h-3 w-3 sm:h-4 sm:w-4 mr-1 sm:mr-2 inline" />
            <span class="hidden sm:inline">POS Terminal</span>
            <span class="sm:hidden">POS</span>
          </button>
          <button
            @click="activeTab = 'transactions'"
            :class="[
              'py-3 sm:py-4 px-1 border-b-2 font-medium text-xs sm:text-sm',
              activeTab === 'transactions'
                ? 'border-blue-500 text-blue-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            ]"
          >
            <Icon name="heroicons:document-text" class="h-3 w-3 sm:h-4 sm:w-4 mr-1 sm:mr-2 inline" />
            <span class="hidden sm:inline">Transactions</span>
            <span class="sm:hidden">History</span>
          </button>
        </nav>
      </div>
    </div>

    <!-- POS Tab Content -->
    <main v-if="activeTab === 'pos'" class="max-w-7xl mx-auto px-3 sm:px-4 lg:px-8 py-3 sm:py-4">
      <div class="grid grid-cols-1 xl:grid-cols-3 gap-3 sm:gap-4">
        <!-- Left Panel - Product Search & Cart -->
        <div class="xl:col-span-2 space-y-3 sm:space-y-4">
          <!-- Enhanced Product Search -->
          <div class="bg-white rounded-lg shadow-sm border p-3 sm:p-4">
            <!-- Search Controls -->
            <div class="grid grid-cols-1 gap-3 sm:gap-4 mb-3 sm:mb-4">
              <div>
                <label class="block text-xs sm:text-sm font-semibold text-gray-700 mb-1 sm:mb-2">Search Products</label>
                <div class="flex space-x-2">
                  <div class="relative flex-1">
                    <Icon name="heroicons:magnifying-glass" class="absolute left-2 sm:left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4 sm:h-5 sm:w-5" />
                    <input
                      v-model="searchTerm"
                      type="text"
                      placeholder="Search by name or code..."
                      class="w-full pl-8 sm:pl-10 pr-3 sm:pr-4 py-2 sm:py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-xs sm:text-sm"
                      @keyup.enter="searchProducts"
                    />
                  </div>
                  <button 
                    @click="searchProducts" 
                    class="btn-primary px-3 sm:px-4 py-2 sm:py-2.5 flex items-center space-x-1 sm:space-x-2 min-w-[60px] sm:min-w-[100px] justify-center"
                  >
                    <Icon name="heroicons:magnifying-glass" class="h-3 w-3 sm:h-4 sm:w-4" />
                    <span class="hidden sm:inline text-xs sm:text-sm">Search</span>
                  </button>
                </div>
              </div>
              
              <div>
                <label class="block text-xs sm:text-sm font-semibold text-gray-700 mb-1 sm:mb-2">Quick Add by Barcode</label>
                <div class="flex space-x-2">
                  <div class="relative flex-1">
                    <Icon name="heroicons:qr-code" class="absolute left-2 sm:left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4 sm:h-5 sm:w-5" />
                    <input
                      v-model="barcodeInput"
                      type="text"
                      placeholder="Scan or enter barcode..."
                      class="w-full pl-8 sm:pl-10 pr-3 sm:pr-4 py-2 sm:py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-xs sm:text-sm"
                      @keyup.enter="addByBarcode"
                    />
                  </div>
                  <button 
                    @click="addByBarcode" 
                    class="btn-primary px-3 sm:px-4 py-2 sm:py-2.5 flex items-center space-x-1 sm:space-x-2 min-w-[50px] sm:min-w-[80px] justify-center"
                  >
                    <Icon name="heroicons:plus" class="h-3 w-3 sm:h-4 sm:w-4" />
                    <span class="hidden sm:inline text-xs sm:text-sm">Add</span>
                  </button>
                </div>
              </div>
            </div>

            <!-- Best Seller Products with Images -->
            <div class="mb-3 sm:mb-4">
              <h3 class="text-xs sm:text-sm font-semibold text-gray-900 mb-2 sm:mb-3 flex items-center">
                <Icon name="heroicons:fire" class="h-3 w-3 sm:h-4 sm:w-4 mr-1 sm:mr-2 text-orange-500" />
                Best Sellers
              </h3>
              <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 xl:grid-cols-5 gap-2 sm:gap-3">
                <div
                  v-for="product in bestSellerProducts"
                  :key="product.id"
                  @click="addToCart(product)"
                  class="group p-2 sm:p-3 border border-gray-200 rounded-lg cursor-pointer hover:border-blue-300 hover:shadow-md transition-all duration-200 bg-white"
                >
                  <!-- Product Image Placeholder -->
                  <div class="w-full h-12 sm:h-16 bg-gradient-to-br from-gray-100 to-gray-200 rounded-md mb-1 sm:mb-2 flex items-center justify-center group-hover:from-blue-50 group-hover:to-blue-100 transition-colors">
                    <img 
                      v-if="product.imageUrl" 
                      :src="`${config.public.apiBase}${product.imageUrl}`" 
                      :alt="product.productName"
                      class="w-full h-full object-cover rounded-md"
                    />
                    <Icon v-else name="heroicons:photo" class="h-4 w-4 sm:h-6 sm:w-6 text-gray-400 group-hover:text-blue-400" />
                  </div>
                  <div class="text-xs font-semibold text-gray-900 truncate mb-1 leading-tight">{{ product.productName }}</div>
                  <div class="text-xs text-blue-600 font-medium">Rp {{ formatCurrency(product.unitPrice) }}</div>
                  <div class="text-xs text-green-600 mt-1">Stock: {{ product.stockQuantity }}</div>
                </div>
              </div>
            </div>

            <!-- Enhanced Search Results with Images -->
            <div v-if="searchResults.length > 0" class="border-t pt-3 sm:pt-4">
              <h4 class="text-xs sm:text-sm font-semibold text-gray-900 mb-2 sm:mb-3">Search Results ({{ searchResults.length }})</h4>
              <div class="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-2 sm:gap-3 max-h-48 sm:max-h-64 overflow-y-auto">
                <div
                  v-for="product in searchResults"
                  :key="product.id"
                  class="group p-2 sm:p-3 border border-gray-200 rounded-lg hover:border-blue-300 hover:shadow-md cursor-pointer transition-all duration-200 bg-white"
                  @click="addToCart(product)"
                >
                  <div class="flex items-start space-x-2 sm:space-x-3">
                    <!-- Product Image Placeholder -->
                    <div class="w-10 h-10 sm:w-12 sm:h-12 bg-gradient-to-br from-gray-100 to-gray-200 rounded-md flex items-center justify-center group-hover:from-blue-50 group-hover:to-blue-100 transition-colors flex-shrink-0">
                      <img 
                        v-if="product.imageUrl" 
                        :src="`${config.public.apiBase}${product.imageUrl}`" 
                        :alt="product.productName"
                        class="w-full h-full object-cover rounded-md"
                      />
                      <Icon v-else name="heroicons:photo" class="h-3 w-3 sm:h-4 sm:w-4 text-gray-400 group-hover:text-blue-400" />
                    </div>
                    <div class="flex-1 min-w-0">
                      <h4 class="font-semibold text-xs sm:text-sm text-gray-900 truncate leading-tight">{{ product.productName }}</h4>
                      <p class="text-xs text-gray-500 truncate">{{ product.productCode }}</p>
                      <div class="flex justify-between items-center mt-1">
                        <p class="text-xs sm:text-sm font-semibold text-blue-600">Rp {{ formatCurrency(product.unitPrice) }}</p>
                        <span class="text-xs bg-green-100 text-green-800 px-1.5 sm:px-2 py-0.5 rounded-full">
                          {{ product.stockQuantity }}
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Enhanced Shopping Cart -->
          <div class="bg-white rounded-lg shadow-sm border p-3 sm:p-4">
            <div class="flex justify-between items-center mb-3 sm:mb-4">
              <h2 class="text-base sm:text-lg font-semibold text-gray-900 flex items-center">
                <Icon name="heroicons:shopping-cart" class="h-4 w-4 sm:h-5 sm:w-5 mr-1 sm:mr-2 text-blue-600" />
                <span class="hidden sm:inline">Shopping Cart</span>
                <span class="sm:hidden">Cart</span>
                <span class="ml-1 sm:ml-2 bg-blue-100 text-blue-800 text-xs sm:text-sm font-medium px-1.5 sm:px-2.5 py-0.5 rounded-full">
                  {{ cartItems.length }}
                </span>
              </h2>
              <div class="flex space-x-1 sm:space-x-2">
                <button 
                  @click="holdTransaction" 
                  :disabled="cartItems.length === 0" 
                  class="btn-secondary text-xs sm:text-sm px-2 sm:px-3 py-1.5 sm:py-2 flex items-center space-x-1 disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  <Icon name="heroicons:pause" class="h-3 w-3 sm:h-4 sm:w-4" />
                  <span class="hidden sm:inline">Hold</span>
                </button>
                <button 
                  @click="clearCart" 
                  :disabled="cartItems.length === 0" 
                  class="btn-danger text-xs sm:text-sm px-2 sm:px-3 py-1.5 sm:py-2 flex items-center space-x-1 disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  <Icon name="heroicons:trash" class="h-3 w-3 sm:h-4 sm:w-4" />
                  <span class="hidden sm:inline">Clear</span>
                </button>
              </div>
            </div>

            <div v-if="cartItems.length === 0" class="text-center py-6 sm:py-8 text-gray-500">
              <Icon name="heroicons:shopping-cart" class="h-8 w-8 sm:h-12 sm:w-12 mx-auto mb-2 sm:mb-3 text-gray-300" />
              <p class="text-xs sm:text-sm font-medium">Cart is empty</p>
              <p class="text-xs text-gray-400 mt-1">Add products to start a transaction</p>
            </div>

            <div v-else class="space-y-2 sm:space-y-3 max-h-60 sm:max-h-72 overflow-y-auto">
              <div
                v-for="(item, index) in cartItems"
                :key="item.id"
                class="flex items-center space-x-2 sm:space-x-3 p-2 sm:p-3 border border-gray-200 rounded-lg hover:border-gray-300 transition-colors bg-gray-50"
              >
                <!-- Product Image Placeholder -->
                <div class="w-10 h-10 sm:w-12 sm:h-12 bg-gradient-to-br from-gray-200 to-gray-300 rounded-md flex items-center justify-center flex-shrink-0">
                  <img 
                    v-if="item.imageUrl" 
                    :src="`${config.public.apiBase}${item.imageUrl}`" 
                    :alt="item.productName"
                    class="w-full h-full object-cover rounded-md"
                  />
                  <Icon v-else name="heroicons:photo" class="h-4 w-4 sm:h-5 sm:w-5 text-gray-500" />
                </div>
                
                <div class="flex-1 min-w-0">
                  <h4 class="font-semibold text-xs sm:text-sm text-gray-900 truncate leading-tight">{{ item.productName }}</h4>
                  <p class="text-xs sm:text-sm font-medium text-blue-600">Rp {{ formatCurrency(item.unitPrice) }}</p>
                  <p class="text-xs text-gray-500 truncate">{{ item.productCode }}</p>
                </div>
                
                <div class="flex items-center space-x-1 sm:space-x-2">
                  <button
                    @click="updateQuantity(index, item.quantity - 1)"
                    class="w-6 h-6 sm:w-8 sm:h-8 rounded-full bg-gray-300 hover:bg-gray-400 flex items-center justify-center text-gray-700 font-medium transition-colors text-xs sm:text-sm"
                  >
                    -
                  </button>
                  <input
                    v-model.number="item.quantity"
                    type="number"
                    min="1"
                    class="w-12 sm:w-16 text-center text-xs sm:text-sm border border-gray-300 rounded-md px-1 sm:px-2 py-1 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                    @change="updateQuantity(index, item.quantity)"
                  />
                  <button
                    @click="updateQuantity(index, item.quantity + 1)"
                    class="w-6 h-6 sm:w-8 sm:h-8 rounded-full bg-gray-300 hover:bg-gray-400 flex items-center justify-center text-gray-700 font-medium transition-colors text-xs sm:text-sm"
                  >
                    +
                  </button>
                  <button
                    @click="removeFromCart(index)"
                    class="w-6 h-6 sm:w-8 sm:h-8 rounded-full bg-red-100 hover:bg-red-200 text-red-600 flex items-center justify-center font-medium transition-colors ml-1 sm:ml-2 text-sm"
                  >
                    ×
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Right Panel - Customer, Promotion & Payment -->
        <div class="space-y-3 sm:space-y-4">
          <!-- Compact Customer Selection -->
          <div class="bg-white rounded-lg shadow p-3 sm:p-4">
            <h3 class="text-sm sm:text-base font-semibold mb-2 sm:mb-3">Customer</h3>
            <div class="space-y-2 sm:space-y-3">
              <div>
                <input
                  v-model="customerSearch"
                  type="text"
                  placeholder="Search customer..."
                  class="form-input text-xs sm:text-sm w-full"
                  @input="searchCustomers"
                />
                
                <!-- Customer Search Results -->
                <div v-if="customers.length > 0" class="mt-1 max-h-20 sm:max-h-24 overflow-y-auto border rounded">
                  <div
                    v-for="customer in customers"
                    :key="customer.id"
                    class="p-1.5 sm:p-2 hover:bg-gray-50 cursor-pointer text-xs"
                    @click="selectCustomer(customer)"
                  >
                    {{ customer.firstName }} {{ customer.lastName }} - {{ customer.phone }}
                  </div>
                </div>
              </div>
              
              <div v-if="selectedCustomer" class="p-2 bg-blue-50 rounded">
                <p class="text-xs font-medium">{{ selectedCustomer.firstName }} {{ selectedCustomer.lastName }}</p>
                <p class="text-xs text-gray-600">{{ selectedCustomer.phone }}</p>
                <button @click="selectedCustomer = null; customerSearch = ''" class="text-xs text-red-600 mt-1">
                  Remove
                </button>
              </div>
            </div>
          </div>

          <!-- Promotion Code -->
          <div class="bg-white rounded-lg shadow p-3 sm:p-4">
            <h3 class="text-sm sm:text-base font-semibold mb-2 sm:mb-3">Promotion</h3>
            <div class="space-y-2 sm:space-y-3">
              <div>
                <div class="flex space-x-2">
                  <input
                    v-model="promotionCode"
                    type="text"
                    placeholder="Enter promotion code..."
                    class="form-input text-xs sm:text-sm flex-1"
                    @keyup.enter="applyPromotion"
                  />
                  <button @click="applyPromotion" :disabled="!promotionCode.trim()" class="btn-primary text-xs px-2 sm:px-3 py-1.5 sm:py-2">
                    Apply
                  </button>
                </div>
              </div>
              
              <div v-if="appliedPromotion" class="p-2 bg-green-50 rounded">
                <div class="flex justify-between items-start">
                  <div>
                    <p class="text-xs font-medium text-green-800">{{ appliedPromotion.promotionName }}</p>
                    <p class="text-xs text-green-600">{{ appliedPromotion.promotionCode }}</p>
                    <p class="text-xs text-green-600">-Rp {{ formatCurrency(promotionDiscount) }}</p>
                  </div>
                  <button @click="removePromotion" class="text-xs text-red-600">
                    ×
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Compact Transaction Summary -->
          <div class="bg-white rounded-lg shadow p-3 sm:p-4">
            <h3 class="text-sm sm:text-base font-semibold mb-2 sm:mb-3">Summary</h3>
            <div class="space-y-1 text-xs sm:text-sm">
              <div class="flex justify-between">
                <span>Subtotal:</span>
                <span>Rp {{ formatCurrency(subtotal) }}</span>
              </div>
              <div v-if="promotionDiscount > 0" class="flex justify-between text-green-600">
                <span>Promotion:</span>
                <span>-Rp {{ formatCurrency(promotionDiscount) }}</span>
              </div>
              <div class="flex justify-between">
                <span>Tax (11%):</span>
                <span>Rp {{ formatCurrency(taxAmount) }}</span>
              </div>
              <hr class="my-2">
              <div class="flex justify-between text-sm sm:text-base font-semibold">
                <span>Total:</span>
                <span>Rp {{ formatCurrency(totalAmount) }}</span>
              </div>
            </div>
          </div>

          <!-- Compact Payment -->
          <div class="bg-white rounded-lg shadow p-3 sm:p-4">
            <h3 class="text-sm sm:text-base font-semibold mb-2 sm:mb-3">Payment</h3>
            <div class="space-y-2 sm:space-y-3">
              <div>
                <label class="block text-xs font-medium text-gray-700 mb-1">Payment Method</label>
                <select v-model="paymentMethod" class="form-input text-xs sm:text-sm w-full">
                  <option value="Cash">Cash</option>
                  <option value="CreditCard">Credit Card</option>
                  <option value="DebitCard">Debit Card</option>
                  <option value="eWallet">e-Wallet</option>
                  <option value="QRIS">QRIS</option>
                </select>
              </div>
              
              <div v-if="paymentMethod === 'Cash'">
                <label class="block text-xs font-medium text-gray-700 mb-1">Amount Received</label>
                <input
                  v-model.number="amountReceived"
                  type="number"
                  step="0.01"
                  class="form-input text-xs sm:text-sm w-full"
                  @input="calculateChange"
                />
                <div v-if="changeAmount > 0" class="mt-1 p-2 bg-green-50 rounded">
                  <p class="text-xs text-green-800 font-semibold">
                    Change: Rp {{ formatCurrency(changeAmount) }}
                  </p>
                </div>
              </div>
              
              <button
                @click="processPayment"
                :disabled="cartItems.length === 0 || processing"
                class="btn-primary w-full text-xs sm:text-sm py-2 sm:py-2.5"
              >
                {{ processing ? 'Processing...' : 'Complete Sale' }}
              </button>
            </div>
          </div>

          <!-- Compact Held Transactions -->
          <div v-if="heldTransactions.length > 0" class="bg-white rounded-lg shadow p-3 sm:p-4">
            <h3 class="text-sm sm:text-base font-semibold mb-2 sm:mb-3">Held Transactions</h3>
            <div class="space-y-1 max-h-28 sm:max-h-32 overflow-y-auto">
              <div
                v-for="transaction in heldTransactions"
                :key="transaction.id"
                class="p-1.5 sm:p-2 border rounded hover:bg-gray-50 cursor-pointer text-xs"
                @click="resumeTransaction(transaction)"
              >
                <div class="flex justify-between">
                  <span class="truncate">{{ transaction.transactionNumber }}</span>
                  <span class="font-semibold ml-2">Rp {{ formatCurrency(transaction.totalAmount) }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Transactions Tab Content -->
    <div v-if="activeTab === 'transactions'" class="max-w-7xl mx-auto px-3 sm:px-4 lg:px-8 py-3 sm:py-4">
      <TransactionsView />
    </div>
  </div>
</template>

<script setup lang="ts">
interface Product {
  id: number
  productName: string
  productCode: string
  barcode?: string
  unitPrice: number
  stockQuantity: number
  categoryName?: string
  imageUrl?: string
}

interface CartItem extends Product {
  quantity: number
}

interface Customer {
  id: number
  firstName: string
  lastName: string
  phone: string
}

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
  paymentMethod?: string
  amountReceived?: number
  changeAmount?: number
  transactionDate: string
  createdAt: string
  updatedAt: string
  items?: TransactionItem[]
  transactionItems?: TransactionItem[]
}

interface TransactionItem {
  productId: number
  unitPrice: number
  quantity: number
  product?: Product
}

definePageMeta({
  middleware: 'auth'
})

const { user, logout } = useAuth()
const { showSuccess, showError, showWarning } = useAlert()
const config = useRuntimeConfig()

// Reactive data
const activeTab = ref('pos')
const searchTerm = ref('')
const barcodeInput = ref('')
const searchResults = ref<Product[]>([])
const cartItems = ref<CartItem[]>([])
const currentTransaction = ref<Transaction | null>(null)
const processing = ref(false)
const bestSellerProducts = ref<Product[]>([])

// Customer data
const customerSearch = ref('')
const customers = ref<Customer[]>([])
const selectedCustomer = ref<Customer | null>(null)

// Promotion data
const promotionCode = ref('')
const appliedPromotion = ref<any>(null)
const promotionDiscount = ref(0)

// Payment data
const paymentMethod = ref('Cash')
const amountReceived = ref(0)
const changeAmount = ref(0)

// Held transactions
const heldTransactions = ref<Transaction[]>([])

// Computed properties
const subtotal = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + (item.unitPrice * item.quantity), 0)
})

const discountAmount = computed(() => {
  return promotionDiscount.value
})

const taxAmount = computed(() => {
  return (subtotal.value - discountAmount.value) * 0.11
})

const totalAmount = computed(() => {
  return subtotal.value - discountAmount.value + taxAmount.value
})

// Methods
const handleLogout = async () => {
  await logout()
}

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('id-ID').format(amount)
}

const selectCustomer = (customer: Customer) => {
  selectedCustomer.value = customer
  customers.value = []
  customerSearch.value = `${customer.firstName} ${customer.lastName}`
}

const applyPromotion = async () => {
  if (!promotionCode.value.trim()) return
  if (cartItems.value.length === 0) {
    showWarning('Please add items to cart before applying promotion')
    return
  }
  
  try {
    const { token } = useAuth()
    
    // Convert cart items to transaction items format
    const items: TransactionItem[] = cartItems.value.map(item => ({
      productId: item.id,
      unitPrice: item.unitPrice,
      quantity: item.quantity,
    }))
    
    // Call the calculate-discount API
    const response = await $fetch<any>('/api/promotion/calculate-discount', {
      method: 'POST',
      body: {
        promotionCode: promotionCode.value,
        items: items
      },
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })
    
    if (!response || !response.promotion) {
      throw new Error('Promotion not found or not applicable')
    }
    
    appliedPromotion.value = response.promotion
    promotionDiscount.value = response.discount
    
    showSuccess('Promotion applied successfully!')
    
  } catch (error) {
    console.error('Failed to apply promotion:', error)
    showWarning('Invalid promotion code or promotion not applicable')
  }
}

const removePromotion = () => {
  appliedPromotion.value = null
  promotionDiscount.value = 0
  promotionCode.value = ''
}

const searchProducts = async () => {
  if (!searchTerm.value.trim()) return
  
  try {
    const { token } = useAuth()
    
    const response = await $fetch<Product[]>(`/api/product/search?searchTerm=${encodeURIComponent(searchTerm.value)}`, {
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })
    
    searchResults.value = response
  } catch (error) {
    console.error('Failed to search products:', error)
  }
}

const addByBarcode = async () => {
  if (!barcodeInput.value.trim()) return
  
  try {
    const { token } = useAuth()
    
    const product = await $fetch<Product>(`/api/product/by-barcode/${encodeURIComponent(barcodeInput.value)}`, {
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })
    
    addToCart(product)
    barcodeInput.value = ''
  } catch (error) {
    console.error('Product not found:', error)
    showWarning('Product not found')
  }
}

const addToCart = (product: Product) => {
  if (product.stockQuantity <= 0) {
    showWarning('Product is out of stock')
    return
  }
  
  const existingIndex = cartItems.value.findIndex(item => item.id === product.id)
  
  if (existingIndex >= 0) {
    cartItems.value[existingIndex]!.quantity += 1
  } else {
    cartItems.value.push({
      ...product,
      quantity: 1
    })
  }
  
  searchResults.value = []
  searchTerm.value = ''
}

const updateQuantity = (index: number, newQuantity: number) => {
  if (newQuantity <= 0) {
    removeFromCart(index)
  } else if (cartItems.value[index]) {
    cartItems.value[index]!.quantity = newQuantity
  }
}

const removeFromCart = (index: number) => {
  cartItems.value.splice(index, 1)
}

const clearCart = () => {
  if (confirm('Are you sure you want to clear the cart?')) {
    cartItems.value = []
    currentTransaction.value = null
  }
}

const calculateChange = () => {
  changeAmount.value = Math.max(0, amountReceived.value - totalAmount.value)
}

const searchCustomers = async () => {
  if (!customerSearch.value.trim()) return
  
  try {
    const { token } = useAuth()
    
    const response = await $fetch<Customer[]>('/api/customer', {
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })
    
    customers.value = response.filter((customer: Customer) => 
      customer.firstName.toLowerCase().includes(customerSearch.value.toLowerCase()) ||
      customer.lastName.toLowerCase().includes(customerSearch.value.toLowerCase()) ||
      customer.phone.includes(customerSearch.value)
    )
  } catch (error) {
    console.error('Failed to search customers:', error)
  }
}

const createTransaction = async () => {
  try {
    const { token } = useAuth()
    
    if (!user.value?.storeId) {
      throw new Error('User store ID not found')
    }
    
    const transaction = await $fetch<Transaction>('/api/transaction', {
      method: 'POST',
      body: {
        storeId: user.value.storeId,
        customerId: selectedCustomer.value?.id || null
      },
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })
    
    currentTransaction.value = transaction
    return transaction
  } catch (error) {
    console.error('Failed to create transaction:', error)
    throw error
  }
}

const addItemsToTransaction = async (transactionId: number) => {
  try {
    const { token } = useAuth()
    
    for (const item of cartItems.value) {
      await $fetch(`/api/transaction/${transactionId}/items`, {
        method: 'POST',
        body: {
          productId: item.id,
          quantity: item.quantity,
          unitPrice: item.unitPrice,
          discountAmount: 0
        },
        headers: {
          Authorization: `Bearer ${token.value ?? ''}`
        },
        baseURL: config.public.apiBase
      })
    }
  } catch (error) {
    console.error('Failed to add items to transaction:', error)
    throw error
  }
}

const processPayment = async () => {
  if (cartItems.value.length === 0) {
    showWarning('Cart is empty')
    return
  }
  
  if (paymentMethod.value === 'Cash' && amountReceived.value < totalAmount.value) {
    showWarning('Insufficient payment amount')
    return
  }
  
  try {
    processing.value = true
    
    let transaction = currentTransaction.value
    
    if (!transaction) {
      transaction = await createTransaction()
      if (!transaction) {
        throw new Error('Failed to create transaction')
      }
      await addItemsToTransaction(transaction.id)
    }
    
    const { token } = useAuth()
    
    // Process payment
    await $fetch(`/api/transaction/${transaction.id}/payment`, {
      method: 'POST',
      body: {
        paymentMethod: paymentMethod.value,
        amount: totalAmount.value,
        receivedAmount: paymentMethod.value === 'Cash' ? amountReceived.value : totalAmount.value,
        changeAmount: paymentMethod.value === 'Cash' ? changeAmount.value : 0,
        promotionCode: promotionCode.value,
        discountAmount: promotionDiscount.value
      },
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })
    
    // Complete the transaction
    await $fetch(`/api/transaction/${transaction.id}/complete`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })
    
    // Show success message with print option
    const printReceipt = confirm('Transaction completed successfully! Would you like to print the receipt?')
    
    if (printReceipt) {
      await printTransactionReceipt(transaction.id)
    }
    
    // Reset everything
    cartItems.value = []
    currentTransaction.value = null
    selectedCustomer.value = null
    customerSearch.value = ''
    promotionCode.value = ''
    appliedPromotion.value = null
    promotionDiscount.value = 0
    paymentMethod.value = 'Cash'
    amountReceived.value = 0
    changeAmount.value = 0
    
  } catch (error) {
    console.error('Failed to process payment:', error)
    showError('Failed to process payment')
  } finally {
    processing.value = false
  }
}

const holdTransaction = async () => {
  if (cartItems.value.length === 0) {
    showWarning('Cart is empty')
    return
  }
  
  try {
    let transaction = currentTransaction.value
    
    if (!transaction) {
      transaction = await createTransaction()
      if (!transaction) {
        throw new Error('Failed to create transaction')
      }
      await addItemsToTransaction(transaction.id)
    }
    
    const { token } = useAuth()
    
    await $fetch(`/api/transaction/${transaction.id}/hold`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })
    
    showSuccess('Transaction held successfully!')
    
    // Reset cart but keep transaction in held list
    cartItems.value = []
    currentTransaction.value = null
    selectedCustomer.value = null
    customerSearch.value = ''
    promotionCode.value = ''
    appliedPromotion.value = null
    promotionDiscount.value = 0
    
    // Refresh held transactions
    await loadHeldTransactions()
    
  } catch (error) {
    console.error('Failed to hold transaction:', error)
    showError('Failed to hold transaction')
  }
}

const loadHeldTransactions = async () => {
  try {
    const { token } = useAuth()
    
    if (!user.value?.storeId) {
      return
    }
    
    const response = await $fetch<Transaction[]>(`/api/transaction/held?storeId=${user.value.storeId}`, {
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })
    
    heldTransactions.value = response
  } catch (error) {
    console.error('Failed to load held transactions:', error)
  }
}

const resumeTransaction = async (transaction: Transaction) => {
  try {
    const { token } = useAuth()
    
    // Get transaction details with items
    const fullTransaction = await $fetch<Transaction>(`/api/transaction/${transaction.id}`, {
      headers: {
        Authorization: `Bearer ${token.value}`
      },
      baseURL: config.public.apiBase
    })
    
    if (!fullTransaction.transactionItems) {
      throw new Error('Transaction items not found')
    }
    
    // Load transaction into cart
    cartItems.value = fullTransaction.transactionItems.map((item: TransactionItem) => ({
      id: item.productId,
      productName: item.product?.productName || 'Unknown Product',
      productCode: item.product?.productCode || '',
      unitPrice: item.unitPrice,
      stockQuantity: 999, // We don't have current stock info
      quantity: item.quantity
    }))
    
    currentTransaction.value = fullTransaction
    
    // Set customer if exists
    if (fullTransaction.customerId && customers.value.length > 0) {
      selectedCustomer.value = customers.value.find(c => c.id === fullTransaction.customerId) || null
    }
    
    showSuccess('Transaction resumed successfully!')
    
  } catch (error) {
    console.error('Failed to resume transaction:', error)
    showError('Failed to resume transaction')
  }
}

const printTransactionReceipt = async (transactionId: number) => {
  try {
    const { token } = useAuth()
    
    // Generate receipt using the PrintController
    const response = await $fetch(`/api/print/receipt/${transactionId}`, {
      method: 'GET',
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })
    
    if (response && typeof response === 'object' && 'filePath' in response) {
      // Open the generated PDF in a new window for printing
      const printWindow = window.open(`${config.public.apiBase}${response.filePath}`, '_blank')
      if (printWindow) {
        printWindow.focus()
        // Trigger print dialog after PDF loads
        printWindow.onload = () => {
          printWindow.print()
        }
      }
    }
  } catch (error) {
    console.error('Failed to print receipt:', error)
    showError('Failed to print receipt')
  }
}

const loadBestSellerProducts = async () => {
  try {
    const { token } = useAuth()
    
    if (!user.value?.storeId) {
      return
    }
    
    const response = await $fetch<any>('/api/report/top-selling-products', {
      method: 'POST',
      body: {
        storeId: user.value.storeId,
        startDate: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString(), // Last 30 days
        endDate: new Date().toISOString(),
        count: 6
      },
      headers: {
        Authorization: `Bearer ${token.value ?? ''}`
      },
      baseURL: config.public.apiBase
    })
    
    if (response && response.products) {
      bestSellerProducts.value = response.products
    }
  } catch (error) {
    console.error('Failed to load best seller products:', error)
  }
}

// Initialize
onMounted(async () => {
  await loadHeldTransactions()
  await loadBestSellerProducts()
})

// Watch for payment method changes
watch(paymentMethod, () => {
  if (paymentMethod.value !== 'Cash') {
    amountReceived.value = totalAmount.value
    changeAmount.value = 0
  }
})

watch(totalAmount, () => {
  if (paymentMethod.value !== 'Cash') {
    amountReceived.value = totalAmount.value
  }
  calculateChange()
})
</script>