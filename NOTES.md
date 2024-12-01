# NOTES

This file contains some notes or previous implementations that I used in the past, and that I want to keep for future reference.

Perhaps, they can be useful for implementing future features or fixing bugs.

## Find original type from a symbol

```fs
let typ = reader.checker.getTypeFromTypeNode typeReferenceNode

// Try find the original type
// For now, I am navigating inside of the symbol information
// to find a reference to the interface declaration via one of
// the members of the type
// Is there a better way of doing it?
match typ.aliasTypeArguments with
| None -> GlueType.Discard
| Some aliasTypeArguments ->
    if aliasTypeArguments.Count <> 1 then
        GlueType.Discard
    else
        let symbol = aliasTypeArguments.[0].symbol

        if isNull symbol || symbol.members.IsNone then
            GlueType.Unknown
        else

            // Take any of the members
            let (_, refMember) = symbol.members.Value.entries () |> Seq.head

            let originalType = refMember.declarations.Value[0].parent

            match originalType.kind with
            | Ts.SyntaxKind.InterfaceDeclaration ->
                let interfaceDeclaration = originalType :?> Ts.InterfaceDeclaration

                let members =
                    interfaceDeclaration.members
                    |> Seq.toList
                    |> List.map reader.ReadDeclaration

                ({
                    FullName = getFullNameOrEmpty reader.checker originalType
                    Name = interfaceDeclaration.name.getText ()
                    Members = members
                    TypeParameters = []
                    HeritageClauses = []
                }
                : GlueInterface)
                |> GlueUtilityType.Partial
                |> GlueType.UtilityType

            | _ -> GlueType.Discard
```
