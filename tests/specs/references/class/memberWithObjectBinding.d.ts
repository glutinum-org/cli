export interface LogOptions {
    prefix: string;
}

export interface Context {
    indentationLevel: number;
}

export declare class Signature {
    toText({ indentationLevel }: Context, data : string, {prefix }?: LogOptions): string;
}
