module Glutinum.Converter.Reader.ConditionalTypeNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Utils
open Glutinum.Converter.Reader.Deferred

let readConditionalType (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let checker = reader.checker

    let conditionalTypeNode = typeNode :?> Ts.ConditionalTypeNode

    let typ = checker.getTypeAtLocation conditionalTypeNode

    // Deferred: resolved with the defaults of the enclosing declaration, else left to the transform
    if isDeferredConditional typ then
        // `JSHandle<T = any>`: `T extends Node ? ElementHandle<T> : null` is both branches
        let bothBranches () =
            let branch (node: Ts.TypeNode) =
                match reader.ReadTypeNode node with
                | GlueType.Literal GlueLiteral.Null -> GlueType.Primitive GluePrimitive.Null
                | glueType -> glueType

            let trueType = branch conditionalTypeNode.trueType
            let falseType = branch conditionalTypeNode.falseType

            if trueType = falseType then
                trueType
            else
                GlueType.Union(GlueTypeUnion [ trueType; falseType ])

        let resolved =
            if isAnyTypeParameter reader conditionalTypeNode.checkType then
                Some(bothBranches ())
            else
                tryResolveDeferred reader typeNode

        match resolved with
        | Some resolved -> resolved
        | None ->

            let warningsCount = reader.Warnings.Count

            let inferred =
                inferTypeNodes conditionalTypeNode.extendsType
                |> List.map (fun inferTypeNode ->
                    ({
                        Name = inferTypeNode.typeParameter.name.getText ()
                        Constraint = Some(inferredConstraint reader inferTypeNode)
                        Default = None
                    }
                    : GlueTypeParameter)
                )
                |> List.distinctBy _.Name

            let conditionalType =
                ({
                    CheckType = reader.ReadTypeNode conditionalTypeNode.checkType
                    ExtendsType = reader.ReadTypeNode conditionalTypeNode.extendsType
                    TrueType = reader.ReadTypeNode conditionalTypeNode.trueType
                    FalseType = reader.ReadTypeNode conditionalTypeNode.falseType
                    Inferred = inferred
                }
                : GlueConditionalType)

            // A branch with `infer` is not read, without a warning
            if reader.Warnings.Count > warningsCount then
                reader.Warnings.RemoveRange(warningsCount, reader.Warnings.Count - warningsCount)
                GlueType.Primitive GluePrimitive.Any
            else
                GlueType.ConditionalType conditionalType
    else

        // If we resolved the type to Any, we fallback to the generic type
        // This is because in F#, we can write
        // type ReturnType<'T> = obj
        // because 'T is not used in the type
        // This is perhaps a bit aggressive, so if needed we can re-visit `readTypeUsingFlags`
        // usage by inlining the logic here and make it more specific
        match typ.flags with
        | HasTypeFlags Ts.TypeFlags.TypeParameter -> GlueType.TypeParameter typ.symbol.name
        | HasTypeFlags Ts.TypeFlags.BooleanLiteral ->
            GlueType.Literal(GlueLiteral.Bool(typ?intrinsicName = "true"))
        | _ ->
            match readTypeUsingFlags reader typ with
            | GlueType.Primitive GluePrimitive.Any ->
                reader.ReadTypeNode conditionalTypeNode.checkType
            | forward -> forward
