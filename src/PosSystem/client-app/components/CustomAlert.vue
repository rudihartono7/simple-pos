<template>
  <Teleport to="body">
    <Transition>
      <div
        v-if="isVisible"
        class="fixed inset-0 z-[9999] flex items-center justify-center p-4"
        @click.self="handleBackdropClick"
      >
        <!-- Backdrop -->
        <div class="absolute inset-0 modal-backdrop"></div>
        
        <!-- Alert Modal -->
        <div class="relative bg-white rounded-lg shadow-xl max-w-md w-full mx-4 overflow-hidden">
          <!-- Header with Icon -->
          <div class="flex items-center p-4 sm:p-6">
            <div
              :class="[
                'flex-shrink-0 w-10 h-10 sm:w-12 sm:h-12 rounded-full flex items-center justify-center mr-3 sm:mr-4',
                iconBgClass
              ]"
            >
              <svg :class="['w-5 h-5 sm:w-6 sm:h-6', iconColorClass]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="getIconPath" />
              </svg>
            </div>
            <div class="flex-1 min-w-0">
              <h3 class="text-base sm:text-lg font-semibold text-black mb-1">
                {{ computedTitle }}
              </h3>
              <p class="text-sm sm:text-base text-neutral-gray break-words">
                {{ message }}
              </p>
            </div>
          </div>
          
          <!-- Actions -->
          <div class="bg-neutral-light px-4 py-3 sm:px-6 sm:py-4 flex flex-col sm:flex-row gap-2 sm:gap-3 sm:justify-end">
            <button
              v-if="showCancel"
              @click="handleCancel"
              class="w-full sm:w-auto px-4 py-2 text-sm font-medium text-black bg-white border border-neutral-medium rounded-md hover:bg-neutral-light focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary transition-colors"
            >
              {{ cancelText }}
            </button>
            <button
              @click="handleConfirm"
              :class="[
                'w-full sm:w-auto px-4 py-2 text-sm font-medium rounded-md focus:outline-none focus:ring-2 focus:ring-offset-2 transition-colors',
                confirmButtonClass
              ]"
            >
              {{ confirmText }}
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { computed, nextTick, onMounted, onUnmounted } from 'vue'

export interface AlertOptions {
  title?: string
  message: string
  type?: 'success' | 'error' | 'warning' | 'info' | 'confirm'
  confirmText?: string
  cancelText?: string
  showCancel?: boolean
  allowBackdropClose?: boolean
}

interface Props {
  isVisible: boolean
  title?: string
  message: string
  type?: 'success' | 'error' | 'warning' | 'info' | 'confirm'
  confirmText?: string
  cancelText?: string
  showCancel?: boolean
  allowBackdropClose?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  title: '',
  type: 'info',
  confirmText: 'OK',
  cancelText: 'Cancel',
  showCancel: false,
  allowBackdropClose: true
})

const emit = defineEmits<{
  confirm: []
  cancel: []
  close: []
}>()

// Computed properties for styling based on type
const iconBgClass = computed(() => {
  switch (props.type) {
    case 'success':
      return 'bg-primary'
    case 'error':
      return 'bg-danger'
    case 'warning':
      return 'bg-primary'
    case 'confirm':
      return 'bg-primary'
    default:
      return 'bg-primary'
  }
})

const iconColorClass = computed(() => {
  switch (props.type) {
    case 'success':
      return 'text-black'
    case 'error':
      return 'text-white'
    case 'warning':
      return 'text-black'
    case 'confirm':
      return 'text-black'
    default:
      return 'text-black'
  }
})

const confirmButtonClass = computed(() => {
  switch (props.type) {
    case 'success':
      return 'text-black bg-primary hover:bg-primary focus:ring-primary'
    case 'error':
      return 'text-white bg-danger hover:bg-danger focus:ring-danger'
    case 'warning':
      return 'text-black bg-primary hover:bg-primary focus:ring-primary'
    case 'confirm':
      return 'text-black bg-primary hover:bg-primary focus:ring-primary'
    default:
      return 'text-black bg-primary hover:bg-primary focus:ring-primary'
  }
})

const getIconPath = computed(() => {
  switch (props.type) {
    case 'success':
      return 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z'
    case 'error':
      return 'M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z'
    case 'warning':
      return 'M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.082 16.5c-.77.833.192 2.5 1.732 2.5z'
    case 'confirm':
      return 'M8.228 9c.549-1.165 2.03-2 3.772-2 2.21 0 4 1.343 4 3 0 1.4-1.278 2.575-3.006 2.907-.542.104-.994.54-.994 1.093m0 3h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z'
    default:
      return 'M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z'
  }
})

const computedTitle = computed(() => {
  if (props.title) return props.title
  
  switch (props.type) {
    case 'success':
      return 'Success'
    case 'error':
      return 'Error'
    case 'warning':
      return 'Warning'
    case 'confirm':
      return 'Confirm'
    default:
      return 'Information'
  }
})

const handleConfirm = () => {
  emit('confirm')
  emit('close')
}

const handleCancel = () => {
  emit('cancel')
  emit('close')
}

const handleBackdropClick = () => {
  if (props.allowBackdropClose) {
    emit('close')
  }
}

const handleEscapeKey = (event: KeyboardEvent) => {
  if (event.key === 'Escape' && props.isVisible) {
    if (props.showCancel) {
      handleCancel()
    } else {
      emit('close')
    }
  }
}

onMounted(() => {
  document.addEventListener('keydown', handleEscapeKey)
})

onUnmounted(() => {
  document.removeEventListener('keydown', handleEscapeKey)
})
</script>

<style scoped>
/* Icons as inline SVG */
svg {
  fill: none;
  stroke: currentColor;
  stroke-width: 2;
  stroke-linecap: round;
  stroke-linejoin: round;
}
</style>