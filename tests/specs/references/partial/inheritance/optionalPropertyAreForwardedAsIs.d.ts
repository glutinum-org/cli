export interface PointGroupOptions {
    dotSize?: number;
    count: number;
}

export interface Options extends Partial<PointGroupOptions> {
    minDistance?: number;
}
