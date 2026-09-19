module Glutinum.Converter.Reader.Utils

open TypeScript
open Glutinum.Converter.GlueAST
open Fable.Core.JS
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Types

let (|HasTypeFlags|_|) (flag: Ts.TypeFlags) (flags: Ts.TypeFlags) =
    if int flags &&& int flag <> 0 then
        Some()
    else
        None

let (|HasSymbolFlags|_|) (flag: Ts.SymbolFlags) (flags: Ts.SymbolFlags) =
    if int flags &&& int flag <> 0 then
        Some()
    else
        None

let (|HasObjectFlags|_|) (flag: Ts.ObjectFlags) (flags: Ts.ObjectFlags) =
    if int flags &&& int flag <> 0 then
        Some()
    else
        None

let private isNumericString (text: string) =
    jsTypeof text = "string" && unbox text |> Constructors.Number.isNaN |> not

let private tryReadNumericLiteral (text: string) =
    if isNumericString text then
        // First, try to parse as an integer
        match System.Int32.TryParse text with
        | (true, i) -> GlueLiteral.Int i |> Some
        | _ ->
            // If it fails, try to parse as a float
            match System.Double.TryParse text with
            | (true, f) -> GlueLiteral.Float f |> Some
            | _ -> None
    else
        None

/// <summary>
/// Determine if a numeric literal source text denotes a float (i.e. has a
/// decimal point or a decimal exponent), as opposed to an integer.
///
/// Radix literals (<c>0x</c>, <c>0b</c>, <c>0o</c>) are always integers, so we
/// don't mistake the <c>e</c> in <c>0xE</c> for an exponent.
/// </summary>
let private isWrittenAsFloat (text: string) =
    let lower = text.ToLower()

    if lower.StartsWith "0x" || lower.StartsWith "0b" || lower.StartsWith "0o" then
        false
    else
        text.Contains "." || lower.Contains "e"

let tryReadLiteral (checker: Ts.TypeChecker) (expression: Ts.Node) =
    match expression.kind with
    | Ts.SyntaxKind.StringLiteral
    | Ts.SyntaxKind.NoSubstitutionTemplateLiteral ->
        let literal = (expression :?> Ts.StringLiteral)

        GlueLiteral.String literal.text |> Some
    | Ts.SyntaxKind.TrueKeyword -> GlueLiteral.Bool true |> Some
    | Ts.SyntaxKind.FalseKeyword -> GlueLiteral.Bool false |> Some
    | Ts.SyntaxKind.NullKeyword -> GlueLiteral.Null |> Some
    // A node synthesized by `typeToTypeNode` has no source text nor location for the checker
    | _ when expression.pos < 0 ->
        match expression?text with
        | null -> None
        | text -> tryReadNumericLiteral text
    | _ ->
        let text = expression.getText ()

        // Source the numeric value from the type checker (robust against
        // numeric separators, hex/binary/octal literals, large values, ...)
        // but decide int vs float from the source text so that `10.0` and
        // `1e3` are kept as floats even though TypeScript normalises them to
        // the integer-valued literal type `10` / `1000`.
        let resolvedType = checker.getTypeAtLocation expression

        match resolvedType.flags with
        | HasTypeFlags Ts.TypeFlags.NumberLiteral ->
            let value = unbox<float> (resolvedType :?> Ts.LiteralType).value

            if Constructors.Number.isSafeInteger value && not (isWrittenAsFloat text) then
                GlueLiteral.Int(unbox<int> value) |> Some
            else
                GlueLiteral.Float value |> Some
        | _ ->
            // Fallback to parsing the source text directly
            tryReadNumericLiteral text

/// Nodes synthesized by <c>typeToTypeNode</c> have no source text
let identifierText (node: Ts.Node) : string =
    if isNull node?text then
        node.getText ()
    else
        node?text

/// The last name of `ns.Foo`, `Foo` for an identifier
let entityNameText (node: Ts.Node) : string =
    if node.kind = Ts.SyntaxKind.QualifiedName then
        identifierText (node :?> Ts.QualifiedName).right
    else
        identifierText node

