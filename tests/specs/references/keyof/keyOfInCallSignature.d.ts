export interface ContextVariableMap {
    metric: string;
}

interface Get {
    <Key extends keyof ContextVariableMap>(key: Key): ContextVariableMap[Key];
}

export interface Ctx {
    get: Get;
}
