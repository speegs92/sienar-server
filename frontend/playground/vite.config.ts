import { resolve } from 'path';
import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';

// https://vite.dev/config/
export default defineConfig({
	resolve: {
		alias: {
			'@': resolve(__dirname, './src'),
			'@sienar/utils': resolve(__dirname, '../utils/src'),
			'@utils': resolve(__dirname, '../utils/src'),
			'@sienar/ui': resolve(__dirname, '../ui/src'),
			'@ui': resolve(__dirname, '../ui/src')
		}
	},
	plugins: [
		vue()
	]
});
