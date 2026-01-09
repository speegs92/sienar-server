import { resolve } from 'path';
import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import dts from 'vite-plugin-dts';

const external = [
	'vue',
	'vue-router'
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
			'@utils': resolve(__dirname, 'src')
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
