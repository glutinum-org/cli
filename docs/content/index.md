---
title: Glutinum
description: F# bindings for JavaScript libraries, generated from their TypeScript declarations
layout: splash
---

<div class="landing">
<section class="landing-hero">

<h1 class="landing-hero__title">Typed JavaScript, from F#</h1>

<p class="landing-hero__lede">Glutinum reads the TypeScript declarations of a JavaScript package and writes the F# binding for it. Fable compiles your F# against that binding, and the JavaScript library runs unchanged.</p>

<p class="landing-actions">
<a class="landing-button landing-button--primary" href="https://glutinum.net/app/">Try it in the browser</a>
<a class="landing-button" href="/guide/getting-started/">Get started</a>
<a class="landing-button" href="https://github.com/glutinum-org/cli">View on GitHub</a>
</p>

</section>

```bash frame="terminal"
npm install date-fns
npx @glutinum/cli date-fns --out-file Glutinum.DateFns.fs
```

<div class="landing-intro">

<h2 class="landing-section__title">One command per package</h2>

<p class="landing-lede">Name an installed package and Glutinum finds its declarations, follows the packages they depend on, and writes one F# file. A single <code>.d.ts</code> file works too.</p>

</div>

```fsharp title="Program.fs"
open Fable.Core
open Glutinum.DateFns

let today = JS.Constructors.Date.Create(2026, 8, 17)
let later = DateFns.addDays (today, 7)

printfn "%s" (DateFns.format (later, "yyyy-MM-dd"))
```

<div class="landing-grid">

<div class="landing-card">
<h3>Members, not dictionaries</h3>
<p>Interfaces become F# interfaces, classes get a constructor in <code>Exports</code>, functions become static members with the right overloads.</p>
</div>

<div class="landing-card">
<h3>Unions you can match on</h3>
<p>String literal unions are string enums, mixed unions are <code>U2</code> and <code>U3</code>, optional members are <code>option</code>.</p>
</div>

<div class="landing-card">
<h3>Shared runtime bindings</h3>
<p>The DOM and Node types come from <code>Glutinum.Web</code> and <code>Glutinum.Node</code>. A binding references them instead of embedding a copy.</p>
</div>

<div class="landing-card">
<h3>Honest about the gaps</h3>
<p>What TypeScript can express and F# cannot becomes <code>obj</code>, and the <a href="/guide/limitations/">limitations page</a> says which constructs those are.</p>
</div>

</div>

<section class="landing-hero landing-hero--closing">

<h2 class="landing-hero__title">Start with a package you already use</h2>

<p class="landing-actions">
<a class="landing-button landing-button--primary" href="/guide/getting-started/">Read the guide</a>
<a class="landing-button" href="/bindings/">Browse the bindings</a>
</p>

</section>
</div>
