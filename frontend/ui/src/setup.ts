import { addAppConfigurator } from '@sienar/utils';
import * as components from '@ui/components/index.ts';
import '@ui/styles/utilities/index.scss';

import type { App } from 'vue';

export default function sienarUiSetup() {
	addAppConfigurator(registerComponents);
}

function registerComponents(app: App<Element>) {
	app
		.component('Container', components.Container);
}

declare module 'vue' {
	export interface GlobalComponents {
		Container: typeof components.Container
	}
}
