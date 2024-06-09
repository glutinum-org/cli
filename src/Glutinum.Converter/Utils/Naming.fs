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
// let nameEqualsDefaultFableValue (name: string) (value: string) : bool =
//     let defaultFableValue = String.lowercaseFirstLetter name

//     defaultFableValue.Equals value

// let nameNotEqualsDefaultFableValue (name: string) (value: string) : bool =
//     not (nameEqualsDefaultFableValue name value)

let private startWithDigit (name: string) : bool =
    name.Length > 0 && Char.IsDigit name.[0]

let private escapeName (name: string) : string =
    if
        name.Contains("-")
        || name.Contains("$")
        || name.Contains("#")
        || startWithDigit name
        || Keywords.fsharp.Contains name
    then
        $"``%s{name}``"
    else
        name

let mapTypeNameToFableCoreAwareName (name: string) : string =
    match name with
    | "Date" -> "JS.Date"
    | "Promise" -> "JS.Promise"
    | "Uint8Array" -> "JS.Uint8Array"
    | "ReadonlyArray" -> "ResizeArray"
    | "Array" -> "ResizeArray"
    | "Boolean" -> "bool"
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

let replaceDot (text: string) : string = text.Replace(".", "_")

type SanitizeNameResult = { Name: string; IsDifferent: bool }

let sanitizeNameWithResult (name: string) : SanitizeNameResult =
    let sanitizedName = name |> replaceDot |> removeSurroundingQuotes

    // Check if the name is different after sanitization
    // This is used to check if the value is different from the default Fable computed value
    // Especially useful for StringEnum values where we sometimes need to use [<CompiledValue(...)>]
    // to provide a different value than the default Fable computed value from the name
    let isDifferent = name <> sanitizedName

    {
        Name = sanitizedName |> escapeName
        IsDifferent = isDifferent
    }

let sanitizeName (name: string) =
    let result = sanitizeNameWithResult name
    result.Name
