<template>
  <div class="min-h-screen bg-neutral-light">
    <!-- Header -->
    <header class="bg-gradient-to-r from-primary to-primary-dark text-black shadow-lg">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center py-6">
          <div class="flex items-center space-x-4">
            <Icon name="heroicons:truck" class="h-8 w-8" />
            <div>
              <h1 class="text-3xl font-bold">Suppliers</h1>
              <p class="text-primary-light text-lg">Manage supplier information and contacts</p>
            </div>
          </div>
          <div class="flex items-center space-x-4">
            <button
              @click="openCreateModal"
              class="bg-white text-primary px-6 py-3 rounded-lg font-semibold hover:bg-primary-light transition-colors flex items-center space-x-2"
            >
              <Icon name="heroicons:plus" class="h-5 w-5" />
              <span>New Supplier</span>
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="max-w-7xl mx-auto py-8 px-4 sm:px-6 lg:px-8">
      <!-- Search and Filters -->
      <div class="bg-white rounded-lg shadow-sm border border-neutral-medium p-6 mb-6">
        <div class="flex flex-col md:flex-row md:items-center md:justify-between space-y-4 md:space-y-0">
          <div class="flex-1 max-w-lg">
            <div class="relative">
              <Icon name="heroicons:magnifying-glass" class="absolute left-3 top-1/2 transform -translate-y-1/2 text-neutral-gray h-5 w-5" />
              <input
                v-model="searchQuery"
                @input="searchSuppliers"
                type="text"
                placeholder="Search suppliers..."
                class="w-full pl-10 pr-4 py-2 border border-neutral-medium rounded-lg focus:ring-primary focus:border-primary"
              />
            </div>
          </div>
          <div class="flex items-center space-x-4">
            <label class="flex items-center">
              <input
                v-model="showActiveOnly"
                @change="filterSuppliers"
                type="checkbox"
                class="rounded border-neutral-medium text-primary focus:ring-primary"
              />
              <span class="ml-2 text-sm text-neutral-gray">Active only</span>
            </label>
            <button
              @click="refreshSuppliers"
              class="text-primary hover:text-primary-dark p-2 rounded-lg hover:bg-primary-light"
              title="Refresh"
            >
              <Icon name="heroicons:arrow-path" class="h-5 w-5" />
            </button>
          </div>
        </div>
      </div>

      <!-- Suppliers Grid -->
      <div v-if="loading" class="text-center py-12">
        <div class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
        <p class="mt-2 text-neutral-gray">Loading suppliers...</p>
      </div>

      <div v-else-if="filteredSuppliers.length === 0" class="text-center py-12">
        <Icon name="heroicons:truck" class="h-12 w-12 text-neutral-gray mx-auto mb-4" />
        <h3 class="text-lg font-medium text-black mb-2">No suppliers found</h3>
        <p class="text-neutral-gray mb-4">
          {{ searchQuery ? 'Try adjusting your search criteria.' : 'Create your first supplier to get started.' }}
        </p>
        <button
          v-if="!searchQuery"
          @click="openCreateModal"
          class="bg-primary text-black px-4 py-2 rounded-lg hover:bg-primary-dark transition-colors"
        >
          Create Supplier
        </button>
      </div>

      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="supplier in filteredSuppliers"
          :key="supplier.id"
          class="bg-white rounded-lg shadow-sm border border-neutral-medium hover:shadow-md transition-shadow"
        >
          <div class="p-6">
            <div class="flex items-start justify-between mb-4">
              <div class="flex-1">
                <h3 class="text-lg font-semibold text-black mb-1">{{ supplier.supplierName }}</h3>
                <p class="text-sm text-neutral-gray mb-2">Code: {{ supplier.supplierCode }}</p>
                <div class="flex items-center space-x-2">
                  <span
                    :class="supplier.isActive ? 'bg-success-light text-success border border-success' : 'bg-danger-light text-danger border border-danger'"
                    class="px-2 py-1 text-xs font-medium rounded-full"
                  >
                    {{ supplier.isActive ? 'Active' : 'Inactive' }}
                  </span>
                </div>
              </div>
              <div class="flex items-center space-x-1">
                <button
                  @click="editSupplier(supplier)"
                  class="text-primary hover:text-primary-dark p-2 rounded-lg hover:bg-primary-light"
                  title="Edit"
                >
                  <Icon name="heroicons:pencil" class="h-4 w-4" />
                </button>
                <button
                  @click="deleteSupplier(supplier.id)"
                  class="text-danger hover:text-danger-dark p-2 rounded-lg hover:bg-danger-light"
                  title="Delete"
                >
                  <Icon name="heroicons:trash" class="h-4 w-4" />
                </button>
              </div>
            </div>
            
            <div class="space-y-2 text-sm text-neutral-gray">
              <div v-if="supplier.contactPerson">
                <Icon name="heroicons:user" class="inline h-4 w-4 mr-1" />
                {{ supplier.contactPerson }}
              </div>
              <div v-if="supplier.phone">
                <Icon name="heroicons:phone" class="inline h-4 w-4 mr-1" />
                {{ supplier.phone }}
              </div>
              <div v-if="supplier.email">
                <Icon name="heroicons:envelope" class="inline h-4 w-4 mr-1" />
                {{ supplier.email }}
              </div>
              <div v-if="supplier.address">
                <Icon name="heroicons:map-pin" class="inline h-4 w-4 mr-1" />
                {{ supplier.address }}
              </div>
            </div>

            <div v-if="supplier.description" class="mt-3 text-sm text-neutral-gray">
              {{ supplier.description }}
            </div>

            <!-- Payment Terms -->
            <div v-if="supplier.paymentTerms" class="mt-4 pt-4 border-t border-neutral-medium">
              <div class="flex items-center justify-between text-sm">
                <span class="text-neutral-gray">Payment Terms:</span>
                <span class="font-medium text-black">{{ supplier.paymentTerms }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Create/Edit Modal -->
    <div v-if="showModal" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-full max-w-2xl shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-black mb-4">
            {{ isEditing ? 'Edit Supplier' : 'Create New Supplier' }}
          </h3>
          
          <form @submit.prevent="saveSupplier" class="space-y-4">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-neutral-gray mb-1">Supplier Name *</label>
                <input
                  v-model="form.supplierName"
                  type="text"
                  required
                  class="w-full px-3 py-2 border border-neutral-medium rounded-md focus:ring-primary focus:border-primary"
                  placeholder="Enter supplier name"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-neutral-gray mb-1">Supplier Code *</label>
                <input
                  v-model="form.supplierCode"
                  type="text"
                  required
                  class="w-full px-3 py-2 border border-neutral-medium rounded-md focus:ring-primary focus:border-primary"
                  placeholder="Enter supplier code"
                />
              </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-neutral-gray mb-1">Contact Person</label>
                <input
                  v-model="form.contactPerson"
                  type="text"
                  class="w-full px-3 py-2 border border-neutral-medium rounded-md focus:ring-primary focus:border-primary"
                  placeholder="Enter contact person name"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-neutral-gray mb-1">Phone</label>
                <input
                  v-model="form.phone"
                  type="tel"
                  class="w-full px-3 py-2 border border-neutral-medium rounded-md focus:ring-primary focus:border-primary"
                  placeholder="Enter phone number"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-neutral-gray mb-1">Email</label>
              <input
                v-model="form.email"
                type="email"
                class="w-full px-3 py-2 border border-neutral-medium rounded-md focus:ring-primary focus:border-primary"
                placeholder="Enter email address"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-neutral-gray mb-1">Address</label>
              <textarea
                v-model="form.address"
                rows="3"
                class="w-full px-3 py-2 border border-neutral-medium rounded-md focus:ring-primary focus:border-primary"
                placeholder="Enter supplier address"
              ></textarea>
            </div>

            <div>
              <label class="block text-sm font-medium text-neutral-gray mb-1">Payment Terms</label>
              <input
                v-model="form.paymentTerms"
                type="text"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                placeholder="e.g., Net 30, COD, etc."
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-neutral-gray mb-1">Description</label>
              <textarea
                v-model="form.description"
                rows="3"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                placeholder="Enter supplier description"
              ></textarea>
            </div>

            <div class="flex items-center">
              <input
                v-model="form.isActive"
                type="checkbox"
                class="rounded border-neutral-medium text-primary focus:ring-primary"
              />
              <label class="ml-2 text-sm text-neutral-gray">Active</label>
            </div>
            
            <div class="flex justify-end space-x-3 pt-4 border-t border-neutral-medium">
              <button
                type="button"
                @click="closeModal"
                class="px-4 py-2 text-sm font-medium text-neutral-gray bg-neutral-light hover:bg-neutral-medium rounded-md"
              >
                Cancel
              </button>
              <button
                type="submit"
                :disabled="saving"
                class="px-4 py-2 text-sm font-medium text-black bg-primary hover:bg-primary-dark rounded-md disabled:opacity-50"
              >
                {{ saving ? 'Saving...' : (isEditing ? 'Update' : 'Create') }}
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

const { showError, showWarning } = useAlert()

interface Supplier {
  id: number
  supplierName: string
  supplierCode: string
  contactPerson?: string
  phone?: string
  email?: string
  address?: string
  paymentTerms?: string
  description?: string
  isActive: boolean
}

const { user } = useAuth()
const config = useRuntimeConfig()

// Reactive data
const loading = ref(false)
const saving = ref(false)
const suppliers = ref<Supplier[]>([])
const filteredSuppliers = ref<Supplier[]>([])
const showModal = ref(false)
const isEditing = ref(false)

// Search and filters
const searchQuery = ref('')
const showActiveOnly = ref(true)

// Form data
const form = ref({
  id: 0,
  supplierName: '',
  supplierCode: '',
  contactPerson: '',
  phone: '',
  email: '',
  address: '',
  paymentTerms: '',
  description: '',
  isActive: true
})

// Methods
const loadSuppliers = async () => {
  loading.value = true
  try {
    const token = useCookie('auth-token')
    const response = await $fetch<Supplier[]>(`${config.public.apiBase}/api/Supplier`, {
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    suppliers.value = response
    filterSuppliers()
  } catch (error) {
    console.error('Failed to load suppliers:', error)
  } finally {
    loading.value = false
  }
}

const searchSuppliers = () => {
  filterSuppliers()
}

const filterSuppliers = () => {
  let filtered = suppliers.value

  if (showActiveOnly.value) {
    filtered = filtered.filter(s => s.isActive)
  }

  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(s =>
      s.supplierName.toLowerCase().includes(query) ||
      s.supplierCode.toLowerCase().includes(query) ||
      (s.contactPerson && s.contactPerson.toLowerCase().includes(query)) ||
      (s.email && s.email.toLowerCase().includes(query))
    )
  }

  filteredSuppliers.value = filtered
}

const refreshSuppliers = () => {
  loadSuppliers()
}

const openCreateModal = () => {
  isEditing.value = false
  form.value = {
    id: 0,
    supplierName: '',
    supplierCode: '',
    contactPerson: '',
    phone: '',
    email: '',
    address: '',
    paymentTerms: '',
    description: '',
    isActive: true
  }
  showModal.value = true
}

const editSupplier = (supplier: Supplier) => {
  isEditing.value = true
  form.value = {
    id: supplier.id,
    supplierName: supplier.supplierName,
    supplierCode: supplier.supplierCode,
    contactPerson: supplier.contactPerson || '',
    phone: supplier.phone || '',
    email: supplier.email || '',
    address: supplier.address || '',
    paymentTerms: supplier.paymentTerms || '',
    description: supplier.description || '',
    isActive: supplier.isActive
  }
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
}

const saveSupplier = async () => {
  if (!form.value.supplierName.trim() || !form.value.supplierCode.trim()) {
    showWarning('Please fill in all required fields')
    return
  }

  saving.value = true
  try {
    const token = useCookie('auth-token')
    const payload = {
      supplierName: form.value.supplierName.trim(),
      supplierCode: form.value.supplierCode.trim(),
      contactPerson: form.value.contactPerson.trim() || null,
      phone: form.value.phone.trim() || null,
      email: form.value.email.trim() || null,
      address: form.value.address.trim() || null,
      paymentTerms: form.value.paymentTerms.trim() || null,
      description: form.value.description.trim() || null,
      isActive: form.value.isActive
    }

    if (isEditing.value) {
      await $fetch(`${config.public.apiBase}/api/Supplier/${form.value.id}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token.value || ''}`,
          'Content-Type': 'application/json'
        },
        body: { ...payload, id: form.value.id }
      })
    } else {
      await $fetch(`${config.public.apiBase}/api/Supplier`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token.value || ''}`,
          'Content-Type': 'application/json'
        },
        body: payload
      })
    }

    closeModal()
    await loadSuppliers()
  } catch (error) {
    console.error('Failed to save supplier:', error)
    showError('Failed to save supplier')
  } finally {
    saving.value = false
  }
}

const deleteSupplier = async (id: number) => {
  if (!confirm('Are you sure you want to delete this supplier? This action cannot be undone.')) return

  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/Supplier/${id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    await loadSuppliers()
  } catch (error) {
    console.error('Failed to delete supplier:', error)
    showError('Failed to delete supplier. It may have existing purchase orders or transactions.')
  }
}

// Watch for search and filter changes
watch([searchQuery, showActiveOnly], () => {
  filterSuppliers()
})

// Load data on mount
onMounted(() => {
  loadSuppliers()
})
</script>