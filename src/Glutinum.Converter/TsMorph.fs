module rec TsMorph

#nowarn "44"

open Fable.Core
open Fable.Core.JsInterop
open System
open TypeScript

/// A `readDirSync` entry, from `@ts-morph/common`
type RuntimeDirEntry =
    abstract member name: string with get, set
    abstract member isFile: bool with get, set
    abstract member isDirectory: bool with get, set
    abstract member isSymlink: bool with get, set

[<AbstractClass>]
[<Erase>]
type Exports =
    /// <summary>
    /// Collection of reusable resolution hosts.
    /// </summary>
    [<Import("ResolutionHosts", "@ts-morph/bootstrap")>]
    static member inline ResolutionHosts: Exports.ResolutionHosts__.Type = nativeOnly

    /// <summary>
    /// Asynchronously creates a new collection of source files to analyze.
    /// </summary>
    /// <param name="options">
    /// Options for creating the project.
    /// </param>
    [<Import("createProject", "@ts-morph/bootstrap")>]
    static member createProject(?options: ProjectOptions) : JS.Promise<Project> = nativeOnly

    /// <summary>
    /// Synchronously creates a new collection of source files to analyze.
    /// </summary>
    /// <param name="options">
    /// Options for creating the project.
    /// </param>
    [<Import("createProjectSync", "@ts-morph/bootstrap")>]
    static member createProjectSync(?options: ProjectOptions) : Project = nativeOnly

    [<Import("CompilerOptionsContainer", "@ts-morph/bootstrap"); EmitConstructor>]
    static member CompilerOptionsContainer
        (?defaultSettings: Ts.CompilerOptions)
        : CompilerOptionsContainer
        =
        nativeOnly

    /// <summary>
    /// Constructor.
    /// </summary>
    [<Import("InMemoryFileSystemHost", "@ts-morph/bootstrap"); EmitConstructor>]
    static member InMemoryFileSystemHost() : InMemoryFileSystemHost = nativeOnly

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="defaultSettings">
    /// The settings to use by default.
    /// </param>
    [<Import("SettingsContainer", "@ts-morph/bootstrap"); EmitConstructor>]
    static member SettingsContainer(defaultSettings: obj) : SettingsContainer = nativeOnly

    [<Import("Project", "@ts-morph/bootstrap"); EmitConstructor>]
    static member Project() : Project = nativeOnly

/// <summary>
/// Holds the compiler options.
/// </summary>
[<AllowNullLiteral>]
[<Interface>]
type CompilerOptionsContainer =
    inherit SettingsContainer<Ts.CompilerOptions>
    /// <summary>
    /// Sets one or all of the compiler options.
    ///
    /// WARNING: Setting the compiler options will cause a complete reparse of all the source files.
    /// </summary>
    /// <param name="settings">
    /// Compiler options to set.
    /// </param>
    abstract member set: settings: CompilerOptionsContainer.set.settings -> unit
    /// <summary>
    /// Gets the encoding from the compiler options or returns utf-8.
    /// </summary>
    abstract member getEncoding: unit -> string

