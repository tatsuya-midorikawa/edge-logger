namespace jp.dsi.logger.tests

open System
open Xunit
open Xunit.Abstractions
open jp.dsi.logger.tools

type PwshTest (output: ITestOutputHelper) =
  let log msg = output.WriteLine $"{msg}"

  //[<Fact>]
  //member __.``get-hotfix test`` () =
  //  [| Pwsh.chain [| Pwsh.hotfix; Pwsh.output'file "./foo.log" |] |]
  //  |> Pwsh.exec
  //  |> log
  //  //|> Pwsh.chain
  //  //|> log

  //  Assert.True(true)

  [<Fact>]
  member __.``get'hesp test`` () =
    ProcessMitigation.get'hesp() |> log
  
  [<Fact>]
  member __.``sandbox test`` () =
    System.Reflection.Assembly.GetExecutingAssembly().Location
    |> log

  [<Fact>]
  member __.``Winsrv.output'win32service'list test`` () =
    Winsrv.output'win32service'list @"C:\logs"
    |> log

  [<Fact>]
  member __.``Winsrv.output'schtasks test`` () =
    Winsrv.output'schtasks @"C:\logs"
    |> log

  [<Fact>]
  member __.``Edge.output'policy test`` () =
    Edge.output'policy @"C:\logs"
    |> log

  [<Fact>]
  member __.``Edge.output'update'policy test`` () =
    Edge.output'update'policy @"C:\logs"
    |> log

  [<Fact>]
  member __.``Edge.output'ext'policy test`` () =
    Edge.output'ext'policy @"C:\logs"
    |> log

  [<Fact>]
  member __.``Edge.output'version test`` () =
    Edge.output'version @"C:\logs"
    |> log

  [<Fact>]
  member __.``Edge.output'msedge'update test`` () =
    Edge.output'msedge'update @"C:\logs"
    |> log

  [<Fact>]
  member __.``IE.output'reg test`` () =
    IE.output'reg @"C:\logs"
    |> log

  [<Fact>]
  member __.``test`` () =
    "foo:bar".Split(":")
    |> Array.iteri (fun i s -> log $"{i}: {s}")