module Glutinum.Converter.Reader.InterfaceDeclaration

open TypeScript
open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types

let readInterfaceDeclaration
    (reader: ITypeScriptReader)
    (declaration: Ts.InterfaceDeclaration)
    : GlueInterface
    =

    let members = declaration.members |> Seq.toList |> List.map reader.ReadDeclaration

    let typeParameters =
        let own = reader.ReadTypeParameters declaration.typeParameters

        // `interface AsyncIterator<T, TReturn, TNext>` merged with
        // `interface AsyncIterator<T, TReturn = any, TNext = any>` in another file
        if own |> List.forall (fun typeParameter -> typeParameter.Default.IsSome) then
            own
        else
            let othersDefaults =
                match reader.checker.getSymbolAtLocation declaration.name with
                | Some symbol ->
                    symbol.declarations
                    |> Option.map Seq.toList
                    |> Option.defaultValue []
                    |> List.filter (fun other ->
                        other.kind = Ts.SyntaxKind.InterfaceDeclaration
                        && not (obj.ReferenceEquals(other, declaration))
                    )
                    |> List.map (fun other ->
                        reader.ReadTypeParameters
                            (other :?> Ts.InterfaceDeclaration).typeParameters
                    )
                | None -> []

            own
            |> List.mapi (fun index typeParameter ->
                if typeParameter.Default.IsSome then
                    typeParameter
                else
                    { typeParameter with
                        Default =
                            othersDefaults
                            |> List.tryPick (fun others ->
                                others |> List.tryItem index |> Option.bind _.Default
                            )
                    }
            )

    {
        Documentation = reader.ReadDocumentationFromNode declaration
        FullName = Utils.getFullNameOrEmpty reader.checker declaration
        Name = declaration.name.getText ()
        Members = members
        TypeParameters = typeParameters
        HeritageClauses = Utils.readHeritageClauses reader declaration.heritageClauses
    }
