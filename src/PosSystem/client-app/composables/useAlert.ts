import { ref, reactive } from 'vue'

export interface AlertOptions {
  title?: string
  message: string
  type?: 'success' | 'error' | 'warning' | 'info' | 'confirm'
  confirmText?: string
  cancelText?: string
  showCancel?: boolean
  allowBackdropClose?: boolean
}

interface AlertState {
  isVisible: boolean
  title: string
  message: string
  type: 'success' | 'error' | 'warning' | 'info' | 'confirm'
  confirmText: string
  cancelText: string
  showCancel: boolean
  allowBackdropClose: boolean
  onConfirm?: () => void
  onCancel?: () => void
}

const alertState = reactive<AlertState>({
  isVisible: false,
  title: '',
  message: '',
  type: 'info',
  confirmText: 'OK',
  cancelText: 'Cancel',
  showCancel: false,
  allowBackdropClose: true,
  onConfirm: undefined,
  onCancel: undefined
})

export const useAlert = () => {
  const showAlert = (options: AlertOptions): Promise<boolean> => {
    return new Promise((resolve) => {
      alertState.isVisible = true
      alertState.title = options.title || ''
      alertState.message = options.message
      alertState.type = options.type || 'info'
      alertState.confirmText = options.confirmText || 'OK'
      alertState.cancelText = options.cancelText || 'Cancel'
      alertState.showCancel = options.showCancel || false
      alertState.allowBackdropClose = options.allowBackdropClose !== false
      
      alertState.onConfirm = () => {
        alertState.isVisible = false
        resolve(true)
      }
      
      alertState.onCancel = () => {
        alertState.isVisible = false
        resolve(false)
      }
    })
  }

  const hideAlert = () => {
    alertState.isVisible = false
  }

  // Convenience methods for different alert types
  const showSuccess = (message: string, title?: string): Promise<boolean> => {
    return showAlert({
      message,
      title,
      type: 'success'
    })
  }

  const showError = (message: string, title?: string): Promise<boolean> => {
    return showAlert({
      message,
      title,
      type: 'error'
    })
  }

  const showWarning = (message: string, title?: string): Promise<boolean> => {
    return showAlert({
      message,
      title,
      type: 'warning'
    })
  }

  const showInfo = (message: string, title?: string): Promise<boolean> => {
    return showAlert({
      message,
      title,
      type: 'info'
    })
  }

  const showConfirm = (
    message: string,
    title?: string,
    confirmText = 'Yes',
    cancelText = 'No'
  ): Promise<boolean> => {
    return showAlert({
      message,
      title,
      type: 'confirm',
      confirmText,
      cancelText,
      showCancel: true,
      allowBackdropClose: false
    })
  }

  // Replace native alert function
  const alert = (message: string): Promise<boolean> => {
    return showInfo(message)
  }

  // Replace native confirm function
  const confirm = (message: string): Promise<boolean> => {
    return showConfirm(message, 'Confirm')
  }

  return {
    alertState,
    showAlert,
    hideAlert,
    showSuccess,
    showError,
    showWarning,
    showInfo,
    showConfirm,
    alert,
    confirm
  }
}

// Global instance for use across the app
export const globalAlert = useAlert()