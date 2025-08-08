<template>
  <div>
    <!-- Header Section -->
    <div class="mb-8">
      <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 class="text-3xl font-bold text-gray-900">Product Management</h1>
          <p class="mt-2 text-sm text-gray-600">Manage your product catalog and inventory</p>
        </div>
        <div class="mt-4 sm:mt-0">
          <button
            @click="showCreateModal = true"
            class="inline-flex items-center px-4 py-2 bg-gradient-to-r from-indigo-600 to-purple-600 text-white text-sm font-medium rounded-xl hover:from-indigo-700 hover:to-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 transition-all duration-200 shadow-lg hover:shadow-xl"
          >
            <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
            </svg>
            Add New Product
          </button>
        </div>
      </div>
    </div>

    <!-- Search and Filters -->
    <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6 mb-8">
      <div class="flex flex-col lg:flex-row gap-4">
        <div class="flex-1">
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <svg class="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"/>
              </svg>
            </div>
            <input
              v-model="searchTerm"
              type="text"
              placeholder="Search products by name, code, or barcode..."
              class="block w-full pl-10 pr-3 py-3 border border-gray-200 rounded-xl leading-5 bg-gray-50 placeholder-gray-500 focus:outline-none focus:placeholder-gray-400 focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all duration-200"
              @input="searchProducts"
            />
          </div>
        </div>
        <div class="flex flex-wrap gap-3">
          <button
            @click="showLowStock = !showLowStock"
            :class="[
              'px-4 py-3 rounded-xl text-sm font-medium transition-all duration-200',
              showLowStock 
                ? 'bg-red-500 text-white shadow-lg' 
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            ]"
          >
            <svg class="w-4 h-4 inline mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z"/>
            </svg>
            Low Stock Only
          </button>
          <button
            @click="refreshProducts"
            class="px-4 py-3 bg-gray-100 text-gray-700 rounded-xl text-sm font-medium hover:bg-gray-200 transition-all duration-200"
          >
            <svg class="w-4 h-4 inline mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"/>
            </svg>
            Refresh
          </button>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="flex items-center justify-center py-12">
      <div class="text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600 mx-auto"></div>
        <p class="mt-4 text-gray-600">Loading products...</p>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else-if="filteredProducts.length === 0" class="text-center py-12">
      <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4"/>
      </svg>
      <h3 class="mt-4 text-lg font-medium text-gray-900">No products found</h3>
      <p class="mt-2 text-gray-500">Get started by adding your first product.</p>
      <div class="mt-6">
        <button
          @click="showCreateModal = true"
          class="inline-flex items-center px-4 py-2 bg-indigo-600 text-white text-sm font-medium rounded-xl hover:bg-indigo-700 transition-colors duration-200"
        >
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
          </svg>
          Add Product
        </button>
      </div>
    </div>

    <!-- Products Grid -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div
        v-for="product in filteredProducts"
        :key="product.id"
        class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6 hover:shadow-lg transition-all duration-200 hover:border-indigo-200"
      >
        <!-- Product Image -->
        <div class="mb-4">
          <div class="w-full h-48 bg-gray-100 rounded-xl overflow-hidden">
            <img
              v-if="product.imageUrl"
              :src="`${config.public.apiBase}${product.imageUrl}`"
              :alt="product.productName"
              class="w-full h-full object-cover"
            />
            <div v-else class="w-full h-full flex items-center justify-center text-gray-400">
              <svg class="w-16 h-16" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"/>
              </svg>
            </div>
          </div>
        </div>

        <!-- Product Header -->
        <div class="flex items-start justify-between mb-4">
          <div class="flex-1">
            <h3 class="text-lg font-semibold text-gray-900 mb-1">{{ product.productName }}</h3>
            <p class="text-sm text-gray-500 line-clamp-2">{{ product.description || 'No description' }}</p>
          </div>
          <div class="relative ml-4">
            <button
              @click="toggleProductActions(product.id)"
              class="p-2 text-gray-400 hover:text-gray-600 rounded-lg hover:bg-gray-100 transition-colors duration-200"
            >
              <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                <path d="M10 6a2 2 0 110-4 2 2 0 010 4zM10 12a2 2 0 110-4 2 2 0 010 4zM10 18a2 2 0 110-4 2 2 0 010 4z"/>
              </svg>
            </button>
            
            <!-- Actions Dropdown -->
            <div
              v-if="activeProductActions === product.id"
              class="absolute right-0 mt-2 w-48 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-10"
            >
              <button
                @click="updateStock(product)"
                class="w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-50 flex items-center"
              >
                <svg class="w-4 h-4 mr-3 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
                </svg>
                Update Stock
              </button>
              <button
                @click="deleteProduct(product)"
                class="w-full text-left px-4 py-2 text-sm text-red-600 hover:bg-red-50 flex items-center"
              >
                <svg class="w-4 h-4 mr-3 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                </svg>
                Delete
              </button>
            </div>
          </div>
        </div>

        <!-- Product Details -->
        <div class="space-y-3 mb-4">
          <div class="flex items-center justify-between text-sm">
            <span class="text-gray-500">Code:</span>
            <span class="font-medium text-gray-900">{{ product.productCode }}</span>
          </div>
          <div v-if="product.barcode" class="flex items-center justify-between text-sm">
            <span class="text-gray-500">Barcode:</span>
            <span class="font-medium text-gray-900">{{ product.barcode }}</span>
          </div>
          <div class="flex items-center justify-between text-sm">
            <span class="text-gray-500">Category:</span>
            <span class="font-medium text-gray-900">{{ product.categoryName || 'N/A' }}</span>
          </div>
          <div class="flex items-center justify-between text-sm">
            <span class="text-gray-500">Unit:</span>
            <span class="font-medium text-gray-900">{{ product.unitOfMeasure || 'PCS' }}</span>
          </div>
        </div>

        <!-- Pricing -->
        <div class="bg-gray-50 rounded-xl p-4 mb-4">
          <div class="flex items-center justify-between mb-2">
            <span class="text-sm text-gray-500">Selling Price</span>
            <span class="text-lg font-bold text-gray-900">Rp {{ formatCurrency(product.unitPrice) }}</span>
          </div>
          <div class="flex items-center justify-between">
            <span class="text-sm text-gray-500">Cost Price</span>
            <span class="text-sm font-medium text-gray-600">Rp {{ formatCurrency(product.costPrice) }}</span>
          </div>
        </div>

        <!-- Stock Status -->
        <div class="flex items-center justify-between">
          <span
            :class="[
              'inline-flex items-center px-3 py-1 rounded-full text-sm font-medium',
              product.stockQuantity <= product.minStockLevel
                ? 'bg-red-100 text-red-800'
                : 'bg-green-100 text-green-800'
            ]"
          >
            <svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
            </svg>
            Stock: {{ product.stockQuantity }}
          </span>
          <button
            @click="editProduct(product)"
            class="inline-flex items-center px-4 py-2 bg-indigo-600 text-white text-sm font-medium rounded-lg hover:bg-indigo-700 transition-colors duration-200"
          >
            <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
            </svg>
            Edit
          </button>
        </div>
      </div>
    </div>

    <!-- Create/Edit Product Modal -->
    <div v-if="showCreateModal || showEditModal" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-11/12 md:w-1/2 shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 mb-4">
            {{ showCreateModal ? 'Add New Product' : 'Edit Product' }}
          </h3>
          
          <form @submit.prevent="saveProduct" class="space-y-4">
            <!-- Product Image Upload -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Product Image</label>
              <div class="space-y-3">
                <!-- Image Preview -->
                <div v-if="imagePreview" class="relative inline-block">
                  <img
                    :src="imagePreview"
                    alt="Product preview"
                    class="w-32 h-32 object-cover rounded-lg border border-gray-200"
                  />
                  <button
                    type="button"
                    @click="removeImage"
                    class="absolute -top-2 -right-2 bg-red-500 text-white rounded-full w-6 h-6 flex items-center justify-center text-xs hover:bg-red-600 transition-colors"
                  >
                    ×
                  </button>
                </div>
                
                <!-- File Input -->
                <div class="flex items-center space-x-3">
                  <input
                    id="product-image"
                    type="file"
                    accept="image/*"
                    @change="handleImageSelect"
                    class="hidden"
                  />
                  <label
                    for="product-image"
                    class="cursor-pointer inline-flex items-center px-4 py-2 bg-gray-100 text-gray-700 text-sm font-medium rounded-lg hover:bg-gray-200 transition-colors duration-200"
                  >
                    <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"/>
                    </svg>
                    Choose Image
                  </label>
                  <span class="text-sm text-gray-500">Max 5MB (JPG, PNG, GIF, WebP)</span>
                </div>
              </div>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Product Name</label>
              <input
                v-model="productForm.productName"
                type="text"
                required
                class="form-input"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Product Code</label>
              <input
                v-model="productForm.productCode"
                type="text"
                required
                class="form-input"
                placeholder="Enter unique product code"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Description</label>
              <textarea
                v-model="productForm.description"
                rows="3"
                class="form-input"
              ></textarea>
            </div>
            
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Barcode</label>
                <input
                  v-model="productForm.barcode"
                  type="text"
                  class="form-input"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Unit of Measure</label>
                <select
                  v-model="productForm.unitOfMeasure"
                  required
                  class="form-input"
                >
                  <option 
                    v-for="option in unitOfMeasureOptions" 
                    :key="option.value" 
                    :value="option.value"
                  >
                    {{ option.label }}
                  </option>
                </select>
              </div>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Category</label>
              <select
                v-model="productForm.categoryId"
                required
                class="form-input"
              >
                <option value="">Select a category</option>
                <option 
                  v-for="category in categories" 
                  :key="category.id" 
                  :value="category.id"
                >
                  {{ category.categoryName }}
                </option>
              </select>
            </div>
            
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Unit Price</label>
                <input
                  v-model="productForm.unitPrice"
                  type="number"
                  step="0.01"
                  required
                  class="form-input"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Cost Price</label>
                <input
                  v-model="productForm.costPrice"
                  type="number"
                  step="0.01"
                  class="form-input"
                />
              </div>
            </div>
            
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Stock Quantity</label>
                <input
                  v-model="productForm.stockQuantity"
                  type="number"
                  class="form-input"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Min Stock Level</label>
                <input
                  v-model="productForm.minStockLevel"
                  type="number"
                  class="form-input"
                />
              </div>
            </div>
            
            <div class="flex items-center">
              <input
                v-model="productForm.isActive"
                type="checkbox"
                class="h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded"
              />
              <label class="ml-2 block text-sm text-gray-900">Active</label>
            </div>
            
            <div class="flex justify-end space-x-3 pt-4">
              <button
                type="button"
                @click="closeModal"
                class="btn-secondary"
              >
                Cancel
              </button>
              <button
                type="submit"
                :disabled="saving"
                class="btn-primary"
              >
                {{ saving ? 'Saving...' : 'Save' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Stock Modal -->
    <div v-if="showStockModal" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 mb-4">
            Update Stock: {{ selectedProduct?.productName }}
          </h3>
          
          <form @submit.prevent="saveStockUpdate" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Current Stock</label>
              <p class="text-lg font-semibold">{{ selectedProduct?.stockQuantity }}</p>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Movement Type</label>
              <select v-model="stockForm.movementType" required class="form-input">
                <option value="StockIn">Stock In</option>
                <option value="StockOut">Stock Out</option>
                <option value="Adjustment">Adjustment</option>
              </select>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Quantity</label>
              <input
                v-model="stockForm.quantity"
                type="number"
                required
                class="form-input"
              />
            </div>
            
            <div class="flex justify-end space-x-3 pt-4">
              <button
                type="button"
                @click="showStockModal = false"
                class="btn-secondary"
              >
                Cancel
              </button>
              <button
                type="submit"
                :disabled="saving"
                class="btn-primary"
              >
                {{ saving ? 'Updating...' : 'Update Stock' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  middleware: 'auth'
})

// Types
interface Product {
  id: number
  productName: string
  description: string
  productCode: string
  barcode?: string
  categoryId: number
  categoryName?: string
  unitPrice: number
  costPrice: number
  stockQuantity: number
  minStockLevel: number
  unitOfMeasure: string
  imageUrl?: string
  isActive: boolean
}

interface Category {
  id: number
  categoryName: string
  description?: string
  isActive: boolean
}

const { user, logout } = useAuth()
const config = useRuntimeConfig()

// Reactive data
const products = ref<Product[]>([])
const filteredProducts = ref<Product[]>([])
const categories = ref<Category[]>([])
const loading = ref(true)
const saving = ref(false)
const searchTerm = ref('')
const showLowStock = ref(false)
const activeProductActions = ref<number | null>(null)

// Modals
const showCreateModal = ref(false)
const showEditModal = ref(false)
const showStockModal = ref(false)
const selectedProduct = ref<Product | null>(null)

// Forms
const productForm = ref({
  productName: '',
  productCode: '',
  description: '',
  barcode: '',
  categoryId: '',
  unitPrice: 0,
  costPrice: 0,
  stockQuantity: 0,
  minStockLevel: 0,
  unitOfMeasure: 'PCS',
  isActive: true
})

const imageFile = ref<File | null>(null)
const imagePreview = ref<string | null>(null)

const stockForm = ref({
  quantity: 0,
  movementType: 'StockIn'
})

// Methods
const handleLogout = async () => {
  await logout()
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('id-ID').format(amount)
}

const fetchCategories = async () => {
  try {
    const { token } = useAuth()
    
    const response = await $fetch<Category[]>('/api/category/active', {
      headers: {
        Authorization: `Bearer ${token.value}`
      },
      baseURL: config.public.apiBase
    })
    
    categories.value = response
  } catch (error) {
    console.error('Failed to fetch categories:', error)
  }
}

const fetchProducts = async () => {
  try {
    loading.value = true
    const { token } = useAuth()
    
    const response = await $fetch<Product[]>('/api/product', {
      headers: {
        Authorization: `Bearer ${token.value}`
      },
      baseURL: config.public.apiBase
    })
    
    products.value = response
    filterProducts()
  } catch (error) {
    console.error('Failed to fetch products:', error)
  } finally {
    loading.value = false
  }
}

const searchProducts = async () => {
  if (searchTerm.value.trim()) {
    try {
      const { token } = useAuth()
      
      const response = await $fetch<Product[]>(`/api/product/search?searchTerm=${encodeURIComponent(searchTerm.value)}`, {
        headers: {
          Authorization: `Bearer ${token.value}`
        },
        baseURL: config.public.apiBase
      })
      
      products.value = response
    } catch (error) {
      console.error('Failed to search products:', error)
    }
  } else {
    await fetchProducts()
  }
  filterProducts()
}

const filterProducts = () => {
  if (showLowStock.value) {
    filteredProducts.value = products.value.filter(p => p.stockQuantity <= p.minStockLevel)
  } else {
    filteredProducts.value = products.value
  }
}

// Image handling methods
const handleImageSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  
  if (file) {
    // Validate file type
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp']
    if (!allowedTypes.includes(file.type)) {
      alert('Please select a valid image file (JPG, PNG, GIF, or WebP)')
      return
    }
    
    // Validate file size (max 5MB)
    if (file.size > 5 * 1024 * 1024) {
      alert('File size cannot exceed 5MB')
      return
    }
    
    imageFile.value = file
    
    // Create preview
    const reader = new FileReader()
    reader.onload = (e) => {
      imagePreview.value = e.target?.result as string
    }
    reader.readAsDataURL(file)
  }
}

const removeImage = () => {
  imageFile.value = null
  imagePreview.value = null
  
  // Clear file input
  const fileInput = document.getElementById('product-image') as HTMLInputElement
  if (fileInput) {
    fileInput.value = ''
  }
}

const unitOfMeasureOptions = [
  { value: 'PCS', label: 'Pieces' },
  { value: 'KG', label: 'Kilogram' },
  { value: 'G', label: 'Gram' },
  { value: 'L', label: 'Liter' },
  { value: 'ML', label: 'Milliliter' },
  { value: 'BOX', label: 'Box' },
  { value: 'SET', label: 'Set' },
  { value: 'UNIT', label: 'Unit' },
  { value: 'DOZEN', label: 'Dozen' },
  { value: 'PACK', label: 'Pack' },
  { value: 'ROLL', label: 'Roll' }
]

const refreshProducts = async () => {
  searchTerm.value = ''
  showLowStock.value = false
  await fetchProducts()
}

const toggleProductActions = (productId: number) => {
  activeProductActions.value = activeProductActions.value === productId ? null : productId
}

const editProduct = (product: Product) => {
  activeProductActions.value = null
  selectedProduct.value = product
  productForm.value = {
    productName: product.productName,
    productCode: product.productCode,
    description: product.description || '',
    barcode: product.barcode || '',
    categoryId: product.categoryId.toString(),
    unitPrice: product.unitPrice,
    costPrice: product.costPrice,
    stockQuantity: product.stockQuantity,
    minStockLevel: product.minStockLevel,
    unitOfMeasure: product.unitOfMeasure || 'PCS',
    isActive: product.isActive
  }
  
  // Set image preview if product has image
  if (product.imageUrl) {
    imagePreview.value = `${config.public.apiBase}${product.imageUrl}`
  } else {
    imagePreview.value = null
  }
  imageFile.value = null
  
  showEditModal.value = true
}

const updateStock = (product: Product) => {
  activeProductActions.value = null
  selectedProduct.value = product
  stockForm.value = {
    quantity: 0,
    movementType: 'StockIn'
  }
  showStockModal.value = true
}

const deleteProduct = async (product: Product) => {
  activeProductActions.value = null
  if (confirm(`Are you sure you want to delete "${product.productName}"?`)) {
    try {
      const { token } = useAuth()
      
      await $fetch(`/api/product/${product.id}`, {
        method: 'DELETE',
        headers: {
          Authorization: `Bearer ${token.value}`
        },
        baseURL: config.public.apiBase
      })
      
      await fetchProducts()
    } catch (error) {
      console.error('Failed to delete product:', error)
      alert('Failed to delete product')
    }
  }
}

const saveProduct = async () => {
  try {
    saving.value = true
    const { token } = useAuth()
    
    // Create FormData for file upload
    const formData = new FormData()
    formData.append('ProductName', productForm.value.productName)
    formData.append('ProductCode', productForm.value.productCode)
    formData.append('Description', productForm.value.description)
    formData.append('Barcode', productForm.value.barcode)
    formData.append('UnitPrice', productForm.value.unitPrice.toString())
    formData.append('CostPrice', productForm.value.costPrice.toString())
    formData.append('StockQuantity', productForm.value.stockQuantity.toString())
    formData.append('MinStockLevel', productForm.value.minStockLevel.toString())
    formData.append('CategoryId', productForm.value.categoryId)
    formData.append('UnitOfMeasure', productForm.value.unitOfMeasure)
    formData.append('IsActive', productForm.value.isActive.toString())
    
    // Add image if selected
    if (imageFile.value) {
      formData.append('Image', imageFile.value)
    }
    
    if (showEditModal.value && selectedProduct.value) {
      // Update existing product
      await $fetch(`/api/product/${selectedProduct.value.id}`, {
        method: 'PUT',
        headers: {
          Authorization: `Bearer ${token.value || ''}`
        },
        body: formData,
        baseURL: config.public.apiBase
      })
    } else {
      // Create new product
      await $fetch('/api/product', {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${token.value || ''}`
        },
        body: formData,
        baseURL: config.public.apiBase
      })
    }
    
    await fetchProducts()
    closeModal()
  } catch (error) {
    console.error('Failed to save product:', error)
    alert('Failed to save product')
  } finally {
    saving.value = false
  }
}

const saveStockUpdate = async () => {
  try {
    saving.value = true
    const { token } = useAuth()
    
    await $fetch(`/api/product/${selectedProduct.value?.id}/stock`, {
      method: 'POST',
      body: stockForm.value,
      headers: {
        Authorization: `Bearer ${token.value}`
      },
      baseURL: config.public.apiBase
    })
    
    showStockModal.value = false
    await fetchProducts()
  } catch (error) {
    console.error('Failed to update stock:', error)
    alert('Failed to update stock')
  } finally {
    saving.value = false
  }
}

const resetForm = () => {
  productForm.value = {
    productName: '',
    productCode: '',
    description: '',
    barcode: '',
    categoryId: '',
    unitPrice: 0,
    costPrice: 0,
    stockQuantity: 0,
    minStockLevel: 0,
    unitOfMeasure: 'PCS',
    isActive: true
  }
  imageFile.value = null
  imagePreview.value = null
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  selectedProduct.value = null
  resetForm()
}

// Watchers
watch(showLowStock, () => {
  filterProducts()
})

// Initialize
onMounted(async () => {
  await fetchCategories()
  await fetchProducts()
})
</script>