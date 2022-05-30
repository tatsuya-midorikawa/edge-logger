open System.Text.Json
open Microsoft.Win32
open System.IO
open System.Diagnostics
open System.Web
open System

open Reg
open ConsoleAppFramework
open System.Runtime.InteropServices

let inline wait<'T> (task: System.Threading.Tasks.Task<'T>) = System.Threading.Tasks.Task.WaitAll (task)
let inline toJson<'T> (object: 'T) = JsonSerializer.Serialize(object, JsonSerializerOptions(WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, IgnoreNullValues = true))
let emptyTask = System.Threading.Tasks.Task.Run<unit>(fun () -> ())

type Command () =
  inherit ConsoleAppBase ()
  [<RootCommand>]
  member __.root(
    [<Option("d", "Specify the dest dir for log output.");Optional;DefaultParameterValue(@"C:\logs")>] dir: string,
    [<Option("w", "Output Windows services info.");Optional;DefaultParameterValue(false)>] winsrv: bool,
    [<Option("e", "Output Edge policy info.");Optional;DefaultParameterValue(false)>] edge: bool,
    [<Option("i", "Output IE info.");Optional;DefaultParameterValue(false)>] ie: bool,
    [<Option("f", "Output full info.");Optional;DefaultParameterValue(false)>] full: bool) = 
    
    let winsrvTask =
      if full || winsrv then
        let path = Logger.winsrv'filepath dir
        Winsrv.getServices() |> Winsrv.collect |> toJson |> Logger.output path
      else
        emptyTask

    let edgeTask =
      if full || edge then
        let path = Logger.edge'filepath dir
        EdgeReg.getEdgeRegistries () |> toJson |> Logger.output path
      else
        emptyTask

    let ieTask =
      if full || ie then
        let path = Logger.ie'filepath dir
        IEReg.getIeRegistries () |> toJson |> Logger.output path
      else
        emptyTask
      
    let basicTask =
      let path = Logger.basic'filepath dir
      Basic.getInfo () |> toJson |> Logger.output path

    task {
      do! winsrvTask
      do! edgeTask
      do! ieTask
      do! basicTask
    }
    |> wait
  
[<EntryPoint>]
let main args =
  ConsoleApp.Run<Command>(args)
  0