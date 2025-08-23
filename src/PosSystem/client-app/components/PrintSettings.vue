<template>
  <div class="print-settings">
    <!-- Print Settings Modal -->
    <div v-if="showSettings" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-lg shadow-xl max-w-lg w-full mx-4 max-h-[90vh] overflow-hidden">
        <div class="flex justify-between items-center p-6 border-b">
          <h3 class="text-xl font-semibold text-gray-900">Print Settings</h3>
          <button @click="closeSettings" class="text-gray-400 hover:text-gray-600">
            <Icon name="heroicons:x-mark" class="h-6 w-6" />
          </button>
        </div>
        
        <div class="p-6 space-y-6 overflow-y-auto" style="max-height: 60vh;">
          <!-- Store Information -->
          <div class="space-y-4">
            <h4 class="text-lg font-medium text-gray-900">Store Information</h4>
            <div class="grid grid-cols-1 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Store Name</label>
                <input
                  v-model="localSettings.storeInfo.name"
                  type="text"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Your Store Name"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Address</label>
                <textarea
                  v-model="localSettings.storeInfo.address"
                  rows="2"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Store Address"
                ></textarea>
              </div>
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">Phone</label>
                  <input
                    v-model="localSettings.storeInfo.phone"
                    type="text"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    placeholder="Phone Number"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">Email</label>
                  <input
                    v-model="localSettings.storeInfo.email"
                    type="email"
                    class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    placeholder="Email Address"
                  />
                </div>
              </div>
            </div>
          </div>

          <!-- Default Print Settings -->
          <div class="space-y-4">
            <h4 class="text-lg font-medium text-gray-900">Default Print Settings</h4>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Default Printer Type</label>
                <select
                  v-model="localSettings.defaultPrinterType"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="thermal">Thermal Printer</option>
                  <option value="standard">Standard Printer</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Default Paper Size</label>
                <select
                  v-model="localSettings.defaultPaperSize"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="80mm">80mm (Thermal)</option>
                  <option value="58mm">58mm (Thermal)</option>
                  <option value="a4">A4 (Standard)</option>
                  <option value="letter">Letter (Standard)</option>
                </select>
              </div>
            </div>
          </div>

          <!-- Receipt Layout Settings -->
          <div class="space-y-4">
            <h4 class="text-lg font-medium text-gray-900">Receipt Layout</h4>
            <div class="space-y-3">
              <div class="flex items-center justify-between">
                <label class="text-sm font-medium text-gray-700">Show Store Logo</label>
                <input
                  v-model="localSettings.layout.showLogo"
                  type="checkbox"
                  class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                />
              </div>
              <div class="flex items-center justify-between">
                <label class="text-sm font-medium text-gray-700">Show Product Codes</label>
                <input
                  v-model="localSettings.layout.showProductCodes"
                  type="checkbox"
                  class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                />
              </div>
              <div class="flex items-center justify-between">
                <label class="text-sm font-medium text-gray-700">Show Tax Details</label>
                <input
                  v-model="localSettings.layout.showTaxDetails"
                  type="checkbox"
                  class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                />
              </div>
              <div class="flex items-center justify-between">
                <label class="text-sm font-medium text-gray-700">Show Cashier Name</label>
                <input
                  v-model="localSettings.layout.showCashier"
                  type="checkbox"
                  class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                />
              </div>
            </div>
          </div>

          <!-- Custom Messages -->
          <div class="space-y-4">
            <h4 class="text-lg font-medium text-gray-900">Custom Messages</h4>
            <div class="space-y-3">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Header Message</label>
                <input
                  v-model="localSettings.messages.header"
                  type="text"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Welcome message (optional)"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Footer Message</label>
                <input
                  v-model="localSettings.messages.footer"
                  type="text"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Thank you message"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Promotional Message</label>
                <input
                  v-model="localSettings.messages.promotion"
                  type="text"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Promotional text (optional)"
                />
              </div>
            </div>
          </div>

          <!-- Print Behavior -->
          <div class="space-y-4">
            <h4 class="text-lg font-medium text-gray-900">Print Behavior</h4>
            <div class="space-y-3">
              <div class="flex items-center justify-between">
                <label class="text-sm font-medium text-gray-700">Auto Print After Transaction</label>
                <input
                  v-model="localSettings.behavior.autoPrint"
                  type="checkbox"
                  class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                />
              </div>
              <div class="flex items-center justify-between">
                <label class="text-sm font-medium text-gray-700">Always Show Preview</label>
                <input
                  v-model="localSettings.behavior.alwaysPreview"
                  type="checkbox"
                  class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Number of Copies</label>
                <select
                  v-model="localSettings.behavior.copies"
                  class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="1">1 Copy</option>
                  <option value="2">2 Copies</option>
                  <option value="3">3 Copies</option>
                </select>
              </div>
            </div>
          </div>
        </div>

        <!-- Action Buttons -->
        <div class="flex justify-between p-6 border-t bg-gray-50">
          <button 
            @click="resetToDefaults" 
            class="px-4 py-2 text-gray-600 bg-white border border-gray-300 rounded-md hover:bg-gray-50"
          >
            Reset to Defaults
          </button>
          <div class="flex space-x-3">
            <button 
              @click="closeSettings" 
              class="px-4 py-2 text-gray-600 bg-white border border-gray-300 rounded-md hover:bg-gray-50"
            >
              Cancel
            </button>
            <button 
              @click="saveSettings" 
              class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 flex items-center"
            >
              <Icon name="heroicons:check" class="h-4 w-4 mr-2" />
              Save Settings
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface StoreInfo {
  name: string
  address: string
  phone: string
  email: string
}

