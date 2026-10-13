export type Fn = (time: number) => number;
export type PowerEasing = (power?: number) => Fn;
export type BackEasing = (overshoot?: number) => Fn;
export type EasingWithParams = PowerEasing | BackEasing;
