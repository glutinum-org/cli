export interface Logger {
    /**
     * Emits a warning message.
     * @param message - The warning message to be logged
     */
    warning(message: string): void;
}
