# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased

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
