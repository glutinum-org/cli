export type ForegroundColor =
	| 'black'
	| 'red'
    | 'green'

export type NoBlack = Exclude<ForegroundColor, 'black'>
