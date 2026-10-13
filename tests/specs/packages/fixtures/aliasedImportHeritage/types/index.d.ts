import { StreamParser as StreamParser$1 } from 'stream-base';

interface StreamParser<State> extends StreamParser$1<State> {
    tokenTable?: { [name: string]: string };
}

declare class StreamLanguage<State> {
    static define<State>(spec: StreamParser<State>): StreamLanguage<State>;
}

export { StreamLanguage, type StreamParser };
