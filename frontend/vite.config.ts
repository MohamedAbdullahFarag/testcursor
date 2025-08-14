import inject from '@rollup/plugin-inject'
import basicSsl from '@vitejs/plugin-basic-ssl'
import react from '@vitejs/plugin-react'
import path from 'path'
import { defineConfig } from 'vite'
/// <reference types="vitest" />

export default defineConfig({
    build: {
        rollupOptions: {
            output: {
                entryFileNames: `Assets/[name]${Date.now()}.js`,
                chunkFileNames: `Assets/[name]${Date.now()}.js`,
                assetFileNames: `Assets/[name]${Date.now()}.[ext]`,
            },
            plugins: [
                inject({
                    React: 'react',
                    exclude: 'src/**',
                }),
            ],
        },
        commonjsOptions: { requireReturnsDefault: 'preferred' },
        target: 'esnext',
       // cssCodeSplit: false,
        modulePreload: false,
       // minify: true,
    },
    plugins: [
        react(),
        basicSsl(),
        //    federation({
        //     name: 'admin_app',
        //     filename: 'remoteEntry.js',
        //     exposes: {
        //         './Dashboard': './src/modules/dashboard/views/index',
        //         './mazaya': './src/modules/mazaya-managment/views/bootstrap.tsx',
        //         //'./mazaya': './src/modules/mazaya-managment/views/index.tsx',
        //     },
        //     shared: ['react', 'react-dom', 'react-router-dom', 'zustand'],
        // }),
    ],
    resolve: {
        alias: {
            '@': path.resolve(__dirname, './src'),
        },
    },
    esbuild: {
        drop: process.env.NODE_ENV === 'production' ? ['console', 'debugger'] : [],
        legalComments: 'none',
    },
    test: {
        environment: 'happy-dom',
        globals: true,
        setupFiles: ['./src/test-setup.ts'],
        include: ['src/**/*.{test,spec}.{js,jsx,ts,tsx}'],
        coverage: {
            reporter: ['text', 'json', 'html'],
            exclude: [
                'node_modules/',
                'src/test-setup.ts',
                '**/*.d.ts',
                '**/*.config.*',
                '**/dist/**',
            ],
        },
    },

    server: {
        port: 5173,
        host: true,
       // strictPort: true,
    },
    preview: {
        port: 5173,
    },
})
