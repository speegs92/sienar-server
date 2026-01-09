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
