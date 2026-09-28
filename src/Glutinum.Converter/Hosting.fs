module rec Glutinum.Converter.Hosting

open Fable.Core
open Fable.Core.JsInterop
open System
open System.Text.RegularExpressions
open TypeScript
open TsMorph

let private declarationFile = Regex(@"\.d\.[cm]?ts$")
let private javaScriptFile = Regex(@"\.[cm]?js$")

// The web app bundles this file, so `fs` and `path` are imported as namespaces: a named import
// from them is not exported by the browser shim and breaks the bundle
[<AutoOpen>]
module private NodeExterns =

    type Stats =
        abstract member isFile: unit -> bool
        abstract member isDirectory: unit -> bool

    type NodeFs =
        abstract member existsSync: path: string -> bool
        abstract member statSync: path: string -> Stats
        abstract member readFileSync: path: string * encoding: string -> string
        abstract member readdirSync: path: string -> string[]
        abstract member realpathSync: path: string -> string

    type NodePath =
        [<Emit("$0.join(...$1)")>]
        abstract member join: parts: string[] -> string

        abstract member dirname: path: string -> string
        abstract member basename: path: string -> string

        [<Emit("$0.resolve(...$1)")>]
        abstract member resolve: parts: string[] -> string

    [<ImportAll("fs")>]
    let nodeFs: NodeFs = jsNative

    [<ImportAll("path")>]
    let nodePath: NodePath = jsNative

/// The path and file-system operations the resolution logic needs, over the disk on the CLI and
/// over a ts-morph in-memory file system in the browser
type Path =
    abstract member join: parts: string[] -> string
    abstract member dirname: path: string -> string
    abstract member basename: path: string -> string
    abstract member resolve: parts: string[] -> string

type Fs =
    abstract member fileExists: path: string -> bool
    abstract member directoryExists: path: string -> bool
    abstract member readFile: path: string -> string
    abstract member readDirectory: path: string -> string[]
    abstract member realPath: path: string -> string

[<AllowNullLiteral>]
type Host =
    abstract member cwd: string
    abstract member path: Path
    abstract member fs: Fs
    abstract member createProject: compilerOptions: Ts.CompilerOptions -> Project

/// A host over a ts-morph in-memory file system
[<AllowNullLiteral>]
type InMemoryHost =
    inherit Host
    abstract member fileSystem: FileSystemHost

type ResolvedInput =
    abstract member kind: string
    abstract member file: string
    abstract member packageDir: string

type SubpathEntry =
    abstract member subpath: string
    abstract member file: string

[<AllowNullLiteral>]
type PackageDescription =
    abstract member name: string
    abstract member runtimeName: string
    abstract member hasRuntime: bool
    abstract member dir: string
    abstract member typesRoot: string
    abstract member entryFile: string
    abstract member subpathEntries: SubpathEntry[]
    /// The `exports` map makes every file it does not list unreachable
    abstract member hasExportsMap: bool

[<AllowNullLiteral>]
type InstalledPackage =
    abstract member name: string
    abstract member version: string

/// `/` separated paths, the only ones of the in-memory file system
module PosixPath =

    let normalize (p: string) : string =
        let absolute = p.StartsWith "/"
        let segments = ResizeArray<string>()

        for segment in p.Split('/') do
            if segment = "" || segment = "." then
                ()
            elif segment = ".." then
                if segments.Count > 0 && segments[segments.Count - 1] <> ".." then
                    segments.RemoveAt(segments.Count - 1)
                elif not absolute then
                    segments.Add ".."
            else
                segments.Add segment

        let joined = String.Join("/", segments)

        if absolute then
            "/" + joined
        elif joined = "" then
            "."
        else
            joined

    let join (parts: string[]) : string =
        parts |> Array.filter (fun part -> part <> "") |> String.concat "/" |> normalize

    let dirname (p: string) : string =
        let normalized = normalize p
        let index = normalized.LastIndexOf '/'

        if index = -1 then
            "."
        elif index = 0 then
            "/"
        else
            normalized.Substring(0, index)

    let basename (p: string) : string =
        let normalized = normalize p
        normalized.Substring(normalized.LastIndexOf '/' + 1)

    let resolve (parts: string[]) : string =
        let mutable result = "/"

        for part in parts do
            result <-
                if part.StartsWith "/" then
                    part
                else
                    result + "/" + part

        normalize result

