import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { resolve } from 'path'

export default defineConfig({
  plugins: [vue()],
  server: {
    port: 8013
  },
  resolve: {
    extensions: ['.js', '.vue', '.json', '.scss'],
    alias: {
      '@': resolve(__dirname, 'src')
    }
  },
})
