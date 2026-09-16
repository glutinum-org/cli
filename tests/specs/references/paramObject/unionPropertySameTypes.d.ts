export interface PresentationOptions {
    title: string;
}

export interface GetSessionOptions {
    createIfNone?: boolean | PresentationOptions;
    forceNewSession?: boolean | PresentationOptions;
}

export declare function getSession(options: GetSessionOptions): void;