let private posixPath =
    { new Path with
        member _.join parts = PosixPath.join parts
        member _.dirname p = PosixPath.dirname p
        member _.basename p = PosixPath.basename p
        member _.resolve parts = PosixPath.resolve parts
    }

/// A host over a ts-morph file system, the in-memory one in the browser
let createFileSystemHost (fileSystem: FileSystemHost) (cwd: string) : InMemoryHost =
    { new InMemoryHost with
        member _.cwd = cwd
        member _.fileSystem = fileSystem
        member _.path = posixPath

        member _.fs =
            { new Fs with
                member _.fileExists p = fileSystem.fileExistsSync p
                member _.directoryExists p = fileSystem.directoryExistsSync p
                member _.readFile p = fileSystem.readFileSync (p, "utf8")

                member _.readDirectory p =
                    fileSystem.readDirSync p |> Seq.map (fun entry -> entry.name) |> Seq.toArray

                member _.realPath p =
                    try
                        fileSystem.realpathSync p
                    with _ ->
                        p
            }

        member _.createProject compilerOptions =
            Exports.createProjectSync (
                ProjectOptions.Create(
                    fileSystem = fileSystem,
                    compilerOptions = compilerOptions,
                    skipAddingFilesFromTsConfig = true
                )
            )
    }

let createInMemoryHost (cwd: string) : InMemoryHost =
    createFileSystemHost (Exports.InMemoryFileSystemHost()) cwd

/// The host of the CLI, over the disk
let createNodeHost (cwd: string) : Host =
    { new Host with
        member _.cwd = cwd

        member _.path =
            { new Path with
                member _.join parts = nodePath.join parts
                member _.dirname p = nodePath.dirname p
                member _.basename p = nodePath.basename p
                member _.resolve parts = nodePath.resolve parts
            }

        member _.fs =
            { new Fs with
                member _.fileExists p =
                    nodeFs.existsSync p && (nodeFs.statSync p).isFile()

                member _.directoryExists p =
                    nodeFs.existsSync p && (nodeFs.statSync p).isDirectory()

                member _.readFile p = nodeFs.readFileSync (p, "utf8")
                member _.readDirectory p = nodeFs.readdirSync p

                member _.realPath p =
                    try
                        nodeFs.realpathSync p
                    with _ ->
                        p
            }

        member _.createProject compilerOptions =
            Exports.createProjectSync (
                ProjectOptions.Create(
                    compilerOptions = compilerOptions,
                    skipAddingFilesFromTsConfig = true
                )
            )
    }

[<AutoOpen>]
module private Dynamic =

    let inline isNullish (x: obj) : bool =
        emitJsExpr x "$0 === null || $0 === undefined"

    let inline jsTypeof (x: obj) : string = emitJsExpr x "typeof $0"

    let inline prop (o: obj) (key: string) : obj = emitJsExpr (o, key) "$0[$1]"

    let inline objEntries (o: obj) : (string * obj)[] = emitJsExpr o "Object.entries($0)"

    let inline objValues (o: obj) : obj[] = emitJsExpr o "Object.values($0)"

    /// `String.prototype.replace(string, string)` replaces the first occurrence only
    let replaceFirst (value: string) (search: string) (replacement: string) : string =
        let index = value.IndexOf search

        if index < 0 then
            value
        else
            value.Substring(0, index) + replacement + value.Substring(index + search.Length)

