<template>
  <div class="min-h-screen bg-neutral-light">
    <!-- Modern Header -->
    <header class="backdrop-blur-sm shadow-sm sticky top-0 z-40">
      <div class="max-w-7xl mx-auto px-3 sm:px-4 lg:px-8">
        <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center py-4 sm:py-6 space-y-3 sm:space-y-0">
          <div class="flex items-center space-x-3 sm:space-x-4">
            <div class="p-1.5 sm:p-2 bg-gradient-to-r from-primary to-primary-dark rounded-xl shadow-lg">
              <svg class="w-5 h-5 sm:w-6 sm:h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
              </svg>
            </div>
            <div>
              <h1 class="text-xl sm:text-2xl font-bold text-black">
                Customer Management
              </h1>
              <p class="text-xs sm:text-sm text-neutral-gray mt-0.5 sm:mt-1">Manage customer information and profiles</p>
            </div>
          </div>
          <div class="flex items-center space-x-3 w-full sm:w-auto">
            <button
              @click="showCreateModal = true"
              class="inline-flex items-center px-3 sm:px-4 py-2 bg-gradient-to-r from-primary to-primary-dark text-black text-xs sm:text-sm font-medium rounded-lg hover:from-primary-dark hover:to-primary focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2 shadow-lg hover:shadow-xl transition-all duration-200 w-full sm:w-auto justify-center"
            >
              <svg class="w-3 h-3 sm:w-4 sm:h-4 mr-1.5 sm:mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
              </svg>
              Add Customer
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="max-w-7xl mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-6 lg:py-8">
      <!-- Enhanced Search and Filters -->
      <div class="bg-white/70 backdrop-blur-sm rounded-2xl shadow-lg border border-neutral-medium p-3 sm:p-4 lg:p-6 mb-4 sm:mb-6 lg:mb-8">
        <div class="flex flex-col sm:flex-row gap-3 sm:gap-4">
          <div class="flex-1 relative">
            <div class="absolute inset-y-0 left-0 pl-2.5 sm:pl-3 flex items-center pointer-events-none">
              <svg class="h-4 w-4 sm:h-5 sm:w-5 text-neutral-gray" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
              </svg>
            </div>
            <input
              v-model="searchTerm"
              type="text"
              placeholder="Search customers by name, email, or phone..."
              class="w-full pl-8 sm:pl-10 pr-3 sm:pr-4 py-2.5 sm:py-3 border border-neutral-medium rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent bg-white/80 backdrop-blur-sm shadow-sm text-sm sm:text-base"
              @input="searchCustomers"
            />
          </div>
          <button
            @click="refreshCustomers"
            :disabled="loading"
            class="inline-flex items-center px-4 sm:px-6 py-2.5 sm:py-3 bg-white text-neutral-gray text-xs sm:text-sm font-medium rounded-xl border border-neutral-medium hover:bg-neutral-light focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2 shadow-sm hover:shadow-md transition-all duration-200 disabled:opacity-50 justify-center"
          >
            <svg class="w-3 h-3 sm:w-4 sm:h-4 mr-1.5 sm:mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"></path>
            </svg>
            <span class="hidden sm:inline">{{ loading ? 'Loading...' : 'Refresh' }}</span>
            <span class="sm:hidden">{{ loading ? '...' : 'Refresh' }}</span>
          </button>
        </div>
      </div>

      <!-- Enhanced Customer List -->
      <div class="bg-white/70 backdrop-blur-sm rounded-2xl shadow-lg border border-neutral-medium overflow-hidden">
        <div class="px-6 py-5 border-b border-neutral-medium bg-gradient-to-r from-neutral-light to-white">
          <div class="flex items-center justify-between">
            <h2 class="text-xl font-semibold text-black flex items-center">
              <svg class="w-5 h-5 mr-2 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
              </svg>
              Customers
            </h2>
            <span class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-primary-light text-primary">
              {{ filteredCustomers.length }} total
            </span>
          </div>
        </div>
        
        <div v-if="loading" class="p-8 sm:p-12 text-center">
          <div class="inline-flex items-center text-neutral-gray">
            <svg class="animate-spin -ml-1 mr-2 sm:mr-3 h-4 w-4 sm:h-5 sm:w-5 text-primary" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            <span class="text-sm sm:text-base">Loading customers...</span>
          </div>
        </div>
        
        <div v-else-if="filteredCustomers.length === 0" class="p-8 sm:p-12 text-center">
          <svg class="mx-auto h-10 w-10 sm:h-12 sm:w-12 text-neutral-gray" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
          </svg>
          <h3 class="mt-3 sm:mt-4 text-base sm:text-lg font-medium text-black">No customers found</h3>
          <p class="mt-2 text-sm text-neutral-gray">Get started by adding your first customer.</p>
        </div>
        
        <div v-else class="divide-y divide-neutral-medium">
          <div
            v-for="customer in filteredCustomers"
            :key="customer.id"
            class="p-3 sm:p-4 lg:p-6 hover:bg-gradient-to-r hover:from-primary-light/50 hover:to-primary-light/50 transition-all duration-200 group"
          >
            <div class="flex flex-col sm:flex-row sm:items-center justify-between space-y-3 sm:space-y-0">
              <div class="flex-1">
                <div class="flex items-center space-x-3 sm:space-x-4">
                  <div class="flex-shrink-0">
                    <div class="w-10 h-10 sm:w-12 sm:h-12 bg-gradient-to-r from-primary to-primary-dark rounded-xl flex items-center justify-center shadow-lg group-hover:shadow-xl transition-shadow duration-200">
                      <span class="text-white font-semibold text-xs sm:text-sm">
                        {{ customer.firstName.charAt(0) }}{{ customer.lastName.charAt(0) }}
                      </span>
                    </div>
                  </div>
                  <div class="flex-1 min-w-0">
                    <div class="flex flex-col sm:flex-row sm:items-center space-y-1 sm:space-y-0 sm:space-x-3">
                      <h3 class="text-base sm:text-lg font-semibold text-black truncate">
                        {{ customer.firstName }} {{ customer.lastName }}
                      </h3>
                      <span
                        :class="customer.isActive ? 'bg-green-100 text-green-800 border-green-200' : 'bg-red-100 text-red-800 border-red-200'"
                        class="inline-flex items-center px-2 sm:px-2.5 py-0.5 rounded-full text-xs font-medium border w-fit"
                      >
                        <span :class="customer.isActive ? 'bg-green-400' : 'bg-red-400'" class="w-1.5 h-1.5 rounded-full mr-1.5"></span>
                        {{ customer.isActive ? 'Active' : 'Inactive' }}
                      </span>
                    </div>
                    <div class="mt-2 flex flex-col sm:flex-row sm:items-center space-y-1 sm:space-y-0 sm:space-x-6 text-xs sm:text-sm text-neutral-gray">
                      <span v-if="customer.email" class="flex items-center">
                        <svg class="w-3 h-3 sm:w-4 sm:h-4 mr-1 sm:mr-1.5 text-neutral-gray" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 4.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path>
                        </svg>
                        <span class="truncate">{{ customer.email }}</span>
                      </span>
                      <span v-if="customer.phone" class="flex items-center">
                        <svg class="w-3 h-3 sm:w-4 sm:h-4 mr-1 sm:mr-1.5 text-neutral-gray" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"></path>
                        </svg>
                        {{ customer.phone }}
                      </span>
                      <span v-if="customer.dateOfBirth" class="flex items-center">
                        <svg class="w-3 h-3 sm:w-4 sm:h-4 mr-1 sm:mr-1.5 text-neutral-gray" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
                        </svg>
                        Born: {{ formatDate(customer.dateOfBirth) }}
                      </span>
                    </div>
                    <div v-if="customer.address" class="mt-2 flex items-start text-xs sm:text-sm text-neutral-gray">
                      <svg class="w-3 h-3 sm:w-4 sm:h-4 mr-1 sm:mr-1.5 text-neutral-gray mt-0.5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"></path>
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path>
                      </svg>
                      <span class="break-words">{{ customer.address }}</span>
                    </div>
                  </div>
                </div>
              </div>
              <div class="flex items-center space-x-2 mt-3 sm:mt-0">
                <button
                  @click="editCustomer(customer)"
                  class="inline-flex items-center px-2 sm:px-3 py-1 sm:py-1.5 text-xs sm:text-sm font-medium text-primary bg-primary-light hover:bg-primary rounded-lg border border-primary hover:border-primary-dark transition-all duration-200"
                >
                  <svg class="w-3 h-3 sm:w-4 sm:h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path>
                  </svg>
                  Edit
                </button>
                <button
                  @click="deleteCustomer(customer)"
                  class="inline-flex items-center px-2 sm:px-3 py-1 sm:py-1.5 text-xs sm:text-sm font-medium text-danger bg-danger-light hover:bg-danger rounded-lg border border-danger hover:border-danger-dark transition-all duration-200"
                >
                  <svg class="w-3 h-3 sm:w-4 sm:h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                  </svg>
                  Delete
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Create/Edit Customer Modal -->
    <div
      v-if="showCreateModal || showEditModal"
      class="fixed inset-0 modal-backdrop flex items-center justify-center p-3 sm:p-4 z-50"
    >
      <div class="bg-white rounded-lg max-w-md w-full max-h-[90vh] overflow-y-auto mx-3 sm:mx-0">
        <div class="px-4 sm:px-6 py-3 sm:py-4 border-b border-neutral-medium">
          <h3 class="text-base sm:text-lg font-medium text-black">
            {{ showCreateModal ? 'Add New Customer' : 'Edit Customer' }}
          </h3>
        </div>
        
        <form @submit.prevent="saveCustomer" class="p-4 sm:p-6 space-y-3 sm:space-y-4">
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-3 sm:gap-4">
            <div>
              <label class="block text-xs sm:text-sm font-medium text-black mb-1">
                First Name *
              </label>
              <input
                v-model="customerForm.firstName"
                type="text"
                required
                class="w-full px-3 py-2 border border-neutral-medium rounded-md focus:ring-2 focus:ring-primary focus:border-transparent text-sm"
              />
            </div>
            <div>
              <label class="block text-xs sm:text-sm font-medium text-black mb-1">
                Last Name
              </label>
              <input
                v-model="customerForm.lastName"
                type="text"
                class="w-full px-3 py-2 border border-neutral-medium rounded-md focus:ring-2 focus:ring-primary focus:border-transparent text-sm"
              />
            </div>
          </div>
          
          <div>
            <label class="block text-xs sm:text-sm font-medium text-black mb-1">
              Email
            </label>
            <input
              v-model="customerForm.email"
              type="email"
              class="w-full px-3 py-2 border border-neutral-medium rounded-md focus:ring-2 focus:ring-primary focus:border-transparent text-sm"
            />
          </div>
          
          <div>
            <label class="block text-xs sm:text-sm font-medium text-black mb-1">
              Phone *
            </label>
            <input
              v-model="customerForm.phone"
              type="tel"
              required
              class="w-full px-3 py-2 border border-neutral-medium rounded-md focus:ring-2 focus:ring-primary focus:border-transparent text-sm"
            />
          </div>
          
          <div>
            <label class="block text-xs sm:text-sm font-medium text-black mb-1">
              Date of Birth
            </label>
            <input
              v-model="customerForm.dateOfBirth"
              type="date"
              class="w-full px-3 py-2 border border-neutral-medium rounded-md focus:ring-2 focus:ring-primary focus:border-transparent text-sm"
            />
          </div>
          
          <div>
            <label class="block text-xs sm:text-sm font-medium text-black mb-1">
              Address
            </label>
            <textarea
              v-model="customerForm.address"
              rows="3"
              class="w-full px-3 py-2 border border-neutral-medium rounded-md focus:ring-2 focus:ring-primary focus:border-transparent text-sm"
            ></textarea>
          </div>
          
          <div class="flex items-center">
            <input
              v-model="customerForm.isActive"
              type="checkbox"
              id="isActive"
              class="h-4 w-4 text-primary focus:ring-primary border-neutral-medium rounded"
            />
            <label for="isActive" class="ml-2 block text-xs sm:text-sm text-black">
              Active Customer
            </label>
          </div>
          
          <div class="flex flex-col sm:flex-row justify-end space-y-2 sm:space-y-0 sm:space-x-3 pt-3 sm:pt-4">
            <button
              type="button"
              @click="closeModal"
              class="btn-secondary w-full sm:w-auto px-4 py-2 text-sm"
            >
              Cancel
            </button>
            <button
              type="submit"
              :disabled="saving"
              class="btn-primary w-full sm:w-auto px-4 py-2 text-sm"
            >
              {{ saving ? 'Saving...' : (showCreateModal ? 'Create Customer' : 'Update Customer') }}
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

