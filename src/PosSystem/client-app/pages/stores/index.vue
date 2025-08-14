<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Main Content -->
    <main class="max-w-7xl mx-auto py-4 sm:py-6 lg:py-8 px-3 sm:px-4 lg:px-8">
      <!-- Header Section -->
      <div class="mb-4 sm:mb-6 lg:mb-8">
        <div class="bg-gradient-to-r from-orange-600 to-orange-700 rounded-lg shadow-lg p-4 sm:p-6 lg:p-8 text-white">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-xl sm:text-2xl lg:text-3xl font-bold mb-1 sm:mb-2">Store Management</h1>
              <p class="text-orange-100 text-sm sm:text-base lg:text-lg">Manage store locations and information</p>
            </div>
            <div class="hidden sm:block">
              <svg class="w-10 h-10 sm:w-12 sm:h-12 lg:w-16 lg:h-16 text-orange-200" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"></path>
              </svg>
            </div>
          </div>
        </div>
      </div>

      <!-- Actions Bar -->
      <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-3 sm:p-4 lg:p-6 mb-4 sm:mb-6">
        <div class="flex flex-col gap-3 sm:gap-4">
          <div class="flex flex-col sm:flex-row gap-3 sm:gap-4">
            <div class="relative flex-1">
              <div class="absolute inset-y-0 left-0 pl-2.5 sm:pl-3 flex items-center pointer-events-none">
                <svg class="h-4 w-4 sm:h-5 sm:w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
                </svg>
              </div>
              <input
                v-model="searchQuery"
                type="text"
                placeholder="Search stores..."
                class="pl-8 sm:pl-10 pr-3 sm:pr-4 py-2 sm:py-2.5 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-orange-500 w-full text-sm sm:text-base"
              />
            </div>
            <select
              v-model="statusFilter"
              class="px-3 sm:px-4 py-2 sm:py-2.5 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-orange-500 text-sm sm:text-base"
            >
              <option value="">All Stores</option>
              <option value="active">Active Only</option>
              <option value="inactive">Inactive Only</option>
            </select>
          </div>
          <div class="flex flex-col sm:flex-row gap-3">
            <button
              @click="loadStores"
              :disabled="loading"
              class="inline-flex items-center justify-center px-3 sm:px-4 py-2 border border-gray-300 text-xs sm:text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-orange-500 disabled:opacity-50 transition-colors"
            >
              <svg class="w-3 h-3 sm:w-4 sm:h-4 mr-1.5 sm:mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"></path>
              </svg>
              <span class="hidden sm:inline">Refresh</span>
              <span class="sm:hidden">Refresh</span>
            </button>
            <button
              @click="openCreateModal"
              class="inline-flex items-center justify-center px-3 sm:px-4 py-2 border border-transparent text-xs sm:text-sm font-medium rounded-md text-white bg-orange-600 hover:bg-orange-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-orange-500 transition-colors"
            >
              <svg class="w-3 h-3 sm:w-4 sm:h-4 mr-1.5 sm:mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
              </svg>
              Add Store
            </button>
          </div>
        </div>
      </div>

        <!-- Loading State -->
        <div v-if="loading" class="text-center py-6 sm:py-8">
          <div class="inline-block animate-spin rounded-full h-6 w-6 sm:h-8 sm:w-8 border-b-2 border-orange-600"></div>
          <p class="mt-2 text-gray-600 text-sm sm:text-base">Loading stores...</p>
        </div>

        <!-- Desktop Table -->
        <div v-else class="hidden lg:block bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
          <div class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Store Name</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Code</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Address</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Phone</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Tax Rate</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Currency</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                <tr v-for="store in filteredStores" :key="store.id" class="hover:bg-gray-50">
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{{ store.storeName }}</td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ store.storeCode }}</td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ store.address || '-' }}</td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ store.phone || '-' }}</td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ (store.taxRate * 100).toFixed(2) }}%</td>
                   <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ store.currency }}</td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    <span :class="store.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'" 
                          class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                      {{ store.isActive ? 'Active' : 'Inactive' }}
                    </span>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    <div class="flex space-x-2">
                      <button @click="openEditModal(store)" class="text-indigo-600 hover:text-indigo-900 transition-colors">Edit</button>
                      <button @click="confirmDelete(store)" class="text-red-600 hover:text-red-900 transition-colors">Delete</button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Mobile/Tablet Cards -->
        <div v-else class="lg:hidden space-y-3 sm:space-y-4">
          <div
            v-for="store in filteredStores"
            :key="store.id"
            class="bg-white rounded-lg shadow-sm border border-gray-200 p-3 sm:p-4"
          >
            <div class="flex items-start justify-between mb-3">
              <div class="flex-1 min-w-0">
                <h3 class="text-sm sm:text-base font-semibold text-gray-900 truncate">{{ store.storeName }}</h3>
                <p class="text-xs sm:text-sm text-gray-500 mt-0.5">Code: {{ store.storeCode }}</p>
              </div>
              <span
                :class="[
                  'inline-flex px-2 py-1 text-xs font-semibold rounded-full ml-2 flex-shrink-0',
                  store.isActive
                    ? 'bg-green-100 text-green-800'
                    : 'bg-red-100 text-red-800'
                ]"
              >
                {{ store.isActive ? 'Active' : 'Inactive' }}
              </span>
            </div>
            
            <div class="space-y-2 mb-3">
              <div class="flex items-start">
                <svg class="w-3 h-3 sm:w-4 sm:h-4 text-gray-400 mt-0.5 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"></path>
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path>
                </svg>
                <span class="text-xs sm:text-sm text-gray-600 break-words">{{ store.address || '-' }}</span>
              </div>
              
              <div class="flex items-center">
                <svg class="w-3 h-3 sm:w-4 sm:h-4 text-gray-400 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"></path>
                </svg>
                <span class="text-xs sm:text-sm text-gray-600">{{ store.phone || '-' }}</span>
              </div>
              
              <div class="flex items-center justify-between">
                <div class="flex items-center">
                  <svg class="w-3 h-3 sm:w-4 sm:h-4 text-gray-400 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z"></path>
                  </svg>
                  <span class="text-xs sm:text-sm text-gray-600">Tax: {{ (store.taxRate * 100).toFixed(2) }}%</span>
                </div>
                <div class="flex items-center">
                  <svg class="w-3 h-3 sm:w-4 sm:h-4 text-gray-400 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"></path>
                  </svg>
                  <span class="text-xs sm:text-sm text-gray-600">{{ store.currency }}</span>
                </div>
              </div>
            </div>
            
            <div class="flex flex-col sm:flex-row gap-2 pt-2 border-t border-gray-100">
              <button
                @click="openEditModal(store)"
                class="flex-1 inline-flex items-center justify-center px-3 py-2 border border-gray-300 text-xs sm:text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-orange-500 transition-colors"
              >
                <svg class="w-3 h-3 sm:w-4 sm:h-4 mr-1.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path>
                </svg>
                Edit
              </button>
              <button
                @click="confirmDelete(store)"
                class="flex-1 inline-flex items-center justify-center px-3 py-2 border border-red-300 text-xs sm:text-sm font-medium rounded-md text-red-700 bg-white hover:bg-red-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 transition-colors"
              >
                <svg class="w-3 h-3 sm:w-4 sm:h-4 mr-1.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                </svg>
                Delete
              </button>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-if="!loading && filteredStores.length === 0" class="text-center py-8 sm:py-12 lg:py-16">
          <svg class="mx-auto h-16 w-16 sm:h-20 sm:w-20 lg:h-24 lg:w-24 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-2m-2 0H7m5 0v-9a2 2 0 00-2-2H8a2 2 0 00-2 2v9m8 0V9a2 2 0 012-2h2a2 2 0 012 2v9M7 7h.01M7 3h.01"></path>
          </svg>
          <h3 class="mt-3 sm:mt-4 text-base sm:text-lg font-medium text-gray-900">No stores found</h3>
          <p class="mt-1 sm:mt-2 text-sm sm:text-base text-gray-500">Get started by creating your first store.</p>
          <div class="mt-4 sm:mt-6">
            <button
              @click="openCreateModal"
              class="inline-flex items-center px-4 sm:px-6 py-2 sm:py-3 border border-transparent text-sm sm:text-base font-medium rounded-md text-white bg-orange-600 hover:bg-orange-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-orange-500 transition-colors"
            >
              <svg class="w-4 h-4 sm:w-5 sm:h-5 mr-1.5 sm:mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
              </svg>
              Add First Store
            </button>
          </div>
        </div>
    </main>

    <!-- Create/Edit Modal -->
    <div v-if="showModal" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
      <div class="relative top-4 sm:top-10 mx-auto p-3 sm:p-5 border w-full max-w-md sm:max-w-2xl shadow-lg rounded-md bg-white mx-3 sm:mx-auto">
        <div class="mt-2 sm:mt-3">
          <h3 class="text-base sm:text-lg font-medium text-gray-900 mb-3 sm:mb-4">
            {{ isEditing ? 'Edit Store' : 'Create New Store' }}
          </h3>
          
          <form @submit.prevent="saveStore" class="space-y-3 sm:space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Store Name</label>
              <input
                v-model="storeForm.storeName"
                type="text"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-orange-500 focus:border-orange-500 text-sm sm:text-base"
                placeholder="Enter store name"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Address</label>
              <textarea
                v-model="storeForm.address"
                rows="3"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-orange-500 focus:border-orange-500 text-sm sm:text-base"
                placeholder="Enter store address"
              ></textarea>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Phone</label>
              <input
                v-model="storeForm.phone"
                type="tel"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-orange-500 focus:border-orange-500 text-sm sm:text-base"
                placeholder="Enter phone number"
              />
            </div>
            
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-3 sm:gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Tax Rate (%)</label>
                <input
                  v-model.number="storeForm.taxRate"
                  type="number"
                  step="0.01"
                  min="0"
                  max="1"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-orange-500 focus:border-orange-500 text-sm sm:text-base"
                  placeholder="0.11"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Currency</label>
                <select
                  v-model="storeForm.currency"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-orange-500 focus:border-orange-500 text-sm sm:text-base"
                >
                  <option value="IDR">IDR</option>
                  <option value="USD">USD</option>
                  <option value="EUR">EUR</option>
                </select>
              </div>
            </div>
            
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-3 sm:gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Latitude</label>
                <input
                  v-model="storeForm.latitude"
                  type="text"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-orange-500 focus:border-orange-500 text-sm sm:text-base"
                  placeholder="e.g., -6.2088"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Longitude</label>
                <input
                  v-model="storeForm.longitude"
                  type="text"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-orange-500 focus:border-orange-500 text-sm sm:text-base"
                  placeholder="e.g., 106.8456"
                />
              </div>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Google Maps Link</label>
              <input
                v-model="storeForm.googleMapsLink"
                type="url"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-orange-500 focus:border-orange-500 text-sm sm:text-base"
                placeholder="https://maps.google.com/..."
              />
            </div>
            
            <div v-if="isEditing">
              <label class="flex items-center">
                <input
                  v-model="storeForm.isActive"
                  type="checkbox"
                  class="rounded border-gray-300 text-orange-600 shadow-sm focus:border-orange-300 focus:ring focus:ring-orange-200 focus:ring-opacity-50"
                />
                <span class="ml-2 text-sm text-gray-700">Active</span>
              </label>
            </div>
            
            <div class="flex flex-col sm:flex-row justify-end gap-2 sm:gap-3 pt-3 sm:pt-4">
              <button
                type="button"
                @click="closeModal"
                class="px-4 py-2 text-xs sm:text-sm font-medium text-gray-700 bg-gray-200 hover:bg-gray-300 rounded-md"
              >
                Cancel
              </button>
              <button
                type="submit"
                :disabled="saving"
                class="px-4 py-2 text-xs sm:text-sm font-medium text-white bg-orange-600 hover:bg-orange-700 rounded-md disabled:opacity-50"
              >
                {{ saving ? 'Saving...' : (isEditing ? 'Update' : 'Create') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div v-if="showDeleteModal" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
      <div class="relative top-4 sm:top-20 mx-auto p-3 sm:p-5 border w-full max-w-sm sm:max-w-md shadow-lg rounded-md bg-white mx-3 sm:mx-auto">
        <div class="mt-2 sm:mt-3 text-center">
          <div class="mx-auto flex items-center justify-center h-10 w-10 sm:h-12 sm:w-12 rounded-full bg-red-100">
            <svg class="h-5 w-5 sm:h-6 sm:w-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
            </svg>
          </div>
          <h3 class="text-base sm:text-lg font-medium text-gray-900 mt-2">Delete Store</h3>
          <p class="text-xs sm:text-sm text-gray-500 mt-2">
            Are you sure you want to delete "{{ storeToDelete?.storeName }}"? This action cannot be undone.
          </p>
          <div class="flex flex-col sm:flex-row justify-center gap-2 sm:gap-3 mt-3 sm:mt-4">
            <button
              @click="closeDeleteModal"
              class="px-4 py-2 text-xs sm:text-sm font-medium text-gray-700 bg-gray-200 hover:bg-gray-300 rounded-md"
            >
              Cancel
            </button>
            <button
              @click="deleteStore"
              :disabled="deleting"
              class="px-4 py-2 text-xs sm:text-sm font-medium text-white bg-red-600 hover:bg-red-700 rounded-md disabled:opacity-50"
            >
              {{ deleting ? 'Deleting...' : 'Delete' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  middleware: 'auth'
})

interface Store {
  id: number
  storeName: string
  storeCode: string
  address: string
  phone: string
  taxRate: number
  currency: string
  latitude: string
  longitude: string
  googleMapsLink: string
  isActive: boolean
  createdAt: string
}

interface StoreForm {
  storeName: string
  address: string
  phone: string
  taxRate: number
  currency: string
  latitude: string
  longitude: string
  googleMapsLink: string
  isActive: boolean
}

const { user, logout } = useAuth()
const config = useRuntimeConfig()

// Reactive data
const stores = ref<Store[]>([])
const loading = ref(false)
const saving = ref(false)
const deleting = ref(false)
const searchQuery = ref('')
const statusFilter = ref('')
const showModal = ref(false)
const showDeleteModal = ref(false)
const isEditing = ref(false)
const editingStoreId = ref<number | null>(null)
const storeToDelete = ref<Store | null>(null)

const storeForm = ref<StoreForm>({
  storeName: '',
  address: '',
  phone: '',
  taxRate: 0.11,
  currency: 'IDR',
  latitude: '',
  longitude: '',
  googleMapsLink: '',
  isActive: true
})

// Computed
const filteredStores = computed(() => {
  let filtered = stores.value

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(store =>
      store.storeName.toLowerCase().includes(query) ||
      store.storeCode.toLowerCase().includes(query) ||
      store.address.toLowerCase().includes(query)
    )
  }

  if (statusFilter.value) {
    filtered = filtered.filter(store => {
      if (statusFilter.value === 'active') return store.isActive
      if (statusFilter.value === 'inactive') return !store.isActive
      return true
    })
  }

  return filtered
})

// Methods
const handleLogout = async () => {
  await logout()
}

const loadStores = async () => {
  loading.value = true
  try {
    const token = useCookie('auth-token')
    const response = await $fetch<Store[]>(`${config.public.apiBase}/api/Store`, {
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    stores.value = response
  } catch (error) {
    console.error('Failed to load stores:', error)
  } finally {
    loading.value = false
  }
}

const openCreateModal = () => {
  isEditing.value = false
  editingStoreId.value = null
  storeForm.value = {
    storeName: '',
    address: '',
    phone: '',
    taxRate: 0.11,
    currency: 'IDR',
    latitude: '',
    longitude: '',
    googleMapsLink: '',
    isActive: true
  }
  showModal.value = true
}

const openEditModal = (store: Store) => {
  isEditing.value = true
  editingStoreId.value = store.id
  storeForm.value = {
    storeName: store.storeName,
    address: store.address,
    phone: store.phone,
    taxRate: store.taxRate,
    currency: store.currency,
    latitude: store.latitude,
    longitude: store.longitude,
    googleMapsLink: store.googleMapsLink,
    isActive: store.isActive
  }
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
  isEditing.value = false
  editingStoreId.value = null
}

const saveStore = async () => {
  saving.value = true
  try {
    const token = useCookie('auth-token')
    
    if (isEditing.value && editingStoreId.value) {
      // Update existing store
      await $fetch(`${config.public.apiBase}/api/Store/${editingStoreId.value}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token.value || ''}`,
          'Content-Type': 'application/json'
        },
        body: storeForm.value
      })
    } else {
      // Create new store
      await $fetch(`${config.public.apiBase}/api/Store`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token.value || ''}`,
          'Content-Type': 'application/json'
        },
        body: storeForm.value
      })
    }
    
    closeModal()
    await loadStores()
  } catch (error) {
    console.error('Failed to save store:', error)
  } finally {
    saving.value = false
  }
}

const confirmDelete = (store: Store) => {
  storeToDelete.value = store
  showDeleteModal.value = true
}

const closeDeleteModal = () => {
  showDeleteModal.value = false
  storeToDelete.value = null
}

const deleteStore = async () => {
  if (!storeToDelete.value) return
  
  deleting.value = true
  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/Store/${storeToDelete.value.id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    
    closeDeleteModal()
    await loadStores()
  } catch (error) {
    console.error('Failed to delete store:', error)
  } finally {
    deleting.value = false
  }
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString()
}

// Load stores on mount
onMounted(() => {
  loadStores()
})
</script>