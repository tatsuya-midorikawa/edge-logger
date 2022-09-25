namespace jp.dsi.logger.tests

open System
open Xunit
open Xunit.Abstractions
open jp.dsi.logger.tools

[<AutoOpen>]
module F =
  [<Struct;System.Runtime.CompilerServices.IsByRefLike;>]
  type s<'a> = { head: 'a; tail: Span<'a> }

  [<Struct;System.Runtime.CompilerServices.IsByRefLike;>]
  type t<'a> =
    | Empty
    | Some of s<'a>

  let split (xs: Span<_>) =
    match xs.Length with
    | 0 -> t.Empty
    | 1 -> t.Some { head = xs[0]; tail = Span<_>.Empty }
    | _ -> t.Some { head = xs[0]; tail = xs.Slice(1) }
    
  //[<Struct;System.Runtime.CompilerServices.IsByRefLike;>]
  //type t'<'a> =
  //  | Empty
  //  | Some of ('a * Span<'a>)

  //let split' (xs: Span<_>) =
  //  match xs.Length with
  //  | 0 -> t'.Empty
  //  | 1 -> t'.Some (xs[0], Span<_>.Empty)
  //  | _ -> t'.Some (xs[0], xs.Slice(1))
    
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

  [<Fact>]
  member __.``exp.002`` () =
    let xs = [| 0; 1; 2; 3; 4; 5 |]
    let span = xs.AsSpan()

    match split span with
    | t.Empty -> log "empty"
    | t.Some x -> x.tail.ToArray() |> Array.iter log
    //| t.Some x -> x.tail.ToArray() |> Array.iter log
    
    //match split' span with
    //| t'.Empty -> log "empty"
    //| t'.Some (head, tail) -> tail.ToArray() |> Array.iter log