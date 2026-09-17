module Glutinum.Node.Tests.Main

open Fable.Core
open Scriptorium.Nib.Assertion
open Glutinum
open Glutinum.Node.Exports

open type Scriptorium.Quill.Runner
open type Scriptorium.Quill.Test

let private utf8 = Node.BufferEncoding.utf8

let private tempDir =
    fs.mkdtempSync (path.join (Node.Exports.os.tmpdir (), "glutinum-node-tests-"))

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
                            let file = path.join (tempDir, "hello.txt")
                            fs.writeFileSync (file, "Hello from Fable")
                            assertThat (fs.existsSync file) (isTrue)
                            assertThat (fs.readFileSync (file, utf8)) (isEqualTo "Hello from Fable")

                            assertThat
                                (fs.readdirSync (tempDir, utf8) |> List.ofSeq)
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
                                let file = path.join (tempDir, "async.txt")
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
                                    Node.http.Exports.createServer<obj, obj> (
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
                                                        unbox<System.Delegate> (fun (chunk: obj) ->
                                                            chunks.Add(string chunk)
                                                        )
                                                    )
                                                    |> ignore

                                                    response.on (
                                                        "end",
                                                        unbox<System.Delegate> (fun () ->
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
        ]
