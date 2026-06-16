/**
 * Represents a type which can release resources, such
 * as event listening or a timer.
 */
interface Disposable {
    dispose(): void;
}
