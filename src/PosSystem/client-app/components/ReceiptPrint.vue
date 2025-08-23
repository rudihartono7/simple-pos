<template>
  <div class="receipt-print-container">
    <!-- Print Preview Modal -->
    <div v-if="showPreview" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-lg shadow-xl max-w-md w-full mx-4 max-h-[90vh] overflow-hidden">
        <div class="flex justify-between items-center p-4 border-b">
          <h3 class="text-lg font-semibold">Print Preview</h3>
          <button @click="closePreview" class="text-gray-400 hover:text-gray-600">
            <Icon name="heroicons:x-mark" class="h-6 w-6" />
          </button>
        </div>
        
        <!-- Print Settings -->
        <div class="p-4 border-b bg-gray-50">
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Printer Type</label>
              <select v-model="printerType" class="w-full px-3 py-2 border border-gray-300 rounded-md text-sm">
                <option value="standard">Standard Printer</option>
                <option value="thermal">Thermal Printer</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Paper Size</label>
              <select v-model="paperSize" class="w-full px-3 py-2 border border-gray-300 rounded-md text-sm">
                <option value="80mm">80mm (Thermal)</option>
                <option value="58mm">58mm (Thermal)</option>
                <option value="a4">A4 (Standard)</option>
                <option value="letter">Letter (Standard)</option>
              </select>
            </div>
          </div>
        </div>

        <!-- Receipt Preview -->
        <div class="overflow-y-auto" style="max-height: 60vh;">
          <div 
            ref="receiptRef" 
            :class="receiptClasses"
            class="receipt-content bg-white p-4 font-mono text-black"
          >
            <div v-html="formattedReceipt"></div>
          </div>
        </div>

        <!-- Action Buttons -->
        <div class="flex justify-end space-x-3 p-4 border-t bg-gray-50">
          <button 
            @click="closePreview" 
            class="px-4 py-2 text-gray-600 bg-white border border-gray-300 rounded-md hover:bg-gray-50"
          >
            Cancel
          </button>
          <button 
            @click="printReceipt" 
            class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 flex items-center"
          >
            <Icon name="heroicons:printer" class="h-4 w-4 mr-2" />
            Print
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import JsBarcode from 'jsbarcode'

interface Transaction {
  id: number
  transactionNumber: string
  storeId: number
  customerId?: number | null
  userId: number
  totalAmount: number
  discountAmount: number
  taxAmount: number
  finalAmount: number
  status: string
  paymentMethod: string | undefined
  amountReceived?: number
  changeAmount?: number
  transactionDate: string
  transactionItems?: TransactionItem[]
  payments?: Payment[],
  storeInfo: StoreInfo,
  user: UserInfo 
}

interface UserInfo {
  firstName: string,
  lastName: string,
  role: string
}

interface TransactionItem {
  productId: number
  unitPrice: number
  quantity: number
  product?: {
    productName: string
    productCode: string
  }
}

interface Payment {
  id: number
  paymentMethod: string
  receivedAmount: number
  changeAmount: number
  amount: number
  status: string
  paymentDate: string
}

interface StoreInfo {
  storeName: string
  storeCode: string
  address: string
  phone: string
  email?: string
}

interface Props {
  transaction: Transaction
  storeInfo?: StoreInfo
  userInfo? : UserInfo
}

const props = defineProps<Props>()

const emit = defineEmits<{
  close: []
  printed: []
}>()

// Reactive data
const showPreview = ref(false)
const printerType = ref<'standard' | 'thermal'>('thermal')
const paperSize = ref<'80mm' | '58mm' | 'a4' | 'letter'>('80mm')
const receiptRef = ref<HTMLElement>()
const barcodeDataUrl = ref<string>('')

// Computed
const receiptClasses = computed(() => {
  const classes = ['receipt-print']
  
  if (printerType.value === 'thermal') {
    classes.push('thermal-receipt')
    if (paperSize.value === '80mm') {
      classes.push('width-80mm')
    } else if (paperSize.value === '58mm') {
      classes.push('width-58mm')
    }
  } else {
    classes.push('standard-receipt')
    if (paperSize.value === 'a4') {
      classes.push('size-a4')
    } else if (paperSize.value === 'letter') {
      classes.push('size-letter')
    }
  }
  
  return classes.join(' ')
})

