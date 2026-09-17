declare var process: NodeJS.Process;

declare function queueMicrotask(callback: () => void): void;

declare namespace NodeJS {
    interface Process {
        cwd(): string;
    }
}
