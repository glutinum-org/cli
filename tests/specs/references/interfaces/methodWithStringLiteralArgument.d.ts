interface Page {
    on(event: 'console', listener: (consoleMessage: void) => any): this;
    on(event: 'ready'): this;
}
