declare function escapeHtml(str: string): string;
declare namespace utils_d_exports {
    export { escapeHtml };
}
declare class MarkdownIt {
    utils: typeof utils_d_exports;
    render(src: string): string;
}
export { MarkdownIt as default };
