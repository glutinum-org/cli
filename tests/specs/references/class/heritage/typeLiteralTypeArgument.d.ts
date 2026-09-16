export declare class Base<I> {
    files: ReadonlyArray<I>;
}

export declare class RenameFeature extends Base<{
    oldUri: string;
    newUri: string;
}> {
    register(): void;
}

export interface RenameEvent extends Base<{
    oldUri: string;
    newUri: string;
}> {
    token: string;
}
