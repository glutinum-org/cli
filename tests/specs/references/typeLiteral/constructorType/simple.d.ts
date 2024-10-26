type ClientLikeCtr = new(config?: string) => number;

declare class Pool {
    Client: ClientLikeCtr;
}
