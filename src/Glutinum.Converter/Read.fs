module rec Glutinum.Converter.Read

open TypeScript
open Glutinum.Converter.Reader.Types
open Glutinum.Converter.Reader.TypeScriptReader

let readSourceFile
    (checker: Ts.TypeChecker)
    (sourceFile: option<Ts.SourceFile>)
    =
    let reader: ITypeScriptReader = TypeScriptReader(checker)

    {|
        GlueAST =
            sourceFile.Value.statements
            |> List.ofSeq
            |> List.map reader.ReadNode
        Warnings = reader.Warnings
    |}
