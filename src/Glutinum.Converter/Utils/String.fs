module String

open System

let replace (oldValue: string) (newValue: string) (input: string) =
    input.Replace(oldValue, newValue)

let removeSingleQuote (text: string) = text.Trim(''')

let removeDoubleQuote (text: string) = text.Trim('"')

let capitalizeFirstLetter (text: string) =
    (string text.[0]).ToUpper() + text.[1..]

let lowercaseFirstLetter (text: string) =
    (string text.[0]).ToLower() + text.[1..]

let splitLines (text: string) =
    text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n') |> Array.toList
