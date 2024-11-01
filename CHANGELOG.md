# Changelog

All notable changes to this project will be documented in this file.

This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!-- EasyBuild: START -->
<!-- last_commit_released: dba758860fabb533a408a08ff7f137c88d79db2d -->
<!-- EasyBuild: END -->

## 0.11.0

### 🚀 Features

* Add support for `ThisParameterType` ([dba7588](https://github.com/glutinum-org/cli/commit/dba758860fabb533a408a08ff7f137c88d79db2d))

    ```ts
    declare function toHex(this: number): string;
    declare function numberToString(n: ThisParameterType<typeof toHex>): string;
    ```

    ```fs
    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("toHex", "REPLACE_ME_WITH_MODULE_NAME")>]
        static member toHex (this: float) : string = nativeOnly
        [<Import("numberToString", "REPLACE_ME_WITH_MODULE_NAME")>]
        static member numberToString (n: float) : string = nativeOnly
    ```

* Alias `Error` to `Exception` ([a5078c8](https://github.com/glutinum-org/cli/commit/a5078c8e9060c43848c3074f9745fcc8a7c1ed11))

    ```ts
    export type T = Error
    ```

    ```fs
    type T =
        Exception
    ```

* Use `ReadonlyArray` from `Glutinum.Types` ([205b596](https://github.com/glutinum-org/cli/commit/205b59603b97bd8a9b5b114381988a7d3b3181a9))

    ```ts
    export type T = ReadonlyArray<number>
    ```

    ```fs
    // You need to add Glutinum.Types NuGet package to your project
    open Glutinum.Types

    type T =
        ReadonlyArray<float>
    ```

* Add support for `ReturnType` ([6075659](https://github.com/glutinum-org/cli/commit/6075659dac8d88bf037537fad6c7328ea9218704))

    ```ts
    export type T1 = ReturnType<any>;
    export type T2 = ReturnType<(s: string) => void>;
    export type T3 = ReturnType<<T extends U, U extends number[]>() => T>;
    ```

    ```fs
    type T1 =
        obj

    type T2 =
        unit

    type T3 =
        ResizeArray<float>
    ```

* Transform `FunctionType` to delegate if there are 2 or more parameters ([b7150a7](https://github.com/glutinum-org/cli/commit/b7150a77683760d366aa774161d8508781fedd79))

    ```ts
    export interface MyObject<A,B, NotNeeded> {
        upper: (s : string) => string;
        random: (min: number, max: number) => number;
        foo: (min: A, max: B) => B;
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type MyObject<'A, 'B, 'NotNeeded> =
        abstract member upper: (string -> string) with get, set
        abstract member random: MyObject.random with get, set
        abstract member foo: MyObject.foo<'A, 'B> with get, set

    module MyObject =

        type random =
            delegate of min: float * max: float -> float

        type foo<'A, 'B> =
            delegate of min: 'A * max: 'B -> 'B
    ```

* Add support for `ReadonlyArray` and it's equivalent `readonly T[]` ([a24335b](https://github.com/glutinum-org/cli/commit/a24335bdd1342dafa41586b72b82c65642ab3b97))

    ```ts
    export type Standard = ReadonlyArray<number>

    export type Alias = readonly number[];
    ```

    ```fs
    type ReadonlyArray<'T> = JS.ReadonlyArray<'T>

    type Standard =
        ReadonlyArray<float>

    type Alias =
        ReadonlyArray<float>
    ```

* Support piping CLI output to a file ([14ba216](https://github.com/glutinum-org/cli/commit/14ba2163fa2e6bd4b7d695deed4d84db46f9480e))
* Add replacement for `Function` to `System.Action` ([23ef41f](https://github.com/glutinum-org/cli/commit/23ef41fc7e03bd45e11da60a6ff1bae9a206c535))

    ```ts
    interface sharedEvents {
        getEventState: Function
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type sharedEvents =
        abstract member getEventState: Action with get, set
    ```

* Remove warning when seeing `ConstructorType` + keep in the output TypeAlias that we don't know how to handle ([60a0f83](https://github.com/glutinum-org/cli/commit/60a0f8368e1a8f357e7b3fa214369d543180f9ed))
* Improve reader error message to include the caller file + use the name of the `SyntaxKind` instead of its numeric value ([54913da](https://github.com/glutinum-org/cli/commit/54913da8887cc04e2e025a71edc1dc9baeb69fa3))

### 🐞 Bug Fixes

* Make Reader error reporter support node without a source file attached to them ([190ea17](https://github.com/glutinum-org/cli/commit/190ea170f7f1b3208c25c6dc8098d97728b2bba3))
* `IndexSignature` decorated with `readonly` should not generate a `setter` ([5cb91ef](https://github.com/glutinum-org/cli/commit/5cb91ef86db0a9695c4d7a1e22d394e756e94b1b))

    ```ts
    export interface MyType {
        readonly [n: number]: string;
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type MyType =
        [<EmitIndexer>]
        abstract member Item: n: float -> string with get
    ```

* Improve support for `ConstructorType` ([d4512a9](https://github.com/glutinum-org/cli/commit/d4512a968374eaa57bb00053dab45f912771c4be))

    ```ts
    type Simple = new(config?: string) => number;

    type WitGeneric<T> = new(config?: string) => T;
    ```

    ```fs
    type Simple =
        obj

    [<Erase>]
    type WitGeneric<'T> =
        | WitGeneric of 'T

        member inline this.Value =
            let (WitGeneric output) = this
            output
    ```

* Allows interface to be printed as type with generics ([f63db34](https://github.com/glutinum-org/cli/commit/f63db3433aab8293bb8ea15bbebfaaafb94463a5))
* Open statement from `open Glutinum.Types` to `open Glutinum.Types.TypeScript` ([7325d86](https://github.com/glutinum-org/cli/commit/7325d86ca0b0ecdb0c7e725b003fdb9a576bb7d7))
* Sanitise the `scopeName` when generating additional types ([0f5228b](https://github.com/glutinum-org/cli/commit/0f5228baef81bdf1c3edf73fbcd7379820eeb566))

    *Note how `params` is escaped*

    ```ts
    export interface Test {
        callback: ((params: {
            table: string;
        }) => void)
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type Test =
        abstract member callback: (Test.callback.``params`` -> unit) with get, set

    module Test =

        module callback =

            [<Global>]
            [<AllowNullLiteral>]
            type ``params``
                [<ParamObject; Emit("$0")>]
                (
                    table: string
                ) =

                member val table : string = nativeOnly with get, set
    ```

* Don’t add a space if `XmlDocLine` is empty ([f1be351](https://github.com/glutinum-org/cli/commit/f1be351391a5d907d4b082a6c8be637c73c4b464))
* Supports generics for `ThisType` ([886d8a5](https://github.com/glutinum-org/cli/commit/886d8a530b81a779f640a58b77a50d42cd066990))

    ```ts
    export interface MyObject<T> {
        instance: () => this;
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type MyObject<'T> =
        abstract member instance: (unit -> MyObject<'T>) with get, set
    ```

* Add `option` when printing F# `Function` if the parameter is marked as optional ([8be3190](https://github.com/glutinum-org/cli/commit/8be3190f5cd442c5d502fc2163e4406b0d213718))

    ```ts
    interface AlertStatic {
      alert: (
        title: string,
        message?: string
      ) => void;
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type AlertStatic =
        abstract member alert: (string -> string option -> unit) with get, set
    ```

* Use `@ts-morph/bootstrap` to setup TypeScript for CLI usage ([8521fc6](https://github.com/glutinum-org/cli/commit/8521fc644ceb8f997bc197ef15769156858806e8))
* Remove debug log ([9567a12](https://github.com/glutinum-org/cli/commit/9567a1247ed32e7529fa304b1d886263c25e9ac0))
* Don't emit warning when seeing an `EmptyStatement` ([85bb658](https://github.com/glutinum-org/cli/commit/85bb65875757414dec40f2d167f295e890656f6c))

    Empty statement happens when a `;` is used in a place where it is not needed

    ```ts
    ;
    ```

    or

    ```ts
    interface sharedEvents {
        getEventState: Function
    };
    ```

* Prevent error reporter to crash if the parent node is `undefined` ([bf0cb8f](https://github.com/glutinum-org/cli/commit/bf0cb8f2211f70b272861ee83cc8360e4fdc5a7d))
* Add support for combination `keyof typeof` ([a0babc8](https://github.com/glutinum-org/cli/commit/a0babc89cd345d103a7e94014429aa6dabb4f1d2))

    ```ts
    export enum ColorsEnum {
        white = '#ffffff',
        black = '#000000',
    }

    export type Colors = keyof typeof ColorsEnum;
    ```

    ```fs
    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type ColorsEnum =
        | [<CompiledName("#ffffff")>] white
        | [<CompiledName("#000000")>] black

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type Colors =
        | white
        | black
    ```

* Support for union using `Array` interface ([1a35d4c](https://github.com/glutinum-org/cli/commit/1a35d4c9d8b96d530c513b63c820da92c660174b))

    ```ts
    export type NumberOrNumberArray = number | Array<number>
    ```

    ```fs
    type NumberOrNumberArray =
        U2<float, ResizeArray<float>>
    ```

* Add support for literal enums with `@`, `<`, `>`, ` ` (space) ([883829c](https://github.com/glutinum-org/cli/commit/883829c647a40ec883a6a40586b833c57b0e4bc6))
* Support resolving name when deconstructing a parameter ([04a82dc](https://github.com/glutinum-org/cli/commit/04a82dccc6663e823d4701447bb4b543a0e1a77c))

    ```ts
    export interface LogOptions {
        prefix: string;
    }

    export interface Context {
        indentationLevel: number;
    }

    declare class Signature {
        toText({ indentationLevel }: Context, data : string, {prefix }?: LogOptions): string;
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type LogOptions =
        abstract member prefix: string with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type Context =
        abstract member indentationLevel: float with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type Signature =
        abstract member toText: arg0: Context * data: string * ?arg2: LogOptions -> string
    ```

## 0.10.0

### 🚀 Features

* Merge duplicated `types` and `module` ([acf4571](https://github.com/glutinum-org/cli/commit/acf45717dc0b950e79d5c0f2123f112aa3da5fe3))

    Interfaces exemple:

    ```ts
    export interface PointGroupOptions {
        size: number;
    }

    export interface PointGroupOptions {
        label: string;
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type PointGroupOptions =
        abstract member size: float with get, set
        abstract member label: string with get, set
    ```

    Modules example:

    ```ts
    export module Log {

        export interface Options {
            prefix: string
        }

        export interface Options {
            suffix: string
        }
    }

    export module Log {

        export function log() : void;

        export interface Options {
            level: number
        }
    }
    ```

    ```fs
    module Log =

        [<AllowNullLiteral>]
        [<Interface>]
        type Options =
            abstract member prefix: string with get, set
            abstract member suffix: string with get, set
            abstract member level: float with get, set

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Emit("$0.log($1...)")>]
            abstract member log: unit -> unit
    ```

* Add `Partial<T>` supports + add `typeMemory` access for later reference ([38eb08e](https://github.com/glutinum-org/cli/commit/38eb08ee26f7cf946aceda4aad5c50a1946c3a18))

    ```ts
    export interface PointGroupOptions {
        dotSize: number;
    }

    export interface Options extends Partial<PointGroupOptions> {
        minDistance?: number;
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type PointGroupOptions =
        abstract member dotSize: float with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type Options =
        abstract member minDistance: float option with get, set
        abstract member dotSize: float option with get, set
    ```

### 🐞 Bug Fixes

* Generate constructor when a class is exported as `default` ([8e60ee7](https://github.com/glutinum-org/cli/commit/8e60ee765ccd30ef7dfd3057c07c7a09b18f0606))
* Do not include `this` argument when generating signature of a `FunctionType` ([dd86e28](https://github.com/glutinum-org/cli/commit/dd86e282fda872f98d485c7a5f54ddecf7eacd66))

## 0.9.0

### 🚀 Features

* Improve detection of StandardLibrary types like `Promise`, `Boolean` and add supports for `RegExp` via a type alias ([bd97580](https://github.com/glutinum-org/cli/commit/bd975809a7d884e35ac6f19967148e3bc7418f02))
* Add support for `TemplateLiteralType` ([e75209f](https://github.com/glutinum-org/cli/commit/e75209f538de5056b00e6fa677a1ab7252b01074))
* Add supports for `TypeAliasDeclaration` when used as a returned type ([cd58d28](https://github.com/glutinum-org/cli/commit/cd58d28fb0caab0ccada9d8d2809e1a24ce538a8))
* Add supports for `Record<A,B>` when used as a returned type ([b705f37](https://github.com/glutinum-org/cli/commit/b705f372811f1204209e97f62ad7fd40f9139087))
* Add supports for `IntersectionType` when used as a type signature ([9d12fce](https://github.com/glutinum-org/cli/commit/9d12fcede395414f4630e5ee41f8399d227a28ee))
* During transformation phase accumulate errors and warnings in memory, so they can be reported in the web tool ([7548317](https://github.com/glutinum-org/cli/commit/7548317cd1a7ce40e4990704b2f556f0c13b156e))
* Add support for `LiteralType` (aka literal values used as types) ([2cce022](https://github.com/glutinum-org/cli/commit/2cce022d9327ff098d4ad49d6d1a9839d5d18e0d))
* Add support for `Record<A,B>` ([1a94129](https://github.com/glutinum-org/cli/commit/1a941296384f3a5509c4486caa66cd4d074f1fbf))
* Add support for `ConditionalType` ([6279137](https://github.com/glutinum-org/cli/commit/6279137c315c0dfa3505db53f3400b672eaf958d))
* Add supports for generics constraints ([b1dff0e](https://github.com/glutinum-org/cli/commit/b1dff0e12f105a3dbdb510892702de00407548ff))

### 🐞 Bug Fixes

* Improve `TypeReference` supports when inside of an Union especially when dealing with `TypeParameters` ([cc99bab](https://github.com/glutinum-org/cli/commit/cc99bab60f370c29a4d35e31260e5cbec5630e03))
* If an argument is decorated with `?` and `undefined` or `null` remove the option type ([a413d14](https://github.com/glutinum-org/cli/commit/a413d14af55a3946b02fd886b4a46980afab17dd))
* Use `REPLACE_ME_WITH_MODULE_NAME` everywhere instead of `module` ([1c78943](https://github.com/glutinum-org/cli/commit/1c789431d09cdfcab3568701d75b08bdcead4b83))
* Supports `TypeQuery` against `FunctionType` ([8fd0e35](https://github.com/glutinum-org/cli/commit/8fd0e35188958c8ea003a3a4d1ce07ddc4c59824))
* Don't generate constraints for `Function` as this is not supported by F# ([e3a573d](https://github.com/glutinum-org/cli/commit/e3a573d224088cf1ac82f2552a5fa68d0084d566))
* Sanitize `ModuleDeclaration` name + generates only 1 `exports` property per `ModuleDeclaration` ([8d40f9f](https://github.com/glutinum-org/cli/commit/8d40f9f343cb98e9a7b52530ddb2c012b419ee09))

## 0.8.0

### 🚀 Features

* Add supports for split get/set declaration field ([fa6128f](https://github.com/glutinum-org/cli/commit/fa6128f46761a24864b6a364b92ed38dafdb2148))
* Add support for `HeritageClauses` on classes ([70c6e11](https://github.com/glutinum-org/cli/commit/70c6e116069c6fcaa9a73585d2e95653e89c5752))
* Add support for `HeritageClauses` on interfaces ([046bccd](https://github.com/glutinum-org/cli/commit/046bccd2c791e16b18f0df76c9a820cf6da6219a))
* Improve `Exports` generation to use `abstract` when the exports is not at the top level ([6fb991c](https://github.com/glutinum-org/cli/commit/6fb991c0815cc5882d14749635248bd885083b64))
* Support transforming `Partial<T>` types ([9b01819](https://github.com/glutinum-org/cli/commit/9b018198870a39e1c10b5c20dcfa7679dbb75859))

### 🐞 Bug Fixes

* Don't crash when handling a type which resolves to `Partial<unknown>` ([0b91986](https://github.com/glutinum-org/cli/commit/0b919869a278a0b1aea5068899de1b21a87a8338))

## 0.7.0 - 2024-07-01

### 🚀 Features

* Generate an interface instead of an erased union when handling an union of type literal
* Add support for `@throws` in TSDoc
* Add TSDoc support for TypeAlias
* Add support for TSDoc `typeParam` + add support for TSDoc on constructors, variables, methodSignature

### 🐞 Bug Fixes

* Apply union type optimisation on return type
* Support typeAlias exporting a single typeParameters: `type PluginFunc<T> = T;`
* If a property is decorated with `?` and `undefined` or `null` only wrap it inside a single option
* Handle `symbol` correctly to not emit a warning because of an unsupported kind
* Generate a normal interface if a type literal has an `IndexSignature`

## 0.6.0 - 2024-05-08

### 🚀 Features

* Support IntersectionType with UnionType of TypeLiteral
* Replace `"module"` placeholder with `"REPLACE_ME_WITH_MODULE_NAME"`
* Add support for `NamedTuple`

### 🐞 Bug Fixes

* If no constructor is defined on an exported class generate a default one
* Don't crash when encountering a `TypeQuery` against a module declaration
* String enum containing a dot should be escaped
* Allow resolution of `keyOf` as the return type of interface property
* Don't assume that a node processed by `keyof` is always an interface, instead read it as standard node
* Fix `@glutinum/cli` to include `fable_modules` in the output

## 0.5.0 - 2024-05-08

### Fixed

* Transform `Promise` to `JS.Promise` (by @nojaf) ([GH-33](https://github.com/glutinum-org/cli/pull/33))
* Transform `Uint8Array` to `JS.Uint8Array`
* Optional argument of F# Method are prefixed with `?` instead of suffixing them with `option`
* Sanitize names coming from TypeScript by removing surrounding quotes (`"`, `'`)
* String enums containing a `-` should be escaped with backtick ([GH-44](https://github.com/glutinum-org/cli/issues/44))
* String enums starting with a number should be escaped with backtick ([GH-43](https://github.com/glutinum-org/cli/issues/43))
* Optional interface properties should be transform into `'T option`
* Don't indent module name when printing the F# code
* When leaving `module` scope, indent the printer memory
* Don't crash when flattening a union

    ```ts
    export type LatLngTuple = [number, number, number?];

    export type LatLngExpression = string | LatLngTuple;
    ```

    ```fs
    type LatLngTuple = float * float * float option

    type LatLngExpression =
        U2<string, LatLngTuple>
    ```

* Prevent infite loop when a class has a reference to a union type which reference the class itself

    ```ts
    export declare class MyClass {
        contains(otherBoundsOrLatLng: MyUnion | string): boolean;
    }

    export type MyUnion = MyClass | string;
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type MyClass =
        abstract member contains: otherBoundsOrLatLng: U2<MyUnion, string> -> bool

    type MyUnion =
        U2<MyClass, string>
    ```

* Don't generate `U1` if an union is resolved to a single type

    ```ts
    export type ColorInfo = number | false;
    ```

    ```fs
    // We can't represent false in F#
    type ColorInfo = float
    ```

### Added

* Add support for `MethodSignature` on interface ([GH-28](https://github.com/glutinum-org/cli/issues/28))
* Ignore `ExportAssignment` as we don't know what to do with it yet
* Add support for literal type alias ([GH-45](https://github.com/glutinum-org/cli/issues/45))

    ```ts
    type Mode = "auto";
    type Rank1 = 1;
    type Trusty = true;
    type Falsy = false;
    type PiValue = 3.14;
    ```

* Add support for `ThisType` ([GH-13](https://github.com/glutinum-org/cli/issues/13))
* Add support for `FunctionType` when used as a type

    ```ts
    export interface MyObject {
        instance: () => this;
        log: (a : Boolean, b : number) => this;
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    type MyObject =
        abstract member instance: (unit -> MyObject) with get, set
        abstract member log: (bool -> float -> MyObject) with get, set
    ```

* Add support for `TupleType`
* Add support for `TypeLiteral`

    ```ts
    type Animal = {
        name: string;
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    type Animal =
        abstract member name: string with get, set
    ```

* Add support for `IntersectionType`

    ```ts
    interface ErrorHandling {
        success: boolean;
        error?: string;
    }

    interface ArtworksData {
        artworks: string[];
    }

    type ArtworksResponse = ArtworksData & ErrorHandling;
    ```

    ```fs
    [<AllowNullLiteral>]
    type ErrorHandling =
        abstract member success: bool with get, set
        abstract member error: string option with get, set

    [<AllowNullLiteral>]
    type ArtworksData =
        abstract member artworks: ResizeArray<string> with get, set

    [<AllowNullLiteral>]
    type ArtworksResponse =
        abstract member artworks: ResizeArray<string> with get, set
        abstract member success: bool with get, set
        abstract member error: string option with get, set
    ```

* Add support for argument spread operator ([GH-57](https://github.com/glutinum-org/cli/issues/57))
* Add support for `{ new (...args: any): any}` (`ConstructSignaure`) ([GH-59](https://github.com/glutinum-org/cli/issues/59))
* Add support for `static member` on classes ([GH-60](https://github.com/glutinum-org/cli/issues/60))

    ```ts
    export class Class {
        static methodA(): void;

        static methodB(arg1 : string, arg2: string): void;
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type Class =
        static member inline methodA () =
            emitJsExpr () $$"""
    import { Class } from "module";
    Class.methodA()"""
        static member inline methodB (arg1: string, arg2: string) =
            emitJsExpr (arg1, arg2) $$"""
    import { Class } from "module";
    Class.methodB($0, $1)"""
    ```

* Add support for more primitive `TypeQuery` (`Any`, `String`, `Number`, `Bool`, `Any`, `Unit`)
* Add support for `TypeQuery` on a class declaration
* Add support for `propertyDeclaration` on a class declaration

    ```ts
    declare class Fuse {
        public version: string
    }

    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type Fuse =
        abstract member version: string with get, set
    ```

    Works also for static properties

    ```ts
    declare class Fuse {
        public static version: string
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type Fuse =
        static member inline version
            with get () : string =
                emitJsExpr () $$"""
    import { Fuse } from "module";
    Fuse.version"""
            and set (value: string) =
                emitJsExpr (value) $$"""
    import { Fuse } from "module";
    Fuse.version = $0"""
    ```

* Supports private static property

    ```ts
    export declare class SettingsContainer {
        static #privateField;
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type SettingsContainer =
        static member inline private ``#privateField``
            with get () : unit =
                emitJsExpr () $$"""
    import { SettingsContainer } from "module";
    SettingsContainer.#privateField"""
            and set (value: unit) =
                emitJsExpr (value) $$"""
    import { SettingsContainer } from "module";
    SettingsContainer.#privateField = $0"""
    ```

* Add support for optional type

    ```ts
    export type LatLngTuple = [number, number, number?];
    ```

    ```fs
    type LatLngTuple = float * float * float option
    ```

* Add support for `readonly` TypeOperator

    ```ts
    export type ReadonlyArray<T> = readonly T[];
    ```

    ```fs
    type ReadonlyArray<'T> = ResizeArray<'T>
    ```

* Optimise `IntersectionType` inside of `TypeArguments`

    ```ts
    export type RecordEntryObject = {
        v: string
        n: number
    }

    export type RecordEntryArrayItem = ReadonlyArray<
        RecordEntryObject & { i: number }
    >
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type RecordEntryObject =
        abstract member v: string with get, set
        abstract member n: float with get, set

    type RecordEntryArrayItem =
        ResizeArray<RecordEntryArrayItem.ReadonlyArray.ReturnType>

    module RecordEntryArrayItem =

        module ReadonlyArray =

            [<AllowNullLiteral>]
            [<Interface>]
            type ReturnType =
                abstract member v: string with get, set
                abstract member n: float with get, set
                abstract member i: float with get, set
    ```

* Add support for default export assignment
* Add support for `object` type alias

        ```ts
        export type MyObject = object;
        ```

        ```fs
        type MyObject = obj
        ```
* Add support for converting tsdoc to xml doc comments

### Changed

* Replace `Boolean` with `bool`
* Map `Date` type to `JS.Date` ([GH-48](https://github.com/glutinum-org/cli/issues/48))
* Decorate all interface with `[<Interface>]` attribute this is to ensure they are erased at runtime even if they only have `static member` attached to them
* Private field are not exposed in the F# code, because F# interface doesn't support them

    ```ts
    export declare class SettingsContainer {
        #privateField;
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type SettingsContainer =
        interface end
    ```

* Don't crash on unsupported syntax, instead we log the warning and continue the process. If needed, we default to `obj`

## 0.4.0 - 2024-01-08

### Changed

* Improve unsupported syntax error message to provide more context information ([GH-26](https://github.com/glutinum-org/cli/pull/26))

## 0.3.1 - 2024-01-02

### Fixed

* Respect CLI arguments casing ([GH-23](https://github.com/glutinum-org/cli/issues/23))

## 0.3.0 - 2024-01-01

### Changed

* Rework a bit the logged information

### Fixed

* Support TypeReference with generics

## 0.2.0 - 2023-12-30

### Added

* Basic CLI interface (help, version, options)
* Ability to write the output to a file (use `--out-file <file>`)

### Fixed

* Map `Date` type to `DateTime`
* Makes `typescript` part of the dependencies and not devDependencies

## 0.1.0 - 2023-12-29

### Added

* Initial release
