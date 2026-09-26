export interface Parsed {
    valid: boolean
}

export declare function parse(text: string): Parsed;
export declare function parse(text: string, format?: string, strict?: boolean): Parsed;
export declare function parse(text: string, format?: string, locale?: string, strict?: boolean): Parsed;
