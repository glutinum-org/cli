export interface Settings {
    optional?: string | string[] | undefined;
    required: string | string[] | undefined;
    update(value?: string | string[] | undefined): void;
}

export interface PartialSettings extends Partial<Settings> {}

export declare class Store {
    value?: string | string[] | undefined;
    constructor(value?: string | string[] | undefined);
}

export declare function read(key?: string | string[] | undefined): string;
