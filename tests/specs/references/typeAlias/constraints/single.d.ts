export class Foo<A> { }

type ReturnType<A, T extends Foo<A>> = T
