module Glutinum.Node.Tests.Main

open Fable.Core
open Scriptorium.Nib.Assertion
open Glutinum
open Glutinum.Node.Exports
open Glutinum.Types.TypeScript
open type Glutinum.Types.TypeScript.Exports

open type Scriptorium.Quill.Runner
open type Scriptorium.Quill.Test

let private utf8 = Node.BufferEncoding.utf8

let private tempDir (prefix: string) =
    fs.mkdtempSync (path.join (Node.Exports.os.tmpdir (), $"glutinum-node-tests-{prefix}-"))

let private awaitPromise (p: JS.Promise<'T>) : Async<'T> = Async.AwaitPromise p

[<EntryPoint>]
let main _ =
    runTests
        [
            testList (
                "Glutinum.Node",
                [
                    test (
                        "os: hostname and platform are available",
                        fun _ ->
                            assertThat (Node.Exports.os.hostname ()) (isNotEqualTo "")
                            assertThat (string (Node.Exports.os.platform ())) (isNotEqualTo "")
                    )

                    test (
                        "path: join, basename and extname",
                        fun _ ->
                            let file = path.join ("demo", "hello.txt")
                            assertThat (path.basename file) (isEqualTo "hello.txt")
                            assertThat (path.extname file) (isEqualTo ".txt")
                    )

                    test (
                        "fs: write, read and list files",
                        fun _ ->
                            let dir = tempDir "fs"
                            let file = path.join (dir, "hello.txt")
                            fs.writeFileSync (file, "Hello from Fable")
                            assertThat (fs.existsSync file) (isTrue)
                            assertThat (fs.readFileSync (file, utf8)) (isEqualTo "Hello from Fable")

                            assertThat
                                (fs.readdirSync (dir, utf8) |> List.ofSeq)
                                (isEqualTo [ "hello.txt" ])
                    )

                    test (
                        "process: the current directory and the environment",
                        fun _ ->
                            let proc = Node.Exports.``process``
                            assertThat (proc.cwd ()) (isNotEqualTo "")
                            assertThat proc.argv.Count (isGreaterOrEqual 1)
                    )

                    test (
                        "events: a listener receives the emitted arguments",
                        fun _ ->
                            let emitter = Node.events.Exports.EventEmitter()
                            let mutable received = ""
                            let mutable pair = ""
                            emitter.on ("greet", (fun (name: string) -> received <- name)) |> ignore

                            emitter.on (
                                "pair",
                                fun (name: string) (count: int) -> pair <- $"{name} {count}"
                            )
                            |> ignore

                            emitter.emit ("greet", "Fable") |> ignore
                            emitter.emit ("pair", "Fable", 2) |> ignore
                            assertThat received (isEqualTo "Fable")
                            assertThat pair (isEqualTo "Fable 2")
                    )

                    testAsync (
                        "fs/promises: readFile with an encoding gives a string",
                        fun _ ->
                            async {
                                let file = path.join (tempDir "promises", "async.txt")
                                fs.writeFileSync (file, "async content")

                                let! content =
                                    awaitPromise (Node.fs_promises.Exports.readFile (file, utf8))

                                assertThat content (isEqualTo "async content")
                            }
                    )

                    testAsync (
                        "http: a server answers a request",
                        fun _ ->
                            async {
                                let server =
                                    Node.http.Exports.createServer<obj, obj>(
                                        Node.http.RequestListener<obj, obj>(fun _ res ->
                                            let res = unbox<Node.http.ServerResponse> res
                                            res.setHeader ("content-type", "text/plain") |> ignore
                                            res.``end`` ("pong", utf8) |> ignore
                                        )
                                    )

                                let! address =
                                    awaitPromise (
                                        Promise.create (fun resolve _ ->
                                            server.listen (
                                                0,
                                                (fun () -> resolve (server.address ()))
                                            )
                                            |> ignore
                                        )
                                    )

                                // `U2<AddressInfo, string>` is erased, its cases can't be told apart at runtime
                                let port = (unbox<Node.net.AddressInfo> address).port

                                let! body =
                                    awaitPromise (
                                        Promise.create (fun resolve _ ->
                                            Node.http.Exports.get (
                                                $"http://127.0.0.1:{int port}/ping",
                                                (fun (response: Node.http.IncomingMessage) ->
                                                    let chunks = ResizeArray<string>()

                                                    response.on (
                                                        "data",
                                                        unbox<System.Delegate>(fun (chunk: obj) ->
                                                            chunks.Add(string chunk)
                                                        )
                                                    )
                                                    |> ignore

                                                    response.on (
                                                        "end",
                                                        unbox<System.Delegate>(fun () ->
                                                            resolve (String.concat "" chunks)
                                                        )
                                                    )
                                                    |> ignore
                                                )
                                            )
                                            |> ignore
                                        )
                                    )

                                server.close () |> ignore
                                assertThat body (isEqualTo "pong")
                            }
                    )
                ]
            )
            testList (
                "Glutinum.Types",
                [
                    test (
                        "Date: constructed from parts, a timestamp or a string",
                        fun _ ->
                            let date = Date.Create(2026, 8, 17, 12, 30)
                            assertThat (date.getFullYear ()) (isEqualTo 2026.0)
                            assertThat (date.getMonth ()) (isEqualTo 8.0)
                            assertThat (date.getMinutes ()) (isEqualTo 30.0)

                            let nextDay = Date.Create(date.getTime () + 86_400_000.0)
                            assertThat (nextDay.getDate ()) (isEqualTo 18.0)

                            let parsed = Date.Create "2026-01-01T00:00:00Z"
                            assertThat (parsed.getTime ()) (isEqualTo (Date.UTC(2026, 0, 1)))

                            assertThat
                                (parsed.toISOString ())
                                (isEqualTo "2026-01-01T00:00:00.000Z")
                    )
                    test (
                        "Date: the static members of the constructor",
                        fun _ ->
                            assertThat (Date.now () > 0.0) isTrue

                            assertThat
                                (Date.parse "2026-01-01T00:00:00Z")
                                (isEqualTo (Date.UTC(2026, 0, 1)))
                    )
                    test (
                        "Number, String, Object, Symbol and Reflect: the static side of the globals",
                        fun _ ->
                            assertThat (Number.isInteger 3.0) isTrue
                            assertThat (Number.isInteger 3.5) isFalse
                            assertThat (Number.parseFloat "2.5") (isEqualTo 2.5)
                            assertThat (Number.Invoke "42") (isEqualTo 42.0)
                            assertThat (String.fromCharCode (72.0, 105.0)) (isEqualTo "Hi")
                            assertThat (String.Invoke 12) (isEqualTo "12")

                            assertThat
                                (Object.keys {| a = 1; b = 2 |} |> String.concat ",")
                                (isEqualTo "a,b")

                            assertThat (Object.hasOwn ({| a = 1 |}, "a")) isTrue

                            assertThat
                                (Symbol.``for`` "glutinum" = Symbol.``for`` "glutinum")
                                isTrue

                            assertThat (Reflect.has ({| a = 1 |}, "a")) isTrue
                    )
                    test (
                        "ReadonlyArray: a sequence with an index",
                        fun _ ->
                            let names: ReadonlyArray<string> = unbox [| "a"; "b"; "c" |]
                            assertThat names.[1] (isEqualTo "b")

                            assertThat
                                (names |> Seq.map (fun name -> name.ToUpper()) |> String.concat "")
                                (isEqualTo "ABC")

                            assertThat (names.length) (isEqualTo 3.0)
                    )
                ]
            )
        ]
