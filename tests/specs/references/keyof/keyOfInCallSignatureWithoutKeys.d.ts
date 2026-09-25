export interface Empty {
}

interface Get {
    <Key extends keyof Empty>(key: Key): Empty[Key];
}

export interface Ctx {
    get: Get;
}
