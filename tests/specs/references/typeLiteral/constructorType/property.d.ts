export interface Instance {
    level: number;
}

export interface Registry {
    instanceType: new () => Instance;
}
