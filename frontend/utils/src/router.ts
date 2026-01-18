import { createRouter as createVueRouter, createWebHistory, useRouter } from 'vue-router';
import { inject } from '@utils/di.ts';

import type { Component, InjectionKey } from 'vue';
import type { Router, RouteRecordRaw } from 'vue-router';

const routes = new Map<(InjectionKey<InjectionKey<Component>>|InjectionKey<Component>), Route[]>();

/**
 * Registers one or more routes to be rendered with the given layout
 *
 * @param layoutKey The injection key of the layout to register
 * @param items The routes to be rendered
 */
export function registerRoutes(
	layoutKey: InjectionKey<InjectionKey<Component>> | InjectionKey<Component>,
	...items: Route[]
): void {
	if (!routes.has(layoutKey)) {
		routes.set(layoutKey, []);
	}

	routes.get(layoutKey)!.push(...items);
}

/**
 * Creates a Vue Router object based on the currently registered routes
 */
export function createRouter(): Router {
	const layoutRoutes: RouteRecordRaw[] = [];
	const mappedRoutes = new Map<Component, Route[]>();

	for (let [layoutKey, childRoutes] of routes) {
		const injectedValue: Component | InjectionKey<Component> = inject(layoutKey);
		const layout = (typeof injectedValue === 'symbol'
			? inject(injectedValue)
			: injectedValue) as Component;

		if (!mappedRoutes.has(layout)) {
			mappedRoutes.set(layout, []);
		}

		mappedRoutes
			.get(layout)!
			.push(...childRoutes);
	}

	for (let [layout, childRoutes] of mappedRoutes) {
		const flatRoutes = flattenRoutes(childRoutes);
		for (let route of flatRoutes) {
			layoutRoutes.push({
				path: route.path,
				component: layout,
				children: [
					{
						path: '',
						component: route.component!
					}
				]
			})
		}
	}

	return createVueRouter({
		history: createWebHistory(),
		routes: layoutRoutes
	});
}

/**
 * A custom composable that navigates to the given URL, whether it is a string URL or an injection key
 */
export function useNavigate() {
	const router = useRouter();

	return async (
		destination: string|InjectionKey<string>,
		queryParams?: Record<string, any>|undefined
	) => {
		const url = typeof destination === 'string'
			? destination
			: inject(destination);

		return router.push({
			path: url,
			query: queryParams
		});
	}
}

function flattenRoutes(routes: Route[]): RouteRecordRaw[] {
	const finalRoutes: RouteRecordRaw[] = [];

	for (const route of routes) {
		const routePath = extractRoutePath(route.path);
		const routeChildren = route.children;
		if (route.component) {
			route.children = undefined;
			finalRoutes.push({
				path: extractRoutePath(route.path),
				component: route.component
			});
		}

		if (routeChildren) {
			const children = flattenRoutes(routeChildren);
			for (const child of children) {
				const childPath = extractRoutePath(child.path);
				finalRoutes.push({
					path: childPath ? `${routePath}/${childPath}` : routePath,
					component: child.component!
				});
			}
		}
	}

	return finalRoutes;
}

function extractRoutePath(path: string|InjectionKey<string>): string {
	if (typeof path === 'string') {
		return path;
	}

	return inject(path);
}

/**
 * Represents a Sienar route
 */
export type Route = {
	/**
	 * The URL of the route
	 */
	path: string | InjectionKey<string>;

	/**
	 * The component to render for the route
	 */
	component?: Component | InjectionKey<Component>;

	/**
	 * The child routes of the route
	 */
	children?: Route[];
}
