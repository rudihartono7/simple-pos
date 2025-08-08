<template>
  <div>
    <!-- Header Section -->
    <div class="mb-8">
      <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 class="text-3xl font-bold text-gray-900">Category Management</h1>
          <p class="mt-2 text-sm text-gray-600">Manage product categories and classifications</p>
        </div>
        <div class="mt-4 sm:mt-0">
          <button
            @click="showCreateModal = true"
            class="inline-flex items-center px-4 py-2 bg-gradient-to-r from-indigo-600 to-purple-600 text-white text-sm font-medium rounded-xl hover:from-indigo-700 hover:to-purple-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 transition-all duration-200 shadow-lg hover:shadow-xl"
          >
            <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
            </svg>
            Add New Category
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
              placeholder="Search categories by name or description..."
              class="block w-full pl-10 pr-3 py-3 border border-gray-200 rounded-xl leading-5 bg-gray-50 placeholder-gray-500 focus:outline-none focus:placeholder-gray-400 focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all duration-200"
              @input="filterCategories"
            />
          </div>
        </div>
        <div class="flex flex-wrap gap-3">
          <label class="inline-flex items-center px-4 py-3 bg-gray-100 text-gray-700 rounded-xl text-sm font-medium hover:bg-gray-200 transition-all duration-200 cursor-pointer">
            <input
              v-model="showActiveOnly"
              type="checkbox"
              class="h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded mr-2"
              @change="filterCategories"
            />
            Active Only
          </label>
          <button
            @click="refreshCategories"
            :disabled="loading"
            class="px-4 py-3 bg-gray-100 text-gray-700 rounded-xl text-sm font-medium hover:bg-gray-200 transition-all duration-200 disabled:opacity-50"
          >
            <svg class="w-4 h-4 inline mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"/>
            </svg>
            {{ loading ? 'Loading...' : 'Refresh' }}
          </button>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="flex items-center justify-center py-12">
      <div class="text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600 mx-auto"></div>
        <p class="mt-4 text-gray-600">Loading categories...</p>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else-if="filteredCategories.length === 0" class="text-center py-12">
      <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"/>
      </svg>
      <h3 class="mt-4 text-lg font-medium text-gray-900">No categories found</h3>
      <p class="mt-2 text-gray-500">Get started by creating your first category.</p>
      <div class="mt-6">
        <button
          @click="showCreateModal = true"
          class="inline-flex items-center px-4 py-2 bg-indigo-600 text-white text-sm font-medium rounded-xl hover:bg-indigo-700 transition-colors duration-200"
        >
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
          </svg>
          Add Category
        </button>
      </div>
    </div>

    <!-- Categories Grid -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div
        v-for="category in filteredCategories"
        :key="category.id"
        class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6 hover:shadow-lg transition-all duration-200 hover:border-indigo-200"
      >
        <!-- Category Header -->
        <div class="flex items-start justify-between mb-4">
          <div class="flex items-center space-x-3 flex-1">
            <div class="flex-shrink-0">
              <div class="w-12 h-12 bg-gradient-to-br from-purple-500 to-indigo-600 rounded-xl flex items-center justify-center shadow-lg">
                <span class="text-white font-bold text-lg">
                  {{ category.categoryName.charAt(0).toUpperCase() }}
                </span>
              </div>
            </div>
            <div class="flex-1 min-w-0">
              <h3 class="text-lg font-semibold text-gray-900 truncate">
                {{ category.categoryName }}
              </h3>
              <span
                :class="[
                  'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium mt-1',
                  category.isActive 
                    ? 'bg-green-100 text-green-800' 
                    : 'bg-red-100 text-red-800'
                ]"
              >
                <svg class="w-3 h-3 mr-1" fill="currentColor" viewBox="0 0 20 20">
                  <path v-if="category.isActive" fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
                  <path v-else fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
                </svg>
                {{ category.isActive ? 'Active' : 'Inactive' }}
              </span>
            </div>
          </div>
          <div class="relative ml-4">
            <button
              @click="toggleCategoryActions(category.id)"
              class="p-2 text-gray-400 hover:text-gray-600 rounded-lg hover:bg-gray-100 transition-colors duration-200"
            >
              <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                <path d="M10 6a2 2 0 110-4 2 2 0 010 4zM10 12a2 2 0 110-4 2 2 0 010 4zM10 18a2 2 0 110-4 2 2 0 010 4z"/>
              </svg>
            </button>
            
            <!-- Actions Dropdown -->
            <div
              v-if="activeCategoryActions === category.id"
              class="absolute right-0 mt-2 w-48 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-10"
            >
              <button
                @click="editCategory(category)"
                class="w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-50 flex items-center"
              >
                <svg class="w-4 h-4 mr-3 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                </svg>
                Edit Category
              </button>
              <button
                @click="deleteCategory(category)"
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

        <!-- Category Description -->
        <div class="mb-4">
          <p class="text-sm text-gray-600 line-clamp-3">
            {{ category.description || 'No description provided' }}
          </p>
        </div>

        <!-- Category Metadata -->
        <div class="bg-gray-50 rounded-xl p-4 mb-4">
          <div class="text-xs text-gray-500 space-y-1">
            <div class="flex items-center justify-between">
              <span>Created:</span>
              <span class="font-medium">{{ formatDate(category.createdAt) }}</span>
            </div>
            <div v-if="category.updatedAt && category.updatedAt !== category.createdAt" class="flex items-center justify-between">
              <span>Updated:</span>
              <span class="font-medium">{{ formatDate(category.updatedAt) }}</span>
            </div>
          </div>
        </div>

        <!-- Action Button -->
        <div class="flex justify-end">
          <button
            @click="editCategory(category)"
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

    <!-- Create/Edit Category Modal -->
    <div
      v-if="showCreateModal || showEditModal"
      class="fixed inset-0 modal-backdrop flex items-center justify-center p-4 z-50"
    >
      <div class="bg-white rounded-lg max-w-md w-full">
        <div class="px-6 py-4 border-b border-gray-200">
          <h3 class="text-lg font-medium text-gray-900">
            {{ showCreateModal ? 'Add New Category' : 'Edit Category' }}
          </h3>
        </div>
        
        <form @submit.prevent="saveCategory" class="p-6 space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Category Name *
            </label>
            <input
              v-model="categoryForm.categoryName"
              type="text"
              required
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              placeholder="Enter category name"
            />
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Description
            </label>
            <textarea
              v-model="categoryForm.description"
              rows="3"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              placeholder="Enter category description (optional)"
            ></textarea>
          </div>
          
          <div v-if="showEditModal" class="flex items-center">
            <input
              v-model="categoryForm.isActive"
              type="checkbox"
              id="isActive"
              class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
            />
            <label for="isActive" class="ml-2 block text-sm text-gray-900">
              Active Category
            </label>
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
              {{ saving ? 'Saving...' : (showCreateModal ? 'Create Category' : 'Update Category') }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  middleware: 'auth'
})

