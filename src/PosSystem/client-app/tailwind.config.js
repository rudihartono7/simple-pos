/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./components/**/*.{js,vue,ts}",
    "./layouts/**/*.vue",
    "./pages/**/*.vue",
    "./plugins/**/*.{js,ts}",
    "./app.vue",
    "./error.vue"
  ],
  theme: {
    extend: {
      fontFamily: {
        'sans': ['Roboto', 'ui-sans-serif', 'system-ui', '-apple-system', 'BlinkMacSystemFont', 'Segoe UI', 'Arial', 'sans-serif'],
      },
      colors: {
        'primary': {
          DEFAULT: '#FFCA40',
          50: '#FFF9E6',
          100: '#FFF2CC',
          200: '#FFE599',
          300: '#FFD966',
          400: '#FFCA40',
          500: '#FFB800',
          600: '#CC9300',
          700: '#996F00',
          800: '#664A00',
          900: '#332500',
        },
        'danger': {
          DEFAULT: '#C8161D',
          50: '#F8E6E7',
          100: '#F1CCCE',
          200: '#E3999D',
          300: '#D5666C',
          400: '#C8161D',
          500: '#A01117',
          600: '#780D11',
          700: '#50080B',
          800: '#280406',
          900: '#140203',
        },
        'neutral': {
          'black': '#000000',
          'gray': '#808080',
          'light': '#F1EFE9',
          'medium': '#D9D9D9',
          'white': '#FFFFFF',
        }
      }
    },
  },
  plugins: [
    require('@tailwindcss/forms'),
    require('@tailwindcss/typography'),
  ],
}