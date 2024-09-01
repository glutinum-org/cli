export interface LogOptions {
    prefix: string;
}

export interface Context {
    indentationLevel: number;
}

declare class Signature {
    toText({ indentationLevel }: Context, data : string, {prefix }?: LogOptions): string;
}
