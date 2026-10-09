declare function escapeHtml(str: string): string;
declare function isSpace(code: number): boolean;
declare namespace utils_d_exports {
    export { escapeHtml, isSpace };
}
export interface MarkdownIt {
    utils: typeof utils_d_exports;
    render(src: string): string;
}
