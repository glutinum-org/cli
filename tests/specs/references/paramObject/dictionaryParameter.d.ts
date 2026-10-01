export interface Options {
    debug?: boolean;
}

export declare class Service {
    create(bodyParams?: { [key: string]: any } | FormData, options?: Options): Promise<string>;
    update(id: string, body: { [key: string]: any }): void;
    plain(options: Options): void;
}

export declare function send(payload: { [key: string]: unknown }): void;
