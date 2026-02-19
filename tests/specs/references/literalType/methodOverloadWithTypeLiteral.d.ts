export interface TelemetryLogger {
    logError(data?: {
        readonly encoding?: string;
    }): void;
    logError(error: Error, data?: {
        readonly permissions?: string[];
    }): void;
}