interface PrintSettings {
  storeInfo: StoreInfo
  defaultPrinterType: 'standard' | 'thermal'
  defaultPaperSize: '80mm' | '58mm' | 'a4' | 'letter'
  layout: {
    showLogo: boolean
    showProductCodes: boolean
    showTaxDetails: boolean
    showCashier: boolean
  }
  messages: {
    header: string
    footer: string
    promotion: string
  }
  behavior: {
    autoPrint: boolean
    alwaysPreview: boolean
    copies: number
  }
}

interface Props {
  settings?: PrintSettings
}

const props = defineProps<Props>()

const emit = defineEmits<{
  close: []
  save: [settings: PrintSettings]
}>()

// Reactive data
const showSettings = ref(false)
const localSettings = ref<PrintSettings>({
  storeInfo: {
    name: 'Your Store Name',
    address: '123 Main Street, City, State 12345',
    phone: '+1 (555) 123-4567',
    email: 'info@yourstore.com'
  },
  defaultPrinterType: 'thermal',
  defaultPaperSize: '80mm',
  layout: {
    showLogo: false,
    showProductCodes: true,
    showTaxDetails: true,
    showCashier: true
  },
  messages: {
    header: '',
    footer: 'Thank you for your purchase!',
    promotion: ''
  },
  behavior: {
    autoPrint: false,
    alwaysPreview: true,
    copies: 1
  }
})

const defaultSettings: PrintSettings = {
  storeInfo: {
    name: 'Your Store Name',
    address: '123 Main Street, City, State 12345',
    phone: '+1 (555) 123-4567',
    email: 'info@yourstore.com'
  },
  defaultPrinterType: 'thermal',
  defaultPaperSize: '80mm',
  layout: {
    showLogo: false,
    showProductCodes: true,
    showTaxDetails: true,
    showCashier: true
  },
  messages: {
    header: '',
    footer: 'Thank you for your purchase!',
    promotion: ''
  },
  behavior: {
    autoPrint: false,
    alwaysPreview: true,
    copies: 1
  }
}

// Methods
const openSettings = () => {
  if (props.settings) {
    localSettings.value = { ...props.settings }
  }
  showSettings.value = true
}

const closeSettings = () => {
  showSettings.value = false
  emit('close')
}

const saveSettings = () => {
  // Save to localStorage
  localStorage.setItem('printSettings', JSON.stringify(localSettings.value))
  
  emit('save', localSettings.value)
  closeSettings()
}

const resetToDefaults = () => {
  if (confirm('Are you sure you want to reset all settings to defaults?')) {
    localSettings.value = { ...defaultSettings }
  }
}

const loadSettings = (): PrintSettings => {
  try {
    const saved = localStorage.getItem('printSettings')
    if (saved) {
      return { ...defaultSettings, ...JSON.parse(saved) }
    }
  } catch (error) {
    console.error('Failed to load print settings:', error)
  }
  return { ...defaultSettings }
}

// Initialize settings from localStorage
onMounted(() => {
  localSettings.value = loadSettings()
})

// Expose methods
defineExpose({
  openSettings,
  closeSettings,
  loadSettings
})
</script>

<style scoped>
/* Custom styles for the print settings component */
.print-settings {
  font-family: system-ui, -apple-system, sans-serif;
}

/* Smooth transitions for modal */
.print-settings .fixed {
  transition: opacity 0.2s ease-in-out;
}

/* Custom checkbox styling */
input[type="checkbox"]:checked {
  background-color: #3b82f6;
  border-color: #3b82f6;
}

/* Focus states */
input:focus,
select:focus,
textarea:focus {
  outline: none;
  ring: 2px;
  ring-color: #3b82f6;
  border-color: #3b82f6;
}
</style>