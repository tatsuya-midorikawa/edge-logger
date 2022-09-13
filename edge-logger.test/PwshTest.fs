module Tests

open System
open Xunit
open Xunit.Abstractions
open jp.dsi.logger.tools

type PwshTest (output: ITestOutputHelper) =
  let log msg = output.WriteLine msg

  [<Fact>]
  member __.``get-hotfix test`` () =
    [| Pwsh.chain [| Pwsh.hotfix; Pwsh.output'file "./foo.log" |] |]
    |> Pwsh.exec
    |> log
    //|> Pwsh.chain
    //|> log

    Assert.True(true)
  
  [<Fact>]
  member __.``sandbox test`` () =
    System.Reflection.Assembly.GetExecutingAssembly().Location
    |> log