/// Identifiers synthesized by <c>typeToTypeNode</c> carry their symbol, they can't be resolved by position
let symbolAtLocation (checker: Ts.TypeChecker) (node: Ts.Node) : Ts.Symbol option =
    if not (isNull node?symbol) then
        Some node?symbol
    elif
        node.kind = Ts.SyntaxKind.QualifiedName
        && not (isNull (node :?> Ts.QualifiedName).right?symbol)
    then
        Some (node :?> Ts.QualifiedName).right?symbol
    // `SyntaxKind.ImportKeyword` synthesized for an enum literal only carries the enum
    elif
        node.kind = Ts.SyntaxKind.QualifiedName
        && not (isNull (node :?> Ts.QualifiedName).left?symbol)
        && (unbox<Ts.Symbol> (node :?> Ts.QualifiedName).left?symbol).flags
           &&& Ts.SymbolFlags.Enum
           <> enum 0
    then
        Some (node :?> Ts.QualifiedName).left?symbol
    else
        checker.getSymbolAtLocation node

let tryGetFullName (checker: Ts.TypeChecker) (node: Ts.Node) =
    // Naive way to check if the node has a symbol
    // The others solutions is to redo a pattern matching on the node.type
    // but it's more complex (can be changed if this solution is too permissive)
    if isNull node?symbol then
        match checker.getSymbolAtLocation node with
        | None -> None
        | Some symbol -> checker.getFullyQualifiedName symbol |> Some
    else
        checker.getFullyQualifiedName node?symbol |> Some

let getFullNameOrEmpty (checker: Ts.TypeChecker) (node: Ts.Node) =
    tryGetFullName checker node |> Option.defaultValue ""

type ModifierUtil =

    static member GetAccessor(modifiers: ResizeArray<Ts.Modifier> option) =
        match modifiers with
        | Some modifiers ->
            modifiers
            |> Seq.exists (fun modifier -> modifier.kind = Ts.SyntaxKind.ReadonlyKeyword)
            |> function
                | true -> GlueAccessor.ReadOnly
                | false -> GlueAccessor.ReadWrite
        | None -> GlueAccessor.ReadWrite

    static member GetAccessor(modifiers: ResizeArray<Ts.ModifierLike> option) =
        ModifierUtil.GetAccessor(unbox<ResizeArray<Ts.Modifier> option> modifiers)

    static member HasModifier(modifiers: ResizeArray<Ts.Modifier> option, modifier: Ts.SyntaxKind) =
        match modifiers with
        | Some modifiers ->
            modifiers |> Seq.exists (fun currentModifier -> currentModifier.kind = modifier)
        | None -> false

    static member HasModifier
        (modifiers: option<ResizeArray<Ts.ModifierLike>>, modifier: Ts.SyntaxKind)
        =
        ModifierUtil.HasModifier(unbox<ResizeArray<Ts.Modifier> option> modifiers, modifier)

/// `K extends keyof Map` where `Map` is a reference, the map gives the typed keys of `K`
let tryReadKeyOfConstraint
    (reader: ITypeScriptReader)
    (typeParameter: Ts.TypeParameterDeclaration)
    =
    match typeParameter.``constraint`` with
    | Some constraintNode when constraintNode.kind = Ts.SyntaxKind.TypeOperator ->
        let typeOperator = constraintNode :?> Ts.TypeOperatorNode

        if
            typeOperator.operator = Ts.SyntaxKind.KeyOfKeyword
            && typeOperator.``type``.kind = Ts.SyntaxKind.TypeReference
        then
            let typeReferenceNode = typeOperator.``type`` :?> Ts.TypeReferenceNode

            // An external binding has a `Key` type for the interfaces of its maps only
            let isExternalAlias =
                symbolAtLocation reader.checker !!typeReferenceNode.typeName
                |> Option.map (fun (symbol: Ts.Symbol) ->
                    match symbol.flags with
                    | HasSymbolFlags Ts.SymbolFlags.Alias -> reader.checker.getAliasedSymbol symbol
                    | _ -> symbol
                )
                |> Option.bind (fun (symbol: Ts.Symbol) -> symbol.declarations)
                |> Option.map (fun declarations ->
                    let isExternal =
                        match reader.PackageContext, Seq.tryHead declarations with
                        | Some packageContext, Some(declaration: Ts.Declaration) ->
                            (packageContext.TryFindPackage(declaration.getSourceFile().fileName))
                                .IsNone
                        | _ -> false

                    isExternal
                    && not (
                        declarations
                        |> Seq.exists (fun (declaration: Ts.Declaration) ->
                            match declaration.kind with
                            | Ts.SyntaxKind.InterfaceDeclaration
                            | Ts.SyntaxKind.ClassDeclaration -> true
                            | _ -> false
                        )
                    )
                )
                |> Option.defaultValue false

            if isExternalAlias then
                None
            else
                match reader.ReadTypeNode typeOperator.``type`` with
                | GlueType.TypeReference _ as map -> Some(GlueType.KeyOf map)
                | _ -> None
        else
            None
    | _ -> None

