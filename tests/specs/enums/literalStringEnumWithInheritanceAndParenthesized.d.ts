declare type ColorC =
	| 'black'

declare type ColorD =
	| 'bgBlack'

declare type ColorE =
	| 'red'

declare type ColorF = ColorC | (ColorD | ColorE);
