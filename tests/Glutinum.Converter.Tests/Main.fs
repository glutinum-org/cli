module Glutinum.Converter.Tests.Main

open Fable.Core.JsInterop
open Scriptorium.Nib.Assertion
open Glutinum.Converter.Hosting

open type Scriptorium.Quill.Runner
open type Scriptorium.Quill.Test

let private satisfies (version: string) (range: string) =
    assertThat (Resolve.satisfiesRange version range) (isEqualTo true)

let private doesNotSatisfy (version: string) (range: string) =
    assertThat (Resolve.satisfiesRange version range) (isEqualTo false)

let private typesVersions (range: string) (pattern: string) (targets: string list) =
    createObj [ range ==> createObj [ pattern ==> Array.ofList targets ] ]

let private hostWith (files: (string * string) list) =
    let host = createInMemoryHost "/"

    for (file, content) in files do
        host.fileSystem.writeFileSync (file, content)

    host :> Host

[<EntryPoint>]
let main _ =
    runTests
        [
            testList (
                "Resolve.satisfiesRange",
                [
                    test (
                        "a partial version stands for its whole range",
                        fun _ ->
                            satisfies "5.5.9" "5.5"
                            doesNotSatisfy "5.6.0" "5.5"
                            satisfies "5.6.0" ">=5.5"
                            satisfies "4.1.5" "<=4.1"
                            doesNotSatisfy "4.1.5" ">4.1"
                            satisfies "4.2.0" ">4.1"
                    )

                    test (
                        "caret and tilde",
                        fun _ ->
                            satisfies "4.9.9" "^4.2.0"
                            doesNotSatisfy "5.0.0" "^4.2.0"
                            satisfies "1.2.9" "~1.2.3"
                            doesNotSatisfy "1.3.0" "~1.2.3"
                    )

                    test (
                        "alternatives and wildcards",
                        fun _ ->
                            satisfies "9.9.9" "*"
                            satisfies "5.1.0" ">=3.1 <4.0 || >=5.0"
                            doesNotSatisfy "4.5.0" ">=3.1 <4.0 || >=5.0"
                    )
                ]
            )

            testList (
                "Resolve.applyTypesVersions",
                [
                    test (
                        "a matching pattern maps the file into its folder",
                        fun _ ->
                            let mapping = typesVersions "*" "*" [ "ts4.5/*" ]

                            assertThat
                                (Resolve.applyTypesVersions mapping "index.d.ts")
                                (isEqualTo "ts4.5/index.d.ts")

                            assertThat
                                (Resolve.applyTypesVersions mapping "./lib/foo.d.ts")
                                (isEqualTo "ts4.5/lib/foo.d.ts")
                    )

                    test (
                        "no mapping without a satisfied range or a matching pattern",
                        fun _ ->
                            assertThat
                                (Resolve.applyTypesVersions null "index.d.ts")
                                (isEqualTo "index.d.ts")

                            assertThat
                                (Resolve.applyTypesVersions
                                    (typesVersions ">=99.0" "*" [ "ts99/*" ])
                                    "index.d.ts")
                                (isEqualTo "index.d.ts")

                            assertThat
                                (Resolve.applyTypesVersions
                                    (typesVersions "*" "lib/*" [ "ts4.5/lib/*" ])
                                    "index.d.ts")
                                (isEqualTo "index.d.ts")
                    )
                ]
            )

            testList (
                "Resolve.describePackage",
                [
                    test (
                        "a types package describes the runtime package it types",
                        fun _ ->
                            let host =
                                hostWith
                                    [
                                        "/pkg/package.json",
                                        """{ "name": "@types/scope__foo", "types": "index.d.ts" }"""
                                        "/pkg/index.d.ts", "export declare const x: number;"
                                    ]

                            let description = (Resolve.describePackage host "/pkg").Value
                            assertThat description.name (isEqualTo "@types/scope__foo")
                            assertThat description.runtimeName (isEqualTo "@scope/foo")
                            assertThat description.hasRuntime (isEqualTo true)
                            assertThat description.entryFile (isEqualTo "/pkg/index.d.ts")
                            assertThat description.hasExportsMap (isEqualTo false)
                            assertThat description.subpathEntries.Length (isEqualTo 0)
                    )

                    test (
                        "the exports map gives the entry and the subpaths",
                        fun _ ->
                            let host =
                                hostWith
                                    [
                                        "/pkg/package.json",
                                        """{ "name": "pkg", "types": "wrong.d.ts", "exports": { ".": { "types": "./dist/index.d.ts" }, "./utils": { "types": "./dist/utils.d.ts" } } }"""
                                        "/pkg/dist/index.d.ts", "export {};"
                                        "/pkg/dist/utils.d.ts", "export {};"
                                    ]

                            let description = (Resolve.describePackage host "/pkg").Value
                            assertThat description.entryFile (isEqualTo "/pkg/dist/index.d.ts")
                            assertThat description.hasExportsMap (isEqualTo true)

                            assertThat
                                (description.subpathEntries
                                 |> Array.map (fun entry -> entry.subpath, entry.file)
                                 |> List.ofArray)
                                (isEqualTo [ "utils", "/pkg/dist/utils.d.ts" ])
                    )

                    test (
                        "typesVersions moves the types root",
                        fun _ ->
                            let host =
                                hostWith
                                    [
                                        "/pkg/package.json",
                                        """{ "name": "pkg", "types": "index.d.ts", "typesVersions": { "*": { "*": ["ts4.5/*"] } } }"""
                                        "/pkg/ts4.5/index.d.ts", "export {};"
                                    ]

                            let description = (Resolve.describePackage host "/pkg").Value
                            assertThat description.entryFile (isEqualTo "/pkg/ts4.5/index.d.ts")
                            assertThat description.typesRoot (isEqualTo "/pkg/ts4.5")
                    )

                    test (
                        "a directory without package.json is not a package",
                        fun _ ->
                            let host = hostWith [ "/pkg/index.d.ts", "export {};" ]
                            assertThat (Resolve.describePackage host "/pkg").IsNone (isEqualTo true)
                    )
                ]
            )
        ]
