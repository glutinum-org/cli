export interface Position {
    line: number;
    character: number;
}

export interface Range {
    start: Position;
    end: Position;
}
