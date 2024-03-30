module Glutinum.Web.Global.Types

[<RequireQualifiedAccess>]
type CompilationResult =
    | Success of fsharpCode: string * warnings: string list
    | Error of string
