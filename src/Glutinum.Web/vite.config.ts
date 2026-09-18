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
        build: {
            // The documentation site links `app.js` and `app.css` from its own page
            rollupOptions: {
                output: {
                    entryFileNames: "app.js",
                    chunkFileNames: "[name].js",
                    assetFileNames: (asset) =>
                        (asset.name ?? "").endsWith(".css") ? "app.css" : "[name][extname]"
                }
            }
        },
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