/// The type parameters of a member: only a `keyof` constraint is kept
let readMemberTypeParameters
    (reader: ITypeScriptReader)
    (typeParameters: ResizeArray<Ts.TypeParameterDeclaration> option)
    : GlueTypeParameter list
    =
    match typeParameters with
    | None -> []
    | Some typeParameters ->
        typeParameters
        |> Seq.toList
        |> List.map (fun typeParameter ->
            {
                Name = identifierText typeParameter.name
                Constraint = tryReadKeyOfConstraint reader typeParameter
                Default = typeParameter.``default`` |> Option.map reader.ReadTypeNode
            }
        )

/// `[string, number]` and the tuple `Parameters<F>` resolves to
let isTupleType (typ: Ts.Type) =
    match typ.flags with
    | HasTypeFlags Ts.TypeFlags.Object ->
        let objectFlags: Ts.ObjectFlags = (typ :?> Ts.ObjectType).objectFlags
        let target: Ts.ObjectType = typ?target

        int objectFlags &&& int Ts.ObjectFlags.Tuple <> 0
        || (not (isNull (box target))
            && int target.objectFlags &&& int Ts.ObjectFlags.Tuple <> 0)
    | _ -> false

let readTypeArguments (reader: ITypeScriptReader) (node: Ts.NodeWithTypeArguments) =
    match node.typeArguments with
    | None -> []
    | Some typeArguments -> typeArguments |> Seq.toList |> List.map (Some >> reader.ReadTypeNode)

/// The type arguments the checker resolved for a type alias or an object type reference
let resolvedTypeArguments (checker: Ts.TypeChecker) (typ: Ts.Type) : Ts.Type list =
    let aliasTypeArguments: ResizeArray<Ts.Type> option = typ?aliasTypeArguments

    match typ.aliasSymbol, aliasTypeArguments with
    | Some _, Some aliasTypeArguments -> aliasTypeArguments |> Seq.toList
    | _ ->
        match typ.flags with
        | HasTypeFlags Ts.TypeFlags.Object when
            int (typ :?> Ts.ObjectType).objectFlags &&& int Ts.ObjectFlags.Reference <> 0
            ->
            checker.getTypeArguments (typ :?> Ts.TypeReference) |> Seq.toList
        | _ -> []

/// `class X extends TempNode` where `const TempNode: { new<T = unknown>(): TempNode<T> }`:
/// the type arguments of the base type the checker instantiated
let resolvedBaseTypeArguments
    (checker: Ts.TypeChecker)
    (expression: Ts.ExpressionWithTypeArguments)
    : Ts.Type list
    =
    let heritageClause: Ts.Node = expression.parent

    let isValue =
        match checker.getSymbolAtLocation expression.expression with
        | Some symbol ->
            // `import TempNode from "./TempNode.js"` is an alias of the constant
            let symbol =
                match symbol.flags with
                | HasSymbolFlags Ts.SymbolFlags.Alias -> checker.getAliasedSymbol symbol
                | _ -> symbol

            match symbol.flags with
            | HasSymbolFlags Ts.SymbolFlags.Variable -> true
            | _ -> false
        | None -> false

    if
        isNull heritageClause
        || heritageClause.kind <> Ts.SyntaxKind.HeritageClause
        || heritageClause?token <> Ts.SyntaxKind.ExtendsKeyword
        || heritageClause.parent.kind <> Ts.SyntaxKind.ClassDeclaration
        || not isValue
    then
        []
    else
        let classType = checker.getTypeAtLocation heritageClause.parent :?> Ts.InterfaceType

        checker.getBaseTypes classType
        |> Seq.tryHead
        |> Option.map (fun baseType -> resolvedTypeArguments checker (unbox<Ts.Type> baseType))
        |> Option.defaultValue []

let readHeritageClauses
    (reader: ITypeScriptReader)
    (heritageClauses: ResizeArray<Ts.HeritageClause> option)
    =
    match heritageClauses with
    | Some heritageClauses ->
        heritageClauses
        |> Seq.toList
        |> List.collect (fun clause -> clause.types |> Seq.toList |> List.map reader.ReadTypeNode)
    | None -> []

