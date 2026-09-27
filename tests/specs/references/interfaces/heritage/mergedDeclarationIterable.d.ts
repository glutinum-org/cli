interface Element {
    tagName: string;
}

interface HTMLCollectionBase {
    length: number;
}

interface HTMLCollectionOf<T extends Element> extends HTMLCollectionBase {
    item(index: number): T | null;
}

interface HTMLCollectionBase {
    [Symbol.iterator](): ArrayIterator<Element>;
}

interface HTMLCollectionOf<T extends Element> {
    [Symbol.iterator](): ArrayIterator<T>;
}
