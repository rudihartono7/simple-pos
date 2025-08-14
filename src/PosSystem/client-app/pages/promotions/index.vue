<template>
  <div class="min-h-screen bg-neutral-light">
    <!-- Modern Header -->
    <header class="bg-white/80 backdrop-blur-sm shadow-sm border-b border-neutral-medium sticky top-0 z-40">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center py-6">
          <div class="flex items-center space-x-4">
            <div class="p-2 bg-gradient-to-r from-primary to-primary-dark rounded-xl shadow-lg">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z"></path>
              </svg>
            </div>
            <div>
              <h1 class="text-2xl font-bold text-black">
                Promotion Management
              </h1>
              <p class="text-sm text-neutral-gray mt-1">Manage discounts and promotional offers</p>
            </div>
          </div>
          <div class="flex items-center space-x-3">
            <button
              @click="openCreateModal"
              class="inline-flex items-center px-4 py-2 bg-gradient-to-r from-primary to-primary-dark text-black text-sm font-medium rounded-lg hover:from-primary-dark hover:to-primary focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2 shadow-lg hover:shadow-xl transition-all duration-200"
            >
              <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
              </svg>
              Add Promotion
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Enhanced Search and Filters -->
      <div class="bg-white/70 backdrop-blur-sm rounded-2xl shadow-lg border border-neutral-medium p-6 mb-8">
        <div class="flex flex-col sm:flex-row gap-4">
          <div class="flex-1 relative">
            <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <svg class="h-5 w-5 text-neutral-gray" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
              </svg>
            </div>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search promotions by name or code..."
              class="w-full pl-10 pr-4 py-3 border border-neutral-medium rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent bg-white/80 backdrop-blur-sm shadow-sm"
            />
          </div>
          <div class="relative">
            <select
              v-model="statusFilter"
              class="appearance-none px-4 py-3 pr-10 border border-neutral-medium rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent bg-white/80 backdrop-blur-sm shadow-sm text-sm font-medium"
            >
              <option value="">All Promotions</option>
              <option value="active">Active Only</option>
              <option value="inactive">Inactive Only</option>
              <option value="expired">Expired</option>
            </select>
            <div class="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none">
              <svg class="h-4 w-4 text-neutral-gray" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
              </svg>
            </div>
          </div>
          <button
            @click="loadAllPromotions"
            :disabled="loading"
            class="inline-flex items-center px-6 py-3 bg-white text-neutral-gray text-sm font-medium rounded-xl border border-neutral-medium hover:bg-neutral-light focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2 shadow-sm hover:shadow-md transition-all duration-200 disabled:opacity-50"
          >
            <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"></path>
            </svg>
            {{ loading ? 'Loading...' : 'Refresh' }}
          </button>
        </div>
      </div>

      <!-- Enhanced Promotions Grid -->
      <div class="bg-white/70 backdrop-blur-sm rounded-2xl shadow-lg border border-neutral-medium overflow-hidden">
        <div class="px-6 py-5 border-b border-neutral-medium bg-gradient-to-r from-neutral-light to-white">
          <div class="flex items-center justify-between">
            <h2 class="text-xl font-semibold text-black flex items-center">
              <svg class="w-5 h-5 mr-2 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z"></path>
              </svg>
              Promotions
            </h2>
            <span class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-primary-light text-primary">
              {{ filteredPromotions.length }} total
            </span>
          </div>
        </div>
        
        <div v-if="loading" class="p-12 text-center">
          <div class="inline-flex items-center text-neutral-gray">
            <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-primary" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            Loading promotions...
          </div>
        </div>
        
        <div v-else-if="filteredPromotions.length === 0" class="p-12 text-center">
          <svg class="mx-auto h-12 w-12 text-neutral-gray" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z"></path>
          </svg>
          <h3 class="mt-4 text-lg font-medium text-black">No promotions found</h3>
          <p class="mt-2 text-sm text-neutral-gray">Get started by creating a new promotion.</p>
        </div>
        
        <div v-else class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div
              v-for="promotion in filteredPromotions"
              :key="promotion.id"
              class="bg-white/80 backdrop-blur-sm border border-neutral-medium rounded-2xl p-6 hover:shadow-xl hover:bg-white transition-all duration-300 group"
            >
              <div class="flex justify-between items-start mb-4">
                <div class="flex-1">
                  <h3 class="text-lg font-semibold text-black mb-2 group-hover:text-primary transition-colors">
                    {{ promotion.promotionName }}
                  </h3>
                  <div class="flex items-center text-sm text-neutral-gray">
                    <svg class="w-4 h-4 mr-1.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z"></path>
                    </svg>
                    Code: {{ promotion.promotionCode }}
                  </div>
                </div>
                <div class="relative">
                  <button
                    @click="togglePromotionActions(promotion.id)"
                    class="p-2 text-neutral-gray hover:text-primary rounded-xl hover:bg-primary-light transition-colors"
                  >
                    <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                      <path d="M10 6a2 2 0 110-4 2 2 0 010 4zM10 12a2 2 0 110-4 2 2 0 010 4zM10 18a2 2 0 110-4 2 2 0 010 4z"></path>
                    </svg>
                  </button>
                  <div
                    v-if="activePromotionActions === promotion.id"
                    class="absolute right-0 mt-2 w-48 bg-white rounded-xl shadow-lg z-10 border border-neutral-medium backdrop-blur-sm"
                  >
                    <div class="py-2">
                      <button
                        @click="openEditModal(promotion)"
                        class="flex items-center w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-purple-50 hover:text-purple-700 transition-colors"
                      >
                        <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path>
                        </svg>
                        Edit Promotion
                      </button>
                      <button
                        @click="confirmDelete(promotion)"
                        class="flex items-center w-full text-left px-4 py-2 text-sm text-red-600 hover:bg-red-50 transition-colors"
                      >
                        <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                        </svg>
                        Delete Promotion
                      </button>
                    </div>
                  </div>
                </div>
              </div>
              
              <div class="flex flex-wrap gap-2 mb-4">
                <span
                  :class="[
                    'inline-flex items-center px-2.5 py-1 text-xs font-medium rounded-full border',
                    getPromotionStatusClass(promotion)
                  ]"
                >
                  <span :class="getPromotionStatus(promotion) === 'Active' ? 'bg-green-400' : getPromotionStatus(promotion) === 'Expired' ? 'bg-red-400' : 'bg-gray-400'" class="w-1.5 h-1.5 rounded-full mr-1.5"></span>
                  {{ getPromotionStatus(promotion) }}
                </span>
                <span
                  :class="[
                    'px-2.5 py-1 text-xs font-medium rounded-full border',
                    promotion.promotionType === 'PERCENTAGE'
                      ? 'bg-blue-50 text-blue-700 border-blue-200'
                      : promotion.promotionType === 'FIXED_AMOUNT'
                      ? 'bg-green-50 text-green-700 border-green-200'
                      : 'bg-purple-50 text-purple-700 border-purple-200'
                  ]"
                >
                  {{ getPromotionTypeLabel(promotion.promotionType) }}
                </span>
              </div>
              
              <div class="space-y-3 text-sm">
                <div class="flex justify-between items-center p-3 bg-gradient-to-r from-purple-50 to-pink-50 rounded-xl">
                  <span class="text-gray-600 flex items-center">
                    <svg class="w-4 h-4 mr-1.5 text-purple-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"></path>
                    </svg>
                    Value:
                  </span>
                  <span class="font-semibold text-purple-700">
                    {{ promotion.promotionType === 'PERCENTAGE' ? `${promotion.value}%` : `$${promotion.value}` }}
                  </span>
                </div>
                <div v-if="promotion.minimumPurchase" class="flex justify-between items-center">
                  <span class="text-gray-600 flex items-center">
                    <svg class="w-4 h-4 mr-1.5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4m0 0L7 3H3m4 10v6a1 1 0 001 1h1m0 0h4m-5 0v-1a1 1 0 011-1h2a1 1 0 011 1v1m-4 0h4"></path>
                    </svg>
                    Min. Purchase:
                  </span>
                  <span class="font-medium">${{ promotion.minimumPurchase }}</span>
                </div>
                <div class="flex justify-between items-center">
                  <span class="text-gray-600 flex items-center">
                    <svg class="w-4 h-4 mr-1.5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
                    </svg>
                    Start Date:
                  </span>
                  <span class="font-medium">{{ formatDate(promotion.startDate) }}</span>
                </div>
                <div class="flex justify-between items-center">
                  <span class="text-gray-600 flex items-center">
                    <svg class="w-4 h-4 mr-1.5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
                    </svg>
                    End Date:
                  </span>
                  <span class="font-medium">{{ formatDate(promotion.endDate) }}</span>
                </div>
                <div v-if="promotion.usageLimit" class="flex justify-between items-center">
                  <span class="text-gray-600 flex items-center">
                    <svg class="w-4 h-4 mr-1.5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                    </svg>
                    Usage Limit:
                  </span>
                  <span class="font-medium">{{ promotion.usageLimit }}</span>
                </div>
                <div v-if="promotion.applicableProducts && promotion.applicableProducts.length > 0" class="flex justify-between items-start">
                  <span class="text-gray-600 flex items-center">
                    <svg class="w-4 h-4 mr-1.5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4"></path>
                    </svg>
                    Products:
                  </span>
                  <span class="font-medium text-right text-sm">{{ promotion.applicableProducts.slice(0, 3).join(', ') }}{{ promotion.applicableProducts.length > 3 ? '...' : '' }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Create/Edit Modal -->
    <div v-if="showModal" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-full max-w-md shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 mb-4">
            {{ isEditing ? 'Edit Promotion' : 'Create New Promotion' }}
          </h3>
          
          <form @submit.prevent="savePromotion" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Promotion Code</label>
              <input
                v-model="promotionForm.promotionCode"
                type="text"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                placeholder="Enter promotion code"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Promotion Name</label>
              <input
                v-model="promotionForm.promotionName"
                type="text"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                placeholder="Enter promotion name"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Promotion Type</label>
              <select
                v-model="promotionForm.promotionType"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Select type</option>
                <option value="PERCENTAGE">Percentage Discount</option>
                <option value="FIXED_AMOUNT">Fixed Amount Discount</option>
              </select>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">
                Value {{ promotionForm.promotionType === 'PERCENTAGE' ? '(%)' : '($)' }}
              </label>
              <input
                v-model.number="promotionForm.value"
                type="number"
                step="0.01"
                min="0"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                placeholder="Enter value"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Minimum Purchase ($)</label>
              <input
                v-model.number="promotionForm.minimumPurchase"
                type="number"
                step="0.01"
                min="0"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                placeholder="Enter minimum purchase amount (optional)"
              />
            </div>
            
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Start Date</label>
                <input
                  v-model="promotionForm.startDate"
                  type="datetime-local"
                  required
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">End Date</label>
                <input
                  v-model="promotionForm.endDate"
                  type="datetime-local"
                  required
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                />
              </div>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Usage Limit</label>
              <input
                v-model.number="promotionForm.usageLimit"
                type="number"
                min="1"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                placeholder="Enter usage limit (optional)"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Applicable Products (SKUs)</label>
              <textarea
                v-model="applicableProductsText"
                rows="3"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                placeholder="Enter product SKUs separated by commas (leave empty for all products)"
              ></textarea>
              <p class="text-xs text-gray-500 mt-1">Separate multiple SKUs with commas</p>
            </div>
            
            <div v-if="isEditing">
              <label class="flex items-center">
                <input
                  v-model="promotionForm.isActive"
                  type="checkbox"
                  class="rounded border-gray-300 text-blue-600 shadow-sm focus:border-blue-300 focus:ring focus:ring-blue-200 focus:ring-opacity-50"
                />
                <span class="ml-2 text-sm text-gray-700">Active</span>
              </label>
            </div>
            
            <div class="flex justify-end space-x-3 pt-4">
              <button
                type="button"
                @click="closeModal"
                class="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-200 hover:bg-gray-300 rounded-md"
              >
                Cancel
              </button>
              <button
                type="submit"
                :disabled="saving"
                class="px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-md disabled:opacity-50"
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
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
        <div class="mt-3 text-center">
          <div class="mx-auto flex items-center justify-center h-12 w-12 rounded-full bg-red-100">
            <svg class="h-6 w-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
            </svg>
          </div>
          <h3 class="text-lg font-medium text-gray-900 mt-2">Delete Promotion</h3>
          <p class="text-sm text-gray-500 mt-2">
            Are you sure you want to delete "{{ promotionToDelete?.promotionName }}"? This action cannot be undone.
          </p>
          <div class="flex justify-center space-x-3 mt-4">
            <button
              @click="closeDeleteModal"
              class="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-200 hover:bg-gray-300 rounded-md"
            >
              Cancel
            </button>
            <button
              @click="deletePromotion"
              :disabled="deleting"
              class="px-4 py-2 text-sm font-medium text-white bg-red-600 hover:bg-red-700 rounded-md disabled:opacity-50"
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

interface Promotion {
  id: number
  promotionCode: string
  promotionName: string
  promotionType: string
  value: number
  minimumPurchase?: number
  startDate: string
  endDate: string
  applicableProducts?: string[]
  usageLimit?: number
  isActive: boolean
  createdAt: string
}

interface PromotionForm {
  promotionCode: string
  promotionName: string
  promotionType: string
  value: number
  minimumPurchase?: number
  startDate: string
  endDate: string
  applicableProducts: string[]
  usageLimit?: number
  isActive: boolean
}

const { user, logout } = useAuth()
const config = useRuntimeConfig()

// Reactive data
const promotions = ref<Promotion[]>([])
const loading = ref(false)
const saving = ref(false)
const deleting = ref(false)
const searchQuery = ref('')
const statusFilter = ref('')
const showModal = ref(false)
const showDeleteModal = ref(false)
const isEditing = ref(false)
const editingPromotionId = ref<number | null>(null)
const promotionToDelete = ref<Promotion | null>(null)
const activePromotionActions = ref<number | null>(null)
const applicableProductsText = ref('')

const promotionForm = ref<PromotionForm>({
  promotionCode: '',
  promotionName: '',
  promotionType: '',
  value: 0,
  minimumPurchase: undefined,
  startDate: '',
  endDate: '',
  applicableProducts: [],
  usageLimit: undefined,
  isActive: true
})

// Computed
const filteredPromotions = computed(() => {
  let filtered = promotions.value

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(promotion =>
      promotion.promotionName.toLowerCase().includes(query) ||
      promotion.promotionCode.toLowerCase().includes(query)
    )
  }

  if (statusFilter.value) {
    const now = new Date()
    filtered = filtered.filter(promotion => {
      const endDate = new Date(promotion.endDate)
      
      if (statusFilter.value === 'active') return promotion.isActive && endDate > now
      if (statusFilter.value === 'inactive') return !promotion.isActive
      if (statusFilter.value === 'expired') return endDate <= now
      return true
    })
  }

  return filtered
})

