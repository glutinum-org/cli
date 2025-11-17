// ts2fable 0.9.0
module rec TypeScript

#nowarn "3390" // disable warnings for invalid XML comments
#nowarn "0044" // disable warnings for `Obsolete` usage

open System
open Fable.Core
open Fable.Core.JS

[<Erase>] type KeyOf<'T> = Key of string
type Array<'T> = System.Collections.Generic.IList<'T>
type ReadonlyArray<'T> = System.Collections.Generic.IReadOnlyList<'T>
type ReadonlyMap<'K, 'V> = Map<'K, 'V>
type ReadonlySet<'T> = Set<'T>
type Symbol = obj

let [<ImportDefault("typescript")>] ts: Ts.IExports = jsNative

module Ts =
    let [<Import("ScriptSnapshot","module/ts")>] scriptSnapshot: ScriptSnapshot.IExports = jsNative

    type NoisTypeNodede =
        class end

    type [<AllowNullLiteral>] IExports =
        abstract versionMajorMinor: obj
        /// The version of the TypeScript compiler release
        abstract version: string
        abstract OperationCanceledException: OperationCanceledExceptionStatic
        abstract sys: System with get, set
        abstract tokenToString: t: SyntaxKind -> string option
        abstract getPositionOfLineAndCharacter: sourceFile: SourceFileLike * line: float * character: float -> float
        abstract getLineAndCharacterOfPosition: sourceFile: SourceFileLike * position: float -> LineAndCharacter
        abstract isWhiteSpaceLike: ch: float -> bool
        /// Does not include line breaks. For that, see isWhiteSpaceLike.
        abstract isWhiteSpaceSingleLine: ch: float -> bool
        abstract isLineBreak: ch: float -> bool
        abstract couldStartTrivia: text: string * pos: float -> bool
        abstract forEachLeadingCommentRange: text: string * pos: float * cb: (float -> float -> CommentKind -> bool -> 'U) -> 'U option
        abstract forEachLeadingCommentRange: text: string * pos: float * cb: (float -> float -> CommentKind -> bool -> 'T -> 'U) * state: 'T -> 'U option
        abstract forEachTrailingCommentRange: text: string * pos: float * cb: (float -> float -> CommentKind -> bool -> 'U) -> 'U option
        abstract forEachTrailingCommentRange: text: string * pos: float * cb: (float -> float -> CommentKind -> bool -> 'T -> 'U) * state: 'T -> 'U option
        abstract reduceEachLeadingCommentRange: text: string * pos: float * cb: (float -> float -> CommentKind -> bool -> 'T -> 'U) * state: 'T * initial: 'U -> 'U option
        abstract reduceEachTrailingCommentRange: text: string * pos: float * cb: (float -> float -> CommentKind -> bool -> 'T -> 'U) * state: 'T * initial: 'U -> 'U option
        abstract getLeadingCommentRanges: text: string * pos: float -> ResizeArray<CommentRange> option
        abstract getTrailingCommentRanges: text: string * pos: float -> ResizeArray<CommentRange> option
        /// Optionally, get the shebang
        abstract getShebang: text: string -> string option
        abstract isIdentifierStart: ch: float * languageVersion: ScriptTarget option -> bool
        abstract isIdentifierPart: ch: float * languageVersion: ScriptTarget option * ?identifierVariant: LanguageVariant -> bool
        abstract createScanner: languageVersion: ScriptTarget * skipTrivia: bool * ?languageVariant: LanguageVariant * ?textInitial: string * ?onError: ErrorCallback * ?start: float * ?length: float -> Scanner
        abstract isExternalModuleNameRelative: moduleName: string -> bool
        abstract sortAndDeduplicateDiagnostics: diagnostics: ResizeArray<'T> -> SortedReadonlyArray<'T> when 'T :> Diagnostic
        abstract getDefaultLibFileName: options: CompilerOptions -> string
        abstract textSpanEnd: span: TextSpan -> float
        abstract textSpanIsEmpty: span: TextSpan -> bool
        abstract textSpanContainsPosition: span: TextSpan * position: float -> bool
        abstract textSpanContainsTextSpan: span: TextSpan * other: TextSpan -> bool
        abstract textSpanOverlapsWith: span: TextSpan * other: TextSpan -> bool
        abstract textSpanOverlap: span1: TextSpan * span2: TextSpan -> TextSpan option
        abstract textSpanIntersectsWithTextSpan: span: TextSpan * other: TextSpan -> bool
        abstract textSpanIntersectsWith: span: TextSpan * start: float * length: float -> bool
        abstract decodedTextSpanIntersectsWith: start1: float * length1: float * start2: float * length2: float -> bool
        abstract textSpanIntersectsWithPosition: span: TextSpan * position: float -> bool
        abstract textSpanIntersection: span1: TextSpan * span2: TextSpan -> TextSpan option
        abstract createTextSpan: start: float * length: float -> TextSpan
        abstract createTextSpanFromBounds: start: float * ``end``: float -> TextSpan
        abstract textChangeRangeNewSpan: range: TextChangeRange -> TextSpan
        abstract textChangeRangeIsUnchanged: range: TextChangeRange -> bool
        abstract createTextChangeRange: span: TextSpan * newLength: float -> TextChangeRange
        /// Called to merge all the changes that occurred across several versions of a script snapshot
        /// into a single change.  i.e. if a user keeps making successive edits to a script we will
        /// have a text change from V1 to V2, V2 to V3, ..., Vn.
        ///
        /// This function will then merge those changes into a single change range valid between V1 and
        /// Vn.
        abstract collapseTextChangeRangesAcrossMultipleVersions: changes: ResizeArray<TextChangeRange> -> TextChangeRange
        abstract getTypeParameterOwner: d: Declaration -> Declaration option
        abstract isParameterPropertyDeclaration: node: Node * parent: Node -> bool
        abstract isEmptyBindingPattern: node: BindingName -> bool
        abstract isEmptyBindingElement: node: U2<BindingElement, ArrayBindingElement> -> bool
        abstract walkUpBindingElementsAndPatterns: binding: BindingElement -> U2<VariableDeclaration, ParameterDeclaration>
        abstract getCombinedModifierFlags: node: Declaration -> ModifierFlags
        abstract getCombinedNodeFlags: node: Node -> NodeFlags
        /// Checks to see if the locale is in the appropriate format,
        /// and if it is, attempts to set the appropriate language.
        abstract validateLocaleAndSetLanguage: locale: string * sys: {| getExecutingFilePath: unit -> string; resolvePath: string -> string; fileExists: string -> bool; readFile: string -> string option |} * ?errors: ResizeArray<Diagnostic> -> unit
        abstract getOriginalNode: node: Node -> Node
        abstract getOriginalNode: node: Node * nodeTest: (Node -> bool) -> 'T when 'T :> Node
        abstract getOriginalNode: node: Node option -> Node option
        abstract getOriginalNode: node: Node option * nodeTest: (Node -> bool) -> 'T option when 'T :> Node
        /// Iterates through the parent chain of a node and performs the callback on each parent until the callback
        /// returns a truthy value, then returns that value.
        /// If no such value is found, it applies the callback until the parent pointer is undefined or the callback returns "quit"
        /// At that point findAncestor returns undefined.
        abstract findAncestor: node: Node option * callback: (Node -> bool) -> 'T option when 'T :> Node
        abstract findAncestor: node: Node option * callback: (Node -> U2<bool, string>) -> Node option
        /// <summary>Gets a value indicating whether a node originated in the parse tree.</summary>
        /// <param name="node">The node to test.</param>
        abstract isParseTreeNode: node: Node -> bool
        /// <summary>Gets the original parse tree node for a node.</summary>
        /// <param name="node">The original node.</param>
        /// <returns>The original parse tree node if found; otherwise, undefined.</returns>
        abstract getParseTreeNode: node: Node option -> Node option
        /// <summary>Gets the original parse tree node for a node.</summary>
        /// <param name="node">The original node.</param>
        /// <param name="nodeTest">A callback used to ensure the correct type of parse tree node is returned.</param>
        /// <returns>The original parse tree node if found; otherwise, undefined.</returns>
        abstract getParseTreeNode: node: 'T option * ?nodeTest: (Node -> bool) -> 'T option when 'T :> Node
        /// Add an extra underscore to identifiers that start with two underscores to avoid issues with magic names like '__proto__'
        abstract escapeLeadingUnderscores: identifier: string -> __String
        /// <summary>Remove extra underscore from escaped identifier text content.</summary>
        /// <param name="identifier">The escaped identifier text.</param>
        /// <returns>The unescaped identifier text.</returns>
        abstract unescapeLeadingUnderscores: identifier: __String -> string
        abstract idText: identifierOrPrivateName: U2<Identifier, PrivateIdentifier> -> string
        /// If the text of an Identifier matches a keyword (including contextual and TypeScript-specific keywords), returns the
        /// SyntaxKind for the matching keyword.
        abstract identifierToKeywordKind: node: Identifier -> KeywordSyntaxKind option
        abstract symbolName: symbol: Symbol -> string
        abstract getNameOfJSDocTypedef: declaration: JSDocTypedefTag -> U2<Identifier, PrivateIdentifier> option
        abstract getNameOfDeclaration: declaration: U2<Declaration, Expression> option -> DeclarationName option
        abstract getDecorators: node: HasDecorators -> ResizeArray<Decorator> option
        abstract getModifiers: node: HasModifiers -> ResizeArray<Modifier> option
        /// <summary>Gets the JSDoc parameter tags for the node if present.</summary>
        /// <remarks>
        /// Returns any JSDoc param tag whose name matches the provided
        /// parameter, whether a param tag on a containing function
        /// expression, or a param tag on a variable declaration whose
        /// initializer is the containing function. The tags closest to the
        /// node are returned first, so in the previous example, the param
        /// tag on the containing function expression would be first.
        ///
        /// For binding patterns, parameter tags are matched by position.
        /// </remarks>
        abstract getJSDocParameterTags: param: ParameterDeclaration -> ResizeArray<JSDocParameterTag>
        /// <summary>Gets the JSDoc type parameter tags for the node if present.</summary>
        /// <remarks>
        /// Returns any JSDoc template tag whose names match the provided
        /// parameter, whether a template tag on a containing function
        /// expression, or a template tag on a variable declaration whose
        /// initializer is the containing function. The tags closest to the
        /// node are returned first, so in the previous example, the template
        /// tag on the containing function expression would be first.
        /// </remarks>
        abstract getJSDocTypeParameterTags: param: TypeParameterDeclaration -> ResizeArray<JSDocTemplateTag>
        /// <summary>Return true if the node has JSDoc parameter tags.</summary>
        /// <remarks>
        /// Includes parameter tags that are not directly on the node,
        /// for example on a variable declaration whose initializer is a function expression.
        /// </remarks>
        abstract hasJSDocParameterTags: node: U2<FunctionLikeDeclaration, SignatureDeclaration> -> bool
        /// Gets the JSDoc augments tag for the node if present
        abstract getJSDocAugmentsTag: node: Node -> JSDocAugmentsTag option
        /// Gets the JSDoc implements tags for the node if present
        abstract getJSDocImplementsTags: node: Node -> ResizeArray<JSDocImplementsTag>
        /// Gets the JSDoc class tag for the node if present
        abstract getJSDocClassTag: node: Node -> JSDocClassTag option
        /// Gets the JSDoc public tag for the node if present
        abstract getJSDocPublicTag: node: Node -> JSDocPublicTag option
        /// Gets the JSDoc private tag for the node if present
        abstract getJSDocPrivateTag: node: Node -> JSDocPrivateTag option
        /// Gets the JSDoc protected tag for the node if present
        abstract getJSDocProtectedTag: node: Node -> JSDocProtectedTag option
        /// Gets the JSDoc protected tag for the node if present
        abstract getJSDocReadonlyTag: node: Node -> JSDocReadonlyTag option
        abstract getJSDocOverrideTagNoCache: node: Node -> JSDocOverrideTag option
        /// Gets the JSDoc deprecated tag for the node if present
        abstract getJSDocDeprecatedTag: node: Node -> JSDocDeprecatedTag option
        /// Gets the JSDoc enum tag for the node if present
        abstract getJSDocEnumTag: node: Node -> JSDocEnumTag option
        /// Gets the JSDoc this tag for the node if present
        abstract getJSDocThisTag: node: Node -> JSDocThisTag option
        /// Gets the JSDoc return tag for the node if present
        abstract getJSDocReturnTag: node: Node -> JSDocReturnTag option
        /// Gets the JSDoc template tag for the node if present
        abstract getJSDocTemplateTag: node: Node -> JSDocTemplateTag option
        abstract getJSDocSatisfiesTag: node: Node -> JSDocSatisfiesTag option
        /// Gets the JSDoc type tag for the node if present and valid
        abstract getJSDocTypeTag: node: Node -> JSDocTypeTag option
        /// <summary>Gets the type node for the node if provided via JSDoc.</summary>
        /// <remarks>
        /// The search includes any JSDoc param tag that relates
        /// to the provided parameter, for example a type tag on the
        /// parameter itself, or a param tag on a containing function
        /// expression, or a param tag on a variable declaration whose
        /// initializer is the containing function. The tags closest to the
        /// node are examined first, so in the previous example, the type
        /// tag directly on the node would be returned.
        /// </remarks>
        abstract getJSDocType: node: Node -> TypeNode option
        /// <summary>Gets the return type node for the node if provided via JSDoc return tag or type tag.</summary>
        /// <remarks>
        /// <c>getJSDocReturnTag</c> just gets the whole JSDoc tag. This function
        /// gets the type from inside the braces, after the fat arrow, etc.
        /// </remarks>
        abstract getJSDocReturnType: node: Node -> TypeNode option
        /// Get all JSDoc tags related to a node, including those on parent nodes.
        abstract getJSDocTags: node: Node -> ResizeArray<JSDocTag>
        /// Gets all JSDoc tags that match a specified predicate
        abstract getAllJSDocTags: node: Node * predicate: (JSDocTag -> bool) -> ResizeArray<'T> when 'T :> JSDocTag
        /// Gets all JSDoc tags of a specified kind
        abstract getAllJSDocTagsOfKind: node: Node * kind: SyntaxKind -> ResizeArray<JSDocTag>
        /// Gets the text of a jsdoc comment, flattening links to their text.
        abstract getTextOfJSDocComment: ?comment: U2<string, ResizeArray<JSDocComment>> -> string option
        /// <summary>
        /// Gets the effective type parameters. If the node was parsed in a
        /// JavaScript file, gets the type parameters from the <c>@template</c> tag from JSDoc.
        ///
        /// This does *not* return type parameters from a jsdoc reference to a generic type, eg
        ///
        /// type Id = &lt;T&gt;(x: T) =&gt; T
        /// /**
        /// </summary>
        abstract getEffectiveTypeParameterDeclarations: node: DeclarationWithTypeParameters -> ResizeArray<TypeParameterDeclaration>
        abstract getEffectiveConstraintOfTypeParameter: node: TypeParameterDeclaration -> TypeNode option
        abstract isMemberName: node: Node -> bool
        abstract isPropertyAccessChain: node: Node -> bool
        abstract isElementAccessChain: node: Node -> bool
        abstract isCallChain: node: Node -> bool
        abstract isOptionalChain: node: Node -> bool
        abstract isNullishCoalesce: node: Node -> bool
        abstract isConstTypeReference: node: Node -> bool
        abstract skipPartiallyEmittedExpressions: node: Expression -> Expression
        abstract skipPartiallyEmittedExpressions: node: Node -> Node
        abstract isNonNullChain: node: Node -> bool
        abstract isBreakOrContinueStatement: node: Node -> bool
        abstract isNamedExportBindings: node: Node -> bool
        [<Obsolete("")>]
        abstract isUnparsedTextLike: node: Node -> bool
        [<Obsolete("")>]
        abstract isUnparsedNode: node: Node -> bool
        abstract isJSDocPropertyLikeTag: node: Node -> bool
        /// True if kind is of some token syntax kind.
        /// For example, this is true for an IfKeyword but not for an IfStatement.
        /// Literals are considered tokens, except TemplateLiteral, but does include TemplateHead/Middle/Tail.
        abstract isTokenKind: kind: SyntaxKind -> bool
        /// True if node is of some token syntax kind.
        /// For example, this is true for an IfKeyword but not for an IfStatement.
        /// Literals are considered tokens, except TemplateLiteral, but does include TemplateHead/Middle/Tail.
        abstract isToken: n: Node -> bool
        abstract isLiteralExpression: node: Node -> bool
        abstract isTemplateLiteralToken: node: Node -> bool
        abstract isTemplateMiddleOrTemplateTail: node: Node -> bool
        abstract isImportOrExportSpecifier: node: Node -> bool
        abstract isTypeOnlyImportDeclaration: node: Node -> bool
        abstract isTypeOnlyExportDeclaration: node: Node -> bool
        abstract isTypeOnlyImportOrExportDeclaration: node: Node -> bool
        abstract isAssertionKey: node: Node -> bool
        abstract isStringTextContainingNode: node: Node -> bool
        abstract isModifier: node: Node -> bool
        abstract isEntityName: node: Node -> bool
        abstract isPropertyName: node: Node -> bool
        abstract isBindingName: node: Node -> bool
        abstract isFunctionLike: node: Node option -> bool
        abstract isClassElement: node: Node -> bool
        abstract isClassLike: node: Node -> bool
        abstract isAccessor: node: Node -> bool
        abstract isAutoAccessorPropertyDeclaration: node: Node -> bool
        abstract isModifierLike: node: Node -> bool
        abstract isTypeElement: node: Node -> bool
        abstract isClassOrTypeElement: node: NoisTypeNodede -> bool
        abstract isObjectLiteralElementLike: node: Node -> bool
        /// <summary>
        /// Node test that determines whether a node is a valid type node.
        /// This differs from the <c>isPartOfTypeNode</c> function which determines whether a node is *part*
        /// of a TypeNode.
        /// </summary>
        abstract isTypeNode: node: Node -> bool
        abstract isFunctionOrConstructorTypeNode: node: Node -> bool
        abstract isArrayBindingElement: node: Node -> bool
        abstract isPropertyAccessOrQualifiedName: node: Node -> bool
        abstract isCallLikeExpression: node: Node -> bool
        abstract isCallOrNewExpression: node: Node -> bool
        abstract isTemplateLiteral: node: Node -> bool
        abstract isLeftHandSideExpression: node: Node -> bool
        abstract isLiteralTypeLiteral: node: Node -> bool
        /// Determines whether a node is an expression based only on its kind.
        abstract isExpression: node: Node -> bool
        abstract isAssertionExpression: node: Node -> bool
        [<Emit("$0.isIterationStatement($1,false)")>] abstract isIterationStatement_false: node: Node -> bool
        abstract isIterationStatement: node: Node * lookInLabeledStatements: bool -> bool
        abstract isConciseBody: node: Node -> bool
        abstract isForInitializer: node: Node -> bool
        abstract isModuleBody: node: Node -> bool
        abstract isNamedImportBindings: node: Node -> bool
        abstract isStatement: node: Node -> bool
        abstract isModuleReference: node: Node -> bool
        abstract isJsxTagNameExpression: node: Node -> bool
        abstract isJsxChild: node: Node -> bool
        abstract isJsxAttributeLike: node: Node -> bool
        abstract isStringLiteralOrJsxExpression: node: Node -> bool
        abstract isJsxOpeningLikeElement: node: Node -> bool
        abstract isCaseOrDefaultClause: node: Node -> bool
        /// True if node is of a kind that may contain comment text.
        abstract isJSDocCommentContainingNode: node: Node -> bool
        abstract isSetAccessor: node: Node -> bool
        abstract isGetAccessor: node: Node -> bool
        /// True if has initializer node attached to it.
        abstract hasOnlyExpressionInitializer: node: Node -> bool
        abstract isObjectLiteralElement: node: Node -> bool
        abstract isStringLiteralLike: node: U2<Node, FileReference> -> bool
        abstract isJSDocLinkLike: node: Node -> bool
        abstract hasRestParameter: s: U2<SignatureDeclaration, JSDocSignature> -> bool
        abstract isRestParameter: node: U2<ParameterDeclaration, JSDocParameterTag> -> bool
        abstract unchangedTextChangeRange: TextChangeRange with get, set
        /// <summary>
        /// This function checks multiple locations for JSDoc comments that apply to a host node.
        /// At each location, the whole comment may apply to the node, or only a specific tag in
        /// the comment. In the first case, location adds the entire <see cref="JSDoc" /> object. In the
        /// second case, it adds the applicable <see cref="JSDocTag" />.
        ///
        /// For example, a JSDoc comment before a parameter adds the entire <see cref="JSDoc" />. But a
        /// <c>@param</c> tag on the parent function only adds the <see cref="JSDocTag" /> for the <c>@param</c>.
        ///
        /// <code lang="ts">
        /// /** JSDoc will be returned for `a` *\/
        /// const a = 0
        /// /**
        ///  * Entire JSDoc will be returned for `b`
        ///  *
        /// </code>
        /// </summary>
        /// <param name="c">
        /// JSDocTag will be returned for <c>c</c>
        /// *\/
        /// function b(/** JSDoc will be returned for <c>c</c> *\/ c) {}
        /// <code>
        /// </code>
        /// </param>
        abstract getJSDocCommentsAndTags: hostNode: Node -> ResizeArray<U2<JSDoc, JSDocTag>>
        [<Obsolete("")>]
        abstract createUnparsedSourceFile: text: string -> UnparsedSource
        [<Obsolete("")>]
        abstract createUnparsedSourceFile: inputFile: InputFiles * ``type``: IExportsCreateUnparsedSourceFile * ?stripInternal: bool -> UnparsedSource
        [<Obsolete("")>]
        abstract createUnparsedSourceFile: text: string * mapPath: string option * map: string option -> UnparsedSource
        [<Obsolete("")>]
        abstract createInputFiles: javascriptText: string * declarationText: string -> InputFiles
        [<Obsolete("")>]
        abstract createInputFiles: javascriptText: string * declarationText: string * javascriptMapPath: string option * javascriptMapText: string option * declarationMapPath: string option * declarationMapText: string option -> InputFiles
        [<Obsolete("")>]
        abstract createInputFiles: readFileText: (string -> string option) * javascriptPath: string * javascriptMapPath: string option * declarationPath: string * declarationMapPath: string option * buildInfoPath: string option -> InputFiles
        /// Create an external source map source file reference
        abstract createSourceMapSource: fileName: string * text: string * ?skipTrivia: (float -> float) -> SourceMapSource
        abstract setOriginalNode: node: 'T * original: Node option -> 'T when 'T :> Node
        abstract factory: NodeFactory
        /// <summary>Clears any <c>EmitNode</c> entries from parse-tree nodes.</summary>
        /// <param name="sourceFile">A source file.</param>
        abstract disposeEmitNodes: sourceFile: SourceFile option -> unit
        /// Sets flags that control emit behavior of a node.
        abstract setEmitFlags: node: 'T * emitFlags: EmitFlags -> 'T when 'T :> Node
        /// Gets a custom text range to use when emitting source maps.
        abstract getSourceMapRange: node: Node -> SourceMapRange
        /// Sets a custom text range to use when emitting source maps.
        abstract setSourceMapRange: node: 'T * range: SourceMapRange option -> 'T when 'T :> Node
        /// Gets the TextRange to use for source maps for a token of a node.
        abstract getTokenSourceMapRange: node: Node * token: SyntaxKind -> SourceMapRange option
        /// Sets the TextRange to use for source maps for a token of a node.
        abstract setTokenSourceMapRange: node: 'T * token: SyntaxKind * range: SourceMapRange option -> 'T when 'T :> Node
        /// Gets a custom text range to use when emitting comments.
        abstract getCommentRange: node: Node -> TextRange
        /// Sets a custom text range to use when emitting comments.
        abstract setCommentRange: node: 'T * range: TextRange -> 'T when 'T :> Node
        abstract getSyntheticLeadingComments: node: Node -> ResizeArray<SynthesizedComment> option
        abstract setSyntheticLeadingComments: node: 'T * comments: ResizeArray<SynthesizedComment> option -> 'T when 'T :> Node
        abstract addSyntheticLeadingComment: node: 'T * kind: SyntaxKind * text: string * ?hasTrailingNewLine: bool -> 'T when 'T :> Node
        abstract getSyntheticTrailingComments: node: Node -> ResizeArray<SynthesizedComment> option
        abstract setSyntheticTrailingComments: node: 'T * comments: ResizeArray<SynthesizedComment> option -> 'T when 'T :> Node
        abstract addSyntheticTrailingComment: node: 'T * kind: SyntaxKind * text: string * ?hasTrailingNewLine: bool -> 'T when 'T :> Node
        abstract moveSyntheticComments: node: 'T * original: Node -> 'T when 'T :> Node
        /// Gets the constant value to emit for an expression representing an enum.
        abstract getConstantValue: node: AccessExpression -> U2<string, float> option
        /// Sets the constant value to emit for an expression.
        abstract setConstantValue: node: AccessExpression * value: U2<string, float> -> AccessExpression
        /// Adds an EmitHelper to a node.
        abstract addEmitHelper: node: 'T * helper: EmitHelper -> 'T when 'T :> Node
        /// Add EmitHelpers to a node.
        abstract addEmitHelpers: node: 'T * helpers: ResizeArray<EmitHelper> option -> 'T when 'T :> Node
        /// Removes an EmitHelper from a node.
        abstract removeEmitHelper: node: Node * helper: EmitHelper -> bool
        /// Gets the EmitHelpers of a node.
        abstract getEmitHelpers: node: Node -> ResizeArray<EmitHelper> option
        /// Moves matching emit helpers from a source node to a target node.
        abstract moveEmitHelpers: source: Node * target: Node * predicate: (EmitHelper -> bool) -> unit
        abstract isNumericLiteral: node: Node -> bool
        abstract isBigIntLiteral: node: Node -> bool
        abstract isStringLiteral: node: Node -> bool
        abstract isJsxText: node: Node -> bool
        abstract isRegularExpressionLiteral: node: Node -> bool
        abstract isNoSubstitutionTemplateLiteral: node: Node -> bool
        abstract isTemplateHead: node: Node -> bool
        abstract isTemplateMiddle: node: Node -> bool
        abstract isTemplateTail: node: Node -> bool
        abstract isDotDotDotToken: node: Node -> bool
        abstract isPlusToken: node: Node -> bool
        abstract isMinusToken: node: Node -> bool
        abstract isAsteriskToken: node: Node -> bool
        abstract isExclamationToken: node: Node -> bool
        abstract isQuestionToken: node: Node -> bool
        abstract isColonToken: node: Node -> bool
        abstract isQuestionDotToken: node: Node -> bool
        abstract isEqualsGreaterThanToken: node: Node -> bool
        abstract isIdentifier: node: Node -> bool
        abstract isPrivateIdentifier: node: Node -> bool
        abstract isAssertsKeyword: node: Node -> bool
        abstract isAwaitKeyword: node: Node -> bool
        abstract isQualifiedName: node: Node -> bool
        abstract isComputedPropertyName: node: Node -> bool
        abstract isTypeParameterDeclaration: node: Node -> bool
        abstract isParameter: node: Node -> bool
        abstract isDecorator: node: Node -> bool
        abstract isPropertySignature: node: Node -> bool
        abstract isPropertyDeclaration: node: Node -> bool
        abstract isMethodSignature: node: Node -> bool
        abstract isMethodDeclaration: node: Node -> bool
        abstract isClassStaticBlockDeclaration: node: Node -> bool
        abstract isConstructorDeclaration: node: Node -> bool
        abstract isGetAccessorDeclaration: node: Node -> bool
        abstract isSetAccessorDeclaration: node: Node -> bool
        abstract isCallSignatureDeclaration: node: Node -> bool
        abstract isConstructSignatureDeclaration: node: Node -> bool
        abstract isIndexSignatureDeclaration: node: Node -> bool
        abstract isTypePredicateNode: node: Node -> bool
        abstract isTypeReferenceNode: node: Node -> bool
        abstract isFunctionTypeNode: node: Node -> bool
        abstract isConstructorTypeNode: node: Node -> bool
        abstract isTypeQueryNode: node: Node -> bool
        abstract isTypeLiteralNode: node: Node -> bool
        abstract isArrayTypeNode: node: Node -> bool
        abstract isTupleTypeNode: node: Node -> bool
        abstract isNamedTupleMember: node: Node -> bool
        abstract isOptionalTypeNode: node: Node -> bool
        abstract isRestTypeNode: node: Node -> bool
        abstract isUnionTypeNode: node: Node -> bool
        abstract isIntersectionTypeNode: node: Node -> bool
        abstract isConditionalTypeNode: node: Node -> bool
        abstract isInferTypeNode: node: Node -> bool
        abstract isParenthesizedTypeNode: node: Node -> bool
        abstract isThisTypeNode: node: Node -> bool
        abstract isTypeOperatorNode: node: Node -> bool
        abstract isIndexedAccessTypeNode: node: Node -> bool
        abstract isMappedTypeNode: node: Node -> bool
        abstract isLiteralTypeNode: node: Node -> bool
        abstract isImportTypeNode: node: Node -> bool
        abstract isTemplateLiteralTypeSpan: node: Node -> bool
        abstract isTemplateLiteralTypeNode: node: Node -> bool
        abstract isObjectBindingPattern: node: Node -> bool
        abstract isArrayBindingPattern: node: Node -> bool
        abstract isBindingElement: node: Node -> bool
        abstract isArrayLiteralExpression: node: Node -> bool
        abstract isObjectLiteralExpression: node: Node -> bool
        abstract isPropertyAccessExpression: node: Node -> bool
        abstract isElementAccessExpression: node: Node -> bool
        abstract isCallExpression: node: Node -> bool
        abstract isNewExpression: node: Node -> bool
        abstract isTaggedTemplateExpression: node: Node -> bool
        abstract isTypeAssertionExpression: node: Node -> bool
        abstract isParenthesizedExpression: node: Node -> bool
        abstract isFunctionExpression: node: Node -> bool
        abstract isArrowFunction: node: Node -> bool
        abstract isDeleteExpression: node: Node -> bool
        abstract isTypeOfExpression: node: Node -> bool
        abstract isVoidExpression: node: Node -> bool
        abstract isAwaitExpression: node: Node -> bool
        abstract isPrefixUnaryExpression: node: Node -> bool
        abstract isPostfixUnaryExpression: node: Node -> bool
        abstract isBinaryExpression: node: Node -> bool
        abstract isConditionalExpression: node: Node -> bool
        abstract isTemplateExpression: node: Node -> bool
        abstract isYieldExpression: node: Node -> bool
        abstract isSpreadElement: node: Node -> bool
        abstract isClassExpression: node: Node -> bool
        abstract isOmittedExpression: node: Node -> bool
        abstract isExpressionWithTypeArguments: node: Node -> bool
        abstract isAsExpression: node: Node -> bool
        abstract isSatisfiesExpression: node: Node -> bool
        abstract isNonNullExpression: node: Node -> bool
        abstract isMetaProperty: node: Node -> bool
        abstract isSyntheticExpression: node: Node -> bool
        abstract isPartiallyEmittedExpression: node: Node -> bool
        abstract isCommaListExpression: node: Node -> bool
        abstract isTemplateSpan: node: Node -> bool
        abstract isSemicolonClassElement: node: Node -> bool
        abstract isBlock: node: Node -> bool
        abstract isVariableStatement: node: Node -> bool
        abstract isEmptyStatement: node: Node -> bool
        abstract isExpressionStatement: node: Node -> bool
        abstract isIfStatement: node: Node -> bool
        abstract isDoStatement: node: Node -> bool
        abstract isWhileStatement: node: Node -> bool
        abstract isForStatement: node: Node -> bool
        abstract isForInStatement: node: Node -> bool
        abstract isForOfStatement: node: Node -> bool
        abstract isContinueStatement: node: Node -> bool
        abstract isBreakStatement: node: Node -> bool
        abstract isReturnStatement: node: Node -> bool
        abstract isWithStatement: node: Node -> bool
        abstract isSwitchStatement: node: Node -> bool
        abstract isLabeledStatement: node: Node -> bool
        abstract isThrowStatement: node: Node -> bool
        abstract isTryStatement: node: Node -> bool
        abstract isDebuggerStatement: node: Node -> bool
        abstract isVariableDeclaration: node: Node -> bool
        abstract isVariableDeclarationList: node: Node -> bool
        abstract isFunctionDeclaration: node: Node -> bool
        abstract isClassDeclaration: node: Node -> bool
        abstract isInterfaceDeclaration: node: Node -> bool
        abstract isTypeAliasDeclaration: node: Node -> bool
        abstract isEnumDeclaration: node: Node -> bool
        abstract isModuleDeclaration: node: Node -> bool
        abstract isModuleBlock: node: Node -> bool
        abstract isCaseBlock: node: Node -> bool
        abstract isNamespaceExportDeclaration: node: Node -> bool
        abstract isImportEqualsDeclaration: node: Node -> bool
        abstract isImportDeclaration: node: Node -> bool
        abstract isImportClause: node: Node -> bool
        abstract isImportTypeAssertionContainer: node: Node -> bool
        abstract isAssertClause: node: Node -> bool
        abstract isAssertEntry: node: Node -> bool
        abstract isNamespaceImport: node: Node -> bool
        abstract isNamespaceExport: node: Node -> bool
        abstract isNamedImports: node: Node -> bool
        abstract isImportSpecifier: node: Node -> bool
        abstract isExportAssignment: node: Node -> bool
        abstract isExportDeclaration: node: Node -> bool
        abstract isNamedExports: node: Node -> bool
        abstract isExportSpecifier: node: Node -> bool
        abstract isMissingDeclaration: node: Node -> bool
        abstract isNotEmittedStatement: node: Node -> bool
        abstract isExternalModuleReference: node: Node -> bool
        abstract isJsxElement: node: Node -> bool
        abstract isJsxSelfClosingElement: node: Node -> bool
        abstract isJsxOpeningElement: node: Node -> bool
        abstract isJsxClosingElement: node: Node -> bool
        abstract isJsxFragment: node: Node -> bool
        abstract isJsxOpeningFragment: node: Node -> bool
        abstract isJsxClosingFragment: node: Node -> bool
        abstract isJsxAttribute: node: Node -> bool
        abstract isJsxAttributes: node: Node -> bool
        abstract isJsxSpreadAttribute: node: Node -> bool
        abstract isJsxExpression: node: Node -> bool
        abstract isJsxNamespacedName: node: Node -> bool
        abstract isCaseClause: node: Node -> bool
        abstract isDefaultClause: node: Node -> bool
        abstract isHeritageClause: node: Node -> bool
        abstract isCatchClause: node: Node -> bool
        abstract isPropertyAssignment: node: Node -> bool
        abstract isShorthandPropertyAssignment: node: Node -> bool
        abstract isSpreadAssignment: node: Node -> bool
        abstract isEnumMember: node: Node -> bool
        [<Obsolete("")>]
        abstract isUnparsedPrepend: node: Node -> bool
        abstract isSourceFile: node: Node -> bool
        abstract isBundle: node: Node -> bool
        [<Obsolete("")>]
        abstract isUnparsedSource: node: Node -> bool
        abstract isJSDocTypeExpression: node: Node -> bool
        abstract isJSDocNameReference: node: Node -> bool
        abstract isJSDocMemberName: node: Node -> bool
        abstract isJSDocLink: node: Node -> bool
        abstract isJSDocLinkCode: node: Node -> bool
        abstract isJSDocLinkPlain: node: Node -> bool
        abstract isJSDocAllType: node: Node -> bool
        abstract isJSDocUnknownType: node: Node -> bool
        abstract isJSDocNullableType: node: Node -> bool
        abstract isJSDocNonNullableType: node: Node -> bool
        abstract isJSDocOptionalType: node: Node -> bool
        abstract isJSDocFunctionType: node: Node -> bool
        abstract isJSDocVariadicType: node: Node -> bool
        abstract isJSDocNamepathType: node: Node -> bool
        abstract isJSDoc: node: Node -> bool
        abstract isJSDocTypeLiteral: node: Node -> bool
        abstract isJSDocSignature: node: Node -> bool
        abstract isJSDocAugmentsTag: node: Node -> bool
        abstract isJSDocAuthorTag: node: Node -> bool
        abstract isJSDocClassTag: node: Node -> bool
        abstract isJSDocCallbackTag: node: Node -> bool
        abstract isJSDocPublicTag: node: Node -> bool
        abstract isJSDocPrivateTag: node: Node -> bool
        abstract isJSDocProtectedTag: node: Node -> bool
        abstract isJSDocReadonlyTag: node: Node -> bool
        abstract isJSDocOverrideTag: node: Node -> bool
        abstract isJSDocOverloadTag: node: Node -> bool
        abstract isJSDocDeprecatedTag: node: Node -> bool
        abstract isJSDocSeeTag: node: Node -> bool
        abstract isJSDocEnumTag: node: Node -> bool
        abstract isJSDocParameterTag: node: Node -> bool
        abstract isJSDocReturnTag: node: Node -> bool
        abstract isJSDocThisTag: node: Node -> bool
        abstract isJSDocTypeTag: node: Node -> bool
        abstract isJSDocTemplateTag: node: Node -> bool
        abstract isJSDocTypedefTag: node: Node -> bool
        abstract isJSDocUnknownTag: node: Node -> bool
        abstract isJSDocPropertyTag: node: Node -> bool
        abstract isJSDocImplementsTag: node: Node -> bool
        abstract isJSDocSatisfiesTag: node: Node -> bool
        abstract isJSDocThrowsTag: node: Node -> bool
        abstract isQuestionOrExclamationToken: node: Node -> bool
        abstract isIdentifierOrThisTypeNode: node: Node -> bool
        abstract isReadonlyKeywordOrPlusOrMinusToken: node: Node -> bool
        abstract isQuestionOrPlusOrMinusToken: node: Node -> bool
        abstract isModuleName: node: Node -> bool
        abstract isBinaryOperatorToken: node: Node -> bool
        abstract setTextRange: range: 'T * location: TextRange option -> 'T when 'T :> TextRange
        abstract canHaveModifiers: node: Node -> bool
        abstract canHaveDecorators: node: Node -> bool
        /// <summary>
        /// Invokes a callback for each child of the given node. The 'cbNode' callback is invoked for all child nodes
        /// stored in properties. If a 'cbNodes' callback is specified, it is invoked for embedded arrays; otherwise,
        /// embedded arrays are flattened and the 'cbNode' callback is invoked for each element. If a callback returns
        /// a truthy value, iteration stops and that value is returned. Otherwise, undefined is returned.
        /// </summary>
        /// <param name="node">a given node to visit its children</param>
        /// <param name="cbNode">a callback to be invoked for all child nodes</param>
        /// <param name="cbNodes">a callback to be invoked for embedded array</param>
        /// <remarks>
        /// <c>forEachChild</c> must visit the children of a node in the order
        /// that they appear in the source code. The language service depends on this property to locate nodes by position.
        /// </remarks>
        abstract forEachChild: node: Node * cbNode: (Node -> 'T option) * ?cbNodes: (ResizeArray<Node> -> 'T option) -> 'T option
        abstract createSourceFile: fileName: string * sourceText: string * languageVersionOrOptions: U2<ScriptTarget, CreateSourceFileOptions> * ?setParentNodes: bool * ?scriptKind: ScriptKind -> SourceFile
        abstract parseIsolatedEntityName: text: string * languageVersion: ScriptTarget -> EntityName option
        /// <summary>Parse json text into SyntaxTree and return node and parse errors if any</summary>
        /// <param name="fileName" />
        /// <param name="sourceText" />
        abstract parseJsonText: fileName: string * sourceText: string -> JsonSourceFile
        abstract isExternalModule: file: SourceFile -> bool
        abstract updateSourceFile: sourceFile: SourceFile * newText: string * textChangeRange: TextChangeRange * ?aggressiveChecks: bool -> SourceFile
        abstract parseCommandLine: commandLine: ResizeArray<string> * ?readFile: (string -> string option) -> ParsedCommandLine
        /// Reads the config file, reports errors if any and exits if the config file cannot be found
        abstract getParsedCommandLineOfConfigFile: configFileName: string * optionsToExtend: CompilerOptions option * host: ParseConfigFileHost * ?extendedConfigCache: Map<string, ExtendedConfigCacheEntry> * ?watchOptionsToExtend: WatchOptions * ?extraFileExtensions: ResizeArray<FileExtensionInfo> -> ParsedCommandLine option
        /// <summary>Read tsconfig.json file</summary>
        /// <param name="fileName">The path to the config file</param>
        abstract readConfigFile: fileName: string * readFile: (string -> string option) -> {| config: obj option; error: Diagnostic option |}
        /// <summary>Parse the text of the tsconfig.json file</summary>
        /// <param name="fileName">The path to the config file</param>
        /// <param name="jsonText">The text of the config file</param>
        abstract parseConfigFileTextToJson: fileName: string * jsonText: string -> {| config: obj option; error: Diagnostic option |}
        /// <summary>Read tsconfig.json file</summary>
        /// <param name="fileName">The path to the config file</param>
        abstract readJsonConfigFile: fileName: string * readFile: (string -> string option) -> TsConfigSourceFile
        /// Convert the json syntax tree into the json value
        abstract convertToObject: sourceFile: JsonSourceFile * errors: ResizeArray<Diagnostic> -> obj option
        /// <summary>Parse the contents of a config file (tsconfig.json).</summary>
        /// <param name="json">The contents of the config file to parse</param>
        /// <param name="host">Instance of ParseConfigHost used to enumerate files in folder.</param>
        /// <param name="basePath">
        /// A root directory to resolve relative path entries in the config
        /// file to. e.g. outDir
        /// </param>
        abstract parseJsonConfigFileContent: json: obj option * host: ParseConfigHost * basePath: string * ?existingOptions: CompilerOptions * ?configFileName: string * ?resolutionStack: ResizeArray<Path> * ?extraFileExtensions: ResizeArray<FileExtensionInfo> * ?extendedConfigCache: Map<string, ExtendedConfigCacheEntry> * ?existingWatchOptions: WatchOptions -> ParsedCommandLine
        /// <summary>Parse the contents of a config file (tsconfig.json).</summary>
        /// <param name="jsonNode">The contents of the config file to parse</param>
        /// <param name="host">Instance of ParseConfigHost used to enumerate files in folder.</param>
        /// <param name="basePath">
        /// A root directory to resolve relative path entries in the config
        /// file to. e.g. outDir
        /// </param>
        abstract parseJsonSourceFileConfigFileContent: sourceFile: TsConfigSourceFile * host: ParseConfigHost * basePath: string * ?existingOptions: CompilerOptions * ?configFileName: string * ?resolutionStack: ResizeArray<Path> * ?extraFileExtensions: ResizeArray<FileExtensionInfo> * ?extendedConfigCache: Map<string, ExtendedConfigCacheEntry> * ?existingWatchOptions: WatchOptions -> ParsedCommandLine
        abstract convertCompilerOptionsFromJson: jsonOptions: obj option * basePath: string * ?configFileName: string -> {| options: CompilerOptions; errors: ResizeArray<Diagnostic> |}
        abstract convertTypeAcquisitionFromJson: jsonOptions: obj option * basePath: string * ?configFileName: string -> {| options: TypeAcquisition; errors: ResizeArray<Diagnostic> |}
        abstract getEffectiveTypeRoots: options: CompilerOptions * host: GetEffectiveTypeRootsHost -> ResizeArray<string> option
        /// <param name="containingFile">
        /// file that contains type reference directive, can be undefined if containing file is unknown.
        /// This is possible in case if resolution is performed for directives specified via 'types' parameter. In this case initial path for secondary lookups
        /// is assumed to be the same as root directory of the project.
        /// </param>
        abstract resolveTypeReferenceDirective: typeReferenceDirectiveName: string * containingFile: string option * options: CompilerOptions * host: ModuleResolutionHost * ?redirectedReference: ResolvedProjectReference * ?cache: TypeReferenceDirectiveResolutionCache * ?resolutionMode: ResolutionMode -> ResolvedTypeReferenceDirectiveWithFailedLookupLocations
        /// Given a set of options, returns the set of type directive names
        ///   that should be included for this program automatically.
        /// This list could either come from the config file,
        ///   or from enumerating the types root + initial secondary types lookup location.
        /// More type directives might appear in the program later as a result of loading actual source files;
        ///   this list is only the set of defaults that are implicitly included.
        abstract getAutomaticTypeDirectiveNames: options: CompilerOptions * host: ModuleResolutionHost -> ResizeArray<string>
        abstract createModuleResolutionCache: currentDirectory: string * getCanonicalFileName: (string -> string) * ?options: CompilerOptions * ?packageJsonInfoCache: PackageJsonInfoCache -> ModuleResolutionCache
        abstract createTypeReferenceDirectiveResolutionCache: currentDirectory: string * getCanonicalFileName: (string -> string) * ?options: CompilerOptions * ?packageJsonInfoCache: PackageJsonInfoCache -> TypeReferenceDirectiveResolutionCache
        abstract resolveModuleNameFromCache: moduleName: string * containingFile: string * cache: ModuleResolutionCache * ?mode: ResolutionMode -> ResolvedModuleWithFailedLookupLocations option
        abstract resolveModuleName: moduleName: string * containingFile: string * compilerOptions: CompilerOptions * host: ModuleResolutionHost * ?cache: ModuleResolutionCache * ?redirectedReference: ResolvedProjectReference * ?resolutionMode: ResolutionMode -> ResolvedModuleWithFailedLookupLocations
        abstract bundlerModuleNameResolver: moduleName: string * containingFile: string * compilerOptions: CompilerOptions * host: ModuleResolutionHost * ?cache: ModuleResolutionCache * ?redirectedReference: ResolvedProjectReference -> ResolvedModuleWithFailedLookupLocations
        abstract nodeModuleNameResolver: moduleName: string * containingFile: string * compilerOptions: CompilerOptions * host: ModuleResolutionHost * ?cache: ModuleResolutionCache * ?redirectedReference: ResolvedProjectReference -> ResolvedModuleWithFailedLookupLocations
        abstract classicNameResolver: moduleName: string * containingFile: string * compilerOptions: CompilerOptions * host: ModuleResolutionHost * ?cache: NonRelativeModuleNameResolutionCache * ?redirectedReference: ResolvedProjectReference -> ResolvedModuleWithFailedLookupLocations
        /// <summary>
        /// Visits a Node using the supplied visitor, possibly returning a new Node in its place.
        ///
        /// - If the input node is undefined, then the output is undefined.
        /// - If the visitor returns undefined, then the output is undefined.
        /// - If the output node is not undefined, then it will satisfy the test function.
        /// - In order to obtain a return type that is more specific than <c>Node</c>, a test
        ///   function _must_ be provided, and that function must be a type predicate.
        /// </summary>
        /// <param name="node">The Node to visit.</param>
        /// <param name="visitor">The callback used to visit the Node.</param>
        /// <param name="test">A callback to execute to verify the Node is valid.</param>
        /// <param name="lift">An optional callback to execute to lift a NodeArray into a valid Node.</param>
        abstract visitNode: node: 'TIn * visitor: Visitor<'TIn, 'TVisited> * test: (Node -> bool) * ?lift: (ResizeArray<Node> -> Node) -> U2<'TOut, obj> when 'TOut :> Node
        /// <summary>
        /// Visits a Node using the supplied visitor, possibly returning a new Node in its place.
        ///
        /// - If the input node is undefined, then the output is undefined.
        /// - If the visitor returns undefined, then the output is undefined.
        /// - If the output node is not undefined, then it will satisfy the test function.
        /// - In order to obtain a return type that is more specific than <c>Node</c>, a test
        ///   function _must_ be provided, and that function must be a type predicate.
        /// </summary>
        /// <param name="node">The Node to visit.</param>
        /// <param name="visitor">The callback used to visit the Node.</param>
        /// <param name="test">A callback to execute to verify the Node is valid.</param>
        /// <param name="lift">An optional callback to execute to lift a NodeArray into a valid Node.</param>
        abstract visitNode: node: 'TIn * visitor: Visitor<'TIn, 'TVisited> * ?test: (Node -> bool) * ?lift: (ResizeArray<Node> -> Node) -> U2<Node, obj>
        /// <summary>
        /// Visits a NodeArray using the supplied visitor, possibly returning a new NodeArray in its place.
        ///
        /// - If the input node array is undefined, the output is undefined.
        /// - If the visitor can return undefined, the node it visits in the array will be reused.
        /// - If the output node array is not undefined, then its contents will satisfy the test.
        /// - In order to obtain a return type that is more specific than <c>NodeArray&lt;Node&gt;</c>, a test
        ///   function _must_ be provided, and that function must be a type predicate.
        /// </summary>
        /// <param name="nodes">The NodeArray to visit.</param>
        /// <param name="visitor">The callback used to visit a Node.</param>
        /// <param name="test">A node test to execute for each node.</param>
        /// <param name="start">An optional value indicating the starting offset at which to start visiting.</param>
        /// <param name="count">An optional value indicating the maximum number of nodes to visit.</param>
        abstract visitNodes: nodes: 'TInArray * visitor: Visitor<'TIn, Node option> * test: (Node -> bool) * ?start: float * ?count: float -> U2<ResizeArray<'TOut>, obj> when 'TIn :> Node and 'TOut :> Node
        /// <summary>
        /// Visits a NodeArray using the supplied visitor, possibly returning a new NodeArray in its place.
        ///
        /// - If the input node array is undefined, the output is undefined.
        /// - If the visitor can return undefined, the node it visits in the array will be reused.
        /// - If the output node array is not undefined, then its contents will satisfy the test.
        /// - In order to obtain a return type that is more specific than <c>NodeArray&lt;Node&gt;</c>, a test
        ///   function _must_ be provided, and that function must be a type predicate.
        /// </summary>
        /// <param name="nodes">The NodeArray to visit.</param>
        /// <param name="visitor">The callback used to visit a Node.</param>
        /// <param name="test">A node test to execute for each node.</param>
        /// <param name="start">An optional value indicating the starting offset at which to start visiting.</param>
        /// <param name="count">An optional value indicating the maximum number of nodes to visit.</param>
        abstract visitNodes: nodes: 'TInArray * visitor: Visitor<'TIn, Node option> * ?test: (Node -> bool) * ?start: float * ?count: float -> U2<ResizeArray<Node>, obj> when 'TIn :> Node
        /// Starts a new lexical environment and visits a statement list, ending the lexical environment
        /// and merging hoisted declarations upon completion.
        abstract visitLexicalEnvironment: statements: ResizeArray<Statement> * visitor: Visitor * context: TransformationContext * ?start: float * ?ensureUseStrict: bool * ?nodesVisitor: NodesVisitor -> ResizeArray<Statement>
        /// Starts a new lexical environment and visits a parameter list, suspending the lexical
        /// environment upon completion.
        abstract visitParameterList: nodes: ResizeArray<ParameterDeclaration> * visitor: Visitor * context: TransformationContext * ?nodesVisitor: NodesVisitor -> ResizeArray<ParameterDeclaration>
        abstract visitParameterList: nodes: ResizeArray<ParameterDeclaration> option * visitor: Visitor * context: TransformationContext * ?nodesVisitor: NodesVisitor -> ResizeArray<ParameterDeclaration> option
        /// Resumes a suspended lexical environment and visits a function body, ending the lexical
        /// environment and merging hoisted declarations upon completion.
        abstract visitFunctionBody: node: FunctionBody * visitor: Visitor * context: TransformationContext -> FunctionBody
        /// Resumes a suspended lexical environment and visits a function body, ending the lexical
        /// environment and merging hoisted declarations upon completion.
        abstract visitFunctionBody: node: FunctionBody option * visitor: Visitor * context: TransformationContext -> FunctionBody option
        /// Resumes a suspended lexical environment and visits a concise body, ending the lexical
        /// environment and merging hoisted declarations upon completion.
        abstract visitFunctionBody: node: ConciseBody * visitor: Visitor * context: TransformationContext -> ConciseBody
        /// Visits an iteration body, adding any block-scoped variables required by the transformation.
        abstract visitIterationBody: body: Statement * visitor: Visitor * context: TransformationContext -> Statement
        /// <summary>Visits the elements of a <see cref="CommaListExpression" />.</summary>
        /// <param name="visitor">The visitor to use when visiting expressions whose result will not be discarded at runtime.</param>
        /// <param name="discardVisitor">The visitor to use when visiting expressions whose result will be discarded at runtime. Defaults to <see cref="visitor" />.</param>
        abstract visitCommaListElements: elements: ResizeArray<Expression> * visitor: Visitor * ?discardVisitor: Visitor -> ResizeArray<Expression>
        /// <summary>Visits each child of a Node using the supplied visitor, possibly returning a new Node of the same kind in its place.</summary>
        /// <param name="node">The Node whose children will be visited.</param>
        /// <param name="visitor">The callback used to visit each child.</param>
        /// <param name="context">A lexical environment context for the visitor.</param>
        abstract visitEachChild: node: 'T * visitor: Visitor * context: TransformationContext -> 'T when 'T :> Node
        /// <summary>Visits each child of a Node using the supplied visitor, possibly returning a new Node of the same kind in its place.</summary>
        /// <param name="node">The Node whose children will be visited.</param>
        /// <param name="visitor">The callback used to visit each child.</param>
        /// <param name="context">A lexical environment context for the visitor.</param>
        abstract visitEachChild: node: 'T option * visitor: Visitor * context: TransformationContext * ?nodesVisitor: obj * ?tokenVisitor: Visitor -> 'T option when 'T :> Node
        abstract getTsBuildInfoEmitOutputFilePath: options: CompilerOptions -> string option
        abstract getOutputFileNames: commandLine: ParsedCommandLine * inputFileName: string * ignoreCase: bool -> ResizeArray<string>
        abstract createPrinter: ?printerOptions: PrinterOptions * ?handlers: PrintHandlers -> Printer
        abstract findConfigFile: searchPath: string * fileExists: (string -> bool) * ?configName: string -> string option
        abstract resolveTripleslashReference: moduleName: string * containingFile: string -> string
        abstract createCompilerHost: options: CompilerOptions * ?setParentNodes: bool -> CompilerHost
        abstract getPreEmitDiagnostics: program: Program * ?sourceFile: SourceFile * ?cancellationToken: CancellationToken -> ResizeArray<Diagnostic>
        abstract formatDiagnostics: diagnostics: ResizeArray<Diagnostic> * host: FormatDiagnosticsHost -> string
        abstract formatDiagnostic: diagnostic: Diagnostic * host: FormatDiagnosticsHost -> string
        abstract formatDiagnosticsWithColorAndContext: diagnostics: ResizeArray<Diagnostic> * host: FormatDiagnosticsHost -> string
        abstract flattenDiagnosticMessageText: diag: U2<string, DiagnosticMessageChain> option * newLine: string * ?indent: float -> string
        /// Calculates the resulting resolution mode for some reference in some file - this is generally the explicitly
        /// provided resolution mode in the reference, unless one is not present, in which case it is the mode of the containing file.
        abstract getModeForFileReference: ref: U2<FileReference, string> * containingFileMode: ResolutionMode -> ResolutionMode
        /// <summary>
        /// Calculates the final resolution mode for an import at some index within a file's imports list. This is generally the explicitly
        /// defined mode of the import if provided, or, if not, the mode of the containing file (with some exceptions: import=require is always commonjs, dynamic import is always esm).
        /// If you have an actual import node, prefer using getModeForUsageLocation on the reference string node.
        /// </summary>
        /// <param name="file">File to fetch the resolution mode within</param>
        /// <param name="index">Index into the file's complete resolution list to get the resolution of - this is a concatenation of the file's imports and module augmentations</param>
        abstract getModeForResolutionAtIndex: file: SourceFile * index: float -> ResolutionMode
        /// <summary>
        /// Calculates the final resolution mode for a given module reference node. This is generally the explicitly provided resolution mode, if
        /// one exists, or the mode of the containing source file. (Excepting import=require, which is always commonjs, and dynamic import, which is always esm).
        /// Notably, this function always returns <c>undefined</c> if the containing file has an <c>undefined</c> <c>impliedNodeFormat</c> - this field is only set when
        /// <c>moduleResolution</c> is <c>node16</c>+.
        /// </summary>
        /// <param name="file">The file the import or import-like reference is contained within</param>
        /// <param name="usage">The module reference string</param>
        /// <returns>The final resolution mode of the import</returns>
        abstract getModeForUsageLocation: file: {| impliedNodeFormat: ResolutionMode option |} * usage: StringLiteralLike -> ModuleKind option
        abstract getConfigFileParsingDiagnostics: configFileParseResult: ParsedCommandLine -> ResizeArray<Diagnostic>
        /// <summary>
        /// A function for determining if a given file is esm or cjs format, assuming modern node module resolution rules, as configured by the
        /// <c>options</c> parameter.
        /// </summary>
        /// <param name="fileName">The normalized absolute path to check the format of (it need not exist on disk)</param>
        /// <param name="packageJsonInfoCache">A cache for package file lookups - it's best to have a cache when this function is called often</param>
        /// <param name="host">The ModuleResolutionHost which can perform the filesystem lookups for package json data</param>
        /// <param name="options">The compiler options to perform the analysis under - relevant options are <c>moduleResolution</c> and <c>traceResolution</c></param>
        /// <returns><c>undefined</c> if the path has no relevant implied format, <c>ModuleKind.ESNext</c> for esm format, and <c>ModuleKind.CommonJS</c> for cjs format</returns>
        abstract getImpliedNodeFormatForFile: fileName: Path * packageJsonInfoCache: PackageJsonInfoCache option * host: ModuleResolutionHost * options: CompilerOptions -> ResolutionMode
        /// <summary>
        /// Create a new 'Program' instance. A Program is an immutable collection of 'SourceFile's and a 'CompilerOptions'
        /// that represent a compilation unit.
        ///
        /// Creating a program proceeds from a set of root files, expanding the set of inputs by following imports and
        /// triple-slash-reference-path directives transitively. '@types' and triple-slash-reference-types are also pulled in.
        /// </summary>
        /// <param name="createProgramOptions">The options for creating a program.</param>
        /// <returns>A 'Program' object.</returns>
        abstract createProgram: createProgramOptions: CreateProgramOptions -> Program
        /// <summary>
        /// Create a new 'Program' instance. A Program is an immutable collection of 'SourceFile's and a 'CompilerOptions'
        /// that represent a compilation unit.
        ///
        /// Creating a program proceeds from a set of root files, expanding the set of inputs by following imports and
        /// triple-slash-reference-path directives transitively. '@types' and triple-slash-reference-types are also pulled in.
        /// </summary>
        /// <param name="rootNames">A set of root files.</param>
        /// <param name="options">The compiler options which should be used.</param>
        /// <param name="host">The host interacts with the underlying file system.</param>
        /// <param name="oldProgram">Reuses an old program structure.</param>
        /// <param name="configFileParsingDiagnostics">error during config file parsing</param>
        /// <returns>A 'Program' object.</returns>
        abstract createProgram: rootNames: ResizeArray<string> * options: CompilerOptions * ?host: CompilerHost * ?oldProgram: Program * ?configFileParsingDiagnostics: ResizeArray<Diagnostic> -> Program
        /// Returns the target config filename of a project reference.
        /// Note: The file might not exist.
        abstract resolveProjectReferencePath: ref: ProjectReference -> ResolvedConfigFileName
        /// Create the builder to manage semantic diagnostics and cache them
        abstract createSemanticDiagnosticsBuilderProgram: newProgram: Program * host: BuilderProgramHost * ?oldProgram: SemanticDiagnosticsBuilderProgram * ?configFileParsingDiagnostics: ResizeArray<Diagnostic> -> SemanticDiagnosticsBuilderProgram
        abstract createSemanticDiagnosticsBuilderProgram: rootNames: ResizeArray<string> option * options: CompilerOptions option * ?host: CompilerHost * ?oldProgram: SemanticDiagnosticsBuilderProgram * ?configFileParsingDiagnostics: ResizeArray<Diagnostic> * ?projectReferences: ResizeArray<ProjectReference> -> SemanticDiagnosticsBuilderProgram
        /// Create the builder that can handle the changes in program and iterate through changed files
        /// to emit the those files and manage semantic diagnostics cache as well
        abstract createEmitAndSemanticDiagnosticsBuilderProgram: newProgram: Program * host: BuilderProgramHost * ?oldProgram: EmitAndSemanticDiagnosticsBuilderProgram * ?configFileParsingDiagnostics: ResizeArray<Diagnostic> -> EmitAndSemanticDiagnosticsBuilderProgram
        abstract createEmitAndSemanticDiagnosticsBuilderProgram: rootNames: ResizeArray<string> option * options: CompilerOptions option * ?host: CompilerHost * ?oldProgram: EmitAndSemanticDiagnosticsBuilderProgram * ?configFileParsingDiagnostics: ResizeArray<Diagnostic> * ?projectReferences: ResizeArray<ProjectReference> -> EmitAndSemanticDiagnosticsBuilderProgram
        /// Creates a builder thats just abstraction over program and can be used with watch
        abstract createAbstractBuilder: newProgram: Program * host: BuilderProgramHost * ?oldProgram: BuilderProgram * ?configFileParsingDiagnostics: ResizeArray<Diagnostic> -> BuilderProgram
        abstract createAbstractBuilder: rootNames: ResizeArray<string> option * options: CompilerOptions option * ?host: CompilerHost * ?oldProgram: BuilderProgram * ?configFileParsingDiagnostics: ResizeArray<Diagnostic> * ?projectReferences: ResizeArray<ProjectReference> -> BuilderProgram
        abstract readBuilderProgram: compilerOptions: CompilerOptions * host: ReadBuildProgramHost -> EmitAndSemanticDiagnosticsBuilderProgram option
        abstract createIncrementalCompilerHost: options: CompilerOptions * ?system: System -> CompilerHost
        abstract createIncrementalProgram: p0: IncrementalProgramOptions<'T> -> 'T when 'T :> BuilderProgram
        /// Create the watch compiler host for either configFile or fileNames and its options
        abstract createWatchCompilerHost: configFileName: string * optionsToExtend: CompilerOptions option * system: System * ?createProgram: CreateProgram<'T> * ?reportDiagnostic: DiagnosticReporter * ?reportWatchStatus: WatchStatusReporter * ?watchOptionsToExtend: WatchOptions * ?extraFileExtensions: ResizeArray<FileExtensionInfo> -> WatchCompilerHostOfConfigFile<'T> when 'T :> BuilderProgram
        abstract createWatchCompilerHost: rootFiles: ResizeArray<string> * options: CompilerOptions * system: System * ?createProgram: CreateProgram<'T> * ?reportDiagnostic: DiagnosticReporter * ?reportWatchStatus: WatchStatusReporter * ?projectReferences: ResizeArray<ProjectReference> * ?watchOptions: WatchOptions -> WatchCompilerHostOfFilesAndCompilerOptions<'T> when 'T :> BuilderProgram
        /// Creates the watch from the host for root files and compiler options
        abstract createWatchProgram: host: WatchCompilerHostOfFilesAndCompilerOptions<'T> -> WatchOfFilesAndCompilerOptions<'T> when 'T :> BuilderProgram
        /// Creates the watch from the host for config file
        abstract createWatchProgram: host: WatchCompilerHostOfConfigFile<'T> -> WatchOfConfigFile<'T> when 'T :> BuilderProgram
        /// Create a function that reports watch status by writing to the system and handles the formating of the diagnostic
        abstract createBuilderStatusReporter: system: System * ?pretty: bool -> DiagnosticReporter
        abstract createSolutionBuilderHost: ?system: System * ?createProgram: CreateProgram<'T> * ?reportDiagnostic: DiagnosticReporter * ?reportSolutionBuilderStatus: DiagnosticReporter * ?reportErrorSummary: ReportEmitErrorSummary -> SolutionBuilderHost<'T> when 'T :> BuilderProgram
        abstract createSolutionBuilderWithWatchHost: ?system: System * ?createProgram: CreateProgram<'T> * ?reportDiagnostic: DiagnosticReporter * ?reportSolutionBuilderStatus: DiagnosticReporter * ?reportWatchStatus: WatchStatusReporter -> SolutionBuilderWithWatchHost<'T> when 'T :> BuilderProgram
        abstract createSolutionBuilder: host: SolutionBuilderHost<'T> * rootNames: ResizeArray<string> * defaultOptions: BuildOptions -> SolutionBuilder<'T> when 'T :> BuilderProgram
        abstract createSolutionBuilderWithWatch: host: SolutionBuilderWithWatchHost<'T> * rootNames: ResizeArray<string> * defaultOptions: BuildOptions * ?baseWatchOptions: WatchOptions -> SolutionBuilder<'T> when 'T :> BuilderProgram
        abstract getDefaultFormatCodeSettings: ?newLineCharacter: string -> FormatCodeSettings
        /// The classifier is used for syntactic highlighting in editors via the TSServer
        abstract createClassifier: unit -> Classifier
        abstract createDocumentRegistry: ?useCaseSensitiveFileNames: bool * ?currentDirectory: string -> DocumentRegistry
        abstract preProcessFile: sourceText: string * ?readImportFiles: bool * ?detectJavaScriptImports: bool -> PreProcessedFileInfo
        abstract transpileModule: input: string * transpileOptions: TranspileOptions -> TranspileOutput
        abstract transpile: input: string * ?compilerOptions: CompilerOptions * ?fileName: string * ?diagnostics: ResizeArray<Diagnostic> * ?moduleName: string -> string
        abstract toEditorSettings: options: U2<EditorOptions, EditorSettings> -> EditorSettings
        abstract displayPartsToString: displayParts: ResizeArray<SymbolDisplayPart> option -> string
        abstract getDefaultCompilerOptions: unit -> CompilerOptions
        abstract getSupportedCodeFixes: unit -> ResizeArray<string>
        abstract createLanguageServiceSourceFile: fileName: string * scriptSnapshot: IScriptSnapshot * scriptTargetOrOptions: U2<ScriptTarget, CreateSourceFileOptions> * version: string * setNodeParents: bool * ?scriptKind: ScriptKind -> SourceFile
        abstract updateLanguageServiceSourceFile: sourceFile: SourceFile * scriptSnapshot: IScriptSnapshot * version: string * textChangeRange: TextChangeRange option * ?aggressiveChecks: bool -> SourceFile
        abstract createLanguageService: host: LanguageServiceHost * ?documentRegistry: DocumentRegistry * ?syntaxOnlyOrLanguageServiceMode: U2<bool, LanguageServiceMode> -> LanguageService
        /// Get the path of the default library files (lib.d.ts) as distributed with the typescript
        /// node package.
        /// The functionality is not supported if the ts module is consumed outside of a node module.
        abstract getDefaultLibFilePath: options: CompilerOptions -> string
        /// The version of the language service API
        abstract servicesVersion: obj
        /// <summary>Transform one or more nodes using the supplied transformers.</summary>
        /// <param name="source">A single <c>Node</c> or an array of <c>Node</c> objects.</param>
        /// <param name="transformers">An array of <c>TransformerFactory</c> callbacks used to process the transformation.</param>
        /// <param name="compilerOptions">Optional compiler options.</param>
        abstract transform: source: U2<'T, ResizeArray<'T>> * transformers: ResizeArray<TransformerFactory<'T>> * ?compilerOptions: CompilerOptions -> TransformationResult<'T> when 'T :> Node

    /// <summary>
    /// Type of objects whose values are all of the same type.
    /// The <c>in</c> and <c>for-in</c> operators can *not* be safely used,
    /// since <c>Object.prototype</c> may be modified by outside code.
    /// </summary>
    type [<AllowNullLiteral>] MapLike<'T> =
        [<EmitIndexer>] abstract Item: index: string -> 'T with get, set

    type [<AllowNullLiteral>] SortedReadonlyArray<'T> =
        inherit ReadonlyArray<'T>
        abstract `` __sortedArrayBrand``: obj option with get, set

    type [<AllowNullLiteral>] SortedArray<'T> =
        inherit Array<'T>
        abstract `` __sortedArrayBrand``: obj option with get, set

    type [<AllowNullLiteral>] Path =
        interface end

    type MatchingKeys<'TRecord, 'TMatch> =
        interface end

    type [<AllowNullLiteral>] TextRange =
        abstract pos: float with get, set
        abstract ``end``: float with get, set

    type [<AllowNullLiteral>] ReadonlyTextRange =
        abstract pos: float
        abstract ``end``: float

    type [<RequireQualifiedAccess>] SyntaxKind =
        | Unknown = 0
        | EndOfFileToken = 1
        | SingleLineCommentTrivia = 2
        | MultiLineCommentTrivia = 3
        | NewLineTrivia = 4
        | WhitespaceTrivia = 5
        | ShebangTrivia = 6
        | ConflictMarkerTrivia = 7
        | NonTextFileMarkerTrivia = 8
        | NumericLiteral = 9
        | BigIntLiteral = 10
        | StringLiteral = 11
        | JsxText = 12
        | JsxTextAllWhiteSpaces = 13
        | RegularExpressionLiteral = 14
        | NoSubstitutionTemplateLiteral = 15
        | TemplateHead = 16
        | TemplateMiddle = 17
        | TemplateTail = 18
        | OpenBraceToken = 19
        | CloseBraceToken = 20
        | OpenParenToken = 21
        | CloseParenToken = 22
        | OpenBracketToken = 23
        | CloseBracketToken = 24
        | DotToken = 25
        | DotDotDotToken = 26
        | SemicolonToken = 27
        | CommaToken = 28
        | QuestionDotToken = 29
        | LessThanToken = 30
        | LessThanSlashToken = 31
        | GreaterThanToken = 32
        | LessThanEqualsToken = 33
        | GreaterThanEqualsToken = 34
        | EqualsEqualsToken = 35
        | ExclamationEqualsToken = 36
        | EqualsEqualsEqualsToken = 37
        | ExclamationEqualsEqualsToken = 38
        | EqualsGreaterThanToken = 39
        | PlusToken = 40
        | MinusToken = 41
        | AsteriskToken = 42
        | AsteriskAsteriskToken = 43
        | SlashToken = 44
        | PercentToken = 45
        | PlusPlusToken = 46
        | MinusMinusToken = 47
        | LessThanLessThanToken = 48
        | GreaterThanGreaterThanToken = 49
        | GreaterThanGreaterThanGreaterThanToken = 50
        | AmpersandToken = 51
        | BarToken = 52
        | CaretToken = 53
        | ExclamationToken = 54
        | TildeToken = 55
        | AmpersandAmpersandToken = 56
        | BarBarToken = 57
        | QuestionToken = 58
        | ColonToken = 59
        | AtToken = 60
        | QuestionQuestionToken = 61
        /// Only the JSDoc scanner produces BacktickToken. The normal scanner produces NoSubstitutionTemplateLiteral and related kinds.
        | BacktickToken = 62
        /// Only the JSDoc scanner produces HashToken. The normal scanner produces PrivateIdentifier.
        | HashToken = 63
        | EqualsToken = 64
        | PlusEqualsToken = 65
        | MinusEqualsToken = 66
        | AsteriskEqualsToken = 67
        | AsteriskAsteriskEqualsToken = 68
        | SlashEqualsToken = 69
        | PercentEqualsToken = 70
        | LessThanLessThanEqualsToken = 71
        | GreaterThanGreaterThanEqualsToken = 72
        | GreaterThanGreaterThanGreaterThanEqualsToken = 73
        | AmpersandEqualsToken = 74
        | BarEqualsToken = 75
        | BarBarEqualsToken = 76
        | AmpersandAmpersandEqualsToken = 77
        | QuestionQuestionEqualsToken = 78
        | CaretEqualsToken = 79
        | Identifier = 80
        | PrivateIdentifier = 81
        /// <summary>Only the special JSDoc comment text scanner produces JSDocCommentTextTokes. One of these tokens spans all text after a tag comment's start and before the next @</summary>
        | JSDocCommentTextToken = 82
        | BreakKeyword = 83
        | CaseKeyword = 84
        | CatchKeyword = 85
        | ClassKeyword = 86
        | ConstKeyword = 87
        | ContinueKeyword = 88
        | DebuggerKeyword = 89
        | DefaultKeyword = 90
        | DeleteKeyword = 91
        | DoKeyword = 92
        | ElseKeyword = 93
        | EnumKeyword = 94
        | ExportKeyword = 95
        | ExtendsKeyword = 96
        | FalseKeyword = 97
        | FinallyKeyword = 98
        | ForKeyword = 99
        | FunctionKeyword = 100
        | IfKeyword = 101
        | ImportKeyword = 102
        | InKeyword = 103
        | InstanceOfKeyword = 104
        | NewKeyword = 105
        | NullKeyword = 106
        | ReturnKeyword = 107
        | SuperKeyword = 108
        | SwitchKeyword = 109
        | ThisKeyword = 110
        | ThrowKeyword = 111
        | TrueKeyword = 112
        | TryKeyword = 113
        | TypeOfKeyword = 114
        | VarKeyword = 115
        | VoidKeyword = 116
        | WhileKeyword = 117
        | WithKeyword = 118
        | ImplementsKeyword = 119
        | InterfaceKeyword = 120
        | LetKeyword = 121
        | PackageKeyword = 122
        | PrivateKeyword = 123
        | ProtectedKeyword = 124
        | PublicKeyword = 125
        | StaticKeyword = 126
        | YieldKeyword = 127
        | AbstractKeyword = 128
        | AccessorKeyword = 129
        | AsKeyword = 130
        | AssertsKeyword = 131
        | AssertKeyword = 132
        | AnyKeyword = 133
        | AsyncKeyword = 134
        | AwaitKeyword = 135
        | BooleanKeyword = 136
        | ConstructorKeyword = 137
        | DeclareKeyword = 138
        | GetKeyword = 139
        | InferKeyword = 140
        | IntrinsicKeyword = 141
        | IsKeyword = 142
        | KeyOfKeyword = 143
        | ModuleKeyword = 144
        | NamespaceKeyword = 145
        | NeverKeyword = 146
        | OutKeyword = 147
        | ReadonlyKeyword = 148
        | RequireKeyword = 149
        | NumberKeyword = 150
        | ObjectKeyword = 151
        | SatisfiesKeyword = 152
        | SetKeyword = 153
        | StringKeyword = 154
        | SymbolKeyword = 155
        | TypeKeyword = 156
        | UndefinedKeyword = 157
        | UniqueKeyword = 158
        | UnknownKeyword = 159
        | UsingKeyword = 160
        | FromKeyword = 161
        | GlobalKeyword = 162
        | BigIntKeyword = 163
        | OverrideKeyword = 164
        | OfKeyword = 165
        | DeferKeyword = 166
        | QualifiedName = 167
        | ComputedPropertyName = 168
        | TypeParameter = 169
        | Parameter = 170
        | Decorator = 171
        | PropertySignature = 172
        | PropertyDeclaration = 173
        | MethodSignature = 174
        | MethodDeclaration = 175
        | ClassStaticBlockDeclaration = 176
        | Constructor = 177
        | GetAccessor = 178
        | SetAccessor = 179
        | CallSignature = 180
        | ConstructSignature = 181
        | IndexSignature = 182
        | TypePredicate = 183
        | TypeReference = 184
        | FunctionType = 185
        | ConstructorType = 186
        | TypeQuery = 187
        | TypeLiteral = 188
        | ArrayType = 189
        | TupleType = 190
        | OptionalType = 191
        | RestType = 192
        | UnionType = 193
        | IntersectionType = 194
        | ConditionalType = 195
        | InferType = 196
        | ParenthesizedType = 197
        | ThisType = 198
        | TypeOperator = 199
        | IndexedAccessType = 200
        | MappedType = 201
        | LiteralType = 202
        | NamedTupleMember = 203
        | TemplateLiteralType = 204
        | TemplateLiteralTypeSpan = 205
        | ImportType = 206
        | ObjectBindingPattern = 207
        | ArrayBindingPattern = 208
        | BindingElement = 209
        | ArrayLiteralExpression = 210
        | ObjectLiteralExpression = 211
        | PropertyAccessExpression = 212
        | ElementAccessExpression = 213
        | CallExpression = 214
        | NewExpression = 215
        | TaggedTemplateExpression = 216
        | TypeAssertionExpression = 217
        | ParenthesizedExpression = 218
        | FunctionExpression = 219
        | ArrowFunction = 220
        | DeleteExpression = 221
        | TypeOfExpression = 222
        | VoidExpression = 223
        | AwaitExpression = 224
        | PrefixUnaryExpression = 225
        | PostfixUnaryExpression = 226
        | BinaryExpression = 227
        | ConditionalExpression = 228
        | TemplateExpression = 229
        | YieldExpression = 230
        | SpreadElement = 231
        | ClassExpression = 232
        | OmittedExpression = 233
        | ExpressionWithTypeArguments = 234
        | AsExpression = 235
        | NonNullExpression = 236
        | MetaProperty = 237
        | SyntheticExpression = 238
        | SatisfiesExpression = 239
        | TemplateSpan = 240
        | SemicolonClassElement = 241
        | Block = 242
        | EmptyStatement = 243
        | VariableStatement = 244
        | ExpressionStatement = 245
        | IfStatement = 246
        | DoStatement = 247
        | WhileStatement = 248
        | ForStatement = 249
        | ForInStatement = 250
        | ForOfStatement = 251
        | ContinueStatement = 252
        | BreakStatement = 253
        | ReturnStatement = 254
        | WithStatement = 255
        | SwitchStatement = 256
        | LabeledStatement = 257
        | ThrowStatement = 258
        | TryStatement = 259
        | DebuggerStatement = 260
        | VariableDeclaration = 261
        | VariableDeclarationList = 262
        | FunctionDeclaration = 263
        | ClassDeclaration = 264
        | InterfaceDeclaration = 265
        | TypeAliasDeclaration = 266
        | EnumDeclaration = 267
        | ModuleDeclaration = 268
        | ModuleBlock = 269
        | CaseBlock = 270
        | NamespaceExportDeclaration = 271
        | ImportEqualsDeclaration = 272
        | ImportDeclaration = 273
        | ImportClause = 274
        | NamespaceImport = 275
        | NamedImports = 276
        | ImportSpecifier = 277
        | ExportAssignment = 278
        | ExportDeclaration = 279
        | NamedExports = 280
        | NamespaceExport = 281
        | ExportSpecifier = 282
        | MissingDeclaration = 283
        | ExternalModuleReference = 284
        | JsxElement = 285
        | JsxSelfClosingElement = 286
        | JsxOpeningElement = 287
        | JsxClosingElement = 288
        | JsxFragment = 289
        | JsxOpeningFragment = 290
        | JsxClosingFragment = 291
        | JsxAttribute = 292
        | JsxAttributes = 293
        | JsxSpreadAttribute = 294
        | JsxExpression = 295
        | JsxNamespacedName = 296
        | CaseClause = 297
        | DefaultClause = 298
        | HeritageClause = 299
        | CatchClause = 300
        | ImportAttributes = 301
        | ImportAttribute = 302
        /// <deprecated />
        | AssertClause = 301
        /// <deprecated />
        | AssertEntry = 302
        /// <deprecated />
        | ImportTypeAssertionContainer = 303
        | PropertyAssignment = 304
        | ShorthandPropertyAssignment = 305
        | SpreadAssignment = 306
        | EnumMember = 307
        | SourceFile = 308
        | Bundle = 309
        | JSDocTypeExpression = 310
        | JSDocNameReference = 311
        | JSDocMemberName = 312
        | JSDocAllType = 313
        | JSDocUnknownType = 314
        | JSDocNullableType = 315
        | JSDocNonNullableType = 316
        | JSDocOptionalType = 317
        | JSDocFunctionType = 318
        | JSDocVariadicType = 319
        | JSDocNamepathType = 320
        | JSDoc = 321
        /// <deprecated>Use SyntaxKind.JSDoc</deprecated>
        | JSDocComment = 321
        | JSDocText = 322
        | JSDocTypeLiteral = 323
        | JSDocSignature = 324
        | JSDocLink = 325
        | JSDocLinkCode = 326
        | JSDocLinkPlain = 327
        | JSDocTag = 328
        | JSDocAugmentsTag = 329
        | JSDocImplementsTag = 330
        | JSDocAuthorTag = 331
        | JSDocDeprecatedTag = 332
        | JSDocClassTag = 333
        | JSDocPublicTag = 334
        | JSDocPrivateTag = 335
        | JSDocProtectedTag = 336
        | JSDocReadonlyTag = 337
        | JSDocOverrideTag = 338
        | JSDocCallbackTag = 339
        | JSDocOverloadTag = 340
        | JSDocEnumTag = 341
        | JSDocParameterTag = 342
        | JSDocReturnTag = 343
        | JSDocThisTag = 344
        | JSDocTypeTag = 345
        | JSDocTemplateTag = 346
        | JSDocTypedefTag = 347
        | JSDocSeeTag = 348
        | JSDocPropertyTag = 349
        | JSDocThrowsTag = 350
        | JSDocSatisfiesTag = 351
        | JSDocImportTag = 352
        | SyntaxList = 353
        | NotEmittedStatement = 354
        | NotEmittedTypeElement = 355
        | PartiallyEmittedExpression = 356
        | CommaListExpression = 357
        | SyntheticReferenceExpression = 358
        | Count = 359
        | FirstAssignment = 64
        | LastAssignment = 79
        | FirstCompoundAssignment = 65
        | LastCompoundAssignment = 79
        | FirstReservedWord = 83
        | LastReservedWord = 118
        | FirstKeyword = 83
        | LastKeyword = 166
        | FirstFutureReservedWord = 119
        | LastFutureReservedWord = 127
        | FirstTypeNode = 183
        | LastTypeNode = 206
        | FirstPunctuation = 19
        | LastPunctuation = 79
        | FirstToken = 0
        | LastToken = 166
        | FirstTriviaToken = 2
        | LastTriviaToken = 7
        | FirstLiteralToken = 9
        | LastLiteralToken = 15
        | FirstTemplateToken = 15
        | LastTemplateToken = 18
        | FirstBinaryOperator = 30
        | LastBinaryOperator = 79
        | FirstStatement = 244
        | LastStatement = 260
        | FirstNode = 167
        | FirstJSDocNode = 310
        | LastJSDocNode = 352
        | FirstJSDocTagNode = 328
        | LastJSDocTagNode = 352
        | FirstContextualKeyword = 128
        | LastContextualKeyword = 166

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.SingleLineCommentTrivia
    ///     | SyntaxKind.MultiLineCommentTrivia
    ///     | SyntaxKind.NewLineTrivia
    ///     | SyntaxKind.WhitespaceTrivia
    ///     | SyntaxKind.ShebangTrivia
    ///     | SyntaxKind.ConflictMarkerTrivia
    /// </code>
    /// </remarks>
    type TriviaSyntaxKind =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.NumericLiteral
    ///     | SyntaxKind.BigIntLiteral
    ///     | SyntaxKind.StringLiteral
    ///     | SyntaxKind.JsxText
    ///     | SyntaxKind.JsxTextAllWhiteSpaces
    ///     | SyntaxKind.RegularExpressionLiteral
    ///     | SyntaxKind.NoSubstitutionTemplateLiteral
    /// </code>
    /// </remarks>
    type LiteralSyntaxKind =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.TemplateHead
    ///     | SyntaxKind.TemplateMiddle
    ///     | SyntaxKind.TemplateTail
    /// </code>
    /// </remarks>
    type PseudoLiteralSyntaxKind =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.OpenBraceToken
    ///     | SyntaxKind.CloseBraceToken
    ///     | SyntaxKind.OpenParenToken
    ///     | SyntaxKind.CloseParenToken
    ///     | SyntaxKind.OpenBracketToken
    ///     | SyntaxKind.CloseBracketToken
    ///     | SyntaxKind.DotToken
    ///     | SyntaxKind.DotDotDotToken
    ///     | SyntaxKind.SemicolonToken
    ///     | SyntaxKind.CommaToken
    ///     | SyntaxKind.QuestionDotToken
    ///     | SyntaxKind.LessThanToken
    ///     | SyntaxKind.LessThanSlashToken
    ///     | SyntaxKind.GreaterThanToken
    ///     | SyntaxKind.LessThanEqualsToken
    ///     | SyntaxKind.GreaterThanEqualsToken
    ///     | SyntaxKind.EqualsEqualsToken
    ///     | SyntaxKind.ExclamationEqualsToken
    ///     | SyntaxKind.EqualsEqualsEqualsToken
    ///     | SyntaxKind.ExclamationEqualsEqualsToken
    ///     | SyntaxKind.EqualsGreaterThanToken
    ///     | SyntaxKind.PlusToken
    ///     | SyntaxKind.MinusToken
    ///     | SyntaxKind.AsteriskToken
    ///     | SyntaxKind.AsteriskAsteriskToken
    ///     | SyntaxKind.SlashToken
    ///     | SyntaxKind.PercentToken
    ///     | SyntaxKind.PlusPlusToken
    ///     | SyntaxKind.MinusMinusToken
    ///     | SyntaxKind.LessThanLessThanToken
    ///     | SyntaxKind.GreaterThanGreaterThanToken
    ///     | SyntaxKind.GreaterThanGreaterThanGreaterThanToken
    ///     | SyntaxKind.AmpersandToken
    ///     | SyntaxKind.BarToken
    ///     | SyntaxKind.CaretToken
    ///     | SyntaxKind.ExclamationToken
    ///     | SyntaxKind.TildeToken
    ///     | SyntaxKind.AmpersandAmpersandToken
    ///     | SyntaxKind.AmpersandAmpersandEqualsToken
    ///     | SyntaxKind.BarBarToken
    ///     | SyntaxKind.BarBarEqualsToken
    ///     | SyntaxKind.QuestionQuestionToken
    ///     | SyntaxKind.QuestionQuestionEqualsToken
    ///     | SyntaxKind.QuestionToken
    ///     | SyntaxKind.ColonToken
    ///     | SyntaxKind.AtToken
    ///     | SyntaxKind.BacktickToken
    ///     | SyntaxKind.HashToken
    ///     | SyntaxKind.EqualsToken
    ///     | SyntaxKind.PlusEqualsToken
    ///     | SyntaxKind.MinusEqualsToken
    ///     | SyntaxKind.AsteriskEqualsToken
    ///     | SyntaxKind.AsteriskAsteriskEqualsToken
    ///     | SyntaxKind.SlashEqualsToken
    ///     | SyntaxKind.PercentEqualsToken
    ///     | SyntaxKind.LessThanLessThanEqualsToken
    ///     | SyntaxKind.GreaterThanGreaterThanEqualsToken
    ///     | SyntaxKind.GreaterThanGreaterThanGreaterThanEqualsToken
    ///     | SyntaxKind.AmpersandEqualsToken
    ///     | SyntaxKind.BarEqualsToken
    ///     | SyntaxKind.CaretEqualsToken
    /// </code>
    /// </remarks>
    type PunctuationSyntaxKind =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// PunctuationSyntaxKind | KeywordSyntaxKind
    /// </code>
    /// </remarks>
    type PunctuationOrKeywordSyntaxKind =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.AbstractKeyword
    ///     | SyntaxKind.AccessorKeyword
    ///     | SyntaxKind.AnyKeyword
    ///     | SyntaxKind.AsKeyword
    ///     | SyntaxKind.AssertsKeyword
    ///     | SyntaxKind.AssertKeyword
    ///     | SyntaxKind.AsyncKeyword
    ///     | SyntaxKind.AwaitKeyword
    ///     | SyntaxKind.BigIntKeyword
    ///     | SyntaxKind.BooleanKeyword
    ///     | SyntaxKind.BreakKeyword
    ///     | SyntaxKind.CaseKeyword
    ///     | SyntaxKind.CatchKeyword
    ///     | SyntaxKind.ClassKeyword
    ///     | SyntaxKind.ConstKeyword
    ///     | SyntaxKind.ConstructorKeyword
    ///     | SyntaxKind.ContinueKeyword
    ///     | SyntaxKind.DebuggerKeyword
    ///     | SyntaxKind.DeclareKeyword
    ///     | SyntaxKind.DefaultKeyword
    ///     | SyntaxKind.DeferKeyword
    ///     | SyntaxKind.DeleteKeyword
    ///     | SyntaxKind.DoKeyword
    ///     | SyntaxKind.ElseKeyword
    ///     | SyntaxKind.EnumKeyword
    ///     | SyntaxKind.ExportKeyword
    ///     | SyntaxKind.ExtendsKeyword
    ///     | SyntaxKind.FalseKeyword
    ///     | SyntaxKind.FinallyKeyword
    ///     | SyntaxKind.ForKeyword
    ///     | SyntaxKind.FromKeyword
    ///     | SyntaxKind.FunctionKeyword
    ///     | SyntaxKind.GetKeyword
    ///     | SyntaxKind.GlobalKeyword
    ///     | SyntaxKind.IfKeyword
    ///     | SyntaxKind.ImplementsKeyword
    ///     | SyntaxKind.ImportKeyword
    ///     | SyntaxKind.InferKeyword
    ///     | SyntaxKind.InKeyword
    ///     | SyntaxKind.InstanceOfKeyword
    ///     | SyntaxKind.InterfaceKeyword
    ///     | SyntaxKind.IntrinsicKeyword
    ///     | SyntaxKind.IsKeyword
    ///     | SyntaxKind.KeyOfKeyword
    ///     | SyntaxKind.LetKeyword
    ///     | SyntaxKind.ModuleKeyword
    ///     | SyntaxKind.NamespaceKeyword
    ///     | SyntaxKind.NeverKeyword
    ///     | SyntaxKind.NewKeyword
    ///     | SyntaxKind.NullKeyword
    ///     | SyntaxKind.NumberKeyword
    ///     | SyntaxKind.ObjectKeyword
    ///     | SyntaxKind.OfKeyword
    ///     | SyntaxKind.PackageKeyword
    ///     | SyntaxKind.PrivateKeyword
    ///     | SyntaxKind.ProtectedKeyword
    ///     | SyntaxKind.PublicKeyword
    ///     | SyntaxKind.ReadonlyKeyword
    ///     | SyntaxKind.OutKeyword
    ///     | SyntaxKind.OverrideKeyword
    ///     | SyntaxKind.RequireKeyword
    ///     | SyntaxKind.ReturnKeyword
    ///     | SyntaxKind.SatisfiesKeyword
    ///     | SyntaxKind.SetKeyword
    ///     | SyntaxKind.StaticKeyword
    ///     | SyntaxKind.StringKeyword
    ///     | SyntaxKind.SuperKeyword
    ///     | SyntaxKind.SwitchKeyword
    ///     | SyntaxKind.SymbolKeyword
    ///     | SyntaxKind.ThisKeyword
    ///     | SyntaxKind.ThrowKeyword
    ///     | SyntaxKind.TrueKeyword
    ///     | SyntaxKind.TryKeyword
    ///     | SyntaxKind.TypeKeyword
    ///     | SyntaxKind.TypeOfKeyword
    ///     | SyntaxKind.UndefinedKeyword
    ///     | SyntaxKind.UniqueKeyword
    ///     | SyntaxKind.UnknownKeyword
    ///     | SyntaxKind.UsingKeyword
    ///     | SyntaxKind.VarKeyword
    ///     | SyntaxKind.VoidKeyword
    ///     | SyntaxKind.WhileKeyword
    ///     | SyntaxKind.WithKeyword
    ///     | SyntaxKind.YieldKeyword
    /// </code>
    /// </remarks>
    type KeywordSyntaxKind =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.AbstractKeyword
    ///     | SyntaxKind.AccessorKeyword
    ///     | SyntaxKind.AsyncKeyword
    ///     | SyntaxKind.ConstKeyword
    ///     | SyntaxKind.DeclareKeyword
    ///     | SyntaxKind.DefaultKeyword
    ///     | SyntaxKind.ExportKeyword
    ///     | SyntaxKind.InKeyword
    ///     | SyntaxKind.PrivateKeyword
    ///     | SyntaxKind.ProtectedKeyword
    ///     | SyntaxKind.PublicKeyword
    ///     | SyntaxKind.ReadonlyKeyword
    ///     | SyntaxKind.OutKeyword
    ///     | SyntaxKind.OverrideKeyword
    ///     | SyntaxKind.StaticKeyword
    /// </code>
    /// </remarks>
    type ModifierSyntaxKind =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.AnyKeyword
    ///     | SyntaxKind.BigIntKeyword
    ///     | SyntaxKind.BooleanKeyword
    ///     | SyntaxKind.IntrinsicKeyword
    ///     | SyntaxKind.NeverKeyword
    ///     | SyntaxKind.NumberKeyword
    ///     | SyntaxKind.ObjectKeyword
    ///     | SyntaxKind.StringKeyword
    ///     | SyntaxKind.SymbolKeyword
    ///     | SyntaxKind.UndefinedKeyword
    ///     | SyntaxKind.UnknownKeyword
    ///     | SyntaxKind.VoidKeyword
    /// </code>
    /// </remarks>
    type KeywordTypeSyntaxKind =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | KeywordTypeSyntaxKind
    ///     | SyntaxKind.TypePredicate
    ///     | SyntaxKind.TypeReference
    ///     | SyntaxKind.FunctionType
    ///     | SyntaxKind.ConstructorType
    ///     | SyntaxKind.TypeQuery
    ///     | SyntaxKind.TypeLiteral
    ///     | SyntaxKind.ArrayType
    ///     | SyntaxKind.TupleType
    ///     | SyntaxKind.NamedTupleMember
    ///     | SyntaxKind.OptionalType
    ///     | SyntaxKind.RestType
    ///     | SyntaxKind.UnionType
    ///     | SyntaxKind.IntersectionType
    ///     | SyntaxKind.ConditionalType
    ///     | SyntaxKind.InferType
    ///     | SyntaxKind.ParenthesizedType
    ///     | SyntaxKind.ThisType
    ///     | SyntaxKind.TypeOperator
    ///     | SyntaxKind.IndexedAccessType
    ///     | SyntaxKind.MappedType
    ///     | SyntaxKind.LiteralType
    ///     | SyntaxKind.TemplateLiteralType
    ///     | SyntaxKind.TemplateLiteralTypeSpan
    ///     | SyntaxKind.ImportType
    ///     | SyntaxKind.ExpressionWithTypeArguments
    ///     | SyntaxKind.JSDocTypeExpression
    ///     | SyntaxKind.JSDocAllType
    ///     | SyntaxKind.JSDocUnknownType
    ///     | SyntaxKind.JSDocNonNullableType
    ///     | SyntaxKind.JSDocNullableType
    ///     | SyntaxKind.JSDocOptionalType
    ///     | SyntaxKind.JSDocFunctionType
    ///     | SyntaxKind.JSDocVariadicType
    ///     | SyntaxKind.JSDocNamepathType
    ///     | SyntaxKind.JSDocSignature
    ///     | SyntaxKind.JSDocTypeLiteral
    /// </code>
    /// </remarks>
    type TypeNodeSyntaxKind =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.Unknown
    ///     | SyntaxKind.EndOfFileToken
    ///     | TriviaSyntaxKind
    ///     | LiteralSyntaxKind
    ///     | PseudoLiteralSyntaxKind
    ///     | PunctuationSyntaxKind
    ///     | SyntaxKind.Identifier
    ///     | KeywordSyntaxKind
    /// </code>
    /// </remarks>
    type TokenSyntaxKind =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.LessThanSlashToken
    ///     | SyntaxKind.EndOfFileToken
    ///     | SyntaxKind.ConflictMarkerTrivia
    ///     | SyntaxKind.JsxText
    ///     | SyntaxKind.JsxTextAllWhiteSpaces
    ///     | SyntaxKind.OpenBraceToken
    ///     | SyntaxKind.LessThanToken
    /// </code>
    /// </remarks>
    type JsxTokenSyntaxKind =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.EndOfFileToken
    ///     | SyntaxKind.WhitespaceTrivia
    ///     | SyntaxKind.AtToken
    ///     | SyntaxKind.NewLineTrivia
    ///     | SyntaxKind.AsteriskToken
    ///     | SyntaxKind.OpenBraceToken
    ///     | SyntaxKind.CloseBraceToken
    ///     | SyntaxKind.LessThanToken
    ///     | SyntaxKind.GreaterThanToken
    ///     | SyntaxKind.OpenBracketToken
    ///     | SyntaxKind.CloseBracketToken
    ///     | SyntaxKind.OpenParenToken
    ///     | SyntaxKind.CloseParenToken
    ///     | SyntaxKind.EqualsToken
    ///     | SyntaxKind.CommaToken
    ///     | SyntaxKind.DotToken
    ///     | SyntaxKind.Identifier
    ///     | SyntaxKind.BacktickToken
    ///     | SyntaxKind.HashToken
    ///     | SyntaxKind.Unknown
    ///     | KeywordSyntaxKind
    /// </code>
    /// </remarks>
    type JSDocSyntaxKind =
        SyntaxKind

    type [<RequireQualifiedAccess>] NodeFlags =
        | None = 0
        | Let = 1
        | Const = 2
        | Using = 4
        | AwaitUsing = 6
        | NestedNamespace = 8
        | Synthesized = 16
        | Namespace = 32
        | OptionalChain = 64
        | ExportContext = 128
        | ContainsThis = 256
        | HasImplicitReturn = 512
        | HasExplicitReturn = 1024
        | GlobalAugmentation = 2048
        | HasAsyncFunctions = 4096
        | DisallowInContext = 8192
        | YieldContext = 16384
        | DecoratorContext = 32768
        | AwaitContext = 65536
        | DisallowConditionalTypesContext = 131072
        | ThisNodeHasError = 262144
        | JavaScriptFile = 524288
        | ThisNodeOrAnySubNodesHasError = 1048576
        | HasAggregatedChildData = 2097152
        | PossiblyContainsDynamicImport = 4194304
        | PossiblyContainsImportMeta = 8388608
        | JSDoc = 16777216
        | Ambient = 33554432
        | InWithStatement = 67108864
        | JsonFile = 134217728
        | TypeCached = 268435456
        | Deprecated = 536870912
        | BlockScoped = 7
        | Constant = 6
        | ReachabilityCheckFlags = 1536
        | ReachabilityAndEmitFlags = 5632
        | ContextFlags = 101441536
        | TypeExcludesFlags = 81920
        | PermanentlySetIncrementalFlags = 12582912
        | IdentifierHasExtendedUnicodeEscape = 256
        | IdentifierIsInJSDocNamespace = 4096

    type [<RequireQualifiedAccess>] ModifierFlags =
        | None = 0
        | Public = 1
        | Private = 2
        | Protected = 4
        | Readonly = 8
        | Override = 16
        | Export = 32
        | Abstract = 64
        | Ambient = 128
        | Static = 256
        | Accessor = 512
        | Async = 1024
        | Default = 2048
        | Const = 4096
        | In = 8192
        | Out = 16384
        | Decorator = 32768
        | Deprecated = 65536
        | JSDocPublic = 8388608
        | JSDocPrivate = 16777216
        | JSDocProtected = 33554432
        | JSDocReadonly = 67108864
        | JSDocOverride = 134217728
        | SyntacticOrJSDocModifiers = 31
        | SyntacticOnlyModifiers = 65504
        | SyntacticModifiers = 65535
        | JSDocCacheOnlyModifiers = 260046848
        | JSDocOnlyModifiers = 65536
        | NonCacheOnlyModifiers = 131071
        | HasComputedJSDocModifiers = 268435456
        | HasComputedFlags = 536870912
        | AccessibilityModifier = 7
        | ParameterPropertyModifier = 31
        | NonPublicAccessibilityModifier = 6
        | TypeScriptModifier = 28895
        | ExportDefault = 2080
        | All = 131071
        | Modifier = 98303

    type [<RequireQualifiedAccess>] JsxFlags =
        | None = 0
        /// An element from a named property of the JSX.IntrinsicElements interface
        | IntrinsicNamedElement = 1
        /// An element inferred from the string index signature of the JSX.IntrinsicElements interface
        | IntrinsicIndexedElement = 2
        | IntrinsicElement = 3

    type [<RequireQualifiedAccess>] RelationComparisonResult =
        | None = 0
        | Succeeded = 1
        | Failed = 2
        | ReportsUnmeasurable = 8
        | ReportsUnreliable = 16
        | ReportsMask = 24
        | ComplexityOverflow = 32
        | StackDepthOverflow = 64
        | Overflow = 96

    type [<RequireQualifiedAccess>] PredicateSemantics =
        | None = 0
        | Always = 1
        | Never = 2
        | Sometimes = 3

    type NodeId =
        float

    type [<AllowNullLiteral>] Node =
        inherit ReadonlyTextRange
        abstract kind: SyntaxKind
        abstract flags: NodeFlags
        abstract modifierFlagsCache: ModifierFlags with get, set
        abstract transformFlags: TransformFlags
        abstract id: NodeId option with get, set
        abstract parent: Node
        abstract getSourceFile: unit -> SourceFile
        abstract getChildCount: ?sourceFile: SourceFile -> float
        abstract getChildAt: index: float * ?sourceFile: SourceFile -> Node
        abstract getChildren: ?sourceFile: SourceFile -> ResizeArray<Node>
        abstract getStart: ?sourceFile: SourceFile * ?includeJsDocComment: bool -> float
        abstract getFullStart: unit -> float
        abstract getEnd: unit -> float
        abstract getWidth: ?sourceFile: SourceFileLike -> float
        abstract getFullWidth: unit -> float
        abstract getLeadingTriviaWidth: ?sourceFile: SourceFile -> float
        abstract getFullText: ?sourceFile: SourceFile -> string
        abstract getText: ?sourceFile: SourceFile -> string
        abstract getFirstToken: ?sourceFile: SourceFile -> Node option
        abstract getLastToken: ?sourceFile: SourceFile -> Node option
        abstract forEachChild: cbNode: (Node -> 'T option) * ?cbNodeArray: (ResizeArray<Node> -> 'T option) -> 'T option

    type [<AllowNullLiteral>] JSDocContainer =
        inherit Node
        abstract _jsdocContainerBrand: obj option with get, set
        abstract jsDoc: JSDocArray option with get, set

    type [<AllowNullLiteral>] JSDocArray =
        inherit Array<JSDoc>
        abstract jsDocCache: ResizeArray<JSDocTag> option with get, set

    type [<AllowNullLiteral>] LocalsContainer =
        inherit Node
        abstract _localsContainerBrand: obj option with get, set
        abstract locals: SymbolTable option with get, set
        abstract nextContainer: HasLocals option with get, set

    type [<AllowNullLiteral>] FlowContainer =
        inherit Node
        abstract _flowContainerBrand: obj option with get, set
        abstract flowNode: FlowNode option with get, set

    type HasFlowNode =
        obj

    type ForEachChildNodes =
        obj

    type HasChildren =
        obj

    type HasJSDoc =
        obj

    type HasType =
        obj

    type HasIllegalType =
        U2<ConstructorDeclaration, SetAccessorDeclaration>

    type HasIllegalTypeParameters =
        U3<ConstructorDeclaration, SetAccessorDeclaration, GetAccessorDeclaration>

    type HasTypeArguments =
        U5<CallExpression, NewExpression, TaggedTemplateExpression, JsxOpeningElement, JsxSelfClosingElement>

    type HasInitializer =
        U5<HasExpressionInitializer, ForStatement, ForInStatement, ForOfStatement, JsxAttribute>

    type HasExpressionInitializer =
        U6<VariableDeclaration, ParameterDeclaration, BindingElement, PropertyDeclaration, PropertyAssignment, EnumMember>

    type HasIllegalExpressionInitializer =
        PropertySignature

    type HasDecorators =
        U7<ParameterDeclaration, PropertyDeclaration, MethodDeclaration, GetAccessorDeclaration, SetAccessorDeclaration, ClassExpression, ClassDeclaration>

    type HasIllegalDecorators =
        obj

    type HasModifiers =
        obj

    type HasIllegalModifiers =
        U5<ClassStaticBlockDeclaration, PropertyAssignment, ShorthandPropertyAssignment, MissingDeclaration, NamespaceExportDeclaration>

    type PrimitiveLiteral =
        U6<BooleanLiteral, NumericLiteral, StringLiteral, NoSubstitutionTemplateLiteral, BigIntLiteral, obj>

    /// <summary>Declarations that can contain other declarations. Corresponds with <c>ContainerFlags.IsContainer</c> in binder.ts.</summary>
    type IsContainer =
        obj

    /// <summary>Nodes that introduce a new block scope. Corresponds with <c>ContainerFlags.IsBlockScopedContainer</c> in binder.ts.</summary>
    type IsBlockScopedContainer =
        U7<IsContainer, CatchClause, ForStatement, ForInStatement, ForOfStatement, CaseBlock, Block>

    /// <summary>Corresponds with <c>ContainerFlags.IsControlFlowContainer</c> in binder.ts.</summary>
    type IsControlFlowContainer =
        obj

    /// <summary>Corresponds with <c>ContainerFlags.IsFunctionLike</c> in binder.ts.</summary>
    type IsFunctionLike =
        obj

    /// <summary>Corresponds with <c>ContainerFlags.IsFunctionExpression</c> in binder.ts.</summary>
    type IsFunctionExpression =
        U2<FunctionExpression, ArrowFunction>

    /// <summary>
    /// Nodes that can have local symbols. Corresponds with <c>ContainerFlags.HasLocals</c>. Constituents should extend
    /// <see cref="LocalsContainer" />.
    /// </summary>
    type HasLocals =
        obj

    /// <summary>Corresponds with <c>ContainerFlags.IsInterface</c> in binder.ts.</summary>
    type IsInterface =
        InterfaceDeclaration

    /// <summary>Corresponds with <c>ContainerFlags.IsObjectLiteralOrClassExpressionMethodOrAccessor</c> in binder.ts.</summary>
    type IsObjectLiteralOrClassExpressionMethodOrAccessor =
        U3<GetAccessorDeclaration, SetAccessorDeclaration, MethodDeclaration>

    /// <summary>Corresponds with <c>ContainerFlags</c> in binder.ts.</summary>
    type HasContainerFlags =
        U8<IsContainer, IsBlockScopedContainer, IsControlFlowContainer, IsFunctionLike, IsFunctionExpression, HasLocals, IsInterface, IsObjectLiteralOrClassExpressionMethodOrAccessor>

    type [<AllowNullLiteral>] MutableNodeArray<'T when 'T :> Node> =
        inherit Array<'T>
        inherit TextRange
        abstract hasTrailingComma: bool with get, set
        abstract transformFlags: TransformFlags with get, set

    type [<AllowNullLiteral>] NodeArray<'T when 'T :> Node> =
        inherit ReadonlyArray<'T>
        inherit ReadonlyTextRange
        abstract hasTrailingComma: bool
        abstract transformFlags: TransformFlags with get, set

    type [<AllowNullLiteral>] Token<'TKind> =
        inherit Node
        abstract kind: 'TKind

    type [<AllowNullLiteral>] EndOfFileToken =
        interface end

    type [<AllowNullLiteral>] PunctuationToken<'TKind> =
        inherit Token<'TKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// PunctuationToken&lt;SyntaxKind.DotToken&gt;
    /// </code>
    /// </remarks>
    type DotToken =
        PunctuationToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// PunctuationToken&lt;SyntaxKind.DotDotDotToken&gt;
    /// </code>
    /// </remarks>
    type DotDotDotToken =
        PunctuationToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// PunctuationToken&lt;SyntaxKind.QuestionToken&gt;
    /// </code>
    /// </remarks>
    type QuestionToken =
        PunctuationToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// PunctuationToken&lt;SyntaxKind.ExclamationToken&gt;
    /// </code>
    /// </remarks>
    type ExclamationToken =
        PunctuationToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// PunctuationToken&lt;SyntaxKind.ColonToken&gt;
    /// </code>
    /// </remarks>
    type ColonToken =
        PunctuationToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// PunctuationToken&lt;SyntaxKind.EqualsToken&gt;
    /// </code>
    /// </remarks>
    type EqualsToken =
        PunctuationToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// PunctuationToken&lt;SyntaxKind.AmpersandAmpersandEqualsToken&gt;
    /// </code>
    /// </remarks>
    type AmpersandAmpersandEqualsToken =
        PunctuationToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// PunctuationToken&lt;SyntaxKind.BarBarEqualsToken&gt;
    /// </code>
    /// </remarks>
    type BarBarEqualsToken =
        PunctuationToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// PunctuationToken&lt;SyntaxKind.QuestionQuestionEqualsToken&gt;
    /// </code>
    /// </remarks>
    type QuestionQuestionEqualsToken =
        PunctuationToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// PunctuationToken&lt;SyntaxKind.AsteriskToken&gt;
    /// </code>
    /// </remarks>
    type AsteriskToken =
        PunctuationToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// PunctuationToken&lt;SyntaxKind.EqualsGreaterThanToken&gt;
    /// </code>
    /// </remarks>
    type EqualsGreaterThanToken =
        PunctuationToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// PunctuationToken&lt;SyntaxKind.PlusToken&gt;
    /// </code>
    /// </remarks>
    type PlusToken =
        PunctuationToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// PunctuationToken&lt;SyntaxKind.MinusToken&gt;
    /// </code>
    /// </remarks>
    type MinusToken =
        PunctuationToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// PunctuationToken&lt;SyntaxKind.QuestionDotToken&gt;
    /// </code>
    /// </remarks>
    type QuestionDotToken =
        PunctuationToken<SyntaxKind>

    type [<AllowNullLiteral>] KeywordToken<'TKind> =
        inherit Token<'TKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// KeywordToken&lt;SyntaxKind.AssertsKeyword&gt;
    /// </code>
    /// </remarks>
    type AssertsKeyword =
        KeywordToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// KeywordToken&lt;SyntaxKind.AssertKeyword&gt;
    /// </code>
    /// </remarks>
    type AssertKeyword =
        KeywordToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// KeywordToken&lt;SyntaxKind.AwaitKeyword&gt;
    /// </code>
    /// </remarks>
    type AwaitKeyword =
        KeywordToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// KeywordToken&lt;SyntaxKind.CaseKeyword&gt;
    /// </code>
    /// </remarks>
    type CaseKeyword =
        KeywordToken<SyntaxKind>

    type [<AllowNullLiteral>] ModifierToken<'TKind> =
        inherit KeywordToken<'TKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// ModifierToken&lt;SyntaxKind.AbstractKeyword&gt;
    /// </code>
    /// </remarks>
    type AbstractKeyword =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// ModifierToken&lt;SyntaxKind.AccessorKeyword&gt;
    /// </code>
    /// </remarks>
    type AccessorKeyword =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// ModifierToken&lt;SyntaxKind.AsyncKeyword&gt;
    /// </code>
    /// </remarks>
    type AsyncKeyword =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// ModifierToken&lt;SyntaxKind.ConstKeyword&gt;
    /// </code>
    /// </remarks>
    type ConstKeyword =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// ModifierToken&lt;SyntaxKind.DeclareKeyword&gt;
    /// </code>
    /// </remarks>
    type DeclareKeyword =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// ModifierToken&lt;SyntaxKind.DefaultKeyword&gt;
    /// </code>
    /// </remarks>
    type DefaultKeyword =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// ModifierToken&lt;SyntaxKind.ExportKeyword&gt;
    /// </code>
    /// </remarks>
    type ExportKeyword =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// ModifierToken&lt;SyntaxKind.InKeyword&gt;
    /// </code>
    /// </remarks>
    type InKeyword =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// ModifierToken&lt;SyntaxKind.PrivateKeyword&gt;
    /// </code>
    /// </remarks>
    type PrivateKeyword =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// ModifierToken&lt;SyntaxKind.ProtectedKeyword&gt;
    /// </code>
    /// </remarks>
    type ProtectedKeyword =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// ModifierToken&lt;SyntaxKind.PublicKeyword&gt;
    /// </code>
    /// </remarks>
    type PublicKeyword =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// ModifierToken&lt;SyntaxKind.ReadonlyKeyword&gt;
    /// </code>
    /// </remarks>
    type ReadonlyKeyword =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// ModifierToken&lt;SyntaxKind.OutKeyword&gt;
    /// </code>
    /// </remarks>
    type OutKeyword =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// ModifierToken&lt;SyntaxKind.OverrideKeyword&gt;
    /// </code>
    /// </remarks>
    type OverrideKeyword =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// ModifierToken&lt;SyntaxKind.StaticKeyword&gt;
    /// </code>
    /// </remarks>
    type StaticKeyword =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | AbstractKeyword
    ///     | AccessorKeyword
    ///     | AsyncKeyword
    ///     | ConstKeyword
    ///     | DeclareKeyword
    ///     | DefaultKeyword
    ///     | ExportKeyword
    ///     | InKeyword
    ///     | PrivateKeyword
    ///     | ProtectedKeyword
    ///     | PublicKeyword
    ///     | OutKeyword
    ///     | OverrideKeyword
    ///     | ReadonlyKeyword
    ///     | StaticKeyword
    /// </code>
    /// </remarks>
    type Modifier =
        ModifierToken<SyntaxKind>

    type ModifierLike =
        U2<Modifier, Decorator>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | PublicKeyword
    ///     | PrivateKeyword
    ///     | ProtectedKeyword
    /// </code>
    /// </remarks>
    type AccessibilityModifier =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | AccessibilityModifier
    ///     | ReadonlyKeyword
    /// </code>
    /// </remarks>
    type ParameterPropertyModifier =
        ModifierToken<SyntaxKind>

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | AccessibilityModifier
    ///     | ReadonlyKeyword
    ///     | StaticKeyword
    ///     | AccessorKeyword
    /// </code>
    /// </remarks>
    type ClassMemberModifier =
        ModifierToken<SyntaxKind>

    type ModifiersArray =
        ResizeArray<Modifier>

    type [<RequireQualifiedAccess>] GeneratedIdentifierFlags =
        | None = 0
        | Auto = 1
        | Loop = 2
        | Unique = 3
        | Node = 4
        | KindMask = 7
        | ReservedInNestedScopes = 8
        | Optimistic = 16
        | FileLevel = 32
        | AllowNameSubstitution = 64

    type [<AllowNullLiteral>] Identifier =
        inherit PrimaryExpression
        inherit Declaration
        inherit JSDocContainer
        inherit FlowContainer
        abstract kind: SyntaxKind
        /// <summary>
        /// Prefer to use <c>id.unescapedText</c>. (Note: This is available only in services, not internally to the TypeScript compiler.)
        /// Text of identifier, but if the identifier begins with two underscores, this will begin with three.
        /// </summary>
        abstract escapedText: __String

    type [<AllowNullLiteral>] TransientIdentifier =
        inherit Identifier
        abstract resolvedSymbol: Symbol with get, set

    type [<AllowNullLiteral>] AutoGenerateInfo =
        abstract flags: GeneratedIdentifierFlags with get, set
        abstract id: float
        abstract prefix: U2<string, GeneratedNamePart> option
        abstract suffix: string option

    type [<AllowNullLiteral>] GeneratedIdentifier =
        inherit Identifier
        abstract emitNode: obj

    type [<AllowNullLiteral>] QualifiedName =
        inherit Node
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract left: EntityName
        abstract right: Identifier

    type EntityName =
        U2<Identifier, QualifiedName>

    type PropertyName =
        U7<Identifier, StringLiteral, NoSubstitutionTemplateLiteral, NumericLiteral, ComputedPropertyName, PrivateIdentifier, BigIntLiteral>

    type MemberName =
        U2<Identifier, PrivateIdentifier>

    type DeclarationName =
        U6<PropertyName, JsxAttributeName, StringLiteralLike, ElementAccessExpression, BindingPattern, EntityNameExpression>

    type [<AllowNullLiteral>] Declaration =
        inherit Node
        abstract _declarationBrand: obj option with get, set
        abstract symbol: Symbol with get, set
        abstract localSymbol: Symbol option with get, set

    type [<AllowNullLiteral>] NamedDeclaration =
        inherit Declaration
        abstract name: DeclarationName option

    type [<AllowNullLiteral>] DynamicNamedDeclaration =
        inherit NamedDeclaration
        abstract name: ComputedPropertyName

    type [<AllowNullLiteral>] DynamicNamedBinaryExpression =
        inherit BinaryExpression
        abstract left: ElementAccessExpression

    type [<AllowNullLiteral>] LateBoundDeclaration =
        inherit DynamicNamedDeclaration
        abstract name: LateBoundName

    type [<AllowNullLiteral>] LateBoundBinaryExpressionDeclaration =
        inherit DynamicNamedBinaryExpression
        abstract left: LateBoundElementAccessExpression

    type [<AllowNullLiteral>] LateBoundElementAccessExpression =
        inherit ElementAccessExpression
        abstract argumentExpression: EntityNameExpression

    type [<AllowNullLiteral>] DeclarationStatement =
        inherit NamedDeclaration
        inherit Statement
        abstract name: U3<Identifier, StringLiteral, NumericLiteral> option

    type [<AllowNullLiteral>] ComputedPropertyName =
        inherit Node
        abstract kind: SyntaxKind
        abstract parent: Declaration
        abstract expression: Expression

    type [<AllowNullLiteral>] PrivateIdentifier =
        inherit PrimaryExpression
        abstract kind: SyntaxKind
        abstract escapedText: __String

    type [<AllowNullLiteral>] GeneratedPrivateIdentifier =
        inherit PrivateIdentifier
        abstract emitNode: obj

    type [<AllowNullLiteral>] LateBoundName =
        inherit ComputedPropertyName
        abstract expression: EntityNameExpression

    type [<AllowNullLiteral>] Decorator =
        inherit Node
        abstract kind: SyntaxKind
        abstract parent: NamedDeclaration
        abstract expression: LeftHandSideExpression

    type [<AllowNullLiteral>] TypeParameterDeclaration =
        inherit NamedDeclaration
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract parent: U2<DeclarationWithTypeParameterChildren, InferTypeNode>
        abstract modifiers: ResizeArray<Modifier> option
        abstract name: Identifier
        /// <summary>Note: Consider calling <c>getEffectiveConstraintOfTypeParameter</c></summary>
        abstract ``constraint``: TypeNode option
        abstract ``default``: TypeNode option
        abstract expression: Expression option with get, set

    type [<AllowNullLiteral>] SignatureDeclarationBase =
        inherit NamedDeclaration
        inherit JSDocContainer
        abstract kind: obj
        abstract name: PropertyName option
        abstract typeParameters: ResizeArray<TypeParameterDeclaration> option
        abstract parameters: ResizeArray<ParameterDeclaration>
        abstract ``type``: TypeNode option
        abstract typeArguments: ResizeArray<TypeNode> option with get, set

    type SignatureDeclaration =
        obj

    type [<AllowNullLiteral>] CallSignatureDeclaration =
        inherit SignatureDeclarationBase
        inherit TypeElement
        inherit LocalsContainer
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] ConstructSignatureDeclaration =
        inherit SignatureDeclarationBase
        inherit TypeElement
        inherit LocalsContainer
        abstract kind: SyntaxKind

    type BindingName =
        U2<Identifier, BindingPattern>

    type [<AllowNullLiteral>] VariableDeclaration =
        inherit NamedDeclaration
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract parent: U2<VariableDeclarationList, CatchClause>
        abstract name: BindingName
        abstract exclamationToken: ExclamationToken option
        abstract ``type``: TypeNode option
        abstract initializer: Expression option

    type [<AllowNullLiteral>] InitializedVariableDeclaration =
        interface end

    type [<AllowNullLiteral>] VariableDeclarationList =
        inherit Node
        abstract kind: SyntaxKind
        abstract parent: U4<VariableStatement, ForStatement, ForOfStatement, ForInStatement>
        abstract declarations: ResizeArray<VariableDeclaration>

    type [<AllowNullLiteral>] ParameterDeclaration =
        inherit NamedDeclaration
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract parent: SignatureDeclaration
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract dotDotDotToken: DotDotDotToken option
        abstract name: BindingName
        abstract questionToken: QuestionToken option
        abstract ``type``: TypeNode option
        abstract initializer: Expression option

    type [<AllowNullLiteral>] BindingElement =
        inherit NamedDeclaration
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract parent: BindingPattern
        abstract propertyName: PropertyName option
        abstract dotDotDotToken: DotDotDotToken option
        abstract name: BindingName
        abstract initializer: Expression option

    type BindingElementGrandparent =
        obj

    type [<AllowNullLiteral>] PropertySignature =
        inherit TypeElement
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract parent: U2<TypeLiteralNode, InterfaceDeclaration>
        abstract modifiers: ResizeArray<Modifier> option
        abstract name: PropertyName
        abstract questionToken: QuestionToken option
        abstract ``type``: TypeNode option
        abstract initializer: Expression option

    type [<AllowNullLiteral>] PropertyDeclaration =
        inherit ClassElement
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract parent: ClassLikeDeclaration
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract name: PropertyName
        abstract questionToken: QuestionToken option
        abstract exclamationToken: ExclamationToken option
        abstract ``type``: TypeNode option
        abstract initializer: Expression option

    type [<AllowNullLiteral>] AutoAccessorPropertyDeclaration =
        inherit PropertyDeclaration
        abstract _autoAccessorBrand: obj option with get, set

    type [<AllowNullLiteral>] PrivateIdentifierPropertyDeclaration =
        inherit PropertyDeclaration
        abstract name: PrivateIdentifier with get, set

    type [<AllowNullLiteral>] PrivateIdentifierAutoAccessorPropertyDeclaration =
        inherit AutoAccessorPropertyDeclaration
        abstract name: PrivateIdentifier with get, set

    type [<AllowNullLiteral>] PrivateIdentifierMethodDeclaration =
        inherit MethodDeclaration
        abstract name: PrivateIdentifier with get, set

    type [<AllowNullLiteral>] PrivateIdentifierGetAccessorDeclaration =
        inherit GetAccessorDeclaration
        abstract name: PrivateIdentifier with get, set

    type [<AllowNullLiteral>] PrivateIdentifierSetAccessorDeclaration =
        inherit SetAccessorDeclaration
        abstract name: PrivateIdentifier with get, set

    type PrivateIdentifierAccessorDeclaration =
        U2<PrivateIdentifierGetAccessorDeclaration, PrivateIdentifierSetAccessorDeclaration>

    type PrivateClassElementDeclaration =
        U5<PrivateIdentifierPropertyDeclaration, PrivateIdentifierAutoAccessorPropertyDeclaration, PrivateIdentifierMethodDeclaration, PrivateIdentifierGetAccessorDeclaration, PrivateIdentifierSetAccessorDeclaration>

    type [<AllowNullLiteral>] InitializedPropertyDeclaration =
        interface end

    type [<AllowNullLiteral>] ObjectLiteralElement =
        inherit NamedDeclaration
        abstract _objectLiteralBrand: obj option with get, set
        abstract name: PropertyName option

    /// Unlike ObjectLiteralElement, excludes JSXAttribute and JSXSpreadAttribute.
    type ObjectLiteralElementLike =
        U5<PropertyAssignment, ShorthandPropertyAssignment, SpreadAssignment, MethodDeclaration, AccessorDeclaration>

    type [<AllowNullLiteral>] PropertyAssignment =
        inherit ObjectLiteralElement
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract parent: ObjectLiteralExpression
        abstract name: PropertyName
        abstract initializer: Expression
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract questionToken: QuestionToken option
        abstract exclamationToken: ExclamationToken option

    type [<AllowNullLiteral>] ShorthandPropertyAssignment =
        inherit ObjectLiteralElement
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract parent: ObjectLiteralExpression
        abstract name: Identifier
        abstract equalsToken: EqualsToken option
        abstract objectAssignmentInitializer: Expression option
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract questionToken: QuestionToken option
        abstract exclamationToken: ExclamationToken option

    type [<AllowNullLiteral>] SpreadAssignment =
        inherit ObjectLiteralElement
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract parent: ObjectLiteralExpression
        abstract expression: Expression

    type VariableLikeDeclaration =
        obj

    type [<AllowNullLiteral>] ObjectBindingPattern =
        inherit Node
        abstract kind: SyntaxKind
        abstract parent: U3<VariableDeclaration, ParameterDeclaration, BindingElement>
        abstract elements: ResizeArray<BindingElement>

    type [<AllowNullLiteral>] ArrayBindingPattern =
        inherit Node
        abstract kind: SyntaxKind
        abstract parent: U3<VariableDeclaration, ParameterDeclaration, BindingElement>
        abstract elements: ResizeArray<ArrayBindingElement>

    type BindingPattern =
        U2<ObjectBindingPattern, ArrayBindingPattern>

    type ArrayBindingElement =
        U2<BindingElement, OmittedExpression>

    /// Several node kinds share function-like features such as a signature,
    /// a name, and a body. These nodes should extend FunctionLikeDeclarationBase.
    /// Examples:
    /// - FunctionDeclaration
    /// - MethodDeclaration
    /// - AccessorDeclaration
    type [<AllowNullLiteral>] FunctionLikeDeclarationBase =
        inherit SignatureDeclarationBase
        abstract _functionLikeDeclarationBrand: obj option with get, set
        abstract asteriskToken: AsteriskToken option
        abstract questionToken: QuestionToken option
        abstract exclamationToken: ExclamationToken option
        abstract body: U2<Block, Expression> option
        abstract endFlowNode: FlowNode option with get, set
        abstract returnFlowNode: FlowNode option with get, set

    type FunctionLikeDeclaration =
        U7<FunctionDeclaration, MethodDeclaration, GetAccessorDeclaration, SetAccessorDeclaration, ConstructorDeclaration, FunctionExpression, ArrowFunction>

    [<Obsolete("Use SignatureDeclaration")>]
    type FunctionLike =
        SignatureDeclaration

    type [<AllowNullLiteral>] FunctionDeclaration =
        inherit FunctionLikeDeclarationBase
        inherit DeclarationStatement
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract name: Identifier option
        abstract body: FunctionBody option

    type [<AllowNullLiteral>] MethodSignature =
        inherit SignatureDeclarationBase
        inherit TypeElement
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract parent: U2<TypeLiteralNode, InterfaceDeclaration>
        abstract modifiers: ResizeArray<Modifier> option
        abstract name: PropertyName

    type [<AllowNullLiteral>] MethodDeclaration =
        inherit FunctionLikeDeclarationBase
        inherit ClassElement
        inherit ObjectLiteralElement
        inherit JSDocContainer
        inherit LocalsContainer
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract parent: U2<ClassLikeDeclaration, ObjectLiteralExpression>
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract name: PropertyName
        abstract body: FunctionBody option
        abstract exclamationToken: ExclamationToken option

    type [<AllowNullLiteral>] ConstructorDeclaration =
        inherit FunctionLikeDeclarationBase
        inherit ClassElement
        inherit JSDocContainer
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract parent: ClassLikeDeclaration
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract body: FunctionBody option
        abstract typeParameters: ResizeArray<TypeParameterDeclaration> option
        abstract ``type``: TypeNode option

    /// For when we encounter a semicolon in a class declaration. ES6 allows these as class elements.
    type [<AllowNullLiteral>] SemicolonClassElement =
        inherit ClassElement
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract parent: ClassLikeDeclaration

    type [<AllowNullLiteral>] GetAccessorDeclaration =
        inherit FunctionLikeDeclarationBase
        inherit ClassElement
        inherit TypeElement
        inherit ObjectLiteralElement
        inherit JSDocContainer
        inherit LocalsContainer
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract parent: U4<ClassLikeDeclaration, ObjectLiteralExpression, TypeLiteralNode, InterfaceDeclaration>
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract name: PropertyName
        abstract body: FunctionBody option
        abstract typeParameters: ResizeArray<TypeParameterDeclaration> option

    type [<AllowNullLiteral>] SetAccessorDeclaration =
        inherit FunctionLikeDeclarationBase
        inherit ClassElement
        inherit TypeElement
        inherit ObjectLiteralElement
        inherit JSDocContainer
        inherit LocalsContainer
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract parent: U4<ClassLikeDeclaration, ObjectLiteralExpression, TypeLiteralNode, InterfaceDeclaration>
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract name: PropertyName
        abstract body: FunctionBody option
        abstract typeParameters: ResizeArray<TypeParameterDeclaration> option
        abstract ``type``: TypeNode option

    type AccessorDeclaration =
        U2<GetAccessorDeclaration, SetAccessorDeclaration>

    type [<AllowNullLiteral>] IndexSignatureDeclaration =
        inherit SignatureDeclarationBase
        inherit ClassElement
        inherit TypeElement
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract parent: ObjectTypeDeclaration
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract ``type``: TypeNode

    type [<AllowNullLiteral>] ClassStaticBlockDeclaration =
        inherit ClassElement
        inherit JSDocContainer
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract parent: U2<ClassDeclaration, ClassExpression>
        abstract body: Block
        abstract endFlowNode: FlowNode option with get, set
        abstract returnFlowNode: FlowNode option with get, set
        abstract modifiers: ResizeArray<ModifierLike> option

    type [<AllowNullLiteral>] TypeNode =
        inherit Node
        abstract _typeNodeBrand: obj option with get, set
        abstract kind: TypeNodeSyntaxKind

    type KeywordTypeNode =
        KeywordTypeNode<KeywordTypeSyntaxKind>

    type [<AllowNullLiteral>] KeywordTypeNode<'TKind> =
        inherit KeywordToken<'TKind>
        inherit TypeNode
        abstract kind: 'TKind

    [<Obsolete("")>]
    type [<AllowNullLiteral>] ImportTypeAssertionContainer =
        inherit Node
        abstract kind: SyntaxKind
        abstract parent: ImportTypeNode
        [<Obsolete("")>]
        abstract assertClause: AssertClause
        abstract multiLine: bool option

    type [<AllowNullLiteral>] ImportTypeNode =
        inherit NodeWithTypeArguments
        abstract kind: SyntaxKind
        abstract isTypeOf: bool
        abstract argument: TypeNode
        [<Obsolete("")>]
        abstract assertions: ImportTypeAssertionContainer option
        abstract attributes: ImportAttributes option
        abstract qualifier: EntityName option

    type [<AllowNullLiteral>] LiteralImportTypeNode =
        interface end

    type [<AllowNullLiteral>] ThisTypeNode =
        inherit TypeNode
        abstract kind: SyntaxKind

    type FunctionOrConstructorTypeNode =
        U2<FunctionTypeNode, ConstructorTypeNode>

    type [<AllowNullLiteral>] FunctionOrConstructorTypeNodeBase =
        inherit TypeNode
        inherit SignatureDeclarationBase
        abstract kind: SyntaxKind
        abstract ``type``: TypeNode

    type [<AllowNullLiteral>] FunctionTypeNode =
        inherit FunctionOrConstructorTypeNodeBase
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract modifiers: obj option

    type [<AllowNullLiteral>] ConstructorTypeNode =
        inherit FunctionOrConstructorTypeNodeBase
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract modifiers: ResizeArray<Modifier> option

    type [<AllowNullLiteral>] NodeWithTypeArguments =
        inherit TypeNode
        abstract typeArguments: ResizeArray<TypeNode> option

    type TypeReferenceType =
        U2<TypeReferenceNode, ExpressionWithTypeArguments>

    type [<AllowNullLiteral>] TypeReferenceNode =
        inherit NodeWithTypeArguments
        abstract kind: SyntaxKind
        abstract typeName: EntityName

    type [<AllowNullLiteral>] TypePredicateNode =
        inherit TypeNode
        abstract kind: SyntaxKind
        abstract parent: U2<SignatureDeclaration, JSDocTypeExpression>
        abstract assertsModifier: AssertsKeyword option
        abstract parameterName: U2<Identifier, ThisTypeNode>
        abstract ``type``: TypeNode option

    type [<AllowNullLiteral>] TypeQueryNode =
        inherit NodeWithTypeArguments
        abstract kind: SyntaxKind
        abstract exprName: EntityName

    type [<AllowNullLiteral>] TypeLiteralNode =
        inherit TypeNode
        inherit Declaration
        abstract kind: SyntaxKind
        abstract members: ResizeArray<TypeElement>

    type [<AllowNullLiteral>] ArrayTypeNode =
        inherit TypeNode
        abstract kind: SyntaxKind
        abstract elementType: TypeNode

    type [<AllowNullLiteral>] TupleTypeNode =
        inherit TypeNode
        abstract kind: SyntaxKind
        abstract elements: ResizeArray<U2<TypeNode, NamedTupleMember>>

    type [<AllowNullLiteral>] NamedTupleMember =
        inherit TypeNode
        inherit Declaration
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract dotDotDotToken: Token<SyntaxKind> option
        abstract name: Identifier
        abstract questionToken: Token<SyntaxKind> option
        abstract ``type``: TypeNode

    type [<AllowNullLiteral>] OptionalTypeNode =
        inherit TypeNode
        abstract kind: SyntaxKind
        abstract ``type``: TypeNode

    type [<AllowNullLiteral>] RestTypeNode =
        inherit TypeNode
        abstract kind: SyntaxKind
        abstract ``type``: TypeNode

    type UnionOrIntersectionTypeNode =
        U2<UnionTypeNode, IntersectionTypeNode>

    type [<AllowNullLiteral>] UnionTypeNode =
        inherit TypeNode
        abstract kind: SyntaxKind
        abstract types: ResizeArray<TypeNode>

    type [<AllowNullLiteral>] IntersectionTypeNode =
        inherit TypeNode
        abstract kind: SyntaxKind
        abstract types: ResizeArray<TypeNode>

    type [<AllowNullLiteral>] ConditionalTypeNode =
        inherit TypeNode
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract checkType: TypeNode
        abstract extendsType: TypeNode
        abstract trueType: TypeNode
        abstract falseType: TypeNode

    type [<AllowNullLiteral>] InferTypeNode =
        inherit TypeNode
        abstract kind: SyntaxKind
        abstract typeParameter: TypeParameterDeclaration

    type [<AllowNullLiteral>] ParenthesizedTypeNode =
        inherit TypeNode
        abstract kind: SyntaxKind
        abstract ``type``: TypeNode

    type [<AllowNullLiteral>] TypeOperatorNode =
        inherit TypeNode
        abstract kind: SyntaxKind
        abstract operator: SyntaxKind
        abstract ``type``: TypeNode

    type [<AllowNullLiteral>] UniqueTypeOperatorNode =
        inherit TypeOperatorNode
        abstract operator: SyntaxKind

    type [<AllowNullLiteral>] IndexedAccessTypeNode =
        inherit TypeNode
        abstract kind: SyntaxKind
        abstract objectType: TypeNode
        abstract indexType: TypeNode

    type [<AllowNullLiteral>] MappedTypeNode =
        inherit TypeNode
        inherit Declaration
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract readonlyToken: U3<ReadonlyKeyword, PlusToken, MinusToken> option
        abstract typeParameter: TypeParameterDeclaration
        abstract nameType: TypeNode option
        abstract questionToken: U3<QuestionToken, PlusToken, MinusToken> option
        abstract ``type``: TypeNode option
        /// Used only to produce grammar errors
        abstract members: ResizeArray<TypeElement> option

    type [<AllowNullLiteral>] LiteralTypeNode =
        inherit TypeNode
        abstract kind: SyntaxKind
        abstract literal: U4<NullLiteral, BooleanLiteral, LiteralExpression, PrefixUnaryExpression>

    type [<AllowNullLiteral>] StringLiteral =
        inherit LiteralExpression
        inherit Declaration
        abstract kind: SyntaxKind
        abstract textSourceNode: U6<Identifier, StringLiteralLike, NumericLiteral, PrivateIdentifier, JsxNamespacedName, BigIntLiteral> option
        /// <summary>Note: this is only set when synthesizing a node, not during parsing.</summary>
        abstract singleQuote: bool option

    type StringLiteralLike =
        U2<StringLiteral, NoSubstitutionTemplateLiteral>

    type PropertyNameLiteral =
        U5<Identifier, StringLiteralLike, NumericLiteral, JsxNamespacedName, BigIntLiteral>

    type [<AllowNullLiteral>] TemplateLiteralTypeNode =
        inherit TypeNode
        abstract kind: SyntaxKind with get, set
        abstract head: TemplateHead
        abstract templateSpans: ResizeArray<TemplateLiteralTypeSpan>

    type [<AllowNullLiteral>] TemplateLiteralTypeSpan =
        inherit TypeNode
        abstract kind: SyntaxKind
        abstract parent: TemplateLiteralTypeNode
        abstract ``type``: TypeNode
        abstract literal: U2<TemplateMiddle, TemplateTail>

    type [<AllowNullLiteral>] Expression =
        inherit Node
        abstract _expressionBrand: obj option with get, set

    type [<AllowNullLiteral>] OmittedExpression =
        inherit Expression
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] PartiallyEmittedExpression =
        inherit LeftHandSideExpression
        abstract kind: SyntaxKind
        abstract expression: Expression

    type [<AllowNullLiteral>] UnaryExpression =
        inherit Expression
        abstract _unaryExpressionBrand: obj option with get, set

    /// Deprecated, please use UpdateExpression
    type IncrementExpression =
        UpdateExpression

    type [<AllowNullLiteral>] UpdateExpression =
        inherit UnaryExpression
        abstract _updateExpressionBrand: obj option with get, set

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.PlusPlusToken
    ///     | SyntaxKind.MinusMinusToken
    ///     | SyntaxKind.PlusToken
    ///     | SyntaxKind.MinusToken
    ///     | SyntaxKind.TildeToken
    ///     | SyntaxKind.ExclamationToken
    /// </code>
    /// </remarks>
    type PrefixUnaryOperator =
        SyntaxKind

    type [<AllowNullLiteral>] PrefixUnaryExpression =
        inherit UpdateExpression
        abstract kind: SyntaxKind
        abstract operator: PrefixUnaryOperator
        abstract operand: UnaryExpression

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.PlusPlusToken
    ///     | SyntaxKind.MinusMinusToken
    /// </code>
    /// </remarks>
    type PostfixUnaryOperator =
        SyntaxKind

    type [<AllowNullLiteral>] PostfixUnaryExpression =
        inherit UpdateExpression
        abstract kind: SyntaxKind
        abstract operand: LeftHandSideExpression
        abstract operator: PostfixUnaryOperator

    type [<AllowNullLiteral>] LeftHandSideExpression =
        inherit UpdateExpression
        abstract _leftHandSideExpressionBrand: obj option with get, set

    type [<AllowNullLiteral>] MemberExpression =
        inherit LeftHandSideExpression
        abstract _memberExpressionBrand: obj option with get, set

    type [<AllowNullLiteral>] PrimaryExpression =
        inherit MemberExpression
        abstract _primaryExpressionBrand: obj option with get, set

    type [<AllowNullLiteral>] NullLiteral =
        inherit PrimaryExpression
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] TrueLiteral =
        inherit PrimaryExpression
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] FalseLiteral =
        inherit PrimaryExpression
        abstract kind: SyntaxKind

    type BooleanLiteral =
        U2<TrueLiteral, FalseLiteral>

    type [<AllowNullLiteral>] ThisExpression =
        inherit PrimaryExpression
        inherit FlowContainer
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] SuperExpression =
        inherit PrimaryExpression
        inherit FlowContainer
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] ImportExpression =
        inherit PrimaryExpression
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] DeleteExpression =
        inherit UnaryExpression
        abstract kind: SyntaxKind
        abstract expression: UnaryExpression

    type [<AllowNullLiteral>] TypeOfExpression =
        inherit UnaryExpression
        abstract kind: SyntaxKind
        abstract expression: UnaryExpression

    type [<AllowNullLiteral>] VoidExpression =
        inherit UnaryExpression
        abstract kind: SyntaxKind
        abstract expression: UnaryExpression

    type [<AllowNullLiteral>] AwaitExpression =
        inherit UnaryExpression
        abstract kind: SyntaxKind
        abstract expression: UnaryExpression

    type [<AllowNullLiteral>] YieldExpression =
        inherit Expression
        abstract kind: SyntaxKind
        abstract asteriskToken: AsteriskToken option
        abstract expression: Expression option

    type [<AllowNullLiteral>] SyntheticExpression =
        inherit Expression
        abstract kind: SyntaxKind
        abstract isSpread: bool
        abstract ``type``: Type
        abstract tupleNameSource: U2<ParameterDeclaration, NamedTupleMember> option

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// SyntaxKind.AsteriskAsteriskToken
    /// </code>
    /// </remarks>
    type ExponentiationOperator =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.AsteriskToken
    ///     | SyntaxKind.SlashToken
    ///     | SyntaxKind.PercentToken
    /// </code>
    /// </remarks>
    type MultiplicativeOperator =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | ExponentiationOperator
    ///     | MultiplicativeOperator
    /// </code>
    /// </remarks>
    type MultiplicativeOperatorOrHigher =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.PlusToken
    ///     | SyntaxKind.MinusToken
    /// </code>
    /// </remarks>
    type AdditiveOperator =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | MultiplicativeOperatorOrHigher
    ///     | AdditiveOperator
    /// </code>
    /// </remarks>
    type AdditiveOperatorOrHigher =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.LessThanLessThanToken
    ///     | SyntaxKind.GreaterThanGreaterThanToken
    ///     | SyntaxKind.GreaterThanGreaterThanGreaterThanToken
    /// </code>
    /// </remarks>
    type ShiftOperator =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | AdditiveOperatorOrHigher
    ///     | ShiftOperator
    /// </code>
    /// </remarks>
    type ShiftOperatorOrHigher =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.LessThanToken
    ///     | SyntaxKind.LessThanEqualsToken
    ///     | SyntaxKind.GreaterThanToken
    ///     | SyntaxKind.GreaterThanEqualsToken
    ///     | SyntaxKind.InstanceOfKeyword
    ///     | SyntaxKind.InKeyword
    /// </code>
    /// </remarks>
    type RelationalOperator =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | ShiftOperatorOrHigher
    ///     | RelationalOperator
    /// </code>
    /// </remarks>
    type RelationalOperatorOrHigher =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.EqualsEqualsToken
    ///     | SyntaxKind.EqualsEqualsEqualsToken
    ///     | SyntaxKind.ExclamationEqualsEqualsToken
    ///     | SyntaxKind.ExclamationEqualsToken
    /// </code>
    /// </remarks>
    type EqualityOperator =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | RelationalOperatorOrHigher
    ///     | EqualityOperator
    /// </code>
    /// </remarks>
    type EqualityOperatorOrHigher =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.AmpersandToken
    ///     | SyntaxKind.BarToken
    ///     | SyntaxKind.CaretToken
    /// </code>
    /// </remarks>
    type BitwiseOperator =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | EqualityOperatorOrHigher
    ///     | BitwiseOperator
    /// </code>
    /// </remarks>
    type BitwiseOperatorOrHigher =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.AmpersandAmpersandToken
    ///     | SyntaxKind.BarBarToken
    /// </code>
    /// </remarks>
    type LogicalOperator =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | BitwiseOperatorOrHigher
    ///     | LogicalOperator
    /// </code>
    /// </remarks>
    type LogicalOperatorOrHigher =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.PlusEqualsToken
    ///     | SyntaxKind.MinusEqualsToken
    ///     | SyntaxKind.AsteriskAsteriskEqualsToken
    ///     | SyntaxKind.AsteriskEqualsToken
    ///     | SyntaxKind.SlashEqualsToken
    ///     | SyntaxKind.PercentEqualsToken
    ///     | SyntaxKind.AmpersandEqualsToken
    ///     | SyntaxKind.BarEqualsToken
    ///     | SyntaxKind.CaretEqualsToken
    ///     | SyntaxKind.LessThanLessThanEqualsToken
    ///     | SyntaxKind.GreaterThanGreaterThanGreaterThanEqualsToken
    ///     | SyntaxKind.GreaterThanGreaterThanEqualsToken
    ///     | SyntaxKind.BarBarEqualsToken
    ///     | SyntaxKind.AmpersandAmpersandEqualsToken
    ///     | SyntaxKind.QuestionQuestionEqualsToken
    /// </code>
    /// </remarks>
    type CompoundAssignmentOperator =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.EqualsToken
    ///     | CompoundAssignmentOperator
    /// </code>
    /// </remarks>
    type AssignmentOperator =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.QuestionQuestionToken
    ///     | LogicalOperatorOrHigher
    ///     | AssignmentOperator
    /// </code>
    /// </remarks>
    type AssignmentOperatorOrHigher =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | AssignmentOperatorOrHigher
    ///     | SyntaxKind.CommaToken
    /// </code>
    /// </remarks>
    type BinaryOperator =
        SyntaxKind

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | SyntaxKind.AmpersandAmpersandEqualsToken
    ///     | SyntaxKind.BarBarEqualsToken
    ///     | SyntaxKind.QuestionQuestionEqualsToken
    /// </code>
    /// </remarks>
    type LogicalOrCoalescingAssignmentOperator =
        SyntaxKind

    type BinaryOperatorToken =
        Token<BinaryOperator>

    type [<AllowNullLiteral>] BinaryExpression =
        inherit Expression
        inherit Declaration
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract left: Expression
        abstract operatorToken: BinaryOperatorToken
        abstract right: Expression

    type AssignmentOperatorToken =
        Token<AssignmentOperator>

    type [<AllowNullLiteral>] AssignmentExpression<'TOperator when 'TOperator :> AssignmentOperatorToken> =
        inherit BinaryExpression
        abstract left: LeftHandSideExpression
        abstract operatorToken: 'TOperator

    type [<AllowNullLiteral>] ObjectDestructuringAssignment =
        inherit AssignmentExpression<EqualsToken>
        abstract left: ObjectLiteralExpression

    type [<AllowNullLiteral>] ArrayDestructuringAssignment =
        inherit AssignmentExpression<EqualsToken>
        abstract left: ArrayLiteralExpression

    type DestructuringAssignment =
        U2<ObjectDestructuringAssignment, ArrayDestructuringAssignment>

    type BindingOrAssignmentElement =
        U4<VariableDeclaration, ParameterDeclaration, ObjectBindingOrAssignmentElement, ArrayBindingOrAssignmentElement>

    type ObjectBindingOrAssignmentElement =
        U4<BindingElement, PropertyAssignment, ShorthandPropertyAssignment, SpreadAssignment>

    type ObjectAssignmentElement =
        Exclude<ObjectBindingOrAssignmentElement, BindingElement>

    type ArrayBindingOrAssignmentElement =
        obj

    type ArrayAssignmentElement =
        Exclude<ArrayBindingOrAssignmentElement, BindingElement>

    type BindingOrAssignmentElementRestIndicator =
        U3<DotDotDotToken, SpreadElement, SpreadAssignment>

    type BindingOrAssignmentElementTarget =
        U5<BindingOrAssignmentPattern, Identifier, PropertyAccessExpression, ElementAccessExpression, OmittedExpression>

    type AssignmentElementTarget =
        Exclude<BindingOrAssignmentElementTarget, BindingPattern>

    type ObjectBindingOrAssignmentPattern =
        U2<ObjectBindingPattern, ObjectLiteralExpression>

    type ArrayBindingOrAssignmentPattern =
        U2<ArrayBindingPattern, ArrayLiteralExpression>

    type AssignmentPattern =
        U2<ObjectLiteralExpression, ArrayLiteralExpression>

    type BindingOrAssignmentPattern =
        U2<ObjectBindingOrAssignmentPattern, ArrayBindingOrAssignmentPattern>

    type [<AllowNullLiteral>] ConditionalExpression =
        inherit Expression
        abstract kind: SyntaxKind
        abstract condition: Expression
        abstract questionToken: QuestionToken
        abstract whenTrue: Expression
        abstract colonToken: ColonToken
        abstract whenFalse: Expression
        abstract flowNodeWhenTrue: FlowNode option with get, set
        abstract flowNodeWhenFalse: FlowNode option with get, set

    type FunctionBody =
        Block

    type ConciseBody =
        U2<FunctionBody, Expression>

    type [<AllowNullLiteral>] FunctionExpression =
        inherit PrimaryExpression
        inherit FunctionLikeDeclarationBase
        inherit JSDocContainer
        inherit LocalsContainer
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract modifiers: ResizeArray<Modifier> option
        abstract name: Identifier option
        abstract body: FunctionBody

    type [<AllowNullLiteral>] ArrowFunction =
        inherit Expression
        inherit FunctionLikeDeclarationBase
        inherit JSDocContainer
        inherit LocalsContainer
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract modifiers: ResizeArray<Modifier> option
        abstract equalsGreaterThanToken: EqualsGreaterThanToken
        abstract body: ConciseBody
        abstract name: obj

    type [<AllowNullLiteral>] LiteralLikeNode =
        inherit Node
        abstract text: string with get, set
        abstract isUnterminated: bool option with get, set
        abstract hasExtendedUnicodeEscape: bool option with get, set

    type [<AllowNullLiteral>] TemplateLiteralLikeNode =
        inherit LiteralLikeNode
        abstract rawText: string option with get, set
        abstract templateFlags: TokenFlags option with get, set

    type [<AllowNullLiteral>] LiteralExpression =
        inherit LiteralLikeNode
        inherit PrimaryExpression
        abstract _literalExpressionBrand: obj option with get, set

    type [<AllowNullLiteral>] RegularExpressionLiteral =
        inherit LiteralExpression
        abstract kind: SyntaxKind

    type [<RequireQualifiedAccess>] RegularExpressionFlags =
        | None = 0
        | HasIndices = 1
        | Global = 2
        | IgnoreCase = 4
        | Multiline = 8
        | DotAll = 16
        | Unicode = 32
        | UnicodeSets = 64
        | Sticky = 128
        | AnyUnicodeMode = 96
        | Modifiers = 28

    type [<AllowNullLiteral>] NoSubstitutionTemplateLiteral =
        inherit LiteralExpression
        inherit TemplateLiteralLikeNode
        inherit Declaration
        abstract kind: SyntaxKind
        abstract templateFlags: TokenFlags option with get, set

    type [<RequireQualifiedAccess>] TokenFlags =
        | None = 0
        | PrecedingLineBreak = 1
        | PrecedingJSDocComment = 2
        | Unterminated = 4
        | ExtendedUnicodeEscape = 8
        | Scientific = 16
        | Octal = 32
        | HexSpecifier = 64
        | BinarySpecifier = 128
        | OctalSpecifier = 256
        | ContainsSeparator = 512
        | UnicodeEscape = 1024
        | ContainsInvalidEscape = 2048
        | HexEscape = 4096
        | ContainsLeadingZero = 8192
        | ContainsInvalidSeparator = 16384
        | PrecedingJSDocLeadingAsterisks = 32768
        | BinaryOrOctalSpecifier = 384
        | WithSpecifier = 448
        | StringLiteralFlags = 7176
        | NumericLiteralFlags = 25584
        | TemplateLiteralLikeFlags = 7176
        | IsInvalid = 26656

    type [<AllowNullLiteral>] NumericLiteral =
        inherit LiteralExpression
        inherit Declaration
        abstract kind: SyntaxKind
        abstract numericLiteralFlags: TokenFlags

    type [<AllowNullLiteral>] BigIntLiteral =
        inherit LiteralExpression
        abstract kind: SyntaxKind

    type LiteralToken =
        U6<NumericLiteral, BigIntLiteral, StringLiteral, JsxText, RegularExpressionLiteral, NoSubstitutionTemplateLiteral>

    type [<AllowNullLiteral>] TemplateHead =
        inherit TemplateLiteralLikeNode
        abstract kind: SyntaxKind
        abstract parent: U2<TemplateExpression, TemplateLiteralTypeNode>
        abstract templateFlags: TokenFlags option with get, set

    type [<AllowNullLiteral>] TemplateMiddle =
        inherit TemplateLiteralLikeNode
        abstract kind: SyntaxKind
        abstract parent: U2<TemplateSpan, TemplateLiteralTypeSpan>
        abstract templateFlags: TokenFlags option with get, set

    type [<AllowNullLiteral>] TemplateTail =
        inherit TemplateLiteralLikeNode
        abstract kind: SyntaxKind
        abstract parent: U2<TemplateSpan, TemplateLiteralTypeSpan>
        abstract templateFlags: TokenFlags option with get, set

    type PseudoLiteralToken =
        U3<TemplateHead, TemplateMiddle, TemplateTail>

    type TemplateLiteralToken =
        U2<NoSubstitutionTemplateLiteral, PseudoLiteralToken>

    type [<AllowNullLiteral>] TemplateExpression =
        inherit PrimaryExpression
        abstract kind: SyntaxKind
        abstract head: TemplateHead
        abstract templateSpans: ResizeArray<TemplateSpan>

    type TemplateLiteral =
        U2<TemplateExpression, NoSubstitutionTemplateLiteral>

    type [<AllowNullLiteral>] TemplateSpan =
        inherit Node
        abstract kind: SyntaxKind
        abstract parent: TemplateExpression
        abstract expression: Expression
        abstract literal: U2<TemplateMiddle, TemplateTail>

    type [<AllowNullLiteral>] ParenthesizedExpression =
        inherit PrimaryExpression
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract expression: Expression

    type [<AllowNullLiteral>] JSDocTypeAssertion =
        inherit ParenthesizedExpression
        abstract _jsDocTypeAssertionBrand: obj

    type [<AllowNullLiteral>] ArrayLiteralExpression =
        inherit PrimaryExpression
        abstract kind: SyntaxKind
        abstract elements: ResizeArray<Expression>
        abstract multiLine: bool option with get, set

    type [<AllowNullLiteral>] SpreadElement =
        inherit Expression
        abstract kind: SyntaxKind
        abstract parent: U3<ArrayLiteralExpression, CallExpression, NewExpression>
        abstract expression: Expression

    /// This interface is a base interface for ObjectLiteralExpression and JSXAttributes to extend from. JSXAttributes is similar to
    /// ObjectLiteralExpression in that it contains array of properties; however, JSXAttributes' properties can only be
    /// JSXAttribute or JSXSpreadAttribute. ObjectLiteralExpression, on the other hand, can only have properties of type
    /// ObjectLiteralElement (e.g. PropertyAssignment, ShorthandPropertyAssignment etc.)
    type [<AllowNullLiteral>] ObjectLiteralExpressionBase<'T when 'T :> ObjectLiteralElement> =
        inherit PrimaryExpression
        inherit Declaration
        abstract properties: ResizeArray<'T>

    type [<AllowNullLiteral>] ObjectLiteralExpression =
        // inherit ObjectLiteralExpressionBase<ObjectLiteralElementLike>
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract multiLine: bool option with get, set

    type EntityNameExpression =
        U2<Identifier, PropertyAccessEntityNameExpression>

    type EntityNameOrEntityNameExpression =
        U2<EntityName, EntityNameExpression>

    type AccessExpression =
        U2<PropertyAccessExpression, ElementAccessExpression>

    type [<AllowNullLiteral>] PropertyAccessExpression =
        inherit MemberExpression
        inherit NamedDeclaration
        inherit JSDocContainer
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract expression: LeftHandSideExpression
        abstract questionDotToken: QuestionDotToken option
        abstract name: MemberName

    type [<AllowNullLiteral>] PrivateIdentifierPropertyAccessExpression =
        inherit PropertyAccessExpression
        abstract name: PrivateIdentifier

    type [<AllowNullLiteral>] PropertyAccessChain =
        inherit PropertyAccessExpression
        abstract _optionalChainBrand: obj option with get, set
        abstract name: MemberName

    type [<AllowNullLiteral>] PropertyAccessChainRoot =
        inherit PropertyAccessChain
        abstract questionDotToken: QuestionDotToken

    type [<AllowNullLiteral>] SuperPropertyAccessExpression =
        inherit PropertyAccessExpression
        abstract expression: SuperExpression

    /// Brand for a PropertyAccessExpression which, like a QualifiedName, consists of a sequence of identifiers separated by dots.
    type [<AllowNullLiteral>] PropertyAccessEntityNameExpression =
        inherit PropertyAccessExpression
        abstract _propertyAccessExpressionLikeQualifiedNameBrand: obj option with get, set
        abstract expression: EntityNameExpression
        abstract name: Identifier

    type [<AllowNullLiteral>] ElementAccessExpression =
        inherit MemberExpression
        inherit Declaration
        inherit JSDocContainer
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract expression: LeftHandSideExpression
        abstract questionDotToken: QuestionDotToken option
        abstract argumentExpression: Expression

    type [<AllowNullLiteral>] ElementAccessChain =
        inherit ElementAccessExpression
        abstract _optionalChainBrand: obj option with get, set

    type [<AllowNullLiteral>] ElementAccessChainRoot =
        inherit ElementAccessChain
        abstract questionDotToken: QuestionDotToken

    type [<AllowNullLiteral>] SuperElementAccessExpression =
        inherit ElementAccessExpression
        abstract expression: SuperExpression

    type SuperProperty =
        U2<SuperPropertyAccessExpression, SuperElementAccessExpression>

    type [<AllowNullLiteral>] CallExpression =
        inherit LeftHandSideExpression
        inherit Declaration
        abstract kind: SyntaxKind
        abstract expression: LeftHandSideExpression
        abstract questionDotToken: QuestionDotToken option
        abstract typeArguments: ResizeArray<TypeNode> option
        abstract arguments: ResizeArray<Expression>

    type [<AllowNullLiteral>] CallChain =
        inherit CallExpression
        abstract _optionalChainBrand: obj option with get, set

    type [<AllowNullLiteral>] CallChainRoot =
        inherit CallChain
        abstract questionDotToken: QuestionDotToken

    type OptionalChain =
        U4<PropertyAccessChain, ElementAccessChain, CallChain, NonNullChain>

    type OptionalChainRoot =
        U3<PropertyAccessChainRoot, ElementAccessChainRoot, CallChainRoot>

    type [<AllowNullLiteral>] BindableObjectDefinePropertyCall =
        interface end

    type BindableStaticNameExpression =
        U2<EntityNameExpression, BindableStaticElementAccessExpression>

    type [<AllowNullLiteral>] LiteralLikeElementAccessExpression =
        interface end

    type [<AllowNullLiteral>] BindableStaticElementAccessExpression =
        interface end

    type [<AllowNullLiteral>] BindableElementAccessExpression =
        interface end

    type BindableStaticAccessExpression =
        U2<PropertyAccessEntityNameExpression, BindableStaticElementAccessExpression>

    type BindableAccessExpression =
        U2<PropertyAccessEntityNameExpression, BindableElementAccessExpression>

    type [<AllowNullLiteral>] BindableStaticPropertyAssignmentExpression =
        inherit BinaryExpression
        abstract left: BindableStaticAccessExpression

    type [<AllowNullLiteral>] BindablePropertyAssignmentExpression =
        inherit BinaryExpression
        abstract left: BindableAccessExpression

    type [<AllowNullLiteral>] SuperCall =
        inherit CallExpression
        abstract expression: SuperExpression

    type [<AllowNullLiteral>] ImportCall =
        inherit CallExpression
        abstract expression: U2<ImportExpression, ImportDeferProperty>

    type [<AllowNullLiteral>] ExpressionWithTypeArguments =
        inherit MemberExpression
        inherit NodeWithTypeArguments
        abstract kind: SyntaxKind
        abstract expression: LeftHandSideExpression

    type [<AllowNullLiteral>] NewExpression =
        inherit PrimaryExpression
        inherit Declaration
        abstract kind: SyntaxKind
        abstract expression: LeftHandSideExpression
        abstract typeArguments: ResizeArray<TypeNode> option
        abstract arguments: ResizeArray<Expression> option

    type [<AllowNullLiteral>] TaggedTemplateExpression =
        inherit MemberExpression
        abstract kind: SyntaxKind
        abstract tag: LeftHandSideExpression
        abstract typeArguments: ResizeArray<TypeNode> option
        abstract template: TemplateLiteral
        abstract questionDotToken: QuestionDotToken option with get, set

    type [<AllowNullLiteral>] InstanceofExpression =
        inherit BinaryExpression
        abstract operatorToken: Token<SyntaxKind>

    type CallLikeExpression =
        U6<CallExpression, NewExpression, TaggedTemplateExpression, Decorator, JsxCallLike, InstanceofExpression>

    type [<AllowNullLiteral>] AsExpression =
        inherit Expression
        abstract kind: SyntaxKind
        abstract expression: Expression
        abstract ``type``: TypeNode

    type [<AllowNullLiteral>] TypeAssertion =
        inherit UnaryExpression
        abstract kind: SyntaxKind
        abstract ``type``: TypeNode
        abstract expression: UnaryExpression

    type [<AllowNullLiteral>] SatisfiesExpression =
        inherit Expression
        abstract kind: SyntaxKind
        abstract expression: Expression
        abstract ``type``: TypeNode

    type AssertionExpression =
        U2<TypeAssertion, AsExpression>

    type [<AllowNullLiteral>] NonNullExpression =
        inherit LeftHandSideExpression
        abstract kind: SyntaxKind
        abstract expression: Expression

    type [<AllowNullLiteral>] NonNullChain =
        inherit NonNullExpression
        abstract _optionalChainBrand: obj option with get, set

    type [<AllowNullLiteral>] MetaProperty =
        inherit PrimaryExpression
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract keywordToken: SyntaxKind
        abstract name: Identifier

    type [<AllowNullLiteral>] ImportMetaProperty =
        inherit MetaProperty
        abstract keywordToken: SyntaxKind
        abstract name: obj

    type [<AllowNullLiteral>] ImportDeferProperty =
        inherit MetaProperty
        abstract keywordToken: SyntaxKind
        abstract name: obj

    type [<AllowNullLiteral>] JsxElement =
        inherit PrimaryExpression
        abstract kind: SyntaxKind
        abstract openingElement: JsxOpeningElement
        abstract children: ResizeArray<JsxChild>
        abstract closingElement: JsxClosingElement

    type JsxOpeningLikeElement =
        U2<JsxSelfClosingElement, JsxOpeningElement>

    type JsxCallLike =
        U2<JsxOpeningLikeElement, JsxOpeningFragment>

    type JsxAttributeLike =
        U2<JsxAttribute, JsxSpreadAttribute>

    type JsxAttributeName =
        U2<Identifier, JsxNamespacedName>

    type JsxTagNameExpression =
        U4<Identifier, ThisExpression, JsxTagNamePropertyAccess, JsxNamespacedName>

    type [<AllowNullLiteral>] JsxTagNamePropertyAccess =
        inherit PropertyAccessExpression
        abstract expression: U3<Identifier, ThisExpression, JsxTagNamePropertyAccess>

    type [<AllowNullLiteral>] JsxAttributes =
        inherit PrimaryExpression
        inherit Declaration
        abstract properties: ResizeArray<JsxAttributeLike>
        abstract kind: SyntaxKind
        abstract parent: JsxOpeningLikeElement

    type [<AllowNullLiteral>] JsxNamespacedName =
        inherit Node
        abstract kind: SyntaxKind
        abstract name: Identifier
        abstract ``namespace``: Identifier

    type [<AllowNullLiteral>] JsxOpeningElement =
        inherit Expression
        abstract kind: SyntaxKind
        abstract parent: JsxElement
        abstract tagName: JsxTagNameExpression
        abstract typeArguments: ResizeArray<TypeNode> option
        abstract attributes: JsxAttributes

    type [<AllowNullLiteral>] JsxSelfClosingElement =
        inherit PrimaryExpression
        abstract kind: SyntaxKind
        abstract tagName: JsxTagNameExpression
        abstract typeArguments: ResizeArray<TypeNode> option
        abstract attributes: JsxAttributes

    type [<AllowNullLiteral>] JsxFragment =
        inherit PrimaryExpression
        abstract kind: SyntaxKind
        abstract openingFragment: JsxOpeningFragment
        abstract children: ResizeArray<JsxChild>
        abstract closingFragment: JsxClosingFragment

    type [<AllowNullLiteral>] JsxOpeningFragment =
        inherit Expression
        abstract kind: SyntaxKind
        abstract parent: JsxFragment

    type [<AllowNullLiteral>] JsxClosingFragment =
        inherit Expression
        abstract kind: SyntaxKind
        abstract parent: JsxFragment

    type [<AllowNullLiteral>] JsxAttribute =
        inherit Declaration
        abstract kind: SyntaxKind
        abstract parent: JsxAttributes
        abstract name: JsxAttributeName
        abstract initializer: JsxAttributeValue option

    type JsxAttributeValue =
        U5<StringLiteral, JsxExpression, JsxElement, JsxSelfClosingElement, JsxFragment>

    type [<AllowNullLiteral>] JsxSpreadAttribute =
        inherit ObjectLiteralElement
        abstract kind: SyntaxKind
        abstract parent: JsxAttributes
        abstract expression: Expression

    type [<AllowNullLiteral>] JsxClosingElement =
        inherit Node
        abstract kind: SyntaxKind
        abstract parent: JsxElement
        abstract tagName: JsxTagNameExpression

    type [<AllowNullLiteral>] JsxExpression =
        inherit Expression
        abstract kind: SyntaxKind
        abstract parent: U3<JsxElement, JsxFragment, JsxAttributeLike>
        abstract dotDotDotToken: Token<SyntaxKind> option
        abstract expression: Expression option

    type [<AllowNullLiteral>] JsxText =
        inherit LiteralLikeNode
        abstract kind: SyntaxKind
        abstract parent: U2<JsxElement, JsxFragment>
        abstract containsOnlyTriviaWhiteSpaces: bool

    type JsxChild =
        U5<JsxText, JsxExpression, JsxElement, JsxSelfClosingElement, JsxFragment>

    type [<AllowNullLiteral>] Statement =
        inherit Node
        inherit JSDocContainer
        abstract _statementBrand: obj option with get, set

    type [<AllowNullLiteral>] NotEmittedStatement =
        inherit Statement
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] NotEmittedTypeElement =
        inherit TypeElement
        abstract kind: SyntaxKind

    /// A list of comma-separated expressions. This node is only created by transformations.
    type [<AllowNullLiteral>] CommaListExpression =
        inherit Expression
        abstract kind: SyntaxKind
        abstract elements: ResizeArray<Expression>

    type [<AllowNullLiteral>] SyntheticReferenceExpression =
        inherit LeftHandSideExpression
        abstract kind: SyntaxKind
        abstract expression: Expression
        abstract thisArg: Expression

    type [<AllowNullLiteral>] EmptyStatement =
        inherit Statement
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] DebuggerStatement =
        inherit Statement
        inherit FlowContainer
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] MissingDeclaration =
        inherit DeclarationStatement
        inherit PrimaryExpression
        abstract kind: SyntaxKind
        abstract name: Identifier option
        abstract modifiers: ResizeArray<ModifierLike> option

    type BlockLike =
        U4<SourceFile, Block, ModuleBlock, CaseOrDefaultClause>

    type [<AllowNullLiteral>] Block =
        inherit Statement
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract statements: ResizeArray<Statement>
        abstract multiLine: bool option with get, set

    type [<AllowNullLiteral>] VariableStatement =
        inherit Statement
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract declarationList: VariableDeclarationList

    type [<AllowNullLiteral>] ExpressionStatement =
        inherit Statement
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract expression: Expression

    type [<AllowNullLiteral>] PrologueDirective =
        inherit ExpressionStatement
        abstract expression: StringLiteral

    type [<AllowNullLiteral>] IfStatement =
        inherit Statement
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract expression: Expression
        abstract thenStatement: Statement
        abstract elseStatement: Statement option

    type [<AllowNullLiteral>] IterationStatement =
        inherit Statement
        abstract statement: Statement

    type [<AllowNullLiteral>] DoStatement =
        inherit IterationStatement
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract expression: Expression

    type [<AllowNullLiteral>] WhileStatement =
        inherit IterationStatement
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract expression: Expression

    type ForInitializer =
        U2<VariableDeclarationList, Expression>

    type [<AllowNullLiteral>] ForStatement =
        inherit IterationStatement
        inherit LocalsContainer
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract initializer: ForInitializer option
        abstract condition: Expression option
        abstract incrementor: Expression option

    type ForInOrOfStatement =
        U2<ForInStatement, ForOfStatement>

    type [<AllowNullLiteral>] ForInStatement =
        inherit IterationStatement
        inherit LocalsContainer
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract initializer: ForInitializer
        abstract expression: Expression

    type [<AllowNullLiteral>] ForOfStatement =
        inherit IterationStatement
        inherit LocalsContainer
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract awaitModifier: AwaitKeyword option
        abstract initializer: ForInitializer
        abstract expression: Expression

    type [<AllowNullLiteral>] BreakStatement =
        inherit Statement
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract label: Identifier option

    type [<AllowNullLiteral>] ContinueStatement =
        inherit Statement
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract label: Identifier option

    type BreakOrContinueStatement =
        U2<BreakStatement, ContinueStatement>

    type [<AllowNullLiteral>] ReturnStatement =
        inherit Statement
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract expression: Expression option

    type [<AllowNullLiteral>] WithStatement =
        inherit Statement
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract expression: Expression
        abstract statement: Statement

    type [<AllowNullLiteral>] SwitchStatement =
        inherit Statement
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract expression: Expression
        abstract caseBlock: CaseBlock
        abstract possiblyExhaustive: bool option with get, set

    type [<AllowNullLiteral>] CaseBlock =
        inherit Node
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract parent: SwitchStatement
        abstract clauses: ResizeArray<CaseOrDefaultClause>

    type [<AllowNullLiteral>] CaseClause =
        inherit Node
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract parent: CaseBlock
        abstract expression: Expression
        abstract statements: ResizeArray<Statement>
        abstract fallthroughFlowNode: FlowNode option with get, set

    type [<AllowNullLiteral>] DefaultClause =
        inherit Node
        abstract kind: SyntaxKind
        abstract parent: CaseBlock
        abstract statements: ResizeArray<Statement>
        abstract fallthroughFlowNode: FlowNode option with get, set

    type CaseOrDefaultClause =
        U2<CaseClause, DefaultClause>

    type [<AllowNullLiteral>] LabeledStatement =
        inherit Statement
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract label: Identifier
        abstract statement: Statement

    type [<AllowNullLiteral>] ThrowStatement =
        inherit Statement
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract expression: Expression

    type [<AllowNullLiteral>] TryStatement =
        inherit Statement
        inherit FlowContainer
        abstract kind: SyntaxKind
        abstract tryBlock: Block
        abstract catchClause: CatchClause option
        abstract finallyBlock: Block option

    type [<AllowNullLiteral>] CatchClause =
        inherit Node
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract parent: TryStatement
        abstract variableDeclaration: VariableDeclaration option
        abstract block: Block

    type ObjectTypeDeclaration =
        U3<ClassLikeDeclaration, InterfaceDeclaration, TypeLiteralNode>

    type DeclarationWithTypeParameters =
        U4<DeclarationWithTypeParameterChildren, JSDocTypedefTag, JSDocCallbackTag, JSDocSignature>

    type DeclarationWithTypeParameterChildren =
        U5<SignatureDeclaration, ClassLikeDeclaration, InterfaceDeclaration, TypeAliasDeclaration, JSDocTemplateTag>

    type [<AllowNullLiteral>] ClassLikeDeclarationBase =
        inherit NamedDeclaration
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract name: Identifier option
        abstract typeParameters: ResizeArray<TypeParameterDeclaration> option
        abstract heritageClauses: ResizeArray<HeritageClause> option
        abstract members: ResizeArray<ClassElement>

    type [<AllowNullLiteral>] ClassDeclaration =
        inherit ClassLikeDeclarationBase
        inherit DeclarationStatement
        abstract kind: SyntaxKind
        abstract modifiers: ResizeArray<ModifierLike> option
        /// <summary>May be undefined in <c>export default class { ... }</c>.</summary>
        abstract name: Identifier option

    type [<AllowNullLiteral>] ClassExpression =
        inherit ClassLikeDeclarationBase
        inherit PrimaryExpression
        abstract kind: SyntaxKind
        abstract modifiers: ResizeArray<ModifierLike> option

    type ClassLikeDeclaration =
        U2<ClassDeclaration, ClassExpression>

    type [<AllowNullLiteral>] ClassElement =
        inherit NamedDeclaration
        abstract _classElementBrand: obj option with get, set
        abstract name: PropertyName option

    type [<AllowNullLiteral>] TypeElement =
        inherit NamedDeclaration
        abstract _typeElementBrand: obj option with get, set
        abstract name: PropertyName option
        abstract questionToken: QuestionToken option

    type [<AllowNullLiteral>] InterfaceDeclaration =
        inherit DeclarationStatement
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract name: Identifier
        abstract typeParameters: ResizeArray<TypeParameterDeclaration> option
        abstract heritageClauses: ResizeArray<HeritageClause> option
        abstract members: ResizeArray<TypeElement>

    type [<AllowNullLiteral>] HeritageClause =
        inherit Node
        abstract kind: SyntaxKind
        abstract parent: U2<InterfaceDeclaration, ClassLikeDeclaration>
        abstract token: SyntaxKind
        abstract types: ResizeArray<ExpressionWithTypeArguments>

    type [<AllowNullLiteral>] TypeAliasDeclaration =
        inherit DeclarationStatement
        inherit JSDocContainer
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract name: Identifier
        abstract typeParameters: ResizeArray<TypeParameterDeclaration> option
        abstract ``type``: TypeNode

    type [<AllowNullLiteral>] EnumMember =
        inherit NamedDeclaration
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract parent: EnumDeclaration
        abstract name: PropertyName
        abstract initializer: Expression option

    type [<AllowNullLiteral>] EnumDeclaration =
        inherit DeclarationStatement
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract name: Identifier
        abstract members: ResizeArray<EnumMember>

    type ModuleName =
        U2<Identifier, StringLiteral>

    type ModuleBody =
        U2<NamespaceBody, JSDocNamespaceBody>

    type [<AllowNullLiteral>] AmbientModuleDeclaration =
        inherit ModuleDeclaration
        abstract body: ModuleBlock option

    type [<AllowNullLiteral>] ModuleDeclaration =
        inherit DeclarationStatement
        inherit JSDocContainer
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract parent: U2<ModuleBody, SourceFile>
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract name: ModuleName
        abstract body: U2<ModuleBody, JSDocNamespaceDeclaration> option

    type NamespaceBody =
        U2<ModuleBlock, NamespaceDeclaration>

    type [<AllowNullLiteral>] NamespaceDeclaration =
        inherit ModuleDeclaration
        abstract name: Identifier
        abstract body: NamespaceBody

    type JSDocNamespaceBody =
        U2<Identifier, JSDocNamespaceDeclaration>

    type [<AllowNullLiteral>] JSDocNamespaceDeclaration =
        inherit ModuleDeclaration
        abstract name: Identifier
        abstract body: JSDocNamespaceBody option

    type [<AllowNullLiteral>] ModuleBlock =
        inherit Node
        inherit Statement
        abstract kind: SyntaxKind
        abstract parent: ModuleDeclaration
        abstract statements: ResizeArray<Statement>

    type ModuleReference =
        U2<EntityName, ExternalModuleReference>

    /// One of:
    /// - import x = require("mod");
    /// - import x = M.x;
    type [<AllowNullLiteral>] ImportEqualsDeclaration =
        inherit DeclarationStatement
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract parent: U2<SourceFile, ModuleBlock>
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract name: Identifier
        abstract isTypeOnly: bool
        abstract moduleReference: ModuleReference

    type [<AllowNullLiteral>] ExternalModuleReference =
        inherit Node
        abstract kind: SyntaxKind
        abstract parent: ImportEqualsDeclaration
        abstract expression: Expression

    type [<AllowNullLiteral>] ImportDeclaration =
        inherit Statement
        abstract kind: SyntaxKind
        abstract parent: U2<SourceFile, ModuleBlock>
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract importClause: ImportClause option
        /// If this is not a StringLiteral it will be a grammar error.
        abstract moduleSpecifier: Expression
        [<Obsolete("")>]
        abstract assertClause: AssertClause option
        abstract attributes: ImportAttributes option

    type NamedImportBindings =
        U2<NamespaceImport, NamedImports>

    type NamedExportBindings =
        U2<NamespaceExport, NamedExports>

    type [<AllowNullLiteral>] ImportClause =
        inherit NamedDeclaration
        abstract kind: SyntaxKind
        abstract parent: U2<ImportDeclaration, JSDocImportTag>
        [<Obsolete("Use `phaseModifier` instead")>]
        abstract isTypeOnly: bool
        abstract phaseModifier: ImportPhaseModifierSyntaxKind option
        abstract name: Identifier option
        abstract namedBindings: NamedImportBindings option

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// SyntaxKind.TypeKeyword | SyntaxKind.DeferKeyword
    /// </code>
    /// </remarks>
    type ImportPhaseModifierSyntaxKind =
        SyntaxKind

    [<Obsolete("")>]
    type AssertionKey =
        ImportAttributeName

    [<Obsolete("")>]
    type [<AllowNullLiteral>] AssertEntry =
        inherit ImportAttribute

    [<Obsolete("")>]
    type [<AllowNullLiteral>] AssertClause =
        inherit ImportAttributes

    type ImportAttributeName =
        U2<Identifier, StringLiteral>

    type [<AllowNullLiteral>] ImportAttribute =
        inherit Node
        abstract kind: SyntaxKind
        abstract parent: ImportAttributes
        abstract name: ImportAttributeName
        abstract value: Expression

    type [<AllowNullLiteral>] ImportAttributes =
        inherit Node
        abstract token: SyntaxKind
        abstract kind: SyntaxKind
        abstract parent: U2<ImportDeclaration, ExportDeclaration>
        abstract elements: ResizeArray<ImportAttribute>
        abstract multiLine: bool option

    type [<AllowNullLiteral>] NamespaceImport =
        inherit NamedDeclaration
        abstract kind: SyntaxKind
        abstract parent: ImportClause
        abstract name: Identifier

    type [<AllowNullLiteral>] NamespaceExport =
        inherit NamedDeclaration
        abstract kind: SyntaxKind
        abstract parent: ExportDeclaration
        abstract name: ModuleExportName

    type [<AllowNullLiteral>] NamespaceExportDeclaration =
        inherit DeclarationStatement
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract name: Identifier
        abstract modifiers: ResizeArray<ModifierLike> option

    type [<AllowNullLiteral>] ExportDeclaration =
        inherit DeclarationStatement
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract parent: U2<SourceFile, ModuleBlock>
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract isTypeOnly: bool
        /// <summary>Will not be assigned in the case of <c>export * from "foo";</c></summary>
        abstract exportClause: NamedExportBindings option
        /// If this is not a StringLiteral it will be a grammar error.
        abstract moduleSpecifier: Expression option
        [<Obsolete("")>]
        abstract assertClause: AssertClause option
        abstract attributes: ImportAttributes option

    type [<AllowNullLiteral>] NamedImports =
        inherit Node
        abstract kind: SyntaxKind
        abstract parent: ImportClause
        abstract elements: ResizeArray<ImportSpecifier>

    type [<AllowNullLiteral>] NamedExports =
        inherit Node
        abstract kind: SyntaxKind
        abstract parent: ExportDeclaration
        abstract elements: ResizeArray<ExportSpecifier>

    type NamedImportsOrExports =
        U2<NamedImports, NamedExports>

    type [<AllowNullLiteral>] ImportSpecifier =
        inherit NamedDeclaration
        abstract kind: SyntaxKind
        abstract parent: NamedImports
        abstract propertyName: ModuleExportName option
        abstract name: Identifier
        abstract isTypeOnly: bool

    type [<AllowNullLiteral>] ExportSpecifier =
        inherit NamedDeclaration
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract parent: NamedExports
        abstract isTypeOnly: bool
        abstract propertyName: ModuleExportName option
        abstract name: ModuleExportName

    type ModuleExportName =
        U2<Identifier, StringLiteral>

    type ImportOrExportSpecifier =
        U2<ImportSpecifier, ExportSpecifier>

    type TypeOnlyCompatibleAliasDeclaration =
        U6<ImportClause, ImportEqualsDeclaration, NamespaceImport, ImportOrExportSpecifier, ExportDeclaration, NamespaceExport>

    type TypeOnlyImportDeclaration =
        obj

    type TypeOnlyExportDeclaration =
        obj

    type TypeOnlyAliasDeclaration =
        U2<TypeOnlyImportDeclaration, TypeOnlyExportDeclaration>

    /// <summary>
    /// This is either an <c>export =</c> or an <c>export default</c> declaration.
    /// Unless <c>isExportEquals</c> is set, this node was parsed as an <c>export default</c>.
    /// </summary>
    type [<AllowNullLiteral>] ExportAssignment =
        inherit DeclarationStatement
        inherit JSDocContainer
        abstract kind: SyntaxKind
        abstract parent: SourceFile
        abstract modifiers: ResizeArray<ModifierLike> option
        abstract isExportEquals: bool option
        abstract expression: Expression

    type [<AllowNullLiteral>] FileReference =
        inherit TextRange
        abstract fileName: string with get, set
        abstract resolutionMode: ResolutionMode option with get, set
        abstract preserve: bool option with get, set

    type [<AllowNullLiteral>] CheckJsDirective =
        inherit TextRange
        abstract enabled: bool with get, set

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// SyntaxKind.SingleLineCommentTrivia | SyntaxKind.MultiLineCommentTrivia
    /// </code>
    /// </remarks>
    type CommentKind =
        SyntaxKind

    type [<AllowNullLiteral>] CommentRange =
        inherit TextRange
        abstract hasTrailingNewLine: bool option with get, set
        abstract kind: CommentKind with get, set

    type [<AllowNullLiteral>] SynthesizedComment =
        inherit CommentRange
        abstract text: string with get, set
        abstract pos: int with get, set
        abstract ``end``: int with get, set
        abstract hasLeadingNewline: bool option with get, set

    type [<AllowNullLiteral>] JSDocTypeExpression =
        inherit TypeNode
        abstract kind: SyntaxKind
        abstract ``type``: TypeNode

    type [<AllowNullLiteral>] JSDocNameReference =
        inherit Node
        abstract kind: SyntaxKind
        abstract name: U2<EntityName, JSDocMemberName>

    /// Class#method reference in JSDoc
    type [<AllowNullLiteral>] JSDocMemberName =
        inherit Node
        abstract kind: SyntaxKind
        abstract left: U2<EntityName, JSDocMemberName>
        abstract right: Identifier

    type [<AllowNullLiteral>] JSDocType =
        inherit TypeNode
        abstract _jsDocTypeBrand: obj option with get, set

    type [<AllowNullLiteral>] JSDocAllType =
        inherit JSDocType
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] JSDocUnknownType =
        inherit JSDocType
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] JSDocNonNullableType =
        inherit JSDocType
        abstract kind: SyntaxKind
        abstract ``type``: TypeNode
        abstract postfix: bool

    type [<AllowNullLiteral>] JSDocNullableType =
        inherit JSDocType
        abstract kind: SyntaxKind
        abstract ``type``: TypeNode
        abstract postfix: bool

    type [<AllowNullLiteral>] JSDocOptionalType =
        inherit JSDocType
        abstract kind: SyntaxKind
        abstract ``type``: TypeNode

    type [<AllowNullLiteral>] JSDocFunctionType =
        inherit JSDocType
        inherit SignatureDeclarationBase
        inherit LocalsContainer
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] JSDocVariadicType =
        inherit JSDocType
        abstract kind: SyntaxKind
        abstract ``type``: TypeNode

    type [<AllowNullLiteral>] JSDocNamepathType =
        inherit JSDocType
        abstract kind: SyntaxKind
        abstract ``type``: TypeNode

    type JSDocTypeReferencingNode =
        U4<JSDocVariadicType, JSDocOptionalType, JSDocNullableType, JSDocNonNullableType>

    type [<AllowNullLiteral>] JSDoc =
        inherit Node
        abstract kind: SyntaxKind
        abstract parent: HasJSDoc
        abstract tags: ResizeArray<JSDocTag> option
        abstract comment: U2<string, ResizeArray<JSDocComment>> option

    type [<AllowNullLiteral>] JSDocTag =
        inherit Node
        abstract parent: U2<JSDoc, JSDocTypeLiteral>
        abstract tagName: Identifier
        abstract comment: U2<string, ResizeArray<JSDocComment>> option

    type [<AllowNullLiteral>] JSDocLink =
        inherit Node
        abstract kind: SyntaxKind
        abstract name: U2<EntityName, JSDocMemberName> option
        abstract text: string with get, set

    type [<AllowNullLiteral>] JSDocLinkCode =
        inherit Node
        abstract kind: SyntaxKind
        abstract name: U2<EntityName, JSDocMemberName> option
        abstract text: string with get, set

    type [<AllowNullLiteral>] JSDocLinkPlain =
        inherit Node
        abstract kind: SyntaxKind
        abstract name: U2<EntityName, JSDocMemberName> option
        abstract text: string with get, set

    type JSDocComment =
        U4<JSDocText, JSDocLink, JSDocLinkCode, JSDocLinkPlain>

    type [<AllowNullLiteral>] JSDocText =
        inherit Node
        abstract kind: SyntaxKind
        abstract text: string with get, set

    type [<AllowNullLiteral>] JSDocUnknownTag =
        inherit JSDocTag
        abstract kind: SyntaxKind

    /// <summary>
    /// Note that <c>@extends</c> is a synonym of <c>@augments</c>.
    /// Both tags are represented by this interface.
    /// </summary>
    type [<AllowNullLiteral>] JSDocAugmentsTag =
        inherit JSDocTag
        abstract kind: SyntaxKind
        abstract ``class``: obj

    type [<AllowNullLiteral>] JSDocImplementsTag =
        inherit JSDocTag
        abstract kind: SyntaxKind
        abstract ``class``: obj

    type [<AllowNullLiteral>] JSDocAuthorTag =
        inherit JSDocTag
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] JSDocDeprecatedTag =
        inherit JSDocTag
        abstract kind: SyntaxKind with get, set

    type [<AllowNullLiteral>] JSDocClassTag =
        inherit JSDocTag
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] JSDocPublicTag =
        inherit JSDocTag
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] JSDocPrivateTag =
        inherit JSDocTag
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] JSDocProtectedTag =
        inherit JSDocTag
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] JSDocReadonlyTag =
        inherit JSDocTag
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] JSDocOverrideTag =
        inherit JSDocTag
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] JSDocEnumTag =
        inherit JSDocTag
        inherit Declaration
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract parent: JSDoc
        abstract typeExpression: JSDocTypeExpression

    type [<AllowNullLiteral>] JSDocThisTag =
        inherit JSDocTag
        abstract kind: SyntaxKind
        abstract typeExpression: JSDocTypeExpression

    type [<AllowNullLiteral>] JSDocTemplateTag =
        inherit JSDocTag
        abstract kind: SyntaxKind
        abstract ``constraint``: JSDocTypeExpression option
        abstract typeParameters: ResizeArray<TypeParameterDeclaration>

    type [<AllowNullLiteral>] JSDocSeeTag =
        inherit JSDocTag
        abstract kind: SyntaxKind
        abstract name: JSDocNameReference option

    type [<AllowNullLiteral>] JSDocReturnTag =
        inherit JSDocTag
        abstract kind: SyntaxKind
        abstract typeExpression: JSDocTypeExpression option

    type [<AllowNullLiteral>] JSDocTypeTag =
        inherit JSDocTag
        abstract kind: SyntaxKind
        abstract typeExpression: JSDocTypeExpression

    type [<AllowNullLiteral>] JSDocTypedefTag =
        inherit JSDocTag
        inherit NamedDeclaration
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract parent: JSDoc
        abstract fullName: U2<JSDocNamespaceDeclaration, Identifier> option
        abstract name: Identifier option
        abstract typeExpression: U2<JSDocTypeExpression, JSDocTypeLiteral> option

    type [<AllowNullLiteral>] JSDocCallbackTag =
        inherit JSDocTag
        inherit NamedDeclaration
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract parent: JSDoc
        abstract fullName: U2<JSDocNamespaceDeclaration, Identifier> option
        abstract name: Identifier option
        abstract typeExpression: JSDocSignature

    type [<AllowNullLiteral>] JSDocOverloadTag =
        inherit JSDocTag
        abstract kind: SyntaxKind
        abstract parent: JSDoc
        abstract typeExpression: JSDocSignature

    type [<AllowNullLiteral>] JSDocThrowsTag =
        inherit JSDocTag
        abstract kind: SyntaxKind
        abstract typeExpression: JSDocTypeExpression option

    type [<AllowNullLiteral>] JSDocSignature =
        inherit JSDocType
        inherit Declaration
        inherit JSDocContainer
        inherit LocalsContainer
        abstract kind: SyntaxKind
        abstract typeParameters: ResizeArray<JSDocTemplateTag> option
        abstract parameters: ResizeArray<JSDocParameterTag>
        abstract ``type``: JSDocReturnTag option

    type [<AllowNullLiteral>] JSDocPropertyLikeTag =
        inherit JSDocTag
        inherit Declaration
        abstract parent: JSDoc
        abstract name: EntityName
        abstract typeExpression: JSDocTypeExpression option
        /// Whether the property name came before the type -- non-standard for JSDoc, but Typescript-like
        abstract isNameFirst: bool
        abstract isBracketed: bool

    type [<AllowNullLiteral>] JSDocPropertyTag =
        inherit JSDocPropertyLikeTag
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] JSDocParameterTag =
        inherit JSDocPropertyLikeTag
        abstract kind: SyntaxKind

    type [<AllowNullLiteral>] JSDocTypeLiteral =
        inherit JSDocType
        inherit Declaration
        abstract kind: SyntaxKind
        abstract jsDocPropertyTags: ResizeArray<JSDocPropertyLikeTag> option
        /// If true, then this type literal represents an *array* of its type.
        abstract isArrayType: bool

    type [<AllowNullLiteral>] JSDocSatisfiesTag =
        inherit JSDocTag
        abstract kind: SyntaxKind
        abstract typeExpression: JSDocTypeExpression

    type [<AllowNullLiteral>] JSDocSatisfiesExpression =
        inherit ParenthesizedExpression
        abstract _jsDocSatisfiesExpressionBrand: obj

    type [<AllowNullLiteral>] JSDocImportTag =
        inherit JSDocTag
        abstract kind: SyntaxKind
        abstract parent: JSDoc
        abstract importClause: ImportClause option
        abstract moduleSpecifier: Expression
        abstract attributes: ImportAttributes option

    type [<RequireQualifiedAccess>] FlowFlags =
        | Unreachable = 1
        | Start = 2
        | BranchLabel = 4
        | LoopLabel = 8
        | Assignment = 16
        | TrueCondition = 32
        | FalseCondition = 64
        | SwitchClause = 128
        | ArrayMutation = 256
        | Call = 512
        | ReduceLabel = 1024
        | Referenced = 2048
        | Shared = 4096
        | Label = 12
        | Condition = 96

    type FlowNode =
        U8<FlowStart, FlowLabel, FlowAssignment, FlowCondition, FlowSwitchClause, FlowArrayMutation, FlowCall, FlowReduceLabel>

    type [<AllowNullLiteral>] FlowNodeBase =
        abstract flags: FlowFlags with get, set
        abstract id: float with get, set
        abstract node: obj with get, set
        abstract antecedent: U2<FlowNode, ResizeArray<FlowNode>> option with get, set

    type [<AllowNullLiteral>] FlowUnreachable =
        inherit FlowNodeBase
        abstract node: obj with get, set
        abstract antecedent: obj with get, set

    type [<AllowNullLiteral>] FlowStart =
        inherit FlowNodeBase
        abstract node: U5<FunctionExpression, ArrowFunction, MethodDeclaration, GetAccessorDeclaration, SetAccessorDeclaration> option with get, set
        abstract antecedent: obj with get, set

    type [<AllowNullLiteral>] FlowLabel =
        inherit FlowNodeBase
        abstract node: obj with get, set
        abstract antecedent: ResizeArray<FlowNode> option with get, set

    type [<AllowNullLiteral>] FlowAssignment =
        inherit FlowNodeBase
        abstract node: U3<Expression, VariableDeclaration, BindingElement> with get, set
        abstract antecedent: FlowNode with get, set

    type [<AllowNullLiteral>] FlowCall =
        inherit FlowNodeBase
        abstract node: CallExpression with get, set
        abstract antecedent: FlowNode with get, set

    type [<AllowNullLiteral>] FlowCondition =
        inherit FlowNodeBase
        abstract node: Expression with get, set
        abstract antecedent: FlowNode with get, set

    type [<AllowNullLiteral>] FlowSwitchClause =
        inherit FlowNodeBase
        abstract node: FlowSwitchClauseData with get, set
        abstract antecedent: FlowNode with get, set

    type [<AllowNullLiteral>] FlowSwitchClauseData =
        abstract switchStatement: SwitchStatement with get, set
        abstract clauseStart: float with get, set
        abstract clauseEnd: float with get, set

    type [<AllowNullLiteral>] FlowArrayMutation =
        inherit FlowNodeBase
        abstract node: U2<CallExpression, BinaryExpression> with get, set
        abstract antecedent: FlowNode with get, set

    type [<AllowNullLiteral>] FlowReduceLabel =
        inherit FlowNodeBase
        abstract node: FlowReduceLabelData with get, set
        abstract antecedent: FlowNode with get, set

    type [<AllowNullLiteral>] FlowReduceLabelData =
        abstract target: FlowLabel with get, set
        abstract antecedents: ResizeArray<FlowNode> with get, set

    type FlowType =
        U2<Type, IncompleteType>

    type [<AllowNullLiteral>] IncompleteType =
        abstract flags: U2<TypeFlags, float> with get, set
        abstract ``type``: Type with get, set

    type [<AllowNullLiteral>] AmdDependency =
        abstract path: string with get, set
        abstract name: string option with get, set

    /// Subset of properties from SourceFile that are used in multiple utility functions
    type [<AllowNullLiteral>] SourceFileLike =
        abstract text: string
        abstract lineMap: ResizeArray<float> option with get, set
        [<Emit("$0.getPositionOfLineAndCharacter($1,$2,true)")>] abstract getPositionOfLineAndCharacter_true: line: float * character: float -> float

    type [<AllowNullLiteral>] FutureSourceFile =
        abstract path: Path
        abstract fileName: string
        abstract impliedNodeFormat: ResolutionMode option
        abstract packageJsonScope: PackageJsonInfo option
        abstract externalModuleIndicator: obj option
        abstract commonJsModuleIndicator: obj option
        abstract statements: ResizeArray<obj>
        abstract imports: ResizeArray<obj>

    type [<AllowNullLiteral>] RedirectInfo =
        /// Source file this redirects to.
        abstract redirectTarget: SourceFile
        /// Source file for the duplicate package. This will not be used by the Program,
        /// but we need to keep this around so we can watch for changes in underlying.
        abstract unredirected: SourceFile

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// ModuleKind.ESNext | ModuleKind.CommonJS | undefined
    /// </code>
    /// </remarks>
    type ResolutionMode =
        ModuleKind

    type [<AllowNullLiteral>] SourceFile =
        inherit Declaration
        inherit LocalsContainer
        inherit ReadonlyPragmaContext
        abstract kind: SyntaxKind
        abstract statements: ResizeArray<Statement>
        abstract endOfFileToken: Token<SyntaxKind>
        abstract fileName: string with get, set
        abstract path: Path with get, set
        abstract text: string with get, set
        /// <summary>
        /// Resolved path can be different from path property,
        /// when file is included through project reference is mapped to its output instead of source
        /// in that case resolvedPath = path to output file
        /// path = input file's path
        /// </summary>
        abstract resolvedPath: Path with get, set
        /// <summary>
        /// Original file name that can be different from fileName,
        /// when file is included through project reference is mapped to its output instead of source
        /// in that case originalFileName = name of input file
        /// fileName = output file's name
        /// </summary>
        abstract originalFileName: string with get, set
        /// <summary>
        /// If two source files are for the same version of the same package, one will redirect to the other.
        /// (See <c>createRedirectSourceFile</c> in program.ts.)
        /// The redirect will have this set. The redirected-to source file will be in <c>redirectTargetsMap</c>.
        /// </summary>
        abstract redirectInfo: RedirectInfo option with get, set
        abstract amdDependencies: ResizeArray<AmdDependency> with get, set
        abstract moduleName: string option with get, set
        abstract referencedFiles: ResizeArray<FileReference> with get, set
        abstract typeReferenceDirectives: ResizeArray<FileReference> with get, set
        abstract libReferenceDirectives: ResizeArray<FileReference> with get, set
        abstract languageVariant: LanguageVariant with get, set
        abstract isDeclarationFile: bool with get, set
        abstract renamedDependencies: ReadonlyMap<string, string> option with get, set
        /// lib.d.ts should have a reference comment like
        ///
        ///  /// <reference no-default-lib="true"/>
        ///
        /// If any other file has this comment, it signals not to include lib.d.ts
        /// because this containing file is intended to act as a default library.
        abstract hasNoDefaultLib: bool with get, set
        abstract languageVersion: ScriptTarget with get, set
        /// <summary>
        /// When <c>module</c> is <c>Node16</c> or <c>NodeNext</c>, this field controls whether the
        /// source file in question is an ESNext-output-format file, or a CommonJS-output-format
        /// module. This is derived by the module resolver as it looks up the file, since
        /// it is derived from either the file extension of the module, or the containing
        /// <c>package.json</c> context, and affects both checking and emit.
        ///
        /// It is _public_ so that (pre)transformers can set this field,
        /// since it switches the builtin <c>node</c> module transform. Generally speaking, if unset,
        /// the field is treated as though it is <c>ModuleKind.CommonJS</c>.
        ///
        /// Note that this field is only set by the module resolution process when
        /// <c>moduleResolution</c> is <c>Node16</c> or <c>NodeNext</c>, which is implied by the <c>module</c> setting
        /// of <c>Node16</c> or <c>NodeNext</c>, respectively, but may be overriden (eg, by a <c>moduleResolution</c>
        /// of <c>node</c>). If so, this field will be unset and source files will be considered to be
        /// CommonJS-output-format by the node module transformer and type checker, regardless of extension or context.
        /// </summary>
        abstract impliedNodeFormat: ResolutionMode option with get, set
        abstract packageJsonLocations: ResizeArray<string> option with get, set
        abstract packageJsonScope: PackageJsonInfo option with get, set
        abstract scriptKind: ScriptKind with get, set
        /// <summary>
        /// The first "most obvious" node that makes a file an external module.
        /// This is intended to be the first top-level import/export,
        /// but could be arbitrarily nested (e.g. <c>import.meta</c>).
        /// </summary>
        abstract externalModuleIndicator: Node option with get, set
        /// <summary>
        /// The callback used to set the external module indicator - this is saved to
        /// be later reused during incremental reparsing, which otherwise lacks the information
        /// to set this field
        /// </summary>
        abstract setExternalModuleIndicator: (SourceFile -> unit) option with get, set
        abstract commonJsModuleIndicator: Node option with get, set
        abstract jsGlobalAugmentations: SymbolTable option with get, set
        abstract identifiers: ReadonlyMap<string, string> with get, set
        abstract nodeCount: float with get, set
        abstract identifierCount: float with get, set
        abstract symbolCount: float with get, set
        abstract parseDiagnostics: ResizeArray<DiagnosticWithLocation> with get, set
        abstract bindDiagnostics: ResizeArray<DiagnosticWithLocation> with get, set
        abstract bindSuggestionDiagnostics: ResizeArray<DiagnosticWithLocation> option with get, set
        abstract jsDocDiagnostics: ResizeArray<DiagnosticWithLocation> option with get, set
        abstract additionalSyntacticDiagnostics: ResizeArray<DiagnosticWithLocation> option with get, set
        abstract lineMap: ResizeArray<float> with get, set
        abstract classifiableNames: ReadonlySet<__String> option with get, set
        abstract commentDirectives: ResizeArray<CommentDirective> option with get, set
        abstract imports: ResizeArray<StringLiteralLike> with get, set
        abstract moduleAugmentations: ResizeArray<U2<StringLiteral, Identifier>> with get, set
        abstract patternAmbientModules: ResizeArray<PatternAmbientModule> option with get, set
        abstract ambientModuleNames: ResizeArray<string> with get, set
        abstract checkJsDirective: CheckJsDirective option with get, set
        abstract version: string with get, set
        abstract pragmas: ReadonlyPragmaMap with get, set
        abstract localJsxNamespace: __String option with get, set
        abstract localJsxFragmentNamespace: __String option with get, set
        abstract localJsxFactory: EntityName option with get, set
        abstract localJsxFragmentFactory: EntityName option with get, set
        abstract endFlowNode: FlowNode option with get, set
        abstract jsDocParsingMode: JSDocParsingMode option with get, set

    type [<AllowNullLiteral>] ReadonlyPragmaContext =
        abstract languageVersion: ScriptTarget with get, set
        abstract pragmas: ReadonlyPragmaMap option with get, set
        abstract checkJsDirective: CheckJsDirective option with get, set
        abstract referencedFiles: ResizeArray<FileReference> with get, set
        abstract typeReferenceDirectives: ResizeArray<FileReference> with get, set
        abstract libReferenceDirectives: ResizeArray<FileReference> with get, set
        abstract amdDependencies: ResizeArray<AmdDependency> with get, set
        abstract hasNoDefaultLib: bool option with get, set
        abstract moduleName: string option with get, set

    type [<AllowNullLiteral>] PragmaContext =
        inherit ReadonlyPragmaContext
        abstract pragmas: PragmaMap option with get, set
        abstract referencedFiles: ResizeArray<FileReference> with get, set
        abstract typeReferenceDirectives: ResizeArray<FileReference> with get, set
        abstract libReferenceDirectives: ResizeArray<FileReference> with get, set
        abstract amdDependencies: ResizeArray<AmdDependency> with get, set

    type [<AllowNullLiteral>] CommentDirective =
        abstract range: TextRange with get, set
        abstract ``type``: CommentDirectiveType with get, set

    type [<RequireQualifiedAccess>] CommentDirectiveType =
        | ExpectError = 0
        | Ignore = 1

    type [<AllowNullLiteral>] Bundle =
        inherit Node
        abstract kind: SyntaxKind
        abstract sourceFiles: ResizeArray<SourceFile>
        abstract syntheticFileReferences: ResizeArray<FileReference> option with get, set
        abstract syntheticTypeReferences: ResizeArray<FileReference> option with get, set
        abstract syntheticLibReferences: ResizeArray<FileReference> option with get, set
        abstract hasNoDefaultLib: bool option with get, set

    type [<AllowNullLiteral>] JsonSourceFile =
        inherit SourceFile
        abstract statements: ResizeArray<JsonObjectExpressionStatement>

    type [<AllowNullLiteral>] TsConfigSourceFile =
        inherit JsonSourceFile
        abstract extendedSourceFiles: ResizeArray<string> option with get, set
        abstract configFileSpecs: ConfigFileSpecs option with get, set

    type [<AllowNullLiteral>] JsonMinusNumericLiteral =
        inherit PrefixUnaryExpression
        abstract kind: SyntaxKind
        abstract operator: SyntaxKind
        abstract operand: NumericLiteral

    type JsonObjectExpression =
        U7<ObjectLiteralExpression, ArrayLiteralExpression, JsonMinusNumericLiteral, NumericLiteral, StringLiteral, BooleanLiteral, NullLiteral>

    type [<AllowNullLiteral>] JsonObjectExpressionStatement =
        inherit ExpressionStatement
        abstract expression: JsonObjectExpression

    type [<AllowNullLiteral>] ScriptReferenceHost =
        abstract getCompilerOptions: unit -> CompilerOptions
        abstract getSourceFile: fileName: string -> SourceFile option
        abstract getSourceFileByPath: path: Path -> SourceFile option
        abstract getCurrentDirectory: unit -> string

    type [<AllowNullLiteral>] ParseConfigHost =
        inherit ModuleResolutionHost
        abstract useCaseSensitiveFileNames: bool with get, set
        abstract readDirectory: rootDir: string * extensions: ResizeArray<string> * excludes: ResizeArray<string> option * includes: ResizeArray<string> * ?depth: float -> ResizeArray<string>
        /// <summary>Gets a value indicating whether the specified path exists and is a file.</summary>
        /// <param name="path">The path to test.</param>
        abstract fileExists: path: string -> bool
        abstract readFile: path: string -> string option
        abstract trace: s: string -> unit

    /// Branded string for keeping track of when we've turned an ambiguous path
    /// specified like "./blah" to an absolute path to an actual
    /// tsconfig file, e.g. "/root/blah/tsconfig.json"
    type [<AllowNullLiteral>] ResolvedConfigFileName =
        interface end

    type [<AllowNullLiteral>] ResolvedRefAndOutputDts =
        abstract resolvedRef: ResolvedProjectReference with get, set
        abstract outputDts: string option with get, set

    type [<AllowNullLiteral>] ResolvedRefAndSource =
        abstract resolvedRef: ResolvedProjectReference with get, set
        abstract source: string option with get, set

    type [<AllowNullLiteral>] WriteFileCallbackData =
        abstract sourceMapUrlPos: float option with get, set
        abstract buildInfo: BuildInfo option with get, set
        abstract diagnostics: ResizeArray<DiagnosticWithLocation> option with get, set
        abstract differsOnlyInMap: bool option with get, set
        abstract skippedDtsWrite: bool option with get, set

    type [<AllowNullLiteral>] WriteFileCallback =
        [<Emit("$0($1...)")>] abstract Invoke: fileName: string * text: string * writeByteOrderMark: bool * ?onError: (string -> unit) * ?sourceFiles: ResizeArray<SourceFile> * ?data: WriteFileCallbackData -> unit

    type [<AllowNullLiteral>] OperationCanceledException =
        interface end

    type [<AllowNullLiteral>] OperationCanceledExceptionStatic =
        [<EmitConstructor>] abstract Create: unit -> OperationCanceledException

    type [<AllowNullLiteral>] CancellationToken =
        abstract isCancellationRequested: unit -> bool
        /// <exception cref="">OperationCanceledException if isCancellationRequested is true</exception>
        abstract throwIfCancellationRequested: unit -> unit

    type [<RequireQualifiedAccess>] FileIncludeKind =
        | RootFile = 0
        | SourceFromProjectReference = 1
        | OutputFromProjectReference = 2
        | Import = 3
        | ReferenceFile = 4
        | TypeReferenceDirective = 5
        | LibFile = 6
        | LibReferenceDirective = 7
        | AutomaticTypeDirectiveFile = 8

    type [<AllowNullLiteral>] RootFile =
        abstract kind: FileIncludeKind with get, set
        abstract index: float with get, set

    type [<AllowNullLiteral>] LibFile =
        abstract kind: FileIncludeKind with get, set
        abstract index: float option with get, set

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | FileIncludeKind.SourceFromProjectReference
    ///     | FileIncludeKind.OutputFromProjectReference
    /// </code>
    /// </remarks>
    type ProjectReferenceFileKind =
        FileIncludeKind

    type [<AllowNullLiteral>] ProjectReferenceFile =
        abstract kind: ProjectReferenceFileKind with get, set
        abstract index: float with get, set

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | FileIncludeKind.Import
    ///     | FileIncludeKind.ReferenceFile
    ///     | FileIncludeKind.TypeReferenceDirective
    ///     | FileIncludeKind.LibReferenceDirective
    /// </code>
    /// </remarks>
    type ReferencedFileKind =
        FileIncludeKind

    type [<AllowNullLiteral>] ReferencedFile =
        abstract kind: ReferencedFileKind with get, set
        abstract file: Path with get, set
        abstract index: float with get, set

    type [<AllowNullLiteral>] AutomaticTypeDirectiveFile =
        abstract kind: FileIncludeKind with get, set
        abstract typeReference: string with get, set
        abstract packageId: PackageId option with get, set

    type FileIncludeReason =
        U5<RootFile, LibFile, ProjectReferenceFile, ReferencedFile, AutomaticTypeDirectiveFile>

    type [<RequireQualifiedAccess>] FilePreprocessingDiagnosticsKind =
        | FilePreprocessingLibReferenceDiagnostic = 0
        | FilePreprocessingFileExplainingDiagnostic = 1
        | ResolutionDiagnostics = 2

    type [<AllowNullLiteral>] FilePreprocessingLibReferenceDiagnostic =
        abstract kind: FilePreprocessingDiagnosticsKind with get, set
        abstract reason: obj with get, set

    type [<AllowNullLiteral>] FilePreprocessingFileExplainingDiagnostic =
        abstract kind: FilePreprocessingDiagnosticsKind with get, set
        abstract file: Path option with get, set
        abstract fileProcessingReason: FileIncludeReason with get, set
        abstract diagnostic: DiagnosticMessage with get, set
        abstract args: DiagnosticArguments with get, set

    type [<AllowNullLiteral>] ResolutionDiagnostics =
        abstract kind: FilePreprocessingDiagnosticsKind with get, set
        abstract diagnostics: ResizeArray<Diagnostic> with get, set

    type FilePreprocessingDiagnostics =
        U3<FilePreprocessingLibReferenceDiagnostic, FilePreprocessingFileExplainingDiagnostic, ResolutionDiagnostics>

    type [<RequireQualifiedAccess>] EmitOnly =
        | Js = 0
        | Dts = 1
        | BuilderSignature = 2

    type LibResolution =
        LibResolution<ResolvedModuleWithFailedLookupLocations>

    type [<AllowNullLiteral>] LibResolution<'T when 'T :> ResolvedModuleWithFailedLookupLocations> =
        abstract resolution: 'T with get, set
        abstract actual: string with get, set

    type [<AllowNullLiteral>] Program =
        inherit ScriptReferenceHost
        inherit TypeCheckerHost
        inherit ModuleSpecifierResolutionHost
        abstract getCurrentDirectory: unit -> string
        /// Get a list of root file names that were passed to a 'createProgram'
        abstract getRootFileNames: unit -> ResizeArray<string>
        /// Get a list of files in the program
        abstract getSourceFiles: unit -> ResizeArray<SourceFile>
        /// <summary>
        /// Get a list of file names that were passed to 'createProgram' or referenced in a
        /// program source file but could not be located.
        /// </summary>
        abstract getMissingFilePaths: unit -> Map<Path, string>
        abstract getModuleResolutionCache: unit -> ModuleResolutionCache option
        abstract getFilesByNameMap: unit -> Map<Path, SourceFile option>
        abstract resolvedModules: Map<Path, ModeAwareCache<ResolvedModuleWithFailedLookupLocations>> option with get, set
        abstract resolvedTypeReferenceDirectiveNames: Map<Path, ModeAwareCache<ResolvedTypeReferenceDirectiveWithFailedLookupLocations>> option with get, set
        abstract getResolvedModule: f: SourceFile * moduleName: string * mode: ResolutionMode -> ResolvedModuleWithFailedLookupLocations option
        abstract getResolvedModuleFromModuleSpecifier: moduleSpecifier: StringLiteralLike * ?sourceFile: SourceFile -> ResolvedModuleWithFailedLookupLocations option
        abstract getResolvedTypeReferenceDirective: f: SourceFile * typeDirectiveName: string * mode: ResolutionMode -> ResolvedTypeReferenceDirectiveWithFailedLookupLocations option
        abstract getResolvedTypeReferenceDirectiveFromTypeReferenceDirective: typedRef: FileReference * sourceFile: SourceFile -> ResolvedTypeReferenceDirectiveWithFailedLookupLocations option
        abstract forEachResolvedModule: callback: (ResolvedModuleWithFailedLookupLocations -> string -> ResolutionMode -> Path -> unit) * ?file: SourceFile -> unit
        abstract forEachResolvedTypeReferenceDirective: callback: (ResolvedTypeReferenceDirectiveWithFailedLookupLocations -> string -> ResolutionMode -> Path -> unit) * ?file: SourceFile -> unit
        /// Emits the JavaScript and declaration files.  If targetSourceFile is not specified, then
        /// the JavaScript and declaration files will be produced for all the files in this program.
        /// If targetSourceFile is specified, then only the JavaScript and declaration for that
        /// specific file will be generated.
        ///
        /// If writeFile is not specified then the writeFile callback from the compiler host will be
        /// used for writing the JavaScript and declaration files.  Otherwise, the writeFile parameter
        /// will be invoked when writing the JavaScript and declaration files.
        abstract emit: ?targetSourceFile: SourceFile * ?writeFile: WriteFileCallback * ?cancellationToken: CancellationToken * ?emitOnlyDtsFiles: bool * ?customTransformers: CustomTransformers -> EmitResult
        abstract emit: ?targetSourceFile: SourceFile * ?writeFile: WriteFileCallback * ?cancellationToken: CancellationToken * ?emitOnly: U2<bool, EmitOnly> * ?customTransformers: CustomTransformers * ?forceDtsEmit: bool * ?skipBuildInfo: bool -> EmitResult
        abstract getOptionsDiagnostics: ?cancellationToken: CancellationToken -> ResizeArray<Diagnostic>
        abstract getGlobalDiagnostics: ?cancellationToken: CancellationToken -> ResizeArray<Diagnostic>
        abstract getSyntacticDiagnostics: ?sourceFile: SourceFile * ?cancellationToken: CancellationToken -> ResizeArray<DiagnosticWithLocation>
        /// The first time this is called, it will return global diagnostics (no location).
        abstract getSemanticDiagnostics: ?sourceFile: SourceFile * ?cancellationToken: CancellationToken -> ResizeArray<Diagnostic>
        abstract getSemanticDiagnostics: sourceFile: SourceFile option * cancellationToken: CancellationToken option * nodesToCheck: ResizeArray<Node> -> ResizeArray<Diagnostic>
        abstract getDeclarationDiagnostics: ?sourceFile: SourceFile * ?cancellationToken: CancellationToken -> ResizeArray<DiagnosticWithLocation>
        abstract getConfigFileParsingDiagnostics: unit -> ResizeArray<Diagnostic>
        abstract getSuggestionDiagnostics: sourceFile: SourceFile * ?cancellationToken: CancellationToken -> ResizeArray<DiagnosticWithLocation>
        abstract getBindAndCheckDiagnostics: sourceFile: SourceFile * ?cancellationToken: CancellationToken -> ResizeArray<Diagnostic>
        abstract getProgramDiagnostics: sourceFile: SourceFile * ?cancellationToken: CancellationToken -> ResizeArray<Diagnostic>
        /// Gets a type checker that can be used to semantically analyze source files in the program.
        abstract getTypeChecker: unit -> TypeChecker
        abstract getCommonSourceDirectory: unit -> string
        abstract getCachedSemanticDiagnostics: sourceFile: SourceFile -> ResizeArray<Diagnostic> option
        abstract getClassifiableNames: unit -> Set<__String>
        abstract getNodeCount: unit -> float
        abstract getIdentifierCount: unit -> float
        abstract getSymbolCount: unit -> float
        abstract getTypeCount: unit -> float
        abstract getInstantiationCount: unit -> float
        abstract getRelationCacheSizes: unit -> {| assignable: float; identity: float; subtype: float; strictSubtype: float |}
        abstract getFileProcessingDiagnostics: unit -> ResizeArray<FilePreprocessingDiagnostics> option
        abstract getAutomaticTypeDirectiveNames: unit -> ResizeArray<string>
        abstract getAutomaticTypeDirectiveResolutions: unit -> ModeAwareCache<ResolvedTypeReferenceDirectiveWithFailedLookupLocations>
        abstract isSourceFileFromExternalLibrary: file: SourceFile -> bool
        abstract isSourceFileDefaultLibrary: file: SourceFile -> bool
        /// <summary>
        /// Calculates the final resolution mode for a given module reference node. This function only returns a result when module resolution
        /// settings allow differing resolution between ESM imports and CJS requires, or when a mode is explicitly provided via import attributes,
        /// which cause an <c>import</c> or <c>require</c> condition to be used during resolution regardless of module resolution settings. In absence of
        /// overriding attributes, and in modes that support differing resolution, the result indicates the syntax the usage would emit to JavaScript.
        /// Some examples:
        ///
        /// <code lang="ts">
        /// // tsc foo.mts --module nodenext
        /// import {} from "mod";
        /// // Result: ESNext - the import emits as ESM due to `impliedNodeFormat` set by .mts file extension
        ///
        /// // tsc foo.cts --module nodenext
        /// import {} from "mod";
        /// // Result: CommonJS - the import emits as CJS due to `impliedNodeFormat` set by .cts file extension
        ///
        /// // tsc foo.ts --module preserve --moduleResolution bundler
        /// import {} from "mod";
        /// // Result: ESNext - the import emits as ESM due to `--module preserve` and `--moduleResolution bundler`
        /// // supports conditional imports/exports
        ///
        /// // tsc foo.ts --module preserve --moduleResolution node10
        /// import {} from "mod";
        /// // Result: undefined - the import emits as ESM due to `--module preserve`, but `--moduleResolution node10`
        /// // does not support conditional imports/exports
        ///
        /// // tsc foo.ts --module commonjs --moduleResolution node10
        /// import type {} from "mod" with { "resolution-mode": "import" };
        /// // Result: ESNext - conditional imports/exports always supported with "resolution-mode" attribute
        /// </code>
        /// </summary>
        abstract getModeForUsageLocation: file: SourceFile * usage: StringLiteralLike -> ResolutionMode
        /// <summary>
        /// Calculates the final resolution mode for an import at some index within a file's <c>imports</c> list. This function only returns a result
        /// when module resolution settings allow differing resolution between ESM imports and CJS requires, or when a mode is explicitly provided
        /// via import attributes, which cause an <c>import</c> or <c>require</c> condition to be used during resolution regardless of module resolution
        /// settings. In absence of overriding attributes, and in modes that support differing resolution, the result indicates the syntax the
        /// usage would emit to JavaScript. Some examples:
        ///
        /// <code lang="ts">
        /// // tsc foo.mts --module nodenext
        /// import {} from "mod";
        /// // Result: ESNext - the import emits as ESM due to `impliedNodeFormat` set by .mts file extension
        ///
        /// // tsc foo.cts --module nodenext
        /// import {} from "mod";
        /// // Result: CommonJS - the import emits as CJS due to `impliedNodeFormat` set by .cts file extension
        ///
        /// // tsc foo.ts --module preserve --moduleResolution bundler
        /// import {} from "mod";
        /// // Result: ESNext - the import emits as ESM due to `--module preserve` and `--moduleResolution bundler`
        /// // supports conditional imports/exports
        ///
        /// // tsc foo.ts --module preserve --moduleResolution node10
        /// import {} from "mod";
        /// // Result: undefined - the import emits as ESM due to `--module preserve`, but `--moduleResolution node10`
        /// // does not support conditional imports/exports
        ///
        /// // tsc foo.ts --module commonjs --moduleResolution node10
        /// import type {} from "mod" with { "resolution-mode": "import" };
        /// // Result: ESNext - conditional imports/exports always supported with "resolution-mode" attribute
        /// </code>
        /// </summary>
        abstract getModeForResolutionAtIndex: file: SourceFile * index: float -> ResolutionMode
        abstract getDefaultResolutionModeForFile: sourceFile: SourceFile -> ResolutionMode
        abstract getImpliedNodeFormatForEmit: sourceFile: SourceFile -> ResolutionMode
        abstract getEmitModuleFormatOfFile: sourceFile: SourceFile -> ModuleKind
        abstract shouldTransformImportCall: sourceFile: SourceFile -> bool
        abstract structureIsReused: StructureIsReused
        abstract getSourceFileFromReference: referencingFile: SourceFile * ref: FileReference -> SourceFile option
        abstract getLibFileFromReference: ref: FileReference -> SourceFile option
        /// <summary>Given a source file, get the name of the package it was imported from.</summary>
        abstract sourceFileToPackageName: Map<Path, string> with get, set
        /// <summary>Set of all source files that some other source file redirects to.</summary>
        abstract redirectTargetsMap: MultiMap<Path, string> with get, set
        /// <summary>
        /// Whether any (non-external, non-declaration) source files use <c>node:</c>-prefixed module specifiers
        /// (except for those that are not available without the prefix).
        /// <c>false</c> indicates that an unprefixed builtin module was seen; <c>undefined</c> indicates that no
        /// builtin modules (or only modules exclusively available with the prefix) were seen.
        /// </summary>
        abstract usesUriStyleNodeCoreModules: bool option
        /// <summary>Map from libFileName to actual resolved location of the lib</summary>
        abstract resolvedLibReferences: Map<string, LibResolution> option with get, set
        abstract getProgramDiagnosticsContainer: (unit -> ProgramDiagnostics) with get, set
        abstract getCurrentPackagesMap: unit -> Map<string, bool> option
        /// <summary>Is the file emitted file</summary>
        abstract isEmittedFile: file: string -> bool
        abstract getFileIncludeReasons: unit -> MultiMap<Path, FileIncludeReason>
        abstract useCaseSensitiveFileNames: unit -> bool
        abstract getCanonicalFileName: GetCanonicalFileName with get, set
        abstract getProjectReferences: unit -> ResizeArray<ProjectReference> option
        abstract getResolvedProjectReferences: unit -> ResizeArray<ResolvedProjectReference option> option
        abstract getRedirectFromSourceFile: fileName: string -> ResolvedRefAndOutputDts option
        abstract forEachResolvedProjectReference: cb: (ResolvedProjectReference -> 'T option) -> 'T option
        abstract getResolvedProjectReferenceByPath: projectReferencePath: Path -> ResolvedProjectReference option
        abstract getRedirectFromOutput: filePath: Path -> ResolvedRefAndSource option
        abstract isSourceOfProjectReferenceRedirect: fileName: string -> bool
        abstract getCompilerOptionsForFile: file: SourceFile -> CompilerOptions
        abstract getBuildInfo: unit -> BuildInfo
        abstract emitBuildInfo: ?writeFile: WriteFileCallback * ?cancellationToken: CancellationToken -> EmitResult
        /// <summary>This implementation handles file exists to be true if file is source of project reference redirect when program is created using useSourceOfProjectReferenceRedirect</summary>
        abstract fileExists: fileName: string -> bool
        /// <summary>Call compilerHost.writeFile on host program was created with</summary>
        abstract writeFile: WriteFileCallback with get, set

    type RedirectTargetsMap =
        ReadonlyMap<Path, ResizeArray<string>>

    type [<AllowNullLiteral>] ResolvedProjectReference =
        abstract commandLine: ParsedCommandLine with get, set
        abstract sourceFile: SourceFile with get, set
        abstract references: ResizeArray<ResolvedProjectReference option> option with get, set

    type [<RequireQualifiedAccess>] StructureIsReused =
        | Not = 0
        | SafeModules = 1
        | Completely = 2

    type [<AllowNullLiteral>] CustomTransformerFactory =
        [<Emit("$0($1...)")>] abstract Invoke: context: TransformationContext -> CustomTransformer

    type [<AllowNullLiteral>] CustomTransformer =
        abstract transformSourceFile: node: SourceFile -> SourceFile
        abstract transformBundle: node: Bundle -> Bundle

    type [<AllowNullLiteral>] CustomTransformers =
        /// Custom transformers to evaluate before built-in .js transformations.
        abstract before: ResizeArray<U2<TransformerFactory<SourceFile>, CustomTransformerFactory>> option with get, set
        /// Custom transformers to evaluate after built-in .js transformations.
        abstract after: ResizeArray<U2<TransformerFactory<SourceFile>, CustomTransformerFactory>> option with get, set
        /// Custom transformers to evaluate after built-in .d.ts transformations.
        abstract afterDeclarations: ResizeArray<U2<TransformerFactory<U2<Bundle, SourceFile>>, CustomTransformerFactory>> option with get, set

    type [<AllowNullLiteral>] EmitTransformers =
        abstract scriptTransformers: ResizeArray<TransformerFactory<U2<SourceFile, Bundle>>> with get, set
        abstract declarationTransformers: ResizeArray<TransformerFactory<U2<SourceFile, Bundle>>> with get, set

    type [<AllowNullLiteral>] SourceMapSpan =
        /// Line number in the .js file.
        abstract emittedLine: float with get, set
        /// Column number in the .js file.
        abstract emittedColumn: float with get, set
        /// Line number in the .ts file.
        abstract sourceLine: float with get, set
        /// Column number in the .ts file.
        abstract sourceColumn: float with get, set
        /// Optional name (index into names array) associated with this span.
        abstract nameIndex: float option with get, set
        /// .ts file (index into sources array) associated with this span
        abstract sourceIndex: float with get, set

    type [<AllowNullLiteral>] SourceMapEmitResult =
        abstract inputSourceFileNames: ResizeArray<string> with get, set
        abstract sourceMap: RawSourceMap with get, set

    /// Return code used by getEmitOutput function to indicate status of the function
    type [<RequireQualifiedAccess>] ExitStatus =
        | Success = 0
        | DiagnosticsPresent_OutputsSkipped = 1
        | DiagnosticsPresent_OutputsGenerated = 2
        | InvalidProject_OutputsSkipped = 3
        | ProjectReferenceCycle_OutputsSkipped = 4

    type [<AllowNullLiteral>] EmitResult =
        abstract emitSkipped: bool with get, set
        /// Contains declaration emit diagnostics
        abstract diagnostics: ResizeArray<Diagnostic> with get, set
        abstract emittedFiles: ResizeArray<string> option with get, set
        abstract sourceMaps: ResizeArray<SourceMapEmitResult> option with get, set

    type [<AllowNullLiteral>] TypeCheckerHost =
        inherit ModuleSpecifierResolutionHost
        inherit SourceFileMayBeEmittedHost
        abstract getCompilerOptions: unit -> CompilerOptions
        abstract getSourceFiles: unit -> ResizeArray<SourceFile>
        abstract getSourceFile: fileName: string -> SourceFile option
        abstract getRedirectFromSourceFile: fileName: string -> ResolvedRefAndOutputDts option
        abstract isSourceOfProjectReferenceRedirect: fileName: string -> bool
        abstract getEmitSyntaxForUsageLocation: file: SourceFile * usage: StringLiteralLike -> ResolutionMode
        abstract getRedirectFromOutput: filePath: Path -> ResolvedRefAndSource option
        abstract getModeForUsageLocation: file: SourceFile * usage: StringLiteralLike -> ResolutionMode
        abstract getDefaultResolutionModeForFile: sourceFile: SourceFile -> ResolutionMode
        abstract getImpliedNodeFormatForEmit: sourceFile: SourceFile -> ResolutionMode
        abstract getEmitModuleFormatOfFile: sourceFile: SourceFile -> ModuleKind
        abstract getResolvedModule: f: SourceFile * moduleName: string * mode: ResolutionMode -> ResolvedModuleWithFailedLookupLocations option
        abstract redirectTargetsMap: RedirectTargetsMap
        abstract typesPackageExists: packageName: string -> bool
        abstract packageBundlesTypes: packageName: string -> bool
        abstract isSourceFileDefaultLibrary: file: SourceFile -> bool

    type [<AllowNullLiteral>] WriterContextOut =
        /// Whether increasing the expansion depth will cause us to expand more types.
        abstract canIncreaseExpansionDepth: bool with get, set
        abstract truncated: bool with get, set

    type [<AllowNullLiteral>] TypeChecker =
        abstract getTypeOfSymbolAtLocation: symbol: Symbol * node: Node -> Type
        abstract getTypeOfSymbol: symbol: Symbol -> Type
        abstract getDeclaredTypeOfSymbol: symbol: Symbol -> Type
        abstract getPropertiesOfType: ``type``: Type -> ResizeArray<Symbol>
        abstract getPropertyOfType: ``type``: Type * propertyName: string -> Symbol option
        abstract getPrivateIdentifierPropertyOfType: leftType: Type * name: string * location: Node -> Symbol option
        abstract getTypeOfPropertyOfType: ``type``: Type * propertyName: string -> Type option
        abstract getIndexInfoOfType: ``type``: Type * kind: IndexKind -> IndexInfo option
        abstract getIndexInfosOfType: ``type``: Type -> ResizeArray<IndexInfo>
        abstract getIndexInfosOfIndexSymbol: (Symbol -> (ResizeArray<Symbol>) option -> ResizeArray<IndexInfo>) with get, set
        abstract getSignaturesOfType: ``type``: Type * kind: SignatureKind -> ResizeArray<Signature>
        abstract getIndexTypeOfType: ``type``: Type * kind: IndexKind -> Type option
        abstract getIndexType: ``type``: Type -> Type
        abstract getBaseTypes: ``type``: InterfaceType -> ResizeArray<BaseType>
        abstract getBaseTypeOfLiteralType: ``type``: Type -> Type
        abstract getWidenedType: ``type``: Type -> Type
        abstract getWidenedLiteralType: ``type``: Type -> Type
        abstract getPromisedTypeOfPromise: promise: Type * ?errorNode: Node -> Type option
        /// <summary>
        /// Gets the "awaited type" of a type.
        ///
        /// If an expression has a Promise-like type, the "awaited type" of the expression is
        /// derived from the type of the first argument of the fulfillment callback for that
        /// Promise's <c>then</c> method. If the "awaited type" is itself a Promise-like, it is
        /// recursively unwrapped in the same manner until a non-promise type is found.
        ///
        /// If an expression does not have a Promise-like type, its "awaited type" is the type
        /// of the expression.
        ///
        /// If the resulting "awaited type" is a generic object type, then it is wrapped in
        /// an <c>Awaited&lt;T&gt;</c>.
        ///
        /// In the event the "awaited type" circularly references itself, or is a non-Promise
        /// object-type with a callable <c>then()</c> method, an "awaited type" cannot be determined
        /// and the value <c>undefined</c> will be returned.
        ///
        /// This is used to reflect the runtime behavior of the <c>await</c> keyword.
        /// </summary>
        abstract getAwaitedType: ``type``: Type -> Type option
        abstract isEmptyAnonymousObjectType: ``type``: Type -> bool
        abstract getReturnTypeOfSignature: signature: Signature -> Type
        /// <summary>
        /// Gets the type of a parameter at a given position in a signature.
        /// Returns <c>any</c> if the index is not valid.
        /// </summary>
        abstract getParameterType: signature: Signature * parameterIndex: float -> Type
        abstract getParameterIdentifierInfoAtPosition: signature: Signature * parameterIndex: float -> {| parameter: Identifier; parameterName: __String; isRestParameter: bool |} option
        abstract getNullableType: ``type``: Type * flags: TypeFlags -> Type
        abstract getNonNullableType: ``type``: Type -> Type
        abstract getNonOptionalType: ``type``: Type -> Type
        abstract isNullableType: ``type``: Type -> bool
        abstract getTypeArguments: ``type``: TypeReference -> ResizeArray<Type>
        /// Note that the resulting nodes cannot be checked.
        abstract typeToTypeNode: ``type``: Type * enclosingDeclaration: Node option * flags: NodeBuilderFlags option -> TypeNode option
        abstract typeToTypeNode: ``type``: Type * enclosingDeclaration: Node option * flags: NodeBuilderFlags option * ?internalFlags: InternalNodeBuilderFlags * ?tracker: SymbolTracker -> TypeNode option
        abstract typePredicateToTypePredicateNode: typePredicate: TypePredicate * enclosingDeclaration: Node option * flags: NodeBuilderFlags option * ?internalFlags: InternalNodeBuilderFlags * ?tracker: SymbolTracker -> TypePredicateNode option
        /// Note that the resulting nodes cannot be checked.
        abstract signatureToSignatureDeclaration: signature: Signature * kind: SyntaxKind * enclosingDeclaration: Node option * flags: NodeBuilderFlags option -> obj option
        abstract signatureToSignatureDeclaration: signature: Signature * kind: SyntaxKind * enclosingDeclaration: Node option * flags: NodeBuilderFlags option * ?internalFlags: InternalNodeBuilderFlags * ?tracker: SymbolTracker -> obj option
        /// Note that the resulting nodes cannot be checked.
        abstract indexInfoToIndexSignatureDeclaration: indexInfo: IndexInfo * enclosingDeclaration: Node option * flags: NodeBuilderFlags option -> IndexSignatureDeclaration option
        abstract indexInfoToIndexSignatureDeclaration: indexInfo: IndexInfo * enclosingDeclaration: Node option * flags: NodeBuilderFlags option * ?internalFlags: InternalNodeBuilderFlags * ?tracker: SymbolTracker -> IndexSignatureDeclaration option
        /// Note that the resulting nodes cannot be checked.
        abstract symbolToEntityName: symbol: Symbol * meaning: SymbolFlags * enclosingDeclaration: Node option * flags: NodeBuilderFlags option -> EntityName option
        /// Note that the resulting nodes cannot be checked.
        abstract symbolToExpression: symbol: Symbol * meaning: SymbolFlags * enclosingDeclaration: Node option * flags: NodeBuilderFlags option -> Expression option
        /// <summary>Note that the resulting nodes cannot be checked.</summary>
        abstract symbolToNode: symbol: Symbol * meaning: SymbolFlags * enclosingDeclaration: Node option * flags: NodeBuilderFlags option * internalFlags: InternalNodeBuilderFlags option -> Node option
        /// Note that the resulting nodes cannot be checked.
        abstract symbolToTypeParameterDeclarations: symbol: Symbol * enclosingDeclaration: Node option * flags: NodeBuilderFlags option -> ResizeArray<TypeParameterDeclaration> option
        /// Note that the resulting nodes cannot be checked.
        abstract symbolToParameterDeclaration: symbol: Symbol * enclosingDeclaration: Node option * flags: NodeBuilderFlags option -> ParameterDeclaration option
        /// Note that the resulting nodes cannot be checked.
        abstract typeParameterToDeclaration: parameter: TypeParameter * enclosingDeclaration: Node option * flags: NodeBuilderFlags option -> TypeParameterDeclaration option
        abstract typeParameterToDeclaration: parameter: TypeParameter * enclosingDeclaration: Node option * flags: NodeBuilderFlags option * ?internalFlags: InternalNodeBuilderFlags * ?tracker: SymbolTracker * ?maximumLength: float * ?verbosityLevel: float * ?out: WriterContextOut -> TypeParameterDeclaration option
        abstract getSymbolsInScope: location: Node * meaning: SymbolFlags -> ResizeArray<Symbol>
        abstract getSymbolAtLocation: node: Node -> Symbol option
        abstract getIndexInfosAtLocation: node: Node -> ResizeArray<IndexInfo> option
        abstract getSymbolsOfParameterPropertyDeclaration: parameter: ParameterDeclaration * parameterName: string -> ResizeArray<Symbol>
        /// The function returns the value (local variable) symbol of an identifier in the short-hand property assignment.
        /// This is necessary as an identifier in short-hand property assignment can contains two meaning: property name and property value.
        abstract getShorthandAssignmentValueSymbol: location: Node option -> Symbol option
        abstract getExportSpecifierLocalTargetSymbol: location: U2<ExportSpecifier, Identifier> -> Symbol option
        /// <summary>
        /// If a symbol is a local symbol with an associated exported symbol, returns the exported symbol.
        /// Otherwise returns its input.
        /// For example, at <c>export type T = number;</c>:
        ///     - <c>getSymbolAtLocation</c> at the location <c>T</c> will return the exported symbol for <c>T</c>.
        ///     - But the result of <c>getSymbolsInScope</c> will contain the *local* symbol for <c>T</c>, not the exported symbol.
        ///     - Calling <c>getExportSymbolOfSymbol</c> on that local symbol will return the exported symbol.
        /// </summary>
        abstract getExportSymbolOfSymbol: symbol: Symbol -> Symbol
        abstract getPropertySymbolOfDestructuringAssignment: location: Identifier -> Symbol option
        abstract getTypeOfAssignmentPattern: pattern: AssignmentPattern -> Type
        abstract getTypeAtLocation: node: Node -> Type
        abstract getTypeFromTypeNode: node: TypeNode -> Type
        abstract signatureToString: signature: Signature * ?enclosingDeclaration: Node * ?flags: TypeFormatFlags * ?kind: SignatureKind -> string
        abstract typeToString: ``type``: Type * ?enclosingDeclaration: Node * ?flags: TypeFormatFlags -> string
        abstract symbolToString: symbol: Symbol * ?enclosingDeclaration: Node * ?meaning: SymbolFlags * ?flags: SymbolFormatFlags -> string
        abstract typePredicateToString: predicate: TypePredicate * ?enclosingDeclaration: Node * ?flags: TypeFormatFlags -> string
        abstract writeSignature: signature: Signature * ?enclosingDeclaration: Node * ?flags: TypeFormatFlags * ?kind: SignatureKind * ?writer: EmitTextWriter * ?maximumLength: float * ?verbosityLevel: float * ?out: WriterContextOut -> string
        abstract writeType: ``type``: Type * ?enclosingDeclaration: Node * ?flags: TypeFormatFlags * ?writer: EmitTextWriter * ?maximumLength: float * ?verbosityLevel: float * ?out: WriterContextOut -> string
        abstract writeSymbol: symbol: Symbol * ?enclosingDeclaration: Node * ?meaning: SymbolFlags * ?flags: SymbolFormatFlags * ?writer: EmitTextWriter -> string
        abstract writeTypePredicate: predicate: TypePredicate * ?enclosingDeclaration: Node * ?flags: TypeFormatFlags * ?writer: EmitTextWriter -> string
        abstract getFullyQualifiedName: symbol: Symbol -> string
        abstract getAugmentedPropertiesOfType: ``type``: Type -> ResizeArray<Symbol>
        abstract getRootSymbols: symbol: Symbol -> ResizeArray<Symbol>
        abstract getSymbolOfExpando: node: Node * allowDeclaration: bool -> Symbol option
        abstract getContextualType: node: Expression -> Type option
        abstract getContextualType: node: Expression * ?contextFlags: ContextFlags -> Type option
        abstract getContextualTypeForObjectLiteralElement: element: ObjectLiteralElementLike -> Type option
        abstract getContextualTypeForArgumentAtIndex: call: CallLikeExpression * argIndex: float -> Type option
        abstract getContextualTypeForJsxAttribute: attribute: U2<JsxAttribute, JsxSpreadAttribute> -> Type option
        abstract isContextSensitive: node: U4<Expression, MethodDeclaration, ObjectLiteralElementLike, JsxAttributeLike> -> bool
        abstract getTypeOfPropertyOfContextualType: ``type``: Type * name: __String -> Type option
        /// <summary>
        /// returns unknownSignature in the case of an error.
        /// returns undefined if the node is not valid.
        /// </summary>
        /// <param name="argumentCount">Apparent number of arguments, passed in case of a possibly incomplete call. This should come from an ArgumentListInfo. See <c>signatureHelp.ts</c>.</param>
        abstract getResolvedSignature: node: CallLikeExpression * ?candidatesOutArray: ResizeArray<Signature> * ?argumentCount: float -> Signature option
        abstract getResolvedSignatureForSignatureHelp: node: CallLikeExpression * ?candidatesOutArray: ResizeArray<Signature> * ?argumentCount: float -> Signature option
        abstract getCandidateSignaturesForStringLiteralCompletions: call: CallLikeExpression * editingArgument: Node -> ResizeArray<Signature>
        abstract getExpandedParameters: ``sig``: Signature -> ResizeArray<ResizeArray<Symbol>>
        abstract hasEffectiveRestParameter: ``sig``: Signature -> bool
        abstract containsArgumentsReference: declaration: SignatureDeclaration -> bool
        abstract getSignatureFromDeclaration: declaration: SignatureDeclaration -> Signature option
        abstract isImplementationOfOverload: node: SignatureDeclaration -> bool option
        abstract isUndefinedSymbol: symbol: Symbol -> bool
        abstract isArgumentsSymbol: symbol: Symbol -> bool
        abstract isUnknownSymbol: symbol: Symbol -> bool
        abstract getMergedSymbol: symbol: Symbol -> Symbol
        abstract symbolIsValue: symbol: Symbol * ?includeTypeOnlyMembers: bool -> bool
        abstract getConstantValue: node: U3<EnumMember, PropertyAccessExpression, ElementAccessExpression> -> U2<string, float> option
        abstract isValidPropertyAccess: node: U3<PropertyAccessExpression, QualifiedName, ImportTypeNode> * propertyName: string -> bool
        /// <summary>Exclude accesses to private properties.</summary>
        abstract isValidPropertyAccessForCompletions: node: U3<PropertyAccessExpression, ImportTypeNode, QualifiedName> * ``type``: Type * property: Symbol -> bool
        /// Follow all aliases to get the original symbol.
        abstract getAliasedSymbol: symbol: Symbol -> Symbol
        /// Follow a *single* alias to get the immediately aliased symbol.
        abstract getImmediateAliasedSymbol: symbol: Symbol -> Symbol option
        abstract getExportsOfModule: moduleSymbol: Symbol -> ResizeArray<Symbol>
        /// <summary>Unlike <c>getExportsOfModule</c>, this includes properties of an <c>export =</c> value.</summary>
        abstract getExportsAndPropertiesOfModule: moduleSymbol: Symbol -> ResizeArray<Symbol>
        abstract forEachExportAndPropertyOfModule: moduleSymbol: Symbol * cb: (Symbol -> __String -> unit) -> unit
        abstract getJsxIntrinsicTagNamesAt: location: Node -> ResizeArray<Symbol>
        abstract isOptionalParameter: node: ParameterDeclaration -> bool
        abstract getAmbientModules: unit -> ResizeArray<Symbol>
        abstract tryGetMemberInModuleExports: memberName: string * moduleSymbol: Symbol -> Symbol option
        /// <summary>
        /// Unlike <c>tryGetMemberInModuleExports</c>, this includes properties of an <c>export =</c> value.
        /// Does *not* return properties of primitive types.
        /// </summary>
        abstract tryGetMemberInModuleExportsAndProperties: memberName: string * moduleSymbol: Symbol -> Symbol option
        abstract getApparentType: ``type``: Type -> Type
        abstract getSuggestedSymbolForNonexistentProperty: name: U2<MemberName, string> * containingType: Type -> Symbol option
        abstract getSuggestedSymbolForNonexistentJSXAttribute: name: U2<Identifier, string> * containingType: Type -> Symbol option
        abstract getSuggestedSymbolForNonexistentSymbol: location: Node * name: string * meaning: SymbolFlags -> Symbol option
        abstract getSuggestedSymbolForNonexistentModule: node: Identifier * target: Symbol -> Symbol option
        abstract getSuggestedSymbolForNonexistentClassMember: name: string * baseType: Type -> Symbol option
        abstract getBaseConstraintOfType: ``type``: Type -> Type option
        abstract getDefaultFromTypeParameter: ``type``: Type -> Type option
        /// <summary>
        /// Gets the intrinsic <c>any</c> type. There are multiple types that act as <c>any</c> used internally in the compiler,
        /// so the type returned by this function should not be used in equality checks to determine if another type
        /// is <c>any</c>. Instead, use <c>type.flags &amp; TypeFlags.Any</c>.
        /// </summary>
        abstract getAnyType: unit -> Type
        abstract getStringType: unit -> Type
        abstract getStringLiteralType: value: string -> StringLiteralType
        abstract getNumberType: unit -> Type
        abstract getNumberLiteralType: value: float -> NumberLiteralType
        abstract getBigIntType: unit -> Type
        abstract getBigIntLiteralType: value: PseudoBigInt -> BigIntLiteralType
        abstract getBooleanType: unit -> Type
        abstract getUnknownType: unit -> Type
        abstract getFalseType: ?fresh: bool -> Type
        abstract getFalseType: unit -> Type
        abstract getTrueType: ?fresh: bool -> Type
        abstract getTrueType: unit -> Type
        abstract getVoidType: unit -> Type
        /// <summary>
        /// Gets the intrinsic <c>undefined</c> type. There are multiple types that act as <c>undefined</c> used internally in the compiler
        /// depending on compiler options, so the type returned by this function should not be used in equality checks to determine
        /// if another type is <c>undefined</c>. Instead, use <c>type.flags &amp; TypeFlags.Undefined</c>.
        /// </summary>
        abstract getUndefinedType: unit -> Type
        /// <summary>
        /// Gets the intrinsic <c>null</c> type. There are multiple types that act as <c>null</c> used internally in the compiler,
        /// so the type returned by this function should not be used in equality checks to determine if another type
        /// is <c>null</c>. Instead, use <c>type.flags &amp; TypeFlags.Null</c>.
        /// </summary>
        abstract getNullType: unit -> Type
        abstract getESSymbolType: unit -> Type
        /// <summary>
        /// Gets the intrinsic <c>never</c> type. There are multiple types that act as <c>never</c> used internally in the compiler,
        /// so the type returned by this function should not be used in equality checks to determine if another type
        /// is <c>never</c>. Instead, use <c>type.flags &amp; TypeFlags.Never</c>.
        /// </summary>
        abstract getNeverType: unit -> Type
        /// <summary>Gets the intrinsic <c>object</c> type.</summary>
        abstract getNonPrimitiveType: unit -> Type
        abstract getOptionalType: unit -> Type
        abstract getUnionType: types: ResizeArray<Type> * ?subtypeReduction: UnionReduction -> Type
        abstract createArrayType: elementType: Type -> Type
        abstract getElementTypeOfArrayType: arrayType: Type -> Type option
        abstract createPromiseType: ``type``: Type -> Type
        abstract getPromiseType: unit -> Type
        abstract getPromiseLikeType: unit -> Type
        abstract getAnyAsyncIterableType: unit -> Type option
        /// Returns true if the "source" type is assignable to the "target" type.
        ///
        /// <code lang="ts">
        /// declare const abcLiteral: ts.Type; // Type of "abc"
        /// declare const stringType: ts.Type; // Type of string
        ///
        /// isTypeAssignableTo(abcLiteral, abcLiteral); // true; "abc" is assignable to "abc"
        /// isTypeAssignableTo(abcLiteral, stringType); // true; "abc" is assignable to string
        /// isTypeAssignableTo(stringType, abcLiteral); // false; string is not assignable to "abc"
        /// isTypeAssignableTo(stringType, stringType); // true; string is assignable to string
        /// </code>
        abstract isTypeAssignableTo: source: Type * target: Type -> bool
        abstract createAnonymousType: symbol: Symbol option * members: SymbolTable * callSignatures: ResizeArray<Signature> * constructSignatures: ResizeArray<Signature> * indexInfos: ResizeArray<IndexInfo> -> Type
        abstract createSignature: declaration: SignatureDeclaration option * typeParameters: ResizeArray<TypeParameter> option * thisParameter: Symbol option * parameters: ResizeArray<Symbol> * resolvedReturnType: Type * typePredicate: TypePredicate option * minArgumentCount: float * flags: SignatureFlags -> Signature
        abstract createSymbol: flags: SymbolFlags * name: __String -> TransientSymbol
        abstract createIndexInfo: keyType: Type * ``type``: Type * isReadonly: bool * ?declaration: SignatureDeclaration -> IndexInfo
        abstract isSymbolAccessible: symbol: Symbol * enclosingDeclaration: Node option * meaning: SymbolFlags * shouldComputeAliasToMarkVisible: bool -> SymbolAccessibilityResult
        abstract tryFindAmbientModule: moduleName: string -> Symbol option
        abstract getSymbolWalker: ?accept: (Symbol -> bool) -> SymbolWalker
        abstract getDiagnostics: ?sourceFile: SourceFile * ?cancellationToken: CancellationToken * ?nodesToCheck: ResizeArray<Node> -> ResizeArray<Diagnostic>
        abstract getGlobalDiagnostics: unit -> ResizeArray<Diagnostic>
        abstract getEmitResolver: ?sourceFile: SourceFile * ?cancellationToken: CancellationToken * ?forceDts: bool -> EmitResolver
        abstract requiresAddingImplicitUndefined: parameter: U2<ParameterDeclaration, JSDocParameterTag> * enclosingDeclaration: Node option -> bool
        abstract getNodeCount: unit -> float
        abstract getIdentifierCount: unit -> float
        abstract getSymbolCount: unit -> float
        abstract getTypeCount: unit -> float
        abstract getInstantiationCount: unit -> float
        abstract getRelationCacheSizes: unit -> {| assignable: float; identity: float; subtype: float; strictSubtype: float |}
        abstract getRecursionIdentity: ``type``: Type -> obj option
        abstract getUnmatchedProperties: source: Type * target: Type * requireOptionalProperties: bool * matchDiscriminantProperties: bool -> IterableIterator<Symbol>
        /// <summary>
        /// True if this type is the <c>Array</c> or <c>ReadonlyArray</c> type from lib.d.ts.
        /// This function will _not_ return true if passed a type which
        /// extends <c>Array</c> (for example, the TypeScript AST's <c>NodeArray</c> type).
        /// </summary>
        abstract isArrayType: ``type``: Type -> bool
        /// True if this type is a tuple type. This function will _not_ return true if
        /// passed a type which extends from a tuple.
        abstract isTupleType: ``type``: Type -> bool
        /// <summary>True if this type is assignable to <c>ReadonlyArray&lt;any&gt;</c>.</summary>
        abstract isArrayLikeType: ``type``: Type -> bool
        /// <summary>
        /// True if <c>contextualType</c> should not be considered for completions because
        /// e.g. it specifies <c>kind: "a"</c> and obj has <c>kind: "b"</c>.
        /// </summary>
        abstract isTypeInvalidDueToUnionDiscriminant: contextualType: Type * obj: U2<ObjectLiteralExpression, JsxAttributes> -> bool
        abstract getExactOptionalProperties: ``type``: Type -> ResizeArray<Symbol>
        /// <summary>
        /// For a union, will include a property if it's defined in *any* of the member types.
        /// So for <c>{ a } | { b }</c>, this will include both <c>a</c> and <c>b</c>.
        /// Does not include properties of primitive types.
        /// </summary>
        abstract getAllPossiblePropertiesOfTypes: ``type``: ResizeArray<Type> -> ResizeArray<Symbol>
        abstract resolveName: name: string * location: Node option * meaning: SymbolFlags * excludeGlobals: bool -> Symbol option
        abstract getJsxNamespace: ?location: Node -> string
        abstract getJsxFragmentFactory: location: Node -> string option
        /// <summary>
        /// Note that this will return undefined in the following case:
        ///     // a.ts
        ///     export namespace N { export class C { } }
        ///     // b.ts
        ///     &lt;&lt;enclosingDeclaration&gt;&gt;
        /// Where <c>C</c> is the symbol we're looking for.
        /// This should be called in a loop climbing parents of the symbol, so we'll get <c>N</c>.
        /// </summary>
        abstract getAccessibleSymbolChain: symbol: Symbol * enclosingDeclaration: Node option * meaning: SymbolFlags * useOnlyExternalAliasing: bool -> ResizeArray<Symbol> option
        abstract getTypePredicateOfSignature: signature: Signature -> TypePredicate option
        abstract resolveExternalModuleName: moduleSpecifier: Expression -> Symbol option
        /// <summary>
        /// An external module with an 'export =' declaration resolves to the target of the 'export =' declaration,
        /// and an external module with no 'export =' declaration resolves to the module itself.
        /// </summary>
        abstract resolveExternalModuleSymbol: symbol: Symbol -> Symbol
        /// <param name="node">A location where we might consider accessing <c>this</c>. Not necessarily a ThisExpression.</param>
        abstract tryGetThisTypeAt: node: Node * ?includeGlobalThis: bool * ?container: ThisContainer -> Type option
        abstract getTypeArgumentConstraint: node: TypeNode -> Type option
        /// <summary>
        /// Does *not* get *all* suggestion diagnostics, just the ones that were convenient to report in the checker.
        /// Others are added in computeSuggestionDiagnostics.
        /// </summary>
        abstract getSuggestionDiagnostics: file: SourceFile * ?cancellationToken: CancellationToken -> ResizeArray<DiagnosticWithLocation>
        /// Depending on the operation performed, it may be appropriate to throw away the checker
        /// if the cancellation token is triggered. Typically, if it is used for error checking
        /// and the operation is cancelled, then it should be discarded, otherwise it is safe to keep.
        abstract runWithCancellationToken: token: CancellationToken * cb: (TypeChecker -> 'T) -> 'T
        abstract runWithCancellationToken: token: CancellationToken option * cb: (TypeChecker -> 'T) -> 'T
        abstract getLocalTypeParametersOfClassOrInterfaceOrTypeAlias: symbol: Symbol -> ResizeArray<TypeParameter> option
        abstract isDeclarationVisible: node: U2<Declaration, AnyImportSyntax> -> bool
        abstract isPropertyAccessible: node: Node * isSuper: bool * isWrite: bool * containingType: Type * property: Symbol -> bool
        abstract getTypeOnlyAliasDeclaration: symbol: Symbol -> TypeOnlyAliasDeclaration option
        abstract getMemberOverrideModifierStatus: node: ClassLikeDeclaration * ``member``: ClassElement * memberSymbol: Symbol -> MemberOverrideStatus
        abstract isTypeParameterPossiblyReferenced: tp: TypeParameter * node: Node -> bool
        abstract typeHasCallOrConstructSignatures: ``type``: Type -> bool
        abstract getSymbolFlags: symbol: Symbol -> SymbolFlags
        abstract fillMissingTypeArguments: typeArguments: ResizeArray<Type> * typeParameters: ResizeArray<TypeParameter> option * minTypeArgumentCount: float * isJavaScriptImplicitAny: bool -> ResizeArray<Type>
        abstract getTypeArgumentsForResolvedSignature: signature: Signature -> ResizeArray<Type> option
        abstract isLibType: ``type``: Type -> bool

    type [<RequireQualifiedAccess>] MemberOverrideStatus =
        | Ok = 0
        | NeedsOverride = 1
        | HasInvalidOverride = 2

    type [<RequireQualifiedAccess>] UnionReduction =
        | None = 0
        | Literal = 1
        | Subtype = 2

    type [<RequireQualifiedAccess>] IntersectionFlags =
        | None = 0
        | NoSupertypeReduction = 1
        | NoConstraintReduction = 2

    type [<RequireQualifiedAccess>] ContextFlags =
        | None = 0
        | Signature = 1
        | NoConstraints = 2
        | Completions = 4
        | SkipBindingPatterns = 8

    type [<RequireQualifiedAccess>] NodeBuilderFlags =
        | None = 0
        | NoTruncation = 1
        | WriteArrayAsGenericType = 2
        | GenerateNamesForShadowedTypeParams = 4
        | UseStructuralFallback = 8
        | ForbidIndexedAccessSymbolReferences = 16
        | WriteTypeArgumentsOfSignature = 32
        | UseFullyQualifiedType = 64
        | UseOnlyExternalAliasing = 128
        | SuppressAnyReturnType = 256
        | WriteTypeParametersInQualifiedName = 512
        | MultilineObjectLiterals = 1024
        | WriteClassExpressionAsTypeLiteral = 2048
        | UseTypeOfFunction = 4096
        | OmitParameterModifiers = 8192
        | UseAliasDefinedOutsideCurrentScope = 16384
        | UseSingleQuotesForStringLiteralType = 268435456
        | NoTypeReduction = 536870912
        | OmitThisParameter = 33554432
        | AllowThisInObjectLiteral = 32768
        | AllowQualifiedNameInPlaceOfIdentifier = 65536
        | AllowAnonymousIdentifier = 131072
        | AllowEmptyUnionOrIntersection = 262144
        | AllowEmptyTuple = 524288
        | AllowUniqueESSymbolType = 1048576
        | AllowEmptyIndexInfoType = 2097152
        | AllowNodeModulesRelativePaths = 67108864
        | IgnoreErrors = 70221824
        | InObjectTypeLiteral = 4194304
        | InTypeAlias = 8388608
        | InInitialEntityName = 16777216

    type [<RequireQualifiedAccess>] InternalNodeBuilderFlags =
        | None = 0
        | WriteComputedProps = 1
        | NoSyntacticPrinter = 2
        | DoNotIncludeSymbolChain = 4
        | AllowUnresolvedNames = 8

    type [<RequireQualifiedAccess>] TypeFormatFlags =
        | None = 0
        | NoTruncation = 1
        | WriteArrayAsGenericType = 2
        | GenerateNamesForShadowedTypeParams = 4
        | UseStructuralFallback = 8
        | WriteTypeArgumentsOfSignature = 32
        | UseFullyQualifiedType = 64
        | SuppressAnyReturnType = 256
        | MultilineObjectLiterals = 1024
        | WriteClassExpressionAsTypeLiteral = 2048
        | UseTypeOfFunction = 4096
        | OmitParameterModifiers = 8192
        | UseAliasDefinedOutsideCurrentScope = 16384
        | UseSingleQuotesForStringLiteralType = 268435456
        | NoTypeReduction = 536870912
        | OmitThisParameter = 33554432
        | AllowUniqueESSymbolType = 1048576
        | AddUndefined = 131072
        | WriteArrowStyleSignature = 262144
        | InArrayType = 524288
        | InElementType = 2097152
        | InFirstTypeArgument = 4194304
        | InTypeAlias = 8388608
        | NodeBuilderFlagsMask = 848330095

    type [<RequireQualifiedAccess>] SymbolFormatFlags =
        | None = 0
        | WriteTypeParametersOrArguments = 1
        | UseOnlyExternalAliasing = 2
        | AllowAnyNodeKind = 4
        | UseAliasDefinedOutsideCurrentScope = 8
        | WriteComputedProps = 16
        | DoNotIncludeSymbolChain = 32

    type [<AllowNullLiteral>] SymbolWalker =
        /// Note: Return values are not ordered.
        abstract walkType: root: Type -> {| visitedTypes: ResizeArray<Type>; visitedSymbols: ResizeArray<Symbol> |}
        /// Note: Return values are not ordered.
        abstract walkSymbol: root: Symbol -> {| visitedTypes: ResizeArray<Type>; visitedSymbols: ResizeArray<Symbol> |}

    type [<AllowNullLiteral>] SymbolWriter =
        abstract writeKeyword: text: string -> unit
        abstract writeOperator: text: string -> unit
        abstract writePunctuation: text: string -> unit
        abstract writeSpace: text: string -> unit
        abstract writeStringLiteral: text: string -> unit
        abstract writeParameter: text: string -> unit
        abstract writeProperty: text: string -> unit
        abstract writeSymbol: text: string * symbol: Symbol -> unit
        abstract writeLine: ?force: bool -> unit
        abstract increaseIndent: unit -> unit
        abstract decreaseIndent: unit -> unit
        abstract clear: unit -> unit

    type [<RequireQualifiedAccess>] SymbolAccessibility =
        | Accessible = 0
        | NotAccessible = 1
        | CannotBeNamed = 2
        | NotResolved = 3

    type [<RequireQualifiedAccess>] TypePredicateKind =
        | This = 0
        | Identifier = 1
        | AssertsThis = 2
        | AssertsIdentifier = 3

    type [<AllowNullLiteral>] TypePredicateBase =
        abstract kind: TypePredicateKind with get, set
        abstract ``type``: Type option with get, set

    type [<AllowNullLiteral>] ThisTypePredicate =
        inherit TypePredicateBase
        abstract kind: TypePredicateKind with get, set
        abstract parameterName: obj with get, set
        abstract parameterIndex: obj with get, set
        abstract ``type``: Type with get, set

    type [<AllowNullLiteral>] IdentifierTypePredicate =
        inherit TypePredicateBase
        abstract kind: TypePredicateKind with get, set
        abstract parameterName: string with get, set
        abstract parameterIndex: float with get, set
        abstract ``type``: Type with get, set

    type [<AllowNullLiteral>] AssertsThisTypePredicate =
        inherit TypePredicateBase
        abstract kind: TypePredicateKind with get, set
        abstract parameterName: obj with get, set
        abstract parameterIndex: obj with get, set
        abstract ``type``: Type option with get, set

    type [<AllowNullLiteral>] AssertsIdentifierTypePredicate =
        inherit TypePredicateBase
        abstract kind: TypePredicateKind with get, set
        abstract parameterName: string with get, set
        abstract parameterIndex: float with get, set
        abstract ``type``: Type option with get, set

    type TypePredicate =
        U4<ThisTypePredicate, IdentifierTypePredicate, AssertsThisTypePredicate, AssertsIdentifierTypePredicate>

    type AnyImportSyntax =
        U2<ImportDeclaration, ImportEqualsDeclaration>

    type AnyImportOrJsDocImport =
        U2<AnyImportSyntax, JSDocImportTag>

    type AnyImportOrRequire =
        U2<AnyImportOrJsDocImport, VariableDeclarationInitializedTo<RequireOrImportCall>>

    type AnyImportOrBareOrAccessedRequire =
        U2<AnyImportSyntax, VariableDeclarationInitializedTo<U2<RequireOrImportCall, AccessExpression>>>

    type AliasDeclarationNode =
        U8<ImportEqualsDeclaration, VariableDeclarationInitializedTo<U2<RequireOrImportCall, AccessExpression>>, ImportClause, NamespaceImport, ImportSpecifier, ExportSpecifier, NamespaceExport, BindingElementOfBareOrAccessedRequire>

    type [<AllowNullLiteral>] BindingElementOfBareOrAccessedRequire =
        interface end

    type AnyImportOrRequireStatement =
        U2<AnyImportSyntax, RequireVariableStatement>

    type AnyImportOrReExport =
        U2<AnyImportSyntax, ExportDeclaration>

    type [<AllowNullLiteral>] ValidImportTypeNode =
        inherit ImportTypeNode
        abstract argument: obj with get, set

    type AnyValidImportOrReExport =
        U3<obj, RequireOrImportCall, ValidImportTypeNode>

    type [<AllowNullLiteral>] RequireOrImportCall =
        interface end

    type [<AllowNullLiteral>] VariableDeclarationInitializedTo<'T when 'T :> Expression> =
        inherit VariableDeclaration
        abstract initializer: 'T

    type [<AllowNullLiteral>] RequireVariableStatement =
        inherit VariableStatement
        abstract declarationList: RequireVariableDeclarationList

    type [<AllowNullLiteral>] RequireVariableDeclarationList =
        inherit VariableDeclarationList
        abstract declarations: ResizeArray<VariableDeclarationInitializedTo<RequireOrImportCall>>

    type CanHaveModuleSpecifier =
        U4<AnyImportOrBareOrAccessedRequire, AliasDeclarationNode, ExportDeclaration, ImportTypeNode>

    type LateVisibilityPaintedStatement =
        U8<AnyImportOrJsDocImport, VariableStatement, ClassDeclaration, FunctionDeclaration, ModuleDeclaration, TypeAliasDeclaration, InterfaceDeclaration, EnumDeclaration>

    type [<AllowNullLiteral>] SymbolVisibilityResult =
        abstract accessibility: SymbolAccessibility with get, set
        abstract aliasesToMakeVisible: ResizeArray<LateVisibilityPaintedStatement> option with get, set
        abstract errorSymbolName: string option with get, set
        abstract errorNode: Node option with get, set

    type [<AllowNullLiteral>] SymbolAccessibilityResult =
        inherit SymbolVisibilityResult
        abstract errorModuleName: string option with get, set

    type [<AllowNullLiteral>] AllAccessorDeclarations =
        abstract firstAccessor: AccessorDeclaration with get, set
        abstract secondAccessor: AccessorDeclaration option with get, set
        abstract getAccessor: GetAccessorDeclaration option with get, set
        abstract setAccessor: SetAccessorDeclaration option with get, set

    type [<AllowNullLiteral>] AllDecorators =
        abstract decorators: ResizeArray<Decorator> option with get, set
        abstract parameters: ResizeArray<ResizeArray<Decorator> option> option with get, set
        abstract getDecorators: ResizeArray<Decorator> option with get, set
        abstract setDecorators: ResizeArray<Decorator> option with get, set

    /// <summary>Indicates how to serialize the name for a TypeReferenceNode when emitting decorator metadata</summary>
    type [<RequireQualifiedAccess>] TypeReferenceSerializationKind =
        | Unknown = 0
        | TypeWithConstructSignatureAndValue = 1
        | VoidNullableOrNeverType = 2
        | NumberLikeType = 3
        | BigIntLikeType = 4
        | StringLikeType = 5
        | BooleanType = 6
        | ArrayLikeType = 7
        | ESSymbolType = 8
        | Promise = 9
        | TypeWithCallSignature = 10
        | ObjectType = 11

    /// <remarks>
    /// Original in TypeScript:
    /// <code lang="typescript">
    /// | NodeCheckFlags.SuperInstance
    ///     | NodeCheckFlags.SuperStatic
    ///     | NodeCheckFlags.MethodWithSuperPropertyAccessInAsync
    ///     | NodeCheckFlags.MethodWithSuperPropertyAssignmentInAsync
    ///     | NodeCheckFlags.ContainsSuperPropertyInStaticInitializer
    ///     | NodeCheckFlags.CaptureArguments
    ///     | NodeCheckFlags.ContainsCapturedBlockScopeBinding
    ///     | NodeCheckFlags.NeedsLoopOutParameter
    ///     | NodeCheckFlags.ContainsConstructorReference
    ///     | NodeCheckFlags.ConstructorReference
    ///     | NodeCheckFlags.CapturedBlockScopedBinding
    ///     | NodeCheckFlags.BlockScopedBindingInLoop
    ///     | NodeCheckFlags.LoopWithCapturedBlockScopedBinding
    /// </code>
    /// </remarks>
    type LazyNodeCheckFlags =
        NodeCheckFlags

    type [<AllowNullLiteral>] EmitResolver =
        abstract hasGlobalName: name: string -> bool
        abstract getReferencedExportContainer: node: Identifier * ?prefixLocals: bool -> U3<SourceFile, ModuleDeclaration, EnumDeclaration> option
        abstract getReferencedImportDeclaration: node: Identifier -> Declaration option
        abstract getReferencedDeclarationWithCollidingName: node: Identifier -> Declaration option
        abstract isDeclarationWithCollidingName: node: Declaration -> bool
        abstract isValueAliasDeclaration: node: Node -> bool
        abstract isReferencedAliasDeclaration: node: Node * ?checkChildren: bool -> bool
        abstract isTopLevelValueImportEqualsWithEntityName: node: ImportEqualsDeclaration -> bool
        abstract hasNodeCheckFlag: node: Node * flags: LazyNodeCheckFlags -> bool
        abstract isDeclarationVisible: node: U2<Declaration, AnyImportSyntax> -> bool
        abstract isLateBound: node: Declaration -> bool
        abstract collectLinkedAliases: node: ModuleExportName * ?setVisibility: bool -> ResizeArray<Node> option
        abstract markLinkedReferences: node: Node -> unit
        abstract isImplementationOfOverload: node: SignatureDeclaration -> bool option
        abstract requiresAddingImplicitUndefined: node: ParameterDeclaration * enclosingDeclaration: Node option -> bool
        abstract isExpandoFunctionDeclaration: node: U2<FunctionDeclaration, VariableDeclaration> -> bool
        abstract getPropertiesOfContainerFunction: node: Declaration -> ResizeArray<Symbol>
        abstract createTypeOfDeclaration: declaration: HasInferredType * enclosingDeclaration: Node * flags: NodeBuilderFlags * internalFlags: InternalNodeBuilderFlags * tracker: SymbolTracker -> TypeNode option
        abstract createReturnTypeOfSignatureDeclaration: signatureDeclaration: SignatureDeclaration * enclosingDeclaration: Node * flags: NodeBuilderFlags * internalFlags: InternalNodeBuilderFlags * tracker: SymbolTracker -> TypeNode option
        abstract createTypeOfExpression: expr: Expression * enclosingDeclaration: Node * flags: NodeBuilderFlags * internalFlags: InternalNodeBuilderFlags * tracker: SymbolTracker -> TypeNode option
        abstract createLiteralConstValue: node: U4<VariableDeclaration, PropertyDeclaration, PropertySignature, ParameterDeclaration> * tracker: SymbolTracker -> Expression
        abstract isSymbolAccessible: symbol: Symbol * enclosingDeclaration: Node option * meaning: SymbolFlags option * shouldComputeAliasToMarkVisible: bool -> SymbolAccessibilityResult
        abstract isEntityNameVisible: entityName: EntityNameOrEntityNameExpression * enclosingDeclaration: Node -> SymbolVisibilityResult
        abstract getConstantValue: node: U3<EnumMember, PropertyAccessExpression, ElementAccessExpression> -> U2<string, float> option
        abstract getEnumMemberValue: node: EnumMember -> EvaluatorResult option
        abstract getReferencedValueDeclaration: reference: Identifier -> Declaration option
        abstract getReferencedValueDeclarations: reference: Identifier -> ResizeArray<Declaration> option
        abstract getTypeReferenceSerializationKind: typeName: EntityName * ?location: Node -> TypeReferenceSerializationKind
        abstract isOptionalParameter: node: ParameterDeclaration -> bool
        abstract isArgumentsLocalBinding: node: Identifier -> bool
        abstract getExternalModuleFileFromDeclaration: declaration: U6<ImportEqualsDeclaration, ImportDeclaration, ExportDeclaration, ModuleDeclaration, ImportTypeNode, ImportCall> -> SourceFile option
        abstract isLiteralConstDeclaration: node: U4<VariableDeclaration, PropertyDeclaration, PropertySignature, ParameterDeclaration> -> bool
        abstract getJsxFactoryEntity: ?location: Node -> EntityName option
        abstract getJsxFragmentFactoryEntity: ?location: Node -> EntityName option
        abstract isBindingCapturedByNode: node: Node * decl: U2<VariableDeclaration, BindingElement> -> bool
        abstract getDeclarationStatementsForSourceFile: node: SourceFile * flags: NodeBuilderFlags * internalFlags: InternalNodeBuilderFlags * tracker: SymbolTracker -> ResizeArray<Statement> option
        abstract isImportRequiredByAugmentation: decl: ImportDeclaration -> bool
        abstract isDefinitelyReferenceToGlobalSymbolObject: node: Node -> bool
        abstract createLateBoundIndexSignatures: cls: ClassLikeDeclaration * enclosingDeclaration: Node * flags: NodeBuilderFlags * internalFlags: InternalNodeBuilderFlags * tracker: SymbolTracker -> ResizeArray<U2<IndexSignatureDeclaration, PropertyDeclaration>> option
        abstract symbolToDeclarations: symbol: Symbol * meaning: SymbolFlags * flags: NodeBuilderFlags * ?maximumLength: float * ?verbosityLevel: float * ?out: WriterContextOut -> ResizeArray<Declaration>

    type [<RequireQualifiedAccess>] SymbolFlags =
        | None = 0
        | FunctionScopedVariable = 1
        | BlockScopedVariable = 2
        | Property = 4
        | EnumMember = 8
        | Function = 16
        | Class = 32
        | Interface = 64
        | ConstEnum = 128
        | RegularEnum = 256
        | ValueModule = 512
        | NamespaceModule = 1024
        | TypeLiteral = 2048
        | ObjectLiteral = 4096
        | Method = 8192
        | Constructor = 16384
        | GetAccessor = 32768
        | SetAccessor = 65536
        | Signature = 131072
        | TypeParameter = 262144
        | TypeAlias = 524288
        | ExportValue = 1048576
        | Alias = 2097152
        | Prototype = 4194304
        | ExportStar = 8388608
        | Optional = 16777216
        | Transient = 33554432
        | Assignment = 67108864
        | ModuleExports = 134217728
        | All = -1
        | Enum = 384
        | Variable = 3
        | Value = 111551
        | Type = 788968
        | Namespace = 1920
        | Module = 1536
        | Accessor = 98304
        | FunctionScopedVariableExcludes = 111550
        | BlockScopedVariableExcludes = 111551
        | ParameterExcludes = 111551
        | PropertyExcludes = 0
        | EnumMemberExcludes = 900095
        | FunctionExcludes = 110991
        | ClassExcludes = 899503
        | InterfaceExcludes = 788872
        | RegularEnumExcludes = 899327
        | ConstEnumExcludes = 899967
        | ValueModuleExcludes = 110735
        | NamespaceModuleExcludes = 0
        | MethodExcludes = 103359
        | GetAccessorExcludes = 46015
        | SetAccessorExcludes = 78783
        | AccessorExcludes = 13247
        | TypeParameterExcludes = 526824
        | TypeAliasExcludes = 788968
        | AliasExcludes = 2097152
        | ModuleMember = 2623475
        | ExportHasLocal = 944
        | BlockScoped = 418
        | PropertyOrAccessor = 98308
        | ClassMember = 106500
        | ExportSupportsDefaultModifier = 112
        | ExportDoesNotSupportDefaultModifier = -113
        | Classifiable = 2885600
        | LateBindingContainer = 6256

    type SymbolId =
        float

    type [<AllowNullLiteral>] Symbol =
        abstract flags: SymbolFlags with get, set
        abstract escapedName: __String with get, set
        abstract declarations: ResizeArray<Declaration> option with get, set
        abstract valueDeclaration: Declaration option with get, set
        abstract members: SymbolTable option with get, set
        abstract exports: SymbolTable option with get, set
        abstract globalExports: SymbolTable option with get, set
        abstract id: SymbolId with get, set
        abstract mergeId: float with get, set
        abstract parent: Symbol option with get, set
        abstract exportSymbol: Symbol option with get, set
        abstract constEnumOnlyModule: bool option with get, set
        abstract isReferenced: SymbolFlags option with get, set
        abstract lastAssignmentPos: float option with get, set
        abstract isReplaceableByMethod: bool option with get, set
        abstract assignmentDeclarationMembers: Map<float, Declaration> option with get, set

    type [<AllowNullLiteral>] SymbolLinks =
        abstract _symbolLinksBrand: obj option with get, set
        abstract immediateTarget: Symbol option with get, set
        abstract aliasTarget: Symbol option with get, set
        abstract target: Symbol option with get, set
        abstract ``type``: Type option with get, set
        abstract writeType: Type option with get, set
        abstract nameType: Type option with get, set
        abstract uniqueESSymbolType: Type option with get, set
        abstract declaredType: Type option with get, set
        abstract typeParameters: ResizeArray<TypeParameter> option with get, set
        abstract instantiations: Map<string, Type> option with get, set
        abstract inferredClassSymbol: Map<SymbolId, TransientSymbol> option with get, set
        abstract mapper: TypeMapper option with get, set
        abstract referenced: bool option with get, set
        abstract containingType: UnionOrIntersectionType option with get, set
        abstract leftSpread: Symbol option with get, set
        abstract rightSpread: Symbol option with get, set
        abstract syntheticOrigin: Symbol option with get, set
        abstract isDiscriminantProperty: bool option with get, set
        abstract resolvedExports: SymbolTable option with get, set
        abstract resolvedMembers: SymbolTable option with get, set
        abstract exportsChecked: bool option with get, set
        abstract typeParametersChecked: bool option with get, set
        abstract isDeclarationWithCollidingName: bool option with get, set
        abstract originatingImport: U2<ImportDeclaration, ImportCall> option with get, set
        abstract lateSymbol: Symbol option with get, set
        abstract specifierCache: Map<ModeAwareCacheKey, string> option with get, set
        abstract extendedContainers: ResizeArray<Symbol> option with get, set
        abstract extendedContainersByFile: Map<NodeId, ResizeArray<Symbol>> option with get, set
        abstract variances: ResizeArray<VarianceFlags> option with get, set
        abstract deferralConstituents: ResizeArray<Type> option with get, set
        abstract deferralWriteConstituents: ResizeArray<Type> option with get, set
        abstract deferralParent: Type option with get, set
        abstract cjsExportMerged: Symbol option with get, set
        abstract typeOnlyDeclaration: TypeOnlyAliasDeclaration option with get, set
        abstract typeOnlyExportStarMap: Map<__String, obj> option with get, set
        abstract typeOnlyExportStarName: __String option with get, set
        abstract isConstructorDeclaredProperty: bool option with get, set
        abstract tupleLabelDeclaration: U2<NamedTupleMember, ParameterDeclaration> option with get, set
        abstract accessibleChainCache: Map<string, ResizeArray<Symbol> option> option with get, set
        abstract filteredIndexSymbolCache: Map<string, Symbol> option with get, set
        abstract requestedExternalEmitHelpers: ExternalEmitHelpers option with get, set

    type [<RequireQualifiedAccess>] CheckFlags =
        | None = 0
        | Instantiated = 1
        | SyntheticProperty = 2
        | SyntheticMethod = 4
        | Readonly = 8
        | ReadPartial = 16
        | WritePartial = 32
        | HasNonUniformType = 64
        | HasLiteralType = 128
        | ContainsPublic = 256
        | ContainsProtected = 512
        | ContainsPrivate = 1024
        | ContainsStatic = 2048
        | Late = 4096
        | ReverseMapped = 8192
        | OptionalParameter = 16384
        | RestParameter = 32768
        | DeferredType = 65536
        | HasNeverType = 131072
        | Mapped = 262144
        | StripOptional = 524288
        | Unresolved = 1048576
        | Synthetic = 6
        | Discriminant = 192
        | Partial = 48

    type [<AllowNullLiteral>] TransientSymbolLinks =
        inherit SymbolLinks
        abstract checkFlags: CheckFlags with get, set

    type [<AllowNullLiteral>] TransientSymbol =
        inherit Symbol
        abstract links: TransientSymbolLinks with get, set

    type [<AllowNullLiteral>] MappedSymbolLinks =
        inherit TransientSymbolLinks
        abstract mappedType: MappedType with get, set
        abstract keyType: Type with get, set

    type [<AllowNullLiteral>] MappedSymbol =
        inherit TransientSymbol
        abstract links: MappedSymbolLinks with get, set

    type [<AllowNullLiteral>] ReverseMappedSymbolLinks =
        inherit TransientSymbolLinks
        abstract propertyType: Type with get, set
        abstract mappedType: MappedType with get, set
        abstract constraintType: IndexType with get, set

    type [<AllowNullLiteral>] ReverseMappedSymbol =
        inherit TransientSymbol
        abstract links: ReverseMappedSymbolLinks with get, set

    type [<StringEnum>] [<RequireQualifiedAccess>] InternalSymbolName =
        | [<CompiledName("__call")>] Call
        | [<CompiledName("__constructor")>] Constructor
        | [<CompiledName("__new")>] New
        | [<CompiledName("__index")>] Index
        | [<CompiledName("__export")>] ExportStar
        | [<CompiledName("__global")>] Global
        | [<CompiledName("__missing")>] Missing
        | [<CompiledName("__type")>] Type
        | [<CompiledName("__object")>] Object
        | [<CompiledName("__jsxAttributes")>] JSXAttributes
        | [<CompiledName("__class")>] Class
        | [<CompiledName("__function")>] Function
        | [<CompiledName("__computed")>] Computed
        | [<CompiledName("__resolving__")>] Resolving
        | [<CompiledName("export=")>] ExportEquals
        | Default
        | This
        | [<CompiledName("__instantiationExpression")>] InstantiationExpression
        | [<CompiledName("__importAttributes")>] ImportAttributes

    /// This represents a string whose leading underscore have been escaped by adding extra leading underscores.
    /// The shape of this brand is rather unique compared to others we've used.
    /// Instead of just an intersection of a string and an object, it is that union-ed
    /// with an intersection of void and an object. This makes it wholly incompatible
    /// with a normal string (which is good, it cannot be misused on assignment or on usage),
    /// while still being comparable with a normal string via === (also good) and castable from a string.
    type __String =
        U2<obj, InternalSymbolName>

    [<Obsolete("Use ReadonlyMap<__String, T> instead.")>]
    type ReadonlyUnderscoreEscapedMap<'T> =
        ReadonlyMap<__String, 'T>

    [<Obsolete("Use Map<__String, T> instead.")>]
    type UnderscoreEscapedMap<'T> =
        Map<__String, 'T>

    /// SymbolTable based on ES6 Map interface.
    type SymbolTable =
        Map<__String, Symbol>

    /// <summary>Used to track a <c>declare module "foo*"</c>-like declaration.</summary>
    type [<AllowNullLiteral>] PatternAmbientModule =
        abstract pattern: Pattern with get, set
        abstract symbol: Symbol with get, set

    type [<RequireQualifiedAccess>] NodeCheckFlags =
        | None = 0
        | TypeChecked = 1
        | LexicalThis = 2
        | CaptureThis = 4
        | CaptureNewTarget = 8
        | SuperInstance = 16
        | SuperStatic = 32
        | ContextChecked = 64
        | MethodWithSuperPropertyAccessInAsync = 128
        | MethodWithSuperPropertyAssignmentInAsync = 256
        | CaptureArguments = 512
        | EnumValuesComputed = 1024
        | LexicalModuleMergesWithClass = 2048
        | LoopWithCapturedBlockScopedBinding = 4096
        | ContainsCapturedBlockScopeBinding = 8192
        | CapturedBlockScopedBinding = 16384
        | BlockScopedBindingInLoop = 32768
        | NeedsLoopOutParameter = 65536
        | AssignmentsMarked = 131072
        | ContainsConstructorReference = 262144
        | ConstructorReference = 536870912
        | ContainsClassWithPrivateIdentifiers = 1048576
        | ContainsSuperPropertyInStaticInitializer = 2097152
        | InCheckIdentifier = 4194304
        | PartiallyTypeChecked = 8388608
        /// <summary>These flags are LazyNodeCheckFlags and can be calculated lazily by <c>hasNodeCheckFlag</c></summary>
        | LazyFlags = 539358128

    type EvaluatorResult =
        EvaluatorResult<U2<string, float> option>

    type [<AllowNullLiteral>] EvaluatorResult<'T> =
        abstract value: 'T with get, set
        abstract isSyntacticallyString: bool with get, set
        abstract resolvedOtherFiles: bool with get, set
        abstract hasExternalReferences: bool with get, set

    type [<AllowNullLiteral>] NodeLinks =
        abstract flags: NodeCheckFlags with get, set
        abstract calculatedFlags: NodeCheckFlags with get, set
        abstract resolvedType: Type option with get, set
        abstract resolvedSignature: Signature option with get, set
        abstract resolvedSymbol: Symbol option with get, set
        abstract effectsSignature: Signature option with get, set
        abstract enumMemberValue: EvaluatorResult option with get, set
        abstract isVisible: bool option with get, set
        abstract containsArgumentsReference: bool option with get, set
        abstract hasReportedStatementInAmbientContext: bool option with get, set
        abstract jsxFlags: JsxFlags with get, set
        abstract resolvedJsxElementAttributesType: Type option with get, set
        abstract resolvedJSDocType: Type option with get, set
        abstract switchTypes: ResizeArray<Type> option with get, set
        abstract jsxNamespace: Symbol option with get, set
        abstract jsxImplicitImportContainer: Symbol option with get, set
        abstract jsxFragmentType: Type option with get, set
        abstract contextFreeType: Type option with get, set
        abstract deferredNodes: Set<Node> option with get, set
        abstract capturedBlockScopeBindings: ResizeArray<Symbol> option with get, set
        abstract outerTypeParameters: ResizeArray<TypeParameter> option with get, set
        abstract isExhaustive: U2<bool, float> option with get, set
        abstract skipDirectInference: bool option with get, set
        abstract declarationRequiresScopeChange: bool option with get, set
        abstract serializedTypes: Map<string, SerializedTypeEntry> option with get, set
        abstract decoratorSignature: Signature option with get, set
        abstract spreadIndices: {| first: float option; last: float option |} option with get, set
        abstract parameterInitializerContainsUndefined: bool option with get, set
        abstract fakeScopeForSignatureDeclaration: NodeLinksFakeScopeForSignatureDeclaration option with get, set
        abstract assertionExpressionType: Type option with get, set
        abstract potentialThisCollisions: ResizeArray<Node> option with get, set
        abstract potentialNewTargetCollisions: ResizeArray<Node> option with get, set
        abstract potentialWeakMapSetCollisions: ResizeArray<Node> option with get, set
        abstract potentialReflectCollisions: ResizeArray<Node> option with get, set
        abstract potentialUnusedRenamedBindingElementsInTypes: ResizeArray<BindingElement> option with get, set
        abstract externalHelpersModule: Symbol option with get, set
        abstract instantiationExpressionTypes: Map<float, Type> option with get, set
        abstract nonExistentPropCheckCache: Set<string> option with get, set

    type TrackedSymbol =
        obj * obj * obj

    type [<AllowNullLiteral>] SerializedTypeEntry =
        abstract node: TypeNode with get, set
        abstract truncating: bool option with get, set
        abstract addedLength: float with get, set
        abstract trackedSymbols: ResizeArray<TrackedSymbol> option with get, set

    type [<RequireQualifiedAccess>] TypeFlags =
        | Any = 1
        | Unknown = 2
        | String = 4
        | Number = 8
        | Boolean = 16
        | Enum = 32
        | BigInt = 64
        | StringLiteral = 128
        | NumberLiteral = 256
        | BooleanLiteral = 512
        | EnumLiteral = 1024
        | BigIntLiteral = 2048
        | ESSymbol = 4096
        | UniqueESSymbol = 8192
        | Void = 16384
        | Undefined = 32768
        | Null = 65536
        | Never = 131072
        | TypeParameter = 262144
        | Object = 524288
        | Union = 1048576
        | Intersection = 2097152
        | Index = 4194304
        | IndexedAccess = 8388608
        | Conditional = 16777216
        | Substitution = 33554432
        | NonPrimitive = 67108864
        | TemplateLiteral = 134217728
        | StringMapping = 268435456
        | Reserved1 = 536870912
        | Reserved2 = 1073741824
        | AnyOrUnknown = 3
        | Nullable = 98304
        | Literal = 2944
        | Unit = 109472
        | Freshable = 2976
        | StringOrNumberLiteral = 384
        | StringOrNumberLiteralOrUnique = 8576
        | DefinitelyFalsy = 117632
        | PossiblyFalsy = 117724
        | Intrinsic = 67359327
        | StringLike = 402653316
        | NumberLike = 296
        | BigIntLike = 2112
        | BooleanLike = 528
        | EnumLike = 1056
        | ESSymbolLike = 12288
        | VoidLike = 49152
        | Primitive = 402784252
        | DefinitelyNonNullable = 470302716
        | DisjointDomains = 469892092
        | UnionOrIntersection = 3145728
        | StructuredType = 3670016
        | TypeVariable = 8650752
        | InstantiableNonPrimitive = 58982400
        | InstantiablePrimitive = 406847488
        | Instantiable = 465829888
        | StructuredOrInstantiable = 469499904
        | ObjectFlagsType = 3899393
        | Simplifiable = 25165824
        | Singleton = 67358815
        | Narrowable = 536624127
        | IncludesMask = 473694207
        | IncludesMissingType = 262144
        | IncludesNonWideningType = 4194304
        | IncludesWildcard = 8388608
        | IncludesEmptyObject = 16777216
        | IncludesInstantiable = 33554432
        | IncludesConstrainedTypeVariable = 536870912
        | IncludesError = 1073741824
        | NotPrimitiveUnion = 36323331

    type DestructuringPattern =
        U3<BindingPattern, ObjectLiteralExpression, ArrayLiteralExpression>

    type TypeId =
        float

    type [<AllowNullLiteral>] Type =
        abstract flags: TypeFlags with get, set
        abstract id: TypeId with get, set
        abstract checker: TypeChecker with get, set
        abstract symbol: Symbol with get, set
        abstract pattern: DestructuringPattern option with get, set
        abstract aliasSymbol: Symbol option with get, set
        abstract aliasTypeArguments: ResizeArray<Type> option with get, set
        abstract permissiveInstantiation: Type option with get, set
        abstract restrictiveInstantiation: Type option with get, set
        abstract immediateBaseConstraint: Type option with get, set
        abstract widened: Type option with get, set

    type [<AllowNullLiteral>] IntrinsicType =
        inherit Type
        abstract intrinsicName: string with get, set
        abstract debugIntrinsicName: string option with get, set
        abstract objectFlags: ObjectFlags with get, set

    type [<AllowNullLiteral>] NullableType =
        inherit IntrinsicType
        abstract objectFlags: ObjectFlags with get, set

    type [<AllowNullLiteral>] FreshableType =
        inherit Type
        abstract freshType: FreshableType with get, set
        abstract regularType: FreshableType with get, set

    type [<AllowNullLiteral>] FreshableIntrinsicType =
        inherit FreshableType
        inherit IntrinsicType

    type [<AllowNullLiteral>] LiteralType =
        inherit FreshableType
        abstract value: U3<string, float, PseudoBigInt> with get, set

    type [<AllowNullLiteral>] UniqueESSymbolType =
        inherit Type
        abstract symbol: Symbol with get, set
        abstract escapedName: __String with get, set

    type [<AllowNullLiteral>] StringLiteralType =
        inherit LiteralType
        abstract value: string with get, set

    type [<AllowNullLiteral>] NumberLiteralType =
        inherit LiteralType
        abstract value: float with get, set

    type [<AllowNullLiteral>] BigIntLiteralType =
        inherit LiteralType
        abstract value: PseudoBigInt with get, set

    type [<AllowNullLiteral>] EnumType =
        inherit FreshableType

    type [<RequireQualifiedAccess>] ObjectFlags =
        | None = 0
        | Class = 1
        | Interface = 2
        | Reference = 4
        | Tuple = 8
        | Anonymous = 16
        | Mapped = 32
        | Instantiated = 64
        | ObjectLiteral = 128
        | EvolvingArray = 256
        | ObjectLiteralPatternWithComputedProperties = 512
        | ReverseMapped = 1024
        | JsxAttributes = 2048
        | JSLiteral = 4096
        | FreshLiteral = 8192
        | ArrayLiteral = 16384
        | PrimitiveUnion = 32768
        | ContainsWideningType = 65536
        | ContainsObjectOrArrayLiteral = 131072
        | NonInferrableType = 262144
        | CouldContainTypeVariablesComputed = 524288
        | CouldContainTypeVariables = 1048576
        | SingleSignatureType = 134217728
        | ClassOrInterface = 3
        | RequiresWidening = 196608
        | PropagatingFlags = 458752
        | InstantiatedMapped = 96
        | ObjectTypeKindMask = 1343
        | ContainsSpread = 2097152
        | ObjectRestType = 4194304
        | InstantiationExpressionType = 8388608
        | IsClassInstanceClone = 16777216
        | IdenticalBaseTypeCalculated = 33554432
        | IdenticalBaseTypeExists = 67108864
        | IsGenericTypeComputed = 2097152
        | IsGenericObjectType = 4194304
        | IsGenericIndexType = 8388608
        | IsGenericType = 12582912
        | ContainsIntersections = 16777216
        | IsUnknownLikeUnionComputed = 33554432
        | IsUnknownLikeUnion = 67108864
        | IsNeverIntersectionComputed = 16777216
        | IsNeverIntersection = 33554432
        | IsConstrainedTypeVariable = 67108864

    type ObjectFlagsType =
        U4<NullableType, ObjectType, UnionType, IntersectionType>

    type [<AllowNullLiteral>] ObjectType =
        inherit Type
        abstract objectFlags: ObjectFlags with get, set
        abstract members: SymbolTable option with get, set
        abstract properties: ResizeArray<Symbol> option with get, set
        abstract callSignatures: ResizeArray<Signature> option with get, set
        abstract constructSignatures: ResizeArray<Signature> option with get, set
        abstract indexInfos: ResizeArray<IndexInfo> option with get, set
        abstract objectTypeWithoutAbstractConstructSignatures: ObjectType option with get, set

    /// Class and interface types (ObjectFlags.Class and ObjectFlags.Interface).
    type [<AllowNullLiteral>] InterfaceType =
        inherit ObjectType
        abstract typeParameters: ResizeArray<TypeParameter> option with get, set
        abstract outerTypeParameters: ResizeArray<TypeParameter> option with get, set
        abstract localTypeParameters: ResizeArray<TypeParameter> option with get, set
        abstract thisType: TypeParameter option with get, set
        abstract resolvedBaseConstructorType: Type option with get, set
        abstract resolvedBaseTypes: ResizeArray<BaseType> with get, set
        abstract baseTypesResolved: bool option with get, set

    type BaseType =
        U3<ObjectType, IntersectionType, TypeVariable>

    type [<AllowNullLiteral>] InterfaceTypeWithDeclaredMembers =
        inherit InterfaceType
        abstract declaredProperties: ResizeArray<Symbol> with get, set
        abstract declaredCallSignatures: ResizeArray<Signature> with get, set
        abstract declaredConstructSignatures: ResizeArray<Signature> with get, set
        abstract declaredIndexInfos: ResizeArray<IndexInfo> with get, set

    /// Type references (ObjectFlags.Reference). When a class or interface has type parameters or
    /// a "this" type, references to the class or interface are made using type references. The
    /// typeArguments property specifies the types to substitute for the type parameters of the
    /// class or interface and optionally includes an extra element that specifies the type to
    /// substitute for "this" in the resulting instantiation. When no extra argument is present,
    /// the type reference itself is substituted for "this". The typeArguments property is undefined
    /// if the class or interface has no type parameters and the reference isn't specifying an
    /// explicit "this" argument.
    type [<AllowNullLiteral>] TypeReference =
        inherit ObjectType
        abstract target: GenericType with get, set
        abstract node: U3<TypeReferenceNode, ArrayTypeNode, TupleTypeNode> option with get, set
        abstract mapper: TypeMapper option with get, set
        abstract resolvedTypeArguments: ResizeArray<Type> option with get, set
        abstract literalType: TypeReference option with get, set
        abstract cachedEquivalentBaseType: Type option with get, set

    type [<AllowNullLiteral>] DeferredTypeReference =
        inherit TypeReference
        abstract node: U3<TypeReferenceNode, ArrayTypeNode, TupleTypeNode> with get, set
        abstract mapper: TypeMapper option with get, set
        abstract instantiations: Map<string, Type> option with get, set

    type [<RequireQualifiedAccess>] VarianceFlags =
        | Invariant = 0
        | Covariant = 1
        | Contravariant = 2
        | Bivariant = 3
        | Independent = 4
        | VarianceMask = 7
        | Unmeasurable = 8
        | Unreliable = 16
        | AllowsStructuralFallback = 24

    type [<AllowNullLiteral>] GenericType =
        inherit InterfaceType
        inherit TypeReference
        abstract instantiations: Map<string, TypeReference> with get, set
        abstract variances: ResizeArray<VarianceFlags> option with get, set

    type [<RequireQualifiedAccess>] ElementFlags =
        | Required = 1
        | Optional = 2
        | Rest = 4
        | Variadic = 8
        | Fixed = 3
        | Variable = 12
        | NonRequired = 14
        | NonRest = 11

    type [<AllowNullLiteral>] TupleType =
        inherit GenericType
        abstract elementFlags: ResizeArray<ElementFlags> with get, set
        /// Number of required or variadic elements
        abstract minLength: float with get, set
        /// Number of initial required or optional elements
        abstract fixedLength: float with get, set
        /// True if tuple has any rest or variadic elements
        [<Obsolete("Use `.combinedFlags & ElementFlags.Variable` instead")>]
        abstract hasRestElement: bool with get, set
        abstract combinedFlags: ElementFlags with get, set
        abstract readonly: bool with get, set
        abstract labeledElementDeclarations: ResizeArray<U2<NamedTupleMember, ParameterDeclaration> option> option with get, set

    type [<AllowNullLiteral>] TupleTypeReference =
        inherit TypeReference
        abstract target: TupleType with get, set

    type [<AllowNullLiteral>] UnionOrIntersectionType =
        inherit Type
        abstract types: ResizeArray<Type> with get, set
        abstract objectFlags: ObjectFlags with get, set
        abstract propertyCache: SymbolTable option with get, set
        abstract propertyCacheWithoutObjectFunctionPropertyAugment: SymbolTable option with get, set
        abstract resolvedProperties: ResizeArray<Symbol> with get, set
        abstract resolvedIndexType: IndexType with get, set
        abstract resolvedStringIndexType: IndexType with get, set
        abstract resolvedBaseConstraint: Type with get, set

    type [<AllowNullLiteral>] UnionType =
        inherit UnionOrIntersectionType
        abstract resolvedReducedType: Type option with get, set
        abstract regularType: UnionType option with get, set
        abstract origin: Type option with get, set
        abstract keyPropertyName: __String option with get, set
        abstract constituentMap: Map<TypeId, Type> option with get, set
        abstract arrayFallbackSignatures: ResizeArray<Signature> option with get, set

    type [<AllowNullLiteral>] IntersectionType =
        inherit UnionOrIntersectionType
        abstract resolvedApparentType: Type with get, set
        abstract uniqueLiteralFilledInstantiation: Type option with get, set

    type StructuredType =
        U3<ObjectType, UnionType, IntersectionType>

    type [<AllowNullLiteral>] AnonymousType =
        inherit ObjectType
        abstract target: AnonymousType option with get, set
        abstract mapper: TypeMapper option with get, set
        abstract instantiations: Map<string, Type> option with get, set

    type [<AllowNullLiteral>] InstantiationExpressionType =
        inherit AnonymousType
        abstract node: NodeWithTypeArguments with get, set

    type [<AllowNullLiteral>] MappedType =
        inherit AnonymousType
        abstract declaration: MappedTypeNode with get, set
        abstract typeParameter: TypeParameter option with get, set
        abstract constraintType: Type option with get, set
        abstract nameType: Type option with get, set
        abstract templateType: Type option with get, set
        abstract modifiersType: Type option with get, set
        abstract resolvedApparentType: Type option with get, set
        abstract containsError: bool option with get, set

    type [<AllowNullLiteral>] EvolvingArrayType =
        inherit ObjectType
        abstract elementType: Type with get, set
        abstract finalArrayType: Type option with get, set

    type [<AllowNullLiteral>] ReverseMappedType =
        inherit ObjectType
        abstract source: Type with get, set
        abstract mappedType: MappedType with get, set
        abstract constraintType: IndexType with get, set

    type [<AllowNullLiteral>] ResolvedType =
        inherit ObjectType
        inherit UnionOrIntersectionType
        abstract members: SymbolTable with get, set
        abstract properties: ResizeArray<Symbol> with get, set
        abstract callSignatures: ResizeArray<Signature> with get, set
        abstract constructSignatures: ResizeArray<Signature> with get, set
        abstract indexInfos: ResizeArray<IndexInfo> with get, set

    type [<AllowNullLiteral>] FreshObjectLiteralType =
        inherit ResolvedType
        abstract regularType: ResolvedType with get, set

    type [<AllowNullLiteral>] IterationTypes =
        abstract yieldType: Type
        abstract returnType: Type
        abstract nextType: Type

    type [<AllowNullLiteral>] IterableOrIteratorType =
        inherit ObjectType
        inherit UnionType
        abstract iterationTypesOfGeneratorReturnType: IterationTypes option with get, set
        abstract iterationTypesOfAsyncGeneratorReturnType: IterationTypes option with get, set
        abstract iterationTypesOfIterable: IterationTypes option with get, set
        abstract iterationTypesOfIterator: IterationTypes option with get, set
        abstract iterationTypesOfAsyncIterable: IterationTypes option with get, set
        abstract iterationTypesOfAsyncIterator: IterationTypes option with get, set
        abstract iterationTypesOfIteratorResult: IterationTypes option with get, set

    type [<AllowNullLiteral>] PromiseOrAwaitableType =
        inherit ObjectType
        inherit UnionType
        abstract promiseTypeOfPromiseConstructor: Type option with get, set
        abstract promisedTypeOfPromise: Type option with get, set
        abstract awaitedTypeOfType: Type option with get, set

    type [<AllowNullLiteral>] SyntheticDefaultModuleType =
        inherit Type
        abstract syntheticType: Type option with get, set
        abstract defaultOnlyType: Type option with get, set

    type [<AllowNullLiteral>] InstantiableType =
        inherit Type
        abstract resolvedBaseConstraint: Type option with get, set
        abstract resolvedIndexType: IndexType option with get, set
        abstract resolvedStringIndexType: IndexType option with get, set

    type [<AllowNullLiteral>] TypeParameter =
        inherit InstantiableType
        /// <summary>Retrieve using getConstraintFromTypeParameter</summary>
        abstract ``constraint``: Type option with get, set
        abstract ``default``: Type option with get, set
        abstract target: TypeParameter option with get, set
        abstract mapper: TypeMapper option with get, set
        abstract isThisType: bool option with get, set
        abstract resolvedDefaultType: Type option with get, set

    type [<RequireQualifiedAccess>] AccessFlags =
        | None = 0
        | IncludeUndefined = 1
        | NoIndexSignatures = 2
        | Writing = 4
        | CacheSymbol = 8
        | AllowMissing = 16
        | ExpressionPosition = 32
        | ReportDeprecated = 64
        | SuppressNoImplicitAnyError = 128
        | Contextual = 256
        | Persistent = 1

    type [<AllowNullLiteral>] IndexedAccessType =
        inherit InstantiableType
        abstract objectType: Type with get, set
        abstract indexType: Type with get, set
        abstract accessFlags: AccessFlags with get, set
        abstract ``constraint``: Type option with get, set
        abstract simplifiedForReading: Type option with get, set
        abstract simplifiedForWriting: Type option with get, set

    type TypeVariable =
        U2<TypeParameter, IndexedAccessType>

    type [<RequireQualifiedAccess>] IndexFlags =
        | None = 0
        | StringsOnly = 1
        | NoIndexSignatures = 2
        | NoReducibleCheck = 4

    type [<AllowNullLiteral>] IndexType =
        inherit InstantiableType
        abstract ``type``: U2<InstantiableType, UnionOrIntersectionType> with get, set
        abstract indexFlags: IndexFlags with get, set

    type [<AllowNullLiteral>] ConditionalRoot =
        abstract node: ConditionalTypeNode with get, set
        abstract checkType: Type with get, set
        abstract extendsType: Type with get, set
        abstract isDistributive: bool with get, set
        abstract inferTypeParameters: ResizeArray<TypeParameter> option with get, set
        abstract outerTypeParameters: ResizeArray<TypeParameter> option with get, set
        abstract instantiations: Map<string, Type> option with get, set
        abstract aliasSymbol: Symbol option with get, set
        abstract aliasTypeArguments: ResizeArray<Type> option with get, set

    type [<AllowNullLiteral>] ConditionalType =
        inherit InstantiableType
        abstract root: ConditionalRoot with get, set
        abstract checkType: Type with get, set
        abstract extendsType: Type with get, set
        abstract resolvedTrueType: Type option with get, set
        abstract resolvedFalseType: Type option with get, set
        abstract resolvedInferredTrueType: Type option with get, set
        abstract resolvedDefaultConstraint: Type option with get, set
        abstract resolvedConstraintOfDistributive: Type option with get, set
        abstract mapper: TypeMapper option with get, set
        abstract combinedMapper: TypeMapper option with get, set

    type [<AllowNullLiteral>] TemplateLiteralType =
        inherit InstantiableType
        abstract texts: ResizeArray<string> with get, set
        abstract types: ResizeArray<Type> with get, set

    type [<AllowNullLiteral>] StringMappingType =
        inherit InstantiableType
        abstract symbol: Symbol with get, set
        abstract ``type``: Type with get, set

    type [<AllowNullLiteral>] SubstitutionType =
        inherit InstantiableType
        abstract objectFlags: ObjectFlags with get, set
        abstract baseType: Type with get, set
        abstract ``constraint``: Type with get, set

    type [<RequireQualifiedAccess>] JsxReferenceKind =
        | Component = 0
        | Function = 1
        | Mixed = 2

    type [<RequireQualifiedAccess>] SignatureKind =
        | Call = 0
        | Construct = 1

    type [<RequireQualifiedAccess>] SignatureFlags =
        | None = 0
        | HasRestParameter = 1
        | HasLiteralTypes = 2
        | Abstract = 4
        | IsInnerCallChain = 8
        | IsOuterCallChain = 16
        | IsUntypedSignatureInJSFile = 32
        | IsNonInferrable = 64
        | IsSignatureCandidateForOverloadFailure = 128
        | PropagatingFlags = 167
        | CallChainFlags = 24

    type [<AllowNullLiteral>] Signature =
        abstract flags: SignatureFlags with get, set
        abstract checker: TypeChecker option with get, set
        abstract declaration: U2<SignatureDeclaration, JSDocSignature> option with get, set
        abstract typeParameters: ResizeArray<TypeParameter> option with get, set
        abstract parameters: ResizeArray<Symbol> with get, set
        abstract thisParameter: Symbol option with get, set
        abstract resolvedReturnType: Type option with get, set
        abstract resolvedTypePredicate: TypePredicate option with get, set
        abstract minArgumentCount: float with get, set
        abstract resolvedMinArgumentCount: float option with get, set
        abstract target: Signature option with get, set
        abstract mapper: TypeMapper option with get, set
        abstract compositeSignatures: ResizeArray<Signature> option with get, set
        abstract compositeKind: TypeFlags option with get, set
        abstract erasedSignatureCache: Signature option with get, set
        abstract canonicalSignatureCache: Signature option with get, set
        abstract baseSignatureCache: Signature option with get, set
        abstract optionalCallSignatureCache: {| inner: Signature option; outer: Signature option |} option with get, set
        abstract isolatedSignatureType: ObjectType option with get, set
        abstract instantiations: Map<string, Signature> option with get, set

    type [<RequireQualifiedAccess>] IndexKind =
        | String = 0
        | Number = 1

    type [<AllowNullLiteral>] ElementWithComputedPropertyName =
        interface end

    type [<AllowNullLiteral>] IndexInfo =
        abstract keyType: Type with get, set
        abstract ``type``: Type with get, set
        abstract isReadonly: bool with get, set
        abstract declaration: IndexSignatureDeclaration option with get, set
        abstract components: ResizeArray<ElementWithComputedPropertyName> option with get, set

    type [<RequireQualifiedAccess>] TypeMapKind =
        | Simple = 0
        | Array = 1
        | Deferred = 2
        | Function = 3
        | Composite = 4
        | Merged = 5

    type TypeMapper =
        U5<{| kind: TypeMapKind; source: Type; target: Type |}, {| kind: TypeMapKind; sources: ResizeArray<Type>; targets: ResizeArray<Type> option |}, {| kind: TypeMapKind; sources: ResizeArray<Type>; targets: ResizeArray<(unit -> Type)> |}, {| kind: TypeMapKind; func: Type -> Type; debugInfo: (unit -> string) option |}, {| kind: TypeMapKind; mapper1: TypeMapper; mapper2: TypeMapper |}>

    type [<RequireQualifiedAccess>] InferencePriority =
        | None = 0
        | NakedTypeVariable = 1
        | SpeculativeTuple = 2
        | SubstituteSource = 4
        | HomomorphicMappedType = 8
        | PartialHomomorphicMappedType = 16
        | MappedTypeConstraint = 32
        | ContravariantConditional = 64
        | ReturnType = 128
        | LiteralKeyof = 256
        | NoConstraints = 512
        | AlwaysStrict = 1024
        | MaxValue = 2048
        | PriorityImpliesCombination = 416
        | Circularity = -1

    type [<AllowNullLiteral>] InferenceInfo =
        abstract typeParameter: TypeParameter with get, set
        abstract candidates: ResizeArray<Type> option with get, set
        abstract contraCandidates: ResizeArray<Type> option with get, set
        abstract inferredType: Type option with get, set
        abstract priority: InferencePriority option with get, set
        abstract topLevel: bool with get, set
        abstract isFixed: bool with get, set
        abstract impliedArity: float option with get, set

    type [<RequireQualifiedAccess>] InferenceFlags =
        | None = 0
        | NoDefault = 1
        | AnyDefault = 2
        | SkippedGenericFunction = 4

    /// <summary>
    /// Ternary values are defined such that
    /// x &amp; y picks the lesser in the order False &lt; Unknown &lt; Maybe &lt; True, and
    /// x | y picks the greater in the order False &lt; Unknown &lt; Maybe &lt; True.
    /// Generally, Ternary.Maybe is used as the result of a relation that depends on itself, and
    /// Ternary.Unknown is used as the result of a variance check that depends on itself. We make
    /// a distinction because we don't want to cache circular variance check results.
    /// </summary>
    type [<RequireQualifiedAccess>] Ternary =
        | False = 0
        | Unknown = 1
        | Maybe = 3
        | True = -1

    type [<AllowNullLiteral>] TypeComparer =
        [<Emit("$0($1...)")>] abstract Invoke: s: Type * t: Type * ?reportErrors: bool -> Ternary

    type [<AllowNullLiteral>] InferenceContext =
        abstract inferences: ResizeArray<InferenceInfo> with get, set
        abstract signature: Signature option with get, set
        abstract flags: InferenceFlags with get, set
        abstract compareTypes: TypeComparer with get, set
        abstract mapper: TypeMapper with get, set
        abstract nonFixingMapper: TypeMapper with get, set
        abstract returnMapper: TypeMapper option with get, set
        abstract outerReturnMapper: TypeMapper option with get, set
        abstract inferredTypeParameters: ResizeArray<TypeParameter> option with get, set
        abstract intraExpressionInferenceSites: ResizeArray<IntraExpressionInferenceSite> option with get, set

    type [<AllowNullLiteral>] IntraExpressionInferenceSite =
        abstract node: U2<Expression, MethodDeclaration> with get, set
        abstract ``type``: Type with get, set

    type [<AllowNullLiteral>] WideningContext =
        abstract parent: WideningContext option with get, set
        abstract propertyName: __String option with get, set
        abstract siblings: ResizeArray<Type> option with get, set
        abstract resolvedProperties: ResizeArray<Symbol> option with get, set

    type [<RequireQualifiedAccess>] AssignmentDeclarationKind =
        | None = 0
        | ExportsProperty = 1
        | ModuleExports = 2
        | PrototypeProperty = 3
        | ThisProperty = 4
        | Property = 5
        | Prototype = 6
        | ObjectDefinePropertyValue = 7
        | ObjectDefinePropertyExports = 8
        | ObjectDefinePrototypeProperty = 9

    type [<AllowNullLiteral>] FileExtensionInfo =
        abstract extension: string with get, set
        abstract isMixedContent: bool with get, set
        abstract scriptKind: ScriptKind option with get, set

    type [<AllowNullLiteral>] DiagnosticMessage =
        abstract key: string with get, set
        abstract category: DiagnosticCategory with get, set
        abstract code: float with get, set
        abstract message: string with get, set
        abstract reportsUnnecessary: DiagnosticMessageReportsUnnecessary option with get, set
        abstract reportsDeprecated: DiagnosticMessageReportsUnnecessary option with get, set
        abstract elidedInCompatabilityPyramid: bool option with get, set

    type [<AllowNullLiteral>] RepopulateModuleNotFoundDiagnosticChain =
        abstract moduleReference: string with get, set
        abstract mode: ResolutionMode with get, set
        abstract packageName: string option with get, set

    type RepopulateModeMismatchDiagnosticChain =
        bool

    type RepopulateDiagnosticChainInfo =
        U2<RepopulateModuleNotFoundDiagnosticChain, RepopulateModeMismatchDiagnosticChain>

    /// A linked list of formatted diagnostic messages to be used as part of a multiline message.
    /// It is built from the bottom up, leaving the head to be the "main" diagnostic.
    /// While it seems that DiagnosticMessageChain is structurally similar to DiagnosticMessage,
    /// the difference is that messages are all preformatted in DMC.
    type [<AllowNullLiteral>] DiagnosticMessageChain =
        abstract messageText: string with get, set
        abstract category: DiagnosticCategory with get, set
        abstract code: float with get, set
        abstract next: ResizeArray<DiagnosticMessageChain> option with get, set
        abstract repopulateInfo: (unit -> RepopulateDiagnosticChainInfo) option with get, set
        abstract canonicalHead: CanonicalDiagnostic option with get, set

    type [<AllowNullLiteral>] Diagnostic =
        inherit DiagnosticRelatedInformation
        /// <summary>May store more in future. For now, this will simply be <c>true</c> to indicate when a diagnostic is an unused-identifier diagnostic.</summary>
        abstract reportsUnnecessary: DiagnosticMessageReportsUnnecessary option with get, set
        abstract reportsDeprecated: DiagnosticMessageReportsUnnecessary option with get, set
        abstract source: string option with get, set
        abstract relatedInformation: ResizeArray<DiagnosticRelatedInformation> option with get, set
        abstract skippedOn: KeyOf<CompilerOptions> option with get, set
        abstract canonicalHead: CanonicalDiagnostic option with get, set

    type [<AllowNullLiteral>] CanonicalDiagnostic =
        abstract code: float with get, set
        abstract messageText: string with get, set

    type DiagnosticArguments =
        ResizeArray<U2<string, float>>

    type DiagnosticAndArguments =
        obj * obj

    type [<AllowNullLiteral>] DiagnosticRelatedInformation =
        abstract category: DiagnosticCategory with get, set
        abstract code: float with get, set
        abstract file: SourceFile option with get, set
        abstract start: float option with get, set
        abstract length: float option with get, set
        abstract messageText: U2<string, DiagnosticMessageChain> with get, set

    type [<AllowNullLiteral>] DiagnosticWithLocation =
        inherit Diagnostic
        abstract file: SourceFile with get, set
        abstract start: float with get, set
        abstract length: float with get, set

    type [<AllowNullLiteral>] DiagnosticWithDetachedLocation =
        inherit Diagnostic
        abstract file: obj with get, set
        abstract fileName: string with get, set
        abstract start: float with get, set
        abstract length: float with get, set

    type [<RequireQualifiedAccess>] DiagnosticCategory =
        | Warning = 0
        | Error = 1
        | Suggestion = 2
        | Message = 3

    type [<RequireQualifiedAccess>] ModuleResolutionKind =
        | Classic = 1
        /// <deprecated>
        /// <c>NodeJs</c> was renamed to <c>Node10</c> to better reflect the version of Node that it targets.
        /// Use the new name or consider switching to a modern module resolution target.
        /// </deprecated>
        | NodeJs = 2
        | Node10 = 2
        | Node16 = 3
        | NodeNext = 99
        | Bundler = 100

    type [<RequireQualifiedAccess>] ModuleDetectionKind =
        /// Files with imports, exports and/or import.meta are considered modules
        | Legacy = 1
        /// Legacy, but also files with jsx under react-jsx or react-jsxdev and esm mode files under moduleResolution: node16+
        | Auto = 2
        /// Consider all non-declaration files modules, regardless of present syntax
        | Force = 3

    type [<AllowNullLiteral>] PluginImport =
        abstract name: string with get, set

    type [<AllowNullLiteral>] ProjectReference =
        /// A normalized path on disk
        abstract path: string with get, set
        /// The path as the user originally wrote it
        abstract originalPath: string option with get, set
        [<Obsolete("")>]
        abstract prepend: bool option with get, set
        /// True if it is intended that this reference form a circularity
        abstract circular: bool option with get, set

    type [<RequireQualifiedAccess>] WatchFileKind =
        | FixedPollingInterval = 0
        | PriorityPollingInterval = 1
        | DynamicPriorityPolling = 2
        | FixedChunkSizePolling = 3
        | UseFsEvents = 4
        | UseFsEventsOnParentDirectory = 5

    type [<RequireQualifiedAccess>] WatchDirectoryKind =
        | UseFsEvents = 0
        | FixedPollingInterval = 1
        | DynamicPriorityPolling = 2
        | FixedChunkSizePolling = 3

    type [<RequireQualifiedAccess>] PollingWatchKind =
        | FixedInterval = 0
        | PriorityInterval = 1
        | DynamicPriority = 2
        | FixedChunkSize = 3

    type CompilerOptionsValue =
        U8<string, float, bool, ResizeArray<U2<string, float>>, ResizeArray<string>, MapLike<ResizeArray<string>>, ResizeArray<PluginImport>, ResizeArray<ProjectReference>> option

    type [<AllowNullLiteral>] CompilerOptions =
        abstract all: bool option with get, set
        abstract allowImportingTsExtensions: bool option with get, set
        abstract allowJs: bool option with get, set
        abstract allowNonTsExtensions: bool option with get, set
        abstract allowArbitraryExtensions: bool option with get, set
        abstract allowSyntheticDefaultImports: bool option with get, set
        abstract allowUmdGlobalAccess: bool option with get, set
        abstract allowUnreachableCode: bool option with get, set
        abstract allowUnusedLabels: bool option with get, set
        abstract alwaysStrict: bool option with get, set
        abstract baseUrl: string option with get, set
        /// <summary>An error if set - this should only go through the -b pipeline and not actually be observed</summary>
        abstract build: bool option with get, set
        [<Obsolete("")>]
        abstract charset: string option with get, set
        abstract checkJs: bool option with get, set
        abstract configFilePath: string option with get, set
        /// <summary>configFile is set as non enumerable property so as to avoid checking of json source files</summary>
        abstract configFile: TsConfigSourceFile option
        abstract customConditions: ResizeArray<string> option with get, set
        abstract declaration: bool option with get, set
        abstract declarationMap: bool option with get, set
        abstract emitDeclarationOnly: bool option with get, set
        abstract declarationDir: string option with get, set
        abstract diagnostics: bool option with get, set
        abstract extendedDiagnostics: bool option with get, set
        abstract disableSizeLimit: bool option with get, set
        abstract disableSourceOfProjectReferenceRedirect: bool option with get, set
        abstract disableSolutionSearching: bool option with get, set
        abstract disableReferencedProjectLoad: bool option with get, set
        abstract downlevelIteration: bool option with get, set
        abstract emitBOM: bool option with get, set
        abstract emitDecoratorMetadata: bool option with get, set
        abstract exactOptionalPropertyTypes: bool option with get, set
        abstract experimentalDecorators: bool option with get, set
        abstract forceConsistentCasingInFileNames: bool option with get, set
        abstract generateCpuProfile: string option with get, set
        abstract generateTrace: string option with get, set
        abstract help: bool option with get, set
        abstract ignoreDeprecations: string option with get, set
        abstract importHelpers: bool option with get, set
        [<Obsolete("")>]
        abstract importsNotUsedAsValues: ImportsNotUsedAsValues option with get, set
        abstract init: bool option with get, set
        abstract inlineSourceMap: bool option with get, set
        abstract inlineSources: bool option with get, set
        abstract isolatedModules: bool option with get, set
        abstract isolatedDeclarations: bool option with get, set
        abstract jsx: JsxEmit option with get, set
        [<Obsolete("")>]
        abstract keyofStringsOnly: bool option with get, set
        abstract lib: ResizeArray<string> option with get, set
        abstract libReplacement: bool option with get, set
        abstract listEmittedFiles: bool option with get, set
        abstract listFiles: bool option with get, set
        abstract explainFiles: bool option with get, set
        abstract listFilesOnly: bool option with get, set
        abstract locale: string option with get, set
        abstract mapRoot: string option with get, set
        abstract maxNodeModuleJsDepth: float option with get, set
        abstract ``module``: ModuleKind option with get, set
        abstract moduleResolution: ModuleResolutionKind option with get, set
        abstract moduleSuffixes: ResizeArray<string> option with get, set
        abstract moduleDetection: ModuleDetectionKind option with get, set
        abstract newLine: NewLineKind option with get, set
        abstract noEmit: bool option with get, set
        abstract noCheck: bool option with get, set
        abstract noEmitForJsFiles: bool option with get, set
        abstract noEmitHelpers: bool option with get, set
        abstract noEmitOnError: bool option with get, set
        abstract noErrorTruncation: bool option with get, set
        abstract noFallthroughCasesInSwitch: bool option with get, set
        abstract noImplicitAny: bool option with get, set
        abstract noImplicitReturns: bool option with get, set
        abstract noImplicitThis: bool option with get, set
        [<Obsolete("")>]
        abstract noStrictGenericChecks: bool option with get, set
        abstract noUnusedLocals: bool option with get, set
        abstract noUnusedParameters: bool option with get, set
        [<Obsolete("")>]
        abstract noImplicitUseStrict: bool option with get, set
        abstract noPropertyAccessFromIndexSignature: bool option with get, set
        abstract assumeChangesOnlyAffectDirectDependencies: bool option with get, set
        abstract noLib: bool option with get, set
        abstract noResolve: bool option with get, set
        abstract noDtsResolution: bool option with get, set
        abstract noUncheckedIndexedAccess: bool option with get, set
        [<Obsolete("")>]
        abstract out: string option with get, set
        abstract outDir: string option with get, set
        abstract outFile: string option with get, set
        abstract paths: MapLike<ResizeArray<string>> option with get, set
        /// <summary>The directory of the config file that specified 'paths'. Used to resolve relative paths when 'baseUrl' is absent.</summary>
        abstract pathsBasePath: string option with get, set
        abstract plugins: ResizeArray<PluginImport> option with get, set
        abstract preserveConstEnums: bool option with get, set
        abstract noImplicitOverride: bool option with get, set
        abstract preserveSymlinks: bool option with get, set
        [<Obsolete("")>]
        abstract preserveValueImports: bool option with get, set
        abstract preserveWatchOutput: bool option with get, set
        abstract project: string option with get, set
        abstract pretty: bool option with get, set
        abstract reactNamespace: string option with get, set
        abstract jsxFactory: string option with get, set
        abstract jsxFragmentFactory: string option with get, set
        abstract jsxImportSource: string option with get, set
        abstract composite: bool option with get, set
        abstract incremental: bool option with get, set
        abstract tsBuildInfoFile: string option with get, set
        abstract removeComments: bool option with get, set
        abstract resolvePackageJsonExports: bool option with get, set
        abstract resolvePackageJsonImports: bool option with get, set
        abstract rewriteRelativeImportExtensions: bool option with get, set
        abstract rootDir: string option with get, set
        abstract rootDirs: ResizeArray<string> option with get, set
        abstract skipLibCheck: bool option with get, set
        abstract skipDefaultLibCheck: bool option with get, set
        abstract sourceMap: bool option with get, set
        abstract sourceRoot: string option with get, set
        abstract strict: bool option with get, set
        abstract strictFunctionTypes: bool option with get, set
        abstract strictBindCallApply: bool option with get, set
        abstract strictNullChecks: bool option with get, set
        abstract strictPropertyInitialization: bool option with get, set
        abstract strictBuiltinIteratorReturn: bool option with get, set
        abstract stripInternal: bool option with get, set
        [<Obsolete("")>]
        abstract suppressExcessPropertyErrors: bool option with get, set
        [<Obsolete("")>]
        abstract suppressImplicitAnyIndexErrors: bool option with get, set
        abstract suppressOutputPathCheck: bool option with get, set
        abstract target: ScriptTarget option with get, set
        abstract traceResolution: bool option with get, set
        abstract useUnknownInCatchVariables: bool option with get, set
        abstract noUncheckedSideEffectImports: bool option with get, set
        abstract resolveJsonModule: bool option with get, set
        abstract types: ResizeArray<string> option with get, set
        /// Paths used to compute primary types search locations
        abstract typeRoots: ResizeArray<string> option with get, set
        abstract verbatimModuleSyntax: bool option with get, set
        abstract erasableSyntaxOnly: bool option with get, set
        abstract version: bool option with get, set
        abstract watch: bool option with get, set
        abstract esModuleInterop: bool option with get, set
        abstract showConfig: bool option with get, set
        abstract useDefineForClassFields: bool option with get, set
        abstract tscBuild: bool option with get, set
        [<EmitIndexer>] abstract Item: option: string -> U2<CompilerOptionsValue, TsConfigSourceFile> option with get, set

    type [<AllowNullLiteral>] WatchOptions =
        abstract watchFile: WatchFileKind option with get, set
        abstract watchDirectory: WatchDirectoryKind option with get, set
        abstract fallbackPolling: PollingWatchKind option with get, set
        abstract synchronousWatchDirectory: bool option with get, set
        abstract excludeDirectories: ResizeArray<string> option with get, set
        abstract excludeFiles: ResizeArray<string> option with get, set
        [<EmitIndexer>] abstract Item: option: string -> CompilerOptionsValue option with get, set

    type [<AllowNullLiteral>] TypeAcquisition =
        abstract enable: bool option with get, set
        abstract ``include``: ResizeArray<string> option with get, set
        abstract exclude: ResizeArray<string> option with get, set
        abstract disableFilenameBasedTypeAcquisition: bool option with get, set
        [<EmitIndexer>] abstract Item: option: string -> CompilerOptionsValue option with get, set

    type [<RequireQualifiedAccess>] ModuleKind =
        | None = 0
        | CommonJS = 1
        | AMD = 2
        | UMD = 3
        | System = 4
        | ES2015 = 5
        | ES2020 = 6
        | ES2022 = 7
        | ESNext = 99
        | Node16 = 100
        | Node18 = 101
        | Node20 = 102
        | NodeNext = 199
        | Preserve = 200

    type [<RequireQualifiedAccess>] JsxEmit =
        | None = 0
        | Preserve = 1
        | React = 2
        | ReactNative = 3
        | ReactJSX = 4
        | ReactJSXDev = 5

    [<Obsolete("")>]
    type [<RequireQualifiedAccess>] ImportsNotUsedAsValues =
        | Remove = 0
        | Preserve = 1
        | Error = 2

    type [<RequireQualifiedAccess>] NewLineKind =
        | CarriageReturnLineFeed = 0
        | LineFeed = 1

    type [<AllowNullLiteral>] LineAndCharacter =
        /// 0-based.
        abstract line: float with get, set
        abstract character: float with get, set

    type [<RequireQualifiedAccess>] ScriptKind =
        | Unknown = 0
        | JS = 1
        | JSX = 2
        | TS = 3
        | TSX = 4
        | External = 5
        | JSON = 6
        /// Used on extensions that doesn't define the ScriptKind but the content defines it.
        /// Deferred extensions are going to be included in all project contexts.
        | Deferred = 7

    type [<RequireQualifiedAccess>] ScriptTarget =
        /// <deprecated />
        | ES3 = 0
        | ES5 = 1
        | ES2015 = 2
        | ES2016 = 3
        | ES2017 = 4
        | ES2018 = 5
        | ES2019 = 6
        | ES2020 = 7
        | ES2021 = 8
        | ES2022 = 9
        | ES2023 = 10
        | ES2024 = 11
        | ESNext = 99
        | JSON = 100
        | Latest = 99

    type [<RequireQualifiedAccess>] LanguageVariant =
        | Standard = 0
        | JSX = 1

    /// Either a parsed command line or a parsed tsconfig.json
    type [<AllowNullLiteral>] ParsedCommandLine =
        abstract options: CompilerOptions with get, set
        abstract typeAcquisition: TypeAcquisition option with get, set
        abstract fileNames: ResizeArray<string> with get, set
        abstract projectReferences: ResizeArray<ProjectReference> option with get, set
        abstract watchOptions: WatchOptions option with get, set
        abstract raw: obj option with get, set
        abstract errors: ResizeArray<Diagnostic> with get, set
        abstract wildcardDirectories: MapLike<WatchDirectoryFlags> option with get, set
        abstract compileOnSave: bool option with get, set

    type [<RequireQualifiedAccess>] WatchDirectoryFlags =
        | None = 0
        | Recursive = 1

    type [<AllowNullLiteral>] ConfigFileSpecs =
        abstract filesSpecs: ResizeArray<string> option with get, set
        /// Present to report errors (user specified specs), validatedIncludeSpecs are used for file name matching
        abstract includeSpecs: ResizeArray<string> option with get, set
        /// Present to report errors (user specified specs), validatedExcludeSpecs are used for file name matching
        abstract excludeSpecs: ResizeArray<string> option with get, set
        abstract validatedFilesSpec: ResizeArray<string> option with get, set
        abstract validatedIncludeSpecs: ResizeArray<string> option with get, set
        abstract validatedExcludeSpecs: ResizeArray<string> option with get, set
        abstract validatedFilesSpecBeforeSubstitution: ResizeArray<string> option with get, set
        abstract validatedIncludeSpecsBeforeSubstitution: ResizeArray<string> option with get, set
        abstract validatedExcludeSpecsBeforeSubstitution: ResizeArray<string> option with get, set
        abstract isDefaultIncludeSpec: bool with get, set

    type ModuleImportResult =
        ModuleImportResult<obj>

    type ModuleImportResult<'T> =
        U2<{| ``module``: 'T; modulePath: string option; error: obj |}, {| ``module``: obj; modulePath: obj option; error: {| stack: string option; message: string option |} |}>

    type [<AllowNullLiteral>] CreateProgramOptions =
        abstract rootNames: ResizeArray<string> with get, set
        abstract options: CompilerOptions with get, set
        abstract projectReferences: ResizeArray<ProjectReference> option with get, set
        abstract host: CompilerHost option with get, set
        abstract oldProgram: Program option with get, set
        abstract configFileParsingDiagnostics: ResizeArray<Diagnostic> option with get, set
        abstract typeScriptVersion: string option with get, set

    type [<AllowNullLiteral>] CommandLineOptionBase =
        abstract name: string with get, set
        abstract ``type``: U2<Map<string, U2<float, string>>, string> with get, set
        abstract isFilePath: bool option with get, set
        abstract shortName: string option with get, set
        abstract description: DiagnosticMessage option with get, set
        abstract defaultValueDescription: U4<string, float, bool, DiagnosticMessage> option with get, set
        abstract paramType: DiagnosticMessage option with get, set
        abstract isTSConfigOnly: bool option with get, set
        abstract isCommandLineOnly: bool option with get, set
        abstract showInSimplifiedHelpView: bool option with get, set
        abstract category: DiagnosticMessage option with get, set
        abstract strictFlag: bool option with get, set
        abstract allowJsFlag: bool option with get, set
        abstract affectsSourceFile: bool option with get, set
        abstract affectsModuleResolution: bool option with get, set
        abstract affectsBindDiagnostics: bool option with get, set
        abstract affectsSemanticDiagnostics: bool option with get, set
        abstract affectsEmit: bool option with get, set
        abstract affectsProgramStructure: bool option with get, set
        abstract affectsDeclarationPath: bool option with get, set
        abstract affectsBuildInfo: bool option with get, set
        abstract transpileOptionValue: bool option with get, set
        abstract extraValidation: (CompilerOptionsValue -> DiagnosticMessage * obj option) option with get, set
        abstract disallowNullOrUndefined: bool option with get, set
        abstract allowConfigDirTemplateSubstitution: bool option with get, set

    type [<AllowNullLiteral>] CommandLineOptionOfStringType =
        inherit CommandLineOptionBase
        abstract ``type``: string with get, set
        abstract defaultValueDescription: U2<string, DiagnosticMessage> option with get, set

    type [<AllowNullLiteral>] CommandLineOptionOfNumberType =
        inherit CommandLineOptionBase
        abstract ``type``: string with get, set
        abstract defaultValueDescription: U2<float, DiagnosticMessage> option with get, set

    type [<AllowNullLiteral>] CommandLineOptionOfBooleanType =
        inherit CommandLineOptionBase
        abstract ``type``: string with get, set
        abstract defaultValueDescription: U2<bool, DiagnosticMessage> option with get, set

    type [<AllowNullLiteral>] CommandLineOptionOfCustomType =
        inherit CommandLineOptionBase
        abstract ``type``: Map<string, U2<float, string>> with get, set
        abstract defaultValueDescription: U3<float, string, DiagnosticMessage> option with get, set
        abstract deprecatedKeys: Set<string> option with get, set

    type [<AllowNullLiteral>] AlternateModeDiagnostics =
        abstract diagnostic: DiagnosticMessage with get, set
        abstract getOptionsNameMap: (unit -> OptionsNameMap) with get, set

    type [<AllowNullLiteral>] DidYouMeanOptionsDiagnostics =
        abstract alternateMode: AlternateModeDiagnostics option with get, set
        abstract optionDeclarations: ResizeArray<CommandLineOption> with get, set
        abstract unknownOptionDiagnostic: DiagnosticMessage with get, set
        abstract unknownDidYouMeanDiagnostic: DiagnosticMessage with get, set

    type [<AllowNullLiteral>] TsConfigOnlyOption =
        inherit CommandLineOptionBase
        abstract ``type``: string with get, set
        abstract elementOptions: Map<string, CommandLineOption> option with get, set
        abstract extraKeyDiagnostics: DidYouMeanOptionsDiagnostics option with get, set

    type [<AllowNullLiteral>] CommandLineOptionOfListType =
        inherit CommandLineOptionBase
        abstract ``type``: CommandLineOptionOfListTypeType with get, set
        abstract element: U5<CommandLineOptionOfCustomType, CommandLineOptionOfStringType, CommandLineOptionOfNumberType, CommandLineOptionOfBooleanType, TsConfigOnlyOption> with get, set
        abstract listPreserveFalsyValues: bool option with get, set

    type CommandLineOption =
        U6<CommandLineOptionOfCustomType, CommandLineOptionOfStringType, CommandLineOptionOfNumberType, CommandLineOptionOfBooleanType, TsConfigOnlyOption, CommandLineOptionOfListType>

    type [<RequireQualifiedAccess>] CharacterCodes =
        | EOF = -1
        | NullCharacter = 0
        | MaxAsciiCharacter = 127
        | LineFeed = 10
        | CarriageReturn = 13
        | LineSeparator = 8232
        | ParagraphSeparator = 8233
        | NextLine = 133
        | Space = 32
        | NonBreakingSpace = 160
        | EnQuad = 8192
        | EmQuad = 8193
        | EnSpace = 8194
        | EmSpace = 8195
        | ThreePerEmSpace = 8196
        | FourPerEmSpace = 8197
        | SixPerEmSpace = 8198
        | FigureSpace = 8199
        | PunctuationSpace = 8200
        | ThinSpace = 8201
        | HairSpace = 8202
        | ZeroWidthSpace = 8203
        | NarrowNoBreakSpace = 8239
        | IdeographicSpace = 12288
        | MathematicalSpace = 8287
        | Ogham = 5760
        | ReplacementCharacter = 65533
        | ``_`` = 95
        | ``$`` = 36
        | _0 = 48
        | _1 = 49
        | _2 = 50
        | _3 = 51
        | _4 = 52
        | _5 = 53
        | _6 = 54
        | _7 = 55
        | _8 = 56
        | _9 = 57
        | A = 97
        | B = 98
        | C = 99
        | D = 100
        | E = 101
        | F = 102
        | G = 103
        | H = 104
        | I = 105
        | J = 106
        | K = 107
        | L = 108
        | M = 109
        | N = 110
        | O = 111
        | P = 112
        | Q = 113
        | R = 114
        | S = 115
        | T = 116
        | U = 117
        | V = 118
        | W = 119
        | X = 120
        | Y = 121
        | Z = 122
        | A = 65
        | B = 66
        | C = 67
        | D = 68
        | E = 69
        | F = 70
        | G = 71
        | H = 72
        | I = 73
        | J = 74
        | K = 75
        | L = 76
        | M = 77
        | N = 78
        | O = 79
        | P = 80
        | Q = 81
        | R = 82
        | S = 83
        | T = 84
        | U = 85
        | V = 86
        | W = 87
        | X = 88
        | Y = 89
        | Z = 90
        | Ampersand = 38
        | Asterisk = 42
        | At = 64
        | Backslash = 92
        | Backtick = 96
        | Bar = 124
        | Caret = 94
        | CloseBrace = 125
        | CloseBracket = 93
        | CloseParen = 41
        | Colon = 58
        | Comma = 44
        | Dot = 46
        | DoubleQuote = 34
        | Equals = 61
        | Exclamation = 33
        | GreaterThan = 62
        | Hash = 35
        | LessThan = 60
        | Minus = 45
        | OpenBrace = 123
        | OpenBracket = 91
        | OpenParen = 40
        | Percent = 37
        | Plus = 43
        | Question = 63
        | Semicolon = 59
        | SingleQuote = 39
        | Slash = 47
        | Tilde = 126
        | Backspace = 8
        | FormFeed = 12
        | ByteOrderMark = 65279
        | Tab = 9
        | VerticalTab = 11

    type [<AllowNullLiteral>] ModuleResolutionHost =
        abstract fileExists: fileName: string -> bool
        abstract readFile: fileName: string -> string option
        abstract trace: s: string -> unit
        abstract directoryExists: directoryName: string -> bool
        /// <summary>Resolve a symbolic link.</summary>
        /// <seealso href="https://nodejs.org/api/fs.html#fs_fs_realpathsync_path_options" />
        abstract realpath: path: string -> string
        abstract getCurrentDirectory: unit -> string
        abstract getDirectories: path: string -> ResizeArray<string>
        abstract useCaseSensitiveFileNames: U2<bool, (unit -> bool)> option with get, set
        abstract getGlobalTypingsCacheLocation: unit -> string option

    /// Used by services to specify the minimum host area required to set up source files under any compilation settings
    type [<AllowNullLiteral>] MinimalResolutionCacheHost =
        inherit ModuleResolutionHost
        abstract getCompilationSettings: unit -> CompilerOptions
        abstract getCompilerHost: unit -> CompilerHost option

    /// <summary>
    /// Represents the result of module resolution.
    /// Module resolution will pick up tsx/jsx/js files even if '--jsx' and '--allowJs' are turned off.
    /// The Program will then filter results based on these flags.
    ///
    /// Prefer to return a <c>ResolvedModuleFull</c> so that the file type does not have to be inferred.
    /// </summary>
    type [<AllowNullLiteral>] ResolvedModule =
        /// Path of the file the module was resolved to.
        abstract resolvedFileName: string with get, set
        /// <summary>True if <c>resolvedFileName</c> comes from <c>node_modules</c>.</summary>
        abstract isExternalLibraryImport: bool option with get, set
        /// True if the original module reference used a .ts extension to refer directly to a .ts file,
        /// which should produce an error during checking if emit is enabled.
        abstract resolvedUsingTsExtension: bool option with get, set

    /// <summary>
    /// ResolvedModule with an explicitly provided <c>extension</c> property.
    /// Prefer this over <c>ResolvedModule</c>.
    /// If changing this, remember to change <c>moduleResolutionIsEqualTo</c>.
    /// </summary>
    type [<AllowNullLiteral>] ResolvedModuleFull =
        inherit ResolvedModule
        abstract originalPath: string option
        /// Extension of resolvedFileName. This must match what's at the end of resolvedFileName.
        /// This is optional for backwards-compatibility, but will be added if not provided.
        abstract extension: string with get, set
        abstract packageId: PackageId option with get, set

    /// <summary>
    /// Unique identifier with a package name and version.
    /// If changing this, remember to change <c>packageIdIsEqual</c>.
    /// </summary>
    type [<AllowNullLiteral>] PackageId =
        /// <summary>
        /// Name of the package.
        /// Should not include <c>@types</c>.
        /// If accessing a non-index file, this should include its name e.g. "foo/bar".
        /// </summary>
        abstract name: string with get, set
        /// Name of a submodule within this package.
        /// May be "".
        abstract subModuleName: string with get, set
        /// Version of the package, e.g. "1.2.3"
        abstract version: string with get, set
        abstract peerDependencies: string option with get, set

    type [<StringEnum>] [<RequireQualifiedAccess>] Extension =
        | [<CompiledName(".ts")>] Ts
        | [<CompiledName(".tsx")>] Tsx
        | [<CompiledName(".d.ts")>] Dts
        | [<CompiledName(".js")>] Js
        | [<CompiledName(".jsx")>] Jsx
        | [<CompiledName(".json")>] Json
        | [<CompiledName(".tsbuildinfo")>] TsBuildInfo
        | [<CompiledName(".mjs")>] Mjs
        | [<CompiledName(".mts")>] Mts
        | [<CompiledName(".d.mts")>] Dmts
        | [<CompiledName(".cjs")>] Cjs
        | [<CompiledName(".cts")>] Cts
        | [<CompiledName(".d.cts")>] Dcts

    type [<AllowNullLiteral>] ResolvedModuleWithFailedLookupLocations =
        abstract resolvedModule: ResolvedModuleFull option
        abstract failedLookupLocations: ResizeArray<string> option with get, set
        abstract affectingLocations: ResizeArray<string> option with get, set
        abstract resolutionDiagnostics: ResizeArray<Diagnostic> option with get, set
        abstract alternateResult: string option with get, set

    type [<AllowNullLiteral>] ResolvedTypeReferenceDirective =
        abstract primary: bool with get, set
        abstract resolvedFileName: string option with get, set
        abstract originalPath: string option with get, set
        abstract packageId: PackageId option with get, set
        /// <summary>True if <c>resolvedFileName</c> comes from <c>node_modules</c>.</summary>
        abstract isExternalLibraryImport: bool option with get, set

    type [<AllowNullLiteral>] ResolvedTypeReferenceDirectiveWithFailedLookupLocations =
        abstract resolvedTypeReferenceDirective: ResolvedTypeReferenceDirective option
        abstract failedLookupLocations: ResizeArray<string> option with get, set
        abstract affectingLocations: ResizeArray<string> option with get, set
        abstract resolutionDiagnostics: ResizeArray<Diagnostic> option with get, set

    type [<AllowNullLiteral>] HasInvalidatedResolutions =
        [<Emit("$0($1...)")>] abstract Invoke: sourceFile: Path -> bool

    type [<AllowNullLiteral>] HasInvalidatedLibResolutions =
        [<Emit("$0($1...)")>] abstract Invoke: libFileName: string -> bool

    type [<AllowNullLiteral>] HasChangedAutomaticTypeDirectiveNames =
        [<Emit("$0($1...)")>] abstract Invoke: unit -> bool

    type [<AllowNullLiteral>] CompilerHost =
        inherit ModuleResolutionHost
        abstract getSourceFile: fileName: string * languageVersionOrOptions: U2<ScriptTarget, CreateSourceFileOptions> * ?onError: (string -> unit) * ?shouldCreateNewSourceFile: bool -> SourceFile option
        abstract getSourceFileByPath: fileName: string * path: Path * languageVersionOrOptions: U2<ScriptTarget, CreateSourceFileOptions> * ?onError: (string -> unit) * ?shouldCreateNewSourceFile: bool -> SourceFile option
        abstract getCancellationToken: unit -> CancellationToken
        abstract getDefaultLibFileName: options: CompilerOptions -> string
        abstract getDefaultLibLocation: unit -> string
        abstract writeFile: WriteFileCallback with get, set
        abstract getCurrentDirectory: unit -> string
        abstract getCanonicalFileName: fileName: string -> string
        abstract useCaseSensitiveFileNames: unit -> bool
        abstract getNewLine: unit -> string
        abstract readDirectory: rootDir: string * extensions: ResizeArray<string> * excludes: ResizeArray<string> option * includes: ResizeArray<string> * ?depth: float -> ResizeArray<string>
        [<Obsolete("supply resolveModuleNameLiterals instead for resolution that can handle newer resolution modes like nodenext")>]
        abstract resolveModuleNames: moduleNames: ResizeArray<string> * containingFile: string * reusedNames: ResizeArray<string> option * redirectedReference: ResolvedProjectReference option * options: CompilerOptions * ?containingSourceFile: SourceFile -> ResizeArray<ResolvedModule option>
        /// <summary>Returns the module resolution cache used by a provided <c>resolveModuleNames</c> implementation so that any non-name module resolution operations (eg, package.json lookup) can reuse it</summary>
        abstract getModuleResolutionCache: unit -> ModuleResolutionCache option
        [<Obsolete("supply resolveTypeReferenceDirectiveReferences instead for resolution that can handle newer resolution modes like nodenext

    This method is a companion for 'resolveModuleNames' and is used to resolve 'types' references to actual type declaration files")>]
        abstract resolveTypeReferenceDirectives: typeReferenceDirectiveNames: U2<ResizeArray<string>, ResizeArray<FileReference>> * containingFile: string * redirectedReference: ResolvedProjectReference option * options: CompilerOptions * ?containingFileMode: ResolutionMode -> ResizeArray<ResolvedTypeReferenceDirective option>
        abstract resolveModuleNameLiterals: moduleLiterals: ResizeArray<StringLiteralLike> * containingFile: string * redirectedReference: ResolvedProjectReference option * options: CompilerOptions * containingSourceFile: SourceFile * reusedNames: ResizeArray<StringLiteralLike> option -> ResizeArray<ResolvedModuleWithFailedLookupLocations>
        abstract resolveTypeReferenceDirectiveReferences: typeDirectiveReferences: ResizeArray<'T> * containingFile: string * redirectedReference: ResolvedProjectReference option * options: CompilerOptions * containingSourceFile: SourceFile option * reusedNames: ResizeArray<'T> option -> ResizeArray<ResolvedTypeReferenceDirectiveWithFailedLookupLocations>
        abstract resolveLibrary: libraryName: string * resolveFrom: string * options: CompilerOptions * libFileName: string -> ResolvedModuleWithFailedLookupLocations
        /// <summary>If provided along with custom resolveLibrary, used to determine if we should redo library resolutions</summary>
        abstract hasInvalidatedLibResolutions: libFileName: string -> bool
        abstract getEnvironmentVariable: name: string -> string option
        abstract onReleaseOldSourceFile: oldSourceFile: SourceFile * oldOptions: CompilerOptions * hasSourceFileByPath: bool * newSourceFileByResolvedPath: SourceFile option -> unit
        abstract onReleaseParsedCommandLine: configFileName: string * oldResolvedRef: ResolvedProjectReference option * optionOptions: CompilerOptions -> unit
        /// If provided along with custom resolveModuleNames or resolveTypeReferenceDirectives, used to determine if unchanged file path needs to re-resolve modules/type reference directives
        abstract hasInvalidatedResolutions: filePath: Path -> bool
        abstract hasChangedAutomaticTypeDirectiveNames: HasChangedAutomaticTypeDirectiveNames option with get, set
        abstract createHash: data: string -> string
        abstract getParsedCommandLine: fileName: string -> ParsedCommandLine option
        abstract useSourceOfProjectReferenceRedirect: unit -> bool
        abstract createDirectory: directory: string -> unit
        abstract getSymlinkCache: unit -> SymlinkCache
        abstract storeSignatureInfo: bool option with get, set
        abstract getBuildInfo: fileName: string * configFilePath: string option -> BuildInfo option
        abstract jsDocParsingMode: JSDocParsingMode option with get, set

    type [<RequireQualifiedAccess>] TransformFlags =
        | None = 0
        | ContainsTypeScript = 1
        | ContainsJsx = 2
        | ContainsESNext = 4
        | ContainsES2022 = 8
        | ContainsES2021 = 16
        | ContainsES2020 = 32
        | ContainsES2019 = 64
        | ContainsES2018 = 128
        | ContainsES2017 = 256
        | ContainsES2016 = 512
        | ContainsES2015 = 1024
        | ContainsGenerator = 2048
        | ContainsDestructuringAssignment = 4096
        | ContainsTypeScriptClassSyntax = 8192
        | ContainsLexicalThis = 16384
        | ContainsRestOrSpread = 32768
        | ContainsObjectRestOrSpread = 65536
        | ContainsComputedPropertyName = 131072
        | ContainsBlockScopedBinding = 262144
        | ContainsBindingPattern = 524288
        | ContainsYield = 1048576
        | ContainsAwait = 2097152
        | ContainsHoistedDeclarationOrCompletion = 4194304
        | ContainsDynamicImport = 8388608
        | ContainsClassFields = 16777216
        | ContainsDecorators = 33554432
        | ContainsPossibleTopLevelAwait = 67108864
        | ContainsLexicalSuper = 134217728
        | ContainsUpdateExpressionForIdentifier = 268435456
        | ContainsPrivateIdentifierInExpression = 536870912
        | HasComputedFlags = -2147483648
        | AssertTypeScript = 1
        | AssertJsx = 2
        | AssertESNext = 4
        | AssertES2022 = 8
        | AssertES2021 = 16
        | AssertES2020 = 32
        | AssertES2019 = 64
        | AssertES2018 = 128
        | AssertES2017 = 256
        | AssertES2016 = 512
        | AssertES2015 = 1024
        | AssertGenerator = 2048
        | AssertDestructuringAssignment = 4096
        | OuterExpressionExcludes = -2147483648
        | PropertyAccessExcludes = -2147483648
        | NodeExcludes = -2147483648
        | ArrowFunctionExcludes = -2072174592
        | FunctionExcludes = -1937940480
        | ConstructorExcludes = -1937948672
        | MethodOrAccessorExcludes = -2005057536
        | PropertyExcludes = -2013249536
        | ClassExcludes = -2147344384
        | ModuleExcludes = -1941676032
        | TypeExcludes = -2
        | ObjectLiteralExcludes = -2147278848
        | ArrayLiteralOrCallOrNewExcludes = -2147450880
        | VariableDeclarationListExcludes = -2146893824
        | ParameterExcludes = -2147483648
        | CatchClauseExcludes = -2147418112
        | BindingPatternExcludes = -2147450880
        | ContainsLexicalThisOrSuper = 134234112
        | PropertyNamePropagatingFlags = 134234112

    type [<AllowNullLiteral>] SourceMapRange =
        inherit TextRange
        abstract source: SourceMapSource option with get, set

    type [<AllowNullLiteral>] SourceMapSource =
        abstract fileName: string with get, set
        abstract text: string with get, set
        abstract lineMap: ResizeArray<float> with get, set
        abstract skipTrivia: (float -> float) option with get, set

    type [<AllowNullLiteral>] EmitNode =
        abstract flags: EmitFlags with get, set
        abstract internalFlags: InternalEmitFlags with get, set
        abstract annotatedNodes: ResizeArray<Node> option with get, set
        abstract leadingComments: ResizeArray<SynthesizedComment> option with get, set
        abstract trailingComments: ResizeArray<SynthesizedComment> option with get, set
        abstract commentRange: TextRange option with get, set
        abstract sourceMapRange: SourceMapRange option with get, set
        abstract tokenSourceMapRanges: ResizeArray<SourceMapRange option> option with get, set
        abstract constantValue: U2<string, float> option with get, set
        abstract externalHelpersModuleName: Identifier option with get, set
        abstract externalHelpers: bool option with get, set
        abstract helpers: ResizeArray<EmitHelper> option with get, set
        abstract startsOnNewLine: bool option with get, set
        abstract snippetElement: SnippetElement option with get, set
        abstract typeNode: TypeNode option with get, set
        abstract classThis: Identifier option with get, set
        abstract assignedName: Expression option with get, set
        abstract identifierTypeArguments: ResizeArray<U2<TypeNode, TypeParameterDeclaration>> option with get, set
        abstract autoGenerate: AutoGenerateInfo option with get, set
        abstract generatedImportReference: ImportSpecifier option with get, set

    type SnippetElement =
        U2<TabStop, Placeholder>

    type [<AllowNullLiteral>] TabStop =
        abstract kind: SnippetKind with get, set
        abstract order: float with get, set

    type [<AllowNullLiteral>] Placeholder =
        abstract kind: SnippetKind with get, set
        abstract order: float with get, set

    type [<RequireQualifiedAccess>] SnippetKind =
        | TabStop = 0
        | Placeholder = 1
        | Choice = 2
        | Variable = 3

    type [<RequireQualifiedAccess>] EmitFlags =
        | None = 0
        | SingleLine = 1
        | MultiLine = 2
        | AdviseOnEmitNode = 4
        | NoSubstitution = 8
        | CapturesThis = 16
        | NoLeadingSourceMap = 32
        | NoTrailingSourceMap = 64
        | NoSourceMap = 96
        | NoNestedSourceMaps = 128
        | NoTokenLeadingSourceMaps = 256
        | NoTokenTrailingSourceMaps = 512
        | NoTokenSourceMaps = 768
        | NoLeadingComments = 1024
        | NoTrailingComments = 2048
        | NoComments = 3072
        | NoNestedComments = 4096
        | HelperName = 8192
        | ExportName = 16384
        | LocalName = 32768
        | InternalName = 65536
        | Indented = 131072
        | NoIndentation = 262144
        | AsyncFunctionBody = 524288
        | ReuseTempVariableScope = 1048576
        | CustomPrologue = 2097152
        | NoHoisting = 4194304
        | Iterator = 8388608
        | NoAsciiEscaping = 16777216

    type [<RequireQualifiedAccess>] InternalEmitFlags =
        | None = 0
        | TypeScriptClassWrapper = 1
        | NeverApplyImportHelper = 2
        | IgnoreSourceNewlines = 4
        | Immutable = 8
        | IndirectCall = 16
        | TransformPrivateStaticElements = 32

    type [<AllowNullLiteral>] EmitHelperBase =
        abstract name: string
        abstract scoped: bool
        abstract text: U2<string, (EmitHelperUniqueNameCallback -> string)>
        abstract priority: float option
        abstract dependencies: ResizeArray<EmitHelper> option

    type [<AllowNullLiteral>] ScopedEmitHelper =
        inherit EmitHelperBase
        abstract scoped: bool

    type [<AllowNullLiteral>] UnscopedEmitHelper =
        inherit EmitHelperBase
        abstract scoped: bool
        abstract importName: string option
        abstract text: string

    type EmitHelper =
        U2<ScopedEmitHelper, UnscopedEmitHelper>

    type [<AllowNullLiteral>] EmitHelperUniqueNameCallback =
        [<Emit("$0($1...)")>] abstract Invoke: name: string -> string

    type [<StringEnum>] [<RequireQualifiedAccess>] LanugageFeatures =
        | [<CompiledName("Classes")>] Classes
        | [<CompiledName("ForOf")>] ForOf
        | [<CompiledName("Generators")>] Generators
        | [<CompiledName("Iteration")>] Iteration
        | [<CompiledName("SpreadElements")>] SpreadElements
        | [<CompiledName("RestElements")>] RestElements
        | [<CompiledName("TaggedTemplates")>] TaggedTemplates
        | [<CompiledName("DestructuringAssignment")>] DestructuringAssignment
        | [<CompiledName("BindingPatterns")>] BindingPatterns
        | [<CompiledName("ArrowFunctions")>] ArrowFunctions
        | [<CompiledName("BlockScopedVariables")>] BlockScopedVariables
        | [<CompiledName("ObjectAssign")>] ObjectAssign
        | [<CompiledName("RegularExpressionFlagsUnicode")>] RegularExpressionFlagsUnicode
        | [<CompiledName("RegularExpressionFlagsSticky")>] RegularExpressionFlagsSticky
        | [<CompiledName("Exponentiation")>] Exponentiation
        | [<CompiledName("AsyncFunctions")>] AsyncFunctions
        | [<CompiledName("ForAwaitOf")>] ForAwaitOf
        | [<CompiledName("AsyncGenerators")>] AsyncGenerators
        | [<CompiledName("AsyncIteration")>] AsyncIteration
        | [<CompiledName("ObjectSpreadRest")>] ObjectSpreadRest
        | [<CompiledName("RegularExpressionFlagsDotAll")>] RegularExpressionFlagsDotAll
        | [<CompiledName("BindinglessCatch")>] BindinglessCatch
        | [<CompiledName("BigInt")>] BigInt
        | [<CompiledName("NullishCoalesce")>] NullishCoalesce
        | [<CompiledName("OptionalChaining")>] OptionalChaining
        | [<CompiledName("LogicalAssignment")>] LogicalAssignment
        | [<CompiledName("TopLevelAwait")>] TopLevelAwait
        | [<CompiledName("ClassFields")>] ClassFields
        | [<CompiledName("PrivateNamesAndClassStaticBlocks")>] PrivateNamesAndClassStaticBlocks
        | [<CompiledName("RegularExpressionFlagsHasIndices")>] RegularExpressionFlagsHasIndices
        | [<CompiledName("ShebangComments")>] ShebangComments
        | [<CompiledName("RegularExpressionFlagsUnicodeSets")>] RegularExpressionFlagsUnicodeSets
        | [<CompiledName("UsingAndAwaitUsing")>] UsingAndAwaitUsing
        | [<CompiledName("ClassAndClassElementDecorators")>] ClassAndClassElementDecorators

    /// <summary>
    /// Used by the checker, this enum keeps track of external emit helpers that should be type
    /// checked.
    /// </summary>
    type [<RequireQualifiedAccess>] ExternalEmitHelpers =
        | Extends = 1
        | Assign = 2
        | Rest = 4
        | Decorate = 8
        | ESDecorateAndRunInitializers = 8
        | Metadata = 16
        | Param = 32
        | Awaiter = 64
        | Generator = 128
        | Values = 256
        | Read = 512
        | SpreadArray = 1024
        | Await = 2048
        | AsyncGenerator = 4096
        | AsyncDelegator = 8192
        | AsyncValues = 16384
        | ExportStar = 32768
        | ImportStar = 65536
        | ImportDefault = 131072
        | MakeTemplateObject = 262144
        | ClassPrivateFieldGet = 524288
        | ClassPrivateFieldSet = 1048576
        | ClassPrivateFieldIn = 2097152
        | SetFunctionName = 4194304
        | PropKey = 8388608
        | AddDisposableResourceAndDisposeResources = 16777216
        | RewriteRelativeImportExtension = 33554432
        | FirstEmitHelper = 1
        | LastEmitHelper = 16777216
        | ForOfIncludes = 256
        | ForAwaitOfIncludes = 16384
        | AsyncGeneratorIncludes = 6144
        | AsyncDelegatorIncludes = 26624
        | SpreadIncludes = 1536

    type [<RequireQualifiedAccess>] EmitHint =
        | SourceFile = 0
        | Expression = 1
        | IdentifierName = 2
        | MappedTypeParameter = 3
        | Unspecified = 4
        | EmbeddedStatement = 5
        | JsxAttributeValue = 6
        | ImportTypeNodeAttributes = 7

    type [<AllowNullLiteral>] SourceFileMayBeEmittedHost =
        abstract getCompilerOptions: unit -> CompilerOptions
        abstract isSourceFileFromExternalLibrary: file: SourceFile -> bool
        abstract getRedirectFromSourceFile: fileName: string -> ResolvedRefAndOutputDts option
        abstract isSourceOfProjectReferenceRedirect: fileName: string -> bool
        abstract getCurrentDirectory: unit -> string
        abstract getCanonicalFileName: GetCanonicalFileName with get, set
        abstract useCaseSensitiveFileNames: unit -> bool

    type [<AllowNullLiteral>] EmitHost =
        inherit ScriptReferenceHost
        inherit ModuleSpecifierResolutionHost
        inherit SourceFileMayBeEmittedHost
        abstract getSourceFiles: unit -> ResizeArray<SourceFile>
        abstract useCaseSensitiveFileNames: unit -> bool
        abstract getCurrentDirectory: unit -> string
        abstract getCommonSourceDirectory: unit -> string
        abstract getCanonicalFileName: fileName: string -> string
        abstract isEmitBlocked: emitFileName: string -> bool
        abstract shouldTransformImportCall: sourceFile: SourceFile -> bool
        abstract getEmitModuleFormatOfFile: sourceFile: SourceFile -> ModuleKind
        abstract writeFile: WriteFileCallback with get, set
        abstract getBuildInfo: unit -> BuildInfo option
        abstract getSourceFileFromReference: obj with get, set
        abstract redirectTargetsMap: RedirectTargetsMap
        abstract createHash: data: string -> string

    type [<AllowNullLiteral>] PropertyDescriptorAttributes =
        abstract enumerable: U2<bool, Expression> option with get, set
        abstract configurable: U2<bool, Expression> option with get, set
        abstract writable: U2<bool, Expression> option with get, set
        abstract value: Expression option with get, set
        abstract get: Expression option with get, set
        abstract set: Expression option with get, set

    type [<RequireQualifiedAccess>] OuterExpressionKinds =
        | Parentheses = 1
        | TypeAssertions = 2
        | NonNullAssertions = 4
        | PartiallyEmittedExpressions = 8
        | ExpressionsWithTypeArguments = 16
        | Satisfies = 32
        | Assertions = 38
        | All = 63
        | ExcludeJSDocTypeAssertion = -2147483648

    type OuterExpression =
        U7<ParenthesizedExpression, TypeAssertion, SatisfiesExpression, AsExpression, NonNullExpression, ExpressionWithTypeArguments, PartiallyEmittedExpression>

    type WrappedExpression<'T when 'T :> Expression> =
        U2<obj, 'T>

    type [<StringEnum>] [<RequireQualifiedAccess>] TypeOfTag =
        | Null
        | Undefined
        | Number
        | Bigint
        | Boolean
        | String
        | Symbol
        | Object
        | Function

    type [<AllowNullLiteral>] CallBinding =
        abstract target: LeftHandSideExpression with get, set
        abstract thisArg: Expression with get, set

    type [<AllowNullLiteral>] ParenthesizerRules =
        abstract getParenthesizeLeftSideOfBinaryForOperator: binaryOperator: SyntaxKind -> (Expression -> Expression)
        abstract getParenthesizeRightSideOfBinaryForOperator: binaryOperator: SyntaxKind -> (Expression -> Expression)
        abstract parenthesizeLeftSideOfBinary: binaryOperator: SyntaxKind * leftSide: Expression -> Expression
        abstract parenthesizeRightSideOfBinary: binaryOperator: SyntaxKind * leftSide: Expression option * rightSide: Expression -> Expression
        abstract parenthesizeExpressionOfComputedPropertyName: expression: Expression -> Expression
        abstract parenthesizeConditionOfConditionalExpression: condition: Expression -> Expression
        abstract parenthesizeBranchOfConditionalExpression: branch: Expression -> Expression
        abstract parenthesizeExpressionOfExportDefault: expression: Expression -> Expression
        abstract parenthesizeExpressionOfNew: expression: Expression -> LeftHandSideExpression
        abstract parenthesizeLeftSideOfAccess: expression: Expression * ?optionalChain: bool -> LeftHandSideExpression
        abstract parenthesizeOperandOfPostfixUnary: operand: Expression -> LeftHandSideExpression
        abstract parenthesizeOperandOfPrefixUnary: operand: Expression -> UnaryExpression
        abstract parenthesizeExpressionsOfCommaDelimitedList: elements: ResizeArray<Expression> -> ResizeArray<Expression>
        abstract parenthesizeExpressionForDisallowedComma: expression: Expression -> Expression
        abstract parenthesizeExpressionOfExpressionStatement: expression: Expression -> Expression
        abstract parenthesizeConciseBodyOfArrowFunction: body: Expression -> Expression
        abstract parenthesizeConciseBodyOfArrowFunction: body: ConciseBody -> ConciseBody
        abstract parenthesizeCheckTypeOfConditionalType: ``type``: TypeNode -> TypeNode
        abstract parenthesizeExtendsTypeOfConditionalType: ``type``: TypeNode -> TypeNode
        abstract parenthesizeOperandOfTypeOperator: ``type``: TypeNode -> TypeNode
        abstract parenthesizeOperandOfReadonlyTypeOperator: ``type``: TypeNode -> TypeNode
        abstract parenthesizeNonArrayTypeOfPostfixType: ``type``: TypeNode -> TypeNode
        abstract parenthesizeElementTypesOfTupleType: types: ResizeArray<U2<TypeNode, NamedTupleMember>> -> ResizeArray<TypeNode>
        abstract parenthesizeElementTypeOfTupleType: ``type``: U2<TypeNode, NamedTupleMember> -> U2<TypeNode, NamedTupleMember>
        abstract parenthesizeTypeOfOptionalType: ``type``: TypeNode -> TypeNode
        abstract parenthesizeConstituentTypeOfUnionType: ``type``: TypeNode -> TypeNode
        abstract parenthesizeConstituentTypesOfUnionType: constituents: ResizeArray<TypeNode> -> ResizeArray<TypeNode>
        abstract parenthesizeConstituentTypeOfIntersectionType: ``type``: TypeNode -> TypeNode
        abstract parenthesizeConstituentTypesOfIntersectionType: constituents: ResizeArray<TypeNode> -> ResizeArray<TypeNode>
        abstract parenthesizeLeadingTypeArgument: typeNode: TypeNode -> TypeNode
        abstract parenthesizeTypeArguments: typeParameters: ResizeArray<TypeNode> option -> ResizeArray<TypeNode> option

    type [<AllowNullLiteral>] NodeConverters =
        abstract convertToFunctionBlock: node: ConciseBody * ?multiLine: bool -> Block
        abstract convertToFunctionExpression: node: FunctionDeclaration -> FunctionExpression
        abstract convertToClassExpression: node: ClassDeclaration -> ClassExpression
        abstract convertToArrayAssignmentElement: element: ArrayBindingOrAssignmentElement -> Expression
        abstract convertToObjectAssignmentElement: element: ObjectBindingOrAssignmentElement -> ObjectLiteralElementLike
        abstract convertToAssignmentPattern: node: BindingOrAssignmentPattern -> AssignmentPattern
        abstract convertToObjectAssignmentPattern: node: ObjectBindingOrAssignmentPattern -> ObjectLiteralExpression
        abstract convertToArrayAssignmentPattern: node: ArrayBindingOrAssignmentPattern -> ArrayLiteralExpression
        abstract convertToAssignmentElementTarget: node: BindingOrAssignmentElementTarget -> Expression

    type [<AllowNullLiteral>] GeneratedNamePart =
        /// <summary>an additional prefix to insert before the text sourced from <c>node</c></summary>
        abstract prefix: string option with get, set
        abstract node: U2<Identifier, PrivateIdentifier> with get, set
        /// <summary>an additional suffix to insert after the text sourced from <c>node</c></summary>
        abstract suffix: string option with get, set

    type [<AllowNullLiteral>] ImmediatelyInvokedFunctionExpression =
        interface end

    type [<AllowNullLiteral>] ImmediatelyInvokedArrowFunction =
        interface end

    type [<AllowNullLiteral>] NodeFactory =
        abstract parenthesizer: ParenthesizerRules
        abstract converters: NodeConverters
        abstract baseFactory: BaseNodeFactory
        abstract flags: NodeFactoryFlags
        abstract createNodeArray: ?elements: ResizeArray<'T> * ?hasTrailingComma: bool -> ResizeArray<'T> when 'T :> Node
        abstract createNumericLiteral: value: U2<string, float> * ?numericLiteralFlags: TokenFlags -> NumericLiteral
        abstract createBigIntLiteral: value: U2<string, PseudoBigInt> -> BigIntLiteral
        abstract createStringLiteral: text: string * ?isSingleQuote: bool -> StringLiteral
        abstract createStringLiteral: text: string * ?isSingleQuote: bool * ?hasExtendedUnicodeEscape: bool -> StringLiteral
        abstract createStringLiteralFromNode: sourceNode: U2<PropertyNameLiteral, PrivateIdentifier> * ?isSingleQuote: bool -> StringLiteral
        abstract createRegularExpressionLiteral: text: string -> RegularExpressionLiteral
        abstract createIdentifier: text: string -> Identifier
        abstract createIdentifier: text: string * ?originalKeywordKind: SyntaxKind * ?hasExtendedUnicodeEscape: bool -> Identifier
        /// <summary>Create a unique temporary variable.</summary>
        /// <param name="recordTempVariable">
        /// An optional callback used to record the temporary variable name. This
        /// should usually be a reference to <c>hoistVariableDeclaration</c> from a <c>TransformationContext</c>, but
        /// can be <c>undefined</c> if you plan to record the temporary variable manually.
        /// </param>
        /// <param name="reservedInNestedScopes">
        /// When <c>true</c>, reserves the temporary variable name in all nested scopes
        /// during emit so that the variable can be referenced in a nested function body. This is an alternative to
        /// setting <c>EmitFlags.ReuseTempVariableScope</c> on the nested function itself.
        /// </param>
        abstract createTempVariable: recordTempVariable: (Identifier -> unit) option * ?reservedInNestedScopes: bool -> Identifier
        abstract createTempVariable: recordTempVariable: (Identifier -> unit) option * ?reservedInNestedScopes: bool * ?prefix: U2<string, GeneratedNamePart> * ?suffix: string -> Identifier
        /// <summary>Create a unique temporary variable for use in a loop.</summary>
        /// <param name="reservedInNestedScopes">
        /// When <c>true</c>, reserves the temporary variable name in all nested scopes
        /// during emit so that the variable can be referenced in a nested function body. This is an alternative to
        /// setting <c>EmitFlags.ReuseTempVariableScope</c> on the nested function itself.
        /// </param>
        abstract createLoopVariable: ?reservedInNestedScopes: bool -> Identifier
        /// Create a unique name based on the supplied text.
        abstract createUniqueName: text: string * ?flags: GeneratedIdentifierFlags -> Identifier
        abstract createUniqueName: text: string * ?flags: GeneratedIdentifierFlags * ?prefix: U2<string, GeneratedNamePart> * ?suffix: string -> Identifier
        /// Create a unique name generated for a node.
        abstract getGeneratedNameForNode: node: Node option * ?flags: GeneratedIdentifierFlags -> Identifier
        abstract getGeneratedNameForNode: node: Node option * ?flags: GeneratedIdentifierFlags * ?prefix: U2<string, GeneratedNamePart> * ?suffix: string -> Identifier
        abstract createPrivateIdentifier: text: string -> PrivateIdentifier
        abstract createUniquePrivateName: ?text: string -> PrivateIdentifier
        abstract createUniquePrivateName: ?text: string * ?prefix: U2<string, GeneratedNamePart> * ?suffix: string -> PrivateIdentifier
        abstract getGeneratedPrivateNameForNode: node: Node -> PrivateIdentifier
        abstract getGeneratedPrivateNameForNode: node: Node * ?prefix: U2<string, GeneratedNamePart> * ?suffix: string -> PrivateIdentifier
        abstract createToken: token: SyntaxKind -> SuperExpression
        abstract createToken: token: 'TKind -> PunctuationToken<'TKind>
        abstract createSuper: unit -> SuperExpression
        abstract createThis: unit -> ThisExpression
        abstract createNull: unit -> NullLiteral
        abstract createTrue: unit -> TrueLiteral
        abstract createFalse: unit -> FalseLiteral
        abstract createModifier: kind: 'T -> ModifierToken<'T>
        abstract createModifiersFromModifierFlags: flags: ModifierFlags -> ResizeArray<Modifier> option
        abstract createQualifiedName: left: EntityName * right: U2<string, Identifier> -> QualifiedName
        abstract updateQualifiedName: node: QualifiedName * left: EntityName * right: Identifier -> QualifiedName
        abstract createComputedPropertyName: expression: Expression -> ComputedPropertyName
        abstract updateComputedPropertyName: node: ComputedPropertyName * expression: Expression -> ComputedPropertyName
        abstract createTypeParameterDeclaration: modifiers: ResizeArray<Modifier> option * name: U2<string, Identifier> * ?``constraint``: TypeNode * ?defaultType: TypeNode -> TypeParameterDeclaration
        abstract updateTypeParameterDeclaration: node: TypeParameterDeclaration * modifiers: ResizeArray<Modifier> option * name: Identifier * ``constraint``: TypeNode option * defaultType: TypeNode option -> TypeParameterDeclaration
        abstract createParameterDeclaration: modifiers: ResizeArray<ModifierLike> option * dotDotDotToken: DotDotDotToken option * name: U2<string, BindingName> * ?questionToken: QuestionToken * ?``type``: TypeNode * ?initializer: Expression -> ParameterDeclaration
        abstract updateParameterDeclaration: node: ParameterDeclaration * modifiers: ResizeArray<ModifierLike> option * dotDotDotToken: DotDotDotToken option * name: U2<string, BindingName> * questionToken: QuestionToken option * ``type``: TypeNode option * initializer: Expression option -> ParameterDeclaration
        abstract createDecorator: expression: Expression -> Decorator
        abstract updateDecorator: node: Decorator * expression: Expression -> Decorator
        abstract createPropertySignature: modifiers: ResizeArray<Modifier> option * name: U2<PropertyName, string> * questionToken: QuestionToken option * ``type``: TypeNode option -> PropertySignature
        abstract updatePropertySignature: node: PropertySignature * modifiers: ResizeArray<Modifier> option * name: PropertyName * questionToken: QuestionToken option * ``type``: TypeNode option -> PropertySignature
        abstract createPropertyDeclaration: modifiers: ResizeArray<ModifierLike> option * name: U2<string, PropertyName> * questionOrExclamationToken: U2<QuestionToken, ExclamationToken> option * ``type``: TypeNode option * initializer: Expression option -> PropertyDeclaration
        abstract updatePropertyDeclaration: node: PropertyDeclaration * modifiers: ResizeArray<ModifierLike> option * name: U2<string, PropertyName> * questionOrExclamationToken: U2<QuestionToken, ExclamationToken> option * ``type``: TypeNode option * initializer: Expression option -> PropertyDeclaration
        abstract createMethodSignature: modifiers: ResizeArray<Modifier> option * name: U2<string, PropertyName> * questionToken: QuestionToken option * typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option -> MethodSignature
        abstract updateMethodSignature: node: MethodSignature * modifiers: ResizeArray<Modifier> option * name: PropertyName * questionToken: QuestionToken option * typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option -> MethodSignature
        abstract createMethodDeclaration: modifiers: ResizeArray<ModifierLike> option * asteriskToken: AsteriskToken option * name: U2<string, PropertyName> * questionToken: QuestionToken option * typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option * body: Block option -> MethodDeclaration
        abstract updateMethodDeclaration: node: MethodDeclaration * modifiers: ResizeArray<ModifierLike> option * asteriskToken: AsteriskToken option * name: PropertyName * questionToken: QuestionToken option * typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option * body: Block option -> MethodDeclaration
        abstract createConstructorDeclaration: modifiers: ResizeArray<ModifierLike> option * parameters: ResizeArray<ParameterDeclaration> * body: Block option -> ConstructorDeclaration
        abstract updateConstructorDeclaration: node: ConstructorDeclaration * modifiers: ResizeArray<ModifierLike> option * parameters: ResizeArray<ParameterDeclaration> * body: Block option -> ConstructorDeclaration
        abstract createGetAccessorDeclaration: modifiers: ResizeArray<ModifierLike> option * name: U2<string, PropertyName> * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option * body: Block option -> GetAccessorDeclaration
        abstract updateGetAccessorDeclaration: node: GetAccessorDeclaration * modifiers: ResizeArray<ModifierLike> option * name: PropertyName * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option * body: Block option -> GetAccessorDeclaration
        abstract createSetAccessorDeclaration: modifiers: ResizeArray<ModifierLike> option * name: U2<string, PropertyName> * parameters: ResizeArray<ParameterDeclaration> * body: Block option -> SetAccessorDeclaration
        abstract updateSetAccessorDeclaration: node: SetAccessorDeclaration * modifiers: ResizeArray<ModifierLike> option * name: PropertyName * parameters: ResizeArray<ParameterDeclaration> * body: Block option -> SetAccessorDeclaration
        abstract createCallSignature: typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option -> CallSignatureDeclaration
        abstract updateCallSignature: node: CallSignatureDeclaration * typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option -> CallSignatureDeclaration
        abstract createConstructSignature: typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option -> ConstructSignatureDeclaration
        abstract updateConstructSignature: node: ConstructSignatureDeclaration * typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option -> ConstructSignatureDeclaration
        abstract createIndexSignature: modifiers: ResizeArray<ModifierLike> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode -> IndexSignatureDeclaration
        abstract createIndexSignature: modifiers: ResizeArray<ModifierLike> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option -> IndexSignatureDeclaration
        abstract updateIndexSignature: node: IndexSignatureDeclaration * modifiers: ResizeArray<ModifierLike> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode -> IndexSignatureDeclaration
        abstract createTemplateLiteralTypeSpan: ``type``: TypeNode * literal: U2<TemplateMiddle, TemplateTail> -> TemplateLiteralTypeSpan
        abstract updateTemplateLiteralTypeSpan: node: TemplateLiteralTypeSpan * ``type``: TypeNode * literal: U2<TemplateMiddle, TemplateTail> -> TemplateLiteralTypeSpan
        abstract createClassStaticBlockDeclaration: body: Block -> ClassStaticBlockDeclaration
        abstract updateClassStaticBlockDeclaration: node: ClassStaticBlockDeclaration * body: Block -> ClassStaticBlockDeclaration
        abstract createKeywordTypeNode: kind: 'TKind -> KeywordTypeNode<'TKind>
        abstract createTypePredicateNode: assertsModifier: AssertsKeyword option * parameterName: U3<Identifier, ThisTypeNode, string> * ``type``: TypeNode option -> TypePredicateNode
        abstract updateTypePredicateNode: node: TypePredicateNode * assertsModifier: AssertsKeyword option * parameterName: U2<Identifier, ThisTypeNode> * ``type``: TypeNode option -> TypePredicateNode
        abstract createTypeReferenceNode: typeName: U2<string, EntityName> * ?typeArguments: ResizeArray<TypeNode> -> TypeReferenceNode
        abstract updateTypeReferenceNode: node: TypeReferenceNode * typeName: EntityName * typeArguments: ResizeArray<TypeNode> option -> TypeReferenceNode
        abstract createFunctionTypeNode: typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode -> FunctionTypeNode
        abstract updateFunctionTypeNode: node: FunctionTypeNode * typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode -> FunctionTypeNode
        abstract createConstructorTypeNode: modifiers: ResizeArray<Modifier> option * typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode -> ConstructorTypeNode
        abstract updateConstructorTypeNode: node: ConstructorTypeNode * modifiers: ResizeArray<Modifier> option * typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode -> ConstructorTypeNode
        abstract createTypeQueryNode: exprName: EntityName * ?typeArguments: ResizeArray<TypeNode> -> TypeQueryNode
        abstract updateTypeQueryNode: node: TypeQueryNode * exprName: EntityName * ?typeArguments: ResizeArray<TypeNode> -> TypeQueryNode
        abstract createTypeLiteralNode: members: ResizeArray<TypeElement> option -> TypeLiteralNode
        abstract updateTypeLiteralNode: node: TypeLiteralNode * members: ResizeArray<TypeElement> -> TypeLiteralNode
        abstract createArrayTypeNode: elementType: TypeNode -> ArrayTypeNode
        abstract updateArrayTypeNode: node: ArrayTypeNode * elementType: TypeNode -> ArrayTypeNode
        abstract createTupleTypeNode: elements: ResizeArray<U2<TypeNode, NamedTupleMember>> -> TupleTypeNode
        abstract updateTupleTypeNode: node: TupleTypeNode * elements: ResizeArray<U2<TypeNode, NamedTupleMember>> -> TupleTypeNode
        abstract createNamedTupleMember: dotDotDotToken: DotDotDotToken option * name: Identifier * questionToken: QuestionToken option * ``type``: TypeNode -> NamedTupleMember
        abstract updateNamedTupleMember: node: NamedTupleMember * dotDotDotToken: DotDotDotToken option * name: Identifier * questionToken: QuestionToken option * ``type``: TypeNode -> NamedTupleMember
        abstract createOptionalTypeNode: ``type``: TypeNode -> OptionalTypeNode
        abstract updateOptionalTypeNode: node: OptionalTypeNode * ``type``: TypeNode -> OptionalTypeNode
        abstract createRestTypeNode: ``type``: TypeNode -> RestTypeNode
        abstract updateRestTypeNode: node: RestTypeNode * ``type``: TypeNode -> RestTypeNode
        abstract createUnionTypeNode: types: ResizeArray<TypeNode> -> UnionTypeNode
        abstract updateUnionTypeNode: node: UnionTypeNode * types: ResizeArray<TypeNode> -> UnionTypeNode
        abstract createIntersectionTypeNode: types: ResizeArray<TypeNode> -> IntersectionTypeNode
        abstract updateIntersectionTypeNode: node: IntersectionTypeNode * types: ResizeArray<TypeNode> -> IntersectionTypeNode
        abstract createConditionalTypeNode: checkType: TypeNode * extendsType: TypeNode * trueType: TypeNode * falseType: TypeNode -> ConditionalTypeNode
        abstract updateConditionalTypeNode: node: ConditionalTypeNode * checkType: TypeNode * extendsType: TypeNode * trueType: TypeNode * falseType: TypeNode -> ConditionalTypeNode
        abstract createInferTypeNode: typeParameter: TypeParameterDeclaration -> InferTypeNode
        abstract updateInferTypeNode: node: InferTypeNode * typeParameter: TypeParameterDeclaration -> InferTypeNode
        abstract createImportTypeNode: argument: TypeNode * ?attributes: ImportAttributes * ?qualifier: EntityName * ?typeArguments: ResizeArray<TypeNode> * ?isTypeOf: bool -> ImportTypeNode
        abstract updateImportTypeNode: node: ImportTypeNode * argument: TypeNode * attributes: ImportAttributes option * qualifier: EntityName option * typeArguments: ResizeArray<TypeNode> option * ?isTypeOf: bool -> ImportTypeNode
        abstract createParenthesizedType: ``type``: TypeNode -> ParenthesizedTypeNode
        abstract updateParenthesizedType: node: ParenthesizedTypeNode * ``type``: TypeNode -> ParenthesizedTypeNode
        abstract createThisTypeNode: unit -> ThisTypeNode
        abstract createTypeOperatorNode: operator: SyntaxKind * ``type``: TypeNode -> TypeOperatorNode
        abstract updateTypeOperatorNode: node: TypeOperatorNode * ``type``: TypeNode -> TypeOperatorNode
        abstract createIndexedAccessTypeNode: objectType: TypeNode * indexType: TypeNode -> IndexedAccessTypeNode
        abstract updateIndexedAccessTypeNode: node: IndexedAccessTypeNode * objectType: TypeNode * indexType: TypeNode -> IndexedAccessTypeNode
        abstract createMappedTypeNode: readonlyToken: U3<ReadonlyKeyword, PlusToken, MinusToken> option * typeParameter: TypeParameterDeclaration * nameType: TypeNode option * questionToken: U3<QuestionToken, PlusToken, MinusToken> option * ``type``: TypeNode option * members: ResizeArray<TypeElement> option -> MappedTypeNode
        abstract updateMappedTypeNode: node: MappedTypeNode * readonlyToken: U3<ReadonlyKeyword, PlusToken, MinusToken> option * typeParameter: TypeParameterDeclaration * nameType: TypeNode option * questionToken: U3<QuestionToken, PlusToken, MinusToken> option * ``type``: TypeNode option * members: ResizeArray<TypeElement> option -> MappedTypeNode
        abstract createLiteralTypeNode: literal: obj -> LiteralTypeNode
        abstract updateLiteralTypeNode: node: LiteralTypeNode * literal: obj -> LiteralTypeNode
        abstract createTemplateLiteralType: head: TemplateHead * templateSpans: ResizeArray<TemplateLiteralTypeSpan> -> TemplateLiteralTypeNode
        abstract updateTemplateLiteralType: node: TemplateLiteralTypeNode * head: TemplateHead * templateSpans: ResizeArray<TemplateLiteralTypeSpan> -> TemplateLiteralTypeNode
        abstract createObjectBindingPattern: elements: ResizeArray<BindingElement> -> ObjectBindingPattern
        abstract updateObjectBindingPattern: node: ObjectBindingPattern * elements: ResizeArray<BindingElement> -> ObjectBindingPattern
        abstract createArrayBindingPattern: elements: ResizeArray<ArrayBindingElement> -> ArrayBindingPattern
        abstract updateArrayBindingPattern: node: ArrayBindingPattern * elements: ResizeArray<ArrayBindingElement> -> ArrayBindingPattern
        abstract createBindingElement: dotDotDotToken: DotDotDotToken option * propertyName: U2<string, PropertyName> option * name: U2<string, BindingName> * ?initializer: Expression -> BindingElement
        abstract updateBindingElement: node: BindingElement * dotDotDotToken: DotDotDotToken option * propertyName: PropertyName option * name: BindingName * initializer: Expression option -> BindingElement
        abstract createArrayLiteralExpression: ?elements: ResizeArray<Expression> * ?multiLine: bool -> ArrayLiteralExpression
        abstract updateArrayLiteralExpression: node: ArrayLiteralExpression * elements: ResizeArray<Expression> -> ArrayLiteralExpression
        abstract createObjectLiteralExpression: ?properties: ResizeArray<ObjectLiteralElementLike> * ?multiLine: bool -> ObjectLiteralExpression
        abstract updateObjectLiteralExpression: node: ObjectLiteralExpression * properties: ResizeArray<ObjectLiteralElementLike> -> ObjectLiteralExpression
        abstract createPropertyAccessExpression: expression: Expression * name: U2<string, MemberName> -> PropertyAccessExpression
        abstract updatePropertyAccessExpression: node: PropertyAccessExpression * expression: Expression * name: MemberName -> PropertyAccessExpression
        abstract createPropertyAccessChain: expression: Expression * questionDotToken: QuestionDotToken option * name: U2<string, MemberName> -> PropertyAccessChain
        abstract updatePropertyAccessChain: node: PropertyAccessChain * expression: Expression * questionDotToken: QuestionDotToken option * name: MemberName -> PropertyAccessChain
        abstract createElementAccessExpression: expression: Expression * index: U2<float, Expression> -> ElementAccessExpression
        abstract updateElementAccessExpression: node: ElementAccessExpression * expression: Expression * argumentExpression: Expression -> ElementAccessExpression
        abstract createElementAccessChain: expression: Expression * questionDotToken: QuestionDotToken option * index: U2<float, Expression> -> ElementAccessChain
        abstract updateElementAccessChain: node: ElementAccessChain * expression: Expression * questionDotToken: QuestionDotToken option * argumentExpression: Expression -> ElementAccessChain
        abstract createCallExpression: expression: Expression * typeArguments: ResizeArray<TypeNode> option * argumentsArray: ResizeArray<Expression> option -> CallExpression
        abstract updateCallExpression: node: CallExpression * expression: Expression * typeArguments: ResizeArray<TypeNode> option * argumentsArray: ResizeArray<Expression> -> CallExpression
        abstract createCallChain: expression: Expression * questionDotToken: QuestionDotToken option * typeArguments: ResizeArray<TypeNode> option * argumentsArray: ResizeArray<Expression> option -> CallChain
        abstract updateCallChain: node: CallChain * expression: Expression * questionDotToken: QuestionDotToken option * typeArguments: ResizeArray<TypeNode> option * argumentsArray: ResizeArray<Expression> -> CallChain
        abstract createNewExpression: expression: Expression * typeArguments: ResizeArray<TypeNode> option * argumentsArray: ResizeArray<Expression> option -> NewExpression
        abstract updateNewExpression: node: NewExpression * expression: Expression * typeArguments: ResizeArray<TypeNode> option * argumentsArray: ResizeArray<Expression> option -> NewExpression
        abstract createTaggedTemplateExpression: tag: Expression * typeArguments: ResizeArray<TypeNode> option * template: TemplateLiteral -> TaggedTemplateExpression
        abstract updateTaggedTemplateExpression: node: TaggedTemplateExpression * tag: Expression * typeArguments: ResizeArray<TypeNode> option * template: TemplateLiteral -> TaggedTemplateExpression
        abstract createTypeAssertion: ``type``: TypeNode * expression: Expression -> TypeAssertion
        abstract updateTypeAssertion: node: TypeAssertion * ``type``: TypeNode * expression: Expression -> TypeAssertion
        abstract createParenthesizedExpression: expression: Expression -> ParenthesizedExpression
        abstract updateParenthesizedExpression: node: ParenthesizedExpression * expression: Expression -> ParenthesizedExpression
        abstract createFunctionExpression: modifiers: ResizeArray<Modifier> option * asteriskToken: AsteriskToken option * name: U2<string, Identifier> option * typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> option * ``type``: TypeNode option * body: Block -> FunctionExpression
        abstract updateFunctionExpression: node: FunctionExpression * modifiers: ResizeArray<Modifier> option * asteriskToken: AsteriskToken option * name: Identifier option * typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option * body: Block -> FunctionExpression
        abstract createArrowFunction: modifiers: ResizeArray<Modifier> option * typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option * equalsGreaterThanToken: EqualsGreaterThanToken option * body: ConciseBody -> ArrowFunction
        abstract updateArrowFunction: node: ArrowFunction * modifiers: ResizeArray<Modifier> option * typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option * equalsGreaterThanToken: EqualsGreaterThanToken * body: ConciseBody -> ArrowFunction
        abstract createDeleteExpression: expression: Expression -> DeleteExpression
        abstract updateDeleteExpression: node: DeleteExpression * expression: Expression -> DeleteExpression
        abstract createTypeOfExpression: expression: Expression -> TypeOfExpression
        abstract updateTypeOfExpression: node: TypeOfExpression * expression: Expression -> TypeOfExpression
        abstract createVoidExpression: expression: Expression -> VoidExpression
        abstract updateVoidExpression: node: VoidExpression * expression: Expression -> VoidExpression
        abstract createAwaitExpression: expression: Expression -> AwaitExpression
        abstract updateAwaitExpression: node: AwaitExpression * expression: Expression -> AwaitExpression
        abstract createPrefixUnaryExpression: operator: PrefixUnaryOperator * operand: Expression -> PrefixUnaryExpression
        abstract updatePrefixUnaryExpression: node: PrefixUnaryExpression * operand: Expression -> PrefixUnaryExpression
        abstract createPostfixUnaryExpression: operand: Expression * operator: PostfixUnaryOperator -> PostfixUnaryExpression
        abstract updatePostfixUnaryExpression: node: PostfixUnaryExpression * operand: Expression -> PostfixUnaryExpression
        abstract createBinaryExpression: left: Expression * operator: U2<BinaryOperator, BinaryOperatorToken> * right: Expression -> BinaryExpression
        abstract updateBinaryExpression: node: BinaryExpression * left: Expression * operator: U2<BinaryOperator, BinaryOperatorToken> * right: Expression -> BinaryExpression
        abstract createConditionalExpression: condition: Expression * questionToken: QuestionToken option * whenTrue: Expression * colonToken: ColonToken option * whenFalse: Expression -> ConditionalExpression
        abstract updateConditionalExpression: node: ConditionalExpression * condition: Expression * questionToken: QuestionToken * whenTrue: Expression * colonToken: ColonToken * whenFalse: Expression -> ConditionalExpression
        abstract createTemplateExpression: head: TemplateHead * templateSpans: ResizeArray<TemplateSpan> -> TemplateExpression
        abstract updateTemplateExpression: node: TemplateExpression * head: TemplateHead * templateSpans: ResizeArray<TemplateSpan> -> TemplateExpression
        abstract createTemplateHead: text: string * ?rawText: string * ?templateFlags: TokenFlags -> TemplateHead
        abstract createTemplateHead: text: string option * rawText: string * ?templateFlags: TokenFlags -> TemplateHead
        abstract createTemplateMiddle: text: string * ?rawText: string * ?templateFlags: TokenFlags -> TemplateMiddle
        abstract createTemplateMiddle: text: string option * rawText: string * ?templateFlags: TokenFlags -> TemplateMiddle
        abstract createTemplateTail: text: string * ?rawText: string * ?templateFlags: TokenFlags -> TemplateTail
        abstract createTemplateTail: text: string option * rawText: string * ?templateFlags: TokenFlags -> TemplateTail
        abstract createNoSubstitutionTemplateLiteral: text: string * ?rawText: string -> NoSubstitutionTemplateLiteral
        abstract createNoSubstitutionTemplateLiteral: text: string option * rawText: string -> NoSubstitutionTemplateLiteral
        abstract createLiteralLikeNode: kind: U2<obj, SyntaxKind> * text: string -> LiteralToken
        abstract createTemplateLiteralLikeNode: kind: obj * text: string * rawText: string * templateFlags: TokenFlags option -> TemplateLiteralLikeNode
        abstract createYieldExpression: asteriskToken: AsteriskToken * expression: Expression -> YieldExpression
        abstract createYieldExpression: asteriskToken: obj * expression: Expression option -> YieldExpression
        abstract createYieldExpression: asteriskToken: AsteriskToken option * expression: Expression option -> YieldExpression
        abstract updateYieldExpression: node: YieldExpression * asteriskToken: AsteriskToken option * expression: Expression option -> YieldExpression
        abstract createSpreadElement: expression: Expression -> SpreadElement
        abstract updateSpreadElement: node: SpreadElement * expression: Expression -> SpreadElement
        abstract createClassExpression: modifiers: ResizeArray<ModifierLike> option * name: U2<string, Identifier> option * typeParameters: ResizeArray<TypeParameterDeclaration> option * heritageClauses: ResizeArray<HeritageClause> option * members: ResizeArray<ClassElement> -> ClassExpression
        abstract updateClassExpression: node: ClassExpression * modifiers: ResizeArray<ModifierLike> option * name: Identifier option * typeParameters: ResizeArray<TypeParameterDeclaration> option * heritageClauses: ResizeArray<HeritageClause> option * members: ResizeArray<ClassElement> -> ClassExpression
        abstract createOmittedExpression: unit -> OmittedExpression
        abstract createExpressionWithTypeArguments: expression: Expression * typeArguments: ResizeArray<TypeNode> option -> ExpressionWithTypeArguments
        abstract updateExpressionWithTypeArguments: node: ExpressionWithTypeArguments * expression: Expression * typeArguments: ResizeArray<TypeNode> option -> ExpressionWithTypeArguments
        abstract createAsExpression: expression: Expression * ``type``: TypeNode -> AsExpression
        abstract updateAsExpression: node: AsExpression * expression: Expression * ``type``: TypeNode -> AsExpression
        abstract createNonNullExpression: expression: Expression -> NonNullExpression
        abstract updateNonNullExpression: node: NonNullExpression * expression: Expression -> NonNullExpression
        abstract createNonNullChain: expression: Expression -> NonNullChain
        abstract updateNonNullChain: node: NonNullChain * expression: Expression -> NonNullChain
        abstract createMetaProperty: keywordToken: obj * name: Identifier -> MetaProperty
        abstract updateMetaProperty: node: MetaProperty * name: Identifier -> MetaProperty
        abstract createSatisfiesExpression: expression: Expression * ``type``: TypeNode -> SatisfiesExpression
        abstract updateSatisfiesExpression: node: SatisfiesExpression * expression: Expression * ``type``: TypeNode -> SatisfiesExpression
        abstract createTemplateSpan: expression: Expression * literal: U2<TemplateMiddle, TemplateTail> -> TemplateSpan
        abstract updateTemplateSpan: node: TemplateSpan * expression: Expression * literal: U2<TemplateMiddle, TemplateTail> -> TemplateSpan
        abstract createSemicolonClassElement: unit -> SemicolonClassElement
        abstract createBlock: statements: ResizeArray<Statement> * ?multiLine: bool -> Block
        abstract updateBlock: node: Block * statements: ResizeArray<Statement> -> Block
        abstract createVariableStatement: modifiers: ResizeArray<ModifierLike> option * declarationList: U2<VariableDeclarationList, ResizeArray<VariableDeclaration>> -> VariableStatement
        abstract updateVariableStatement: node: VariableStatement * modifiers: ResizeArray<ModifierLike> option * declarationList: VariableDeclarationList -> VariableStatement
        abstract createEmptyStatement: unit -> EmptyStatement
        abstract createExpressionStatement: expression: Expression -> ExpressionStatement
        abstract updateExpressionStatement: node: ExpressionStatement * expression: Expression -> ExpressionStatement
        abstract createIfStatement: expression: Expression * thenStatement: Statement * ?elseStatement: Statement -> IfStatement
        abstract updateIfStatement: node: IfStatement * expression: Expression * thenStatement: Statement * elseStatement: Statement option -> IfStatement
        abstract createDoStatement: statement: Statement * expression: Expression -> DoStatement
        abstract updateDoStatement: node: DoStatement * statement: Statement * expression: Expression -> DoStatement
        abstract createWhileStatement: expression: Expression * statement: Statement -> WhileStatement
        abstract updateWhileStatement: node: WhileStatement * expression: Expression * statement: Statement -> WhileStatement
        abstract createForStatement: initializer: ForInitializer option * condition: Expression option * incrementor: Expression option * statement: Statement -> ForStatement
        abstract updateForStatement: node: ForStatement * initializer: ForInitializer option * condition: Expression option * incrementor: Expression option * statement: Statement -> ForStatement
        abstract createForInStatement: initializer: ForInitializer * expression: Expression * statement: Statement -> ForInStatement
        abstract updateForInStatement: node: ForInStatement * initializer: ForInitializer * expression: Expression * statement: Statement -> ForInStatement
        abstract createForOfStatement: awaitModifier: AwaitKeyword option * initializer: ForInitializer * expression: Expression * statement: Statement -> ForOfStatement
        abstract updateForOfStatement: node: ForOfStatement * awaitModifier: AwaitKeyword option * initializer: ForInitializer * expression: Expression * statement: Statement -> ForOfStatement
        abstract createContinueStatement: ?label: U2<string, Identifier> -> ContinueStatement
        abstract updateContinueStatement: node: ContinueStatement * label: Identifier option -> ContinueStatement
        abstract createBreakStatement: ?label: U2<string, Identifier> -> BreakStatement
        abstract updateBreakStatement: node: BreakStatement * label: Identifier option -> BreakStatement
        abstract createReturnStatement: ?expression: Expression -> ReturnStatement
        abstract updateReturnStatement: node: ReturnStatement * expression: Expression option -> ReturnStatement
        abstract createWithStatement: expression: Expression * statement: Statement -> WithStatement
        abstract updateWithStatement: node: WithStatement * expression: Expression * statement: Statement -> WithStatement
        abstract createSwitchStatement: expression: Expression * caseBlock: CaseBlock -> SwitchStatement
        abstract updateSwitchStatement: node: SwitchStatement * expression: Expression * caseBlock: CaseBlock -> SwitchStatement
        abstract createLabeledStatement: label: U2<string, Identifier> * statement: Statement -> LabeledStatement
        abstract updateLabeledStatement: node: LabeledStatement * label: Identifier * statement: Statement -> LabeledStatement
        abstract createThrowStatement: expression: Expression -> ThrowStatement
        abstract updateThrowStatement: node: ThrowStatement * expression: Expression -> ThrowStatement
        abstract createTryStatement: tryBlock: Block * catchClause: CatchClause option * finallyBlock: Block option -> TryStatement
        abstract updateTryStatement: node: TryStatement * tryBlock: Block * catchClause: CatchClause option * finallyBlock: Block option -> TryStatement
        abstract createDebuggerStatement: unit -> DebuggerStatement
        abstract createVariableDeclaration: name: U2<string, BindingName> * ?exclamationToken: ExclamationToken * ?``type``: TypeNode * ?initializer: Expression -> VariableDeclaration
        abstract updateVariableDeclaration: node: VariableDeclaration * name: BindingName * exclamationToken: ExclamationToken option * ``type``: TypeNode option * initializer: Expression option -> VariableDeclaration
        abstract createVariableDeclarationList: declarations: ResizeArray<VariableDeclaration> * ?flags: NodeFlags -> VariableDeclarationList
        abstract updateVariableDeclarationList: node: VariableDeclarationList * declarations: ResizeArray<VariableDeclaration> -> VariableDeclarationList
        abstract createFunctionDeclaration: modifiers: ResizeArray<ModifierLike> option * asteriskToken: AsteriskToken option * name: U2<string, Identifier> option * typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option * body: Block option -> FunctionDeclaration
        abstract updateFunctionDeclaration: node: FunctionDeclaration * modifiers: ResizeArray<ModifierLike> option * asteriskToken: AsteriskToken option * name: Identifier option * typeParameters: ResizeArray<TypeParameterDeclaration> option * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option * body: Block option -> FunctionDeclaration
        abstract createClassDeclaration: modifiers: ResizeArray<ModifierLike> option * name: U2<string, Identifier> option * typeParameters: ResizeArray<TypeParameterDeclaration> option * heritageClauses: ResizeArray<HeritageClause> option * members: ResizeArray<ClassElement> -> ClassDeclaration
        abstract updateClassDeclaration: node: ClassDeclaration * modifiers: ResizeArray<ModifierLike> option * name: Identifier option * typeParameters: ResizeArray<TypeParameterDeclaration> option * heritageClauses: ResizeArray<HeritageClause> option * members: ResizeArray<ClassElement> -> ClassDeclaration
        abstract createInterfaceDeclaration: modifiers: ResizeArray<ModifierLike> option * name: U2<string, Identifier> * typeParameters: ResizeArray<TypeParameterDeclaration> option * heritageClauses: ResizeArray<HeritageClause> option * members: ResizeArray<TypeElement> -> InterfaceDeclaration
        abstract updateInterfaceDeclaration: node: InterfaceDeclaration * modifiers: ResizeArray<ModifierLike> option * name: Identifier * typeParameters: ResizeArray<TypeParameterDeclaration> option * heritageClauses: ResizeArray<HeritageClause> option * members: ResizeArray<TypeElement> -> InterfaceDeclaration
        abstract createTypeAliasDeclaration: modifiers: ResizeArray<ModifierLike> option * name: U2<string, Identifier> * typeParameters: ResizeArray<TypeParameterDeclaration> option * ``type``: TypeNode -> TypeAliasDeclaration
        abstract updateTypeAliasDeclaration: node: TypeAliasDeclaration * modifiers: ResizeArray<ModifierLike> option * name: Identifier * typeParameters: ResizeArray<TypeParameterDeclaration> option * ``type``: TypeNode -> TypeAliasDeclaration
        abstract createEnumDeclaration: modifiers: ResizeArray<ModifierLike> option * name: U2<string, Identifier> * members: ResizeArray<EnumMember> -> EnumDeclaration
        abstract updateEnumDeclaration: node: EnumDeclaration * modifiers: ResizeArray<ModifierLike> option * name: Identifier * members: ResizeArray<EnumMember> -> EnumDeclaration
        abstract createModuleDeclaration: modifiers: ResizeArray<ModifierLike> option * name: ModuleName * body: ModuleBody option * ?flags: NodeFlags -> ModuleDeclaration
        abstract updateModuleDeclaration: node: ModuleDeclaration * modifiers: ResizeArray<ModifierLike> option * name: ModuleName * body: ModuleBody option -> ModuleDeclaration
        abstract createModuleBlock: statements: ResizeArray<Statement> -> ModuleBlock
        abstract updateModuleBlock: node: ModuleBlock * statements: ResizeArray<Statement> -> ModuleBlock
        abstract createCaseBlock: clauses: ResizeArray<CaseOrDefaultClause> -> CaseBlock
        abstract updateCaseBlock: node: CaseBlock * clauses: ResizeArray<CaseOrDefaultClause> -> CaseBlock
        abstract createNamespaceExportDeclaration: name: U2<string, Identifier> -> NamespaceExportDeclaration
        abstract updateNamespaceExportDeclaration: node: NamespaceExportDeclaration * name: Identifier -> NamespaceExportDeclaration
        abstract createImportEqualsDeclaration: modifiers: ResizeArray<ModifierLike> option * isTypeOnly: bool * name: U2<string, Identifier> * moduleReference: ModuleReference -> ImportEqualsDeclaration
        abstract updateImportEqualsDeclaration: node: ImportEqualsDeclaration * modifiers: ResizeArray<ModifierLike> option * isTypeOnly: bool * name: Identifier * moduleReference: ModuleReference -> ImportEqualsDeclaration
        abstract createImportDeclaration: modifiers: ResizeArray<ModifierLike> option * importClause: ImportClause option * moduleSpecifier: Expression * ?attributes: ImportAttributes -> ImportDeclaration
        abstract updateImportDeclaration: node: ImportDeclaration * modifiers: ResizeArray<ModifierLike> option * importClause: ImportClause option * moduleSpecifier: Expression * attributes: ImportAttributes option -> ImportDeclaration
        abstract createImportClause: phaseModifier: ImportPhaseModifierSyntaxKind option * name: Identifier option * namedBindings: NamedImportBindings option -> ImportClause
        [<Obsolete("")>]
        abstract createImportClause: isTypeOnly: bool * name: Identifier option * namedBindings: NamedImportBindings option -> ImportClause
        abstract updateImportClause: node: ImportClause * phaseModifier: ImportPhaseModifierSyntaxKind option * name: Identifier option * namedBindings: NamedImportBindings option -> ImportClause
        [<Obsolete("")>]
        abstract updateImportClause: node: ImportClause * isTypeOnly: bool * name: Identifier option * namedBindings: NamedImportBindings option -> ImportClause
        [<Obsolete("")>]
        abstract createAssertClause: elements: ResizeArray<AssertEntry> * ?multiLine: bool -> AssertClause
        [<Obsolete("")>]
        abstract updateAssertClause: node: AssertClause * elements: ResizeArray<AssertEntry> * ?multiLine: bool -> AssertClause
        [<Obsolete("")>]
        abstract createAssertEntry: name: AssertionKey * value: Expression -> AssertEntry
        [<Obsolete("")>]
        abstract updateAssertEntry: node: AssertEntry * name: AssertionKey * value: Expression -> AssertEntry
        [<Obsolete("")>]
        abstract createImportTypeAssertionContainer: clause: AssertClause * ?multiLine: bool -> ImportTypeAssertionContainer
        [<Obsolete("")>]
        abstract updateImportTypeAssertionContainer: node: ImportTypeAssertionContainer * clause: AssertClause * ?multiLine: bool -> ImportTypeAssertionContainer
        abstract createImportAttributes: elements: ResizeArray<ImportAttribute> * ?multiLine: bool -> ImportAttributes
        abstract createImportAttributes: elements: ResizeArray<ImportAttribute> * ?multiLine: bool * ?token: obj -> ImportAttributes
        abstract updateImportAttributes: node: ImportAttributes * elements: ResizeArray<ImportAttribute> * ?multiLine: bool -> ImportAttributes
        abstract createImportAttribute: name: ImportAttributeName * value: Expression -> ImportAttribute
        abstract updateImportAttribute: node: ImportAttribute * name: ImportAttributeName * value: Expression -> ImportAttribute
        abstract createNamespaceImport: name: Identifier -> NamespaceImport
        abstract updateNamespaceImport: node: NamespaceImport * name: Identifier -> NamespaceImport
        abstract createNamespaceExport: name: ModuleExportName -> NamespaceExport
        abstract updateNamespaceExport: node: NamespaceExport * name: ModuleExportName -> NamespaceExport
        abstract createNamedImports: elements: ResizeArray<ImportSpecifier> -> NamedImports
        abstract updateNamedImports: node: NamedImports * elements: ResizeArray<ImportSpecifier> -> NamedImports
        abstract createImportSpecifier: isTypeOnly: bool * propertyName: ModuleExportName option * name: Identifier -> ImportSpecifier
        abstract updateImportSpecifier: node: ImportSpecifier * isTypeOnly: bool * propertyName: ModuleExportName option * name: Identifier -> ImportSpecifier
        abstract createExportAssignment: modifiers: ResizeArray<ModifierLike> option * isExportEquals: bool option * expression: Expression -> ExportAssignment
        abstract updateExportAssignment: node: ExportAssignment * modifiers: ResizeArray<ModifierLike> option * expression: Expression -> ExportAssignment
        abstract createExportDeclaration: modifiers: ResizeArray<ModifierLike> option * isTypeOnly: bool * exportClause: NamedExportBindings option * ?moduleSpecifier: Expression * ?attributes: ImportAttributes -> ExportDeclaration
        abstract updateExportDeclaration: node: ExportDeclaration * modifiers: ResizeArray<ModifierLike> option * isTypeOnly: bool * exportClause: NamedExportBindings option * moduleSpecifier: Expression option * attributes: ImportAttributes option -> ExportDeclaration
        abstract createNamedExports: elements: ResizeArray<ExportSpecifier> -> NamedExports
        abstract updateNamedExports: node: NamedExports * elements: ResizeArray<ExportSpecifier> -> NamedExports
        abstract createExportSpecifier: isTypeOnly: bool * propertyName: U2<string, ModuleExportName> option * name: U2<string, ModuleExportName> -> ExportSpecifier
        abstract updateExportSpecifier: node: ExportSpecifier * isTypeOnly: bool * propertyName: ModuleExportName option * name: ModuleExportName -> ExportSpecifier
        abstract createMissingDeclaration: unit -> MissingDeclaration
        abstract createExternalModuleReference: expression: Expression -> ExternalModuleReference
        abstract updateExternalModuleReference: node: ExternalModuleReference * expression: Expression -> ExternalModuleReference
        abstract createJSDocAllType: unit -> JSDocAllType
        abstract createJSDocUnknownType: unit -> JSDocUnknownType
        abstract createJSDocNonNullableType: ``type``: TypeNode * ?postfix: bool -> JSDocNonNullableType
        abstract updateJSDocNonNullableType: node: JSDocNonNullableType * ``type``: TypeNode -> JSDocNonNullableType
        abstract createJSDocNullableType: ``type``: TypeNode * ?postfix: bool -> JSDocNullableType
        abstract updateJSDocNullableType: node: JSDocNullableType * ``type``: TypeNode -> JSDocNullableType
        abstract createJSDocOptionalType: ``type``: TypeNode -> JSDocOptionalType
        abstract updateJSDocOptionalType: node: JSDocOptionalType * ``type``: TypeNode -> JSDocOptionalType
        abstract createJSDocFunctionType: parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option -> JSDocFunctionType
        abstract updateJSDocFunctionType: node: JSDocFunctionType * parameters: ResizeArray<ParameterDeclaration> * ``type``: TypeNode option -> JSDocFunctionType
        abstract createJSDocVariadicType: ``type``: TypeNode -> JSDocVariadicType
        abstract updateJSDocVariadicType: node: JSDocVariadicType * ``type``: TypeNode -> JSDocVariadicType
        abstract createJSDocNamepathType: ``type``: TypeNode -> JSDocNamepathType
        abstract updateJSDocNamepathType: node: JSDocNamepathType * ``type``: TypeNode -> JSDocNamepathType
        abstract createJSDocTypeExpression: ``type``: TypeNode -> JSDocTypeExpression
        abstract updateJSDocTypeExpression: node: JSDocTypeExpression * ``type``: TypeNode -> JSDocTypeExpression
        abstract createJSDocNameReference: name: U2<EntityName, JSDocMemberName> -> JSDocNameReference
        abstract updateJSDocNameReference: node: JSDocNameReference * name: U2<EntityName, JSDocMemberName> -> JSDocNameReference
        abstract createJSDocMemberName: left: U2<EntityName, JSDocMemberName> * right: Identifier -> JSDocMemberName
        abstract updateJSDocMemberName: node: JSDocMemberName * left: U2<EntityName, JSDocMemberName> * right: Identifier -> JSDocMemberName
        abstract createJSDocLink: name: U2<EntityName, JSDocMemberName> option * text: string -> JSDocLink
        abstract updateJSDocLink: node: JSDocLink * name: U2<EntityName, JSDocMemberName> option * text: string -> JSDocLink
        abstract createJSDocLinkCode: name: U2<EntityName, JSDocMemberName> option * text: string -> JSDocLinkCode
        abstract updateJSDocLinkCode: node: JSDocLinkCode * name: U2<EntityName, JSDocMemberName> option * text: string -> JSDocLinkCode
        abstract createJSDocLinkPlain: name: U2<EntityName, JSDocMemberName> option * text: string -> JSDocLinkPlain
        abstract updateJSDocLinkPlain: node: JSDocLinkPlain * name: U2<EntityName, JSDocMemberName> option * text: string -> JSDocLinkPlain
        abstract createJSDocTypeLiteral: ?jsDocPropertyTags: ResizeArray<JSDocPropertyLikeTag> * ?isArrayType: bool -> JSDocTypeLiteral
        abstract updateJSDocTypeLiteral: node: JSDocTypeLiteral * jsDocPropertyTags: ResizeArray<JSDocPropertyLikeTag> option * isArrayType: bool option -> JSDocTypeLiteral
        abstract createJSDocSignature: typeParameters: ResizeArray<JSDocTemplateTag> option * parameters: ResizeArray<JSDocParameterTag> * ?``type``: JSDocReturnTag -> JSDocSignature
        abstract updateJSDocSignature: node: JSDocSignature * typeParameters: ResizeArray<JSDocTemplateTag> option * parameters: ResizeArray<JSDocParameterTag> * ``type``: JSDocReturnTag option -> JSDocSignature
        abstract createJSDocTemplateTag: tagName: Identifier option * ``constraint``: JSDocTypeExpression option * typeParameters: ResizeArray<TypeParameterDeclaration> * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocTemplateTag
        abstract updateJSDocTemplateTag: node: JSDocTemplateTag * tagName: Identifier option * ``constraint``: JSDocTypeExpression option * typeParameters: ResizeArray<TypeParameterDeclaration> * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocTemplateTag
        abstract createJSDocTypedefTag: tagName: Identifier option * ?typeExpression: U2<JSDocTypeExpression, JSDocTypeLiteral> * ?fullName: U2<Identifier, JSDocNamespaceDeclaration> * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocTypedefTag
        abstract updateJSDocTypedefTag: node: JSDocTypedefTag * tagName: Identifier option * typeExpression: U2<JSDocTypeExpression, JSDocTypeLiteral> option * fullName: U2<Identifier, JSDocNamespaceDeclaration> option * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocTypedefTag
        abstract createJSDocParameterTag: tagName: Identifier option * name: EntityName * isBracketed: bool * ?typeExpression: JSDocTypeExpression * ?isNameFirst: bool * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocParameterTag
        abstract updateJSDocParameterTag: node: JSDocParameterTag * tagName: Identifier option * name: EntityName * isBracketed: bool * typeExpression: JSDocTypeExpression option * isNameFirst: bool * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocParameterTag
        abstract createJSDocPropertyTag: tagName: Identifier option * name: EntityName * isBracketed: bool * ?typeExpression: JSDocTypeExpression * ?isNameFirst: bool * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocPropertyTag
        abstract updateJSDocPropertyTag: node: JSDocPropertyTag * tagName: Identifier option * name: EntityName * isBracketed: bool * typeExpression: JSDocTypeExpression option * isNameFirst: bool * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocPropertyTag
        abstract createJSDocTypeTag: tagName: Identifier option * typeExpression: JSDocTypeExpression * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocTypeTag
        abstract updateJSDocTypeTag: node: JSDocTypeTag * tagName: Identifier option * typeExpression: JSDocTypeExpression * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocTypeTag
        abstract createJSDocSeeTag: tagName: Identifier option * nameExpression: JSDocNameReference option * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocSeeTag
        abstract updateJSDocSeeTag: node: JSDocSeeTag * tagName: Identifier option * nameExpression: JSDocNameReference option * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocSeeTag
        abstract createJSDocReturnTag: tagName: Identifier option * ?typeExpression: JSDocTypeExpression * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocReturnTag
        abstract updateJSDocReturnTag: node: JSDocReturnTag * tagName: Identifier option * typeExpression: JSDocTypeExpression option * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocReturnTag
        abstract createJSDocThisTag: tagName: Identifier option * typeExpression: JSDocTypeExpression * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocThisTag
        abstract updateJSDocThisTag: node: JSDocThisTag * tagName: Identifier option * typeExpression: JSDocTypeExpression option * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocThisTag
        abstract createJSDocEnumTag: tagName: Identifier option * typeExpression: JSDocTypeExpression * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocEnumTag
        abstract updateJSDocEnumTag: node: JSDocEnumTag * tagName: Identifier option * typeExpression: JSDocTypeExpression * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocEnumTag
        abstract createJSDocCallbackTag: tagName: Identifier option * typeExpression: JSDocSignature * ?fullName: U2<Identifier, JSDocNamespaceDeclaration> * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocCallbackTag
        abstract updateJSDocCallbackTag: node: JSDocCallbackTag * tagName: Identifier option * typeExpression: JSDocSignature * fullName: U2<Identifier, JSDocNamespaceDeclaration> option * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocCallbackTag
        abstract createJSDocOverloadTag: tagName: Identifier option * typeExpression: JSDocSignature * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocOverloadTag
        abstract updateJSDocOverloadTag: node: JSDocOverloadTag * tagName: Identifier option * typeExpression: JSDocSignature * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocOverloadTag
        abstract createJSDocAugmentsTag: tagName: Identifier option * className: obj * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocAugmentsTag
        abstract updateJSDocAugmentsTag: node: JSDocAugmentsTag * tagName: Identifier option * className: obj * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocAugmentsTag
        abstract createJSDocImplementsTag: tagName: Identifier option * className: obj * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocImplementsTag
        abstract updateJSDocImplementsTag: node: JSDocImplementsTag * tagName: Identifier option * className: obj * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocImplementsTag
        abstract createJSDocAuthorTag: tagName: Identifier option * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocAuthorTag
        abstract updateJSDocAuthorTag: node: JSDocAuthorTag * tagName: Identifier option * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocAuthorTag
        abstract createJSDocClassTag: tagName: Identifier option * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocClassTag
        abstract updateJSDocClassTag: node: JSDocClassTag * tagName: Identifier option * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocClassTag
        abstract createJSDocPublicTag: tagName: Identifier option * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocPublicTag
        abstract updateJSDocPublicTag: node: JSDocPublicTag * tagName: Identifier option * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocPublicTag
        abstract createJSDocPrivateTag: tagName: Identifier option * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocPrivateTag
        abstract updateJSDocPrivateTag: node: JSDocPrivateTag * tagName: Identifier option * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocPrivateTag
        abstract createJSDocProtectedTag: tagName: Identifier option * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocProtectedTag
        abstract updateJSDocProtectedTag: node: JSDocProtectedTag * tagName: Identifier option * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocProtectedTag
        abstract createJSDocReadonlyTag: tagName: Identifier option * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocReadonlyTag
        abstract updateJSDocReadonlyTag: node: JSDocReadonlyTag * tagName: Identifier option * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocReadonlyTag
        abstract createJSDocUnknownTag: tagName: Identifier * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocUnknownTag
        abstract updateJSDocUnknownTag: node: JSDocUnknownTag * tagName: Identifier * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocUnknownTag
        abstract createJSDocDeprecatedTag: tagName: Identifier option * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocDeprecatedTag
        abstract updateJSDocDeprecatedTag: node: JSDocDeprecatedTag * tagName: Identifier option * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocDeprecatedTag
        abstract createJSDocOverrideTag: tagName: Identifier option * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocOverrideTag
        abstract updateJSDocOverrideTag: node: JSDocOverrideTag * tagName: Identifier option * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocOverrideTag
        abstract createJSDocThrowsTag: tagName: Identifier * typeExpression: JSDocTypeExpression option * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocThrowsTag
        abstract updateJSDocThrowsTag: node: JSDocThrowsTag * tagName: Identifier option * typeExpression: JSDocTypeExpression option * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocThrowsTag
        abstract createJSDocSatisfiesTag: tagName: Identifier option * typeExpression: JSDocTypeExpression * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocSatisfiesTag
        abstract updateJSDocSatisfiesTag: node: JSDocSatisfiesTag * tagName: Identifier option * typeExpression: JSDocTypeExpression * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocSatisfiesTag
        abstract createJSDocImportTag: tagName: Identifier option * importClause: ImportClause option * moduleSpecifier: Expression * ?attributes: ImportAttributes * ?comment: U2<string, ResizeArray<JSDocComment>> -> JSDocImportTag
        abstract updateJSDocImportTag: node: JSDocImportTag * tagName: Identifier option * importClause: ImportClause option * moduleSpecifier: Expression * attributes: ImportAttributes option * comment: U2<string, ResizeArray<JSDocComment>> option -> JSDocImportTag
        abstract createJSDocText: text: string -> JSDocText
        abstract updateJSDocText: node: JSDocText * text: string -> JSDocText
        abstract createJSDocComment: ?comment: U2<string, ResizeArray<JSDocComment>> * ?tags: ResizeArray<JSDocTag> -> JSDoc
        abstract updateJSDocComment: node: JSDoc * comment: U2<string, ResizeArray<JSDocComment>> option * tags: ResizeArray<JSDocTag> option -> JSDoc
        abstract createJsxElement: openingElement: JsxOpeningElement * children: ResizeArray<JsxChild> * closingElement: JsxClosingElement -> JsxElement
        abstract updateJsxElement: node: JsxElement * openingElement: JsxOpeningElement * children: ResizeArray<JsxChild> * closingElement: JsxClosingElement -> JsxElement
        abstract createJsxSelfClosingElement: tagName: JsxTagNameExpression * typeArguments: ResizeArray<TypeNode> option * attributes: JsxAttributes -> JsxSelfClosingElement
        abstract updateJsxSelfClosingElement: node: JsxSelfClosingElement * tagName: JsxTagNameExpression * typeArguments: ResizeArray<TypeNode> option * attributes: JsxAttributes -> JsxSelfClosingElement
        abstract createJsxOpeningElement: tagName: JsxTagNameExpression * typeArguments: ResizeArray<TypeNode> option * attributes: JsxAttributes -> JsxOpeningElement
        abstract updateJsxOpeningElement: node: JsxOpeningElement * tagName: JsxTagNameExpression * typeArguments: ResizeArray<TypeNode> option * attributes: JsxAttributes -> JsxOpeningElement
        abstract createJsxClosingElement: tagName: JsxTagNameExpression -> JsxClosingElement
        abstract updateJsxClosingElement: node: JsxClosingElement * tagName: JsxTagNameExpression -> JsxClosingElement
        abstract createJsxFragment: openingFragment: JsxOpeningFragment * children: ResizeArray<JsxChild> * closingFragment: JsxClosingFragment -> JsxFragment
        abstract createJsxText: text: string * ?containsOnlyTriviaWhiteSpaces: bool -> JsxText
        abstract updateJsxText: node: JsxText * text: string * ?containsOnlyTriviaWhiteSpaces: bool -> JsxText
        abstract createJsxOpeningFragment: unit -> JsxOpeningFragment
        abstract createJsxJsxClosingFragment: unit -> JsxClosingFragment
        abstract updateJsxFragment: node: JsxFragment * openingFragment: JsxOpeningFragment * children: ResizeArray<JsxChild> * closingFragment: JsxClosingFragment -> JsxFragment
        abstract createJsxAttribute: name: JsxAttributeName * initializer: JsxAttributeValue option -> JsxAttribute
        abstract updateJsxAttribute: node: JsxAttribute * name: JsxAttributeName * initializer: JsxAttributeValue option -> JsxAttribute
        abstract createJsxAttributes: properties: ResizeArray<JsxAttributeLike> -> JsxAttributes
        abstract updateJsxAttributes: node: JsxAttributes * properties: ResizeArray<JsxAttributeLike> -> JsxAttributes
        abstract createJsxSpreadAttribute: expression: Expression -> JsxSpreadAttribute
        abstract updateJsxSpreadAttribute: node: JsxSpreadAttribute * expression: Expression -> JsxSpreadAttribute
        abstract createJsxExpression: dotDotDotToken: DotDotDotToken option * expression: Expression option -> JsxExpression
        abstract updateJsxExpression: node: JsxExpression * expression: Expression option -> JsxExpression
        abstract createJsxNamespacedName: ``namespace``: Identifier * name: Identifier -> JsxNamespacedName
        abstract updateJsxNamespacedName: node: JsxNamespacedName * ``namespace``: Identifier * name: Identifier -> JsxNamespacedName
        abstract createCaseClause: expression: Expression * statements: ResizeArray<Statement> -> CaseClause
        abstract updateCaseClause: node: CaseClause * expression: Expression * statements: ResizeArray<Statement> -> CaseClause
        abstract createDefaultClause: statements: ResizeArray<Statement> -> DefaultClause
        abstract updateDefaultClause: node: DefaultClause * statements: ResizeArray<Statement> -> DefaultClause
        abstract createHeritageClause: token: obj * types: ResizeArray<ExpressionWithTypeArguments> -> HeritageClause
        abstract updateHeritageClause: node: HeritageClause * types: ResizeArray<ExpressionWithTypeArguments> -> HeritageClause
        abstract createCatchClause: variableDeclaration: U3<string, BindingName, VariableDeclaration> option * block: Block -> CatchClause
        abstract updateCatchClause: node: CatchClause * variableDeclaration: VariableDeclaration option * block: Block -> CatchClause
        abstract createPropertyAssignment: name: U2<string, PropertyName> * initializer: Expression -> PropertyAssignment
        abstract updatePropertyAssignment: node: PropertyAssignment * name: PropertyName * initializer: Expression -> PropertyAssignment
        abstract createShorthandPropertyAssignment: name: U2<string, Identifier> * ?objectAssignmentInitializer: Expression -> ShorthandPropertyAssignment
        abstract updateShorthandPropertyAssignment: node: ShorthandPropertyAssignment * name: Identifier * objectAssignmentInitializer: Expression option -> ShorthandPropertyAssignment
        abstract createSpreadAssignment: expression: Expression -> SpreadAssignment
        abstract updateSpreadAssignment: node: SpreadAssignment * expression: Expression -> SpreadAssignment
        abstract createEnumMember: name: U2<string, PropertyName> * ?initializer: Expression -> EnumMember
        abstract updateEnumMember: node: EnumMember * name: PropertyName * initializer: Expression option -> EnumMember
        abstract createSourceFile: statements: ResizeArray<Statement> * endOfFileToken: EndOfFileToken * flags: NodeFlags -> SourceFile
        abstract updateSourceFile: node: SourceFile * statements: ResizeArray<Statement> * ?isDeclarationFile: bool * ?referencedFiles: ResizeArray<FileReference> * ?typeReferences: ResizeArray<FileReference> * ?hasNoDefaultLib: bool * ?libReferences: ResizeArray<FileReference> -> SourceFile
        abstract createRedirectedSourceFile: redirectInfo: RedirectInfo -> SourceFile
        abstract createSyntheticExpression: ``type``: Type * ?isSpread: bool * ?tupleNameSource: U2<ParameterDeclaration, NamedTupleMember> -> SyntheticExpression
        abstract createSyntaxList: children: ResizeArray<Node> -> SyntaxList
        abstract createNotEmittedStatement: original: Node -> NotEmittedStatement
        abstract createNotEmittedTypeElement: unit -> NotEmittedTypeElement
        abstract createPartiallyEmittedExpression: expression: Expression * ?original: Node -> PartiallyEmittedExpression
        abstract updatePartiallyEmittedExpression: node: PartiallyEmittedExpression * expression: Expression -> PartiallyEmittedExpression
        abstract createSyntheticReferenceExpression: expression: Expression * thisArg: Expression -> SyntheticReferenceExpression
        abstract updateSyntheticReferenceExpression: node: SyntheticReferenceExpression * expression: Expression * thisArg: Expression -> SyntheticReferenceExpression
        abstract createCommaListExpression: elements: ResizeArray<Expression> -> CommaListExpression
        abstract updateCommaListExpression: node: CommaListExpression * elements: ResizeArray<Expression> -> CommaListExpression
        abstract createBundle: sourceFiles: ResizeArray<SourceFile> -> Bundle
        abstract updateBundle: node: Bundle * sourceFiles: ResizeArray<SourceFile> -> Bundle
        abstract createComma: left: Expression * right: Expression -> BinaryExpression
        abstract createAssignment: left: U2<ObjectLiteralExpression, ArrayLiteralExpression> * right: Expression -> DestructuringAssignment
        abstract createAssignment: left: Expression * right: Expression -> AssignmentExpression<EqualsToken>
        abstract createLogicalOr: left: Expression * right: Expression -> BinaryExpression
        abstract createLogicalAnd: left: Expression * right: Expression -> BinaryExpression
        abstract createBitwiseOr: left: Expression * right: Expression -> BinaryExpression
        abstract createBitwiseXor: left: Expression * right: Expression -> BinaryExpression
        abstract createBitwiseAnd: left: Expression * right: Expression -> BinaryExpression
        abstract createStrictEquality: left: Expression * right: Expression -> BinaryExpression
        abstract createStrictInequality: left: Expression * right: Expression -> BinaryExpression
        abstract createEquality: left: Expression * right: Expression -> BinaryExpression
        abstract createInequality: left: Expression * right: Expression -> BinaryExpression
        abstract createLessThan: left: Expression * right: Expression -> BinaryExpression
        abstract createLessThanEquals: left: Expression * right: Expression -> BinaryExpression
        abstract createGreaterThan: left: Expression * right: Expression -> BinaryExpression
        abstract createGreaterThanEquals: left: Expression * right: Expression -> BinaryExpression
        abstract createLeftShift: left: Expression * right: Expression -> BinaryExpression
        abstract createRightShift: left: Expression * right: Expression -> BinaryExpression
        abstract createUnsignedRightShift: left: Expression * right: Expression -> BinaryExpression
        abstract createAdd: left: Expression * right: Expression -> BinaryExpression
        abstract createSubtract: left: Expression * right: Expression -> BinaryExpression
        abstract createMultiply: left: Expression * right: Expression -> BinaryExpression
        abstract createDivide: left: Expression * right: Expression -> BinaryExpression
        abstract createModulo: left: Expression * right: Expression -> BinaryExpression
        abstract createExponent: left: Expression * right: Expression -> BinaryExpression
        abstract createPrefixPlus: operand: Expression -> PrefixUnaryExpression
        abstract createPrefixMinus: operand: Expression -> PrefixUnaryExpression
        abstract createPrefixIncrement: operand: Expression -> PrefixUnaryExpression
        abstract createPrefixDecrement: operand: Expression -> PrefixUnaryExpression
        abstract createBitwiseNot: operand: Expression -> PrefixUnaryExpression
        abstract createLogicalNot: operand: Expression -> PrefixUnaryExpression
        abstract createPostfixIncrement: operand: Expression -> PostfixUnaryExpression
        abstract createPostfixDecrement: operand: Expression -> PostfixUnaryExpression
        abstract createImmediatelyInvokedFunctionExpression: statements: ResizeArray<Statement> -> CallExpression
        abstract createImmediatelyInvokedFunctionExpression: statements: ResizeArray<Statement> * param: ParameterDeclaration * paramValue: Expression -> CallExpression
        abstract createImmediatelyInvokedArrowFunction: statements: ResizeArray<Statement> -> ImmediatelyInvokedArrowFunction
        abstract createImmediatelyInvokedArrowFunction: statements: ResizeArray<Statement> * param: ParameterDeclaration * paramValue: Expression -> ImmediatelyInvokedArrowFunction
        abstract createVoidZero: unit -> VoidExpression
        abstract createExportDefault: expression: Expression -> ExportAssignment
        abstract createExternalModuleExport: exportName: Identifier -> ExportDeclaration
        abstract createTypeCheck: value: Expression * tag: TypeOfTag -> Expression
        abstract createIsNotTypeCheck: value: Expression * tag: TypeOfTag -> Expression
        abstract createMethodCall: object: Expression * methodName: U2<string, Identifier> * argumentsList: ResizeArray<Expression> -> CallExpression
        abstract createGlobalMethodCall: globalObjectName: string * globalMethodName: string * argumentsList: ResizeArray<Expression> -> CallExpression
        abstract createFunctionBindCall: target: Expression * thisArg: Expression * argumentsList: ResizeArray<Expression> -> CallExpression
        abstract createFunctionCallCall: target: Expression * thisArg: Expression * argumentsList: ResizeArray<Expression> -> CallExpression
        abstract createFunctionApplyCall: target: Expression * thisArg: Expression * argumentsExpression: Expression -> CallExpression
        abstract createObjectDefinePropertyCall: target: Expression * propertyName: U2<string, Expression> * attributes: Expression -> CallExpression
        abstract createObjectGetOwnPropertyDescriptorCall: target: Expression * propertyName: U2<string, Expression> -> CallExpression
        abstract createReflectGetCall: target: Expression * propertyKey: Expression * ?receiver: Expression -> CallExpression
        abstract createReflectSetCall: target: Expression * propertyKey: Expression * value: Expression * ?receiver: Expression -> CallExpression
        abstract createPropertyDescriptor: attributes: PropertyDescriptorAttributes * ?singleLine: bool -> ObjectLiteralExpression
        abstract createArraySliceCall: array: Expression * ?start: U2<float, Expression> -> CallExpression
        abstract createArrayConcatCall: array: Expression * values: ResizeArray<Expression> -> CallExpression
        abstract createCallBinding: expression: Expression * recordTempVariable: (Identifier -> unit) * ?languageVersion: ScriptTarget * ?cacheIdentifiers: bool -> CallBinding
        /// <summary>
        /// Wraps an expression that cannot be an assignment target in an expression that can be.
        ///
        /// Given a <c>paramName</c> of <c>_a</c>:
        /// <code>
        /// Reflect.set(obj, "x", _a)
        /// </code>
        /// Becomes
        /// <code lang="ts">
        /// ({ set value(_a) { Reflect.set(obj, "x", _a); } }).value
        /// </code>
        /// </summary>
        /// <param name="paramName" />
        /// <param name="expression" />
        abstract createAssignmentTargetWrapper: paramName: Identifier * expression: Expression -> PropertyAccessExpression
        abstract inlineExpressions: expressions: ResizeArray<Expression> -> Expression
        /// <summary>
        /// Gets the internal name of a declaration. This is primarily used for declarations that can be
        /// referred to by name in the body of an ES5 class function body. An internal name will *never*
        /// be prefixed with an module or namespace export modifier like "exports." when emitted as an
        /// expression. An internal name will also *never* be renamed due to a collision with a block
        /// scoped variable.
        /// </summary>
        /// <param name="node">The declaration.</param>
        /// <param name="allowComments">A value indicating whether comments may be emitted for the name.</param>
        /// <param name="allowSourceMaps">A value indicating whether source maps may be emitted for the name.</param>
        abstract getInternalName: node: Declaration * ?allowComments: bool * ?allowSourceMaps: bool -> Identifier
        /// <summary>
        /// Gets the local name of a declaration. This is primarily used for declarations that can be
        /// referred to by name in the declaration's immediate scope (classes, enums, namespaces). A
        /// local name will *never* be prefixed with an module or namespace export modifier like
        /// "exports." when emitted as an expression.
        /// </summary>
        /// <param name="node">The declaration.</param>
        /// <param name="allowComments">A value indicating whether comments may be emitted for the name.</param>
        /// <param name="allowSourceMaps">A value indicating whether source maps may be emitted for the name.</param>
        /// <param name="ignoreAssignedName">Indicates that the assigned name of a declaration shouldn't be considered.</param>
        abstract getLocalName: node: Declaration * ?allowComments: bool * ?allowSourceMaps: bool * ?ignoreAssignedName: bool -> Identifier
        /// <summary>
        /// Gets the export name of a declaration. This is primarily used for declarations that can be
        /// referred to by name in the declaration's immediate scope (classes, enums, namespaces). An
        /// export name will *always* be prefixed with a module or namespace export modifier like
        /// <c>"exports."</c> when emitted as an expression if the name points to an exported symbol.
        /// </summary>
        /// <param name="node">The declaration.</param>
        /// <param name="allowComments">A value indicating whether comments may be emitted for the name.</param>
        /// <param name="allowSourceMaps">A value indicating whether source maps may be emitted for the name.</param>
        abstract getExportName: node: Declaration * ?allowComments: bool * ?allowSourceMaps: bool -> Identifier
        /// <summary>Gets the name of a declaration for use in declarations.</summary>
        /// <param name="node">The declaration.</param>
        /// <param name="allowComments">A value indicating whether comments may be emitted for the name.</param>
        /// <param name="allowSourceMaps">A value indicating whether source maps may be emitted for the name.</param>
        abstract getDeclarationName: node: Declaration option * ?allowComments: bool * ?allowSourceMaps: bool -> Identifier
        /// <summary>Gets a namespace-qualified name for use in expressions.</summary>
        /// <param name="ns">The namespace identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="allowComments">A value indicating whether comments may be emitted for the name.</param>
        /// <param name="allowSourceMaps">A value indicating whether source maps may be emitted for the name.</param>
        abstract getNamespaceMemberName: ns: Identifier * name: Identifier * ?allowComments: bool * ?allowSourceMaps: bool -> PropertyAccessExpression
        /// <summary>
        /// Gets the exported name of a declaration for use in expressions.
        ///
        /// An exported name will *always* be prefixed with an module or namespace export modifier like
        /// "exports." if the name points to an exported symbol.
        /// </summary>
        /// <param name="ns">The namespace identifier.</param>
        /// <param name="node">The declaration.</param>
        /// <param name="allowComments">A value indicating whether comments may be emitted for the name.</param>
        /// <param name="allowSourceMaps">A value indicating whether source maps may be emitted for the name.</param>
        abstract getExternalModuleOrNamespaceExportName: ns: Identifier option * node: Declaration * ?allowComments: bool * ?allowSourceMaps: bool -> U2<Identifier, PropertyAccessExpression>
        abstract restoreOuterExpressions: outerExpression: Expression option * innerExpression: Expression * ?kinds: OuterExpressionKinds -> Expression
        abstract restoreEnclosingLabel: node: Statement * outermostLabeledStatement: LabeledStatement option * ?afterRestoreLabelCallback: (LabeledStatement -> unit) -> Statement
        abstract createUseStrictPrologue: unit -> PrologueDirective
        /// <summary>Copies any necessary standard and custom prologue-directives into target array.</summary>
        /// <param name="source">origin statements array</param>
        /// <param name="target">result statements array</param>
        /// <param name="ensureUseStrict">boolean determining whether the function need to add prologue-directives</param>
        /// <param name="visitor">Optional callback used to visit any custom prologue directives.</param>
        abstract copyPrologue: source: ResizeArray<Statement> * target: ResizeArray<Statement> * ?ensureUseStrict: bool * ?visitor: (Node -> VisitResult<Node option>) -> float
        /// <summary>Copies only the standard (string-expression) prologue-directives into the target statement-array.</summary>
        /// <param name="source">origin statements array</param>
        /// <param name="target">result statements array</param>
        /// <param name="statementOffset">The offset at which to begin the copy.</param>
        /// <param name="ensureUseStrict">boolean determining whether the function need to add prologue-directives</param>
        abstract copyStandardPrologue: source: ResizeArray<Statement> * target: ResizeArray<Statement> * statementOffset: float option * ?ensureUseStrict: bool -> float
        /// <summary>Copies only the custom prologue-directives into target statement-array.</summary>
        /// <param name="source">origin statements array</param>
        /// <param name="target">result statements array</param>
        /// <param name="statementOffset">The offset at which to begin the copy.</param>
        /// <param name="visitor">Optional callback used to visit any custom prologue directives.</param>
        abstract copyCustomPrologue: source: ResizeArray<Statement> * target: ResizeArray<Statement> * statementOffset: float * ?visitor: (Node -> VisitResult<Node option>) * ?filter: (Statement -> bool) -> float
        abstract copyCustomPrologue: source: ResizeArray<Statement> * target: ResizeArray<Statement> * statementOffset: float option * ?visitor: (Node -> VisitResult<Node option>) * ?filter: (Statement -> bool) -> float option
        abstract ensureUseStrict: statements: ResizeArray<Statement> -> ResizeArray<Statement>
        abstract liftToBlock: nodes: ResizeArray<Node> -> Statement
        /// <summary>Merges generated lexical declarations into a new statement list.</summary>
        abstract mergeLexicalEnvironment: statements: ResizeArray<Statement> * declarations: ResizeArray<Statement> option -> ResizeArray<Statement>
        /// <summary>
        /// Creates a shallow, memberwise clone of a node.
        /// - The result will have its <c>original</c> pointer set to <c>node</c>.
        /// - The result will have its <c>pos</c> and <c>end</c> set to <c>-1</c>.
        /// - *DO NOT USE THIS* if a more appropriate function is available.
        /// </summary>
        abstract cloneNode: node: 'T -> 'T
        /// Updates a node that may contain modifiers, replacing only the modifiers of the node.
        abstract replaceModifiers: node: 'T * modifiers: U2<ResizeArray<Modifier>, ModifierFlags> option -> 'T when 'T :> HasModifiers
        /// Updates a node that may contain decorators or modifiers, replacing only the decorators and modifiers of the node.
        abstract replaceDecoratorsAndModifiers: node: 'T * modifiers: ResizeArray<ModifierLike> option -> 'T when 'T :> HasModifiers and 'T :> HasDecorators
        /// Updates a node that contains a property name, replacing only the name of the node.
        abstract replacePropertyName: node: 'T * name: obj -> 'T

    type [<RequireQualifiedAccess>] LexicalEnvironmentFlags =
        | None = 0
        | InParameters = 1
        | VariablesHoistedInParameters = 2

    type [<AllowNullLiteral>] CoreTransformationContext =
        abstract factory: NodeFactory
        /// Gets the compiler options supplied to the transformer.
        abstract getCompilerOptions: unit -> CompilerOptions
        /// Starts a new lexical environment.
        abstract startLexicalEnvironment: unit -> unit
        abstract setLexicalEnvironmentFlags: flags: LexicalEnvironmentFlags * value: bool -> unit
        abstract getLexicalEnvironmentFlags: unit -> LexicalEnvironmentFlags
        /// Suspends the current lexical environment, usually after visiting a parameter list.
        abstract suspendLexicalEnvironment: unit -> unit
        /// Resumes a suspended lexical environment, usually before visiting a function body.
        abstract resumeLexicalEnvironment: unit -> unit
        /// Ends a lexical environment, returning any declarations.
        abstract endLexicalEnvironment: unit -> ResizeArray<Statement> option
        /// Hoists a function declaration to the containing scope.
        abstract hoistFunctionDeclaration: node: FunctionDeclaration -> unit
        /// Hoists a variable declaration to the containing scope.
        abstract hoistVariableDeclaration: node: Identifier -> unit
        abstract startBlockScope: unit -> unit
        abstract endBlockScope: unit -> ResizeArray<Statement> option
        abstract addBlockScopedVariable: node: Identifier -> unit
        /// <summary>Adds an initialization statement to the top of the lexical environment.</summary>
        abstract addInitializationStatement: node: Statement -> unit

    type [<AllowNullLiteral>] TransformationContext =
        inherit CoreTransformationContext
        abstract getEmitResolver: unit -> EmitResolver
        abstract getEmitHost: unit -> EmitHost
        abstract getEmitHelperFactory: unit -> EmitHelperFactory
        /// Records a request for a non-scoped emit helper in the current context.
        abstract requestEmitHelper: helper: EmitHelper -> unit
        /// Gets and resets the requested non-scoped emit helpers.
        abstract readEmitHelpers: unit -> ResizeArray<EmitHelper> option
        /// Enables expression substitutions in the pretty printer for the provided SyntaxKind.
        abstract enableSubstitution: kind: SyntaxKind -> unit
        /// Determines whether expression substitutions are enabled for the provided node.
        abstract isSubstitutionEnabled: node: Node -> bool
        /// <summary>
        /// Hook used by transformers to substitute expressions just before they
        /// are emitted by the pretty printer.
        ///
        /// NOTE: Transformation hooks should only be modified during <c>Transformer</c> initialization,
        /// before returning the <c>NodeTransformer</c> callback.
        /// </summary>
        abstract onSubstituteNode: (EmitHint -> Node -> Node) with get, set
        /// Enables before/after emit notifications in the pretty printer for the provided
        /// SyntaxKind.
        abstract enableEmitNotification: kind: SyntaxKind -> unit
        /// Determines whether before/after emit notifications should be raised in the pretty
        /// printer when it emits a node.
        abstract isEmitNotificationEnabled: node: Node -> bool
        /// <summary>
        /// Hook used to allow transformers to capture state before or after
        /// the printer emits a node.
        ///
        /// NOTE: Transformation hooks should only be modified during <c>Transformer</c> initialization,
        /// before returning the <c>NodeTransformer</c> callback.
        /// </summary>
        abstract onEmitNode: (EmitHint -> Node -> (EmitHint -> Node -> unit) -> unit) with get, set
        abstract addDiagnostic: diag: DiagnosticWithLocation -> unit

    type [<AllowNullLiteral>] TransformationResult<'T when 'T :> Node> =
        /// Gets the transformed source files.
        abstract transformed: ResizeArray<'T> with get, set
        /// Gets diagnostics for the transformation.
        abstract diagnostics: ResizeArray<DiagnosticWithLocation> option with get, set
        /// <summary>Gets a substitute for a node, if one is available; otherwise, returns the original node.</summary>
        /// <param name="hint">A hint as to the intended usage of the node.</param>
        /// <param name="node">The node to substitute.</param>
        abstract substituteNode: hint: EmitHint * node: Node -> Node
        /// <summary>Emits a node with possible notification.</summary>
        /// <param name="hint">A hint as to the intended usage of the node.</param>
        /// <param name="node">The node to emit.</param>
        /// <param name="emitCallback">A callback used to emit the node.</param>
        abstract emitNodeWithNotification: hint: EmitHint * node: Node * emitCallback: (EmitHint -> Node -> unit) -> unit
        /// <summary>Indicates if a given node needs an emit notification</summary>
        /// <param name="node">The node to emit.</param>
        abstract isEmitNotificationEnabled: node: Node -> bool
        /// Clean up EmitNode entries on any parse-tree nodes.
        abstract dispose: unit -> unit

    /// <summary>
    /// A function that is used to initialize and return a <c>Transformer</c> callback, which in turn
    /// will be used to transform one or more nodes.
    /// </summary>
    type [<AllowNullLiteral>] TransformerFactory<'T when 'T :> Node> =
        /// <summary>
        /// A function that is used to initialize and return a <c>Transformer</c> callback, which in turn
        /// will be used to transform one or more nodes.
        /// </summary>
        [<Emit("$0($1...)")>] abstract Invoke: context: TransformationContext -> Transformer<'T>

    /// A function that transforms a node.
    type [<AllowNullLiteral>] Transformer<'T when 'T :> Node> =
        /// A function that transforms a node.
        [<Emit("$0($1...)")>] abstract Invoke: node: 'T -> 'T

    /// A function that accepts and possibly transforms a node.
    type Visitor =
        Visitor<Node, Node option>

    /// A function that accepts and possibly transforms a node.
    type Visitor<'TIn when 'TIn :> Node> =
        Visitor<'TIn, 'TIn option>

    /// A function that accepts and possibly transforms a node.
    type [<AllowNullLiteral>] Visitor<'TIn, 'TOut when 'TIn :> Node> =
        /// A function that accepts and possibly transforms a node.
        [<Emit("$0($1...)")>] abstract Invoke: node: 'TIn -> VisitResult<'TOut>

    /// <summary>
    /// A function that walks a node using the given visitor, lifting node arrays into single nodes,
    /// returning an node which satisfies the test.
    ///
    /// - If the input node is undefined, then the output is undefined.
    /// - If the visitor returns undefined, then the output is undefined.
    /// - If the output node is not undefined, then it will satisfy the test function.
    /// - In order to obtain a return type that is more specific than <c>Node</c>, a test
    ///   function _must_ be provided, and that function must be a type predicate.
    ///
    /// For the canonical implementation of this type,
    /// </summary>
    /// <seealso cref="visitNode">.</seealso>
    type [<AllowNullLiteral>] NodeVisitor =
        [<Emit("$0($1...)")>] abstract Invoke: node: 'TIn * visitor: Visitor<'TIn, 'TVisited> * test: (Node -> bool) * ?lift: (ResizeArray<Node> -> Node) -> U2<'TOut, obj> when 'TOut :> Node
        [<Emit("$0($1...)")>] abstract Invoke: node: 'TIn * visitor: Visitor<'TIn, 'TVisited> * ?test: (Node -> bool) * ?lift: (ResizeArray<Node> -> Node) -> U2<Node, obj>

    /// <summary>
    /// A function that walks a node array using the given visitor, returning an array whose contents satisfy the test.
    ///
    /// - If the input node array is undefined, the output is undefined.
    /// - If the visitor can return undefined, the node it visits in the array will be reused.
    /// - If the output node array is not undefined, then its contents will satisfy the test.
    /// - In order to obtain a return type that is more specific than <c>NodeArray&lt;Node&gt;</c>, a test
    ///   function _must_ be provided, and that function must be a type predicate.
    ///
    /// For the canonical implementation of this type,
    /// </summary>
    /// <seealso cref="visitNodes">.</seealso>
    type [<AllowNullLiteral>] NodesVisitor =
        [<Emit("$0($1...)")>] abstract Invoke: nodes: 'TInArray * visitor: Visitor<'TIn, Node option> * test: (Node -> bool) * ?start: float * ?count: float -> U2<ResizeArray<'TOut>, obj> when 'TIn :> Node and 'TOut :> Node
        [<Emit("$0($1...)")>] abstract Invoke: nodes: 'TInArray * visitor: Visitor<'TIn, Node option> * ?test: (Node -> bool) * ?start: float * ?count: float -> U2<ResizeArray<Node>, obj> when 'TIn :> Node

    type VisitResult<'T> =
        U2<'T, ResizeArray<Node>>

    type [<AllowNullLiteral>] Printer =
        /// <summary>Print a node and its subtree as-is, without any emit transformations.</summary>
        /// <param name="hint">
        /// A value indicating the purpose of a node. This is primarily used to
        /// distinguish between an <c>Identifier</c> used in an expression position, versus an
        /// <c>Identifier</c> used as an <c>IdentifierName</c> as part of a declaration. For most nodes you
        /// should just pass <c>Unspecified</c>.
        /// </param>
        /// <param name="node">
        /// The node to print. The node and its subtree are printed as-is, without any
        /// emit transformations.
        /// </param>
        /// <param name="sourceFile">
        /// A source file that provides context for the node. The source text of
        /// the file is used to emit the original source content for literals and identifiers, while
        /// the identifiers of the source file are used when generating unique names to avoid
        /// collisions.
        /// </param>
        abstract printNode: hint: EmitHint * node: Node * sourceFile: SourceFile -> string
        /// Prints a list of nodes using the given format flags
        abstract printList: format: ListFormat * list: ResizeArray<'T> * sourceFile: SourceFile -> string when 'T :> Node
        /// Prints a source file as-is, without any emit transformations.
        abstract printFile: sourceFile: SourceFile -> string
        /// Prints a bundle of source files as-is, without any emit transformations.
        abstract printBundle: bundle: Bundle -> string
        abstract writeNode: hint: EmitHint * node: Node * sourceFile: SourceFile option * writer: EmitTextWriter -> unit
        abstract writeList: format: ListFormat * list: ResizeArray<'T> option * sourceFile: SourceFile option * writer: EmitTextWriter -> unit when 'T :> Node
        abstract writeFile: sourceFile: SourceFile * writer: EmitTextWriter * sourceMapGenerator: SourceMapGenerator option -> unit
        abstract writeBundle: bundle: Bundle * writer: EmitTextWriter * sourceMapGenerator: SourceMapGenerator option -> unit

    type [<AllowNullLiteral>] BuildInfo =
        abstract version: string with get, set

    type [<AllowNullLiteral>] BuildInfoFileVersionMap =
        abstract fileInfos: Map<Path, string> with get, set
        abstract roots: Map<Path, Path option> with get, set

    type [<AllowNullLiteral>] PrintHandlers =
        /// A hook used by the Printer when generating unique names to avoid collisions with
        /// globally defined names that exist outside of the current source file.
        abstract hasGlobalName: name: string -> bool
        /// <summary>
        /// A hook used by the Printer to provide notifications prior to emitting a node. A
        /// compatible implementation **must** invoke <c>emitCallback</c> with the provided <c>hint</c> and
        /// <c>node</c> values.
        /// </summary>
        /// <param name="hint">A hint indicating the intended purpose of the node.</param>
        /// <param name="node">The node to emit.</param>
        /// <param name="emitCallback">A callback that, when invoked, will emit the node.</param>
        /// <example>
        /// <code lang="ts">
        /// var printer = createPrinter(printerOptions, {
        ///   onEmitNode(hint, node, emitCallback) {
        ///     // set up or track state prior to emitting the node...
        ///     emitCallback(hint, node);
        ///     // restore state after emitting the node...
        ///   }
        /// });
        /// </code>
        /// </example>
        abstract onEmitNode: hint: EmitHint * node: Node * emitCallback: (EmitHint -> Node -> unit) -> unit
        /// <summary>A hook used to check if an emit notification is required for a node.</summary>
        /// <param name="node">The node to emit.</param>
        abstract isEmitNotificationEnabled: node: Node -> bool
        /// <summary>
        /// A hook used by the Printer to perform just-in-time substitution of a node. This is
        /// primarily used by node transformations that need to substitute one node for another,
        /// such as replacing <c>myExportedVar</c> with <c>exports.myExportedVar</c>.
        /// </summary>
        /// <param name="hint">A hint indicating the intended purpose of the node.</param>
        /// <param name="node">The node to emit.</param>
        /// <example>
        /// <code lang="ts">
        /// var printer = createPrinter(printerOptions, {
        ///   substituteNode(hint, node) {
        ///     // perform substitution if necessary...
        ///     return node;
        ///   }
        /// });
        /// </code>
        /// </example>
        abstract substituteNode: hint: EmitHint * node: Node -> Node
        abstract onEmitSourceMapOfNode: (EmitHint -> Node -> (EmitHint -> Node -> unit) -> unit) option with get, set
        abstract onEmitSourceMapOfToken: (Node option -> SyntaxKind -> (string -> unit) -> float -> (SyntaxKind -> (string -> unit) -> float -> float) -> float) option with get, set
        abstract onEmitSourceMapOfPosition: (float -> unit) option with get, set
        abstract onSetSourceFile: (SourceFile -> unit) option with get, set
        abstract onBeforeEmitNode: (Node option -> unit) option with get, set
        abstract onAfterEmitNode: (Node option -> unit) option with get, set
        abstract onBeforeEmitNodeArray: (ResizeArray<obj option> option -> unit) option with get, set
        abstract onAfterEmitNodeArray: (ResizeArray<obj option> option -> unit) option with get, set
        abstract onBeforeEmitToken: (Node -> unit) option with get, set
        abstract onAfterEmitToken: (Node -> unit) option with get, set

    type [<AllowNullLiteral>] PrinterOptions =
        abstract removeComments: bool option with get, set
        abstract newLine: NewLineKind option with get, set
        abstract omitTrailingSemicolon: bool option with get, set
        abstract noEmitHelpers: bool option with get, set
        abstract ``module``: obj option with get, set
        abstract moduleResolution: obj option with get, set
        abstract target: obj option with get, set
        abstract sourceMap: bool option with get, set
        abstract inlineSourceMap: bool option with get, set
        abstract inlineSources: bool option with get, set
        abstract omitBraceSourceMapPositions: bool option with get, set
        abstract extendedDiagnostics: bool option with get, set
        abstract onlyPrintJsDocStyle: bool option with get, set
        abstract neverAsciiEscape: bool option with get, set
        abstract stripInternal: bool option with get, set
        abstract preserveSourceNewlines: bool option with get, set
        abstract terminateUnterminatedLiterals: bool option with get, set

    type [<AllowNullLiteral>] RawSourceMap =
        abstract version: int with get, set
        abstract file: string with get, set
        abstract sourceRoot: string option with get, set
        abstract sources: ResizeArray<string> with get, set
        abstract sourcesContent: ResizeArray<string option> option with get, set
        abstract mappings: string with get, set
        abstract names: ResizeArray<string> option with get, set

    /// <summary>Generates a source map.</summary>
    type [<AllowNullLiteral>] SourceMapGenerator =
        abstract getSources: unit -> ResizeArray<string>
        /// Adds a source to the source map.
        abstract addSource: fileName: string -> float
        /// Set the content for a source.
        abstract setSourceContent: sourceIndex: float * content: string option -> unit
        /// Adds a name.
        abstract addName: name: string -> float
        /// Adds a mapping without source information.
        abstract addMapping: generatedLine: float * generatedCharacter: float -> unit
        /// Adds a mapping with source information.
        abstract addMapping: generatedLine: float * generatedCharacter: float * sourceIndex: float * sourceLine: float * sourceCharacter: float * ?nameIndex: float -> unit
        /// Appends a source map.
        abstract appendSourceMap: generatedLine: float * generatedCharacter: float * sourceMap: RawSourceMap * sourceMapPath: string * ?start: LineAndCharacter * ?``end``: LineAndCharacter -> unit
        /// <summary>Gets the source map as a <c>RawSourceMap</c> object.</summary>
        abstract toJSON: unit -> RawSourceMap
        /// Gets the string representation of the source map.
        abstract toString: unit -> string

    type [<AllowNullLiteral>] DocumentPositionMapperHost =
        abstract getSourceFileLike: fileName: string -> SourceFileLike option
        abstract getCanonicalFileName: path: string -> string
        abstract log: text: string -> unit

    /// <summary>Maps positions between source and generated files.</summary>
    type [<AllowNullLiteral>] DocumentPositionMapper =
        abstract getSourcePosition: input: DocumentPosition -> DocumentPosition
        abstract getGeneratedPosition: input: DocumentPosition -> DocumentPosition

    type [<AllowNullLiteral>] DocumentPosition =
        abstract fileName: string with get, set
        abstract pos: float with get, set

    type [<AllowNullLiteral>] EmitTextWriter =
        inherit SymbolWriter
        abstract write: s: string -> unit
        abstract writeTrailingSemicolon: text: string -> unit
        abstract writeComment: text: string -> unit
        abstract getText: unit -> string
        abstract rawWrite: s: string -> unit
        abstract writeLiteral: s: string -> unit
        abstract getTextPos: unit -> float
        abstract getLine: unit -> float
        abstract getColumn: unit -> float
        abstract getIndent: unit -> float
        abstract isAtStartOfLine: unit -> bool
        abstract hasTrailingComment: unit -> bool
        abstract hasTrailingWhitespace: unit -> bool
        abstract nonEscapingWrite: text: string -> unit

    type [<AllowNullLiteral>] GetEffectiveTypeRootsHost =
        abstract getCurrentDirectory: unit -> string

    type [<AllowNullLiteral>] HasCurrentDirectory =
        abstract getCurrentDirectory: unit -> string

    type [<AllowNullLiteral>] ModuleSpecifierResolutionHost =
        abstract useCaseSensitiveFileNames: unit -> bool
        abstract fileExists: path: string -> bool
        abstract getCurrentDirectory: unit -> string
        abstract directoryExists: path: string -> bool
        abstract readFile: path: string -> string option
        abstract realpath: path: string -> string
        abstract getSymlinkCache: unit -> SymlinkCache
        abstract getModuleSpecifierCache: unit -> ModuleSpecifierCache
        abstract getPackageJsonInfoCache: unit -> PackageJsonInfoCache option
        abstract getGlobalTypingsCacheLocation: unit -> string option
        abstract getNearestAncestorDirectoryWithPackageJson: fileName: string * ?rootDir: string -> string option
        abstract redirectTargetsMap: RedirectTargetsMap
        abstract getRedirectFromSourceFile: fileName: string -> ResolvedRefAndOutputDts option
        abstract isSourceOfProjectReferenceRedirect: fileName: string -> bool
        abstract getFileIncludeReasons: unit -> MultiMap<Path, FileIncludeReason>
        abstract getCommonSourceDirectory: unit -> string
        abstract getDefaultResolutionModeForFile: sourceFile: SourceFile -> ResolutionMode
        abstract getModeForResolutionAtIndex: file: SourceFile * index: float -> ResolutionMode
        abstract getModuleResolutionCache: unit -> ModuleResolutionCache option
        abstract trace: s: string -> unit

    type [<AllowNullLiteral>] ModulePath =
        abstract path: string with get, set
        abstract isInNodeModules: bool with get, set
        abstract isRedirect: bool with get, set

    type [<AllowNullLiteral>] ResolvedModuleSpecifierInfo =
        abstract kind: ResolvedModuleSpecifierInfoKind with get, set
        abstract modulePaths: ResizeArray<ModulePath> option with get, set
        abstract packageName: string option with get, set
        abstract moduleSpecifiers: ResizeArray<string> option with get, set
        abstract isBlockedByPackageJsonDependencies: bool option with get, set

    type [<AllowNullLiteral>] ModuleSpecifierOptions =
        abstract overrideImportMode: ResolutionMode option with get, set

    type [<AllowNullLiteral>] ModuleSpecifierCache =
        abstract get: fromFileName: Path * toFileName: Path * preferences: UserPreferences * options: ModuleSpecifierOptions -> obj option
        abstract set: fromFileName: Path * toFileName: Path * preferences: UserPreferences * options: ModuleSpecifierOptions * kind: obj * modulePaths: ResizeArray<ModulePath> * moduleSpecifiers: ResizeArray<string> -> unit
        abstract setBlockedByPackageJsonDependencies: fromFileName: Path * toFileName: Path * preferences: UserPreferences * options: ModuleSpecifierOptions * packageName: string option * isBlockedByPackageJsonDependencies: bool -> unit
        abstract setModulePaths: fromFileName: Path * toFileName: Path * preferences: UserPreferences * options: ModuleSpecifierOptions * modulePaths: ResizeArray<ModulePath> -> unit
        abstract clear: unit -> unit
        abstract count: unit -> float

    type [<AllowNullLiteral>] SymbolTracker =
        abstract trackSymbol: symbol: Symbol * enclosingDeclaration: Node option * meaning: SymbolFlags -> bool
        abstract reportInaccessibleThisError: unit -> unit
        abstract reportPrivateInBaseOfClassExpression: propertyName: string -> unit
        abstract reportInaccessibleUniqueSymbolError: unit -> unit
        abstract reportCyclicStructureError: unit -> unit
        abstract reportLikelyUnsafeImportRequiredError: specifier: string -> unit
        abstract reportTruncationError: unit -> unit
        abstract moduleResolverHost: obj option with get, set
        abstract reportNonlocalAugmentation: containingFile: SourceFile * parentSymbol: Symbol * augmentingSymbol: Symbol -> unit
        abstract reportNonSerializableProperty: propertyName: string -> unit
        abstract reportInferenceFallback: node: Node -> unit
        abstract pushErrorFallbackNode: node: Declaration option -> unit
        abstract popErrorFallbackNode: unit -> unit

    type [<AllowNullLiteral>] TextSpan =
        abstract start: float with get, set
        abstract length: float with get, set

    type [<AllowNullLiteral>] TextChangeRange =
        abstract span: TextSpan with get, set
        abstract newLength: float with get, set

    type [<AllowNullLiteral>] ErrorOutputContainer =
        abstract errors: ResizeArray<Diagnostic> option with get, set
        abstract skipLogging: bool option with get, set

    type [<AllowNullLiteral>] DiagnosticCollection =
        abstract add: diagnostic: Diagnostic -> unit
        abstract lookup: diagnostic: Diagnostic -> Diagnostic option
        abstract getGlobalDiagnostics: unit -> ResizeArray<Diagnostic>
        abstract getDiagnostics: unit -> ResizeArray<Diagnostic>
        abstract getDiagnostics: fileName: string -> ResizeArray<DiagnosticWithLocation>

    type [<AllowNullLiteral>] SyntaxList =
        inherit Node
        abstract kind: SyntaxKind with get, set
        abstract _children: ResizeArray<Node> with get, set

    type [<RequireQualifiedAccess>] ListFormat =
        | None = 0
        | SingleLine = 0
        | MultiLine = 1
        | PreserveLines = 2
        | LinesMask = 3
        | NotDelimited = 0
        | BarDelimited = 4
        | AmpersandDelimited = 8
        | CommaDelimited = 16
        | AsteriskDelimited = 32
        | DelimitersMask = 60
        | AllowTrailingComma = 64
        | Indented = 128
        | SpaceBetweenBraces = 256
        | SpaceBetweenSiblings = 512
        | Braces = 1024
        | Parenthesis = 2048
        | AngleBrackets = 4096
        | SquareBrackets = 8192
        | BracketsMask = 15360
        | OptionalIfUndefined = 16384
        | OptionalIfEmpty = 32768
        | Optional = 49152
        | PreferNewLine = 65536
        | NoTrailingNewLine = 131072
        | NoInterveningComments = 262144
        | NoSpaceIfEmpty = 524288
        | SingleElement = 1048576
        | SpaceAfterList = 2097152
        | Modifiers = 2359808
        | HeritageClauses = 512
        | SingleLineTypeLiteralMembers = 768
        | MultiLineTypeLiteralMembers = 32897
        | SingleLineTupleTypeElements = 528
        | MultiLineTupleTypeElements = 657
        | UnionTypeConstituents = 516
        | IntersectionTypeConstituents = 520
        | ObjectBindingPatternElements = 525136
        | ArrayBindingPatternElements = 524880
        | ObjectLiteralExpressionProperties = 526226
        | ImportAttributes = 526226
        /// <deprecated />
        | ImportClauseEntries = 526226
        | ArrayLiteralExpressionElements = 8914
        | CommaListElements = 528
        | CallExpressionArguments = 2576
        | NewExpressionArguments = 18960
        | TemplateExpressionSpans = 262144
        | SingleLineBlockStatements = 768
        | MultiLineBlockStatements = 129
        | VariableDeclarationList = 528
        | SingleLineFunctionBodyStatements = 768
        | MultiLineFunctionBodyStatements = 1
        | ClassHeritageClauses = 0
        | ClassMembers = 129
        | InterfaceMembers = 129
        | EnumMembers = 145
        | CaseBlockClauses = 129
        | NamedImportsOrExportsElements = 525136
        | JsxElementOrFragmentChildren = 262144
        | JsxElementAttributes = 262656
        | CaseOrDefaultClauseStatements = 163969
        | HeritageClauseTypes = 528
        | SourceFileStatements = 131073
        | Decorators = 2146305
        | TypeArguments = 53776
        | TypeParameters = 53776
        | Parameters = 2576
        | IndexSignatureParameters = 8848
        | JSDocComment = 33

    type [<RequireQualifiedAccess>] PragmaKindFlags =
        | None = 0
        /// Triple slash comment of the form
        /// /// <pragma-name argname="value" />
        | TripleSlashXML = 1
        /// <summary>
        /// Single line comment of the form
        /// //
        /// </summary>
        | SingleLine = 2
        /// <summary>
        /// Multiline non-jsdoc pragma of the form
        /// /*
        /// </summary>
        | MultiLine = 4
        | All = 7
        | Default = 7

    type [<AllowNullLiteral>] PragmaArgumentSpecification<'TName> =
        abstract name: 'TName with get, set
        abstract optional: bool option with get, set
        abstract captureSpan: bool option with get, set

    type PragmaDefinition =
        PragmaDefinition<string, string, string, string>

    type PragmaDefinition<'T1> =
        PragmaDefinition<'T1, string, string, string>

    type PragmaDefinition<'T1, 'T2> =
        PragmaDefinition<'T1, 'T2, string, string>

    type PragmaDefinition<'T1, 'T2, 'T3> =
        PragmaDefinition<'T1, 'T2, 'T3, string>

    type [<AllowNullLiteral>] PragmaDefinition<'T1, 'T2, 'T3, 'T4> =
        abstract args: U4<PragmaArgumentSpecification<'T1>, PragmaArgumentSpecification<'T1> * PragmaArgumentSpecification<'T2>, PragmaArgumentSpecification<'T1> * PragmaArgumentSpecification<'T2> * PragmaArgumentSpecification<'T3>, PragmaArgumentSpecification<'T1> * PragmaArgumentSpecification<'T2> * PragmaArgumentSpecification<'T3> * PragmaArgumentSpecification<'T4>> option with get, set
        abstract kind: PragmaKindFlags option with get, set

    type [<RequireQualifiedAccess>] JSDocParsingMode =
        /// Always parse JSDoc comments and include them in the AST.
        ///
        /// This is the default if no mode is provided.
        | ParseAll = 0
        /// Never parse JSDoc comments, mo matter the file type.
        | ParseNone = 1
        /// <summary>
        /// Parse only JSDoc comments which are needed to provide correct type errors.
        ///
        /// This will always parse JSDoc in non-TS files, but only parse JSDoc comments
        /// containing <c>@see</c> and <c>@link</c> in TS files.
        /// </summary>
        | ParseForTypeErrors = 2
        /// <summary>
        /// Parse only JSDoc comments which are needed to provide correct type info.
        ///
        /// This will always parse JSDoc in non-TS files, but never in TS files.
        ///
        /// Note: Do not use this mode if you require accurate type errors; use <see cref="ParseForTypeErrors" /> instead.
        /// </summary>
        | ParseForTypeInfo = 3

    type PragmaArgTypeMaybeCapture<'TDesc> =
        obj

    type PragmaArgTypeOptional<'TDesc, 'TName> =
        obj

    type UnionToIntersection<'U> =
        obj

    type ArgumentDefinitionToFieldUnion<'T> =
        obj

    /// <summary>Maps a pragma definition into the desired shape for its arguments object</summary>
    type PragmaArgumentType =
        obj

    type [<AllowNullLiteral>] ConcretePragmaSpecs =
        abstract reference: {| args: {| name: string; optional: bool; captureSpan: bool |} * {| name: string; optional: bool; captureSpan: bool |} * {| name: string; optional: bool; captureSpan: bool |} * {| name: string; optional: bool |} * {| name: string; optional: bool |} * {| name: string; optional: bool |}; kind: PragmaKindFlags |}
        abstract ``amd-dependency``: {| args: {| name: string |} * {| name: string; optional: bool |}; kind: PragmaKindFlags |}
        abstract ``amd-module``: {| args: {| name: string |}; kind: PragmaKindFlags |}
        abstract ``ts-check``: {| kind: PragmaKindFlags |}
        abstract ``ts-nocheck``: {| kind: PragmaKindFlags |}
        abstract jsx: {| args: {| name: string |}; kind: PragmaKindFlags |}
        abstract jsxfrag: {| args: {| name: string |}; kind: PragmaKindFlags |}
        abstract jsximportsource: {| args: {| name: string |}; kind: PragmaKindFlags |}
        abstract jsxruntime: {| args: {| name: string |}; kind: PragmaKindFlags |}

    type [<AllowNullLiteral>] PragmaPseudoMap =
        interface end

    type PragmaPseudoMapEntry =
        obj

    type [<AllowNullLiteral>] ReadonlyPragmaMap =
        inherit ReadonlyMap<string, U2<obj, ResizeArray<obj>>>
        abstract get: key: KeyOf<PragmaPseudoMap> -> U2<obj, ResizeArray<obj>>
        abstract forEach: action: (U2<obj, ResizeArray<obj>> -> KeyOf<PragmaPseudoMap> -> ReadonlyPragmaMap -> unit) -> unit

    /// <summary>
    /// A strongly-typed es6 map of pragma entries, the values of which are either a single argument
    /// value (if only one was found), or an array of multiple argument values if the pragma is present
    /// in multiple places
    /// </summary>
    type [<AllowNullLiteral>] PragmaMap =
        inherit Map<string, U2<obj, ResizeArray<obj>>>
        inherit ReadonlyPragmaMap
        abstract set: key: KeyOf<PragmaPseudoMap> * value: U2<obj, ResizeArray<obj>> -> PragmaMap
        abstract get: key: KeyOf<PragmaPseudoMap> -> U2<obj, ResizeArray<obj>>
        abstract forEach: action: (U2<obj, ResizeArray<obj>> -> KeyOf<PragmaPseudoMap> -> PragmaMap -> unit) -> unit

    type [<AllowNullLiteral>] CommentDirectivesMap =
        abstract getUnusedExpectations: unit -> ResizeArray<CommentDirective>
        abstract markUsed: matchedLine: float -> bool

    type [<AllowNullLiteral>] UserPreferences =
        abstract disableSuggestions: bool option
        abstract quotePreference: UserPreferencesQuotePreference option
        /// <summary>
        /// If enabled, TypeScript will search through all external modules' exports and add them to the completions list.
        /// This affects lone identifier completions but not completions on the right hand side of <c>obj.</c>.
        /// </summary>
        abstract includeCompletionsForModuleExports: bool option
        /// <summary>
        /// Enables auto-import-style completions on partially-typed import statements. E.g., allows
        /// <c>import write|</c> to be completed to <c>import { writeFile } from "fs"</c>.
        /// </summary>
        abstract includeCompletionsForImportStatements: bool option
        /// <summary>Allows completions to be formatted with snippet text, indicated by <c>CompletionItem["isSnippet"]</c>.</summary>
        abstract includeCompletionsWithSnippetText: bool option
        /// <summary>
        /// Unless this option is <c>false</c>, or <c>includeCompletionsWithInsertText</c> is not enabled,
        /// member completion lists triggered with <c>.</c> will include entries on potentially-null and potentially-undefined
        /// values, with insertion text to replace preceding <c>.</c> tokens with <c>?.</c>.
        /// </summary>
        abstract includeAutomaticOptionalChainCompletions: bool option
        /// <summary>
        /// If enabled, the completion list will include completions with invalid identifier names.
        /// For those entries, The <c>insertText</c> and <c>replacementSpan</c> properties will be set to change from <c>.x</c> property access to <c>["x"]</c>.
        /// </summary>
        abstract includeCompletionsWithInsertText: bool option
        /// <summary>
        /// If enabled, completions for class members (e.g. methods and properties) will include
        /// a whole declaration for the member.
        /// E.g., <c>class A { f| }</c> could be completed to <c>class A { foo(): number {} }</c>, instead of
        /// <c>class A { foo }</c>.
        /// </summary>
        abstract includeCompletionsWithClassMemberSnippets: bool option
        /// <summary>
        /// If enabled, object literal methods will have a method declaration completion entry in addition
        /// to the regular completion entry containing just the method name.
        /// E.g., <c>const objectLiteral: T = { f| }</c> could be completed to <c>const objectLiteral: T = { foo(): void {} }</c>,
        /// in addition to <c>const objectLiteral: T = { foo }</c>.
        /// </summary>
        abstract includeCompletionsWithObjectLiteralMethodSnippets: bool option
        /// <summary>
        /// Indicates whether <see cref="CompletionEntry.labelDetails">completion entry label details</see> are supported.
        /// If not, contents of <c>labelDetails</c> may be included in the <see cref="CompletionEntry.name" /> property.
        /// </summary>
        abstract useLabelDetailsInCompletionEntries: bool option
        abstract allowIncompleteCompletions: bool option
        abstract importModuleSpecifierPreference: UserPreferencesImportModuleSpecifierPreference option
        /// <summary>Determines whether we import <c>foo/index.ts</c> as "foo", "foo/index", or "foo/index.js"</summary>
        abstract importModuleSpecifierEnding: UserPreferencesImportModuleSpecifierEnding option
        abstract allowTextChangesInNewFiles: bool option
        abstract providePrefixAndSuffixTextForRename: bool option
        abstract includePackageJsonAutoImports: UserPreferencesIncludePackageJsonAutoImports option
        abstract provideRefactorNotApplicableReason: bool option
        abstract jsxAttributeCompletionStyle: UserPreferencesJsxAttributeCompletionStyle option
        abstract includeInlayParameterNameHints: UserPreferencesIncludeInlayParameterNameHints option
        abstract includeInlayParameterNameHintsWhenArgumentMatchesName: bool option
        abstract includeInlayFunctionParameterTypeHints: bool option
        abstract includeInlayVariableTypeHints: bool option
        abstract includeInlayVariableTypeHintsWhenTypeMatchesName: bool option
        abstract includeInlayPropertyDeclarationTypeHints: bool option
        abstract includeInlayFunctionLikeReturnTypeHints: bool option
        abstract includeInlayEnumMemberValueHints: bool option
        abstract interactiveInlayHints: bool option
        abstract allowRenameOfImportPath: bool option
        abstract autoImportFileExcludePatterns: ResizeArray<string> option
        abstract autoImportSpecifierExcludeRegexes: ResizeArray<string> option
        abstract preferTypeOnlyAutoImports: bool option
        /// Indicates whether imports should be organized in a case-insensitive manner.
        abstract organizeImportsIgnoreCase: U2<bool, string> option
        /// <summary>
        /// Indicates whether imports should be organized via an "ordinal" (binary) comparison using the numeric value
        /// of their code points, or via "unicode" collation (via the
        /// <see href="https://unicode.org/reports/tr10/#Scope">Unicode Collation Algorithm</see>) using rules associated with the locale
        /// specified in <see cref="organizeImportsCollationLocale" />.
        ///
        /// Default: <c>"ordinal"</c>.
        /// </summary>
        abstract organizeImportsCollation: UserPreferencesOrganizeImportsCollation option
        /// <summary>
        /// Indicates the locale to use for "unicode" collation. If not specified, the locale <c>"en"</c> is used as an invariant
        /// for the sake of consistent sorting. Use <c>"auto"</c> to use the detected UI locale.
        ///
        /// This preference is ignored if <see cref="organizeImportsCollation" /> is not <c>"unicode"</c>.
        ///
        /// Default: <c>"en"</c>
        /// </summary>
        abstract organizeImportsLocale: string option
        /// <summary>
        /// Indicates whether numeric collation should be used for digit sequences in strings. When <c>true</c>, will collate
        /// strings such that <c>a1z &lt; a2z &lt; a100z</c>. When <c>false</c>, will collate strings such that <c>a1z &lt; a100z &lt; a2z</c>.
        ///
        /// This preference is ignored if <see cref="organizeImportsCollation" /> is not <c>"unicode"</c>.
        ///
        /// Default: <c>false</c>
        /// </summary>
        abstract organizeImportsNumericCollation: bool option
        /// <summary>
        /// Indicates whether accents and other diacritic marks are considered unequal for the purpose of collation. When
        /// <c>true</c>, characters with accents and other diacritics will be collated in the order defined by the locale specified
        /// in <see cref="organizeImportsCollationLocale" />.
        ///
        /// This preference is ignored if <see cref="organizeImportsCollation" /> is not <c>"unicode"</c>.
        ///
        /// Default: <c>true</c>
        /// </summary>
        abstract organizeImportsAccentCollation: bool option
        /// <summary>
        /// Indicates whether upper case or lower case should sort first. When <c>false</c>, the default order for the locale
        /// specified in <see cref="organizeImportsCollationLocale" /> is used.
        ///
        /// This preference is ignored if <see cref="organizeImportsCollation" /> is not <c>"unicode"</c>. This preference is also
        /// ignored if we are using case-insensitive sorting, which occurs when <see cref="organizeImportsIgnoreCase" /> is <c>true</c>,
        /// or if <see cref="organizeImportsIgnoreCase" /> is <c>"auto"</c> and the auto-detected case sensitivity is determined to be
        /// case-insensitive.
        ///
        /// Default: <c>false</c>
        /// </summary>
        abstract organizeImportsCaseFirst: UserPreferencesOrganizeImportsCaseFirst option
        /// <summary>
        /// Indicates where named type-only imports should sort. "inline" sorts named imports without regard to if the import is
        /// type-only.
        ///
        /// Default: <c>last</c>
        /// </summary>
        abstract organizeImportsTypeOrder: OrganizeImportsTypeOrder option
        /// Indicates whether to exclude standard library and node_modules file symbols from navTo results.
        abstract excludeLibrarySymbolsInNavTo: bool option
        abstract lazyConfiguredProjectsFromExternalProject: bool option
        abstract displayPartsForJSDoc: bool option
        abstract generateReturnInDocTemplate: bool option
        abstract disableLineTextInReferences: bool option
        /// <summary>
        /// A positive integer indicating the maximum length of a hover text before it is truncated.
        ///
        /// Default: <c>500</c>
        /// </summary>
        abstract maximumHoverLength: float option

    type [<StringEnum>] [<RequireQualifiedAccess>] OrganizeImportsTypeOrder =
        | Last
        | Inline
        | First

    /// Represents a bigint literal value without requiring bigint support
    type [<AllowNullLiteral>] PseudoBigInt =
        abstract negative: bool with get, set
        abstract base10Value: string with get, set

    type [<AllowNullLiteral>] Queue<'T> =
        abstract enqueue: [<ParamArray>] items: 'T[] -> unit
        abstract dequeue: unit -> 'T
        abstract isEmpty: unit -> bool

    type [<AllowNullLiteral>] EvaluationResolver =
        abstract evaluateEntityNameExpression: expr: EntityNameExpression * location: Declaration option -> EvaluatorResult
        abstract evaluateElementAccessExpression: expr: ElementAccessExpression * location: Declaration option -> EvaluatorResult

    type HasInferredType =
        U5<Exclude<VariableLikeDeclaration, U2<JsxAttribute, EnumMember>>, PropertyAccessExpression, ElementAccessExpression, BinaryExpression, ExportAssignment>

    type [<AllowNullLiteral>] SyntacticTypeNodeBuilderContext =
        abstract flags: NodeBuilderFlags with get, set
        abstract tracker: Required<obj> with get, set
        abstract enclosingFile: SourceFile option with get, set
        abstract enclosingDeclaration: Node option with get, set
        abstract approximateLength: float with get, set
        abstract noInferenceFallback: bool option with get, set
        abstract suppressReportInferenceFallback: bool with get, set

    type [<AllowNullLiteral>] SyntacticTypeNodeBuilderResolver =
        abstract isOptionalParameter: p: ParameterDeclaration -> bool
        abstract isUndefinedIdentifierExpression: name: Identifier -> bool
        abstract isExpandoFunctionDeclaration: name: U2<FunctionDeclaration, VariableDeclaration> -> bool
        abstract getAllAccessorDeclarations: declaration: AccessorDeclaration -> AllAccessorDeclarations
        abstract requiresAddingImplicitUndefined: declaration: U5<ParameterDeclaration, PropertySignature, JSDocParameterTag, JSDocPropertyTag, PropertyDeclaration> * symbol: Symbol option * enclosingDeclaration: Node option -> bool
        abstract isDefinitelyReferenceToGlobalSymbolObject: node: Node -> bool
        abstract isEntityNameVisible: context: SyntacticTypeNodeBuilderContext * entityName: EntityNameOrEntityNameExpression * ?shouldComputeAliasToMakeVisible: bool -> SymbolVisibilityResult
        abstract serializeExistingTypeNode: context: SyntacticTypeNodeBuilderContext * node: TypeNode * ?addUndefined: bool -> TypeNode option
        abstract serializeReturnTypeForSignature: context: SyntacticTypeNodeBuilderContext * signatureDeclaration: U2<SignatureDeclaration, JSDocSignature> * symbol: Symbol option -> TypeNode option
        abstract serializeTypeOfExpression: context: SyntacticTypeNodeBuilderContext * expr: Expression -> TypeNode
        abstract serializeTypeOfDeclaration: context: SyntacticTypeNodeBuilderContext * node: U3<HasInferredType, GetAccessorDeclaration, SetAccessorDeclaration> * symbol: Symbol option -> TypeNode option
        abstract serializeNameOfParameter: context: SyntacticTypeNodeBuilderContext * parameter: ParameterDeclaration -> U2<BindingName, string>
        abstract serializeTypeName: context: SyntacticTypeNodeBuilderContext * node: EntityName * ?isTypeOf: bool * ?typeArguments: ResizeArray<TypeNode> -> TypeNode option
        abstract serializeEntityName: context: SyntacticTypeNodeBuilderContext * node: EntityNameExpression -> Expression option
        abstract getJsDocPropertyOverride: context: SyntacticTypeNodeBuilderContext * jsDocTypeLiteral: JSDocTypeLiteral * jsDocProperty: JSDocPropertyLikeTag -> TypeNode option
        abstract enterNewScope: context: SyntacticTypeNodeBuilderContext * node: U2<IntroducesNewScopeNode, ConditionalTypeNode> -> (unit -> unit)
        abstract markNodeReuse: context: SyntacticTypeNodeBuilderContext * range: 'T * location: Node option -> 'T when 'T :> Node
        abstract trackExistingEntityName: context: SyntacticTypeNodeBuilderContext * node: 'T -> {| introducesError: bool; node: 'T |} when 'T :> EntityNameOrEntityNameExpression
        abstract trackComputedName: context: SyntacticTypeNodeBuilderContext * accessExpression: EntityNameOrEntityNameExpression -> unit
        abstract evaluateEntityNameExpression: expression: EntityNameExpression -> EvaluatorResult
        abstract getModuleSpecifierOverride: context: SyntacticTypeNodeBuilderContext * parent: ImportTypeNode * lit: StringLiteral -> string option
        abstract canReuseTypeNode: context: SyntacticTypeNodeBuilderContext * existing: TypeNode -> bool
        abstract canReuseTypeNodeAnnotation: context: SyntacticTypeNodeBuilderContext * node: Declaration * existing: TypeNode * symbol: Symbol option * ?requiresAddingUndefined: bool -> bool
        abstract shouldRemoveDeclaration: context: SyntacticTypeNodeBuilderContext * node: DynamicNamedDeclaration -> bool
        abstract hasLateBindableName: node: Declaration -> bool
        abstract createRecoveryBoundary: context: SyntacticTypeNodeBuilderContext -> {| startRecoveryScope: unit -> (unit -> unit); finalizeBoundary: unit -> bool; markError: unit -> unit; hadError: unit -> bool |}

    type [<AllowNullLiteral>] SyntacticNodeBuilder =
        abstract serializeTypeOfDeclaration: (HasInferredType -> Symbol -> SyntacticTypeNodeBuilderContext -> TypeNode option) with get, set
        abstract serializeReturnTypeForSignature: (U2<SignatureDeclaration, JSDocSignature> -> Symbol -> SyntacticTypeNodeBuilderContext -> TypeNode option) with get, set
        abstract serializeTypeOfExpression: (U2<Expression, JsxAttributeValue> -> SyntacticTypeNodeBuilderContext -> (bool) option -> (bool) option -> TypeNode) with get, set
        abstract tryReuseExistingTypeNode: (SyntacticTypeNodeBuilderContext -> TypeNode -> TypeNode option) with get, set
        abstract serializeTypeOfAccessor: (AccessorDeclaration -> Symbol -> SyntacticTypeNodeBuilderContext -> TypeNode option) with get, set

    type IntroducesNewScopeNode =
        U3<SignatureDeclaration, JSDocSignature, MappedTypeNode>

    type [<StringEnum>] [<RequireQualifiedAccess>] NodeLinksFakeScopeForSignatureDeclaration =
        | Params
        | TypeParams

    type [<AllowNullLiteral>] DiagnosticMessageReportsUnnecessary =
        interface end

    type [<StringEnum>] [<RequireQualifiedAccess>] CommandLineOptionOfListTypeType =
        | List
        | ListOrElement

    type [<StringEnum>] [<RequireQualifiedAccess>] ResolvedModuleSpecifierInfoKind =
        | Node_modules
        | Paths
        | Redirect
        | Relative
        | Ambient

    type [<StringEnum>] [<RequireQualifiedAccess>] UserPreferencesQuotePreference =
        | Auto
        | Double
        | Single

    type [<StringEnum>] [<RequireQualifiedAccess>] UserPreferencesImportModuleSpecifierPreference =
        | Shortest
        | [<CompiledName("project-relative")>] ProjectRelative
        | Relative
        | [<CompiledName("non-relative")>] NonRelative

    type [<StringEnum>] [<RequireQualifiedAccess>] UserPreferencesImportModuleSpecifierEnding =
        | Auto
        | Minimal
        | Index
        | Js

    type [<StringEnum>] [<RequireQualifiedAccess>] UserPreferencesIncludePackageJsonAutoImports =
        | Auto
        | On
        | Off

    type [<StringEnum>] [<RequireQualifiedAccess>] UserPreferencesJsxAttributeCompletionStyle =
        | Auto
        | Braces
        | None

    type [<StringEnum>] [<RequireQualifiedAccess>] UserPreferencesIncludeInlayParameterNameHints =
        | None
        | Literals
        | All

    type [<StringEnum>] [<RequireQualifiedAccess>] UserPreferencesOrganizeImportsCollation =
        | Ordinal
        | Unicode

    type [<StringEnum>] [<RequireQualifiedAccess>] UserPreferencesOrganizeImportsCaseFirst =
        | Upper
        | Lower
