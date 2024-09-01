export interface LogOptions {
    prefix: string;
}

export interface Context {
    indentationLevel: number;
}

declare function toText({ indentationLevel }: Context, data : string, {prefix }?: LogOptions): string;
