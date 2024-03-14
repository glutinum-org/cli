/// <reference types="vitest" />
import { defineConfig } from 'vite'

export default defineConfig({
    // Vitest don't support yet clearScreen option via CLI
    // but it does respect it when it's set in the vite config
    // https://github.com/vitest-dev/vitest/issues/5185
    clearScreen: false,
})
