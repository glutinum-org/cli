export interface PointGroupOptions {
    size?: number | undefined;
    size2?: number | null;
    size3: number | null;
    size4: number | undefined;
}

export interface Options extends Partial<PointGroupOptions> {
    minDistance?: number;
}
