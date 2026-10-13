export interface Range {
    from: number;
}

export interface RangeSetUpdate {
    add?: readonly Range[];
    filterFrom?: number;
}

export declare class RangeSet {
    update(updateSpec: RangeSetUpdate): RangeSet;
    readonly ranges: readonly Range[];
}
