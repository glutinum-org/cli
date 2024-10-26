type ClientLikeCtr<T> = new(config?: string) => T;

declare class Pool<T> {
    Client: ClientLikeCtr<T>;
}