const formattedReceipt = computed(() => {
  const { transaction, storeInfo } = props
  const isThermal = printerType.value === 'thermal'
  const lineWidth = paperSize.value === '58mm' ? 32 : (paperSize.value === '80mm' ? 48 : 80)
  
  let receipt = ''
  
  // Store Header - Centered
  if (storeInfo) {
    console.log(storeInfo)
    receipt += centerText(storeInfo.storeName.toUpperCase(), lineWidth) + '\n'
    if (storeInfo.address) {
      receipt += centerText(storeInfo.address, lineWidth) + '\n'
    }
    if (storeInfo.phone) {
      receipt += centerText(`Tel: ${storeInfo.phone}`, lineWidth) + '\n'
    }
    if (storeInfo.email) {
      receipt += centerText(storeInfo.email, lineWidth) + '\n'
    }
  }
  
  receipt += '='.repeat(lineWidth) + '\n'
  
  // Transaction Info - Centered
  receipt += centerText(`Receipt No: ${transaction.transactionNumber}`, lineWidth) + '\n'
  receipt += centerText(`Date: ${formatDate(transaction.transactionDate)}`, lineWidth) + '\n'
  receipt += centerText(`Cashier: User ${transaction.userId} - {userInfo.firstName}`, lineWidth) + '\n'
  
  // Barcode
  if (barcodeDataUrl.value) {
    receipt += '\n' + centerText('[BARCODE]', lineWidth) + '\n'
    receipt += `<img src="${barcodeDataUrl.value}" style="display: block; margin: 10px auto; max-width: 200px;" alt="Barcode: ${transaction.transactionNumber}" />\n`
  }
  
  receipt += '-'.repeat(lineWidth) + '\n'
  
  // Items Header - Right aligned
  if (isThermal) {
    receipt += rightAlign('Item                 Qty  Price', lineWidth) + '\n'
  } else {
    receipt += rightAlign('Item' + ' '.repeat(26) + 'Qty' + ' '.repeat(5) + 'Price' + ' '.repeat(7) + 'Total', lineWidth) + '\n'
  }
  receipt += '-'.repeat(lineWidth) + '\n'
  
  // Transaction Items - Right aligned
  if (transaction.transactionItems) {
    transaction.transactionItems.forEach(item => {
      const productName = item.product?.productName || `Product ${item.productId}`
      const total = item.quantity * item.unitPrice
      
      if (isThermal) {
        // Thermal format - compact, right aligned
        const truncatedName = productName.length > 20 ? productName.substring(0, 17) + '...' : productName
        const itemLine = truncatedName + ' '.repeat(Math.max(1, 20 - truncatedName.length)) + item.quantity.toString() + ' '.repeat(Math.max(1, 5 - item.quantity.toString().length)) + formatCurrency(item.unitPrice)
        receipt += rightAlign(itemLine, lineWidth) + '\n'
        if (item.quantity > 1) {
          receipt += rightAlign(formatCurrency(total), lineWidth) + '\n'
        }
      } else {
        // Standard format - full width, right aligned
        const itemLine = productName + ' '.repeat(Math.max(1, 30 - productName.length)) + item.quantity.toString() + ' '.repeat(Math.max(1, 8 - item.quantity.toString().length)) + formatCurrency(item.unitPrice) + ' '.repeat(Math.max(1, 12 - formatCurrency(item.unitPrice).length)) + formatCurrency(total)
        receipt += rightAlign(itemLine, lineWidth) + '\n'
      }
    })
  }
  
  receipt += '-'.repeat(lineWidth) + '\n'
  
  // Totals
  receipt += rightAlign(`Subtotal: ${formatCurrency(transaction.subTotal)}`, lineWidth) + '\n'
  
  if (transaction.discountAmount > 0) {
    receipt += rightAlign(`Discount: -${formatCurrency(transaction.discountAmount)}`, lineWidth) + '\n'
  }
  
  if (transaction.taxAmount > 0) {
    receipt += rightAlign(`Tax: ${formatCurrency(transaction.taxAmount)}`, lineWidth) + '\n'
  }
  
  receipt += '='.repeat(lineWidth) + '\n'
  receipt += rightAlign(`TOTAL: ${formatCurrency(transaction.totalAmount)}`, lineWidth) + '\n'
  receipt += '='.repeat(lineWidth) + '\n'
  
  // Payment Info
  if (transaction.payments && transaction.payments.length > 0) {
    transaction.payments.forEach(payment => {
      receipt += `${payment.paymentMethod}: ${formatCurrency(payment.amount)}\n`
    })
  } else {
    receipt += `${transaction.paymentMethod}: ${formatCurrency(transaction.amountReceived || transaction.finalAmount)}\n`
  }
  
  if (transaction.changeAmount && transaction.changeAmount > 0) {
    receipt += `Change: ${formatCurrency(transaction.changeAmount)}\n`
  }
  
  receipt += '\n'
  // Footer - Centered
  receipt += centerText('Thank you for your purchase!', lineWidth) + '\n'
  receipt += centerText('Please come again', lineWidth) + '\n'
  
  if (isThermal) {
    receipt += '\n\n\n' // Extra spacing for thermal printers
  }
  
  return receipt.replace(/\n/g, '<br>')
})

