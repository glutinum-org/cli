interface User {
    id: string;
    name: string;
}

type Picked<T, K extends keyof T> = Pick<T, K>;

export declare function f(x: Picked<User, "id">): void;
