interface ScreenshotOptions {
    base64?: boolean;
    save?: boolean;
}

export interface BrowserPage {
    screenshot(options: Omit<ScreenshotOptions, "save">): string;
    screenshot(options: Omit<ScreenshotOptions, "base64">): number;
}
