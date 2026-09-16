export interface ChartOptions {
    title: string;
    axis?: AxisOptions;
}

export interface AxisOptions {
    min?: number;
}

export declare class Chart {
    constructor(options: ChartOptions);
}
