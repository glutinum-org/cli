module Glutinum.Converter.Reader.ExpressionWithTypeArguments

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Utils

let readExpressionWithTypeArguments (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let checker = reader.checker

    let expression = typeNode :?> Ts.ExpressionWithTypeArguments

    let typ = checker.getTypeFromTypeNode expression

    // Getting the type from the expression seems more robust for getting a Symbol resolved
    // than using:
    //
    // let symbolOpt = checker.getSymbolAtLocation (expression.expression)
    let symbolOpt =
        // Alias symbol give us better result for utility types like Omit, Partial, etc...
        typ.aliasSymbol
        // If not available, we fallback to the symbol of the type
        |> Option.orElse (
            if isNull (box typ.symbol) then
                None
            else
                Some typ.symbol
        )
        // An erroneous type (`Uint8Array<T>` with an older lib) has no symbol, its name has
        |> Option.orElse (checker.getSymbolAtLocation expression.expression)

    // Specialize the utility types we know how to resolve so they work in
    // heritage clauses too (e.g. `interface Y extends Omit<X, "a">`). The
    // node is structurally compatible with a `TypeReferenceNode` for the
    // properties the reader needs (`typeArguments`).
    let isFromEs5 = isFromEs5Lib symbolOpt

    match isFromEs5, getFullNameOrEmpty checker expression.expression with
    | true, "Omit" -> UtilityType.readOmit reader (unbox<Ts.TypeReferenceNode> expression)
    | true, "Pick" -> UtilityType.readPick reader (unbox<Ts.TypeReferenceNode> expression)
    | _ ->
        let isQualified =
            expression.expression.kind = Ts.SyntaxKind.PropertyAccessExpression

        // The module path is computed from the resolved symbol, so the name must be its name too
        let name =
            match symbolOpt with
            // `export default class DatasetController` is the `default` symbol
            | Some symbol when symbol.name = "default" ->
                declaredName symbol |> Option.defaultValue symbol.name
            | Some symbol when not isFromEs5 -> symbol.name
            | _ ->
                if isQualified then
                    (unbox<Ts.PropertyAccessExpression> expression.expression).name?text
                else
                    expression.expression.getText ()

        let isExternal = isExternalToPackages checker reader.PackageContext symbolOpt

        // `extends ReturnType<...>` resolves to an anonymous type, it has no declaration to inherit
        let isAnonymous =
            match symbolOpt with
            | Some symbol -> symbol.name = "__type" || symbol.name = "__object"
            | None -> false

        // An external base type can't be inherited, `inherit obj` is invalid, `Partial` is
        // expanded by the transform
        if
            isAnonymous
            || (isExternal
                && not (knownExternalTypeNames.Contains name)
                && not (isFromEs5 && name = "Partial"))
        then
            GlueType.Discard
        else
            ({
                Name =
                    if name.Contains "." then
                        name
                    else
                        Naming.sanitizeTypeName name
                // The name of the declaration, not of an import alias
                FullName =
                    match symbolOpt |> Option.bind (resolveValueAlias checker) with
                    | Some symbol -> checker.getFullyQualifiedName symbol
                    | None -> getFullNameOrEmpty checker expression.expression
                ModulePath =
                    if isLibraryName reader name then
                        []
                    else
                        modulePathForSymbol
                            checker
                            reader.PackageContext
                            (isQualified
                             || crossesNamespace
                                 checker
                                 reader.PackageContext
                                 symbolOpt
                                 (landingOf reader expression))
                            symbolOpt
                TypeArguments =
                    match readTypeArguments reader expression with
                    | [] ->
                        // The alias instantiation holds the arguments `extends CoreApp` leaves implicit
                        let isThroughAnotherAlias =
                            match
                                checker.getSymbolAtLocation expression.expression, typ.aliasSymbol
                            with
                            | Some written, Some alias -> not (obj.ReferenceEquals(written, alias))
                            | _ -> false

                        let ofAlias =
                            if isThroughAnotherAlias then
                                typ.aliasTypeArguments
                                |> Option.map Seq.toList
                                |> Option.defaultValue []
                            else
                                []

                        let arguments =
                            if ofAlias.IsEmpty then
                                resolvedBaseTypeArguments checker expression
                            else
                                ofAlias

                        arguments
                        |> List.map (fun argument ->
                            let flags = typeNodeBuilderFlags

                            checker.typeToTypeNode (argument, None, Some flags)
                            |> reader.ReadTypeNode
                        )
                    | typeArguments -> typeArguments
                IsStandardLibrary = isFromEs5 || isExternal || isLibraryName reader name
            })
            |> GlueType.TypeReference
