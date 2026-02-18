export class Type2<A = string> {}

interface Task {}

export class Type1<A extends Task = Task> {}
