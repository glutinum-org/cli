module Glutinum.Converter.Reader.MemberTypes

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Utils

// Properties created by a mapped type (e.g. `{ [K in Keys]: string }`) have no declaration
type IntersectionTypePropertyResult =
    | Single of Ts.Symbol * Ts.Declaration
    | WithoutDeclaration of Ts.Symbol
    | ForceAny

let readPropertyWithoutDeclaration
    (reader: ITypeScriptReader)
    (contextNode: Ts.Node)
    (property: Ts.Symbol)
    : GlueMember option
    =
    let typ = reader.checker.getTypeOfSymbol property

    let flags = typeNodeBuilderFlags

    match reader.checker.typeToTypeNode (typ, None, Some flags) with
    | Some typeNode ->
        let previousContext = reader.SyntheticContext
        reader.SyntheticContext <- Some contextNode

        let propertyType =
            try
                reader.ReadTypeNode typeNode
            finally
                reader.SyntheticContext <- previousContext

        ({
            Name = property.name
            Documentation = []
            Type = propertyType
            IsOptional =
                match property.flags with
                | HasSymbolFlags Ts.SymbolFlags.Optional -> true
                | _ -> false
            IsStatic = false
            Accessor = GlueAccessor.ReadWrite
            IsPrivate = false
        }
        : GlueProperty)
        |> GlueMember.Property
        |> Some
    | None ->
        Report.readerError (
            "type node",
            $"Could not resolve the type of the property '%s{property.name}'",
            contextNode
        )
        |> reader.Warnings.Add

        None

// The declaration of a member of an instantiated generic type (e.g. `Foo<string>`)
// only refers to the type parameters, the checker knows the instantiated type
let readInstantiatedMember
    (reader: ITypeScriptReader)
    (contextNode: Ts.Node)
    (property: Ts.Symbol)
    (declaration: Ts.Declaration)
    : GlueMember
    =
    let checker = reader.checker
    let instantiatedType = checker.getTypeOfSymbol property

    let isInstantiated =
        not (isNull declaration?symbol)
        && not (obj.ReferenceEquals(instantiatedType, checker.getTypeOfSymbol declaration?symbol))

    let declaredMember = reader.ReadDeclaration declaration

    if not isInstantiated then
        declaredMember
    else
        let flags = typeNodeBuilderFlags

        match
            checker.typeToTypeNode (
                instantiatedType,
                enclosingDeclarationOf contextNode,
                Some flags
            )
        with
        | None -> declaredMember
        | Some typeNode ->
            let previousContext = reader.SyntheticContext
            reader.SyntheticContext <- Some contextNode

            let instantiatedMember =
                try
                    reader.ReadTypeNode typeNode
                finally
                    reader.SyntheticContext <- previousContext

            match declaredMember, instantiatedMember with
            | GlueMember.Property info, typ -> GlueMember.Property { info with Type = typ }
            | GlueMember.MethodSignature info, GlueType.FunctionType functionType ->
                GlueMember.MethodSignature
                    { info with
                        Parameters = functionType.Parameters
                        Type = functionType.Type
                    }
            | GlueMember.Method info, GlueType.FunctionType functionType ->
                GlueMember.Method
                    { info with
                        Parameters = functionType.Parameters
                        Type = functionType.Type
                    }
            | declaredMember, _ -> declaredMember
