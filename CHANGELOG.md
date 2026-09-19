---
last_commit_released: 865f54ebdd8defd9b7f8e3f26e42bf563043372d
name: Glutinum.Converter.CLI
exclude:
  - src/Glutinum.Types/
  - bindings/
updaters:
  - package.json:
      file: package.json
  - regex:
      file: src/Glutinum.Converter/Prelude.fs
      pattern: (?<=let VERSION = ").*(?=")
---

# Changelog

All notable changes to this project will be documented in this file.

This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 0.14.0 - 2026-09-19

### 🚀 Features

* Forward interface documentation to F# code ([e4c9598](https://github.com/glutinum-org/cli/commit/e4c9598c135a6cf12218ff95894326e657ffc78e))
* Unwrap methods with a string-literal first argument ([9afcfab](https://github.com/glutinum-org/cli/commit/9afcfabf5971c7611a3583fbe1eb67180c208e38))
* Forward namespace documentation to F# code ([d533e05](https://github.com/glutinum-org/cli/commit/d533e05d891a996dcd0b77997c11e7253658b448))
* Generate a concrete type when applying a generic type alias ([d4bcb96](https://github.com/glutinum-org/cli/commit/d4bcb96be9e2081149279ff39f7d5bd9a97e1463))
* Warn when the code is too long to report an issue ([3f1052a](https://github.com/glutinum-org/cli/commit/3f1052a23065fd0a4434056c50a6c27f048963a0))
* Generate a TypeScriptTaggedUnion for unions of tagged type literals ([4d0909d](https://github.com/glutinum-org/cli/commit/4d0909d4adbfa8eed8e8baa9edd1666e6e76b961))
* Forward documentation to generated classes ([8c33918](https://github.com/glutinum-org/cli/commit/8c339189c06ba8f33f7ba4a78192579a3773ffa3))
* Generate a `[<ParamObject>]` class for interfaces used as arguments ([2531649](https://github.com/glutinum-org/cli/commit/2531649418f64abb266d3797befeb30d5b444408))
* Generate constructor overloads for union properties of `[<ParamObject>]` classes ([f63f0f6](https://github.com/glutinum-org/cli/commit/f63f0f65a30127d8895c26bae649d530d1aa7eda))
* Generate bindings for a package and its dependencies ([39d98fd](https://github.com/glutinum-org/cli/commit/39d98fd50a34879302b198fbd755b724f817337a))
* Generate the target package as a module named after it ([3126f30](https://github.com/glutinum-org/cli/commit/3126f30563084f1a80713692018868f1fd87510e))
* Generate several packages in one file ([bb7ad21](https://github.com/glutinum-org/cli/commit/bb7ad21c39323723bc86e5b3a5889e133e5d6d04))
* A file made of an ambient module is that module ([84c248d](https://github.com/glutinum-org/cli/commit/84c248d5926d3e947ceb8fff73da08f3bc741642))
* Gather the `Exports` of the files of a package in one module ([1267114](https://github.com/glutinum-org/cli/commit/1267114da8996a3aa36a99aa4c0fb1529d83844d))
* Download the declaration files of a package from jsDelivr into a file system ([6787d5d](https://github.com/glutinum-org/cli/commit/6787d5d98154db4b6adb1c5cde7b51d5383a5e51))
* Generate the bindings of an npm package from the web app ([2a71612](https://github.com/glutinum-org/cli/commit/2a716122fabef7d9a45cf2c19f0d6dad3bfde5f4))
* Generate the DOM API from `@types/web` ([d4ee538](https://github.com/glutinum-org/cli/commit/d4ee53853f96b4dc83c4185715a0fb0bc615abb6))
* Reference the DOM and Node types from the Glutinum.Web and Glutinum.Node bindings ([4153298](https://github.com/glutinum-org/cli/commit/41532980f97d7aea0bb831d7ba202400bc898563))
* Delegates for callable types, iterable interfaces and defaulted type parameters ([6775a84](https://github.com/glutinum-org/cli/commit/6775a84d1a31c8ecf4e3aebb110725c8d31cb9df))
* Focus the package input when the mode is selected ([00827c1](https://github.com/glutinum-org/cli/commit/00827c114245adf983a6ee429d2f109c6a12ce04))
* One overload per case of a union parameter ([01dcad8](https://github.com/glutinum-org/cli/commit/01dcad887b2029e1faed0381c8696bf2aebd0498))
* Packages are modules of the `Glutinum` namespace ([b11533b](https://github.com/glutinum-org/cli/commit/b11533b452091b4f312cf44482a1b6756d1fdae7))
* Publish Glutinum.Web and Glutinum.Node from the repository ([e90f3dd](https://github.com/glutinum-org/cli/commit/e90f3ddb54c929c213d6af2c50ee2ecd9a221e92))
* Typed keys for `keyof` maps ([9f6e4b8](https://github.com/glutinum-org/cli/commit/9f6e4b8e41e230874240fd39ae7ed9f4be2488e4))
* Package globals at the package level and typed keys for functions ([b85805d](https://github.com/glutinum-org/cli/commit/b85805dfc7b166c7c0160e2afb06ab67872d5021))
* Typed listeners of the Node event emitter ([ea428f4](https://github.com/glutinum-org/cli/commit/ea428f41632137188d4ca7596b82667f1f1918f1))
* Conditional types resolved by the checker, the constraints and the known keys ([b135bd5](https://github.com/glutinum-org/cli/commit/b135bd50e549e00f7043cda368235528554c9039))
* Overloads through a generic union alias and a type parameter case ([109a3d1](https://github.com/glutinum-org/cli/commit/109a3d15e9d4eeee8f5f7d10dc6d7e318d68894a))
* A package subpath is an input ([32b5bc2](https://github.com/glutinum-org/cli/commit/32b5bc29584e03f85c0bb7a2986ca0cb63676a5d))
* --external references a package from its own binding ([7177391](https://github.com/glutinum-org/cli/commit/7177391d7d38c33f93fa238a489ad9577c553462))
* A property typed by a callable is a method ([b9d2984](https://github.com/glutinum-org/cli/commit/b9d29842ebf5ddd9937c5604020abd36ba414c19))
* Plain arguments where a union alias, a default or an option got in the way ([2517555](https://github.com/glutinum-org/cli/commit/2517555e42660c00c5ace5e5ddd47745d356b4ca))
* Arrays for readonly parameters, calls and values through the default import ([d359e77](https://github.com/glutinum-org/cli/commit/d359e776cf05a2dfa420467012bfc2acca1bb080))
* Infer constraints in conditional types and non-generic overloads of defaulted functions ([af408c6](https://github.com/glutinum-org/cli/commit/af408c6e79273e7e40aa0d5728a4f6bc678b3f1e))
* Mixed enums, variadic tuples, `this` parameters and assertion signatures ([0602097](https://github.com/glutinum-org/cli/commit/06020971729daac74c7f2fe284d90010cae5992d))
* The web app is a page of the documentation site ([2c577dc](https://github.com/glutinum-org/cli/commit/2c577dc5e7e19f9b2ad7620fda150eafd19e0c9e))
* `Glutinum.Types` is generated from the ES library files ([3197153](https://github.com/glutinum-org/cli/commit/31971538c26476615d1c90f9ae6fff57901f37da))
* `Date` and its constructor come from `Glutinum.Types` ([a55bb5f](https://github.com/glutinum-org/cli/commit/a55bb5f05cf21aa379eeb92fa7ba9f5343d23dbb))
* The type parameters of a method are declared ([4c79e16](https://github.com/glutinum-org/cli/commit/4c79e1677afea19071d2fd054195f4d4744dd1d7))
* The static side of `Number`, `String`, `Object`, `Symbol` and `Reflect` in `Glutinum.Types` ([120c2c0](https://github.com/glutinum-org/cli/commit/120c2c001008ff649846251f5c64c528aeeab903))

### 🐞 Bug Fixes

* Keep boolean literals in unions instead of dropping them ([0e1a3fb](https://github.com/glutinum-org/cli/commit/0e1a3fb3daab0286a5dd07e2bf1b7751c79d89ba))

    A union mixing a boolean literal with string literals is now represented
    as a `StringEnum` using `[<CompiledValue(...)>]` for the boolean case,
    and a union mixing a boolean literal with another primitive is
    represented
    as an erased union (`U2`).

    ```ts
    export type DevTool = false | 'eval'
    ```

    ```fs
    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type DevTool =
        | [<CompiledValue(false)>] False
        | eval
    ```
* Resolve template literal types to a StringEnum when possible ([3ba3335](https://github.com/glutinum-org/cli/commit/3ba33350fbb5c1be19177c5f8f514dbd64a374e5))
* Read numeric literal values from the TypeChecker ([22b2a53](https://github.com/glutinum-org/cli/commit/22b2a53c9c6dae3e117c158f53eaa9100294b5dc))
* Replace `$` and `/` in type and union case names ([482388c](https://github.com/glutinum-org/cli/commit/482388ca51a36bc9329702850f982988b7f06269))
* Don't merge a union of type literals into a single interface ([0c7d9ba](https://github.com/glutinum-org/cli/commit/0c7d9ba51b0acb63b8163fcd3b2e4ffc9614579e))
* Resolve enum member values via the TypeChecker ([8ecdd5f](https://github.com/glutinum-org/cli/commit/8ecdd5f58f2d0345bb23b9e88de04adb2a3b9366))
* Remove unecessary warning on the reader side ([c036674](https://github.com/glutinum-org/cli/commit/c0366740275ff9cec674b39a6be7d958b127c486))
* Resolve `ReturnType<this[...]>` ([5ccc8dd](https://github.com/glutinum-org/cli/commit/5ccc8ddbd75921de2aece807b3798653ef026e72))
* Resolve `Omit<_>` in interface heritage clause ([73c771f](https://github.com/glutinum-org/cli/commit/73c771f71b0b271c9f96281aa906b18eeea765ed))
* Generate an interface for callable type literals ([4ee01d2](https://github.com/glutinum-org/cli/commit/4ee01d2be21466424ee066afbedb1cfcb85f893b))
* Use TypeScript bundled with @ts-morph/bootstrap ([b9a890b](https://github.com/glutinum-org/cli/commit/b9a890b1d7bb9dec187111ee70bf0167283e525f))
* Generate an empty interface for empty enums ([35b20ff](https://github.com/glutinum-org/cli/commit/35b20ffa208d911d472a375e39108135c0b0a4c9))
* Remove stray quote after `<see>` in documentation links ([144ab05](https://github.com/glutinum-org/cli/commit/144ab05616443635836cb6abeb3df77ef4fbfdef))
* Support `@link` block tag in documentation ([874c1ff](https://github.com/glutinum-org/cli/commit/874c1ff848472199fa96ba01876551efe7116091))
* Support parenthesized and unsupported types in `keyof` ([7191505](https://github.com/glutinum-org/cli/commit/7191505600dfccc7a8566701b65b71ee1351e17c))
* Generate an interface for type aliases with unused type parameters ([9beaeed](https://github.com/glutinum-org/cli/commit/9beaeedddd8fb7b30ee61e83136702b5a787823b))
* Don't crash when `Exclude<_>` doesn't resolve to a union ([06db4bd](https://github.com/glutinum-org/cli/commit/06db4bd66bf1af72357758281d6fcafd26a28adc))
* Rename delegates generated for overloads with a function type argument ([754e323](https://github.com/glutinum-org/cli/commit/754e3239933e751fd2d33c6f9049335c8543715b))
* Rename intersection and `Omit<_>` types generated for overloads ([a8198e9](https://github.com/glutinum-org/cli/commit/a8198e970f99c06e9b55873345db9f72c245c685))
* Prevent infinite recursion on recursive `Partial<_>` ([7fd1d7c](https://github.com/glutinum-org/cli/commit/7fd1d7c062ec1bdfa0afae44b5e8eab48ddccb9d))
* Rename `Partial<_>` types generated for overloads ([caf5272](https://github.com/glutinum-org/cli/commit/caf527290002bb7b12eb195471e155bbd73528ac))
* Keep non-literal types in unions with string literals ([eba770b](https://github.com/glutinum-org/cli/commit/eba770b855f47cc14a4cfb5984a89e1e34aa357b))
* Keep non-literal types in unions with numeric literals ([827c843](https://github.com/glutinum-org/cli/commit/827c843a30ea04c1f65ae511d0816d5e6c7c9100))
* Support unions mixing string and numeric literals ([b4409e9](https://github.com/glutinum-org/cli/commit/b4409e988289c1cf9d1289a2153ab0a3f9b35606))
* Read `{@link}` at the start of `@returns` and `@throws` ([d7046f5](https://github.com/glutinum-org/cli/commit/d7046f595207cfff3decf49bc6a41d6444e66704))
* Resolve type alias applications to primitive types ([219a43e](https://github.com/glutinum-org/cli/commit/219a43e90674c45c811daa50e699016d6b4f028e))
* Keep sealed type parameters on type declarations and expose a specialized alias ([195b9da](https://github.com/glutinum-org/cli/commit/195b9da4f16f1fcfacb9c6424c90c435672308be))
* Read a single type union with a leading pipe as its type ([1576711](https://github.com/glutinum-org/cli/commit/1576711e265051d2b8a66e682376a8af3c3634d7))
* Prevent infinite recursion on recursive `Partial<_>` in a heritage clause ([6ce7d02](https://github.com/glutinum-org/cli/commit/6ce7d02eead668429f7276ddf1c7cf86edc61534))
* Generate namespaces declared with a dotted name ([bd1b428](https://github.com/glutinum-org/cli/commit/bd1b428a3e5e2a6f0ff5228ae7423d91652f1339))
* Don't wrap optional unions twice in `option` ([2a5d428](https://github.com/glutinum-org/cli/commit/2a5d4288ea234e8c56022f79e1d2de1a58899d15))
* Keep the parameters and return type of constructor types ([195dbe6](https://github.com/glutinum-org/cli/commit/195dbe60275c02b41a28e15c53ee2fc4a33152ba))
* Read properties created by mapped types ([0ce0de4](https://github.com/glutinum-org/cli/commit/0ce0de4bb868eba2e22c3027c229f38ce1a580bc))
* Declare type parameters on interfaces generated for object types ([253d433](https://github.com/glutinum-org/cli/commit/253d433eb3ac801c1fb5aa71ffee604e9a28d27f))
* Don't escape a name twice when it is used as a scope ([367efa5](https://github.com/glutinum-org/cli/commit/367efa5074662e814d0065c1e86b3783007c1600))
* Read `typeof` of a method as a function type ([cfeb4c5](https://github.com/glutinum-org/cli/commit/cfeb4c5a5c3ba53e886455006242e6a7d7193a53))
* Nest the type of an exported variable under a module named after it ([52d027f](https://github.com/glutinum-org/cli/commit/52d027f8252caf80baedb6f6d5a136cdeffbac92))
* Generate default type parameter aliases for type aliases ([dc9ed18](https://github.com/glutinum-org/cli/commit/dc9ed185831cacb28203f264ecf9f04d7f980856))
* Read the variables of an ambient namespace as exported ([b4dab8b](https://github.com/glutinum-org/cli/commit/b4dab8b32f63de545bae2b7bdd32c942b96e241e))
* Make `--all` work on packages like minimatch ([5d36589](https://github.com/glutinum-org/cli/commit/5d36589b8cb57e87daf92ddfeebc7ae9a2eedbef))
* Reference the declaration name of a renamed import ([9ab569b](https://github.com/glutinum-org/cli/commit/9ab569b2858c4b05bd149550cf607bd4ce6cbe4c))
* Read the members of an instantiated generic type from the checker ([ba59fdf](https://github.com/glutinum-org/cli/commit/ba59fdf438eed856f54c9cf74ccdd31140f9d11f))
* Generate valid constraints and delegates for generic declarations ([9c00a72](https://github.com/glutinum-org/cli/commit/9c00a729c75869af6495250bd75df0b1a28350d5))
* Generate a class for a subclass of an `Error` subclass ([8e3f65b](https://github.com/glutinum-org/cli/commit/8e3f65b2925ca73fb8400673d87e2764e7f29b36))
* Don't generate overloads F# can't tell apart ([bfc2475](https://github.com/glutinum-org/cli/commit/bfc24756904e719dcc62e4f3e1d12c084b5ac129))
* Resolve `Foo["bar"]` and export aliases through the checker ([26f30ae](https://github.com/glutinum-org/cli/commit/26f30ae005ead1bde4272c643ede03ad6b4bf75d))
* Expose the anonymous type of an optional alias under a `Value` module ([f90cf52](https://github.com/glutinum-org/cli/commit/f90cf52ec25c3c37b7cf1b67d1ff126b1610e40d))
* Don't clash a default export with a member of the same name ([2515e53](https://github.com/glutinum-org/cli/commit/2515e53829ffa250b758ae9bfc49605d021e6bad))
* Generate packages installed by pnpm ([5b3be4b](https://github.com/glutinum-org/cli/commit/5b3be4b53837376566ffc1c95bdea833e2d73b67))
* Drop nominal constraints and generate the remaining ts-morph constructs ([8a3674c](https://github.com/glutinum-org/cli/commit/8a3674c4ce9368e18607f766f00df52afc03f402))
* Generate the packages used by the Ionide extension ([8d1083e](https://github.com/glutinum-org/cli/commit/8d1083ee6db98e013ce8ba861fb064af3254d296))
* Generate `@ts-morph/bootstrap` without error ([72b156a](https://github.com/glutinum-org/cli/commit/72b156abd3fb8646f0b7ca43296d34f42bdae150))
* Generate `@types/node` without error ([9a6792e](https://github.com/glutinum-org/cli/commit/9a6792e3942a30cc46529f2fcaa84e2eaaa1b3ec))
* Only the properties of an intersection carry the enclosing type parameters ([6b08935](https://github.com/glutinum-org/cli/commit/6b0893511307f1d5467a395228047b4aa90654a1))
* Import an ambient module by its name and follow its `export =` ([925577f](https://github.com/glutinum-org/cli/commit/925577fa71adf760f74f991b1e3f4b8cbe239daa))
* Generate vite, vitest and @vitejs/plugin-react ([59aae06](https://github.com/glutinum-org/cli/commit/59aae06e0a8d63b0594fd78f3feb6f064158a7c2))
* Generate `@types/node` without warning ([b4064c2](https://github.com/glutinum-org/cli/commit/b4064c2008f31ba7bfd66aa128941a0cbd6c89f1))
* The members of an `export =` object are the exports of the module ([e6f331d](https://github.com/glutinum-org/cli/commit/e6f331d0ee34219478b65b7ce074d7d578820f69))
* Names and generics found by generating Playwright ([9df1aee](https://github.com/glutinum-org/cli/commit/9df1aee3f6a906a29bb365f49df6ae772196f138))
* A `void` alias property has no setter and constructors are distinct through aliases ([676df2f](https://github.com/glutinum-org/cli/commit/676df2f136680466472a8009bb6ead3af0947a62))
* An anonymous type identical to one of its scope takes its name ([3aa5613](https://github.com/glutinum-org/cli/commit/3aa56133337704342710141341d9b752d1d2444d))
* `Parameters<F>`, intrinsic string types, `NoInfer` and tuple rest parameters ([a23cb77](https://github.com/glutinum-org/cli/commit/a23cb7721937c76edde8634e2727f685106d8917))
* Generate date-fns, Chart.js, CodeMirror and ECharts ([761ebeb](https://github.com/glutinum-org/cli/commit/761ebebf32d076a808c03f5d071a5329c8b3d0f3))
* Generate express, luxon, leaflet, yaml, d3, three and rxjs ([f60b33c](https://github.com/glutinum-org/cli/commit/f60b33cc6a527e433b47fa2f1e818d7ef5441b61))
* `export =` and `export default class` are default imports ([effe9f0](https://github.com/glutinum-org/cli/commit/effe9f06aec7463ae7d62ca190549c1c056fac45))
* A property declared by several members of an intersection keeps its type ([40ff6b8](https://github.com/glutinum-org/cli/commit/40ff6b8a464d633ca24c3120c3a4018c2af5dfe6))
* Signatures copied from another file, synthesized signatures and Partial aliases ([4621e8f](https://github.com/glutinum-org/cli/commit/4621e8f26206bac5879bfccb79e7fdae0fa9f42a))
* The type parameters of a one-parameter function type are their default ([c1175a1](https://github.com/glutinum-org/cli/commit/c1175a12c2e64c324d8418e54e5b5d7802ba6719))
* The members of an `export =` object go through the default import ([152d3f2](https://github.com/glutinum-org/cli/commit/152d3f28df0ad095cc5176ff7ea7b3937f43a78c))
* `Partial`, `Omit` and `Exclude` written by the checker are expanded ([5bf78c8](https://github.com/glutinum-org/cli/commit/5bf78c8540c45e7ded322f8ca9ef64923541d508))
* A variable without a type annotation has the type of its initializer ([f6b5404](https://github.com/glutinum-org/cli/commit/f6b5404bea3aa6182fe97c8be47d913c5dc7dc3b))
* Renamed local exports and classes that are not exported ([b79494d](https://github.com/glutinum-org/cli/commit/b79494d12d1550d8f8ab09f23bc1aaa587200237))
* No solution folder shares a name with a project ([420b976](https://github.com/glutinum-org/cli/commit/420b9761e7d8ad41b548349bda2d500fd71d524e))
* The generated bindings are written formatted ([7c7d588](https://github.com/glutinum-org/cli/commit/7c7d588b0717aaa317a4db15c36071991792239b))
* Every generated binding is formatted, not only the one under src ([17764a8](https://github.com/glutinum-org/cli/commit/17764a85a92b04a6dcb032efe7fcb773cf052877))

<strong><small>[View changes on Github](https://github.com/glutinum-org/cli/compare/24210b74ed4fac16d7c232d822ede822c7a2c38b..865f54ebdd8defd9b7f8e3f26e42bf563043372d)</small></strong>

## 0.13.0

### 🐞 Bug Fixes

* Fix: Generate a concrete version of interface with constrained/default type parameters ([24210b7](https://github.com/glutinum-org/cli/commit/24210b74ed4fac16d7c232d822ede822c7a2c38b))

    ```ts
    declare interface Options {}

    declare interface User<T extends Options = Options> {}
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type User<'T when 'T :> Options> =
        interface end

    type User =
        User<Options>
    ```

    Fix #211

* Add support for `+`, `~` and `""` (empty strings) in string enum literals ([73c5803](https://github.com/glutinum-org/cli/commit/73c5803b3f8d6f088afbace9a4f50ae2fbd83234))

    ```ts
    type ClauseCombinator = '' | '>' | '+' | '~' | '>='
    ```

    ```fs
    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type ClauseCombinator =
        | [<CompiledName("")>] _EMPTY_
        | ``>``
        | [<CompiledName("+")>] _PLUS_
        | ``~``
        | ``>=``
    ```

* Alias empty literal string enum case to `Empty` ([903ac33](https://github.com/glutinum-org/cli/commit/903ac33abc292e5c7cdbe3afe8a23ce877daf839))

    ```ts
    type DevToolPosition = 'eval-' | '';
    ```

    ```fs
    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type DevToolPosition =
        | ``eval-``
        | [<CompiledName("")>] Empty
    ```

    Fix #206

* Transform Documentation for class and methods ([c69e79b](https://github.com/glutinum-org/cli/commit/c69e79b5bd705262cf05303cda56fa0a6c9d603d))

    ```ts
    /**
     * Represents a type which can release resources, such
     * as event listening or a timer.
     */
    declare class Disposable {
        /**
         * Dispose this object.
         */
        dispose(): any;
    }
    ```

    ```fs
    /// <summary>
    /// Represents a type which can release resources, such
    /// as event listening or a timer.
    /// </summary>
    [<AllowNullLiteral>]
    [<Interface>]
    type Disposable =
        /// <summary>
        /// Dispose this object.
        /// </summary>
        abstract member dispose: unit -> obj
    ```

    Fix #196

* Make generated delegate inherit the generic from their parent ([55d66ae](https://github.com/glutinum-org/cli/commit/55d66ae0db77cf626b1f0f2d18e380be116bb4b0))

    ```ts
    interface Thenable<R> {}

    declare function funcA<R>(task: (progress: any, data: any) =>
    Thenable<R>): Thenable<R>;
    ```

    ```fs
    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("funcA", "REPLACE_ME_WITH_MODULE_NAME")>]
        static member funcA<'R> (task: Exports.funcA.task<'R>) :
    Thenable<'R> = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type Thenable<'R> =
        interface end

    module Exports =

        module funcA =

            type task<'R> =
                delegate of progress: obj * data: obj -> Thenable<'R>
    ```

* Enum value in signature should be alias to their type directly ([2227bab](https://github.com/glutinum-org/cli/commit/2227bab3b03180965347a67c0a8b1f4e8fac5d53))
* Mangle generated types due to overload resolution (mainly for TypeLiterals, Records, etc.) ([b22c85c](https://github.com/glutinum-org/cli/commit/b22c85c1eecc37a0d8cd0b342dd36bb17e279fed))

    ```ts
    export interface TelemetryLogger {
        logError(eventName: string, data?: Record<string, any>): void;
        logError(error: Error, data?: Record<string, any>): void;
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type TelemetryLogger =
        abstract member logError: eventName: string * ?data:
    TelemetryLogger.logError.data -> unit
        abstract member logError: error: Exception * ?data:
    TelemetryLogger.logError.data_1 -> unit

    module TelemetryLogger =

        module logError =

            [<AllowNullLiteral>]
            [<Interface>]
            type data =
                [<EmitIndexer>]
                abstract member Item: key: string -> obj with get, set

            [<AllowNullLiteral>]
            [<Interface>]
            type data_1 =
                [<EmitIndexer>]
                abstract member Item: key: string -> obj with get, set
    ```

    Notice how each overload as its dedicated "data" type generated

* Generate a concrete version of `TaskProvider<T extends Task = Task>` ([b0c15ef](https://github.com/glutinum-org/cli/commit/b0c15ef073f3ef2366f4441c5c877bc0bdc244cd))

    ```ts
    export class Type2<A = string> {}
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type Type2<'A> =
        interface end

    type Type2 =
        Type2<string>
    ```

* Use `AbstractClass` to represent class extending `Error` from ES5 module ([5142edc](https://github.com/glutinum-org/cli/commit/5142edc4d760f0fb8cd3f25ef52b07990458642f))

    ```ts
    export class CancellationError extends Error {
        constructor();
    }
    ```

    ```fs
    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("CancellationError", "REPLACE_ME_WITH_MODULE_NAME");
    EmitConstructor>]
        static member CancellationError () : CancellationError = nativeOnly

    [<AllowNullLiteral>]
    [<AbstractClass>]
    type CancellationError =
        inherit Exception
    ```

* Remove duplicates from Generics generated from TypeLiteral ([5541a34](https://github.com/glutinum-org/cli/commit/5541a341c23ed9aaa0cb86e4c0105dfafe3754c2))
* Re-work how workspace exports their function/variable declaration ([fada073](https://github.com/glutinum-org/cli/commit/fada073b541bd72c5481850ba010b960b3b8a079))

    We are now using `abstract member` with `Emit` instead of static member

    ```ts
    declare module 'vscode' {
        export const version: string;
    }
    ```

    ```fs
    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
        static member inline vscode_
            with get () : vscode.Exports =
                nativeOnly

    module vscode =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Emit("$0.version")>]
            abstract member version: string
    ```

* Resolve type parameter when it is attached to the signature instead of the member ([63d5b05](https://github.com/glutinum-org/cli/commit/63d5b053384d2afb3df39a4ca7167068bf14229f))

    ```ts
    declare class Test {
        parseArg?: <T>(value: string, previous: T) => T;
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type Test =
        abstract member parseArg: Test.parseArg<'T> option with get, set

    module Test =

        type parseArg<'T> =
            delegate of value: string * previous: 'T -> 'T
    ```

    Note: The generated code is invalid because 'T definition is missing on
    `Test` type but this can be improved later. And user can more easily fix
    the code with the new version

* Alias functions with `...args` in signature as `System.Delegate` ([30c9fe2](https://github.com/glutinum-org/cli/commit/30c9fe29f0a50b4f4588aaa7c9d9a95b64b647cb))

    ```ts
    export class Commander {
        action(fn: (...args: any[]) => Promise<void>);
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type Commander =
        abstract member action: fn: System.Delegate -> unit
    ```

* Respect `readonly` property when defining type literal ([8d89cbe](https://github.com/glutinum-org/cli/commit/8d89cbeb15457085f95f50c484f9b777eba3b9d0))

    ```ts
    export declare const settings: {
        readonly enable: boolean;
    }
    ```

    ```fs
    module Exports =

        [<Global>]
        [<AllowNullLiteral>]
        type settings
            [<ParamObject; Emit("$0")>]
            (
                enable: bool
            ) =

            member val enable : bool = nativeOnly with get
    ```

    Fix #179

* Forward generics to the generated types when transforming a `TypeLiteral` ([e0039f4](https://github.com/glutinum-org/cli/commit/e0039f4b43737324b6cf7b056e85d046e4b7bf02))

    ```ts
    export type Callback<Param, AtomType> = (event: {
        atom: AtomType;
    }) => void;
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type Callback<'Param, 'AtomType> =
        [<Emit("$0($1...)")>]
        abstract member Invoke: event: Callback.event<'AtomType> -> unit

    module Callback =

        [<Global>]
        [<AllowNullLiteral>]
        type event<'AtomType>
            [<ParamObject; Emit("$0")>]
            (
                atom: 'AtomType
            ) =

            member val atom : 'AtomType = nativeOnly with get, set
    ```

    Fix #148

* If a tuple is empty generate `obj` (we don't have an equivalent to empty tuple in F#) ([ea1c0ce](https://github.com/glutinum-org/cli/commit/ea1c0ce2fa67e820edcfba66da57d66222618de9))

    ```ts
    type MyType = []
    ```

    ```fs
    type MyType =
        obj
    ```

* Support `@deprecated` with a multiline comment ([82766ab](https://github.com/glutinum-org/cli/commit/82766ab3737a9c0b24f4999133790c3dbbb1d00d))
* Prevent infinite loop on recursive type ([092b4f7](https://github.com/glutinum-org/cli/commit/092b4f7111783ce2631ef464a2c2b3b0ca0a3a98))

    ```ts
    export interface DiagnosticCollection {
        forEach(callback: (collection: DiagnosticCollection) => any): void;
    }
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type DiagnosticCollection =
        abstract member forEach: callback: (DiagnosticCollection -> obj) -> unit
    ```

### 🚀 Features

* Detect generic resulting in sealed type ([229a893](https://github.com/glutinum-org/cli/commit/229a893d161c81129874980ed40c84cab8f7b0a8))

    ```ts
    export function showInformationMessage<T extends string>() : T | number;
    ```

    ```fs
    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("showInformationMessage", "REPLACE_ME_WITH_MODULE_NAME")>]
        static member showInformationMessage () : U2<string, float> =
    nativeOnly
    ```

    Without this feature, we would resolve `<'T when 'T :> string>` which is
    not valid in F# because `:> string` is a sealed type and can only
    resolved to `string`

* Add support for additional Fable.Core.JS types (#172) ([01fa782](https://github.com/glutinum-org/cli/commit/01fa7828d9058ca23cec7d19691163926430d910))

    * `Int8Array`
    * `Uint8ClampedArray`
    * `Int16Array`
    * `Uint16Array`
    * `Int32Array`
    * `Uint32Array`
    * `Float32Array`
    * `Float64Array`

## 0.12.0

### 🚀 Features

* Add basic support for `Readonly<T>` ([20dab77](https://github.com/glutinum-org/cli/commit/20dab778db50f4529c7c641da8e8e51463b00eef))

    ```ts
    export interface TerminalOptions {
        prefix: string
    }

    export type ReadonlyTerminalOptions = Readonly<TerminalOptions>
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type TerminalOptions =
        abstract member prefix: string with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type ReadonlyTerminalOptions =
        abstract member prefix: string with get
    ```

* Make `Partial<T>` support more robust ([b7e9030](https://github.com/glutinum-org/cli/commit/b7e90308e4986429df8389fcb215a03d09c2f7da))

    * Support literal types:

        ```ts
        export type TodoPreview = Partial<{
            title: string;
            description: string;
        }>;
        ```

        ```fs
        [<AllowNullLiteral>]
        [<Interface>]
        type TodoPreview =
            abstract member title: string option with get, set
            abstract member description: string option with get, set
        ```

    * Support intersection

        ```ts
        interface Todo {
            title: string;
        }

        interface TodoExtra {
            author: string;
        }

        export type TodoPreview = Partial<Todo & TodoExtra>;
        ```

        ```fs
        [<AllowNullLiteral>]
        [<Interface>]
        type Todo =
            abstract member title: string with get, set

        [<AllowNullLiteral>]
        [<Interface>]
        type TodoExtra =
            abstract member author: string with get, set

        [<AllowNullLiteral>]
        [<Interface>]
        type TodoPreview =
            abstract member title: string option with get, set
            abstract member author: string option with get, set
        ```

* Add support for `Iterable` type ([12ad780](https://github.com/glutinum-org/cli/commit/12ad780b6050dc54eeb288859c2f7aec30551aaa))

    Iterable can be detect if a type `implements Iterable<...>` or if a TypeLiteral has a `[Symbol.iterator]()` method signature.

    For `[Symbol.iterator]()`, we also support auto detection of the type looking at the `next.value` signature type.

    ```ts
    export declare class DataTransfer implements Iterable<string> {
        [Symbol.iterator](): IterableIterator<string>
    }

    export type MyIterable = {
        [Symbol.iterator](): {
            next(): {
                done: boolean;
                value: number;
            };
            return(): {
                done: boolean;
            };
        };
    };
    ```

    ```fs
    <AllowNullLiteral>]
    [<Interface>]
    type DataTransfer =
        inherit Iterable<string>

    [<AllowNullLiteral>]
    [<Interface>]
    type MyIterable =
        inherit Iterable<float>
    ```

* Add support for `Omit<Type, Keys>` ([3e00942](https://github.com/glutinum-org/cli/commit/3e009427578702cbc0863b0be80003e4eec8d2f4))

    ```ts
    export interface Todo {
        title: string;
        description: string;
        completed: boolean;
        createdAt: number;
    }

    export type TodoPreview = Omit<Todo, "description">;
    ```

    ```fs
    [<AllowNullLiteral>]
    [<Interface>]
    type Todo =
        abstract member title: string with get, set
        abstract member description: string with get, set
        abstract member completed: bool with get, set
        abstract member createdAt: float with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type TodoPreview =
        abstract member title: string with get, set
        abstract member completed: bool with get, set
        abstract member createdAt: float with get, set
    ```

### 🐞 Bug Fixes

* Support single case out of `Exclude<UnionType, ExcludedMembers>` ([7b8a7c0](https://github.com/glutinum-org/cli/commit/7b8a7c029f873df20678fef644617b39f5b7de07))

    ```ts
    export type NumberB = Exclude<1 | 2, 2>;

    export type PrimitiveResult = Exclude<"a" | "b", "a">;
    ```

    ```fs
    [<RequireQualifiedAccess>]
    type NumberB =
        | ``1`` = 1

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type PrimitiveResult =
        | b
    ```

* Type constraint should not be generated when the type is a "return type" ([73d6cd7](https://github.com/glutinum-org/cli/commit/73d6cd734fd815508d29c9e430727db1ecc45d6b))

    ```ts
    class A {}

    class User<T extends A = A> {}
    ```

    ```fs
    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("User", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
        static member User<'T when 'T :> A> () : User<'T> = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type A =
        interface end

    [<AllowNullLiteral>]
    [<Interface>]
    type User<'T when 'T :> A> =
        interface end
    ```

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
