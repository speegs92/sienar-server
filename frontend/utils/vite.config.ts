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
	plugins: [
		vue(),
		dts({
			rollupTypes: true,
			tsconfigPath: './tsconfig.app.json'
		})
	]
});
