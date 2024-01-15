export interface Logger {
    hello() : string;
}

type L = keyof Logger;