/// <summary>
/// Represents a file system that can be interacted with.
/// </summary>
[<AllowNullLiteral>]
[<Interface>]
type FileSystemHost =
    /// <summary>
    /// Gets if this file system is case sensitive.
    /// </summary>
    abstract member isCaseSensitive: unit -> bool
    /// <summary>
    /// Asynchronously deletes the specified file or directory.
    /// </summary>
    abstract member delete: path: string -> JS.Promise<unit>
    /// <summary>
    /// Synchronously deletes the specified file or directory
    /// </summary>
    abstract member deleteSync: path: string -> unit
    /// <summary>
    /// Reads all the child directories and files.
    /// </summary>
    /// <remarks>
    /// Implementers should have this return the full file path.
    /// </remarks>
    abstract member readDirSync: dirPath: string -> ResizeArray<RuntimeDirEntry>
    /// <summary>
    /// Asynchronously reads a file at the specified path.
    /// </summary>
    abstract member readFile: filePath: string * ?encoding: string -> JS.Promise<string>
    /// <summary>
    /// Synchronously reads a file at the specified path.
    /// </summary>
    abstract member readFileSync: filePath: string * ?encoding: string -> string
    /// <summary>
    /// Asynchronously writes a file to the file system.
    /// </summary>
    abstract member writeFile: filePath: string * fileText: string -> JS.Promise<unit>
    /// <summary>
    /// Synchronously writes a file to the file system.
    /// </summary>
    abstract member writeFileSync: filePath: string * fileText: string -> unit
    /// <summary>
    /// Asynchronously creates a directory at the specified path.
    /// </summary>
    abstract member mkdir: dirPath: string -> JS.Promise<unit>
    /// <summary>
    /// Synchronously creates a directory at the specified path.
    /// </summary>
    abstract member mkdirSync: dirPath: string -> unit
    /// <summary>
    /// Asynchronously moves a file or directory.
    /// </summary>
    abstract member move: srcPath: string * destPath: string -> JS.Promise<unit>
    /// <summary>
    /// Synchronously moves a file or directory.
    /// </summary>
    abstract member moveSync: srcPath: string * destPath: string -> unit
    /// <summary>
    /// Asynchronously copies a file or directory.
    /// </summary>
    abstract member copy: srcPath: string * destPath: string -> JS.Promise<unit>
    /// <summary>
    /// Synchronously copies a file or directory.
    /// </summary>
    abstract member copySync: srcPath: string * destPath: string -> unit
    /// <summary>
    /// Asynchronously checks if a file exists.
    /// </summary>
    /// <remarks>
    /// Implementers should throw an <c>errors.FileNotFoundError</c> when it does not exist.
    /// </remarks>
    abstract member fileExists: filePath: string -> JS.Promise<bool>
    /// <summary>
    /// Synchronously checks if a file exists.
    /// </summary>
    /// <remarks>
    /// Implementers should throw an <c>errors.FileNotFoundError</c> when it does not exist.
    /// </remarks>
    abstract member fileExistsSync: filePath: string -> bool
    /// <summary>
    /// Asynchronously checks if a directory exists.
    /// </summary>
    abstract member directoryExists: dirPath: string -> JS.Promise<bool>
    /// <summary>
    /// Synchronously checks if a directory exists.
    /// </summary>
    abstract member directoryExistsSync: dirPath: string -> bool
    /// <summary>
    /// See https://nodejs.org/api/fs.html#fs_fs_realpathsync_path_options
    /// </summary>
    abstract member realpathSync: path: string -> string
    /// <summary>
    /// Gets the current directory of the environment.
    /// </summary>
    abstract member getCurrentDirectory: unit -> string
    /// <summary>
    /// Uses pattern matching to find files or directories.
    /// </summary>
    abstract member glob: patterns: ResizeArray<string> -> JS.Promise<ResizeArray<string>>
    /// <summary>
    /// Synchronously uses pattern matching to find files or directories.
    /// </summary>
    abstract member globSync: patterns: ResizeArray<string> -> ResizeArray<string>

/// <summary>
/// An implementation of a file system that exists in memory only.
/// </summary>
[<AllowNullLiteral>]
[<Interface>]
type InMemoryFileSystemHost =
    inherit FileSystemHost
    /// <summary>
    /// Gets if this file system is case sensitive.
    /// </summary>
    abstract member isCaseSensitive: unit -> bool
    /// <summary>
    /// Asynchronously deletes the specified file or directory.
    /// </summary>
    abstract member delete: path: string -> JS.Promise<unit>
    /// <summary>
    /// Synchronously deletes the specified file or directory
    /// </summary>
    abstract member deleteSync: path: string -> unit
    /// <summary>
    /// Reads all the child directories and files.
    /// </summary>
    abstract member readDirSync: dirPath: string -> ResizeArray<RuntimeDirEntry>
    /// <summary>
    /// Asynchronously reads a file at the specified path.
    /// </summary>
    abstract member readFile: filePath: string * ?encoding: string -> JS.Promise<string>
    /// <summary>
    /// Synchronously reads a file at the specified path.
    /// </summary>
    abstract member readFileSync: filePath: string * ?encoding: string -> string
    /// <summary>
    /// Asynchronously writes a file to the file system.
    /// </summary>
    abstract member writeFile: filePath: string * fileText: string -> JS.Promise<unit>
    /// <summary>
    /// Synchronously writes a file to the file system.
    /// </summary>
    abstract member writeFileSync: filePath: string * fileText: string -> unit
    /// <summary>
    /// Asynchronously creates a directory at the specified path.
    /// </summary>
    abstract member mkdir: dirPath: string -> JS.Promise<unit>
    /// <summary>
    /// Synchronously creates a directory at the specified path.
    /// </summary>
    abstract member mkdirSync: dirPath: string -> unit
    /// <summary>
    /// Asynchronously moves a file or directory.
    /// </summary>
    abstract member move: srcPath: string * destPath: string -> JS.Promise<unit>
    /// <summary>
    /// Synchronously moves a file or directory.
    /// </summary>
    abstract member moveSync: srcPath: string * destPath: string -> unit
    /// <summary>
    /// Asynchronously copies a file or directory.
    /// </summary>
    abstract member copy: srcPath: string * destPath: string -> JS.Promise<unit>
    /// <summary>
    /// Synchronously copies a file or directory.
    /// </summary>
    abstract member copySync: srcPath: string * destPath: string -> unit
    /// <summary>
    /// Asynchronously checks if a file exists.
    /// </summary>
    abstract member fileExists: filePath: string -> JS.Promise<bool>
    /// <summary>
    /// Synchronously checks if a file exists.
    /// </summary>
    abstract member fileExistsSync: filePath: string -> bool
    /// <summary>
    /// Asynchronously checks if a directory exists.
    /// </summary>
    abstract member directoryExists: dirPath: string -> JS.Promise<bool>
    /// <summary>
    /// Synchronously checks if a directory exists.
    /// </summary>
    abstract member directoryExistsSync: dirPath: string -> bool
    /// <summary>
    /// See https://nodejs.org/api/fs.html#fs_fs_realpathsync_path_options
    /// </summary>
    abstract member realpathSync: path: string -> string
    /// <summary>
    /// Gets the current directory of the environment.
    /// </summary>
    abstract member getCurrentDirectory: unit -> string
    /// <summary>
    /// Uses pattern matching to find files or directories.
    /// </summary>
    abstract member glob: patterns: ResizeArray<string> -> JS.Promise<ResizeArray<string>>
    /// <summary>
    /// Synchronously uses pattern matching to find files or directories.
    /// </summary>
    abstract member globSync: patterns: ResizeArray<string> -> ResizeArray<string>

