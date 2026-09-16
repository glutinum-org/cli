export interface AxiosError<T = any> extends Error {
    config: T;
    code?: string;
}