// Methods
const handleLogout = async () => {
  await logout()
}

const togglePromotionActions = (promotionId: number) => {
  activePromotionActions.value = activePromotionActions.value === promotionId ? null : promotionId
}

const loadActivePromotions = async () => {
  loading.value = true
  try {
    const token = useCookie('auth-token')
    const response = await $fetch<Promotion[]>(`${config.public.apiBase}/api/Promotion/active`, {
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    promotions.value = response
  } catch (error) {
    console.error('Failed to load promotions:', error)
  } finally {
    loading.value = false
  }
}

const loadAllPromotions = async () => {
  loading.value = true
  try {
    const token = useCookie('auth-token')
    // Since there's no "all" endpoint, we'll use the active endpoint for now
    // In a real implementation, you might want to add a GET /api/Promotion endpoint
    const response = await $fetch<Promotion[]>(`${config.public.apiBase}/api/Promotion/active`, {
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    promotions.value = response
  } catch (error) {
    console.error('Failed to load promotions:', error)
  } finally {
    loading.value = false
  }
}

const openCreateModal = () => {
  isEditing.value = false
  editingPromotionId.value = null
  const now = new Date()
  const tomorrow = new Date(now)
  tomorrow.setDate(tomorrow.getDate() + 1)
  
  promotionForm.value = {
    promotionCode: '',
    promotionName: '',
    promotionType: '',
    value: 0,
    minimumPurchase: undefined,
    startDate: now.toISOString().slice(0, 16),
    endDate: tomorrow.toISOString().slice(0, 16),
    applicableProducts: [],
    usageLimit: undefined,
    isActive: true
  }
  applicableProductsText.value = ''
  showModal.value = true
}

const openEditModal = (promotion: Promotion) => {
  activePromotionActions.value = null
  isEditing.value = true
  editingPromotionId.value = promotion.id
  promotionForm.value = {
    promotionCode: promotion.promotionCode,
    promotionName: promotion.promotionName,
    promotionType: promotion.promotionType,
    value: promotion.value,
    minimumPurchase: promotion.minimumPurchase,
    startDate: new Date(promotion.startDate).toISOString().slice(0, 16),
    endDate: new Date(promotion.endDate).toISOString().slice(0, 16),
    applicableProducts: promotion.applicableProducts || [],
    usageLimit: promotion.usageLimit,
    isActive: promotion.isActive
  }
  applicableProductsText.value = (promotion.applicableProducts || []).join(', ')
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
  isEditing.value = false
  editingPromotionId.value = null
}

const savePromotion = async () => {
  saving.value = true
  try {
    const token = useCookie('auth-token')
    
    // Parse applicable products from text
    const applicableProducts = applicableProductsText.value
      .split(',')
      .map(sku => sku.trim())
      .filter(sku => sku.length > 0)
    
    const requestBody = {
      ...promotionForm.value,
      applicableProducts,
      startDate: new Date(promotionForm.value.startDate).toISOString(),
      endDate: new Date(promotionForm.value.endDate).toISOString()
    }
    
    if (isEditing.value && editingPromotionId.value) {
      // Update existing promotion
      await $fetch(`${config.public.apiBase}/api/Promotion/${editingPromotionId.value}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token.value || ''}`,
          'Content-Type': 'application/json'
        },
        body: requestBody
      })
    } else {
      // Create new promotion
      await $fetch(`${config.public.apiBase}/api/Promotion`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token.value || ''}`,
          'Content-Type': 'application/json'
        },
        body: requestBody
      })
    }
    
    closeModal()
    await loadAllPromotions()
  } catch (error) {
    console.error('Failed to save promotion:', error)
  } finally {
    saving.value = false
  }
}

const confirmDelete = (promotion: Promotion) => {
  activePromotionActions.value = null
  promotionToDelete.value = promotion
  showDeleteModal.value = true
}

const closeDeleteModal = () => {
  showDeleteModal.value = false
  promotionToDelete.value = null
}

const deletePromotion = async () => {
  if (!promotionToDelete.value) return
  
  deleting.value = true
  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/Promotion/${promotionToDelete.value.id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    
    closeDeleteModal()
    await loadAllPromotions()
  } catch (error) {
    console.error('Failed to delete promotion:', error)
  } finally {
    deleting.value = false
  }
}

const getPromotionStatus = (promotion: Promotion) => {
  const now = new Date()
  const endDate = new Date(promotion.endDate)
  
  if (!promotion.isActive) return 'Inactive'
  if (endDate <= now) return 'Expired'
  return 'Active'
}

const getPromotionStatusClass = (promotion: Promotion) => {
  const status = getPromotionStatus(promotion)
  
  if (status === 'Active') return 'bg-green-100 text-green-800'
  if (status === 'Expired') return 'bg-red-100 text-red-800'
  return 'bg-gray-100 text-gray-800'
}

const getPromotionTypeLabel = (type: string) => {
  switch (type) {
    case 'PERCENTAGE': return 'Percentage'
    case 'FIXED_AMOUNT': return 'Fixed Amount'
    case 'BUY_X_GET_Y': return 'Buy X Get Y'
    default: return type
  }
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString()
}

// Load promotions on mount
onMounted(() => {
  loadAllPromotions()
})
</script>