/// <reference types="vitest/config" />
import { defineConfig } from 'vite'
import { configDefaults } from 'vitest/config'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
  ],
  // Playwright specs in e2e/ are run by `npm run e2e`, not Vitest.
  test: {
    exclude: [...configDefaults.exclude, 'e2e/**'],
  },
})
