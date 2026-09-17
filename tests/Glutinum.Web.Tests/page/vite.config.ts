import { defineConfig } from 'vite'

// The page is opened as a file by the test, so the assets are relative
export default defineConfig({
    base: './',
    build: {
        outDir: 'dist',
        emptyOutDir: true,
    },
    clearScreen: false,
})
