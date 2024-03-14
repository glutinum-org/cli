module Tests.Dayjs

// open Glutinum.Dayjs
open Glutinum.Vitest

open type Glutinum.Vitest.Exports

test("2 + 2", fun _ ->
    expect.Invoke(2+2).toEqual(4)
)
