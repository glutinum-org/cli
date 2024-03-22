interface Locale {
    hello(config: {
        prefix: string | undefined
        suffix?: string
        verbose: Boolean
    }): string;
}