/// <summary>
/// Determine if the type is from the ES5 library
///
/// This is to detect native utility types usage
/// </summary>
/// <param name="symbolOpt"></param>
/// <returns>
/// <c>True</c> if the type is from the ES5 library otherwise <c>False</c>
/// </returns>
let isFromEs5Lib (symbolOpt: Ts.Symbol option) =
    match symbolOpt with
    | None -> false
    | Some symbol ->
        match symbol.declarations with
        | None ->
            // For some reason, I can't seem to resolve the actual symbol for some Es5 types
            // So, we make a naive fallback checking the name of the symbol
            [ "Iterable"; "IterableIterator" ] |> List.contains symbol.name
        | Some declarations ->
            // A declaration synthesized by the checker has no parent
            if declarations.Count = 0 || isNull declarations[0].parent then
                false
            else
                match declarations[0].parent.kind with
                | Ts.SyntaxKind.SourceFile ->
                    let sourceFile = declarations[0].parent :?> Ts.SourceFile

                    sourceFile.fileName.EndsWith("lib/lib.es5.d.ts")
                | _ -> false

/// `PromiseConstructor` of `lib.es2015.promise.d.ts`: a type of the ECMAScript libraries
let isFromEsLib (symbolOpt: Ts.Symbol option) =
    match symbolOpt with
    | None -> false
    | Some symbol ->
        match symbol.declarations with
        | Some declarations when declarations.Count > 0 ->
            let fileName = String.normalizePath (declarations[0].getSourceFile().fileName)
            fileName.Contains "/lib/lib.es"
        | _ -> false

/// Library types the converter maps to an existing F# type, kept even when declared outside the packages
let knownExternalTypeNames =
    set
        [
            "Date"
            "Promise"
            "Array"
            "ReadonlyArray"
            "Boolean"
            "Function"
            "Error"
            "RegExp"
            "Iterable"
            "IterableIterator"
            "IteratorObject"
            "ArrayIterator"
            "MapIterator"
            "SetIterator"
            "StringIterator"
            "Uint8Array"
            "Int8Array"
            "Uint8ClampedArray"
            "Int16Array"
            "Uint16Array"
            "Int32Array"
            "Uint32Array"
            "Float32Array"
            "Float64Array"
        ]

let resolveAlias (checker: Ts.TypeChecker) (symbol: Ts.Symbol) =
    match symbol.flags with
    | HasSymbolFlags Ts.SymbolFlags.Alias ->
        // `getAliasedSymbol` throws when the alias can't be resolved
        try
            let aliased = checker.getAliasedSymbol symbol

            if isNull (box aliased) then
                None
            else
                Some aliased
        with _ ->
            None
    | _ -> Some symbol

/// Name of the declaration behind an `import { X as Y }` alias, `None` for a non-renamed symbol
/// `Omit<LabelOption, "rotate">` where `interface LabelOption<T = Params>`: `T` is `Params`
let defaultTypeArguments
    (reader: ITypeScriptReader)
    (typeNode: Ts.TypeNode)
    : Collections.Map<string, GlueType>
    =
    if typeNode.kind <> Ts.SyntaxKind.TypeReference then
        Collections.Map.empty
    else
        let typeReferenceNode = typeNode :?> Ts.TypeReferenceNode

        let typeArguments =
            match typeReferenceNode.typeArguments with
            | Some typeArguments -> typeArguments |> Seq.toList
            | None -> []

        symbolAtLocation reader.checker !!typeReferenceNode.typeName
        |> Option.bind (resolveAlias reader.checker)
        |> Option.bind (fun symbol -> symbol.declarations)
        |> Option.bind Seq.tryHead
        |> Option.bind (fun declaration ->
            let typeParameters: ResizeArray<Ts.TypeParameterDeclaration> option =
                declaration?typeParameters

            typeParameters
        )
        |> Option.map (fun typeParameters ->
            typeParameters
            |> Seq.toList
            |> List.mapi (fun index typeParameter ->
                let argument =
                    typeArguments
                    |> List.tryItem index
                    |> Option.map (Some >> reader.ReadTypeNode)
                    |> Option.orElse (typeParameter.``default`` |> Option.map reader.ReadTypeNode)

                identifierText typeParameter.name, argument
            )
            |> List.choose (fun (name, argument) ->
                argument |> Option.map (fun argument -> name, argument)
            )
            |> Collections.Map.ofList
        )
        |> Option.defaultValue Collections.Map.empty

/// `export default class DatasetController`: the name of the declaration behind the `default` symbol
let declaredName (symbol: Ts.Symbol) =
    symbol.declarations
    |> Option.bind Seq.tryHead
    |> Option.bind (fun declaration ->
        match declaration.kind with
        | Ts.SyntaxKind.ExportAssignment ->
            let expression: Ts.Node = (declaration :?> Ts.ExportAssignment).expression

            if expression.kind = Ts.SyntaxKind.Identifier then
                Some(identifierText expression)
            else
                None
        | _ ->
            let name: Ts.Node = declaration?name

            if isNull name then
                None
            else
                Some(identifierText name)
    )

