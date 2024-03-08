module Glutinum.Web.Global.Types

[<RequireQualifiedAccess>]
type CompilationResult =
    | Success of fsharpCode: string * warnings: string list
    | TypeScriptReaderException of string
    | Error of string
