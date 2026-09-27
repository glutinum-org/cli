export interface Options {
    alias?: string;
    describe?: string;
}

export interface Parser {
    option<O extends Options>(key: string, options: O): Parser;
}
