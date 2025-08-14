<template>
  <div class="p-3 sm:p-4 lg:p-6">
    <!-- Header -->
    <div class="mb-4 sm:mb-6 lg:mb-8">
      <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between space-y-3 sm:space-y-0">
        <div>
          <h1 class="text-xl sm:text-2xl lg:text-3xl font-bold text-gray-900">User Management</h1>
          <p class="text-gray-600 mt-1 sm:mt-2 text-sm sm:text-base">Manage system users and their permissions</p>
        </div>
        <button
          @click="openCreateModal"
          class="bg-indigo-600 hover:bg-indigo-700 text-white px-4 sm:px-6 py-2 sm:py-3 rounded-xl font-medium shadow-lg hover:shadow-xl transition-all duration-200 flex items-center space-x-2 text-sm sm:text-base w-full sm:w-auto justify-center"
        >
          <svg class="w-4 h-4 sm:w-5 sm:h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
          </svg>
          <span>Add New User</span>
        </button>
      </div>
    </div>

    <!-- Search and Filters -->
    <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-3 sm:p-4 lg:p-6 mb-4 sm:mb-6">
      <div class="flex flex-col sm:flex-row gap-3 sm:gap-4 items-stretch sm:items-center">
        <div class="flex-1 min-w-0">
          <div class="relative">
            <svg class="absolute left-2.5 sm:left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 sm:w-5 sm:h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search users by name, email, or username..."
              class="w-full pl-8 sm:pl-10 pr-3 sm:pr-4 py-2.5 sm:py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all duration-200 text-sm sm:text-base"
            />
          </div>
        </div>
        <div class="flex flex-col sm:flex-row gap-3 sm:gap-4">
          <select
            v-model="roleFilter"
            class="px-3 sm:px-4 py-2.5 sm:py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-indigo-500 focus:border-transparent bg-white text-sm sm:text-base"
          >
            <option value="">All Roles</option>
            <option value="Admin">Admin</option>
            <option value="Manager">Manager</option>
            <option value="Cashier">Cashier</option>
          </select>
          <select
            v-model="statusFilter"
            class="px-3 sm:px-4 py-2.5 sm:py-3 border border-gray-200 rounded-xl focus:ring-2 focus:ring-indigo-500 focus:border-transparent bg-white text-sm sm:text-base"
          >
            <option value="">All Status</option>
            <option value="active">Active Only</option>
            <option value="inactive">Inactive Only</option>
          </select>
          <button
            @click="loadUsers"
            :disabled="loading"
            class="bg-gray-100 hover:bg-gray-200 text-gray-700 px-3 sm:px-4 py-2.5 sm:py-3 rounded-xl font-medium disabled:opacity-50 transition-all duration-200 flex items-center justify-center space-x-2 text-sm sm:text-base"
          >
            <svg class="w-3 h-3 sm:w-4 sm:h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
            </svg>
            <span class="hidden sm:inline">Refresh</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="flex items-center justify-center py-8 sm:py-12">
      <div class="text-center">
        <div class="inline-block animate-spin rounded-full h-6 w-6 sm:h-8 sm:w-8 border-b-2 border-indigo-600"></div>
        <p class="mt-3 sm:mt-4 text-gray-600 font-medium text-sm sm:text-base">Loading users...</p>
      </div>
    </div>

    <!-- Users Grid -->
    <div v-else-if="filteredUsers.length > 0" class="grid gap-3 sm:gap-4">
      <div
        v-for="userItem in filteredUsers"
        :key="userItem.id"
        class="bg-white rounded-2xl shadow-sm border border-gray-100 p-3 sm:p-4 lg:p-6 hover:shadow-md transition-all duration-200"
      >
        <div class="flex flex-col sm:flex-row sm:items-center justify-between space-y-3 sm:space-y-0">
          <div class="flex items-center space-x-3 sm:space-x-4 flex-1">
            <div class="flex-shrink-0">
              <div class="h-10 w-10 sm:h-12 sm:w-12 rounded-full bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center shadow-lg">
                <span class="text-sm sm:text-lg font-bold text-white">
                  {{ userItem.firstName?.charAt(0) }}{{ userItem.lastName?.charAt(0) }}
                </span>
              </div>
            </div>
            <div class="flex-1 min-w-0">
              <div class="flex flex-col sm:flex-row sm:items-center space-y-1 sm:space-y-0 sm:space-x-3 mb-1">
                <h3 class="text-base sm:text-lg font-semibold text-gray-900 truncate">
                  {{ userItem.firstName }} {{ userItem.lastName }}
                </h3>
                <div class="flex flex-wrap gap-2">
                  <span
                    :class="[
                      'px-2 sm:px-3 py-1 text-xs font-medium rounded-full w-fit',
                      userItem.isActive
                        ? 'bg-green-100 text-green-800 border border-green-200'
                        : 'bg-red-100 text-red-800 border border-red-200'
                    ]"
                  >
                    {{ userItem.isActive ? 'Active' : 'Inactive' }}
                  </span>
                  <span class="px-2 sm:px-3 py-1 text-xs font-medium rounded-full bg-indigo-100 text-indigo-800 border border-indigo-200 w-fit">
                    {{ userItem.role }}
                  </span>
                </div>
              </div>
              <div class="text-xs sm:text-sm text-gray-600 space-y-1">
                <div class="flex flex-col sm:flex-row sm:items-center space-y-1 sm:space-y-0 sm:space-x-4">
                  <span class="flex items-center space-x-1">
                    <svg class="w-3 h-3 sm:w-4 sm:h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 12a4 4 0 10-8 0 4 4 0 008 0zm0 0v1.5a2.5 2.5 0 005 0V12a9 9 0 10-9 9m4.5-1.206a8.959 8.959 0 01-4.5 1.207" />
                    </svg>
                    <span class="truncate">{{ userItem.email }}</span>
                  </span>
                  <span class="flex items-center space-x-1">
                    <svg class="w-3 h-3 sm:w-4 sm:h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                    </svg>
                    <span>{{ userItem.userName }}</span>
                  </span>
                </div>
                <div class="flex flex-col sm:flex-row sm:items-center space-y-1 sm:space-y-0 sm:space-x-4">
                  <span v-if="userItem.storeName" class="flex items-center space-x-1">
                    <svg class="w-3 h-3 sm:w-4 sm:h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                    </svg>
                    <span class="truncate">{{ userItem.storeName }}</span>
                  </span>
                  <span v-if="userItem.phoneNumber" class="flex items-center space-x-1">
                    <svg class="w-3 h-3 sm:w-4 sm:h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                    </svg>
                    <span>{{ userItem.phoneNumber }}</span>
                  </span>
                </div>
              </div>
            </div>
          </div>
          <div class="flex items-center space-x-2 mt-3 sm:mt-0">
            <button
              @click="openEditModal(userItem)"
              class="bg-indigo-50 hover:bg-indigo-100 text-indigo-700 px-3 sm:px-4 py-1.5 sm:py-2 rounded-xl text-xs sm:text-sm font-medium transition-all duration-200 flex items-center space-x-1"
            >
              <svg class="w-3 h-3 sm:w-4 sm:h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
              </svg>
              <span>Edit</span>
            </button>
            <div class="relative">
              <button
                @click="toggleUserActions(userItem.id)"
                class="bg-gray-50 hover:bg-gray-100 text-gray-700 px-2.5 sm:px-3 py-1.5 sm:py-2 rounded-xl text-xs sm:text-sm font-medium transition-all duration-200"
              >
                <svg class="w-3 h-3 sm:w-4 sm:h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 5v.01M12 12v.01M12 19v.01M12 6a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2z" />
                </svg>
              </button>
              <div
                v-if="activeUserActions === userItem.id"
                class="absolute right-0 mt-2 w-44 sm:w-48 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-10"
              >
                <button
                  v-if="userItem.isActive"
                  @click="deactivateUser(userItem)"
                  class="w-full text-left px-3 sm:px-4 py-2 text-xs sm:text-sm text-yellow-700 hover:bg-yellow-50 flex items-center space-x-2"
                >
                  <svg class="w-3 h-3 sm:w-4 sm:h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18.364 18.364A9 9 0 005.636 5.636m12.728 12.728L5.636 5.636m12.728 12.728L18.364 5.636M5.636 18.364l12.728-12.728" />
                  </svg>
                  <span>Deactivate</span>
                </button>
                <button
                  v-else
                  @click="activateUser(userItem)"
                  class="w-full text-left px-3 sm:px-4 py-2 text-xs sm:text-sm text-green-700 hover:bg-green-50 flex items-center space-x-2"
                >
                  <svg class="w-3 h-3 sm:w-4 sm:h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                  <span>Activate</span>
                </button>
                <button
                  @click="openResetPasswordModal(userItem)"
                  class="w-full text-left px-3 sm:px-4 py-2 text-xs sm:text-sm text-purple-700 hover:bg-purple-50 flex items-center space-x-2"
                >
                  <svg class="w-3 h-3 sm:w-4 sm:h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 7a2 2 0 012 2m0 0a2 2 0 012 2m-2-2a2 2 0 00-2 2m2-2V5a2 2 0 00-2-2H9a2 2 0 00-2 2v2m0 0a2 2 0 102 2m-2-2a2 2 0 012 2m0 0V9a2 2 0 012-2m-2 2a2 2 0 002-2" />
                  </svg>
                  <span>Reset Password</span>
                </button>
                <button
                  @click="confirmDelete(userItem)"
                  class="w-full text-left px-3 sm:px-4 py-2 text-xs sm:text-sm text-red-700 hover:bg-red-50 flex items-center space-x-2"
                >
                  <svg class="w-3 h-3 sm:w-4 sm:h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                  </svg>
                  <span>Delete</span>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else class="text-center py-8 sm:py-12 lg:py-16">
      <div class="mx-auto w-16 h-16 sm:w-20 sm:h-20 lg:w-24 lg:h-24 bg-gray-100 rounded-full flex items-center justify-center mb-4 sm:mb-6">
        <svg class="w-8 h-8 sm:w-10 sm:h-10 lg:w-12 lg:h-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5 0a4 4 0 11-8 0 4 4 0 018 0z"></path>
        </svg>
      </div>
      <h3 class="text-lg sm:text-xl font-semibold text-gray-900 mb-2">No users found</h3>
      <p class="text-gray-600 mb-4 sm:mb-6 text-sm sm:text-base">Get started by creating your first user account.</p>
      <button
        @click="openCreateModal"
        class="bg-indigo-600 hover:bg-indigo-700 text-white px-4 sm:px-6 py-2 sm:py-3 rounded-xl font-medium shadow-lg hover:shadow-xl transition-all duration-200 inline-flex items-center space-x-2 text-sm sm:text-base"
      >
        <svg class="w-4 h-4 sm:w-5 sm:h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
        </svg>
        <span>Add First User</span>
      </button>
    </div>

    <!-- Create/Edit Modal -->
    <div v-if="showModal" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
      <div class="relative top-4 sm:top-10 mx-auto p-3 sm:p-5 border w-full max-w-lg shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-base sm:text-lg font-medium text-gray-900 mb-4">
            {{ isEditing ? 'Edit User' : 'Create New User' }}
          </h3>
          
          <form @submit.prevent="saveUser" class="space-y-4">
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">First Name</label>
                <input
                  v-model="userForm.firstName"
                  type="text"
                  required
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                  placeholder="Enter first name"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Last Name</label>
                <input
                  v-model="userForm.lastName"
                  type="text"
                  required
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                  placeholder="Enter last name"
                />
              </div>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Username</label>
              <input
                v-model="userForm.userName"
                type="text"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                placeholder="Enter username"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Email</label>
              <input
                v-model="userForm.email"
                type="email"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                placeholder="Enter email address"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Phone Number</label>
              <input
                v-model="userForm.phoneNumber"
                type="tel"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                placeholder="Enter phone number"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Role</label>
              <select
                v-model="userForm.role"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
              >
                <option value="">Select role</option>
                <option value="Admin">Admin</option>
                <option value="Manager">Manager</option>
                <option value="Cashier">Cashier</option>
              </select>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Store</label>
              <select
                v-model="userForm.storeId"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
              >
                <option value="">No store assigned</option>
                <option v-for="store in stores" :key="store.id" :value="store.id">
                  {{ store.storeName }}
                </option>
              </select>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">PIN (4 digits)</label>
              <input
                v-model="userForm.pin"
                type="text"
                maxlength="4"
                pattern="[0-9]{4}"
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                placeholder="Enter 4-digit PIN"
              />
            </div>
            
            <div v-if="!isEditing">
              <label class="block text-sm font-medium text-gray-700 mb-1">Password</label>
              <input
                v-model="userForm.password"
                type="password"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                placeholder="Enter password"
              />
            </div>
            
            <div v-if="isEditing">
              <label class="flex items-center">
                <input
                  v-model="userForm.isActive"
                  type="checkbox"
                  class="rounded border-gray-300 text-blue-600 shadow-sm focus:border-blue-300 focus:ring focus:ring-blue-200 focus:ring-opacity-50"
                />
                <span class="ml-2 text-sm text-gray-700">Active</span>
              </label>
            </div>
            
            <div class="flex flex-col sm:flex-row justify-end space-y-3 sm:space-y-0 sm:space-x-3 pt-4">
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

    <!-- Reset Password Modal -->
    <div v-if="showResetPasswordModal" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 sm:top-20 mx-auto p-3 sm:p-5 border w-full max-w-sm sm:w-96 shadow-lg rounded-md bg-white">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Reset Password</h3>
          <p class="text-sm text-gray-600 mb-4">
            Reset password for {{ userToResetPassword?.firstName }} {{ userToResetPassword?.lastName }}
          </p>
          
          <form @submit.prevent="resetPassword" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">New Password</label>
              <input
                v-model="newPassword"
                type="password"
                required
                class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                placeholder="Enter new password"
              />
            </div>
            
            <div class="flex flex-col sm:flex-row justify-end space-y-3 sm:space-y-0 sm:space-x-3 pt-4">
              <button
                type="button"
                @click="closeResetPasswordModal"
                class="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-200 hover:bg-gray-300 rounded-md"
              >
                Cancel
              </button>
              <button
                type="submit"
                :disabled="resettingPassword"
                class="px-4 py-2 text-sm font-medium text-white bg-purple-600 hover:bg-purple-700 rounded-md disabled:opacity-50"
              >
                {{ resettingPassword ? 'Resetting...' : 'Reset Password' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div v-if="showDeleteModal" class="fixed inset-0 modal-backdrop overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 sm:top-20 mx-auto p-3 sm:p-5 border w-full max-w-sm sm:w-96 shadow-lg rounded-md bg-white">
        <div class="mt-3 text-center">
          <div class="mx-auto flex items-center justify-center h-12 w-12 rounded-full bg-red-100">
            <svg class="h-6 w-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z"></path>
            </svg>
          </div>
          <h3 class="text-lg font-medium text-gray-900 mt-2">Delete User</h3>
          <p class="text-sm text-gray-500 mt-2">
            Are you sure you want to delete "{{ userToDelete?.firstName }} {{ userToDelete?.lastName }}"? This action cannot be undone.
          </p>
          <div class="flex justify-center space-x-3 mt-4">
            <button
              @click="closeDeleteModal"
              class="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-200 hover:bg-gray-300 rounded-md"
            >
              Cancel
            </button>
            <button
              @click="deleteUser"
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

interface User {
  id: number
  userName: string
  email: string
  firstName: string
  lastName: string
  role: string
  phoneNumber?: string
  isActive: boolean
  storeId?: number
  storeName?: string
  createdAt: string
  updatedAt?: string
  lastLoginAt?: string
}

interface UserForm {
  userName: string
  email: string
  firstName: string
  lastName: string
  role: string
  phoneNumber?: string
  pin?: string
  storeId?: number
  password?: string
  isActive: boolean
}

interface Store {
  id: number
  storeName: string
  storeCode: string
  isActive: boolean
}

const { user, logout } = useAuth()
const config = useRuntimeConfig()

// Reactive data
const users = ref<User[]>([])
const stores = ref<Store[]>([])
const loading = ref(false)
const saving = ref(false)
const deleting = ref(false)
const resettingPassword = ref(false)
const searchQuery = ref('')
const roleFilter = ref('')
const statusFilter = ref('')
const showModal = ref(false)
const showDeleteModal = ref(false)
const showResetPasswordModal = ref(false)
const isEditing = ref(false)
const editingUserId = ref<number | null>(null)
const userToDelete = ref<User | null>(null)
const userToResetPassword = ref<User | null>(null)
const newPassword = ref('')
const activeUserActions = ref<number | null>(null)

const userForm = ref<UserForm>({
  userName: '',
  email: '',
  firstName: '',
  lastName: '',
  role: '',
  phoneNumber: '',
  pin: '',
  storeId: undefined,
  password: '',
  isActive: true
})

// Computed
const filteredUsers = computed(() => {
  let filtered = users.value

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(userItem =>
      userItem.firstName.toLowerCase().includes(query) ||
      userItem.lastName.toLowerCase().includes(query) ||
      userItem.userName.toLowerCase().includes(query) ||
      userItem.email.toLowerCase().includes(query)
    )
  }

  if (roleFilter.value) {
    filtered = filtered.filter(userItem => userItem.role === roleFilter.value)
  }

  if (statusFilter.value) {
    filtered = filtered.filter(userItem => {
      if (statusFilter.value === 'active') return userItem.isActive
      if (statusFilter.value === 'inactive') return !userItem.isActive
      return true
    })
  }

  return filtered
})

// Methods
const handleLogout = async () => {
  await logout()
}

const toggleUserActions = (userId: number) => {
  activeUserActions.value = activeUserActions.value === userId ? null : userId
}

const loadUsers = async () => {
  loading.value = true
  try {
    const token = useCookie('auth-token')
    const response = await $fetch<User[]>(`${config.public.apiBase}/api/UserManagement`, {
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    users.value = response
  } catch (error) {
    console.error('Failed to load users:', error)
  } finally {
    loading.value = false
  }
}

const loadStores = async () => {
  try {
    const token = useCookie('auth-token')
    const response = await $fetch<Store[]>(`${config.public.apiBase}/api/Store`, {
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    stores.value = response.filter(store => store.isActive)
  } catch (error) {
    console.error('Failed to load stores:', error)
  }
}

const openCreateModal = () => {
  isEditing.value = false
  editingUserId.value = null
  userForm.value = {
    userName: '',
    email: '',
    firstName: '',
    lastName: '',
    role: '',
    phoneNumber: '',
    pin: '',
    storeId: undefined,
    password: '',
    isActive: true
  }
  showModal.value = true
}

const openEditModal = (userItem: User) => {
  isEditing.value = true
  editingUserId.value = userItem.id
  userForm.value = {
    userName: userItem.userName,
    email: userItem.email,
    firstName: userItem.firstName,
    lastName: userItem.lastName,
    role: userItem.role,
    phoneNumber: userItem.phoneNumber,
    pin: '',
    storeId: userItem.storeId,
    isActive: userItem.isActive
  }
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
  isEditing.value = false
  editingUserId.value = null
}

const saveUser = async () => {
  saving.value = true
  try {
    const token = useCookie('auth-token')
    
    if (isEditing.value && editingUserId.value) {
      // Update existing user
      await $fetch(`${config.public.apiBase}/api/UserManagement/${editingUserId.value}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token.value || ''}`,
          'Content-Type': 'application/json'
        },
        body: {
          userName: userForm.value.userName,
          email: userForm.value.email,
          firstName: userForm.value.firstName,
          lastName: userForm.value.lastName,
          role: userForm.value.role,
          phoneNumber: userForm.value.phoneNumber,
          pin: userForm.value.pin,
          storeId: userForm.value.storeId,
          isActive: userForm.value.isActive
        }
      })
    } else {
      // Create new user
      await $fetch(`${config.public.apiBase}/api/UserManagement`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token.value || ''}`,
          'Content-Type': 'application/json'
        },
        body: {
          userName: userForm.value.userName,
          email: userForm.value.email,
          firstName: userForm.value.firstName,
          lastName: userForm.value.lastName,
          role: userForm.value.role,
          phoneNumber: userForm.value.phoneNumber,
          pin: userForm.value.pin,
          storeId: userForm.value.storeId,
          password: userForm.value.password
        }
      })
    }
    
    closeModal()
    await loadUsers()
  } catch (error) {
    console.error('Failed to save user:', error)
  } finally {
    saving.value = false
  }
}

const activateUser = async (userItem: User) => {
  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/UserManagement/${userItem.id}/activate`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    activeUserActions.value = null // Close dropdown
    await loadUsers()
  } catch (error) {
    console.error('Failed to activate user:', error)
  }
}

const deactivateUser = async (userItem: User) => {
  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/UserManagement/${userItem.id}/deactivate`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    activeUserActions.value = null // Close dropdown
    await loadUsers()
  } catch (error) {
    console.error('Failed to deactivate user:', error)
  }
}

const openResetPasswordModal = (userItem: User) => {
  userToResetPassword.value = userItem
  newPassword.value = ''
  activeUserActions.value = null // Close dropdown
  showResetPasswordModal.value = true
}

const closeResetPasswordModal = () => {
  showResetPasswordModal.value = false
  userToResetPassword.value = null
  newPassword.value = ''
}

const resetPassword = async () => {
  if (!userToResetPassword.value) return
  
  resettingPassword.value = true
  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/UserManagement/${userToResetPassword.value.id}/reset-password`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`,
        'Content-Type': 'application/json'
      },
      body: {
        newPassword: newPassword.value
      }
    })
    
    closeResetPasswordModal()
  } catch (error) {
    console.error('Failed to reset password:', error)
  } finally {
    resettingPassword.value = false
  }
}

const confirmDelete = (userItem: User) => {
  userToDelete.value = userItem
  activeUserActions.value = null // Close dropdown
  showDeleteModal.value = true
}

const closeDeleteModal = () => {
  showDeleteModal.value = false
  userToDelete.value = null
}

const deleteUser = async () => {
  if (!userToDelete.value) return
  
  deleting.value = true
  try {
    const token = useCookie('auth-token')
    await $fetch(`${config.public.apiBase}/api/UserManagement/${userToDelete.value.id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token.value || ''}`
      }
    })
    
    closeDeleteModal()
    await loadUsers()
  } catch (error) {
    console.error('Failed to delete user:', error)
  } finally {
    deleting.value = false
  }
}

// Load data on mount
onMounted(async () => {
  await Promise.all([loadUsers(), loadStores()])
})
</script>