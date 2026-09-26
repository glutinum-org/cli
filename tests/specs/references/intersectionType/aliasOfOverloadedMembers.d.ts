export interface Instance {
    listen(port: number): void
    listen(): void
}

export interface Brand {
    kind: string
}

export type Branded = Instance & Brand
