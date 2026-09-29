interface Instance {
    parse(): void;
}

declare const createInstance: (args?: ReadonlyArray<string> | string, cwd?: string) => Instance;

export default createInstance;
