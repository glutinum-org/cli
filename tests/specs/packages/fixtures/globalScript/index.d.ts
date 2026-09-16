interface Doc {
    title: string;
}

declare var document: Doc;

declare function alert(message?: string): void;

declare namespace Intl2 {
    function format(value: number): string;
}
