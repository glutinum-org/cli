export interface WithMethod {
    name: string;
    toString(): string;
}

export declare function withMethod(value: WithMethod): void;
