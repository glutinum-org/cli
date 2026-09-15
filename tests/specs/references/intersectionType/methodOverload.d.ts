interface ScreenshotOptions {
    base64?: boolean;
    save?: boolean;
}

export interface BrowserPage {
    screenshot(options: Omit<ScreenshotOptions, "save"> & { save: false }): string;
    screenshot(options: Omit<ScreenshotOptions, "base64"> & { base64: true }): number;
}
