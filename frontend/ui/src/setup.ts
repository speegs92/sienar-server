import { addAppConfigurator } from '@sienar/utils';
import * as components from '@ui/components/index.ts';

import '@ui/styles/utilities/index.scss';
import '@mdi/font/fonts/materialdesignicons-webfont.eot'
import '@mdi/font/fonts/materialdesignicons-webfont.ttf'
import '@mdi/font/fonts/materialdesignicons-webfont.woff'
import '@mdi/font/fonts/materialdesignicons-webfont.woff2'
import '@mdi/font/css/materialdesignicons.min.css';

import type { App } from 'vue';

export default function sienarUiSetup() {
	addAppConfigurator(registerComponents);
}

function registerComponents(app: App<Element>) {
	app
		.component('Container', components.Container)
		.component('Icon', components.Icon);
}

declare module 'vue' {
	export interface GlobalComponents {
		Container: typeof components.Container;
		Icon: typeof components.Icon;
	}
}
