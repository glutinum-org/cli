module Naming

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

let escapeName (name: string) : string =
    if Keywords.fsharp.Contains name then
        $"``%s{name}``"
    else
        name

let mapDateToDateTime (name: string) : string =
    match name with
    | "Date" -> "DateTime"
    | name -> name
