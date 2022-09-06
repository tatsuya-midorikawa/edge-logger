module PipesTest

open System
open Xunit
open Xunit.Abstractions
//open jp.dsi.logger.misc


type PipesTest (output: ITestOutputHelper) =
  [<Fact>]
  member __.``My test 1`` () =
    Assert.True(true)
  [<Fact>]
  member __.``My test 2`` () =
    Assert.True(true)
  [<Fact>]
  member __.``My test 3`` () =
    Assert.True(true)
