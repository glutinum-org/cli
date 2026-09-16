export interface Collection<StoreType = unknown, ContextType = StoreType> {
    start: StoreType;
    context: ContextType;
}

export declare class TracingChannel<StoreType = unknown, ContextType extends object = {}> implements Collection<StoreType, ContextType> {
    start: StoreType;
    context: ContextType;
}

export declare class Named<Options = { verbose: boolean }> {
    options: Options;
}
