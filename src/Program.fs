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
    [<Option("nx", "Collecting net-export logs.");Optional;DefaultParameterValue(false)>] netexport: bool,
    [<Option("f", "Output full info.");Optional;DefaultParameterValue(false)>] full: bool) = 
    
    let log = Logger.log dir
    let winsrv'task =
      if full || winsrv
      then 
        task {
          // Windows Service information
          try do! Winsrv.getServices() |> Winsrv.collect |> toJson |> Logger.output (Logger.winsrv'filepath dir) with e -> log e.Message |> wait
          // Task scheduler information
          // schtasks /query /V /FO CSV
          try do! Cmd.schtasks |> Cmd.exec |> Logger.output (Logger.schtasks'filepath dir) with e -> log e.Message |> wait
         }
      else empty'task

    let edge'task =
      if full || edge 
      then
        task {
          // edge policy registry
          try do! EdgePolicy.fetch () |> toJson |> Logger.output (Logger.edge'filepath dir) with e -> log e.Message |> wait
          // msedge_installer.log
          try EdgePolicy.installer'log |> Logger.copy (Logger.edge'installer'filepath dir) with e -> log e.Message |> wait
          // MicrosoftEdgeUpdate.log
          try EdgePolicy.update'log |> Logger.copy (Logger.edge'update'filepath dir) with e -> log e.Message |> wait
        }
      else empty'task

    let ie'task =
      if full || ie
      // internet option registry
      then try IEReg.getIeRegistries () |> toJson |> Logger.output (Logger.ie'filepath dir) with e -> log e.Message
      else empty'task

    let usr'task =
      if full || usr
      then
        task {
          // dsregcmd /status
          try do! Cmd.dsregcmd |> Cmd.exec |> Logger.output (Logger.dsregcmd'filepath dir) with e -> log e.Message |> wait
          // whoami
          try do! Cmd.whoami |> Cmd.exec |> Logger.output (Logger.whoami'filepath dir) with e -> log e.Message |> wait
          // cmdkey /list
          try do! Cmd.cmdkey |> Cmd.exec |> Logger.output (Logger.cmdkey'filepath dir) with e -> log e.Message |> wait
          // get-hotfix
          try do! Pwsh.hotfix |> Pwsh.exec |> Logger.output (Logger.hotfix'filepath dir) with e -> log e.Message |> wait
        }
      else
        empty'task

    // Collection of various configuration information.
    task {
      try do! winsrv'task with e -> log e.Message |> wait
      try do! edge'task with e -> log e.Message |> wait
      try do! ie'task with e -> log e.Message |> wait
      try do! usr'task with e -> log e.Message |> wait
    }
    |> wait
    
    if netexport then 
      Cmd.psr'start dir |> Cmd.exec |> ignore
      
    // Collecting net-export logs.
    if netexport then
      try
        let readkey() = System.Console.ReadKey()
        let rec wait'for'input () =
          printfn "Entry (y) to exit."
          match readkey().Key with ConsoleKey.Y -> () | _ -> wait'for'input()
        Cmd.netexport dir |> Cmd.exec |> ignore
        wait'for'input ()
      with
        e -> log e.Message |> wait
       
    if netexport then 
      Cmd.psr'stop |> Cmd.exec |> ignore

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
  //Reg.read (root.HKLM,  @"SOFTWARE\Policies\Microsoft\Edge")
  //|> toJson
  //|> printfn "%s"

  //[|
  //  (root.HKLM,  @"SOFTWARE\Policies\Microsoft\Edge")
  //  (root.HKCU,  @"SOFTWARE\Policies\Microsoft\Edge")
  //|]
  //|> Reg.reads
  //|> toJson
  //|> printfn "%s"

  //System.Diagnostics.Process.GetProcesses()
  //|> Array.filter (fun p -> p.ProcessName = "msedge")
  //|> Array.map (fun p -> sprintf "taskkill /pid %d" p.Id)
  //|> Cmd.exec
  //|> ignore

  [| "\"C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe\" --log-net-log=\"C:\\logs\\export.json\" --net-log-capture-mode=Everything" |]
  |> Cmd.exec
  |> ignore

  printfn "test"
  System.Console.ReadLine() |> printfn "%s"

  0

#else
[<EntryPoint>]
let main args =
  ConsoleApp.Run<Command>(args)
  clear()
  printfn "This process has been completed."
  0
#endif