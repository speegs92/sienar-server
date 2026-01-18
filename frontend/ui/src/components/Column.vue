<script
	lang='ts'
	setup
>
import type { BlockContainer, ColumnSize } from '@ui/theme.ts';

export type ColumnProps = {
	/**
	 * The default colum size (below the lowest breakpoint)
	 */
	col?: ColumnSize;

	/**
	 * The column size for small screens
	 */
	sm?: ColumnSize;

	/**
	 * The column size for medium screens
	 */
	md?: ColumnSize;

	/**
	 * The column size for large screens
	 */
	lg?: ColumnSize;

	/**
	 * The column size for extra-large screens
	 */
	xl?: ColumnSize;

	/**
	 * The column size for extra-extra-large screens
	 */
	xxl?: ColumnSize;

	/**
	 * The HTML tag with which to render the column
	 */
	tag?: BlockContainer;
}

const {
	col = '12',
	tag = 'div'
} = defineProps<ColumnProps>();
</script>

<template>
	<component
		:class='[
			"column",
			`col-${col}`,
			{
				[`col-sm-${sm}`]: !!sm,
				[`col-md-${md}`]: !!md,
				[`col-lg-${lg}`]: !!lg,
				[`col-xl-${xl}`]: !!xl,
				[`col-xxl-${xxl}`]: !!xxl
			}
		]'
		:is='tag'
	>
		<slot/>
	</component>
</template>
