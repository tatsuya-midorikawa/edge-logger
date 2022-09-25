namespace jp.dsi.logger.tests

open System
open Xunit
open Xunit.Abstractions
open jp.dsi.logger.tools

type experiment (output: ITestOutputHelper) =
  let log msg = output.WriteLine $"{msg}"

  [<Fact>]
  member __.``exp.001`` () =
    let (|Some|Empty|) (xs: array<_>) =
      if xs = defaultof<array<_>> 
      then Empty
      else
        match xs.Length with
        | 0 -> Empty
        | 1 -> Some (xs[0], Array.empty<_>)
        | _ -> Some ((xs[0], xs[1..]))

    let xs = [| 0 |]
    //let xs = [| 0; 1; 2; 3; 4; 5 |]
    match xs with
    | Empty -> log "empty"
    | Some (head, tail) -> tail |> Array.iter log