let importedName (checker: Ts.TypeChecker) (symbol: Ts.Symbol) =
    match symbol.flags with
    | HasSymbolFlags Ts.SymbolFlags.Alias ->
        match resolveAlias checker symbol with
        | Some target when target.name = "default" -> declaredName target
        | Some target when target.name <> symbol.name && target.name <> "export=" ->
            Some target.name
        | _ -> None
    | _ when symbol.name = "default" -> declaredName symbol
    | _ -> None

let private fileOfDeclaration (declaration: Ts.Node) =
    declaration.getSourceFile().fileName |> String.normalizePath

let private isTypeDeclaration (declaration: Ts.Node) =
    match declaration.kind with
    | Ts.SyntaxKind.InterfaceDeclaration
    | Ts.SyntaxKind.ClassDeclaration
    | Ts.SyntaxKind.TypeAliasDeclaration
    | Ts.SyntaxKind.EnumDeclaration
    | Ts.SyntaxKind.ModuleDeclaration -> true
    | _ -> false

/// <summary>
/// The declaration the symbol is read from and referenced by. The first one, unless it belongs
/// to an external binding while another one belongs to a generated package (<c>AbortSignal</c>
/// of the DOM lib redeclared by <c>@types/node</c>), or the entry file of its package declares
/// it too (<c>NodeList</c> of <c>iterable.d.ts</c> and <c>index.d.ts</c>).
/// </summary>
let mainDeclaration (packageContext: PackageContext option) (symbol: Ts.Symbol) =
    match symbol.declarations with
    | Some declarations when declarations.Count > 0 ->
        let declarations = Seq.toList declarations
        let first = declarations.Head

        match packageContext with
        | None -> Some first
        | Some packageContext ->
            // `var TextDecoder` of a package doesn't stand for the `interface TextDecoder` of the DOM lib
            let generated =
                declarations
                |> List.filter (fun declaration ->
                    (packageContext.TryFindPackage(fileOfDeclaration declaration)).IsSome
                    && isTypeDeclaration declaration = isTypeDeclaration first
                )

            let candidates =
                if
                    (packageContext.TryFindExternalModulePath(fileOfDeclaration first)).IsSome
                    && not generated.IsEmpty
                then
                    generated
                else
                    declarations

            let first = candidates.Head

            match packageContext.TryFindPackage(fileOfDeclaration first) with
            | Some package ->
                candidates
                |> List.tryFind (fun declaration ->
                    declaration.kind = first.kind
                    && fileOfDeclaration declaration = package.EntryFile
                )
                |> Option.defaultValue first
                |> Some
            | None -> Some first
    | _ -> None

/// <summary>
/// In package mode, the interface declarations of the symbol in other files of the same package:
/// they are merged into the main declaration
/// </summary>
let otherFileInterfaceDeclarations
    (packageContext: PackageContext option)
    (symbol: Ts.Symbol)
    (declaration: Ts.Node)
    : Ts.InterfaceDeclaration list
    =
    match packageContext, symbol.declarations with
    | Some packageContext, Some declarations ->
        match packageContext.TryFindPackage(fileOfDeclaration declaration) with
        | Some package ->
            declarations
            |> Seq.toList
            |> List.filter (fun other ->
                other.kind = Ts.SyntaxKind.InterfaceDeclaration
                && fileOfDeclaration other <> fileOfDeclaration declaration
                && (
                    match packageContext.TryFindPackage(fileOfDeclaration other) with
                    | Some otherPackage -> otherPackage.Dir = package.Dir
                    | None -> false
                )
            )
            |> List.map (fun other -> other :?> Ts.InterfaceDeclaration)
        | None -> []
    | _ -> []

/// An interface declaration merged into the main declaration of another file
let isMergedInterfaceDeclaration
    (checker: Ts.TypeChecker)
    (packageContext: PackageContext option)
    (statement: Ts.Node)
    =
    match packageContext, statement.kind with
    | Some _, Ts.SyntaxKind.InterfaceDeclaration ->
        let declaration = statement :?> Ts.InterfaceDeclaration

        match checker.getSymbolAtLocation declaration.name with
        | Some symbol ->
            match mainDeclaration packageContext symbol with
            | Some main ->
                not (obj.ReferenceEquals(main, statement))
                && not (otherFileInterfaceDeclarations packageContext symbol main).IsEmpty
                && fileOfDeclaration main <> fileOfDeclaration statement
            | None -> false
        | None -> false
    | _ -> false