/// <summary>
/// Host for implementing custom module and/or type reference directive resolution.
/// </summary>
[<AllowNullLiteral>]
[<Interface>]
type ResolutionHost =
    abstract member resolveModuleNames: ResolutionHost.resolveModuleNames option with get, set

    abstract member getResolvedModuleWithFailedLookupLocationsFromCache:
        ResolutionHost.getResolvedModuleWithFailedLookupLocationsFromCache option with get, set

    abstract member resolveTypeReferenceDirectives:
        ResolutionHost.resolveTypeReferenceDirectives option with get, set

/// <summary>
/// Factory used to create a resolution host.
/// </summary>
/// <remarks>
/// The compiler options are retrieved via a function in order to get the project's current compiler options.
/// </remarks>
type ResolutionHostFactory =
    delegate of
        moduleResolutionHost: Ts.ModuleResolutionHost *
        getCompilerOptions: (unit -> Ts.CompilerOptions) ->
            ResolutionHost

[<AllowNullLiteral>]
[<Interface>]
type SettingsContainer<'T> =
    abstract member _settings: 'T with get, set
    /// <summary>
    /// Resets the settings to the default.
    /// </summary>
    abstract member reset: unit -> unit
    /// <summary>
    /// Gets a copy of the settings as an object.
    /// </summary>
    abstract member get: unit -> 'T
    /// <summary>
    /// Sets one or all of the settings.
    /// </summary>
    /// <param name="settings">
    /// Settings to set.
    /// </param>
    abstract member set: settings: SettingsContainer.set.settings -> unit
    /// <summary>
    /// Subscribe to modifications in the settings container.
    /// </summary>
    /// <param name="action">
    /// Action to execute when the settings change.
    /// </param>
    abstract member onModified: action: (unit -> unit) -> unit

type SettingsContainer = SettingsContainer<obj>

