import { addAppConfigurator, createRouter, createSienarApp, provide, registerRoutes } from '@sienar/utils';
import { setup as sienarUiSetup } from '@sienar/ui';
import HomeView from '@/views/Home.vue';
import MainLayout from '@/layouts/MainLayout.vue';

import type { Component, InjectionKey } from 'vue';

sienarUiSetup();

const MAIN_LAYOUT = Symbol() as InjectionKey<Component>;
provide(MAIN_LAYOUT, MainLayout);

registerRoutes(MAIN_LAYOUT, {
	path: '',
	component: HomeView
});

addAppConfigurator(app => {
	app.use(createRouter());
});

createSienarApp();
