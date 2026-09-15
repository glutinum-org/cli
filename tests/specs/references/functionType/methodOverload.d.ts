export interface Progress {
    withProgress(task: (progress: any, data: any) => boolean): void;
    withProgress(task: (progress: any, data: any) => string): void;
}
