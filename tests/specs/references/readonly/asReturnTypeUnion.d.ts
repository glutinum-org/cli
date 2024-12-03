export interface TerminalOptions {
    prefix: string
}

export interface ExtersionTerminalOptions {
    suffix: string
}


export interface Foo {
    terminal: Readonly<TerminalOptions | ExtersionTerminalOptions>
}
