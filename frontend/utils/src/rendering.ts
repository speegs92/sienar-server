import { createApp } from 'vue';
import { provide } from '@utils/di.ts';
import SienarApp from '@utils/components/SienarApp.vue';

import type { App, InjectionKey } from 'vue';

const appConfigurators: AppConfigurator[] = [];

export const APP = Symbol() as InjectionKey<App<Element>>;

/**
 * Adds a configurator function to the app startup pipeline
 *
 * @param configurator The configurator to add
 */
export function addAppConfigurator(
	configurator: AppConfigurator
): void {
	appConfigurators.push(configurator);
}

/**
 * Creates a Sienar app
 *
 * @param rootId The ID of the element to use as the mount point. Include the hash symbol.
 */
export function createSienarApp(rootId: string = '#app') {
	const app = createApp(SienarApp);

	appConfigurators.forEach(c => c(app));

	app.mount(rootId);

	provide(APP, app);
}

/**
 * A function that configures the Vue app instance prior to mounting
 */
export type AppConfigurator = {
	(app: App<Element>): void
}