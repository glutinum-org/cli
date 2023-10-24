module rec Glutinum.Converter.Program

open Fable.Core
open System
open Node
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.FSharpAST
open Node.Api
open Fable.Core.JS
open Glutinum.Converter

let main (filePath: string) =
    if fs.existsSync (U2.Case1 filePath) |> not then
        failwith $"File does not exist: {filePath}"

    let files = [ filePath ]

    let options =
        jsOptions<Ts.CompilerOptions> (fun o ->
            o.target <- Some Ts.ScriptTarget.ES2015
            o.``module`` <- Some Ts.ModuleKind.CommonJS
        )

    let program = ts.createProgram (ResizeArray files, options)

    let checker = program.getTypeChecker ()

    let sourceFile = program.getSourceFile filePath

    let printer = new Printer.Printer()

    let glueAst = Read.readSourceFile checker sourceFile

    // printfn "glueAst: %A" glueAst

    let res = Transform.transform glueAst

    // printfn "fsharpAst: %A" res

    let outFile = {
        Name = None
        Opens = [ "Fable.Core"; "System" ]
    }

    Printer.printOutFile printer outFile

    Printer.print printer res

    printer.ToString()

// print res

// log(printer.ToString())

// let res = transform "./tests/specs/enums/literalStringEnumWithInheritance.d.ts"
let res = main "./tests/specs/keyof/simpleObject.d.ts"
// let res = transform "./tests/specs/enums/literalNumericEnum.d.ts"
// let res = transform "./tests/specs/enums/literalStringEnum.d.ts"

// printfn "'%A'" res
