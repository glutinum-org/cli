declare class Agent {
    constructor(name: string);
    dispatch(): void;
}

export default Agent;

export declare class BaseError {
    name: string;
    code: string;
}

export declare class TimeoutError extends BaseError {
    name: string;
    timeout: number;
}
