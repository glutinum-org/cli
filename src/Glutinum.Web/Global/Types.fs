module Glutinum.Web.Global.Types

[<RequireQualifiedAccess>]
type CompilationResult =
    | Success of
        fsharpCode: string *
        warnings: string list *
        errors: string list
    | Error of string
