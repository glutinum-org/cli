export declare namespace Settings {
    const method: "settings/get";
    function get(key: string): string;
    interface Options {
        debug: boolean;
    }
}