let private declarationFile (packageContext: PackageContext option) (symbol: Ts.Symbol) =
    mainDeclaration packageContext symbol
    |> Option.map (fun declaration -> declaration.getSourceFile().fileName |> String.normalizePath)

/// In package mode, whether the symbol is declared outside every package being generated
let isExternalToPackages
    (checker: Ts.TypeChecker)
    (packageContext: PackageContext option)
    (symbolOpt: Ts.Symbol option)
    =
    match packageContext, symbolOpt with
    | Some packageContext, Some symbol ->
        match resolveAlias checker symbol with
        | None -> true
        | Some symbol ->
            match declarationFile (Some packageContext) symbol with
            | Some fileName -> packageContext.IsExternal fileName
            | None -> true
    | _ -> false

/// <summary>
/// The ambient module (<c>declare module "os" { ... }</c>) a file is made of: its members are
/// the members of the file and its types are not nested in a module named after it.
/// A file also declaring other things keeps its ambient modules nested. When several ambient
/// modules are declared, the one named like the file wins, then the first one.
/// </summary>
let promotedAmbientModule (sourceFile: Ts.SourceFile) : Ts.ModuleDeclaration option =
    let isAmbientModule (statement: Ts.Node) =
        statement.kind = Ts.SyntaxKind.ModuleDeclaration
        && (statement :?> Ts.ModuleDeclaration).name?kind = Ts.SyntaxKind.StringLiteral

    let statements = sourceFile.statements |> Seq.toList

    let hasOtherDeclarations =
        statements
        |> List.exists (fun statement ->
            match statement.kind with
            | Ts.SyntaxKind.ImportDeclaration
            | Ts.SyntaxKind.ImportEqualsDeclaration
            | Ts.SyntaxKind.ExportDeclaration
            | Ts.SyntaxKind.EmptyStatement -> false
            | _ -> not (isAmbientModule statement)
        )

    if hasOtherDeclarations then
        None
    else
        let ambientModules =
            statements
            |> List.filter isAmbientModule
            |> List.map (fun statement -> statement :?> Ts.ModuleDeclaration)

        let fileName =
            System.Text.RegularExpressions.Regex.Replace(
                (String.normalizePath sourceFile.fileName).Split('/') |> Seq.last,
                "\\.d\\.[cm]?ts$",
                ""
            )

        // `declare module "node:http"` holds the declarations, `declare module "http"` re-exports them
        let moduleName (moduleDeclaration: Ts.ModuleDeclaration) =
            let name: string = Naming.removeSurroundingQuotes (moduleDeclaration.name?text)

            if name.StartsWith "node:" then
                name.Substring "node:".Length
            else
                name

        let hasDeclarations (moduleDeclaration: Ts.ModuleDeclaration) =
            match moduleDeclaration.body with
            | Some body when body?kind = Ts.SyntaxKind.ModuleBlock ->
                (unbox<Ts.ModuleBlock> body).statements
                |> Seq.exists (fun statement -> statement.kind <> Ts.SyntaxKind.ExportDeclaration)
            | _ -> false

        let namedLikeTheFile =
            ambientModules
            |> List.filter (fun moduleDeclaration -> moduleName moduleDeclaration = fileName)

        namedLikeTheFile
        |> List.tryFind hasDeclarations
        |> Option.orElse (List.tryHead namedLikeTheFile)
        |> Option.orElse (List.tryHead ambientModules)

let isPromotedAmbientModule (declaration: Ts.ModuleDeclaration) =
    let parent: Ts.Node = !!declaration.parent

    not (isNull (box parent))
    && parent.kind = Ts.SyntaxKind.SourceFile
    && (
        match promotedAmbientModule (parent :?> Ts.SourceFile) with
        | Some promoted -> obj.ReferenceEquals(promoted, declaration)
        | None -> false
    )

