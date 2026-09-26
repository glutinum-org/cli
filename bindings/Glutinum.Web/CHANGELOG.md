---
last_commit_released: 6e1a85ac32de3f60132d7c0200cf63a67110b141
name: Glutinum.Web
---

# Changelog

All notable changes to this project will be documented in this file.

This project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

This changelog is generated using [EasyBuild.ShipIt](https://github.com/easybuild-org/EasyBuild.ShipIt).

⚠ Only edit the front matter metadata at the top of this file. All other changes will be overwritten when a new release is created.

## 1.0.0 - 2026-09-26

### 🏗️ Breaking changes

* An object type is an interface with a `Create` static member ([7e36e60](https://github.com/glutinum-org/cli/commit/7e36e609984c8ff806a258bc94cb8fca3e6a39e3))

### 🐞 Bug Fixes

* An anonymous type reached twice is one definition ([9310a98](https://github.com/glutinum-org/cli/commit/9310a98f75f1db9c870d71955859400ae9a238c5))
* An overload subsumed by a longer one is erased ([e9e348e](https://github.com/glutinum-org/cli/commit/e9e348e897ae0779fee2b27c2fd571bc1bd18554))
* An anonymous type declares the type parameters its members use ([eabdb5f](https://github.com/glutinum-org/cli/commit/eabdb5f2fe2620136e425518a8c0d0d043eddf5f))
* The companion module of a static member no longer shadows it ([6e1a85a](https://github.com/glutinum-org/cli/commit/6e1a85ac32de3f60132d7c0200cf63a67110b141))

<strong><small>[View changes on Github](https://github.com/glutinum-org/cli/compare/865f54ebdd8defd9b7f8e3f26e42bf563043372d..6e1a85ac32de3f60132d7c0200cf63a67110b141)</small></strong>

## 0.1.0 - 2026-09-19

### 🚀 Features

* Publish Glutinum.Web and Glutinum.Node from the repository ([e90f3dd](https://github.com/glutinum-org/cli/commit/e90f3ddb54c929c213d6af2c50ee2ecd9a221e92))
* Typed keys for `keyof` maps ([9f6e4b8](https://github.com/glutinum-org/cli/commit/9f6e4b8e41e230874240fd39ae7ed9f4be2488e4))
* Package globals at the package level and typed keys for functions ([b85805d](https://github.com/glutinum-org/cli/commit/b85805dfc7b166c7c0160e2afb06ab67872d5021))
* Typed listeners of the Node event emitter ([ea428f4](https://github.com/glutinum-org/cli/commit/ea428f41632137188d4ca7596b82667f1f1918f1))
* Overloads through a generic union alias and a type parameter case ([109a3d1](https://github.com/glutinum-org/cli/commit/109a3d15e9d4eeee8f5f7d10dc6d7e318d68894a))
* A property typed by a callable is a method ([b9d2984](https://github.com/glutinum-org/cli/commit/b9d29842ebf5ddd9937c5604020abd36ba414c19))
* Plain arguments where a union alias, a default or an option got in the way ([2517555](https://github.com/glutinum-org/cli/commit/2517555e42660c00c5ace5e5ddd47745d356b4ca))
* Arrays for readonly parameters, calls and values through the default import ([d359e77](https://github.com/glutinum-org/cli/commit/d359e776cf05a2dfa420467012bfc2acca1bb080))
* Infer constraints in conditional types and non-generic overloads of defaulted functions ([af408c6](https://github.com/glutinum-org/cli/commit/af408c6e79273e7e40aa0d5728a4f6bc678b3f1e))
* `Glutinum.Types` is generated from the ES library files ([3197153](https://github.com/glutinum-org/cli/commit/31971538c26476615d1c90f9ae6fff57901f37da))
* `Date` and its constructor come from `Glutinum.Types` ([a55bb5f](https://github.com/glutinum-org/cli/commit/a55bb5f05cf21aa379eeb92fa7ba9f5343d23dbb))
* The type parameters of a method are declared ([4c79e16](https://github.com/glutinum-org/cli/commit/4c79e1677afea19071d2fd054195f4d4744dd1d7))

### 🐞 Bug Fixes

* An anonymous type identical to one of its scope takes its name ([3aa5613](https://github.com/glutinum-org/cli/commit/3aa56133337704342710141341d9b752d1d2444d))
* Generate date-fns, Chart.js, CodeMirror and ECharts ([761ebeb](https://github.com/glutinum-org/cli/commit/761ebebf32d076a808c03f5d071a5329c8b3d0f3))
* Every generated binding is formatted, not only the one under src ([17764a8](https://github.com/glutinum-org/cli/commit/17764a85a92b04a6dcb032efe7fcb773cf052877))

<strong><small>[View changes on Github](https://github.com/glutinum-org/cli/compare/12ad780b6050dc54eeb288859c2f7aec30551aaa..865f54ebdd8defd9b7f8e3f26e42bf563043372d)</small></strong>

## 0.0.0
