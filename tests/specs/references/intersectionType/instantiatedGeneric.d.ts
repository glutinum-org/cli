export interface Options {
    selector: string;
}

export interface RegistrationType<RO> {
    method: string;
    options: RO;
}

export interface DynamicFeature<RO> {
    registrationType: RegistrationType<RO>;
    register(options: RO): RO[];
    plain: number;
}

export interface SendFeature<T> {
    send: T;
}

export type TextDocumentFeature = DynamicFeature<Options> & SendFeature<(document: string) => Promise<void>>;

export interface UnionFeature<RO> {
    register(options: RO | undefined): RO[] | null;
}

export type UnionTextDocumentFeature = UnionFeature<Options> & { extra: string };
