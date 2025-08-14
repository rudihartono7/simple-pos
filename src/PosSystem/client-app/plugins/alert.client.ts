import { globalAlert } from '~/composables/useAlert'

export default defineNuxtPlugin(() => {
  // Make alert functions available globally
  return {
    provide: {
      alert: globalAlert.alert,
      confirm: globalAlert.confirm,
      showSuccess: globalAlert.showSuccess,
      showError: globalAlert.showError,
      showWarning: globalAlert.showWarning,
      showInfo: globalAlert.showInfo,
      showConfirm: globalAlert.showConfirm,
      showAlert: globalAlert.showAlert
    }
  }
})

// Override native alert and confirm functions
if (process.client) {
  window.alert = (message: string) => {
    globalAlert.alert(message)
  }
  
  window.confirm = (message?: string): boolean => {
    // Note: This is a synchronous override, but our custom confirm is async
    // For better UX, we should replace confirm() calls with await $confirm() in components
    let result = false
    globalAlert.confirm(message || 'Are you sure?').then(confirmed => {
      result = confirmed
    })
    return result
  }
}