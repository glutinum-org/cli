export module Log {

    export interface Options {
        prefix: string
    }

    export interface Options {
        suffix: string
    }
}

export module Log {

    export function log() : void;

    export interface Options {
        level: number
    }
}
