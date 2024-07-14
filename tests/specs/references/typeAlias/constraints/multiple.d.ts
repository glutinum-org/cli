export class Foo<A> { }

type ReturnType<A, T extends Foo<A>, B extends Foo<A>> = [A, T, B]
