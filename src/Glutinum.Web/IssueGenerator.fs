module Glutinum.Web.IssueGenerator

open System
open Browser
open Glutinum.Converter
open Glutinum.Web.Global.Types

let private baseUrl = "https://github.com/glutinum-org/cli/issues/new"

type CreateUrlArgs =
    {
        TypeScriptCode: string
        CompilationResult: CompilationResult
    }

let private compilationResultToText (result: CompilationResult) =
    match result with
    | CompilationResult.Success(fsharpCode, warnings, errors) ->
        if warnings.IsEmpty && errors.IsEmpty then
            $"""**FSharp**

```fs
%s{fsharpCode}
```"""

        else
            let warningsText =
                if warnings.IsEmpty then
                    ""
                else
                    let warningsList =
                        warnings
                        |> List.map _.Replace("\n", "\n> ")
                        |> String.concat "\n> ```\n> ```"

                    $"""
> [!WARNING]
> ```
> %s{warningsList}
> ```
"""

            let errorsText =
                if errors.IsEmpty then
                    ""
                else
                    let errorsList =
                        errors |> List.map _.Replace("\n", "\n> ") |> String.concat "\n> ```\n> ```"

                    $"""
> [!CAUTION]
> ```
> %s{errorsList}
> ```
"""

            $"""**FSharp (with warnings/errors)**

```fs
%s{fsharpCode}
```
%s{warningsText}%s{errorsText}"""

    | CompilationResult.Error error ->
        $"""**Error**

```
%s{error}
```"""

let createUrl (args: CreateUrlArgs) =
    let hashUrl =
        args.TypeScriptCode
        |> Some
        |> Router.EditorsRoute.FSharpCode
        |> Router.Route.Editors
        |> Router.toHash

    let toolUrl =
        [
            window.location.protocol
            "//"
            window.location.host
            window.location.pathname
            hashUrl
        ]
        |> String.concat ""

    let issueBody =
        $"""<!---

IMPORTANT:

When reporting an issue, please try to make your code as minimal as possible.

-->

Issue created from [Glutinum Tool](%s{toolUrl})

**Glutinum version -** %s{Prelude.VERSION}

**TypeScript**

```ts
%s{args.TypeScriptCode}
```

%s{compilationResultToText args.CompilationResult}

**Problem description**

<!-- Please describe the problem you are facing below this line -->

        """
        // Remove leading and trailing whitespaces
        |> _.Trim()
        // Add new lines making it easier to write the problem description
        |> (fun str -> str + "\n\n")
        |> window.encodeURIComponent

    $"{baseUrl}?body=%s{issueBody}"
