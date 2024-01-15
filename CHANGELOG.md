# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased

### Fixed

* Transform `Promise` to `JS.Promise` (by @nojaf) ([GH-33](https://github.com/glutinum-org/cli/pull/33))

### Added

* Add support for `MethodSignature` on interface ([GH-28](https://github.com/glutinum-org/cli/issues/28]))

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
