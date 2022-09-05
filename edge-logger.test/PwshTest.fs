module Tests

open System
open Xunit
open Xunit.Abstractions

let pwsh = System.IO.Path.GetFullPath "./pwsh.exe"
let debug'log = System.Diagnostics.Debug.WriteLine

type PwshTest (output: ITestOutputHelper) =
  [<Fact>]
  member __.``My test`` () =
    output.WriteLine pwsh
    Assert.True(true)
