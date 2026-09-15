interface Foo {
    a: string;
    b: number;
}

export type FooKeys = keyof (Foo);
