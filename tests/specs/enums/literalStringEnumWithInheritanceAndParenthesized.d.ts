export type ColorA =
	| 'black'

export type ColorB =
	| 'bgBlack'

export type ColorC =
	| 'red'

export type Color = ColorA | (ColorB | ColorC);
