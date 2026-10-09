export interface IntervalTree {
    from: number;
}
export interface RangeSetPrototype {
    addKey(key: string): RangeSet;
}
export type RangeSet = RangeSetPrototype & IntervalTree;
export interface RangeSetConstructor {
    new (): RangeSet;
}
declare var RangeSet$1: RangeSetConstructor;
export { RangeSet$1 as RangeSet };
