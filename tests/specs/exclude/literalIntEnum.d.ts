export type NumberA =
	| 1
	| 2
    | 3

export type NumberB = Exclude<NumberA, 3>;
