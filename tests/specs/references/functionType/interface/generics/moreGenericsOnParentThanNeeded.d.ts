export interface MyObject<A,B, NotNeeded> {
    foo: (min: A, max: B) => B;
}
