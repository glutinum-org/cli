[<AutoOpen>]
module Glutinum.Converter.Reader.Report

open TypeScript
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type Report =

    static member readerError
        (
            errorContext: string,
            reason: string,
            node: Ts.Node,
            [<CallerFilePath; Optional; DefaultParameterValue("")>] filePath:
                string,
            [<CallerLineNumber; Optional; DefaultParameterValue(0)>] fileLine:
                int
        )

        =

        let errorOrigin =
            let filePath =
                let index = filePath.IndexOf("src/Glutinum.Converter")
                "./" + filePath.Substring(index)

            $"%s{filePath}(%d{fileLine})".Replace("\\", "/")

        let sourceFile = node.getSourceFile ()

        if isNull sourceFile then
            $"""%s{errorOrigin}: Error while reading %s{errorContext} from:
(source file not available for report)

%s{reason}"""
        else
            let lineAndChar = sourceFile.getLineAndCharacterOfPosition node.pos
            let line = int lineAndChar.line + 1
            let column = int lineAndChar.character + 1
            let typeFileName = sourceFile.fileName.Replace("\\", "/")

            let parentText =
                if isNull node.parent then
                    ""
                else
                    $"""
--- Parent text ---
%s{node.parent.getFullText ()}
---"""

            $"""%s{errorOrigin}: Error while reading %s{errorContext} from:
%s{typeFileName}(%d{line},%d{column})

%s{reason}

--- Text ---
%s{node.getFullText ()}
---%s{parentText}"""
