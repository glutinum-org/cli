module Naming

open System

/// <summary>
/// Check if the value is the same as the default Fable computed value computed from the name.
/// </summary>
/// <param name="name">Name to be transformed by Fable</param>
/// <param name="value">Value to compare to</param>
/// <returns>
/// <c>true</c> if provided value is the same as the default Fable computed value computed from the name.
/// <c>false</c> otherwise
/// </returns>
let nameEqualsDefaultFableValue (name: string) (value: string) : bool =
    let defaultFableValue = String.lowercaseFirstLetter name

    defaultFableValue.Equals value

let nameNotEqualsDefaultFableValue (name: string) (value: string) : bool =
    not (nameEqualsDefaultFableValue name value)

let startWithDigit (name: string) : bool =
    name.Length > 0 && Char.IsDigit name.[0]

let escapeName (name: string) : string =
    if
        name.Contains("-")
        || startWithDigit name
        || Keywords.fsharp.Contains name
    then
        $"``%s{name}``"
    else
        name

let mapTypeNameToFableCoreAwareName (name: string) : string =
    match name with
    | "Date" -> "DateTime"
    | "Promise" -> "JS.Promise"
    | "Uint8Array" -> "JS.Uint8Array"
    | name -> name

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

let sanitizeName (name: string) : string =
    name |> removeSurroundingQuotes |> escapeName