/// The package resolution logic, ported from `js/resolve.js`
module Resolve =

    let findNodeModules (host: Host) (cwd: string) : string option =
        let mutable dir = host.path.resolve [| cwd |]
        let mutable result = None
        let mutable go = true

        while go do
            let candidate = host.path.join [| dir; "node_modules" |]

            if host.fs.directoryExists candidate then
                result <- Some candidate
                go <- false
            else
                let parent = host.path.dirname dir

                if parent = dir then
                    go <- false
                else
                    dir <- parent

        result

    let findPackageDir (host: Host) (file: string) : string option =
        let mutable dir = host.path.dirname (host.path.resolve [| file |])
        let mutable result = None
        let mutable go = true

        while go do
            let packageJsonPath = host.path.join [| dir; "package.json" |]

            // `dist/esm/package.json` files only holding `{ "type": "module" }` do not delimit a package
            if
                host.fs.fileExists packageJsonPath
                && not (isNullish (prop (JS.JSON.parse (host.fs.readFile packageJsonPath)) "name"))
            then
                result <- Some(host.fs.realPath dir)
                go <- false
            else
                let parent = host.path.dirname dir

                if parent = dir then
                    go <- false
                else
                    dir <- parent

        result

    let private collectExportsTypes (exportsField: obj) : (string * string) list =
        let result = ResizeArray<string * string>()

        let rec visit (subpath: string) (node: obj) =
            if isNullish node then
                ()
            elif jsTypeof node = "string" then
                let text: string = unbox node

                if declarationFile.IsMatch text then
                    result.Add(subpath, text)
            elif jsTypeof node = "object" then
                for (key, value) in objEntries node do
                    if key.StartsWith "." then
                        visit key value
                    else
                        visit subpath value

        visit "." exportsField

        // Keep the first declaration file found for a subpath
        let bySubpath = Collections.Generic.Dictionary<string, string>()

        for (subpath, file) in result do
            if not (bySubpath.ContainsKey subpath) then
                bySubpath[subpath] <- file

        [ for entry in bySubpath -> entry.Key, entry.Value ]

    let rec private exportsRuntimeFile (node: obj) : bool =
        if jsTypeof node = "string" then
            not (declarationFile.IsMatch(unbox node))
        elif not (isNullish node) && jsTypeof node = "object" then
            objValues node |> Array.exists exportsRuntimeFile
        else
            false

    let private hasRuntime (host: Host) (packageDir: string) (pkg: obj) : bool =
        // `csstype` declares `"main": ""`
        let declared (field: obj) =
            if jsTypeof field = "string" then
                unbox<string> field <> ""
            else
                not (isNullish field)

        if
            declared (prop pkg "main")
            || declared (prop pkg "module")
            || declared (prop pkg "bin")
            || declared (prop pkg "browser")
        then
            true
        elif exportsRuntimeFile (prop pkg "exports") then
            true
        else
            [ "index.js"; "index.mjs"; "index.cjs"; ".glutinum-runtime" ]
            |> List.exists (fun file -> host.fs.fileExists (host.path.join [| packageDir; file |]))

    let private parseVersion (version: string) : int[] =
        let parts = version.Split('.')

        let at i =
            if i < parts.Length then
                match Int32.TryParse parts[i] with
                | true, n -> n
                | _ -> 0
            else
                0

        [| at 0; at 1; at 2 |]

    let private compareVersions (a: int[]) (b: int[]) : int =
        let mutable result = 0
        let mutable i = 0

        while i < 3 && result = 0 do
            if a[i] <> b[i] then
                result <- a[i] - b[i]

            i <- i + 1

        result

    let private satisfiesComparator (version: int[]) (comparator: string) : bool =
        let m = Regex.Match(comparator, @"^(>=|<=|>|<|=|\^|~)?\s*(.+)$")

        if not m.Success then
            false
        else
            let operator =
                if m.Groups[1].Success && m.Groups[1].Value <> "" then
                    m.Groups[1].Value
                else
                    "="

            let text = m.Groups[2].Value

            if text = "*" || text = "x" then
                true
            else
                let parts =
                    text.Split('.')
                    |> Array.choose (fun part ->
                        match Int32.TryParse part with
                        | true, n -> Some n
                        | _ -> None
                    )

                let g i =
                    if i < parts.Length then
                        parts[i]
                    else
                        0

                let lower = [| g 0; g 1; g 2 |]

                // `5.5` stands for every `5.5.x`, the bound after it is `5.6.0`
                let upper =
                    match parts.Length with
                    | 1 -> [| lower[0] + 1; 0; 0 |]
                    | 2 -> [| lower[0]; lower[1] + 1; 0 |]
                    | _ -> [| lower[0]; lower[1]; lower[2] + 1 |]

                match operator with
                | ">=" -> compareVersions version lower >= 0
                | ">" -> compareVersions version upper >= 0
                | "<" -> compareVersions version lower < 0
                | "<=" -> compareVersions version upper < 0
                | "^" ->
                    compareVersions version lower >= 0
                    && compareVersions version [| lower[0] + 1; 0; 0 |] < 0
                | "~" ->
                    compareVersions version lower >= 0
                    && compareVersions version [| lower[0]; lower[1] + 1; 0 |] < 0
                | _ -> compareVersions version lower >= 0 && compareVersions version upper < 0

    let private satisfiesRange (version: string) (range: string) : bool =
        let parsed = parseVersion version

        range.Split([| "||" |], StringSplitOptions.None)
        |> Array.exists (fun alternative ->
            Regex.Split(alternative.Trim(), @"\s+")
            |> Array.forall (fun comparator -> satisfiesComparator parsed comparator)
        )

    let private applyTypesVersions (typesVersions: obj) (file: string) : string =
        if isNullish typesVersions then
            file
        else
            let paths =
                objEntries typesVersions
                |> Array.tryFind (fun (range, _) -> satisfiesRange ts.version range)
                |> Option.map snd

            match paths with
            | None -> file
            | Some paths ->
                let normalized = Regex.Replace(file, @"^\./", "")
                let mutable found = None

                for (pattern, targets) in objEntries paths do
                    if found.IsNone then
                        let split = pattern.Split('*')
                        let prefix = split[0]

                        let suffix =
                            if split.Length > 1 then
                                split[1]
                            else
                                ""

                        let targets: string[] = unbox targets

                        if
                            normalized.StartsWith prefix
                            && normalized.EndsWith suffix
                            && targets.Length > 0
                        then
                            let captured =
                                normalized.Substring(
                                    prefix.Length,
                                    normalized.Length - suffix.Length - prefix.Length
                                )

                            found <- Some(targets[0].Replace("*", captured))

                match found with
                | Some f -> f
                | None -> file

    let describePackage (host: Host) (packageDir: string) : PackageDescription option =
        let packageJsonPath = host.path.join [| packageDir; "package.json" |]

        if not (host.fs.fileExists packageJsonPath) then
            None
        else
            let pkg = JS.JSON.parse (host.fs.readFile packageJsonPath)

            let name: string =
                match prop pkg "name" with
                | n when not (isNullish n) -> unbox n
                | _ -> host.path.basename packageDir

            // `@types/foo` describes the `foo` package, `@types/scope__foo` the `@scope/foo` one
            let mutable runtimeName = name

            if name.StartsWith "@types/" then
                let typed = name.Substring("@types/".Length)

                runtimeName <-
                    if typed.Contains "__" then
                        "@" + typed.Replace("__", "/")
                    else
                        typed

            let candidates = ResizeArray<string * string>()

            let typesField =
                match prop pkg "types" with
                | n when not (isNullish n) -> n
                | _ -> prop pkg "typings"

            if jsTypeof typesField = "string" then
                candidates.Add(".", unbox typesField)

            candidates.AddRange(collectExportsTypes (prop pkg "exports"))

            let mainField = prop pkg "main"

            if jsTypeof mainField = "string" then
                candidates.Add(".", Regex.Replace(unbox mainField, @"\.[cm]?js$", ".d.ts"))

            candidates.Add(".", "index.d.ts")

            let entries =
                ResizeArray<
                    {|
                        subpath: string
                        file: string
                        isMapped: bool
                    |}
                 >()

            for (subpath, candidateFile) in candidates do
                let mappedFile = applyTypesVersions (prop pkg "typesVersions") candidateFile
                let mutable file = host.path.resolve [| packageDir; mappedFile |]

                // `"types": "./lib/umd/main"` is allowed without an extension
                if not (declarationFile.IsMatch file) && host.fs.fileExists (file + ".d.ts") then
                    file <- file + ".d.ts"

                if host.fs.fileExists file && declarationFile.IsMatch file then
                    if
                        not (
                            entries
                            |> Seq.exists (fun entry ->
                                entry.subpath = subpath || entry.file = file
                            )
                        )
                    then
                        entries.Add
                            {|
                                subpath = subpath
                                file = file
                                isMapped = mappedFile <> candidateFile
                            |}

            match entries |> Seq.tryFind (fun entry -> entry.subpath = ".") with
            | None -> None
            | Some main ->
                Some(
                    { new PackageDescription with
                        member _.name = name
                        member _.runtimeName = runtimeName

                        member _.hasRuntime =
                            name.StartsWith "@types/" || hasRuntime host packageDir pkg

                        member _.dir = packageDir
                        // The files of a `typesVersions` folder are named as if they were at the root
                        member _.typesRoot =
                            if main.isMapped then
                                host.path.dirname main.file
                            else
                                packageDir

                        member _.entryFile = main.file

                        member _.hasExportsMap = not (isNullish (prop pkg "exports"))

                        member _.subpathEntries =
                            entries
                            |> Seq.filter (fun entry -> entry.subpath <> ".")
                            |> Seq.map (fun entry ->
                                { new SubpathEntry with
                                    member _.subpath = Regex.Replace(entry.subpath, @"^\./", "")
                                    member _.file = entry.file
                                }
                            )
                            |> Seq.toArray
                    }
                )

    let private resolvedFile (file: string) : ResolvedInput =
        { new ResolvedInput with
            member _.kind = "file"
            member _.file = file
            member _.packageDir = ""
        }

    let private resolvedPackage (packageDir: string) : ResolvedInput =
        { new ResolvedInput with
            member _.kind = "package"
            member _.file = ""
            member _.packageDir = packageDir
        }

    let resolveInput (host: Host) (input: string) : ResolvedInput =
        let asPath = host.path.resolve [| host.cwd; input |]

        if declarationFile.IsMatch input && host.fs.fileExists asPath then
            resolvedFile asPath
        elif host.fs.directoryExists asPath then
            resolvedPackage (host.fs.realPath asPath)
        else
            match findNodeModules host host.cwd with
            | Some nodeModules ->
                // `date-fns/locale` is a subpath of `date-fns`, generated with the package
                let segments = input.Split('/')

                let packageName =
                    if input.StartsWith "@" then
                        String.Join("/", segments[0..1])
                    else
                        segments[0]

                let typesPackage =
                    host.path.join
                        [| "@types"; replaceFirst (Regex.Replace(packageName, "^@", "")) "/" "__" |]

                let mutable withoutDeclaration = None
                let mutable result = None

                // A package without declaration files (`ws`) is described by its `@types` package
                for candidate in [ packageName; typesPackage ] do
                    if result.IsNone then
                        let packageDir =
                            host.fs.realPath (host.path.join [| nodeModules; candidate |])

                        if host.fs.fileExists (host.path.join [| packageDir; "package.json" |]) then
                            if (describePackage host packageDir).IsSome then
                                result <- Some(resolvedPackage packageDir)
                            elif withoutDeclaration.IsNone then
                                withoutDeclaration <- Some candidate

                match result with
                | Some resolved -> resolved
                | None ->
                    match withoutDeclaration with
                    | Some _ ->
                        failwith
                            $"'{withoutDeclaration.Value}' ships no type declaration, install '{typesPackage}' and generate that instead."
                    | None ->
                        failwith
                            $"Could not find '{input}': it is neither a declaration file, a package directory, nor a package installed in node_modules."
            | None ->
                failwith
                    $"Could not find '{input}': it is neither a declaration file, a package directory, nor a package installed in node_modules."

    let listInstalledPackages (host: Host) : string[] =
        match findNodeModules host host.cwd with
        | None -> [||]
        | Some nodeModules ->
            let result = ResizeArray<string>()

            for entry in host.fs.readDirectory nodeModules do
                if not (entry.StartsWith ".") then
                    let entryPath = host.path.join [| nodeModules; entry |]

                    if entry.StartsWith "@" then
                        for scoped in host.fs.readDirectory entryPath do
                            result.Add(host.path.join [| entryPath; scoped |])
                    else
                        result.Add entryPath

            result
            |> Seq.filter (fun dir -> host.fs.fileExists (host.path.join [| dir; "package.json" |]))
            |> Seq.map host.fs.realPath
            |> Seq.filter (fun dir -> (describePackage host dir).IsSome)
            |> Seq.toArray

