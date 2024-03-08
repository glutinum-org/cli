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
    | CompilationResult.Success(fsharpCode, warnings) ->
        if warnings.IsEmpty then
            $"""**FSharp**

```fs
%s{fsharpCode}
```"""

        else
            let warningsText =
                warnings
                |> List.map (fun warning ->
                    printfn "%A" (warning.Split('\n'))

                    warning.Replace("\n", "\n> ")
                )
                |> String.concat "\n> ```\n> ```"

            $"""**FSharp (with warnings)**

```fs
%s{fsharpCode}
```

> [!WARNING]
> ```
> %s{warningsText}
> ```"""

    | CompilationResult.TypeScriptReaderException error ->
        $"""**Reader failed**

```
%s{error}
```"""

    | CompilationResult.Error error ->
        $"""**Error**

```
%s{error}
```"""

let createUrl (args: CreateUrlArgs) =
    let hashUrl =
        args.TypeScriptCode |> Some |> Router.Route.Editors |> Router.toHash

    let toolUrl =
        [
            window.location.protocol
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
