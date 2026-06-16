interface User {
    id: string;
    name: string;
    password: string;
}

type Picked<T, K extends keyof T> = Pick<T, K>;

export type IdName = Picked<User, "id" | "name">;
