import { defineConfig } from 'vitest/config'

export default defineConfig({
    test: {
        forceRerunTriggers: [
            "**/specs/**/*.d.ts",
            "**/specs/**/*.fsx",
        ],
        reporters: [
            "dot",
        ],
    },
})
