export interface TerminalOptions {
    prefix: string
}

export interface ExtensionTerminalOptions {
    suffix: string
}

export type Test = Readonly<TerminalOptions & ExtensionTerminalOptions>
