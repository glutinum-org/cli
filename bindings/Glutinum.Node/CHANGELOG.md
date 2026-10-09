---
last_commit_released: e1058ae6bee597cc187ed10925d0cf9529f279bb
name: Glutinum.Node
---

# Changelog

All notable changes to this project will be documented in this file.

This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

This changelog is generated using [EasyBuild.ShipIt](https://github.com/easybuild-org/EasyBuild.ShipIt).

⚠ Only edit the front matter metadata at the top of this file. All other changes will be overwritten when a new release is created.

## 1.0.0-beta.7 - 2026-10-09

### 🐞 Bug Fixes

* Constructs found by compiling 21 candidate packages ([e1058ae](https://github.com/glutinum-org/cli/commit/e1058ae6bee597cc187ed10925d0cf9529f279bb))

<strong><small>[View changes on Github](https://github.com/glutinum-org/cli/compare/ef5678e05c1cc95590a4afe7d63c3133d9a70d3f..e1058ae6bee597cc187ed10925d0cf9529f279bb)</small></strong>

## 1.0.0-beta.6 - 2026-10-09

### 🚀 Features

* A named function type with one parameter is an F# lambda, not a delegate ([ef5678e](https://github.com/glutinum-org/cli/commit/ef5678e05c1cc95590a4afe7d63c3133d9a70d3f))

<strong><small>[View changes on Github](https://github.com/glutinum-org/cli/compare/9612425df7d3dc823d984920fb0505f00e954a2b..ef5678e05c1cc95590a4afe7d63c3133d9a70d3f)</small></strong>

## 1.0.0-beta.5 - 2026-10-09

### 🐞 Bug Fixes

* Import or global attribute on the class types, a type test needs the class ([9612425](https://github.com/glutinum-org/cli/commit/9612425df7d3dc823d984920fb0505f00e954a2b))

<strong><small>[View changes on Github](https://github.com/glutinum-org/cli/compare/25e4e950d6172c66305950971f9e7b1340592cc5..9612425df7d3dc823d984920fb0505f00e954a2b)</small></strong>

## 1.0.0-beta.4 - 2026-10-07

### 🚀 Features

* Generate Create for every interface made of properties ([d458929](https://github.com/glutinum-org/cli/commit/d45892922c5e3554fce201c7b8d4d2e65ffc73bd))

### 🐞 Bug Fixes

* Print one erased cast for the typed array names Fable.Core shares ([25e4e95](https://github.com/glutinum-org/cli/commit/25e4e950d6172c66305950971f9e7b1340592cc5))

<strong><small>[View changes on Github](https://github.com/glutinum-org/cli/compare/f7edba103bf435df7b4f7be955725218d61f7e5a..25e4e950d6172c66305950971f9e7b1340592cc5)</small></strong>

## 1.0.0-beta.3 - 2026-10-02

### 🚀 Features

* Create a param object for an object literal type alias ([89ca42b](https://github.com/glutinum-org/cli/commit/89ca42be6ca5a724dabd3f6c3b3fabf920959e95))
* Name the values of a boolean in a literal union ([410e23a](https://github.com/glutinum-org/cli/commit/410e23ad7f696c07083dc9f7f2e407af28703b97))
* Create a param object for a base interface ([08037e0](https://github.com/glutinum-org/cli/commit/08037e0e66b48a107b90f85e5646fd7b2428b813))
* Create a param object for a nested or generic interface ([5d4f7ef](https://github.com/glutinum-org/cli/commit/5d4f7ef256bb99fd4d034d33141fabb632392198))
* Create a param object for every interface used as an argument ([b49f371](https://github.com/glutinum-org/cli/commit/b49f371cd21d278adce456ff54c903d5d107f00f))
* Resolve Omit and Pick with the TypeScript checker ([c4e7939](https://github.com/glutinum-org/cli/commit/c4e7939eea34dac6dca7c941c58825176bcbc4d0))
* Nest the module of an unpublished file after its directories ([c9bbfee](https://github.com/glutinum-org/cli/commit/c9bbfeea84763c50fa9a22a6a76174d13624907f))
* Import a Node built-in through its node: alias ([f07b5de](https://github.com/glutinum-org/cli/commit/f07b5de65cf1c77215b1fb4204b155247815589d))
* Nest an ambient module of a script after its specifier ([7ba3ef7](https://github.com/glutinum-org/cli/commit/7ba3ef747e426d45700f97c4be771f7b855f814e))
* Resolve conditional types with the type checker ([dc4fa6f](https://github.com/glutinum-org/cli/commit/dc4fa6fa2a488490e37c9bb9381191bf47744de0))
* Generate Create for an option bag with an index signature ([13b31fb](https://github.com/glutinum-org/cli/commit/13b31fbf0329b991766c8cc335e3d6db26173234))

### 🐞 Bug Fixes

* Drop a parameter that can only be undefined ([5fee048](https://github.com/glutinum-org/cli/commit/5fee0484fe34ed6acb3bd55bd79d95616e131b94))
* An optional method is an optional function member ([efba79c](https://github.com/glutinum-org/cli/commit/efba79cb45418c8920784afae710113cd33c9849))
* Inherit the class of an intersection instead of falling back to obj ([af3932d](https://github.com/glutinum-org/cli/commit/af3932d321f514f26cca270735b75c4faa6e81bc))
* Make overloads sharing their required parameters distinguishable ([4dda3b4](https://github.com/glutinum-org/cli/commit/4dda3b49d9323e6196e65234ffd078328cf5555c))
* Keep the union overload beside the expanded arms ([aeb7f13](https://github.com/glutinum-org/cli/commit/aeb7f1360356ec586bd3a3792cccc93f35f6a76f))
* Recover the declarations generated as empty types ([8ac0e98](https://github.com/glutinum-org/cli/commit/8ac0e98d2854b26b2c7d72676e718057fa7489cc))
* Read a Pick of unbound keys as obj ([43cd73e](https://github.com/glutinum-org/cli/commit/43cd73edf328cadcf1fdb72e307912a81d996fe4))
* Read the declared type of a default exported variable ([7413dfe](https://github.com/glutinum-org/cli/commit/7413dfe3a173215d1857d2c7bfb3ea58d50c2726))
* Number an anonymous type inside its module ([326837f](https://github.com/glutinum-org/cli/commit/326837fc97f3e1be8a131c6e5b9cd8900f20a4c7))
* Compile the msal-browser binding ([63b31fc](https://github.com/glutinum-org/cli/commit/63b31fcdbb2f2460f47e483752db3e4962051959))
* Drop the heritage a redeclared method makes ambiguous ([31bcafa](https://github.com/glutinum-org/cli/commit/31bcafa2c3349947cafd082281ab18445e8c22e1))

<strong><small>[View changes on Github](https://github.com/glutinum-org/cli/compare/bc253d603fb4d98975432ad990924d4c3ddd58c6..f7edba103bf435df7b4f7be955725218d61f7e5a)</small></strong>

## 1.0.0-beta.2 - 2026-09-28

### 🚀 Features

* Convert a value into an erased union implicitly ([92cf675](https://github.com/glutinum-org/cli/commit/92cf6755bb1db1a5a395f767b0518fe6ada61852))

### 🐞 Bug Fixes

* Type test a JavaScript error against its imported class ([086325d](https://github.com/glutinum-org/cli/commit/086325d075b62a90dc75194f4400412129d7683a))
* Type a method of a param object as a function ([214cf4a](https://github.com/glutinum-org/cli/commit/214cf4a8e7edb1486e14cf7d190d3acae5b5cec4))

<strong><small>[View changes on Github](https://github.com/glutinum-org/cli/compare/21b5bdf926daf6e2939b61a241861beb1a0623a6..bc253d603fb4d98975432ad990924d4c3ddd58c6)</small></strong>

## 1.0.0-beta.1 - 2026-09-27

### 🏗️ Breaking changes

* An object type is an interface with a `Create` static member ([7e36e60](https://github.com/glutinum-org/cli/commit/7e36e609984c8ff806a258bc94cb8fca3e6a39e3))

### 🚀 Features

* Run on TypeScript 6 with a compiler binding generated by Glutinum ([6a10fac](https://github.com/glutinum-org/cli/commit/6a10fac67e6aa9231b5d2bd49f63f86331cdb600))
* Emit callable members as `Emit` attributes so bindings stay pure interop ([31df455](https://github.com/glutinum-org/cli/commit/31df455fd87195c4cdfdb358e6dc194b91e1a11d))

### 🐞 Bug Fixes

* A types-only package generates no import ([cb6356b](https://github.com/glutinum-org/cli/commit/cb6356bad5d9ef17e35a3e61b2ddc9ea679bf223))
* `typeof` a namespace is the `Exports` of its module ([017cf78](https://github.com/glutinum-org/cli/commit/017cf786dbbde9356f581232b7a34e84e12ad50a))
* An anonymous type reached twice is one definition ([9310a98](https://github.com/glutinum-org/cli/commit/9310a98f75f1db9c870d71955859400ae9a238c5))
* An overload subsumed by a longer one is erased ([e9e348e](https://github.com/glutinum-org/cli/commit/e9e348e897ae0779fee2b27c2fd571bc1bd18554))
* A dropped type parameter constraint or default exposes no type ([2276503](https://github.com/glutinum-org/cli/commit/22765030704d8f68142daea55ace2ff528f19f56))
* An anonymous type declares the type parameters its members use ([eabdb5f](https://github.com/glutinum-org/cli/commit/eabdb5f2fe2620136e425518a8c0d0d043eddf5f))
* An imported type is identified by its declaration ([d8ed49b](https://github.com/glutinum-org/cli/commit/d8ed49ba9f02fe58552110c7ed4fbf11fdffe379))
* A conditional collapses when its branches share a type ([d4aeb06](https://github.com/glutinum-org/cli/commit/d4aeb06d81c62f13e43a93656e9b036adc7de09a))
* An intersection F# cannot merge is inherited instead of erased ([659d420](https://github.com/glutinum-org/cli/commit/659d420bc9a7bae26be5fef89a92762056c6f8d2))
* An overload two others subsume is the one that resolves ([68ef1b5](https://github.com/glutinum-org/cli/commit/68ef1b576e0a97a0d8f0ffce1367510c120ec8e8))
* The companion module of a static member no longer shadows it ([6e1a85a](https://github.com/glutinum-org/cli/commit/6e1a85ac32de3f60132d7c0200cf63a67110b141))
* A tuple parameter is one argument, not several ([30139da](https://github.com/glutinum-org/cli/commit/30139daadb2636050427e63905e07e6b7325c438))
* Overloads F# cannot tell apart collapse into one ([f58b910](https://github.com/glutinum-org/cli/commit/f58b910b2c78b0c31facaf2f18e19f959ad0c643))
* A constraint written the same way is forwarded to the re-export ([05ebca7](https://github.com/glutinum-org/cli/commit/05ebca7bacbbb06859ef27c061861bbf93cfdf7f))
* An interface constraining an argument type parameter gets `Create` ([eb6af6e](https://github.com/glutinum-org/cli/commit/eb6af6e34272e5166fc76e416393ad6fdc2644d9))

<strong><small>[View changes on Github](https://github.com/glutinum-org/cli/compare/865f54ebdd8defd9b7f8e3f26e42bf563043372d..21b5bdf926daf6e2939b61a241861beb1a0623a6)</small></strong>

## 0.1.0 - 2026-09-19

### 🚀 Features

* Publish Glutinum.Web and Glutinum.Node from the repository ([e90f3dd](https://github.com/glutinum-org/cli/commit/e90f3ddb54c929c213d6af2c50ee2ecd9a221e92))
* Typed keys for `keyof` maps ([9f6e4b8](https://github.com/glutinum-org/cli/commit/9f6e4b8e41e230874240fd39ae7ed9f4be2488e4))
* Package globals at the package level and typed keys for functions ([b85805d](https://github.com/glutinum-org/cli/commit/b85805dfc7b166c7c0160e2afb06ab67872d5021))
* Typed listeners of the Node event emitter ([ea428f4](https://github.com/glutinum-org/cli/commit/ea428f41632137188d4ca7596b82667f1f1918f1))
* Conditional types resolved by the checker, the constraints and the known keys ([b135bd5](https://github.com/glutinum-org/cli/commit/b135bd50e549e00f7043cda368235528554c9039))
* Overloads through a generic union alias and a type parameter case ([109a3d1](https://github.com/glutinum-org/cli/commit/109a3d15e9d4eeee8f5f7d10dc6d7e318d68894a))
* A property typed by a callable is a method ([b9d2984](https://github.com/glutinum-org/cli/commit/b9d29842ebf5ddd9937c5604020abd36ba414c19))
* Plain arguments where a union alias, a default or an option got in the way ([2517555](https://github.com/glutinum-org/cli/commit/2517555e42660c00c5ace5e5ddd47745d356b4ca))
* Arrays for readonly parameters, calls and values through the default import ([d359e77](https://github.com/glutinum-org/cli/commit/d359e776cf05a2dfa420467012bfc2acca1bb080))
* Infer constraints in conditional types and non-generic overloads of defaulted functions ([af408c6](https://github.com/glutinum-org/cli/commit/af408c6e79273e7e40aa0d5728a4f6bc678b3f1e))
* Mixed enums, variadic tuples, `this` parameters and assertion signatures ([0602097](https://github.com/glutinum-org/cli/commit/06020971729daac74c7f2fe284d90010cae5992d))
* `Glutinum.Types` is generated from the ES library files ([3197153](https://github.com/glutinum-org/cli/commit/31971538c26476615d1c90f9ae6fff57901f37da))
* `Date` and its constructor come from `Glutinum.Types` ([a55bb5f](https://github.com/glutinum-org/cli/commit/a55bb5f05cf21aa379eeb92fa7ba9f5343d23dbb))
* The type parameters of a method are declared ([4c79e16](https://github.com/glutinum-org/cli/commit/4c79e1677afea19071d2fd054195f4d4744dd1d7))

### 🐞 Bug Fixes

* Names and generics found by generating Playwright ([9df1aee](https://github.com/glutinum-org/cli/commit/9df1aee3f6a906a29bb365f49df6ae772196f138))
* An anonymous type identical to one of its scope takes its name ([3aa5613](https://github.com/glutinum-org/cli/commit/3aa56133337704342710141341d9b752d1d2444d))
* Generate date-fns, Chart.js, CodeMirror and ECharts ([761ebeb](https://github.com/glutinum-org/cli/commit/761ebebf32d076a808c03f5d071a5329c8b3d0f3))
* Generate express, luxon, leaflet, yaml, d3, three and rxjs ([f60b33c](https://github.com/glutinum-org/cli/commit/f60b33cc6a527e433b47fa2f1e818d7ef5441b61))
* A property declared by several members of an intersection keeps its type ([40ff6b8](https://github.com/glutinum-org/cli/commit/40ff6b8a464d633ca24c3120c3a4018c2af5dfe6))
* The members of an `export =` object go through the default import ([152d3f2](https://github.com/glutinum-org/cli/commit/152d3f28df0ad095cc5176ff7ea7b3937f43a78c))
* Every generated binding is formatted, not only the one under src ([17764a8](https://github.com/glutinum-org/cli/commit/17764a85a92b04a6dcb032efe7fcb773cf052877))

<strong><small>[View changes on Github](https://github.com/glutinum-org/cli/compare/12ad780b6050dc54eeb288859c2f7aec30551aaa..865f54ebdd8defd9b7f8e3f26e42bf563043372d)</small></strong>

## 0.0.0
