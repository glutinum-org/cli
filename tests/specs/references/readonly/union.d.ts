export interface TerminalOptions {
    prefix: string
}

export interface ExtensionTerminalOptions {
    suffix: string
}

export type Foo = Readonly<TerminalOptions | ExtensionTerminalOptions>;
