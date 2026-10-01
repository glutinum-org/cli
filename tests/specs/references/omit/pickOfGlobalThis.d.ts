export type WindowLike = Pick<typeof globalThis, 'DocumentFragment' | 'Node'> & {
    trustedTypes?: string;
};

export declare function isSupported(window: WindowLike): boolean;
