export interface TerminalOptions {
    prefix: string
}

export interface Foo {
    terminal: Readonly<TerminalOptions>
}
