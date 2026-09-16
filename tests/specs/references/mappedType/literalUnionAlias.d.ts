export type Signals = "SIGINT" | "SIGTERM";

export type SignalConstants = {
    [key in Signals]: number;
};

export declare let styles: {
    [K in "special" | "number"]: string;
};
