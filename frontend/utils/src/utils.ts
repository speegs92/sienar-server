/**
 * Simulates waiting for a specified amount of time. Probably only useful during development.
 *
 * @param time The amount of time to "sleep", in milliseconds
 */
export function sleep(time: number): Promise<void> {
	return new Promise(resolve => setTimeout(resolve, time));
}

/**
 * Converts a GMT date from the server into a local date string
 *
 * @param date The GMT date from the server
 * @param removeAt Whether to remove the ' at' separating the date and the time in the date string
 * @returns The parsed date if the date is valid, else <code>null</code>
 */
export function getDateString(
	date: string|null|undefined,
	removeAt: boolean = true
): string|null {
	if (!date) return null;

	const parsed = new Date(`${date}Z`);
	if (isNaN(parsed.getTime())) return null;
	const parsedString = parsed.toLocaleString('en-US', {
		timeZoneName: 'short',
		hour12: true,
		weekday: 'long',
		month: 'long',
		day: 'numeric',
		year: 'numeric',
		hour: '2-digit',
		minute: '2-digit',
		second: '2-digit'
	});

	return removeAt
		? parsedString.replace(' at', '')
		: parsedString;
}

/**
 * Retrieves a JavaScript-accessible cookie value by name
 *
 * @param name The name of the cookie to retrieve
 * @returns The value of the cookie if set, else <code>undefined</code>
 */
export function getCookie(name: string): string|undefined {
	// The cookie string separates cookies by a semicolon
	const cookies = document.cookie.split(';');

	for (let cookie of cookies) {
		// Cookies are stored as name=value pairs
		const parts = cookie.split('=');

		// The name is the first part, trimmed
		const cookieName = parts.shift()!.trim();

		if (cookieName === name) {
			// The value is the rest of the parts joined by a '=' character, trimmed
			// The join is performed in case a '=' character was found in the value of the cookie
			// in which case the parts constant will have length > 1
			return parts.join('=').trim();
		}
	}

	// The cookie isn't set
	return undefined;
}

/**
 * A base type containing the fields required by all Sienar entities
 */
export type EntityBase = {
	/**
	 * The primary key of the entity
	 */
	id: number

	/**
	 * A unique value that ensures the entity is not modified concurrently
	 */
	concurrencyStamp: string
}
