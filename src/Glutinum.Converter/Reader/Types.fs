module Glutinum.Converter.Reader.Types

open Fable.Core
open TypeScript
open Glutinum.Converter.GlueAST

exception TypeScriptReaderException of message: string

type ITypeScriptReader =
    abstract checker: Ts.TypeChecker with get
    abstract Warnings: ResizeArray<string> with get

    abstract ReadNode: node: Ts.Node -> GlueType

    abstract ReadTypeNode: typNode: Ts.TypeNode -> GlueType

    abstract ReadTypeNode: typNode: Ts.TypeNode option -> GlueType

    abstract ReadEnumDeclaration:
        enumDeclaration: Ts.EnumDeclaration -> GlueType

    abstract ReadTypeAliasDeclaration:
        typeAliasDeclaration: Ts.TypeAliasDeclaration -> GlueType

    abstract ReadInterfaceDeclaration:
        interfaceDeclaration: Ts.InterfaceDeclaration -> GlueType

    abstract ReadVariableStatement:
        variableStatement: Ts.VariableStatement -> GlueType

    abstract ReadFunctionDeclaration:
        functionDeclaration: Ts.FunctionDeclaration -> GlueType

    abstract ReadModuleDeclaration:
        moduleDeclaration: Ts.ModuleDeclaration -> GlueType

    abstract ReadClassDeclaration:
        classDeclaration: Ts.ClassDeclaration -> GlueType

    abstract ReadParameters:
        parameters: ResizeArray<Ts.ParameterDeclaration> -> GlueParameter list

    abstract ReadNamedDeclaration:
        declaration: Ts.NamedDeclaration -> GlueMember

    abstract ReadUnionTypeNode: unionType: Ts.UnionTypeNode -> GlueType

    abstract ReadTypeOperatorNode: node: Ts.TypeOperatorNode -> GlueType

    abstract ReadIndexedAccessType:
        declaration: Ts.IndexedAccessType -> GlueType

    abstract ReadTypeParameters:
        typeParametersOpt: ResizeArray<Ts.TypeParameterDeclaration> option ->
            GlueTypeParameter list
