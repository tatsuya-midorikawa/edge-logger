module PwshTest

open System
open System.Threading
open System.Threading.Tasks
open Xunit
open Xunit.Abstractions
//open jp.dsi.logger.misc


let pwsh = System.IO.Path.GetFullPath "./pwsh.exe"
let debug'log = System.Diagnostics.Debug.WriteLine
let cts = new CancellationTokenSource()

type PwshTest (output: ITestOutputHelper) =
  interface IDisposable with
    member __.Dispose () =
      cts.Dispose()

  [<Fact>]
  member __.``My test`` () =
    output.WriteLine pwsh
    Assert.True(true)

  [<Fact>]
  member __.``My test 2`` () =
    Assert.True(true)
