declare function f1(): { a: number; b: string };

export type T4 = ReturnType<typeof f1>;
