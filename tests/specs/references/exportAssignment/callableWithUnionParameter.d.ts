declare const tool: tool.Tool;

declare namespace tool {
    interface Tool {
        (args?: readonly string[] | string, cwd?: string): Tool;
        run(): void;
    }
}

export = tool;
