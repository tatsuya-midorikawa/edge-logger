namespace jp.dsi.logger.tests

open System
open Xunit
open Xunit.Abstractions
open jp.dsi.logger.tools

type LoggerTest (output: ITestOutputHelper) =
  let log msg = output.WriteLine $"{msg}"

  [<Fact>]
  member __.``Logger.log test`` () =
    Logger.log @"C:\logs" "foo"
    |> wait

  [<Fact>]
  member __.``test`` () = 
    //let desktop'dir = combine' [| System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop); "IEDigest" |]
    //desktop'dir |> log
    //Tools.remove desktop'dir
    //System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)
    System.Environment.GetFolderPath(System.Environment.SpecialFolder.Windows)
    |> log