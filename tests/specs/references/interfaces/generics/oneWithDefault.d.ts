export interface Type2<A = string> {}

interface Task {}

export interface Type1<A extends Task = Task> {}
