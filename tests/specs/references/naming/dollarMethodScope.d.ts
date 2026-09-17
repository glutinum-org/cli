export interface Page {
    $(selector: string, options?: { strict?: boolean }): Promise<string>;
}