/// <summary>
/// Options for creating a project.
/// </summary>
[<AllowNullLiteral>]
[<Interface>]
type ProjectOptions =
    /// <summary>
    /// Compiler options
    /// </summary>
    abstract member compilerOptions: Ts.CompilerOptions option with get, set
    /// <summary>
    /// File path to the tsconfig.json file.
    /// </summary>
    abstract member tsConfigFilePath: string option with get, set
    /// <summary>
    /// Whether to skip adding source files from the specified tsconfig.json.
    /// </summary>
    abstract member skipAddingFilesFromTsConfig: bool option with get, set
    /// <summary>
    /// Skip resolving file dependencies when providing a ts config file path and adding the files from tsconfig.
    /// </summary>
    abstract member skipFileDependencyResolution: bool option with get, set
    /// <summary>
    /// Skip loading the lib files. Unlike the compiler API, ts-morph does not load these
    /// from the node_modules folder, but instead loads them from some other JS code
    /// and uses a fake path for their existence. If you want to use a custom lib files
    /// folder path, then provide one using the libFolderPath options.
    /// </summary>
    abstract member skipLoadingLibFiles: bool option with get, set
    /// <summary>
    /// The folder to use for loading lib files.
    /// </summary>
    abstract member libFolderPath: string option with get, set
    /// <summary>
    /// Whether to use an in-memory file system.
    /// </summary>
    abstract member useInMemoryFileSystem: bool option with get, set
    /// <summary>
    /// Optional file system host. Useful for mocking access to the file system.
    /// </summary>
    /// <remarks>
    /// Consider using <c>useInMemoryFileSystem</c> instead.
    /// </remarks>
    abstract member fileSystem: FileSystemHost option with get, set
    /// <summary>
    /// Creates a resolution host for specifying custom module and/or type reference directive resolution.
    /// </summary>
    abstract member resolutionHost: ResolutionHostFactory option with get, set
    /// <summary>
    /// Unstable and will probably be removed in the future.
    /// I believe this option should be internal to the library and if you know how to achieve
    /// that then please consider submitting a PR.
    /// </summary>
    abstract member isKnownTypesPackageName: (string -> bool) option with get, set

    [<ParamObject; Emit("$0")>]
    static member Create
        (
            ?compilerOptions: Ts.CompilerOptions,
            ?tsConfigFilePath: string,
            ?skipAddingFilesFromTsConfig: bool,
            ?skipFileDependencyResolution: bool,
            ?skipLoadingLibFiles: bool,
            ?libFolderPath: string,
            ?useInMemoryFileSystem: bool,
            ?fileSystem: FileSystemHost,
            ?resolutionHost: ResolutionHostFactory,
            ?isKnownTypesPackageName: (string -> bool)
        )
        : ProjectOptions
        =
        nativeOnly

