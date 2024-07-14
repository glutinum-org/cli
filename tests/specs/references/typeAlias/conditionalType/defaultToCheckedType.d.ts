export class Foo {}
export class Bar<A> {}

// Works with concrete types
export type ReturnType = Foo extends(...args:any)=>infer R?R:any;
export type ReturnType1<A> = Bar<A> extends(...args:any)=>infer R?R:any;

// Works with generic types
export type ReturnType2<T extends(...args:any)=>any>=T extends(...args:any)=>infer R?R:any;