/// The declaration downloader, ported from `js/npm.js`
module Npm =

    [<Literal>]
    let private REGISTRY = "https://data.jsdelivr.com/v1/package"

    [<Literal>]
    let private CDN = "https://cdn.jsdelivr.net/npm"

    [<Literal>]
    let private CONCURRENCY = 8

    // Written next to a package shipping JavaScript, read by `hasRuntime` of the resolver
    [<Literal>]
    let private RUNTIME_MARKER = ".glutinum-runtime"

    type private Response =
        abstract member ok: bool
        abstract member status: int
        abstract member url: string
        abstract member json: unit -> JS.Promise<obj>
        abstract member text: unit -> JS.Promise<string>

    type FetchFn = string -> JS.Promise<Response>

    [<Emit("globalThis.fetch")>]
    let private globalFetch: FetchFn = jsNative

    let inline private encodeURIComponent (value: string) : string =
        emitJsExpr value "encodeURIComponent($0)"

    /// `@types/foo` for `foo`, `@types/scope__foo` for `@scope/foo`
    let typesPackageName (name: string) : string =
        "@types/" + replaceFirst (Regex.Replace(name, "^@", "")) "/" "__"

    /// `chalk`, `chalk@5`, `@scope/name@^1.2`
    let parsePackageSpec (spec: string) : {| name: string; range: string |} =
        let at = spec.IndexOf("@", 1)

        if at = -1 then
            {| name = spec; range = "latest" |}
        else
            let range = spec.Substring(at + 1)

            {|
                name = spec.Substring(0, at)
                range =
                    (if range = "" then
                         "latest"
                     else
                         range)
            |}

    /// Download the declaration files of a package and of the packages it depends on into
    /// `/node_modules` of the file system, like `npm install` would, from jsDelivr
    let installPackage
        (fileSystem: FileSystemHost)
        (spec: string)
        (options: obj)
        : JS.Promise<InstalledPackage>
        =
        let fetchImpl: FetchFn =
            match prop options "fetch" with
            | f when not (isNullish f) -> unbox f
            | _ -> globalFetch

        let onProgress: string -> unit =
            match prop options "onProgress" with
            | f when not (isNullish f) -> unbox f
            | _ -> ignore

        // `@types/node` describes the runtime, it is not needed to generate a package
        let skip =
            Collections.Generic.HashSet<string>(
                match prop options "skip" with
                | s when not (isNullish s) -> unbox<string[]> s :> seq<string>
                | _ -> [ "@types/node" ] :> seq<string>
            )

        let installed = Collections.Generic.Dictionary<string, InstalledPackage>()

        let ignoreP (p: JS.Promise<'T>) : JS.Promise<unit> =
            promise {
                let! _ = p
                return ()
            }

        let getJson (url: string) : JS.Promise<obj> =
            promise {
                let! response = fetchImpl url

                if not response.ok then
                    return failwith $"{url}: {response.status}"
                else
                    return! response.json ()
            }

        let getText (url: string) : JS.Promise<string> =
            promise {
                let! response = fetchImpl url

                if not response.ok then
                    return failwith $"{url}: {response.status}"
                else
                    return! response.text ()
            }

        let rec install (name: string) (range: string) : JS.Promise<InstalledPackage option> =
            promise {
                if installed.ContainsKey name || skip.Contains name then
                    return
                        (if installed.ContainsKey name then
                             Some installed[name]
                         else
                             None)
                else
                    let! resolveResponse =
                        fetchImpl $"{REGISTRY}/resolve/npm/{name}@{encodeURIComponent range}"

                    if resolveResponse.status = 404 then
                        return failwith $"No package named '{name}' on npm"
                    elif not resolveResponse.ok then
                        return failwith $"{resolveResponse.url}: {resolveResponse.status}"
                    else
                        let! resolved = resolveResponse.json ()

                        if isNullish (prop resolved "version") then
                            return failwith $"No version of '{name}' matches '{range}'"
                        else
                            let version: string = unbox (prop resolved "version")

                            installed[name] <-
                                { new InstalledPackage with
                                    member _.name = name
                                    member _.version = version
                                }

                            onProgress $"{name}@{version}: listing files"

                            let! flat = getJson $"{REGISTRY}/npm/{name}@{version}/flat"
                            let files: obj[] = unbox (prop flat "files")

                            let wanted =
                                ResizeArray<string>(
                                    files
                                    |> Array.map (fun file -> unbox<string>(prop file "name"))
                                    |> Array.filter (fun file ->
                                        declarationFile.IsMatch file
                                        || file.EndsWith "/package.json"
                                    )
                                )

                            let mutable doneCount = 0

                            let worker () =
                                promise {
                                    while wanted.Count > 0 do
                                        let file = wanted[0]
                                        wanted.RemoveAt 0
                                        let! content = getText $"{CDN}/{name}@{version}{file}"

                                        fileSystem.writeFileSync (
                                            $"/node_modules/{name}{file}",
                                            content
                                        )

                                        doneCount <- doneCount + 1
                                        onProgress $"{name}@{version}: {doneCount} files"
                                }

                            let! _ = Promise.all [| for _ in 1..CONCURRENCY -> worker () |]

                            let packageJson =
                                JS.JSON.parse (
                                    fileSystem.readFileSync (
                                        $"/node_modules/{name}/package.json",
                                        "utf8"
                                    )
                                )

                            // The JavaScript is never downloaded, the file system cannot be asked for it
                            if
                                files
                                |> Array.exists (fun file ->
                                    javaScriptFile.IsMatch(unbox<string>(prop file "name"))
                                )
                            then
                                fileSystem.writeFileSync (
                                    $"/node_modules/{name}/{RUNTIME_MARKER}",
                                    ""
                                )

                            let hasDeclarations =
                                files
                                |> Array.exists (fun file ->
                                    declarationFile.IsMatch(unbox<string>(prop file "name"))
                                )

                            // A package without declaration files (`ws`) is described by its `@types` package
                            if not hasDeclarations && not (name.StartsWith "@types/") then
                                try
                                    do! ignoreP (install (typesPackageName name) "latest")
                                with _ ->
                                    ()

                            let dependencies =
                                match prop packageJson "dependencies" with
                                | d when not (isNullish d) -> d
                                | _ -> createObj []

                            for (dependency, dependencyRange) in objEntries dependencies do
                                do! ignoreP (install dependency (unbox<string> dependencyRange))

                            return
                                (if installed.ContainsKey name then
                                     Some installed[name]
                                 else
                                     None)
            }

        promise {
            let parsed = parsePackageSpec spec
            let! result = install parsed.name parsed.range
            return Option.toObj result
        }

/// The program creation, ported from `js/bootstrap.js`
module Bootstrap =

    let createProgramForCLI (_fileName: string) (source: string) : Ts.Program =
        // `strict`, so that `T | undefined` is kept by the checker
        let compilerOptions = createEmpty<Ts.CompilerOptions>
        compilerOptions.strict <- Some true

        let project = Exports.createProjectSync (ProjectOptions.Create(compilerOptions))

        project.createSourceFile (_fileName, source) |> ignore
        project.createProgram ()

    /// Create a program from declaration files of the host, following their imports
    let createProgramFromFiles
        (host: Host)
        (entryFiles: string[])
        (withoutDomLib: bool)
        (noLib: bool)
        : Ts.Program
        =
        // ESNext, so lib types such as `AsyncIterable` resolve; `types: []` stops TypeScript from
        // loading every `node_modules/@types`; Bundler resolution follows `exports` maps
        let compilerOptions = createEmpty<Ts.CompilerOptions>
        compilerOptions.target <- Some Ts.ScriptTarget.ESNext
        compilerOptions.``module`` <- Some Ts.ModuleKind.ESNext
        compilerOptions.moduleResolution <- Some Ts.ModuleResolutionKind.Bundler
        compilerOptions.types <- Some(ResizeArray())
        compilerOptions.strict <- Some true

        // A package replacing the DOM lib (`@types/web`) redeclares its globals
        if withoutDomLib then
            compilerOptions.lib <-
                Some(
                    ResizeArray
                        [ "lib.esnext.d.ts"; "lib.decorators.d.ts"; "lib.decorators.legacy.d.ts" ]
                )

        // A package made of the ES library files declares the library itself
        if noLib then
            compilerOptions.noLib <- Some true

        let project = host.createProject compilerOptions

        for entryFile in entryFiles do
            project.addSourceFileAtPathSync entryFile |> ignore

        project.resolveSourceFileDependencies ()
        project.createProgram ()

    /// The files reachable from the entry files through imports and references, without going
    /// through a package of `excludedRuntimeNames` (`node` for `@types/node`)
    let reachableFiles
        (host: Host)
        (program: Ts.Program)
        (entryFiles: string[])
        (excludedRuntimeNames: string[])
        : string[]
        =
        let runtimeNames = Collections.Generic.Dictionary<string, string option>()

        let isExcluded (fileName: string) =
            match Resolve.findPackageDir host fileName with
            | None -> false
            | Some dir ->
                if not (runtimeNames.ContainsKey dir) then
                    runtimeNames[dir] <-
                        (Resolve.describePackage host dir |> Option.map (fun d -> d.runtimeName))

                match runtimeNames[dir] with
                | Some runtimeName -> Array.contains runtimeName excludedRuntimeNames
                | None -> false

        let seen = Collections.Generic.HashSet<string>()
        let queue = Collections.Generic.List<string>(entryFiles)

        let visit (fileName: string) =
            if
                not (isNullish fileName)
                && not (seen.Contains fileName)
                && not (isExcluded fileName)
            then
                queue.Add fileName

        while queue.Count > 0 do
            let fileName = queue[queue.Count - 1]
            queue.RemoveAt(queue.Count - 1)

            if not (seen.Contains fileName) then
                match program.getSourceFile fileName with
                | None -> ()
                | Some sourceFile ->
                    seen.Add fileName |> ignore

                    let imports: obj[] =
                        match sourceFile?imports with
                        | i when not (isNullish i) -> unbox i
                        | _ -> [||]

                    for usage in imports do
                        let resolvedFileName: string =
                            emitJsExpr
                                (program, usage, sourceFile)
                                "$0.getResolvedModuleFromModuleSpecifier($1, $2)?.resolvedModule?.resolvedFileName"

                        visit resolvedFileName

                    for reference in sourceFile.referencedFiles do
                        visit (
                            host.path.resolve
                                [| host.path.dirname sourceFile.fileName; reference.fileName |]
                        )

                    for reference in sourceFile.typeReferenceDirectives do
                        let resolvedFileName: string =
                            emitJsExpr
                                (program, sourceFile, reference)
                                "$0.getResolvedTypeReferenceDirective($1, $2.fileName, $2.resolutionMode)?.resolvedTypeReferenceDirective?.resolvedFileName"

                        visit resolvedFileName

        seen |> Seq.toArray
