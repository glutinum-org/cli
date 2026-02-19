export interface TelemetryLogger {
    logError(eventName: string, data?: Record<string, any>): void;
    logError(error: Error, data?: Record<string, any>): void;
}
