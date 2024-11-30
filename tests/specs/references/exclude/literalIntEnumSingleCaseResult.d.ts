export type NumberA =
	| 1
	| 2

export type NumberB = Exclude<NumberA, 2>;
