export interface HighlightResult {
    value: string;
    secondBest?: Omit<HighlightResult, 'second_best'>;
}
