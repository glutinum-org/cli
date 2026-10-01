declare class Client {
    constructor(baseURL?: string);
    health(): boolean;
}

declare function helper(): void;

declare class Internal {
    hidden(): void;
}

export { Client as default, helper };
