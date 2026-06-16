module Naming

open System

[<Literal>]
let MODULE_PLACEHOLDER = "REPLACE_ME_WITH_MODULE_NAME"

let private startWithDigit (name: string) : bool =
    name.Length > 0 && Char.IsDigit name.[0]

let private escapeName (name: string) : string =
    if
        name.Contains("-")
        || name.Contains("$")
        || name.Contains("/")
        || name.Contains("#")
        || name.Contains("<")
        || name.Contains(">")
        || name.Contains(" ")
        || name.Contains("~")
        || startWithDigit name
        || Keywords.fsharp.Contains name
    then
        $"``%s{name}``"
    else
        name

let removeSurroundingQuotes (text: string) : string =
    if String.IsNullOrEmpty text then
        ""
    elif text.Length < 1 then
        text
    else
        // only remove quotes when at start AND end
        let startChar, endChar = text.[0], text.[text.Length - 1]

        match startChar, endChar with
        | '"', '"'
        | ''', ''' -> text.Substring(1, text.Length - 2)
        | _ -> text

let private replaceDot (text: string) : string = text.Replace(".", "_")

let private replaceAt (text: string) : string = text.Replace("@", "_AT_")

let private replaceEmpty (text: string) : string =
    if String.IsNullOrWhiteSpace text then
        "_EMPTY_"
    else
        text

let private replacePlus (text: string) : string = text.Replace("+", "_PLUS_")

let private replaceDollar (text: string) : string = text.Replace("$", "_DOLLAR_")

let private replaceForwardSlash (text: string) : string = text.Replace("/", "_SLASH_")

type SanitizeNameResult = { Name: string; IsDifferent: bool }

let private sanitizeWith (extraReplace: string -> string) (name: string) : SanitizeNameResult =
    let sanitizedName =
        name
        |> replaceDot
        |> replaceAt
        |> replaceEmpty
        |> replacePlus
        |> extraReplace
        |> removeSurroundingQuotes

    // Check if the name is different after sanitization
    // This is used to check if the value is different from the default Fable computed value
    // Especially useful for StringEnum values where we sometimes need to use [<CompiledValue(...)>]
    // to provide a different value than the default Fable computed value from the name
    let isDifferent = name <> sanitizedName

    {
        Name = sanitizedName |> escapeName
        IsDifferent = isDifferent
    }

let sanitizeNameWithResult (name: string) : SanitizeNameResult = sanitizeWith id name

/// <summary>
/// Like <see cref="sanitizeNameWithResult"/> but also replaces characters that
/// are invalid in F# namespace, module, type or union case names (<c>$</c> and
/// <c>/</c>). Use this for type-level names and union/enum case names, not for
/// member names.
/// </summary>
let sanitizeTypeNameWithResult (name: string) : SanitizeNameResult =
    sanitizeWith (replaceDollar >> replaceForwardSlash) name

let sanitizeName (name: string) =
    let result = sanitizeNameWithResult name
    result.Name

/// <summary>
/// Like <see cref="sanitizeName"/> but also replaces characters that are
/// invalid in F# namespace, module, type or union case names (<c>$</c> and
/// <c>/</c>). Use this for type-level names and union/enum case names, not for
/// member names.
/// </summary>
let sanitizeTypeName (name: string) =
    let result = sanitizeTypeNameWithResult name
    result.Name
