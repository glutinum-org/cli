interface A {
    a: string;
}

interface B {
    b: number;
}

export interface Page {
    m(options: Partial<A>): string;
    m(options: Partial<B>): number;
}
