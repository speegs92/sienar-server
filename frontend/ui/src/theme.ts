/**
 * The theme colors supported by Sienar UI
 */
export type Color =
	| 'primary'
	| 'secondary'
	| 'tertiary'
	| 'success'
	| 'info'
	| 'warning'
	| 'error'
	| 'light'
	| 'dark';

/**
 * The color solidity variants supported by Sienar UI
 */
export type Variant =
	| 'solid'
	| 'outlined'
	| 'text';

/**
 * Horizontal aligment
 */
export type Alignment =
	| 'left'
	| 'right'
	| 'center';

/**
 * Width breakpoints
 */
export type Breakpoint =
	| 'sm'
	| 'md'
	| 'lg'
	| 'xl'
	| 'xxl';

/**
 * HTML5 semantic block tag names
 *
 * This excludes standard HTML elements such as <code>&lt;ul&gt;</code> or <code>&lt;p&gt;</code>. <code>SemanticBlockContainer</code> is intended only to describe HTML tags which are functionally identical to <code>&lt;div&gt;</code> but which provide additional semantic meaning.
 */
export type SemanticBlockContainer =
	| 'article'
	| 'aside'
	| 'footer'
	| 'header'
	| 'main'
	| 'nav'
	| 'section';

/**
 * HTML block tag names
 *
 * This excludes standard HTML elements such as <code>&lt;ul&gt;</code> or <code>&lt;p&gt;</code>. <code>SemanticBlockContainer</code> is intended only to describe HTML tags which are functionally identical to <code>&lt;div&gt;</code>.
 */
export type BlockContainer = SemanticBlockContainer | 'div';