// Generate barcode
const generateBarcode = () => {
  try {
    const canvas = document.createElement('canvas')
    JsBarcode(canvas, props.transaction.transactionNumber, {
      format: 'CODE128',
      width: 2,
      height: 50,
      displayValue: true,
      fontSize: 12,
      margin: 10
    })
    barcodeDataUrl.value = canvas.toDataURL('image/png')
  } catch (error) {
    console.error('Error generating barcode:', error)
    barcodeDataUrl.value = ''
  }
}

// Watch for transaction changes to regenerate barcode
watch(() => props.transaction.transactionNumber, () => {
  generateBarcode()
}, { immediate: true })

// Methods
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('id-ID').format(amount)
}

const formatDate = (dateString: string): string => {
  return new Date(dateString).toLocaleDateString('id-ID', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const centerText = (text: string, width: number): string => {
  const padding = Math.max(0, Math.floor((width - text.length) / 2))
  return ' '.repeat(padding) + text
}

const leftPad = (text: string, width: number): string => {
  return text.padEnd(width, ' ')
}

const rightPad = (text: string, width: number): string => {
  return text.padStart(width, ' ')
}

const rightAlign = (text: string, width: number): string => {
  return text.padStart(width, ' ')
}

const openPreview = () => {
  showPreview.value = true
}

const closePreview = () => {
  showPreview.value = false
  emit('close')
}

const printReceipt = () => {
  if (receiptRef.value) {
    const printWindow = window.open('', '_blank')
    if (printWindow) {
      const receiptContent = receiptRef.value.innerHTML
      
      printWindow.document.write(`
        <!DOCTYPE html>
        <html>
        <head>
          <title>Receipt - ${props.transaction.transactionNumber}</title>
          <style>
            body {
              margin: 0;
              padding: 20px;
              font-family: 'Courier New', monospace;
              font-size: 12px;
              line-height: 1.2;
            }
            .thermal-receipt {
              width: ${paperSize.value === '58mm' ? '58mm' : '80mm'};
              margin: 0 auto;
            }
            .standard-receipt {
              width: ${paperSize.value === 'a4' ? '210mm' : '216mm'};
              margin: 0 auto;
            }
            .width-58mm { max-width: 58mm; }
            .width-80mm { max-width: 80mm; }
            .size-a4 { max-width: 210mm; }
            .size-letter { max-width: 216mm; }
            @media print {
              body { margin: 0; padding: 0; }
              .thermal-receipt { width: ${paperSize.value === '58mm' ? '58mm' : '80mm'}; }
              .standard-receipt { width: 100%; }
            }
          </style>
        </head>
        <body>
          <div class="${receiptClasses.value}">
            ${receiptContent}
          </div>
        </body>
        </html>
      `)
      
      printWindow.document.close()
      printWindow.focus()
      
      setTimeout(() => {
        printWindow.print()
        printWindow.close()
        emit('printed')
        closePreview()
      }, 250)
    }
  }
}

// Expose methods
defineExpose({
  openPreview,
  closePreview,
  printReceipt
})
</script>

<style scoped>
.receipt-print-container {
  font-family: 'Courier New', monospace;
}

.receipt-content {
  white-space: pre-line;
  font-size: 12px;
  line-height: 1.2;
}

.thermal-receipt {
  font-size: 11px;
}

.standard-receipt {
  font-size: 12px;
}

.width-58mm {
  max-width: 58mm;
}

.width-80mm {
  max-width: 80mm;
}

.size-a4 {
  max-width: 210mm;
}

.size-letter {
  max-width: 216mm;
}

@media print {
  .receipt-print-container {
    margin: 0;
    padding: 0;
  }
  
  .thermal-receipt {
    width: 80mm;
  }
  
  .standard-receipt {
    width: 100%;
  }
}
</style>