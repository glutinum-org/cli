interface Animal {
    live(): void;
}

interface Dog extends Animal {
    woof(): void;
}

export type Example1 = Dog extends Animal ? number : string;

export type Example2 = RegExp extends Animal ? number : string;
