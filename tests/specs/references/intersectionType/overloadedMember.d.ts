export interface Instance {
    foo: string
    listen(port: number): void
    listen(): void
}

export interface Wrapper<T> {
    then(): void
}

export declare function make(): Instance & Wrapper<Instance>;
