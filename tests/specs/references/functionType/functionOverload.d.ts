declare function withProgress<R>(task: (progress: any, data: any) => boolean): void;

declare function withProgress<R>(task: (progress: any, data: any) => string | Promise<R>): void;
