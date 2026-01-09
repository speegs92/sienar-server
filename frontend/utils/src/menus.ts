import { ref } from 'vue';
import { inject, provide } from '@utils/di.ts';

import type { Component, InjectionKey } from 'vue';

export const activeMenu = ref<InjectionKey<MenuLinkProvider>>(Symbol());
export const activeUtilsMenu = ref<InjectionKey<MenuLinkProvider>>(Symbol());

/**
 * Sets the active menu
 *
 * @param menu The new menu to make active
 */
export function setActiveMenu(menu: InjectionKey<MenuLinkProvider>): void {
	activeMenu.value = menu;
}

/**
 * Sets the active utility menu
 *
 * @param menu The new utility menu to make active
 */
export function setActiveUtilsMenu(menu: InjectionKey<MenuLinkProvider>): void {
	activeUtilsMenu.value = menu;
}

/**
 * Creates a new menu in the DI container
 *
 * @param key The injection key of the menu
 * @param displayName The display name of the menu
 */
export function nameMenu(
	key: InjectionKey<MenuLinkProvider>,
	displayName: string
): void {
	let menu = inject(key, true);
	if (menu) {
		menu.displayName = displayName;
		return;
	}

	menu = {
		displayName,
		links: {} as LinkDictionary
	};

	provide(key, menu);
}

/**
 * Adds a {@link MenuLink} to a {@link MenuLinkProvider} with normal priority
 *
 * @param key The key of the menu to which to add links
 * @param links The links to add
 */
export function addLinks(
	key: InjectionKey<MenuLinkProvider>,
	...links: MenuLink[]
): void {
	addLinksWithPriority(key, 'normal', ...links);
}

/**
 * Adds a {@link MenuLink} to a {@link MenuLinkProvider}
 *
 * @param key The key of the menu to which to add links
 * @param links The links to add
 * @param priority The priority of the link to add
 */
export function addLinksWithPriority(
	key: InjectionKey<MenuLinkProvider>,
	priority: MenuPriority,
	...links: MenuLink[]
) {
	links.forEach(link => {
		link.allRolesRequired ??= true;
		link.requireLoggedIn ??= false;
		link.requireLoggedOut ??= false;
		link.roles ??= [];
	});

	let dictionary = inject(key, true);
	if (!dictionary) {
		dictionary = {
			displayName: '',
			links: {} as LinkDictionary
		};
		provide(key, dictionary);
	}

	dictionary.links[priority] ??= [];
	dictionary.links[priority].push(...links);
}

/**
 * Aggregates an array of {@link MenuLink} that have been registered to the given menu name
 *
 * @param key The key of the menu from which to aggregate links
 */
export function aggregateLinks(key: InjectionKey<MenuLinkProvider>): MenuLink[] {
	const includedLinks: MenuLink[] = [];
	const menu = inject(key);

	const priorities: MenuPriority[] = [
		'highest',
		'high',
		'normal',
		'low',
		'lowest'
	];
	for (let priority of priorities) {
		const prioritizedLinks = menu.links[priority];
		prioritizedLinks?.forEach(l => {
			if (l.childMenu) {
				l.sublinks = aggregateLinks(l.childMenu);
			}
			includedLinks.push(...prioritizedLinks);
		});
	}

	return includedLinks;
}

/**
 * Filters links to determine which links the current user is able to view. Works recursively for {@link MenuLink} with nested links.
 *
 * @param links The array of {@link MenuLink} to filter
 * @param userIsLoggedIn Whether the current user is logged in
 * @param userRoles The roles of the current user
 */
export function filterLinks(
	links: MenuLink[],
	userIsLoggedIn: boolean,
	userRoles: string[]): MenuLink[] {
	const includedLinks: MenuLink[] = [];

	for (let link of links) {
		if (!userIsAuthorizedToViewLink(link, userIsLoggedIn, userRoles)) {
			continue;
		}

		if (link.sublinks) {
			link.sublinks = filterLinks(link.sublinks, userIsLoggedIn, userRoles);
		}

		includedLinks.push(link);
	}

	return includedLinks;
}

/**
 * Determines if a user is authorized to view a link based on their sign-in status and current roles
 *
 * @param link The link to check for authorization
 * @param userIsSignedIn Whether the current user is signed in to the application
 * @param userRoles The roles of the current user
 */
export function userIsAuthorizedToViewLink(
	link: MenuLink,
	userIsSignedIn: boolean,
	userRoles: string[]): boolean {
	if (link.requireLoggedIn && !userIsSignedIn) return false;
	if (link.requireLoggedOut && userIsSignedIn) return false;
	if (!link.roles || link.roles.length === 0) return true;

	const linkRoles: string[] = [];
	if (Array.isArray(link.roles)) {
		linkRoles.push(...link.roles);
	} else {
		linkRoles.push(link.roles);
	}

	for (let role of linkRoles) {
		if (userRoles.includes(role)) {
			if (link.allRolesRequired) continue;
			return true;
		}

		if (link.allRolesRequired) return false;
	}

	// Default is added when the links are added, so this is guaranteed not to be undefined
	return link.allRolesRequired as boolean;
}

/**
 * A container for {@link MenuLink} objects
 */
export type MenuLinkProvider = {
	/**
	 * The display name of the menu
	 */
	displayName: string,

	/**
	 * The links contained in the menu
	 */
	links: LinkDictionary
};

/**
 * A container for {@link MenuLink} arrays with a {@link MenuPriority} key representing the render order of that key's links
 */
export type LinkDictionary = {
	[id in MenuPriority]: MenuLink[];
};

/**
 * Contains all the data needed to create a menu link
 */
export type MenuLink = {
	/**
	 * The display text of the link
	 */
	text: string

	/**
	 * The URL the link points to, if any
	 */
	href?: string|InjectionKey<string>

	/**
	 * The icon to show along with the link, if any
	 */
	icon?: Component

	/**
	 * Whether the authorization requirements stored in the roles array should be satisfied by all roles in the array being present, or only by a single role being present
	 */
	allRolesRequired?: boolean

	/**
	 * Whether the link should only be displayed if the user is logged in
	 */
	requireLoggedIn?: boolean

	/**
	 * Whether the link should only be displayed if the user is logged out
	 */
	requireLoggedOut?: boolean

	/**
	 * The role(s) required to see the link in the menu, if any
	 */
	roles?: string[]|string

	/**
	 * The menu to render as a submenu, if any
	 */
	childMenu?: InjectionKey<MenuLinkProvider>

	/**
	 * Child links to display in a submenu, if any
	 */
	sublinks?: MenuLink[]

	/**
	 * The icon to render at the end of a menu link
	 */
	endIcon?: Component
}

/**
 * Represents the priority order in which menu items should be rendered
 */
export type MenuPriority =
	| 'lowest'
	| 'low'
	| 'normal'
	| 'high'
	| 'highest';
