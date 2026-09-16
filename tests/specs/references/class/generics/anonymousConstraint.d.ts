export interface Options {
    selector: string;
}

export interface NotifyingFeature<P extends { textDocument: string }> {
    onNotificationSent: P;
}

export declare class TextDocumentLanguageFeature<PO, RO extends Options & PO, CO = object> implements NotifyingFeature<RO> {
    onNotificationSent: RO;
    getOptions(): RO;
}

export interface SendFeature<T extends Function> {
    send: T;
}

export type CM<C extends string | undefined, S extends string | undefined> = {
    client: C;
    server: S;
};
