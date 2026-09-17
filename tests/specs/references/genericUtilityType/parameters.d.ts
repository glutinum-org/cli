export type Handler = (name: string, count: number) => void;

export type HandlerArgs = Parameters<Handler>;

export declare function apply(...args: Parameters<Handler>): void;
