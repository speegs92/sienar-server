import { resolve } from 'path';
import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import dts from 'vite-plugin-dts';

const external = [
	'vue',
	'vue-router',
	'@sienar/utils'
];

// https://vite.dev/config/
export default defineConfig({
	build: {
		lib: {
			entry: {
				'index': './src/index.ts'
			},
			formats: [ 'es' ]
		},
		rollupOptions: { external }
	},
	esbuild: {
		minifyIdentifiers: false
	},
	optimizeDeps: {
		exclude: external
	},
	resolve: {
		alias: {
			'@ui': resolve(__dirname, 'src'),
			'@sienar/utils': resolve(__dirname, '../utils/src'),
			'@utils': resolve(__dirname, '../utils/src'),
		}
	},
	plugins: [
		vue(),
		dts({
			rollupTypes: true,
			tsconfigPath: './tsconfig.app.json'
		})
	]
});
