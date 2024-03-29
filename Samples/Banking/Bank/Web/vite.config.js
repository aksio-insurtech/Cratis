// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { ViteEjsPlugin } from 'vite-plugin-ejs';
import path from 'path';

export default defineConfig({
    build: {
        outDir: './wwwroot',
        assetsDir: '',
        rollupOptions: {
            external: [
                "@aksio/applications"
            ]
        }
    },
    plugins: [
        react(),
        ViteEjsPlugin((viteConfig) => {
            return {
                root: viteConfig.root,
                domain: 'cratis.io',
                title: 'Cratis Sample'
            };
        })
    ],
    server: {
        port: 9100,
        proxy: {
            '/api': {
                target: 'http://localhost:5100',
                ws: true
            },
            '/swagger': {
                target: 'http://localhost:5100'
            }
        }
    },
    resolve: {
        alias: {
            'API': path.resolve('./API')
        }
    },
});
