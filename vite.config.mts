import { defineConfig } from 'vite';
import path from 'path'
import dts from 'vite-plugin-dts'

export default defineConfig({
    plugins: [
        dts({
            entryRoot: 'src/ROCratePCC',
            include: ['src/ROCratePCC/**/*.ts', 'src/ROCratePCC/**/*.mts'],
        }),
    ],
    build: {
        outDir: 'dist/ts', // Both JS and .d.ts files will go here
        target: 'esnext',
        lib: {
          entry: path.resolve(__dirname, 'src/ROCratePCC/index.ts'),
          name: 'ROCratePCC', // Global variable name if using UMD/IIFE
          fileName: (format) => `ROCratePCC.${format}.js`,
          formats: ['es'], // Common formats
        },
        rollupOptions: {
            external: [
                'fable-org/fable-library-js',
                '@nfdi4plants/exceljs',
                'isomorphic-fetch',
                'fs',
                'path',
                'stream',
                'buffer',
                'fs/promises',
            ], // put your external packages here
            output: {
                preserveModules: true,
                preserveModulesRoot: 'src/ROCratePCC',
                entryFileNames: "[name].js",
                chunkFileNames: "[name].js",
            },
            treeshake: false
        },
        minify: false
    },
    test: {
        globals: true,
        include : ['Main.fs.ts', '*.test.ts'],
        testTimeout: 1_000_000,
    }
  });