/// Whether a top-level declaration of a module file is reachable from the outside: through
/// `export` or an `export { x }` list. Declarations of a script and of an ambient namespace
/// always are, `export default x` and `export = x` are read by their own statement.
let isExportedDeclaration (statement: Ts.Node) (names: Collections.Set<string>) =
    let hasExportModifier =
        let modifiers: ResizeArray<Ts.Node> option = statement?modifiers

        match modifiers with
        | Some modifiers ->
            modifiers
            |> Seq.exists (fun modifier -> modifier.kind = Ts.SyntaxKind.ExportKeyword)
        | None -> false

    let isInsideNamespace = statement.parent?kind = Ts.SyntaxKind.ModuleBlock

    let isGlobal =
        statement.parent?kind = Ts.SyntaxKind.SourceFile
        && not (ts.isExternalModule (statement.getSourceFile ()))

    let isInExportList =
        statement.parent?kind = Ts.SyntaxKind.SourceFile
        && (statement.getSourceFile ()).statements
           |> Seq.exists (fun other ->
               match other.kind with
               | Ts.SyntaxKind.ExportDeclaration ->
                   let exportDeclaration = other :?> Ts.ExportDeclaration

                   exportDeclaration.moduleSpecifier.IsNone
                   && (
                       match exportDeclaration.exportClause with
                       | Some exportClause when exportClause?kind = Ts.SyntaxKind.NamedExports ->
                           let namedExports: Ts.NamedExports = !!exportClause

                           namedExports.elements
                           |> Seq.exists (fun specifier ->
                               let local: Ts.Node =
                                   match specifier.propertyName with
                                   | Some propertyName -> !!propertyName
                                   | None -> !!specifier.name

                               Microsoft.FSharp.Collections.Set.contains
                                   (identifierText local)
                                   names
                           )
                       | _ -> false
                   )
               | _ -> false
           )

    hasExportModifier || isInsideNamespace || isGlobal || isInExportList

/// `declare class Dispatcher {}; export default Dispatcher` or `export = Dispatcher`
let isExportAssignmentTarget (statement: Ts.Node) (names: Collections.Set<string>) =
    statement.parent?kind = Ts.SyntaxKind.SourceFile
    && (statement.getSourceFile ()).statements
       |> Seq.exists (fun other ->
           other.kind = Ts.SyntaxKind.ExportAssignment
           && (let expression: Ts.Node = (other :?> Ts.ExportAssignment).expression

               expression.kind = Ts.SyntaxKind.Identifier
               && Microsoft.FSharp.Collections.Set.contains (identifierText expression) names)
       )

/// `declare const wm: WebMidi; export { wm as WebMidi }`: the name the declaration is exported under
let exportedAlias (statement: Ts.Node) (localName: string) : string option =
    if statement.parent?kind <> Ts.SyntaxKind.SourceFile then
        None
    else
        (statement.getSourceFile ()).statements
        |> Seq.tryPick (fun other ->
            if other.kind <> Ts.SyntaxKind.ExportDeclaration then
                None
            else
                let exportDeclaration = other :?> Ts.ExportDeclaration

                match exportDeclaration.exportClause with
                | Some exportClause when
                    exportDeclaration.moduleSpecifier.IsNone
                    && exportClause?kind = Ts.SyntaxKind.NamedExports
                    ->
                    let namedExports: Ts.NamedExports = !!exportClause

                    namedExports.elements
                    |> Seq.tryPick (fun specifier ->
                        match specifier.propertyName with
                        | Some propertyName when identifierText !!propertyName = localName ->
                            Some specifier.name.text
                        | _ -> None
                    )
                | _ -> None
        )

let isGlobalAugmentation (declaration: Ts.ModuleDeclaration) =
    int declaration.flags &&& int Ts.NodeFlags.GlobalAugmentation <> 0

/// A script file of a package which is not its entry: its declarations are globals of the package
let isGlobalScriptOfPackage (packageContext: PackageContext option) (sourceFile: Ts.SourceFile) =
    match packageContext with
    | Some packageContext ->
        not (ts.isExternalModule sourceFile)
        && (promotedAmbientModule sourceFile).IsNone
        && not (packageContext.IsEntryFile sourceFile.fileName)
    | None -> false

let private isAmbientModuleDeclaration (node: Ts.Node) =
    node.kind = Ts.SyntaxKind.ModuleDeclaration
    && (node :?> Ts.ModuleDeclaration).name?kind = Ts.SyntaxKind.StringLiteral

/// In package mode, a declaration of a `global { }` block or of a script file is a global of the package,
/// the `declare module "x"` of a script file are modules
let isPackageGlobal (packageContext: PackageContext option) (declaration: Ts.Node) =
    let rec inGlobalBlock (node: Ts.Node) =
        not (isNull node)
        && ((node.kind = Ts.SyntaxKind.ModuleDeclaration
             && isGlobalAugmentation (node :?> Ts.ModuleDeclaration))
            || inGlobalBlock node.parent)

    let rec inAmbientModule (node: Ts.Node) =
        not (isNull node)
        && (isAmbientModuleDeclaration node || inAmbientModule node.parent)

    packageContext.IsSome
    && (inGlobalBlock declaration.parent
        || (isGlobalScriptOfPackage packageContext (declaration.getSourceFile ())
            && not (inAmbientModule declaration)))

