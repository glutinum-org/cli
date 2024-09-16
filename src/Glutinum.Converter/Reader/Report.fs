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
        let sourceFile = node.getSourceFile ()
        let lineAndChar = sourceFile.getLineAndCharacterOfPosition node.pos
        let line = int lineAndChar.line + 1
        let column = int lineAndChar.character + 1

        let errorOrigin =
            let filePath =
                let index = filePath.IndexOf("src/Glutinum.Converter")
                "./" + filePath.Substring(index)

            $"%s{filePath}(%d{fileLine})".Replace("\\", "/")

        let typeFileName = sourceFile.fileName.Replace("\\", "/")

        $"""%s{errorOrigin}: Error while reading %s{errorContext} from:
%s{typeFileName}(%d{line},%d{column})

%s{reason}

--- Text ---
%s{node.getFullText ()}
---

--- Parent text ---
%s{node.parent.getFullText ()}
---"""