const { showSuccess, showError } = useAlert()

interface Customer {
  id: number
  firstName: string
  lastName: string
  email?: string
  phone: string
  dateOfBirth?: string
  address?: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

interface CustomerForm {
  firstName: string
  lastName: string
  email: string
  phone: string
  dateOfBirth: string
  address: string
  isActive: boolean
}

const { user, token } = useAuth()
const config = useRuntimeConfig()

// Reactive data
const customers = ref<Customer[]>([])
const filteredCustomers = ref<Customer[]>([])
const searchTerm = ref('')
const loading = ref(false)
const saving = ref(false)

// Modal states
const showCreateModal = ref(false)
const showEditModal = ref(false)
const editingCustomer = ref<Customer | null>(null)

// Form data
const customerForm = ref<CustomerForm>({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  dateOfBirth: '',
  address: '',
  isActive: true
})

// Computed
const filteredCustomersComputed = computed(() => {
  if (!searchTerm.value.trim()) {
    return customers.value
  }
  
  const search = searchTerm.value.toLowerCase()
  return customers.value.filter(customer =>
    customer.firstName.toLowerCase().includes(search) ||
    customer.lastName.toLowerCase().includes(search) ||
    customer.email?.toLowerCase().includes(search) ||
    customer.phone.includes(search)
  )
})

// Watch for search changes
watch(filteredCustomersComputed, (newValue) => {
  filteredCustomers.value = newValue
}, { immediate: true })

// Methods
const loadCustomers = async () => {
  try {
    loading.value = true
    
    const response = await $fetch<Customer[]>('/api/customer', {
      headers: {
        Authorization: `Bearer ${token.value || ''}`
      },
      baseURL: config.public.apiBase
    })
    
    customers.value = response
  } catch (error) {
    console.error('Failed to load customers:', error)
    showError('Failed to load customers')
  } finally {
    loading.value = false
  }
}

const searchCustomers = () => {
  // Search is handled by computed property
}

const refreshCustomers = async () => {
  await loadCustomers()
}

const editCustomer = (customer: Customer) => {
  editingCustomer.value = customer
  customerForm.value = {
    firstName: customer.firstName,
    lastName: customer.lastName,
    email: customer.email || '',
    phone: customer.phone,
    dateOfBirth: customer.dateOfBirth || '',
    address: customer.address || '',
    isActive: customer.isActive
  }
  showEditModal.value = true
}

const deleteCustomer = async (customer: Customer) => {
  if (!confirm(`Are you sure you want to delete ${customer.firstName} ${customer.lastName}?`)) {
    return
  }
  
  try {
    await $fetch(`/api/customer/${customer.id}`, {
      method: 'DELETE',
      headers: {
        Authorization: `Bearer ${token.value || ''}`
      },
      baseURL: config.public.apiBase
    })
    
    await loadCustomers()
    showSuccess('Customer deleted successfully!')
  } catch (error) {
    console.error('Failed to delete customer:', error)
    showError('Failed to delete customer')
  }
}

const saveCustomer = async () => {
  try {
    saving.value = true
    
    const customerData = {
      firstName: customerForm.value.firstName,
      lastName: customerForm.value.lastName,
      email: customerForm.value.email || null,
      phone: customerForm.value.phone,
      dateOfBirth: customerForm.value.dateOfBirth || null,
      address: customerForm.value.address || null,
      isActive: customerForm.value.isActive
    }
    
    if (showCreateModal.value) {
      await $fetch('/api/customer', {
        method: 'POST',
        body: customerData,
        headers: {
          Authorization: `Bearer ${token.value || ''}`
        },
        baseURL: config.public.apiBase
      })
      showSuccess('Customer created successfully!')
    } else {
      await $fetch(`/api/customer/${editingCustomer.value?.id}`, {
        method: 'PUT',
        body: customerData,
        headers: {
          Authorization: `Bearer ${token.value || ''}`
        },
        baseURL: config.public.apiBase
      })
      showSuccess('Customer updated successfully!')
    }
    
    closeModal()
    await loadCustomers()
  } catch (error) {
    console.error('Failed to save customer:', error)
    showError('Failed to save customer')
  } finally {
    saving.value = false
  }
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  editingCustomer.value = null
  customerForm.value = {
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    dateOfBirth: '',
    address: '',
    isActive: true
  }
}

const formatDate = (dateString: string): string => {
  return new Date(dateString).toLocaleDateString()
}

// Load customers on mount
onMounted(() => {
  loadCustomers()
})
</script>