/// <summary>
/// Project that holds source files.
/// </summary>
[<AllowNullLiteral>]
[<Interface>]
type Project =
    /// <summary>
    /// Gets the compiler options for modification.
    /// </summary>
    abstract member compilerOptions: CompilerOptionsContainer with get
    /// <summary>
    /// Gets the file system host used for this project.
    /// </summary>
    abstract member fileSystem: FileSystemHost with get

    /// <summary>
    /// Asynchronously adds an existing source file from a file path or throws if it doesn't exist.
    ///
    /// Will return the source file if it was already added.
    /// </summary>
    /// <remarks>
    /// Throws:
    /// -------
    ///
    /// FileNotFoundError when the file is not found.
    /// </remarks>
    /// <param name="filePath">
    /// File path to get the file from.
    /// </param>
    /// <param name="options">
    /// Options for adding the file.
    /// </param>
    abstract member addSourceFileAtPath:
        filePath: string * ?options: Project.addSourceFileAtPath.options ->
            JS.Promise<Ts.SourceFile>

    /// <summary>
    /// Synchronously adds an existing source file from a file path or throws if it doesn't exist.
    ///
    /// Will return the source file if it was already added.
    /// </summary>
    /// <remarks>
    /// Throws:
    /// -------
    ///
    /// FileNotFoundError when the file is not found.
    /// </remarks>
    /// <param name="filePath">
    /// File path to get the file from.
    /// </param>
    /// <param name="options">
    /// Options for adding the file.
    /// </param>
    abstract member addSourceFileAtPathSync:
        filePath: string * ?options: Project.addSourceFileAtPathSync.options -> Ts.SourceFile

    /// <summary>
    /// Asynchronously adds a source file from a file path if it exists or returns undefined.
    ///
    /// Will return the source file if it was already added.
    /// </summary>
    /// <param name="filePath">
    /// File path to get the file from.
    /// </param>
    /// <param name="options">
    /// Options for adding the file.
    /// </param>
    abstract member addSourceFileAtPathIfExists:
        filePath: string * ?options: Project.addSourceFileAtPathIfExists.options ->
            JS.Promise<Ts.SourceFile option>

    /// <summary>
    /// Synchronously adds a source file from a file path if it exists or returns undefined.
    ///
    /// Will return the source file if it was already added.
    /// </summary>
    /// <param name="filePath">
    /// File path to get the file from.
    /// </param>
    /// <param name="options">
    /// Options for adding the file.
    /// </param>
    abstract member addSourceFileAtPathIfExistsSync:
        filePath: string * ?options: Project.addSourceFileAtPathIfExistsSync.options ->
            Ts.SourceFile option

    /// <summary>
    /// Asynchronously adds source files based on file globs.
    /// </summary>
    /// <param name="fileGlobs">
    /// File glob or globs to add files based on.
    /// </param>
    /// <returns>
    /// The matched source files.
    /// </returns>
    abstract member addSourceFilesByPaths:
        fileGlobs: string -> JS.Promise<ResizeArray<Ts.SourceFile>>

    /// <summary>
    /// Asynchronously adds source files based on file globs.
    /// </summary>
    /// <param name="fileGlobs">
    /// File glob or globs to add files based on.
    /// </param>
    /// <returns>
    /// The matched source files.
    /// </returns>
    abstract member addSourceFilesByPaths:
        fileGlobs: ResizeArray<string> -> JS.Promise<ResizeArray<Ts.SourceFile>>

    /// <summary>
    /// Synchronously adds source files based on file globs.
    /// </summary>
    /// <remarks>
    /// This is much slower than the asynchronous version.
    /// </remarks>
    /// <param name="fileGlobs">
    /// File glob or globs to add files based on.
    /// </param>
    /// <returns>
    /// The matched source files.
    /// </returns>
    abstract member addSourceFilesByPathsSync: fileGlobs: string -> ResizeArray<Ts.SourceFile>

    /// <summary>
    /// Synchronously adds source files based on file globs.
    /// </summary>
    /// <remarks>
    /// This is much slower than the asynchronous version.
    /// </remarks>
    /// <param name="fileGlobs">
    /// File glob or globs to add files based on.
    /// </param>
    /// <returns>
    /// The matched source files.
    /// </returns>
    abstract member addSourceFilesByPathsSync:
        fileGlobs: ResizeArray<string> -> ResizeArray<Ts.SourceFile>

    /// <summary>
    /// Asynchronously adds all the source files from the specified tsconfig.json.
    ///
    /// Note that this is done by default when specifying a tsconfig file in the constructor and not explicitly setting the
    /// <c>skipAddingSourceFilesFromTsConfig</c> option to <c>true</c>.
    /// </summary>
    /// <param name="tsConfigFilePath">
    /// File path to the tsconfig.json file.
    /// </param>
    abstract member addSourceFilesFromTsConfig:
        tsConfigFilePath: string -> JS.Promise<ResizeArray<Ts.SourceFile>>

    /// <summary>
    /// Synchronously adds all the source files from the specified tsconfig.json.
    ///
    /// Note that this is done by default when specifying a tsconfig file in the constructor and not explicitly setting the
    /// <c>skipAddingSourceFilesFromTsConfig</c> option to <c>true</c>.
    /// </summary>
    /// <param name="tsConfigFilePath">
    /// File path to the tsconfig.json file.
    /// </param>
    abstract member addSourceFilesFromTsConfigSync:
        tsConfigFilePath: string -> ResizeArray<Ts.SourceFile>

    /// <summary>
    /// Creates a source file at the specified file path with the specified text.
    ///
    /// Note: The file will not be created and saved to the file system until .save() is called on the source file.
    /// </summary>
    /// <remarks>
    /// Throws:
    /// -------
    ///
    /// - InvalidOperationError if a source file already exists at the provided file path.
    /// </remarks>
    /// <param name="filePath">
    /// File path of the source file.
    /// </param>
    /// <param name="sourceFileText">
    /// Text to use for the source file.
    /// </param>
    /// <param name="options">
    /// Options.
    /// </param>
    abstract member createSourceFile:
        filePath: string * ?sourceFileText: string * ?options: Project.createSourceFile.options ->
            Ts.SourceFile

    /// <summary>
    /// Updates the source file stored in the project at the specified path.
    /// Updates the source file stored in the project. The <c>fileName</c> of the source file object is used to tell which file to update.
    /// </summary>
    /// <param name="filePath">
    /// File path of the source file.
    /// </param>
    /// <param name="sourceFileText">
    /// Text of the source file.
    /// </param>
    /// <param name="options">
    /// Options for updating the source file.
    /// </param>
    abstract member updateSourceFile:
        filePath: string * sourceFileText: string * ?options: Project.updateSourceFile.options ->
            Ts.SourceFile

    /// <summary>
    /// Updates the source file stored in the project at the specified path.
    /// Updates the source file stored in the project. The <c>fileName</c> of the source file object is used to tell which file to update.
    /// </summary>
    /// <param name="newSourceFile">
    /// The new source file.
    /// </param>
    abstract member updateSourceFile: newSourceFile: Ts.SourceFile -> Ts.SourceFile
    /// <summary>
    /// Removes the source file at the provided file path.
    /// Removes the provided source file based on its <c>fileName</c>.
    /// </summary>
    /// <param name="filePath">
    /// File path of the source file.
    /// </param>
    abstract member removeSourceFile: filePath: string -> unit
    /// <summary>
    /// Removes the source file at the provided file path.
    /// Removes the provided source file based on its <c>fileName</c>.
    /// </summary>
    /// <param name="sourceFile">
    /// Source file to remove.
    /// </param>
    abstract member removeSourceFile: sourceFile: Ts.SourceFile -> unit
    /// <summary>
    /// Adds the source files the project's source files depend on to the project.
    /// </summary>
    /// <remarks>
    /// * This should be done after source files are added to the project, preferably once to
    /// avoid doing more work than necessary.
    /// * This is done by default when creating a Project and providing a tsconfig.json and
    /// not specifying to not add the source files.
    /// </remarks>
    abstract member resolveSourceFileDependencies: unit -> unit
    /// <summary>
    /// Creates a new program.
    /// Note: You should get a new program any time source files are added, removed, or changed.
    /// </summary>
    abstract member createProgram: ?options: Ts.CreateProgramOptions -> Ts.Program
    /// <summary>
    /// Gets the language service.
    /// </summary>
    abstract member getLanguageService: unit -> Ts.LanguageService
    /// <summary>
    /// Gets a source file by a file name or file path. Throws an error if it doesn't exist.
    /// Gets a source file by a search function. Throws an error if it doesn't exist.
    /// </summary>
    /// <param name="fileNameOrPath">
    /// File name or path that the path could end with or equal.
    /// </param>
    abstract member getSourceFileOrThrow: fileNameOrPath: string -> Ts.SourceFile
    /// <summary>
    /// Gets a source file by a file name or file path. Throws an error if it doesn't exist.
    /// Gets a source file by a search function. Throws an error if it doesn't exist.
    /// </summary>
    /// <param name="searchFunction">
    /// Search function.
    /// </param>
    abstract member getSourceFileOrThrow: searchFunction: (Ts.SourceFile -> bool) -> Ts.SourceFile
    /// <summary>
    /// Gets a source file by a file name or file path. Returns undefined if none exists.
    /// Gets a source file by a search function. Returns undefined if none exists.
    /// </summary>
    /// <param name="fileNameOrPath">
    /// File name or path that the path could end with or equal.
    /// </param>
    abstract member getSourceFile: fileNameOrPath: string -> Ts.SourceFile option
    /// <summary>
    /// Gets a source file by a file name or file path. Returns undefined if none exists.
    /// Gets a source file by a search function. Returns undefined if none exists.
    /// </summary>
    /// <param name="searchFunction">
    /// Search function.
    /// </param>
    abstract member getSourceFile: searchFunction: (Ts.SourceFile -> bool) -> Ts.SourceFile option
    /// <summary>
    /// Gets the source files in the project.
    /// </summary>
    abstract member getSourceFiles: unit -> ResizeArray<Ts.SourceFile>

    /// <summary>
    /// Formats an array of diagnostics with their color and context into a string.
    /// </summary>
    /// <param name="diagnostics">
    /// Diagnostics to get a string of.
    /// </param>
    /// <param name="options">
    /// Collection of options. For example, the new line character to use (defaults to the OS' new line character).
    /// </param>
    abstract member formatDiagnosticsWithColorAndContext:
        diagnostics: ResizeArray<Ts.Diagnostic> *
        ?opts: Project.formatDiagnosticsWithColorAndContext.opts ->
            string

    /// <summary>
    /// Gets a ts.ModuleResolutionHost for the project.
    /// </summary>
    abstract member getModuleResolutionHost: unit -> Ts.ModuleResolutionHost

