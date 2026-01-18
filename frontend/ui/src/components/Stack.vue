<script
	lang='ts'
	setup
>
import { computed } from 'vue';
import type { BlockContainer, FlexAlign, FlexDirection, FlexJustify } from '@ui/theme.ts';

export type StackProps = {
	/**
	 * The item placement axis of the stack
	 */
	direction?: FlexDirection;

	/**
	 * The cross axis flex distribution
	 */
	align?: FlexAlign;

	/**
	 * The main axis flex distribution
	 */
	justify?: FlexJustify;

	/**
	 * Whether the flex direction should be reversed
	 */
	reverse?: boolean;

	/**
	 * The HTML tag with which to render the stack
	 */
	tag?: BlockContainer;
}

const {
	direction = 'horizontal',
	align: flexAlign,
	justify: flexJustify,
	tag = 'div'
} = defineProps<StackProps>();

const align = computed<StackProps['align']>(() => {
	console.log(direction);
	if (flexAlign) {
		return flexAlign;
	}

	return direction === 'horizontal'
		? 'start'
		: 'stretch';
});

const justify = computed<StackProps['justify']>(() => {
	if (flexJustify) {
		return flexJustify;
	}

	return direction === 'horizontal'
		? 'between'
		: 'end';
});
</script>

<template>
	<component
		:is='tag'
		:class='[
			"stack",
			"d-flex",
			"flex-wrap",
			`justify-content-${justify}`,
			`align-items-${align}`,
			{
				"flex-row": direction === "horizontal" && !reverse,
				"flex-row-reverse": direction === "horizontal" && reverse,
				"flex-column": direction === "vertical" && !reverse,
				"flex-column-reverse": direction === "vertical" && reverse
			}
		]'
	>
		<slot/>
	</component>
</template>
