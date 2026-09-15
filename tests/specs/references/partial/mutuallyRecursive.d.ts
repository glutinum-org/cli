export interface A {
    b: Partial<B>;
}

export interface B {
    a: Partial<A>;
}