module CompilerOptionsContainer =

    module set =

        [<AllowNullLiteral>]
        [<Interface>]
        type settings =
            abstract member allowImportingTsExtensions: bool option with get, set
            abstract member allowJs: bool option with get, set
            abstract member allowArbitraryExtensions: bool option with get, set
            abstract member allowSyntheticDefaultImports: bool option with get, set
            abstract member allowUmdGlobalAccess: bool option with get, set
            abstract member allowUnreachableCode: bool option with get, set
            abstract member allowUnusedLabels: bool option with get, set
            abstract member alwaysStrict: bool option with get, set

            [<Obsolete>]
            abstract member baseUrl: string option with get, set

            [<Obsolete>]
            abstract member charset: string option with get, set

            abstract member checkJs: bool option with get, set
            abstract member customConditions: ResizeArray<string> option with get, set
            abstract member declaration: bool option with get, set
            abstract member declarationMap: bool option with get, set
            abstract member emitDeclarationOnly: bool option with get, set
            abstract member declarationDir: string option with get, set
            abstract member disableSizeLimit: bool option with get, set
            abstract member disableSourceOfProjectReferenceRedirect: bool option with get, set
            abstract member disableSolutionSearching: bool option with get, set
            abstract member disableReferencedProjectLoad: bool option with get, set

            [<Obsolete>]
            abstract member downlevelIteration: bool option with get, set

            abstract member emitBOM: bool option with get, set
            abstract member emitDecoratorMetadata: bool option with get, set
            abstract member exactOptionalPropertyTypes: bool option with get, set
            abstract member experimentalDecorators: bool option with get, set
            abstract member forceConsistentCasingInFileNames: bool option with get, set
            abstract member ignoreDeprecations: string option with get, set
            abstract member importHelpers: bool option with get, set

            [<Obsolete>]
            abstract member importsNotUsedAsValues: Ts.ImportsNotUsedAsValues option with get, set

            abstract member inlineSourceMap: bool option with get, set
            abstract member inlineSources: bool option with get, set
            abstract member isolatedModules: bool option with get, set
            abstract member isolatedDeclarations: bool option with get, set
            abstract member jsx: Ts.JsxEmit option with get, set

            [<Obsolete>]
            abstract member keyofStringsOnly: bool option with get, set

            abstract member lib: ResizeArray<string> option with get, set
            abstract member libReplacement: bool option with get, set
            abstract member locale: string option with get, set
            abstract member mapRoot: string option with get, set
            abstract member maxNodeModuleJsDepth: float option with get, set
            abstract member ``module``: Ts.ModuleKind option with get, set
            abstract member moduleResolution: Ts.ModuleResolutionKind option with get, set
            abstract member moduleSuffixes: ResizeArray<string> option with get, set
            abstract member moduleDetection: Ts.ModuleDetectionKind option with get, set
            abstract member newLine: Ts.NewLineKind option with get, set
            abstract member noEmit: bool option with get, set
            abstract member noCheck: bool option with get, set
            abstract member noEmitHelpers: bool option with get, set
            abstract member noEmitOnError: bool option with get, set
            abstract member noErrorTruncation: bool option with get, set
            abstract member noFallthroughCasesInSwitch: bool option with get, set
            abstract member noImplicitAny: bool option with get, set
            abstract member noImplicitReturns: bool option with get, set
            abstract member noImplicitThis: bool option with get, set

            [<Obsolete>]
            abstract member noStrictGenericChecks: bool option with get, set

            abstract member noUnusedLocals: bool option with get, set
            abstract member noUnusedParameters: bool option with get, set

            [<Obsolete>]
            abstract member noImplicitUseStrict: bool option with get, set

            abstract member noPropertyAccessFromIndexSignature: bool option with get, set
            abstract member assumeChangesOnlyAffectDirectDependencies: bool option with get, set
            abstract member noLib: bool option with get, set
            abstract member noResolve: bool option with get, set
            abstract member noUncheckedIndexedAccess: bool option with get, set

            [<Obsolete>]
            abstract member out: string option with get, set

            abstract member outDir: string option with get, set
            abstract member outFile: string option with get, set
            abstract member paths: Ts.MapLike<ResizeArray<string>> option with get, set
            abstract member preserveConstEnums: bool option with get, set
            abstract member noImplicitOverride: bool option with get, set
            abstract member preserveSymlinks: bool option with get, set

            [<Obsolete>]
            abstract member preserveValueImports: bool option with get, set

            abstract member project: string option with get, set
            abstract member reactNamespace: string option with get, set
            abstract member jsxFactory: string option with get, set
            abstract member jsxFragmentFactory: string option with get, set
            abstract member jsxImportSource: string option with get, set
            abstract member composite: bool option with get, set
            abstract member incremental: bool option with get, set
            abstract member tsBuildInfoFile: string option with get, set
            abstract member removeComments: bool option with get, set
            abstract member resolvePackageJsonExports: bool option with get, set
            abstract member resolvePackageJsonImports: bool option with get, set
            abstract member rewriteRelativeImportExtensions: bool option with get, set
            abstract member rootDir: string option with get, set
            abstract member rootDirs: ResizeArray<string> option with get, set
            abstract member skipLibCheck: bool option with get, set
            abstract member skipDefaultLibCheck: bool option with get, set
            abstract member sourceMap: bool option with get, set
            abstract member sourceRoot: string option with get, set
            abstract member strict: bool option with get, set
            abstract member strictFunctionTypes: bool option with get, set
            abstract member strictBindCallApply: bool option with get, set
            abstract member strictNullChecks: bool option with get, set
            abstract member strictPropertyInitialization: bool option with get, set
            abstract member strictBuiltinIteratorReturn: bool option with get, set
            abstract member stripInternal: bool option with get, set

            [<Obsolete>]
            abstract member suppressExcessPropertyErrors: bool option with get, set

            [<Obsolete>]
            abstract member suppressImplicitAnyIndexErrors: bool option with get, set

            abstract member target: Ts.ScriptTarget option with get, set
            abstract member traceResolution: bool option with get, set
            abstract member useUnknownInCatchVariables: bool option with get, set
            abstract member noUncheckedSideEffectImports: bool option with get, set
            abstract member resolveJsonModule: bool option with get, set
            abstract member types: ResizeArray<string> option with get, set
            /// <summary>
            /// Paths used to compute primary types search locations
            /// </summary>
            abstract member typeRoots: ResizeArray<string> option with get, set
            abstract member verbatimModuleSyntax: bool option with get, set
            abstract member erasableSyntaxOnly: bool option with get, set
            abstract member esModuleInterop: bool option with get, set
            abstract member useDefineForClassFields: bool option with get, set

