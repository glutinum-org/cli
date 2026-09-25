export declare class Marked<ParserOutput = string, RendererOutput = string> {
    Parser: {
        new (options?: ParserOutput): RendererOutput;
        parse<ParserOutput_1 = string>(options?: ParserOutput_1): ParserOutput_1;
    };
}
