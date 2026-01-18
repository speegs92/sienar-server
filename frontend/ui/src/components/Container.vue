<script
	setup
	lang="ts">
import type { Alignment, BlockContainer, Breakpoint } from '@ui/theme.ts';

/**
 * The props for the container component
 */
export type ContainerProps = {
	/**
	 * The horizontal alignment of the container
	 */
	alignment?: Alignment;

	/**
	 * The maximum width of the container
	 */
	maxWidth?: Breakpoint;

	/**
	 * The HTML tag with which to render the container
	 */
	tag?: BlockContainer;
}

const {
	alignment = 'center',
	maxWidth,
	tag = 'div'
} = defineProps<ContainerProps>();
</script>

<template>
	<component
		:is='tag'
		:class='[
			"container",
			`container--${alignment}`,
			{
				[`container--${maxWidth}`]: !!maxWidth
			}
		]'>
		<slot/>
	</component>
</template>
