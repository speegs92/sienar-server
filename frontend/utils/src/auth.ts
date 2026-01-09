import { computed, ref } from 'vue';
import type { ComputedRef } from 'vue';

export const authData = ref<AuthData>({
	isLoggedIn: false,
	username: null,
	roles: []
});

/**
 * Logs the user in to the UI
 *
 * @summary The <code>login()</code> function does not actually log the user in to the application on the server; rather, it modifies the application's global state to include values of <code>isLoggedIn = true, username = &lt;username&gt;, roles = &lt;roles&gt;</code>. Calling this function can simulate a user login for the client-side application, but without calling other application code to actually log in to the backend application, the user is still functionally logged out.
 *
 * @param username The user's username
 * @param roles The user's roles. If no roles are present, an empty array should be provided
 */
export function login(username: string, roles: string[]) {
	authData.value.isLoggedIn = true;
	authData.value.username = username;
	authData.value.roles = roles;
}

/**
 * Logs the user out in the UI
 *
 * @summary The <code>logout()</code> function does not actually log the user out of the application on the server; rather, it modifies the application's global state to include values of <code>isLoggedIn = false, username = null, roles = []</code>. If the user refreshes the page, they will once again appear to be logged into the application if an automatic function call verifies the user's login status with the backend. The method of logging out of the application will depend on the backend used.
 */
export function logout() {
	authData.value.isLoggedIn = false;
	authData.value.username = null;
	authData.value.roles = [];
}

/**
 * Determines whether the user is authorized according to the supplied auth criteria
 *
 * If <code>authRoles</code> is falsy, it checks if the user is logged in. If <code>authRoles</code> is a string, it checks if the user is in any role matching the value of that string. If <code>authRoles</code> is an array, it checks if the user matches the roles in that array.
 *
 * If <code>any</code> is <code>true</code>, the user must satisfy any number of roles in the <code>authRoles</code> array. If <code>any</code> is false, the user must satisfy all roles in the <code>authRoles</code> array. If <code>authRoles</code> is not an array, the <code>any<code> parameter does nothing.
 *
 * @param authRoles The role(s) a user should have in order to be authorized
 * @param any Whether the user should match any or all roles
 */
export function useAuthorized(
	authRoles: string|string[]|null = null,
	any: boolean = false
): ComputedRef<boolean> {
	return computed<boolean>(() => {
		const { isLoggedIn, roles: userRoles } = authData.value;

		if (!authRoles) {
			return isLoggedIn;
		}

		if (!isLoggedIn) {
			return false;
		}

		if (typeof authRoles === 'string') {
			return userRoles.includes(authRoles);
		}

		if (Array.isArray(authRoles)) {
			for (let role of authRoles) {
				const found = userRoles.includes(role);

				// If we found the role and any role will do, the user is authorized
				if (found && any) {
					return true;
				}

				// If we didn't find the role and all roles are required, the user isn't authorized
				else if (!found && !any) {
					return false;
				}
			}

			// If we got here, either no roles were found when any role will do,
			// or all roles were found when all roles were required
			// so the result is the opposite of whether the any prop is set
			return !any;
		}

		// Shouldn't ever get here...famous last words
		// This might happen if, for example, a developer is not using Typescript
		// and passes a non-string, non-array value to authRoles
		return false;
	});
}

/**
 * The authentication data of the current user
 */
export type AuthData = {
	/**
	 * Whether the user is logged in
	 */
	isLoggedIn: boolean;

	/**
	 * The user's username if they are logged in, else <code>null</code>
	 */
	username: string|null;

	/**
	 * The user's assigned roles
	 */
	roles: string[];
}
