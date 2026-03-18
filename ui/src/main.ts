import "@/styles/index.css"
import "@/styles/main.css"
import { ViteSSG } from "vite-ssg"

// Auto generated routes by https://github.com/hannoeru/vite-plugin-pages 
import { setupLayouts } from "virtual:generated-layouts"
import generatedRoutes from "~pages"

import App from "@/App.vue"

const routes = setupLayouts(generatedRoutes)

// https://github.com/antfu/vite-ssg
export const createApp = ViteSSG(
    App,
    { routes },
    (ctx) => {
        // install all modules under `modules/` (except datadog-rum which lazy-loads separately)
        Object.values(import.meta.globEager('./modules/!(datadog-rum).ts')).map(i => (i as any).install?.(ctx))

        // lazy-load DD RUM so it doesn't block page render
        if (!import.meta.env.SSR) {
            import('./modules/datadog-rum').then(m => m.install?.(ctx))
        }
    },
)
