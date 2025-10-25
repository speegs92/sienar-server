import { resolve } from 'path';
import { defineConfig } from 'vite';

export default defineConfig({
	build: {
		lib: {
			entry: {},
			formats: [ 'es' ],
			name: 'sienar'
		},
		rollupOptions: {
			input: {
				'sienar-ui': './src/main.ts',
				'theme-sienar': resolve(__dirname, 'styles/themes/sienar/index.scss'),
				'sienar-utils': resolve(__dirname, 'styles/utils/index.scss')
			}
		},
		outDir: resolve(__dirname, '../wwwroot'),
		cssCodeSplit: true,
		emptyOutDir: true
	},
	esbuild: {
		minifyIdentifiers: false
	}
});