// ts2fable 0.8.0
module rec Glutinum.FastGlob

open System
open Fable.Core
open Fable.Core.JS

let [<Import("default","fast-glob")>] fastGlob: FastGlob.IExports = jsNative

type Pattern = string

type [<AllowNullLiteral>] Options =
    /// <summary>Return the absolute path for entries.</summary>
    /// <default>false</default>
    abstract absolute: bool option with get, set
    /// <summary>
    /// If set to <c>true</c>, then patterns without slashes will be matched against
    /// the basename of the path if it contains slashes.
    /// </summary>
    /// <default>false</default>
    abstract baseNameMatch: bool option with get, set
    /// <summary>Enables Bash-like brace expansion.</summary>
    /// <default>true</default>
    abstract braceExpansion: bool option with get, set
    /// <summary>Enables a case-sensitive mode for matching files.</summary>
    /// <default>true</default>
    abstract caseSensitiveMatch: bool option with get, set
    /// <summary>
    /// Specifies the maximum number of concurrent requests from a reader to read
    /// directories.
    /// </summary>
    /// <default>os.cpus().length</default>
    abstract concurrency: float option with get, set
    /// <summary>The current working directory in which to search.</summary>
    /// <default>process.cwd()</default>
    abstract cwd: string option with get, set
    /// <summary>
    /// Specifies the maximum depth of a read directory relative to the start
    /// directory.
    /// </summary>
    /// <default>Infinity</default>
    abstract deep: float option with get, set
    /// <summary>Allow patterns to match entries that begin with a period (<c>.</c>).</summary>
    /// <default>false</default>
    abstract dot: bool option with get, set
    /// <summary>Enables Bash-like <c>extglob</c> functionality.</summary>
    /// <default>true</default>
    abstract extglob: bool option with get, set
    /// <summary>Indicates whether to traverse descendants of symbolic link directories.</summary>
    /// <default>true</default>
    abstract followSymbolicLinks: bool option with get, set
    /// <summary>Custom implementation of methods for working with the file system.</summary>
    /// <default>fs.*</default>
    abstract fs: obj option with get, set
    /// <summary>
    /// Enables recursively repeats a pattern containing <c>**</c>.
    /// If <c>false</c>, <c>**</c> behaves exactly like <c>*</c>.
    /// </summary>
    /// <default>true</default>
    abstract globstar: bool option with get, set
    /// <summary>
    /// An array of glob patterns to exclude matches.
    /// This is an alternative way to use negative patterns.
    /// </summary>
    /// <default>[]</default>
    abstract ignore: ResizeArray<Pattern> option with get, set
    /// <summary>Mark the directory path with the final slash.</summary>
    /// <default>false</default>
    abstract markDirectories: bool option with get, set
    /// <summary>Returns objects (instead of strings) describing entries.</summary>
    /// <default>false</default>
    abstract objectMode: bool option with get, set
    /// <summary>Return only directories.</summary>
    /// <default>false</default>
    abstract onlyDirectories: bool option with get, set
    /// <summary>Return only files.</summary>
    /// <default>true</default>
    abstract onlyFiles: bool option with get, set
    /// <summary>Enables an object mode (<c>objectMode</c>) with an additional <c>stats</c> field.</summary>
    /// <default>false</default>
    abstract stats: bool option with get, set
    /// <summary>
    /// By default this package suppress only <c>ENOENT</c> errors.
    /// Set to <c>true</c> to suppress any error.
    /// </summary>
    /// <default>false</default>
    abstract suppressErrors: bool option with get, set
    /// <summary>
    /// Throw an error when symbolic link is broken if <c>true</c> or safely
    /// return <c>lstat</c> call if <c>false</c>.
    /// </summary>
    /// <default>false</default>
    abstract throwErrorOnBrokenSymbolicLink: bool option with get, set
    /// <summary>Ensures that the returned entries are unique.</summary>
    /// <default>true</default>
    abstract unique: bool option with get, set

type [<AllowNullLiteral>] IExports =
    [<Emit("$0($1...)")>]
    abstract Invoke: source: Pattern * ?options: Options -> Promise<ResizeArray<string>>
    [<Emit("$0($1...)")>]
    abstract Invoke: source: ResizeArray<Pattern> * ?options: Options -> Promise<ResizeArray<string>>
