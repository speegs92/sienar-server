import type { InjectionKey } from 'vue';

const di: Record<InjectionKey<any>, any> = {};

/**
 * Registers a value in the Sienar DI container
 *
 * @param key The unique key that identifies the value in the container
 * @param value The value to register
 * @param override Whether to override an existing value if one is already registered
 */
export function provide<T>(
	key: InjectionKey<T>,
	value: T,
	override: boolean = true
) {
	if (override) di[key] = value;
	else di[key] ??= value;
}

/**
 * Injects a required value from the Sienar DI container
 *
 * @param key The unique key that identifies the value in the container
 * @param optional Whether to fail if the value is not found
 * @returns The value if it exists
 */
export function inject<T>(key: InjectionKey<T>, optional?: false): T;

/**
 * Injects an optional value from the Sienar DI container
 *
 * @param key The unique key that identifies the value in the container
 * @param optional Whether to fail if the value is not found
 * @returns The value
 *
 * @throws Error If no value is registered with the given key
 */
export function inject<T>(key: InjectionKey<T>, optional: true): T|undefined;

export function inject<T>(key: InjectionKey<T>, optional: boolean = false): T {
	const value = di[key];
	if (typeof value === 'undefined' && !optional) throw new Error('Unable to locate value with the provided key');

	return value;
}
