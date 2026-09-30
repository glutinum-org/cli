[<RequireQualifiedAccess>]
module String

open System

let replace (oldValue: string) (newValue: string) (input: string) =
    input.Replace(oldValue, newValue)

let normalizePath (path: string) = path.Replace("\\", "/")

let removeSingleQuote (text: string) = text.Trim(''')

let removeDoubleQuote (text: string) = text.Trim('"')

let capitalizeFirstLetter (text: string) =
    (string text.[0]).ToUpper() + text.[1..]

/// `index.d.ts`, `index.d.mts` and `index.d.cts` without their extension
let withoutDeclarationExtension (fileName: string) =
    System.Text.RegularExpressions.Regex.Replace(fileName, "\\.d\\.[cm]?ts$", "")

let splitLines (text: string) =
    text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n') |> Array.toList
