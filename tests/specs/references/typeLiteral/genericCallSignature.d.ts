export interface Static {
    memoize: {
        <T>(func: T): T;
        Cache: string;
    };
}
