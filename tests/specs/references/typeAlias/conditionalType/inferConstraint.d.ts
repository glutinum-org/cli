export interface Options<DateType extends Date = Date> {
    in?: DateType;
}

export interface Interval {
    start: Date | number;
    end: Date | number;
}

export type Result<IntervalType extends Interval, Opts extends Options | undefined> = Array<
    Opts extends Options<infer DateType>
        ? DateType
        : IntervalType["start"] extends Date
          ? IntervalType["start"]
          : Date
>;

export function eachDay<IntervalType extends Interval, Opts extends Options | undefined = undefined>(
    interval: IntervalType,
    options?: Opts,
): Result<IntervalType, Opts>;