interface Category {
  id: number
  categoryName: string
  description?: string
  isActive: boolean
  createdAt: string
  updatedAt?: string
}

interface CategoryForm {
  categoryName: string
  description: string
  isActive: boolean
}

const { user, token } = useAuth()
const config = useRuntimeConfig()

// Reactive data
const categories = ref<Category[]>([])
const filteredCategories = ref<Category[]>([])
const searchTerm = ref('')
const showActiveOnly = ref(false)
const loading = ref(false)
const saving = ref(false)
const activeCategoryActions = ref<number | null>(null)

// Modal states
const showCreateModal = ref(false)
const showEditModal = ref(false)
const editingCategory = ref<Category | null>(null)

// Form data
const categoryForm = ref<CategoryForm>({
  categoryName: '',
  description: '',
  isActive: true
})

// Methods
const loadCategories = async () => {
  try {
    loading.value = true
    
    const response = await $fetch<Category[]>('/api/category', {
      headers: {
        Authorization: `Bearer ${token.value || ''}`
      },
      baseURL: config.public.apiBase
    })
    
    categories.value = response
    filterCategories()
  } catch (error) {
    console.error('Failed to load categories:', error)
    alert('Failed to load categories')
  } finally {
    loading.value = false
  }
}

const filterCategories = () => {
  let filtered = categories.value

  // Filter by active status
  if (showActiveOnly.value) {
    filtered = filtered.filter(category => category.isActive)
  }

  // Filter by search term
  if (searchTerm.value.trim()) {
    const search = searchTerm.value.toLowerCase()
    filtered = filtered.filter(category =>
      category.categoryName.toLowerCase().includes(search) ||
      category.description?.toLowerCase().includes(search)
    )
  }

  filteredCategories.value = filtered
}

const refreshCategories = async () => {
  await loadCategories()
}

const toggleCategoryActions = (categoryId: number) => {
  activeCategoryActions.value = activeCategoryActions.value === categoryId ? null : categoryId
}

const editCategory = (category: Category) => {
  activeCategoryActions.value = null
  editingCategory.value = category
  categoryForm.value = {
    categoryName: category.categoryName,
    description: category.description || '',
    isActive: category.isActive
  }
  showEditModal.value = true
}

const deleteCategory = async (category: Category) => {
  if (!confirm(`Are you sure you want to delete the category "${category.categoryName}"?`)) {
    return
  }
  
  activeCategoryActions.value = null
  
  try {
    // Check if category has products first
    const hasProductsResponse = await $fetch<{ hasProducts: boolean }>(`/api/category/${category.id}/has-products`, {
      headers: {
        Authorization: `Bearer ${token.value || ''}`
      },
      baseURL: config.public.apiBase
    })
    
    if (hasProductsResponse.hasProducts) {
      alert('Cannot delete category that has products. Please remove or reassign products first.')
      return
    }
    
    await $fetch(`/api/category/${category.id}`, {
      method: 'DELETE',
      headers: {
        Authorization: `Bearer ${token.value || ''}`
      },
      baseURL: config.public.apiBase
    })
    
    await loadCategories()
    alert('Category deleted successfully!')
  } catch (error) {
    console.error('Failed to delete category:', error)
    alert('Failed to delete category')
  }
}

const saveCategory = async () => {
  try {
    saving.value = true
    
    const categoryData = {
      categoryName: categoryForm.value.categoryName,
      description: categoryForm.value.description || null,
      isActive: categoryForm.value.isActive
    }
    
    if (showCreateModal.value) {
      await $fetch('/api/category', {
        method: 'POST',
        body: categoryData,
        headers: {
          Authorization: `Bearer ${token.value || ''}`
        },
        baseURL: config.public.apiBase
      })
      alert('Category created successfully!')
    } else {
      await $fetch(`/api/category/${editingCategory.value?.id}`, {
        method: 'PUT',
        body: categoryData,
        headers: {
          Authorization: `Bearer ${token.value || ''}`
        },
        baseURL: config.public.apiBase
      })
      alert('Category updated successfully!')
    }
    
    closeModal()
    await loadCategories()
  } catch (error) {
    console.error('Failed to save category:', error)
    alert('Failed to save category')
  } finally {
    saving.value = false
  }
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  editingCategory.value = null
  categoryForm.value = {
    categoryName: '',
    description: '',
    isActive: true
  }
}

const formatDate = (dateString: string): string => {
  return new Date(dateString).toLocaleDateString()
}

// Load categories on mount
onMounted(() => {
  loadCategories()
})
</script>