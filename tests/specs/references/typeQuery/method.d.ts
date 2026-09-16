export declare class Headers {
    static parseParameters(value: string): number;
    get(parser: typeof Headers.parseParameters): number;
}

export interface Api {
    run(value: string): boolean;
}

export declare const api: Api;

export interface UsesSignature {
    runner: typeof api.run;
}
