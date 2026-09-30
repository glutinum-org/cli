/// `listener: (...args: any[]) => void`: overloads taking a lambda of one to three arguments,
/// the `System.Delegate` overload stays for the others
module Glutinum.Converter.Transformer.AnyFunctionOverloads

open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST
open Glutinum.Converter.Transformer.Utils
open Glutinum.Converter.Transformer.TypeParameters

let private MAX_ARITY = 3

let private isAnyFunction (typ: FSharpType) =
    match typ with
    | FSharpType.Function {
                              Parameters = [ {
                                                 Attributes = attributes
                                                 Type = FSharpType.Primitive FSharpPrimitive.Null
                                             } ]
                              ReturnType = FSharpType.Primitive FSharpPrimitive.Unit
                          } -> attributes |> List.contains FSharpAttribute.ParamArray
    | _ -> false

let private typeParameterNames (taken: Set<string>) (arity: int) =
    [ "A"; "B"; "C" ]
    |> List.take arity
    |> List.map (fun name ->
        if taken.Contains name then
            name + "1"
        else
            name
    )

let private expand (info: FSharpMemberInfo) : FSharpMemberInfo list =
    match
        info.Parameters
        |> List.tryFindIndex (fun parameter -> isAnyFunction parameter.Type)
    with
    | None -> []
    | Some index ->
        let taken =
            info.TypeParameters
            |> List.choose (
                function
                | FSharpTypeParameter.FSharpTypeParameter typeParameter -> Some typeParameter.Name
                | FSharpTypeParameter.FSharpType _ -> None
            )
            |> set

        [
            for arity in 1..MAX_ARITY do
                let names = typeParameterNames taken arity

                let lambda =
                    ({
                        Parameters =
                            names
                            |> List.map (fun name ->
                                {
                                    Attributes = []
                                    Name = name.ToLowerInvariant()
                                    IsOptional = false
                                    Type = FSharpType.TypeParameter name
                                    OriginalGlueMember = None
                                }
                            )
                        ReturnType = FSharpType.Primitive FSharpPrimitive.Unit
                    }
                    : FSharpFunctionType)
                    |> FSharpType.Function

                { info with
                    Parameters =
                        info.Parameters
                        |> List.mapi (fun i parameter ->
                            if i = index then
                                { parameter with Type = lambda }
                            else
                                parameter
                        )
                    TypeParameters = info.TypeParameters @ declaredTypeParameters names
                }
        ]

let expandMembers (members: FSharpMember list) : FSharpMember list =
    members
    |> List.collect (fun fsharpMember ->
        match fsharpMember with
        | FSharpMember.Method info ->
            [ yield! expand info |> List.map FSharpMember.Method; fsharpMember ]
        | _ -> [ fsharpMember ]
    )
