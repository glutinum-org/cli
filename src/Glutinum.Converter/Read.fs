module rec Glutinum.Converter.Read

open TypeScript

let readSourceFile
    (checker: Ts.TypeChecker)
    (sourceFile: option<Ts.SourceFile>)
    =
    let reader = Reader.TypeScriptReader.typeScriptReader checker

    {|
        GlueAST =
            sourceFile.Value.statements
            |> List.ofSeq
            |> List.map reader.ReadNode
        Warnings = reader.Warnings
    |}
