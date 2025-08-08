// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: false },

  modules: [
    '@nuxt/content',
    '@nuxt/eslint',
    '@nuxt/image',
    '@nuxt/scripts',
    '@nuxt/test-utils'
  ],

  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || 'http://localhost:5001'
    }
  },

  css: ['./assets/css/main.css'],

  postcss: {
    plugins: {
      '@tailwindcss/postcss': {},
      autoprefixer: {},
    },
  },

  // Disable HTTPS in development
  devServer: {
    https: false
  },

  // Production build configuration
  ssr: false, // Ensure SPA mode for production
  
  // Disable automatic HTTPS redirects
  nitro: {
    devProxy: {
      '/api': {
        target: process.env.NUXT_PUBLIC_API_BASE || 'http://localhost:5001',
        secure: false,
        changeOrigin: true
      }
    },
    // Exclude native modules that cause build issues in Docker
    experimental: {
      wasm: false
    },
    rollupConfig: {
      external: ['better-sqlite3']
    }
  }
})
