export interface User { name: string }

export declare function assertIsUser(value: unknown): asserts value is User;

export declare function assertDefined(value: unknown): asserts value;
