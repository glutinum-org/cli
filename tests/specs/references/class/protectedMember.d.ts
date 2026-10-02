export declare class Service {
    protected baseUrl: string;
    protected static instances: number;
    protected get client(): string;
    protected request(path: string): Promise<string>;
    get(path: string): Promise<string>;
}
