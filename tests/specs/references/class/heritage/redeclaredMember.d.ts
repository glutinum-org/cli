export interface CommonOptions {
    headers?: Record<string, string>;
}

export interface RecordOptions extends CommonOptions {
    expand?: string;
}

export declare abstract class BaseService {
    readonly client: string;
}

export declare abstract class CrudService<M> extends BaseService {
    decode<T = M>(data: string): T;
    getOne<T = M>(id: string, options?: CommonOptions): Promise<T>;
    delete(id: string, options?: CommonOptions): Promise<boolean>;
}

export declare class RecordService<M = string> extends CrudService<M> {
    readonly collectionIdOrName: string;
    getOne<T = M>(id: string, options?: RecordOptions): Promise<T>;
    delete(id: string, options?: CommonOptions): Promise<boolean>;
}

export declare class LogService extends CrudService<string> {
    delete(id: string, options?: CommonOptions): Promise<boolean>;
}
