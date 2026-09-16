export interface TerminalOptions {
    name?: string;
}

export interface Terminal {
    readonly options: TerminalOptions;
    dispose(): void;
}

export declare function createTerminal(options: TerminalOptions): Terminal;
