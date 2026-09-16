export interface Base {
    id: string;
}

export interface Session extends Base {
    open(): void;
}

export declare class Session extends Base {
    close(): void;
}
