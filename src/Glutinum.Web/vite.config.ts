import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig((env) => {
    const isDevelpoment = env.mode === 'development';

    return {
        css: {
            devSourcemap: isDevelpoment
        },
        plugins: [react()],
        server: {
            watch: {
                ignored: [
                    "**/*.fs"
                ]
            }
        },
        clearScreen: false
    }
})