/// A module declaration at the top of a file or of the ambient module the file is made of
let isTopLevelModuleDeclaration (packageContext: PackageContext option) (node: Ts.Node) =
    not (isNull node.parent)
    && (node.parent.kind = Ts.SyntaxKind.SourceFile
        || (node.parent.kind = Ts.SyntaxKind.ModuleBlock
            && node.parent.parent.kind = Ts.SyntaxKind.ModuleDeclaration
            && isPromotedAmbientModule (node.parent.parent :?> Ts.ModuleDeclaration)))
    && not (isPackageGlobal packageContext node)

/// The F# modules generated for the namespaces enclosing a declaration, outermost first
let private namespaceChain (packageContext: PackageContext option) (declaration: Ts.Node) =
    let rec collect (node: Ts.Node) (acc: string list) =
        if isNull node then
            acc
        else
            match node.kind with
            // The file is the module
            | Ts.SyntaxKind.ModuleDeclaration when
                isPromotedAmbientModule (node :?> Ts.ModuleDeclaration)
                ->
                collect node.parent acc

            | Ts.SyntaxKind.ModuleDeclaration when
                packageContext.IsSome && isGlobalAugmentation (node :?> Ts.ModuleDeclaration)
                ->
                acc

            | Ts.SyntaxKind.ModuleDeclaration ->
                let moduleDeclaration = node :?> Ts.ModuleDeclaration

                let rawName =
                    (unbox<Ts.Node> moduleDeclaration.name).getText()
                    |> Naming.removeSurroundingQuotes

                // The suffix is part of the name to escape (`assert_`, not ``` ``assert``_ ```)
                let name =
                    if isTopLevelModuleDeclaration packageContext node then
                        Naming.sanitizeTypeName (rawName + "_")
                    else
                        Naming.sanitizeTypeName rawName

                collect node.parent (name :: acc)
            | _ -> collect node.parent acc

    collect declaration.parent []

/// The F# modules qualifying a declaration from another file: its file modules and its namespaces
let modulePathForDeclaration (packageContext: PackageContext) (declaration: Ts.Node) : string list =
    let fileName = declaration.getSourceFile().fileName |> String.normalizePath

    packageContext.ModulePath(fileName, not (isPackageGlobal (Some packageContext) declaration))
    @ namespaceChain (Some packageContext) declaration

/// <summary>
/// The F# modules qualifying a reference to the symbol: the package and file modules
/// in package mode, and the namespaces when the reference is written qualified.
/// </summary>
let modulePathForSymbol
    (checker: Ts.TypeChecker)
    (packageContext: PackageContext option)
    (isQualified: bool)
    (symbolOpt: Ts.Symbol option)
    : string list
    =
    match symbolOpt |> Option.bind (resolveAlias checker) with
    | None -> []
    | Some symbol ->
        let declaration = mainDeclaration packageContext symbol

        let filePath =
            match packageContext, declarationFile packageContext symbol with
            | Some packageContext, Some fileName ->
                let includeFile =
                    match declaration with
                    | Some declaration -> not (isPackageGlobal (Some packageContext) declaration)
                    | None -> true

                packageContext.ModulePath(fileName, includeFile)
            | _ -> []

        // A path to another file is absolute, so it includes the namespaces
        let namespaces =
            match declaration with
            | Some declaration when isQualified || not filePath.IsEmpty ->
                namespaceChain packageContext declaration
            | _ -> []

        filePath @ namespaces

module Type =

    module StringLiteral =

        let (|String|Other|) (literalType: Ts.Type) =
            match literalType.flags with
            | HasTypeFlags Ts.TypeFlags.StringLiteral ->
                let literalType = literalType :?> Ts.LiteralType

                String(unbox<string> literalType.value)
            | _ -> Other

    module NumberLiteral =

        let (|Int|Float|Other|) (literalType: Ts.Type) =
            match literalType.flags with
            | HasTypeFlags Ts.TypeFlags.NumberLiteral ->
                let literalType = literalType :?> Ts.LiteralType

                if Constructors.Number.isSafeInteger literalType.value then
                    Int(unbox<int> literalType.value)
                else
                    Float(unbox<float> literalType.value)
            | _ -> Other
