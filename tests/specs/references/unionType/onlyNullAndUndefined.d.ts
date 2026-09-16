export interface Converter {
    asHover(hover: undefined | null): undefined;
    asHover(hover: string): string;
}
