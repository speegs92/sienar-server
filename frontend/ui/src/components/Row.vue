<script
	lang='ts'
	setup
>
import type { BlockContainer } from '@ui/theme.ts';

export type RowProps = {
	/**
	 * the HTML tag with which to render the row
	 */
	tag?: BlockContainer;
}

const { tag = 'div' } = defineProps<RowProps>();
</script>

<template>
	<component
		class='row'
		:is='tag'
	>
		<slot/>
	</component>
</template>
