export type Message = { uid: string; nsp: string } & { type: "ping"; data: number };

export interface Publisher {
    publish(message: Omit<Message, "nsp" | "uid">): void;
}

export interface Distributive<T> {
    send(message: T extends any ? Omit<T, "uid"> : never): void;
}

export type Sender = Distributive<Message> & { name: string };
