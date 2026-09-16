export interface Config {
    options: {
        strict: boolean;
    };
}

export declare function get<T extends Config, K extends keyof T["options"] = keyof T["options"]>(config: T, key: K): void;
