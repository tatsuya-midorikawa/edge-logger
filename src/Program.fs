open System.Text.Json
open Microsoft.Win32
open System.IO
open System.Diagnostics
open System.Web
open System
open System.Linq
open System.Collections.Generic

open InternetOption
open ConsoleAppFramework
open System.Runtime.InteropServices

let inline wait<'T> (task: System.Threading.Tasks.Task<'T>) = System.Threading.Tasks.Task.WaitAll (task)
let inline toJson<'T> (object: 'T) = JsonSerializer.Serialize(object, JsonSerializerOptions(WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, IgnoreNullValues = true))
let inline clear () = System.Console.Clear()
let empty'task = System.Threading.Tasks.Task.Run<unit>(fun () -> ())

type Command () =
  inherit ConsoleAppBase ()
  [<RootCommand>]
  member __.root(
    [<Option("d", "Specify the output dir for log.");Optional;DefaultParameterValue(@"C:\logs")>] dir: string,
    [<Option("w", "Output Windows settings info.");Optional;DefaultParameterValue(false)>] winsrv: bool,
    [<Option("e", "Output Edge settings info.");Optional;DefaultParameterValue(false)>] edge: bool,
    [<Option("i", "Output internet option info.");Optional;DefaultParameterValue(false)>] ie: bool,
    [<Option("u", "Output Logon user info.");Optional;DefaultParameterValue(false)>] usr: bool,
    [<Option("f", "Output full info.");Optional;DefaultParameterValue(false)>] full: bool) = 
    
    let log = Logger.log dir
    let winsrv'task =
      if full || winsrv
      then 
        task {
          try do! Winsrv.getServices() |> Winsrv.collect |> toJson |> Logger.output (Logger.winsrv'filepath dir) with e -> log e.Message |> wait
          try do! Cmd.schtasks |> Cmd.exec |> Logger.output (Logger.schtasks'filepath dir) with e -> log e.Message |> wait
         }
      else empty'task

    let edge'task =
      if full || edge 
      then
        task {
          try do! EdgePolicy.fetch () |> toJson |> Logger.output (Logger.edge'filepath dir) with e -> log e.Message |> wait
          try EdgePolicy.installer'log |> Logger.copy (Logger.edge'installer'filepath dir) with e -> log e.Message |> wait
          try EdgePolicy.update'log |> Logger.copy (Logger.edge'update'filepath dir) with e -> log e.Message |> wait
        }
      else empty'task

    let ie'task =
      if full || ie
      then try IEReg.getIeRegistries () |> toJson |> Logger.output (Logger.ie'filepath dir) with e -> log e.Message
      else empty'task

    let usr'task =
      if full || usr
      then
        task {
          try do! Cmd.dsregcmd |> Cmd.exec |> Logger.output (Logger.dsregcmd'filepath dir) with e -> log e.Message |> wait
          try do! Cmd.whoami |> Cmd.exec |> Logger.output (Logger.whoami'filepath dir) with e -> log e.Message |> wait
          try do! Cmd.cmdkey |> Cmd.exec |> Logger.output (Logger.cmdkey'filepath dir) with e -> log e.Message |> wait
          try do! Pwsh.hotfix |> Pwsh.exec |> Logger.output (Logger.hotfix'filepath dir) with e -> log e.Message |> wait
        }
      else
        empty'task

    task {
      try do! winsrv'task with e -> log e.Message |> wait
      try do! edge'task with e -> log e.Message |> wait
      try do! ie'task with e -> log e.Message |> wait
      try do! usr'task with e -> log e.Message |> wait
    }
    |> wait
    
    Cmd.exec [$"explorer %s{dir}"] |> ignore
  
#if DEBUG
[<EntryPoint>]
let main args =
  let dir = "C:\\logs" |> Path.GetFullPath
  //Pwsh.hotfix |> Pwsh.exec |> printfn "%s"
  //clear()
  //Cmd.exec [$"explorer %s{dir}"] |> ignore

  //System.Threading.Tasks.Task.WaitAll (Cmd.schtasks |> Cmd.exec |> Logger.output (Logger.schtasks'filepath "C:\\logs"))
  //task {
  //  do! EdgePolicy.fetch () |> toJson |> Logger.output (Logger.edge'filepath dir)
  //  EdgePolicy.installer'log |> Logger.copy (Logger.edge'installer'filepath dir)
  //  EdgePolicy.update'log |> Logger.copy (Logger.edge'update'filepath dir)
  //}
  //|> System.Threading.Tasks.Task.WaitAll 

  //Reg.read (root.HKCU,  @"SOFTWARE\Policies\Microsoft\Edge")
  Reg.read (root.HKLM,  @"SOFTWARE\Policies\Microsoft\Edge")
  |> toJson
  |> printfn "%s"

  //[|
  //  (root.HKLM,  @"SOFTWARE\Policies\Microsoft\Edge")
  //  (root.HKCU,  @"SOFTWARE\Policies\Microsoft\Edge")
  //|]
  //|> Reg.reads
  //|> toJson
  //|> printfn "%s"

  0

#else
[<EntryPoint>]
let main args =
  ConsoleApp.Run<Command>(args)
  clear()
  printfn "This process has been completed."
  0
#endif