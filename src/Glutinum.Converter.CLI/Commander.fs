namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

type RegExp = Text.RegularExpressions.Regex

module Commander =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("createCommand", "commander")>]
        static member createCommand(?name: string) : Commander.Command = nativeOnly

        [<Import("createOption", "commander")>]
        static member createOption(flags: string, ?description: string) : Commander.Option =
            nativeOnly

        [<Import("createArgument", "commander")>]
        static member createArgument(name: string, ?description: string) : Commander.Argument =
            nativeOnly

        [<Import("program", "commander")>]
        static member inline program: Commander.Command = nativeOnly

        /// <summary>
        /// Constructs the CommanderError class
        /// </summary>
        /// <param name="exitCode">
        /// suggested exit code which could be used with process.exit
        /// </param>
        /// <param name="code">
        /// an id string representing the error
        /// </param>
        /// <param name="message">
        /// human-readable description of the error
        /// </param>
        [<Import("CommanderError", "commander"); EmitConstructor>]
        static member CommanderError
            (exitCode: float, code: string, message: string)
            : CommanderError
            =
            nativeOnly

        /// <summary>
        /// Constructs the InvalidArgumentError class
        /// </summary>
        /// <param name="message">
        /// explanation of why argument is invalid
        /// </param>
        [<Import("InvalidArgumentError", "commander"); EmitConstructor>]
        static member InvalidArgumentError(message: string) : InvalidArgumentError = nativeOnly

        /// <summary>
        /// Initialize a new command argument with the given name and description.
        /// The default is that the argument is required, and you can explicitly
        /// indicate this with <> around the name. Put [] around the name for an optional argument.
        /// </summary>
        [<Import("Argument", "commander"); EmitConstructor>]
        static member Argument(arg: string, ?description: string) : Argument = nativeOnly

        [<Import("Option", "commander"); EmitConstructor>]
        static member Option(flags: string, ?description: string) : Option = nativeOnly

        [<Import("Help", "commander"); EmitConstructor>]
        static member Help() : Help = nativeOnly

        [<Import("Command", "commander"); EmitConstructor>]
        static member Command(?name: string) : Command = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type LiteralUnion<'LiteralType, 'BaseType> = interface end

    [<AllowNullLiteral>]
    [<AbstractClass>]
    [<Import("CommanderError", "commander")>]
    type CommanderError =
        inherit Exception
        abstract member code: string with get, set
        abstract member exitCode: float with get, set
        abstract member message: string with get, set
        abstract member nestedError: string option with get, set

    [<AllowNullLiteral>]
    [<AbstractClass>]
    [<Import("InvalidArgumentError", "commander")>]
    type InvalidArgumentError =
        inherit Commander.CommanderError

    [<AllowNullLiteral>]
    [<Interface>]
    type ErrorOptions =
        /// <summary>
        /// an id string representing the error
        /// </summary>
        abstract member code: string option with get, set
        /// <summary>
        /// suggested exit code which could be used with process.exit
        /// </summary>
        abstract member exitCode: float option with get, set

        [<ParamObject; Emit("$0")>]
        static member Create(?code: string, ?exitCode: float) : ErrorOptions = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type Argument =
        abstract member description: string with get, set
        abstract member required: bool with get, set
        abstract member variadic: bool with get, set
        abstract member defaultValue: obj option with get, set
        abstract member defaultValueDescription: string option with get, set
        abstract member parseArg: Argument.parseArg<obj> option with get, set
        abstract member argChoices: ResizeArray<string> option with get, set
        /// <summary>
        /// Return argument name.
        /// </summary>
        abstract member name: unit -> string
        /// <summary>
        /// Set the default value, and optionally supply the description to be displayed in the help.
        /// </summary>
        abstract member ``default``: value: obj * ?description: string -> Argument
        /// <summary>
        /// Set the custom handler for processing CLI command arguments into argument values.
        /// </summary>
        abstract member argParser<'T> : fn: Argument.argParser.fn<'T> -> Argument
        /// <summary>
        /// Only allow argument value to be one of choices.
        /// </summary>
        abstract member choices: values: ResizeArray<string> -> Argument
        /// <summary>
        /// Make argument required.
        /// </summary>
        abstract member argRequired: unit -> Argument
        /// <summary>
        /// Make argument optional.
        /// </summary>
        abstract member argOptional: unit -> Argument

    [<AllowNullLiteral>]
    [<Interface>]
    type Option =
        abstract member flags: string with get, set
        abstract member description: string with get, set
        abstract member required: bool with get, set
        abstract member optional: bool with get, set
        abstract member variadic: bool with get, set
        abstract member mandatory: bool with get, set
        abstract member short: string option with get, set
        abstract member long: string option with get, set
        abstract member negate: bool with get, set
        abstract member defaultValue: obj option with get, set
        abstract member defaultValueDescription: string option with get, set
        abstract member presetArg: obj option with get, set
        abstract member envVar: string option with get, set
        abstract member parseArg: Option.parseArg<obj> option with get, set
        abstract member hidden: bool with get, set
        abstract member argChoices: ResizeArray<string> option with get, set
        abstract member helpGroupHeading: string option with get, set
        /// <summary>
        /// Set the default value, and optionally supply the description to be displayed in the help.
        /// </summary>
        abstract member ``default``: value: obj * ?description: string -> Option
        /// <summary>
        /// Preset to use when option used without option-argument, especially optional but also boolean and negated.
        /// The custom processing (parseArg) is called.
        /// </summary>
        /// <example>
        /// <code lang="ts">
        /// new Option('--color').default('GREYSCALE').preset('RGB');
        /// new Option('--donate [amount]').preset('20').argParser(parseFloat);
        /// </code>
        /// </example>
        abstract member preset: arg: obj -> Option
        /// <summary>
        /// Add option name(s) that conflict with this option.
        /// An error will be displayed if conflicting options are found during parsing.
        /// </summary>
        /// <example>
        /// <code lang="ts">
        /// new Option('--rgb').conflicts('cmyk');
        /// new Option('--js').conflicts(['ts', 'jsx']);
        /// </code>
        /// </example>
        abstract member conflicts: names: string -> Option
        /// <summary>
        /// Add option name(s) that conflict with this option.
        /// An error will be displayed if conflicting options are found during parsing.
        /// </summary>
        /// <example>
        /// <code lang="ts">
        /// new Option('--rgb').conflicts('cmyk');
        /// new Option('--js').conflicts(['ts', 'jsx']);
        /// </code>
        /// </example>
        abstract member conflicts: names: ResizeArray<string> -> Option
        /// <summary>
        /// Specify implied option values for when this option is set and the implied options are not.
        ///
        /// The custom processing (parseArg) is not called on the implied values.
        /// </summary>
        /// <example>
        /// program
        ///   .addOption(new Option('--log', 'write logging information to file'))
        ///   .addOption(new Option('--trace', 'log extra details').implies({ log: 'trace.txt' }));
        /// </example>
        abstract member implies: optionValues: Commander.OptionValues -> Option
        /// <summary>
        /// Set environment variable to check for option value.
        ///
        /// An environment variables is only used if when processed the current option value is
        /// undefined, or the source of the current value is 'default' or 'config' or 'env'.
        /// </summary>
        abstract member env: name: string -> Option
        /// <summary>
        /// Set the custom handler for processing CLI option arguments into option values.
        /// </summary>
        abstract member argParser<'T> : fn: Option.argParser.fn<'T> -> Option
        /// <summary>
        /// Whether the option is mandatory and must have a value after parsing.
        /// </summary>
        abstract member makeOptionMandatory: ?mandatory: bool -> Option
        /// <summary>
        /// Hide option in help.
        /// </summary>
        abstract member hideHelp: ?hide: bool -> Option
        /// <summary>
        /// Only allow option value to be one of choices.
        /// </summary>
        abstract member choices: values: ResizeArray<string> -> Option
        /// <summary>
        /// Return option name.
        /// </summary>
        abstract member name: unit -> string
        /// <summary>
        /// Return option name, in a camelcase format that can be used
        /// as an object attribute key.
        /// </summary>
        abstract member attributeName: unit -> string
        /// <summary>
        /// Set the help group heading.
        /// </summary>
        abstract member helpGroup: heading: string -> Option
        /// <summary>
        /// Return whether a boolean option.
        ///
        /// Options are one of boolean, negated, required argument, or optional argument.
        /// </summary>
        abstract member isBoolean: unit -> bool

    [<AllowNullLiteral>]
    [<Interface>]
    type Help =
        /// <summary>
        /// output helpWidth, long lines are wrapped to fit
        /// </summary>
        abstract member helpWidth: float option with get, set
        abstract member minWidthToWrap: float with get, set
        abstract member sortSubcommands: bool with get, set
        abstract member sortOptions: bool with get, set
        abstract member showGlobalOptions: bool with get, set
        abstract member prepareContext: contextOptions: Help.prepareContext.contextOptions -> unit
        /// <summary>
        /// Get the command term to show in the list of subcommands.
        /// </summary>
        abstract member subcommandTerm: cmd: Commander.Command -> string
        /// <summary>
        /// Get the command summary to show in the list of subcommands.
        /// </summary>
        abstract member subcommandDescription: cmd: Commander.Command -> string
        /// <summary>
        /// Get the option term to show in the list of options.
        /// </summary>
        abstract member optionTerm: option: Commander.Option -> string
        /// <summary>
        /// Get the option description to show in the list of options.
        /// </summary>
        abstract member optionDescription: option: Commander.Option -> string
        /// <summary>
        /// Get the argument term to show in the list of arguments.
        /// </summary>
        abstract member argumentTerm: argument: Commander.Argument -> string
        /// <summary>
        /// Get the argument description to show in the list of arguments.
        /// </summary>
        abstract member argumentDescription: argument: Commander.Argument -> string
        /// <summary>
        /// Get the command usage to be displayed at the top of the built-in help.
        /// </summary>
        abstract member commandUsage: cmd: Commander.Command -> string
        /// <summary>
        /// Get the description for the command.
        /// </summary>
        abstract member commandDescription: cmd: Commander.Command -> string
        /// <summary>
        /// Get an array of the visible subcommands. Includes a placeholder for the implicit help command, if there is one.
        /// </summary>
        abstract member visibleCommands: cmd: Commander.Command -> ResizeArray<Commander.Command>
        /// <summary>
        /// Get an array of the visible options. Includes a placeholder for the implicit help option, if there is one.
        /// </summary>
        abstract member visibleOptions: cmd: Commander.Command -> ResizeArray<Commander.Option>

        /// <summary>
        /// Get an array of the visible global options. (Not including help.)
        /// </summary>
        abstract member visibleGlobalOptions:
            cmd: Commander.Command -> ResizeArray<Commander.Option>

        /// <summary>
        /// Get an array of the arguments which have descriptions.
        /// </summary>
        abstract member visibleArguments: cmd: Commander.Command -> ResizeArray<Commander.Argument>

        /// <summary>
        /// Get the longest command term length.
        /// </summary>
        abstract member longestSubcommandTermLength:
            cmd: Commander.Command * helper: Commander.Help -> float

        /// <summary>
        /// Get the longest option term length.
        /// </summary>
        abstract member longestOptionTermLength:
            cmd: Commander.Command * helper: Commander.Help -> float

        /// <summary>
        /// Get the longest global option term length.
        /// </summary>
        abstract member longestGlobalOptionTermLength:
            cmd: Commander.Command * helper: Commander.Help -> float

        /// <summary>
        /// Get the longest argument term length.
        /// </summary>
        abstract member longestArgumentTermLength:
            cmd: Commander.Command * helper: Commander.Help -> float

        /// <summary>
        /// Return display width of string, ignoring ANSI escape sequences. Used in padding and wrapping calculations.
        /// </summary>
        abstract member displayWidth: str: string -> float
        /// <summary>
        /// Style the titles. Called with 'Usage:', 'Options:', etc.
        /// </summary>
        abstract member styleTitle: title: string -> string
        /// <summary>
        /// Usage: <str>
        /// </summary>
        abstract member styleUsage: str: string -> string
        /// <summary>
        /// Style for command name in usage string.
        /// </summary>
        abstract member styleCommandText: str: string -> string
        abstract member styleCommandDescription: str: string -> string
        abstract member styleOptionDescription: str: string -> string
        abstract member styleSubcommandDescription: str: string -> string
        abstract member styleArgumentDescription: str: string -> string
        /// <summary>
        /// Base style used by descriptions.
        /// </summary>
        abstract member styleDescriptionText: str: string -> string
        abstract member styleOptionTerm: str: string -> string
        abstract member styleSubcommandTerm: str: string -> string
        abstract member styleArgumentTerm: str: string -> string
        /// <summary>
        /// Base style used in terms and usage for options.
        /// </summary>
        abstract member styleOptionText: str: string -> string
        /// <summary>
        /// Base style used in terms and usage for subcommands.
        /// </summary>
        abstract member styleSubcommandText: str: string -> string
        /// <summary>
        /// Base style used in terms and usage for arguments.
        /// </summary>
        abstract member styleArgumentText: str: string -> string
        /// <summary>
        /// Calculate the pad width from the maximum term length.
        /// </summary>
        abstract member padWidth: cmd: Commander.Command * helper: Commander.Help -> float
        /// <summary>
        /// Wrap a string at whitespace, preserving existing line breaks.
        /// Wrapping is skipped if the width is less than <c>minWidthToWrap</c>.
        /// </summary>
        abstract member boxWrap: str: string * width: float -> string
        /// <summary>
        /// Detect manually wrapped and indented strings by checking for line break followed by whitespace.
        /// </summary>
        abstract member preformatted: str: string -> bool

        /// <summary>
        /// Format the "item", which consists of a term and description. Pad the term and wrap the description, indenting the following lines.
        ///
        /// So "TTT", 5, "DDD DDDD DD DDD" might be formatted for this.helpWidth=17 like so:
        ///   TTT    DDD DDDD
        ///          DD DDD
        /// </summary>
        abstract member formatItem:
            term: string * termWidth: float * description: string * helper: Commander.Help -> string

        /// <summary>
        /// Format a list of items, given a heading and an array of formatted items.
        /// </summary>
        abstract member formatItemList:
            heading: string * items: ResizeArray<string> * helper: Commander.Help ->
                ResizeArray<string>

        /// <summary>
        /// Group items by their help group heading.
        /// </summary>
        abstract member groupItems<'T> :
            unsortedItems: ResizeArray<'T> *
            visibleItems: ResizeArray<'T> *
            getGroup: ('T -> string) ->
                obj

        /// <summary>
        /// Generate the built-in help text.
        /// </summary>
        abstract member formatHelp: cmd: Commander.Command * helper: Commander.Help -> string

    [<AllowNullLiteral>]
    [<Interface>]
    type HelpConfiguration =
        /// <summary>
        /// output helpWidth, long lines are wrapped to fit
        /// </summary>
        abstract member helpWidth: float option with get, set
        abstract member minWidthToWrap: float option with get, set
        abstract member sortSubcommands: bool option with get, set
        abstract member sortOptions: bool option with get, set
        abstract member showGlobalOptions: bool option with get, set
        abstract member prepareContext: contextOptions: Help.prepareContext.contextOptions -> unit
        /// <summary>
        /// Get the command term to show in the list of subcommands.
        /// </summary>
        abstract member subcommandTerm: cmd: Commander.Command -> string
        /// <summary>
        /// Get the command summary to show in the list of subcommands.
        /// </summary>
        abstract member subcommandDescription: cmd: Commander.Command -> string
        /// <summary>
        /// Get the option term to show in the list of options.
        /// </summary>
        abstract member optionTerm: option: Commander.Option -> string
        /// <summary>
        /// Get the option description to show in the list of options.
        /// </summary>
        abstract member optionDescription: option: Commander.Option -> string
        /// <summary>
        /// Get the argument term to show in the list of arguments.
        /// </summary>
        abstract member argumentTerm: argument: Commander.Argument -> string
        /// <summary>
        /// Get the argument description to show in the list of arguments.
        /// </summary>
        abstract member argumentDescription: argument: Commander.Argument -> string
        /// <summary>
        /// Get the command usage to be displayed at the top of the built-in help.
        /// </summary>
        abstract member commandUsage: cmd: Commander.Command -> string
        /// <summary>
        /// Get the description for the command.
        /// </summary>
        abstract member commandDescription: cmd: Commander.Command -> string
        /// <summary>
        /// Get an array of the visible subcommands. Includes a placeholder for the implicit help command, if there is one.
        /// </summary>
        abstract member visibleCommands: cmd: Commander.Command -> ResizeArray<Commander.Command>
        /// <summary>
        /// Get an array of the visible options. Includes a placeholder for the implicit help option, if there is one.
        /// </summary>
        abstract member visibleOptions: cmd: Commander.Command -> ResizeArray<Commander.Option>

        /// <summary>
        /// Get an array of the visible global options. (Not including help.)
        /// </summary>
        abstract member visibleGlobalOptions:
            cmd: Commander.Command -> ResizeArray<Commander.Option>

        /// <summary>
        /// Get an array of the arguments which have descriptions.
        /// </summary>
        abstract member visibleArguments: cmd: Commander.Command -> ResizeArray<Commander.Argument>

        /// <summary>
        /// Get the longest command term length.
        /// </summary>
        abstract member longestSubcommandTermLength:
            cmd: Commander.Command * helper: Commander.Help -> float

        /// <summary>
        /// Get the longest option term length.
        /// </summary>
        abstract member longestOptionTermLength:
            cmd: Commander.Command * helper: Commander.Help -> float

        /// <summary>
        /// Get the longest global option term length.
        /// </summary>
        abstract member longestGlobalOptionTermLength:
            cmd: Commander.Command * helper: Commander.Help -> float

        /// <summary>
        /// Get the longest argument term length.
        /// </summary>
        abstract member longestArgumentTermLength:
            cmd: Commander.Command * helper: Commander.Help -> float

        /// <summary>
        /// Return display width of string, ignoring ANSI escape sequences. Used in padding and wrapping calculations.
        /// </summary>
        abstract member displayWidth: str: string -> float
        /// <summary>
        /// Style the titles. Called with 'Usage:', 'Options:', etc.
        /// </summary>
        abstract member styleTitle: title: string -> string
        /// <summary>
        /// Usage: <str>
        /// </summary>
        abstract member styleUsage: str: string -> string
        /// <summary>
        /// Style for command name in usage string.
        /// </summary>
        abstract member styleCommandText: str: string -> string
        abstract member styleCommandDescription: str: string -> string
        abstract member styleOptionDescription: str: string -> string
        abstract member styleSubcommandDescription: str: string -> string
        abstract member styleArgumentDescription: str: string -> string
        /// <summary>
        /// Base style used by descriptions.
        /// </summary>
        abstract member styleDescriptionText: str: string -> string
        abstract member styleOptionTerm: str: string -> string
        abstract member styleSubcommandTerm: str: string -> string
        abstract member styleArgumentTerm: str: string -> string
        /// <summary>
        /// Base style used in terms and usage for options.
        /// </summary>
        abstract member styleOptionText: str: string -> string
        /// <summary>
        /// Base style used in terms and usage for subcommands.
        /// </summary>
        abstract member styleSubcommandText: str: string -> string
        /// <summary>
        /// Base style used in terms and usage for arguments.
        /// </summary>
        abstract member styleArgumentText: str: string -> string
        /// <summary>
        /// Calculate the pad width from the maximum term length.
        /// </summary>
        abstract member padWidth: cmd: Commander.Command * helper: Commander.Help -> float
        /// <summary>
        /// Wrap a string at whitespace, preserving existing line breaks.
        /// Wrapping is skipped if the width is less than <c>minWidthToWrap</c>.
        /// </summary>
        abstract member boxWrap: str: string * width: float -> string
        /// <summary>
        /// Detect manually wrapped and indented strings by checking for line break followed by whitespace.
        /// </summary>
        abstract member preformatted: str: string -> bool

        /// <summary>
        /// Format the "item", which consists of a term and description. Pad the term and wrap the description, indenting the following lines.
        ///
        /// So "TTT", 5, "DDD DDDD DD DDD" might be formatted for this.helpWidth=17 like so:
        ///   TTT    DDD DDDD
        ///          DD DDD
        /// </summary>
        abstract member formatItem:
            term: string * termWidth: float * description: string * helper: Commander.Help -> string

        /// <summary>
        /// Format a list of items, given a heading and an array of formatted items.
        /// </summary>
        abstract member formatItemList:
            heading: string * items: ResizeArray<string> * helper: Commander.Help ->
                ResizeArray<string>

        /// <summary>
        /// Group items by their help group heading.
        /// </summary>
        abstract member groupItems<'T> :
            unsortedItems: ResizeArray<'T> *
            visibleItems: ResizeArray<'T> *
            getGroup: ('T -> string) ->
                obj

        /// <summary>
        /// Generate the built-in help text.
        /// </summary>
        abstract member formatHelp: cmd: Commander.Command * helper: Commander.Help -> string

    [<AllowNullLiteral>]
    [<Interface>]
    type ParseOptions =
        abstract member from: ParseOptions.from with get, set

        [<ParamObject; Emit("$0")>]
        static member Create(from: ParseOptions.from) : ParseOptions = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type HelpContext =
        abstract member error: bool with get, set

        [<ParamObject; Emit("$0")>]
        static member Create(error: bool) : HelpContext = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type AddHelpTextContext =
        abstract member error: bool with get, set
        abstract member command: Commander.Command with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type OutputConfiguration =
        abstract member writeOut: str: string -> unit
        abstract member writeErr: str: string -> unit
        abstract member outputError: str: string * write: (string -> unit) -> unit
        abstract member getOutHelpWidth: unit -> float
        abstract member getErrHelpWidth: unit -> float
        abstract member getOutHasColors: unit -> bool
        abstract member getErrHasColors: unit -> bool
        abstract member stripColor: str: string -> string

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type AddHelpTextPosition =
        | beforeAll
        | before
        | after
        | afterAll

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type HookEvent =
        | preSubcommand
        | preAction
        | postAction

    type OptionValueSource = Commander.LiteralUnion<OptionValueSource.Value, string> option

    [<AllowNullLiteral>]
    [<Interface>]
    type OptionValues =
        [<EmitIndexer>]
        abstract member Item: key: string -> obj with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type Command =
        abstract member args: ResizeArray<string> with get, set
        abstract member processedArgs: ResizeArray<obj> with get, set
        abstract member commands: ReadonlyArray<Commander.Command> with get
        abstract member options: ReadonlyArray<Commander.Option> with get
        abstract member registeredArguments: ReadonlyArray<Commander.Argument> with get
        abstract member parent: Commander.Command option with get, set
        /// <summary>
        /// Set the program version to <c>str</c>.
        ///
        /// This method auto-registers the "-V, --version" flag
        /// which will print the version number when passed.
        ///
        /// You can optionally supply the  flags and description to override the defaults.
        /// Get the program version.
        /// </summary>
        abstract member version: str: string * ?flags: string * ?description: string -> Command
        /// <summary>
        /// Set the program version to <c>str</c>.
        ///
        /// This method auto-registers the "-V, --version" flag
        /// which will print the version number when passed.
        ///
        /// You can optionally supply the  flags and description to override the defaults.
        /// Get the program version.
        /// </summary>
        abstract member version: unit -> string option

        /// <summary>
        /// Define a command, implemented using an action handler.
        /// Define a command, implemented in a separate executable file.
        /// </summary>
        /// <remarks>
        /// The command description is supplied using <c>.description</c>, not as a parameter to <c>.command</c>.
        /// </remarks>
        /// <example>
        /// <code lang="ts">
        /// program
        ///   .command('clone <source> [destination]')
        ///   .description('clone a repository into a newly created directory')
        ///   .action((source, destination) => {
        ///     console.log('clone command called');
        ///   });
        /// </code>
        /// </example>
        /// <param name="nameAndArgs">
        /// command name and arguments, args are  <c><required></c> or <c>[optional]</c> and last may also be <c>variadic...</c>
        /// </param>
        /// <param name="opts">
        /// configuration options
        /// </param>
        /// <returns>
        /// new command
        /// </returns>
        abstract member command:
            nameAndArgs: string * ?opts: Commander.CommandOptions -> Commander.Command

        /// <summary>
        /// Define a command, implemented using an action handler.
        /// Define a command, implemented in a separate executable file.
        /// </summary>
        /// <remarks>
        /// The command description is supplied as the second parameter to <c>.command</c>.
        /// </remarks>
        /// <example>
        /// <code lang="ts">
        ///  program
        ///    .command('start <service>', 'start named service')
        ///    .command('stop [service]', 'stop named service, or all if no name supplied');
        /// </code>
        /// </example>
        /// <param name="nameAndArgs">
        /// command name and arguments, args are  <c><required></c> or <c>[optional]</c> and last may also be <c>variadic...</c>
        /// </param>
        /// <param name="description">
        /// description of executable command
        /// </param>
        /// <param name="opts">
        /// configuration options
        /// </param>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member command:
            nameAndArgs: string * description: string * ?opts: Commander.ExecutableCommandOptions ->
                Command

        /// <summary>
        /// Factory routine to create a new unattached command.
        ///
        /// See .command() for creating an attached subcommand, which uses this routine to
        /// create the command. You can override createCommand to customise subcommands.
        /// </summary>
        abstract member createCommand: ?name: string -> Commander.Command

        /// <summary>
        /// Add a prepared subcommand.
        ///
        /// See .command() for creating an attached subcommand which inherits settings from its parent.
        /// </summary>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member addCommand:
            cmd: Commander.Command * ?opts: Commander.CommandOptions -> Command

        /// <summary>
        /// Factory routine to create a new unattached argument.
        ///
        /// See .argument() for creating an attached argument, which uses this routine to
        /// create the argument. You can override createArgument to return a custom argument.
        /// </summary>
        abstract member createArgument: name: string * ?description: string -> Commander.Argument

        /// <summary>
        /// Define argument syntax for command.
        ///
        /// The default is that the argument is required, and you can explicitly
        /// indicate this with <> around the name. Put [] around the name for an optional argument.
        /// </summary>
        /// <example>
        /// <code>
        /// program.argument('<input-file>');
        /// program.argument('[output-file]');
        /// </code>
        /// </example>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member argument<'T> :
            flags: string *
            description: string *
            parseArg: Command.argument.parseArg<'T> *
            ?defaultValue: 'T ->
                Command

        /// <summary>
        /// Define argument syntax for command.
        ///
        /// The default is that the argument is required, and you can explicitly
        /// indicate this with <> around the name. Put [] around the name for an optional argument.
        /// </summary>
        abstract member argument:
            name: string * ?description: string * ?defaultValue: obj -> Command

        /// <summary>
        /// Define argument syntax for command, adding a prepared argument.
        /// </summary>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member addArgument: arg: Commander.Argument -> Command
        /// <summary>
        /// Define argument syntax for command, adding multiple at once (without descriptions).
        ///
        /// See also .argument().
        /// </summary>
        /// <example>
        /// <code>
        /// program.arguments('<cmd> [env]');
        /// </code>
        /// </example>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member arguments: names: string -> Command
        /// <summary>
        /// Customise or override default help command. By default a help command is automatically added if your command has subcommands.
        /// </summary>
        /// <example>
        /// <code lang="ts">
        /// program.helpCommand('help [cmd]');
        /// program.helpCommand('help [cmd]', 'show help');
        /// program.helpCommand(false); // suppress default help command
        /// program.helpCommand(true); // add help command even if no subcommands
        /// </code>
        /// </example>
        abstract member helpCommand: nameAndArgs: string * ?description: string -> Command
        /// <summary>
        /// Customise or override default help command. By default a help command is automatically added if your command has subcommands.
        /// </summary>
        abstract member helpCommand: enable: bool -> Command
        /// <summary>
        /// Add prepared custom help command.
        /// </summary>
        abstract member addHelpCommand: cmd: Commander.Command -> Command

        /// <summary>
        /// Add prepared custom help command.
        /// </summary>
        [<Obsolete("since v12, instead use helpCommand")>]
        abstract member addHelpCommand: nameAndArgs: string * ?description: string -> Command

        /// <summary>
        /// Add prepared custom help command.
        /// </summary>
        [<Obsolete("since v12, instead use helpCommand")>]
        abstract member addHelpCommand: ?enable: bool -> Command

        /// <summary>
        /// Add hook for life cycle event.
        /// </summary>
        abstract member hook:
            event: Commander.HookEvent * listener: Command.hook.listener -> Command

        /// <summary>
        /// Register callback to use as replacement for calling process.exit.
        /// </summary>
        abstract member exitOverride:
            ?callback: (Commander.CommanderError -> U2<obj, unit>) -> Command

        /// <summary>
        /// Display error message and exit (or call exitOverride).
        /// </summary>
        abstract member error: message: string * ?errorOptions: Commander.ErrorOptions -> obj
        /// <summary>
        /// You can customise the help with a subclass of Help by overriding createHelp,
        /// or by overriding Help properties using configureHelp().
        /// </summary>
        abstract member createHelp: unit -> Commander.Help
        /// <summary>
        /// You can customise the help by overriding Help properties using configureHelp(),
        /// or with a subclass of Help by overriding createHelp().
        /// Get configuration
        /// </summary>
        abstract member configureHelp: configuration: Commander.HelpConfiguration -> Command
        /// <summary>
        /// You can customise the help by overriding Help properties using configureHelp(),
        /// or with a subclass of Help by overriding createHelp().
        /// Get configuration
        /// </summary>
        abstract member configureHelp: unit -> Commander.HelpConfiguration
        /// <summary>
        /// The default output goes to stdout and stderr. You can customise this for special
        /// applications. You can also customise the display of errors by overriding outputError.
        ///
        /// The configuration properties are all functions:
        /// <code>
        /// // functions to change where being written, stdout and stderr
        /// writeOut(str)
        /// writeErr(str)
        /// // matching functions to specify width for wrapping help
        /// getOutHelpWidth()
        /// getErrHelpWidth()
        /// // functions based on what is being written out
        /// outputError(str, write) // used for displaying errors, and not used for displaying help
        /// </code>
        /// Get configuration
        /// </summary>
        abstract member configureOutput: configuration: Commander.OutputConfiguration -> Command
        /// <summary>
        /// The default output goes to stdout and stderr. You can customise this for special
        /// applications. You can also customise the display of errors by overriding outputError.
        ///
        /// The configuration properties are all functions:
        /// <code>
        /// // functions to change where being written, stdout and stderr
        /// writeOut(str)
        /// writeErr(str)
        /// // matching functions to specify width for wrapping help
        /// getOutHelpWidth()
        /// getErrHelpWidth()
        /// // functions based on what is being written out
        /// outputError(str, write) // used for displaying errors, and not used for displaying help
        /// </code>
        /// Get configuration
        /// </summary>
        abstract member configureOutput: unit -> Commander.OutputConfiguration
        /// <summary>
        /// Copy settings that are useful to have in common across root command and subcommands.
        ///
        /// (Used internally when adding a command using <c>.command()</c> so subcommands inherit parent settings.)
        /// </summary>
        abstract member copyInheritedSettings: sourceCommand: Commander.Command -> Command
        /// <summary>
        /// Display the help or a custom message after an error occurs.
        /// </summary>
        abstract member showHelpAfterError: unit -> Command
        /// <summary>
        /// Display the help or a custom message after an error occurs.
        /// </summary>
        abstract member showHelpAfterError: displayHelp: bool -> Command
        /// <summary>
        /// Display the help or a custom message after an error occurs.
        /// </summary>
        abstract member showHelpAfterError: displayHelp: string -> Command
        /// <summary>
        /// Display suggestion of similar commands for unknown commands, or options for unknown options.
        /// </summary>
        abstract member showSuggestionAfterError: ?displaySuggestion: bool -> Command
        /// <summary>
        /// Register callback <c>fn</c> for the command.
        /// </summary>
        /// <example>
        /// <code>
        /// program
        ///   .command('serve')
        ///   .description('start service')
        ///   .action(function() {
        ///     // do work here
        ///   });
        /// </code>
        /// </example>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member action: fn: System.Delegate -> Command

        /// <summary>
        /// Define option with <c>flags</c>, <c>description</c>, and optional argument parsing function or <c>defaultValue</c> or both.
        ///
        /// The <c>flags</c> string contains the short and/or long flags, separated by comma, a pipe or space. A required
        /// option-argument is indicated by <c><></c> and an optional option-argument by <c>[]</c>.
        ///
        /// See the README for more details, and see also addOption() and requiredOption().
        /// </summary>
        /// <example>
        /// <code lang="js">
        /// program
        ///     .option('-p, --pepper', 'add pepper')
        ///     .option('--pt, --pizza-type <TYPE>', 'type of pizza') // required option-argument
        ///     .option('-c, --cheese [CHEESE]', 'add extra cheese', 'mozzarella') // optional option-argument with default
        ///     .option('-t, --tip <VALUE>', 'add tip to purchase cost', parseFloat) // custom parse function
        /// </code>
        /// </example>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member option:
            flags: string *
            ?description: string *
            ?defaultValue: U3<string, bool, ResizeArray<string>> ->
                Command

        /// <summary>
        /// Define option with <c>flags</c>, <c>description</c>, and optional argument parsing function or <c>defaultValue</c> or both.
        ///
        /// The <c>flags</c> string contains the short and/or long flags, separated by comma, a pipe or space. A required
        /// option-argument is indicated by <c><></c> and an optional option-argument by <c>[]</c>.
        ///
        /// See the README for more details, and see also addOption() and requiredOption().
        /// </summary>
        abstract member option<'T> :
            flags: string *
            description: string *
            parseArg: Command.option.parseArg<'T> *
            ?defaultValue: 'T ->
                Command

        /// <summary>
        /// Define option with <c>flags</c>, <c>description</c>, and optional argument parsing function or <c>defaultValue</c> or both.
        ///
        /// The <c>flags</c> string contains the short and/or long flags, separated by comma, a pipe or space. A required
        /// option-argument is indicated by <c><></c> and an optional option-argument by <c>[]</c>.
        ///
        /// See the README for more details, and see also addOption() and requiredOption().
        /// </summary>
        [<Obsolete("since v7, instead use choices or a custom function")>]
        abstract member option: flags: string * description: string * regexp: RegExp -> Command

        /// <summary>
        /// Define option with <c>flags</c>, <c>description</c>, and optional argument parsing function or <c>defaultValue</c> or both.
        ///
        /// The <c>flags</c> string contains the short and/or long flags, separated by comma, a pipe or space. A required
        /// option-argument is indicated by <c><></c> and an optional option-argument by <c>[]</c>.
        ///
        /// See the README for more details, and see also addOption() and requiredOption().
        /// </summary>
        [<Obsolete("since v7, instead use choices or a custom function")>]
        abstract member option:
            flags: string * description: string * regexp: RegExp * defaultValue: string -> Command

        /// <summary>
        /// Define option with <c>flags</c>, <c>description</c>, and optional argument parsing function or <c>defaultValue</c> or both.
        ///
        /// The <c>flags</c> string contains the short and/or long flags, separated by comma, a pipe or space. A required
        /// option-argument is indicated by <c><></c> and an optional option-argument by <c>[]</c>.
        ///
        /// See the README for more details, and see also addOption() and requiredOption().
        /// </summary>
        [<Obsolete("since v7, instead use choices or a custom function")>]
        abstract member option:
            flags: string * description: string * regexp: RegExp * defaultValue: bool -> Command

        /// <summary>
        /// Define option with <c>flags</c>, <c>description</c>, and optional argument parsing function or <c>defaultValue</c> or both.
        ///
        /// The <c>flags</c> string contains the short and/or long flags, separated by comma, a pipe or space. A required
        /// option-argument is indicated by <c><></c> and an optional option-argument by <c>[]</c>.
        ///
        /// See the README for more details, and see also addOption() and requiredOption().
        /// </summary>
        [<Obsolete("since v7, instead use choices or a custom function")>]
        abstract member option:
            flags: string * description: string * regexp: RegExp * defaultValue: ResizeArray<string> ->
                Command

        /// <summary>
        /// Define a required option, which must have a value after parsing. This usually means
        /// the option must be specified on the command line. (Otherwise the same as .option().)
        ///
        /// The <c>flags</c> string contains the short and/or long flags, separated by comma, a pipe or space.
        /// </summary>
        abstract member requiredOption:
            flags: string *
            ?description: string *
            ?defaultValue: U3<string, bool, ResizeArray<string>> ->
                Command

        /// <summary>
        /// Define a required option, which must have a value after parsing. This usually means
        /// the option must be specified on the command line. (Otherwise the same as .option().)
        ///
        /// The <c>flags</c> string contains the short and/or long flags, separated by comma, a pipe or space.
        /// </summary>
        abstract member requiredOption<'T> :
            flags: string *
            description: string *
            parseArg: Command.requiredOption.parseArg<'T> *
            ?defaultValue: 'T ->
                Command

        /// <summary>
        /// Define a required option, which must have a value after parsing. This usually means
        /// the option must be specified on the command line. (Otherwise the same as .option().)
        ///
        /// The <c>flags</c> string contains the short and/or long flags, separated by comma, a pipe or space.
        /// </summary>
        [<Obsolete("since v7, instead use choices or a custom function")>]
        abstract member requiredOption:
            flags: string * description: string * regexp: RegExp -> Command

        /// <summary>
        /// Define a required option, which must have a value after parsing. This usually means
        /// the option must be specified on the command line. (Otherwise the same as .option().)
        ///
        /// The <c>flags</c> string contains the short and/or long flags, separated by comma, a pipe or space.
        /// </summary>
        [<Obsolete("since v7, instead use choices or a custom function")>]
        abstract member requiredOption:
            flags: string * description: string * regexp: RegExp * defaultValue: string -> Command

        /// <summary>
        /// Define a required option, which must have a value after parsing. This usually means
        /// the option must be specified on the command line. (Otherwise the same as .option().)
        ///
        /// The <c>flags</c> string contains the short and/or long flags, separated by comma, a pipe or space.
        /// </summary>
        [<Obsolete("since v7, instead use choices or a custom function")>]
        abstract member requiredOption:
            flags: string * description: string * regexp: RegExp * defaultValue: bool -> Command

        /// <summary>
        /// Define a required option, which must have a value after parsing. This usually means
        /// the option must be specified on the command line. (Otherwise the same as .option().)
        ///
        /// The <c>flags</c> string contains the short and/or long flags, separated by comma, a pipe or space.
        /// </summary>
        [<Obsolete("since v7, instead use choices or a custom function")>]
        abstract member requiredOption:
            flags: string * description: string * regexp: RegExp * defaultValue: ResizeArray<string> ->
                Command

        /// <summary>
        /// Factory routine to create a new unattached option.
        ///
        /// See .option() for creating an attached option, which uses this routine to
        /// create the option. You can override createOption to return a custom option.
        /// </summary>
        abstract member createOption: flags: string * ?description: string -> Commander.Option
        /// <summary>
        /// Add a prepared Option.
        ///
        /// See .option() and .requiredOption() for creating and attaching an option in a single call.
        /// </summary>
        abstract member addOption: option: Commander.Option -> Command
        /// <summary>
        /// Whether to store option values as properties on command object,
        /// or store separately (specify false). In both cases the option values can be accessed using .opts().
        /// </summary>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member storeOptionsAsProperties: unit -> obj
        /// <summary>
        /// Whether to store option values as properties on command object,
        /// or store separately (specify false). In both cases the option values can be accessed using .opts().
        /// </summary>
        abstract member storeOptionsAsProperties: ?storeAsProperties: bool -> Command
        /// <summary>
        /// Retrieve option value.
        /// </summary>
        abstract member getOptionValue: key: string -> obj
        /// <summary>
        /// Store option value.
        /// </summary>
        abstract member setOptionValue: key: string * value: obj -> Command

        /// <summary>
        /// Store option value and where the value came from.
        /// </summary>
        abstract member setOptionValueWithSource:
            key: string * value: obj * source: Command.setOptionValueWithSource.source option ->
                Command

        /// <summary>
        /// Store option value and where the value came from.
        /// </summary>
        abstract member setOptionValueWithSource:
            key: string * value: obj * source: obj option -> Command

        /// <summary>
        /// Get source of option value.
        /// </summary>
        abstract member getOptionValueSource: key: string -> Commander.OptionValueSource option

        /// <summary>
        /// Get source of option value. See also .optsWithGlobals().
        /// </summary>
        abstract member getOptionValueSourceWithGlobals:
            key: string -> Commander.OptionValueSource option

        /// <summary>
        /// Alter parsing of short flags with optional values.
        /// </summary>
        /// <example>
        /// <c></c><c>
        /// // for </c>.option('-f,--flag [value]'):
        /// .combineFlagAndOptionalValue(true)  // <c>-f80</c> is treated like <c>--flag=80</c>, this is the default behaviour
        /// .combineFlagAndOptionalValue(false) // <c>-fb</c> is treated like <c>-f -b</c>
        /// <c></c>`
        /// </example>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member combineFlagAndOptionalValue: ?combine: bool -> Command
        /// <summary>
        /// Allow unknown options on the command line.
        /// </summary>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member allowUnknownOption: ?allowUnknown: bool -> Command
        /// <summary>
        /// Allow excess command-arguments on the command line. Pass false to make excess arguments an error.
        /// </summary>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member allowExcessArguments: ?allowExcess: bool -> Command
        /// <summary>
        /// Enable positional options. Positional means global options are specified before subcommands which lets
        /// subcommands reuse the same option names, and also enables subcommands to turn on passThroughOptions.
        ///
        /// The default behaviour is non-positional and global options may appear anywhere on the command line.
        /// </summary>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member enablePositionalOptions: ?positional: bool -> Command
        /// <summary>
        /// Pass through options that come after command-arguments rather than treat them as command-options,
        /// so actual command-options come before command-arguments. Turning this on for a subcommand requires
        /// positional options to have been enabled on the program (parent commands).
        ///
        /// The default behaviour is non-positional and options may appear before or after command-arguments.
        /// </summary>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member passThroughOptions: ?passThrough: bool -> Command

        /// <summary>
        /// Parse <c>argv</c>, setting options and invoking commands when defined.
        ///
        /// Use parseAsync instead of parse if any of your action handlers are async.
        ///
        /// Call with no parameters to parse <c>process.argv</c>. Detects Electron and special node options like <c>node --eval</c>. Easy mode!
        ///
        /// Or call with an array of strings to parse, and optionally where the user arguments start by specifying where the arguments are <c>from</c>:
        /// - <c>'node'</c>: default, <c>argv[0]</c> is the application and <c>argv[1]</c> is the script being run, with user arguments after that
        /// - <c>'electron'</c>: <c>argv[0]</c> is the application and <c>argv[1]</c> varies depending on whether the electron application is packaged
        /// - <c>'user'</c>: just user arguments
        /// </summary>
        /// <example>
        /// <code>
        /// program.parse(); // parse process.argv and auto-detect electron and special node flags
        /// program.parse(process.argv); // assume argv[0] is app and argv[1] is script
        /// program.parse(my-args, { from: 'user' }); // just user supplied arguments, nothing special about argv[0]
        /// </code>
        /// </example>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member parse:
            ?argv: ResizeArray<string> * ?parseOptions: Commander.ParseOptions -> Command

        /// <summary>
        /// Parse <c>argv</c>, setting options and invoking commands when defined.
        ///
        /// Call with no parameters to parse <c>process.argv</c>. Detects Electron and special node options like <c>node --eval</c>. Easy mode!
        ///
        /// Or call with an array of strings to parse, and optionally where the user arguments start by specifying where the arguments are <c>from</c>:
        /// - <c>'node'</c>: default, <c>argv[0]</c> is the application and <c>argv[1]</c> is the script being run, with user arguments after that
        /// - <c>'electron'</c>: <c>argv[0]</c> is the application and <c>argv[1]</c> varies depending on whether the electron application is packaged
        /// - <c>'user'</c>: just user arguments
        /// </summary>
        /// <example>
        /// <code>
        /// await program.parseAsync(); // parse process.argv and auto-detect electron and special node flags
        /// await program.parseAsync(process.argv); // assume argv[0] is app and argv[1] is script
        /// await program.parseAsync(my-args, { from: 'user' }); // just user supplied arguments, nothing special about argv[0]
        /// </code>
        /// </example>
        /// <returns>
        /// Promise
        /// </returns>
        abstract member parseAsync:
            ?argv: ResizeArray<string> * ?parseOptions: Commander.ParseOptions ->
                JS.Promise<Command>

        /// <summary>
        /// Called the first time parse is called to save state and allow a restore before subsequent calls to parse.
        /// Not usually called directly, but available for subclasses to save their custom state.
        ///
        /// This is called in a lazy way. Only commands used in parsing chain will have state saved.
        /// </summary>
        abstract member saveStateBeforeParse: unit -> unit
        /// <summary>
        /// Restore state before parse for calls after the first.
        /// Not usually called directly, but available for subclasses to save their custom state.
        ///
        /// This is called in a lazy way. Only commands used in parsing chain will have state restored.
        /// </summary>
        abstract member restoreStateBeforeParse: unit -> unit
        /// <summary>
        /// Parse options from <c>argv</c> removing known options,
        /// and return argv split into operands and unknown arguments.
        ///
        /// Side effects: modifies command by storing options. Does not reset state if called again.
        ///
        ///     argv => operands, unknown
        ///     --known kkk op => [op], []
        ///     op --known kkk => [op], []
        ///     sub --unknown uuu op => [sub], [--unknown uuu op]
        ///     sub -- --unknown uuu op => [sub --unknown uuu op], []
        /// </summary>
        abstract member parseOptions: argv: ResizeArray<string> -> Commander.ParseOptionsResult
        /// <summary>
        /// Return an object containing local option values as key-value pairs
        /// </summary>
        abstract member opts<'T> : unit -> 'T
        /// <summary>
        /// Return an object containing merged local and global option values as key-value pairs.
        /// </summary>
        abstract member optsWithGlobals<'T> : unit -> 'T
        /// <summary>
        /// Set the description.
        /// Get the description.
        /// </summary>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member description: str: string -> Command

        /// <summary>
        /// Set the description.
        /// Get the description.
        /// </summary>
        [<Obsolete("since v8, instead use .argument to add command argument with description")>]
        abstract member description:
            str: string * argsDescription: Command.description.argsDescription -> Command

        /// <summary>
        /// Set the description.
        /// Get the description.
        /// </summary>
        abstract member description: unit -> string
        /// <summary>
        /// Set the summary. Used when listed as subcommand of parent.
        /// Get the summary.
        /// </summary>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member summary: str: string -> Command
        /// <summary>
        /// Set the summary. Used when listed as subcommand of parent.
        /// Get the summary.
        /// </summary>
        abstract member summary: unit -> string
        /// <summary>
        /// Set an alias for the command.
        ///
        /// You may call more than once to add multiple aliases. Only the first alias is shown in the auto-generated help.
        /// Get alias for the command.
        /// </summary>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member alias: alias: string -> Command
        /// <summary>
        /// Set an alias for the command.
        ///
        /// You may call more than once to add multiple aliases. Only the first alias is shown in the auto-generated help.
        /// Get alias for the command.
        /// </summary>
        abstract member alias: unit -> string
        /// <summary>
        /// Set aliases for the command.
        ///
        /// Only the first alias is shown in the auto-generated help.
        /// Get aliases for the command.
        /// </summary>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member aliases: aliases: ResizeArray<string> -> Command
        /// <summary>
        /// Set aliases for the command.
        ///
        /// Only the first alias is shown in the auto-generated help.
        /// Get aliases for the command.
        /// </summary>
        abstract member aliases: unit -> ResizeArray<string>
        /// <summary>
        /// Set the command usage.
        /// Get the command usage.
        /// </summary>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member usage: str: string -> Command
        /// <summary>
        /// Set the command usage.
        /// Get the command usage.
        /// </summary>
        abstract member usage: unit -> string
        /// <summary>
        /// Set the name of the command.
        /// Get the name of the command.
        /// </summary>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member name: str: string -> Command
        /// <summary>
        /// Set the name of the command.
        /// Get the name of the command.
        /// </summary>
        abstract member name: unit -> string
        /// <summary>
        /// Set the name of the command from script filename, such as process.argv[1],
        /// or import.meta.filename.
        ///
        /// (Used internally and public although not documented in README.)
        /// </summary>
        /// <example>
        /// <code lang="ts">
        /// program.nameFromFilename(import.meta.filename);
        /// </code>
        /// </example>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member nameFromFilename: filename: string -> Command
        /// <summary>
        /// Set the directory for searching for executable subcommands of this command.
        /// Get the executable search directory.
        /// </summary>
        /// <example>
        /// <code lang="ts">
        /// program.executableDir(import.meta.dirname);
        /// // or
        /// program.executableDir('subcommands');
        /// </code>
        /// </example>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member executableDir: path: string -> Command
        /// <summary>
        /// Set the directory for searching for executable subcommands of this command.
        /// Get the executable search directory.
        /// </summary>
        abstract member executableDir: unit -> string option
        /// <summary>
        /// Set the help group heading for this subcommand in parent command's help.
        /// Get the help group heading for this subcommand in parent command's help.
        /// </summary>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member helpGroup: heading: string -> Command
        /// <summary>
        /// Set the help group heading for this subcommand in parent command's help.
        /// Get the help group heading for this subcommand in parent command's help.
        /// </summary>
        abstract member helpGroup: unit -> string
        /// <summary>
        /// Set the default help group heading for subcommands added to this command.
        /// (This does not override a group set directly on the subcommand using .helpGroup().)
        /// Get the default help group heading for subcommands added to this command.
        /// </summary>
        /// <example>
        /// program.commandsGroup('Development Commands:);
        /// program.command('watch')...
        /// program.command('lint')...
        /// ...
        /// </example>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member commandsGroup: heading: string -> Command
        /// <summary>
        /// Set the default help group heading for subcommands added to this command.
        /// (This does not override a group set directly on the subcommand using .helpGroup().)
        /// Get the default help group heading for subcommands added to this command.
        /// </summary>
        abstract member commandsGroup: unit -> string
        /// <summary>
        /// Set the default help group heading for options added to this command.
        /// (This does not override a group set directly on the option using .helpGroup().)
        /// Get the default help group heading for options added to this command.
        /// </summary>
        /// <example>
        /// program
        ///   .optionsGroup('Development Options:')
        ///   .option('-d, --debug', 'output extra debugging')
        ///   .option('-p, --profile', 'output profiling information')
        /// </example>
        /// <returns>
        /// <c>this</c> command for chaining
        /// </returns>
        abstract member optionsGroup: heading: string -> Command
        /// <summary>
        /// Set the default help group heading for options added to this command.
        /// (This does not override a group set directly on the option using .helpGroup().)
        /// Get the default help group heading for options added to this command.
        /// </summary>
        abstract member optionsGroup: unit -> string
        /// <summary>
        /// Output help information for this command.
        ///
        /// Outputs built-in help, and custom text added using <c>.addHelpText()</c>.
        /// </summary>
        abstract member outputHelp: ?context: Commander.HelpContext -> unit

        /// <summary>
        /// Output help information for this command.
        ///
        /// Outputs built-in help, and custom text added using <c>.addHelpText()</c>.
        /// </summary>
        [<Obsolete("since v7")>]
        abstract member outputHelp: cb: (string -> string) -> unit

        /// <summary>
        /// Return command help documentation.
        /// </summary>
        abstract member helpInformation: ?context: Commander.HelpContext -> string
        /// <summary>
        /// You can pass in flags and a description to override the help
        /// flags and help description for your command. Pass in false
        /// to disable the built-in help option.
        /// </summary>
        abstract member helpOption: unit -> Command
        /// <summary>
        /// You can pass in flags and a description to override the help
        /// flags and help description for your command. Pass in false
        /// to disable the built-in help option.
        /// </summary>
        abstract member helpOption: flags: string * ?description: string -> Command
        /// <summary>
        /// You can pass in flags and a description to override the help
        /// flags and help description for your command. Pass in false
        /// to disable the built-in help option.
        /// </summary>
        abstract member helpOption: flags: bool * ?description: string -> Command
        /// <summary>
        /// Supply your own option to use for the built-in help option.
        /// This is an alternative to using helpOption() to customise the flags and description etc.
        /// </summary>
        abstract member addHelpOption: option: Commander.Option -> Command
        /// <summary>
        /// Output help information and exit.
        ///
        /// Outputs built-in help, and custom text added using <c>.addHelpText()</c>.
        /// </summary>
        abstract member help: ?context: Commander.HelpContext -> obj

        /// <summary>
        /// Output help information and exit.
        ///
        /// Outputs built-in help, and custom text added using <c>.addHelpText()</c>.
        /// </summary>
        [<Obsolete("since v7")>]
        abstract member help: cb: (string -> string) -> obj

        /// <summary>
        /// Add additional text to be displayed with the built-in help.
        ///
        /// Position is 'before' or 'after' to affect just this command,
        /// and 'beforeAll' or 'afterAll' to affect this command and all its subcommands.
        /// </summary>
        abstract member addHelpText:
            position: Commander.AddHelpTextPosition * text: string -> Command

        /// <summary>
        /// Add additional text to be displayed with the built-in help.
        ///
        /// Position is 'before' or 'after' to affect just this command,
        /// and 'beforeAll' or 'afterAll' to affect this command and all its subcommands.
        /// </summary>
        abstract member addHelpText:
            position: Commander.AddHelpTextPosition * text: (Commander.AddHelpTextContext -> string) ->
                Command

        /// <summary>
        /// Add a listener (callback) for when events occur. (Implemented using EventEmitter.)
        /// </summary>
        abstract member on<'A> : event: string * listener: ('A -> unit) -> Command
        /// <summary>
        /// Add a listener (callback) for when events occur. (Implemented using EventEmitter.)
        /// </summary>
        abstract member on<'A, 'B> : event: string * listener: ('A -> 'B -> unit) -> Command

        /// <summary>
        /// Add a listener (callback) for when events occur. (Implemented using EventEmitter.)
        /// </summary>
        abstract member on<'A, 'B, 'C> :
            event: string * listener: ('A -> 'B -> 'C -> unit) -> Command

        /// <summary>
        /// Add a listener (callback) for when events occur. (Implemented using EventEmitter.)
        /// </summary>
        abstract member on: event: string * listener: System.Delegate -> Command
        /// <summary>
        /// Add a listener (callback) for when events occur. (Implemented using EventEmitter.)
        /// </summary>
        abstract member on<'A> : event: obj * listener: ('A -> unit) -> Command
        /// <summary>
        /// Add a listener (callback) for when events occur. (Implemented using EventEmitter.)
        /// </summary>
        abstract member on<'A, 'B> : event: obj * listener: ('A -> 'B -> unit) -> Command
        /// <summary>
        /// Add a listener (callback) for when events occur. (Implemented using EventEmitter.)
        /// </summary>
        abstract member on<'A, 'B, 'C> : event: obj * listener: ('A -> 'B -> 'C -> unit) -> Command
        /// <summary>
        /// Add a listener (callback) for when events occur. (Implemented using EventEmitter.)
        /// </summary>
        abstract member on: event: obj * listener: System.Delegate -> Command

    [<AllowNullLiteral>]
    [<Interface>]
    type CommandOptions =
        abstract member hidden: bool option with get, set
        abstract member isDefault: bool option with get, set

        [<Obsolete("since v7, replaced by hidden")>]
        abstract member noHelp: bool option with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type ExecutableCommandOptions =
        inherit Commander.CommandOptions
        abstract member executableFile: string option with get, set

        [<ParamObject; Emit("$0")>]
        static member Create
            (?hidden: bool, ?isDefault: bool, ?noHelp: bool, ?executableFile: string)
            : ExecutableCommandOptions
            =
            nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type ParseOptionsResult =
        abstract member operands: ResizeArray<string> with get, set
        abstract member unknown: ResizeArray<string> with get, set

    type LiteralUnion<'LiteralType> = LiteralUnion<'LiteralType, U2<string, float>>

    module Argument =

        type parseArg<'T> = delegate of value: string * previous: 'T -> 'T

        module argParser =

            type fn<'T> = delegate of value: string * previous: 'T -> 'T

    module Option =

        type parseArg<'T> = delegate of value: string * previous: 'T -> 'T

        module argParser =

            type fn<'T> = delegate of value: string * previous: 'T -> 'T

    module Help =

        module prepareContext =

            [<AllowNullLiteral>]
            [<Interface>]
            type contextOptions =
                abstract member error: bool option with get, set
                abstract member helpWidth: float option with get, set
                abstract member outputHasColors: bool option with get, set

                [<ParamObject; Emit("$0")>]
                static member Create
                    (?error: bool, ?helpWidth: float, ?outputHasColors: bool)
                    : contextOptions
                    =
                    nativeOnly

    module ParseOptions =

        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.None)>]
        type from =
            | node
            | electron
            | user

    module OptionValueSource =

        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.None)>]
        type Value =
            | ``default``
            | config
            | env
            | cli
            | implied

    module Command =

        module argument =

            type parseArg<'T> = delegate of value: string * previous: 'T -> 'T

        module hook =

            type listener =
                delegate of
                    thisCommand: Commander.Command * actionCommand: Commander.Command ->
                        U2<unit, JS.Promise<unit>>

        module option =

            type parseArg<'T> = delegate of value: string * previous: 'T -> 'T

        module requiredOption =

            type parseArg<'T> = delegate of value: string * previous: 'T -> 'T

        module setOptionValueWithSource =

            [<RequireQualifiedAccess>]
            [<StringEnum(CaseRules.None)>]
            type source =
                | ``default``
                | config
                | env
                | cli
                | implied

        module description =

            [<AllowNullLiteral>]
            [<Interface>]
            type argsDescription =
                [<EmitIndexer>]
                abstract member Item: key: string -> string with get, set
