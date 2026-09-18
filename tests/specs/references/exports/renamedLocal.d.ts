declare class WebMidi {
    enabled: boolean;
    enable(): Promise<WebMidi>;
}

declare const wm: WebMidi;

export { wm as WebMidi };