module ResolutionHost =

    type resolveModuleNames =
        delegate of
            moduleNames: ResizeArray<string> *
            containingFile: string *
            reusedNames: ResizeArray<string> option *
            redirectedReference: Ts.ResolvedProjectReference option *
            options: Ts.CompilerOptions *
            ?containingSourceFile: Ts.SourceFile ->
                ResizeArray<Ts.ResolvedModule option>

    type getResolvedModuleWithFailedLookupLocationsFromCache =
        delegate of
            modulename: string * containingFile: string * ?resolutionMode: Ts.ResolutionMode ->
                Ts.ResolvedModuleWithFailedLookupLocations option

    type resolveTypeReferenceDirectives =
        delegate of
            typeDirectiveNames: U2<ResizeArray<string>, ResizeArray<Ts.FileReference>> *
            containingFile: string *
            redirectedReference: Ts.ResolvedProjectReference option *
            options: Ts.CompilerOptions *
            ?containingFileMode: Ts.ResolutionMode ->
                ResizeArray<Ts.ResolvedTypeReferenceDirective option>

module SettingsContainer =

    module set =

        [<AllowNullLiteral>]
        [<Interface>]
        type settings = interface end

module Project =

    module addSourceFileAtPath =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member scriptKind: Ts.ScriptKind option with get, set

            [<ParamObject; Emit("$0")>]
            static member Create(?scriptKind: Ts.ScriptKind) : options = nativeOnly

    module addSourceFileAtPathSync =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member scriptKind: Ts.ScriptKind option with get, set

            [<ParamObject; Emit("$0")>]
            static member Create(?scriptKind: Ts.ScriptKind) : options = nativeOnly

    module addSourceFileAtPathIfExists =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member scriptKind: Ts.ScriptKind option with get, set

            [<ParamObject; Emit("$0")>]
            static member Create(?scriptKind: Ts.ScriptKind) : options = nativeOnly

    module addSourceFileAtPathIfExistsSync =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member scriptKind: Ts.ScriptKind option with get, set

            [<ParamObject; Emit("$0")>]
            static member Create(?scriptKind: Ts.ScriptKind) : options = nativeOnly

    module createSourceFile =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member scriptKind: Ts.ScriptKind option with get, set

            [<ParamObject; Emit("$0")>]
            static member Create(?scriptKind: Ts.ScriptKind) : options = nativeOnly

    module updateSourceFile =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member scriptKind: Ts.ScriptKind option with get, set

            [<ParamObject; Emit("$0")>]
            static member Create(?scriptKind: Ts.ScriptKind) : options = nativeOnly

    module formatDiagnosticsWithColorAndContext =

        [<AllowNullLiteral>]
        [<Interface>]
        type opts =
            abstract member newLineChar:
                Project.formatDiagnosticsWithColorAndContext.opts.newLineChar option with get, set

            [<ParamObject; Emit("$0")>]
            static member Create
                (?newLineChar: Project.formatDiagnosticsWithColorAndContext.opts.newLineChar)
                : opts
                =
                nativeOnly

        module opts =

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type newLineChar =
                | [<CompiledName("\n")>] _NEWLINE_
                | [<CompiledName("\r\n")>] _CARRIAGE_RETURN__NEWLINE_

module Exports =

    module ResolutionHosts__ =

        [<AllowNullLiteral>]
        [<Interface>]
        type Type =
            abstract member deno: ResolutionHostFactory with get, set

            [<ParamObject; Emit("$0")>]
            static member Create(deno: ResolutionHostFactory) : Type = nativeOnly

        [<AutoOpen>]
        module TypeExtensions =

            type Type with
                member inline this.deno
                    (
                        moduleResolutionHost: Ts.ModuleResolutionHost,
                        getCompilerOptions: (unit -> Ts.CompilerOptions)
                    )
                    : ResolutionHost
                    =
                    this.deno.Invoke(moduleResolutionHost, getCompilerOptions)
