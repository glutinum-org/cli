module Glutinum.Converter.Reader.TypeScriptReader

open TypeScript
open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open Glutinum.Converter.Reader.ClassDeclaration
open Glutinum.Converter.Reader.EnumDeclaration
open Glutinum.Converter.Reader.FunctionDeclaration
open Glutinum.Converter.Reader.IndexedAccessType
open Glutinum.Converter.Reader.InterfaceDeclaration
open Glutinum.Converter.Reader.ModuleDeclaration
open Glutinum.Converter.Reader.NamedDeclaration
open Glutinum.Converter.Reader.Node
open Glutinum.Converter.Reader.Parameters
open Glutinum.Converter.Reader.TypeAliasDeclaration
open Glutinum.Converter.Reader.TypeNode
open Glutinum.Converter.Reader.TypeOperatorNode
open Glutinum.Converter.Reader.TypeParameters
open Glutinum.Converter.Reader.UnionTypeNode
open Glutinum.Converter.Reader.VariableStatement

let typeScriptReader checker =
    { new TypeScriptReader(checker) with
        override _.checker: Ts.TypeChecker = checker

        member this.ReadClassDeclaration
            (classDeclaration: Ts.ClassDeclaration)
            : GlueType
            =
            readClassDeclaration this classDeclaration
            |> GlueType.ClassDeclaration

        member this.ReadEnumDeclaration
            (enumDeclaration: Ts.EnumDeclaration)
            : GlueType
            =
            readEnumDeclaration this enumDeclaration |> GlueType.Enum

        member this.ReadFunctionDeclaration
            (functionDeclaration: Ts.FunctionDeclaration)
            : GlueType
            =
            readFunctionDeclaration this functionDeclaration
            |> GlueType.FunctionDeclaration

        member this.ReadInterfaceDeclaration
            (interfaceDeclaration: Ts.InterfaceDeclaration)
            : GlueType
            =
            readInterfaceDeclaration this interfaceDeclaration
            |> GlueType.Interface

        member this.ReadModuleDeclaration
            (moduleDeclaration: Ts.ModuleDeclaration)
            : GlueType
            =
            readModuleDeclaration this moduleDeclaration
            |> GlueType.ModuleDeclaration

        member this.ReadNode(node: Ts.Node) : GlueType =
            readNode this node

        member this.ReadTypeAliasDeclaration
            (typeAliasDeclaration: Ts.TypeAliasDeclaration)
            : GlueType
            =
            readTypeAliasDeclaration this typeAliasDeclaration

        member this.ReadTypeNode(typNode: Ts.TypeNode) : GlueType =
            this.ReadTypeNode(Some typNode)

        member this.ReadTypeNode(typNode: Ts.TypeNode option) : GlueType =
            match typNode with
            | Some typNode -> readTypeNode this typNode
            | None -> GlueType.Primitive GluePrimitive.Unit

        member this.ReadVariableStatement
            (variableStatement: Ts.VariableStatement)
            : GlueType
            =
            readVariableStatement this variableStatement

        member this.ReadNamedDeclaration
            (declaration: Ts.NamedDeclaration)
            : GlueMember
            =
            readNamedDeclaration this declaration

        member this.ReadParameters
            (parameters: ResizeArray<Ts.ParameterDeclaration>)
            : GlueParameter list
            =
            readParameters this parameters

        member this.ReadUnionTypeNode
            (unionType: Ts.UnionTypeNode)
            : GlueType
            =
            readUnionTypeNode this unionType

        member this.ReadTypeOperatorNode
            (node: Ts.TypeOperatorNode)
            : GlueType
            =
            readTypeOperatorNode this node

        member this.ReadIndexedAccessType
            (declaration: Ts.IndexedAccessType)
            : GlueType
            =
            readIndexedAccessType this declaration

        member this.ReadTypeParameters
            (typeParametersOpt: ResizeArray<Ts.TypeParameterDeclaration> option)
            : GlueTypeParameter list
            =
            readTypeParameters this typeParametersOpt
    }
