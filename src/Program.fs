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
let empty'task = System.Threading.Tasks.Task.Run<unit>(fun () -> ())

type Command () =
  inherit ConsoleAppBase ()
  [<RootCommand>]
  member __.root(
    [<Option("d", "Specify the output dir for log.");Optional;DefaultParameterValue(@"C:\logs")>] dir: string,
    [<Option("w", "Output Windows services info.");Optional;DefaultParameterValue(false)>] winsrv: bool,
    [<Option("e", "Output Edge policy info.");Optional;DefaultParameterValue(false)>] edge: bool,
    [<Option("i", "Output internet option info.");Optional;DefaultParameterValue(false)>] ie: bool,
    [<Option("u", "Output Logon user info.");Optional;DefaultParameterValue(false)>] usr: bool,
    [<Option("f", "Output full info.");Optional;DefaultParameterValue(false)>] full: bool) = 
    
    let winsrv'task =
      if full || winsrv then
        let path = Logger.winsrv'filepath dir
        Winsrv.getServices() |> Winsrv.collect |> toJson |> Logger.output path
      else
        empty'task

    let edge'task =
      if full || edge then
        let path = Logger.edge'filepath dir
        EdgePolicy.fetch () |> toJson |> Logger.output path
      else
        empty'task

    let ie'task =
      if full || ie then
        let path = Logger.ie'filepath dir
        IEReg.getIeRegistries () |> toJson |> Logger.output path
      else
        empty'task

    let usr'task =
      if full || usr then
        task {
          do! Cmd.dsregcmd |> Cmd.exec |> Logger.output (Logger.dsregcmd'filepath dir)
          do! Cmd.whoami |> Cmd.exec |> Logger.output (Logger.whoami'filepath dir)
          do! Cmd.cmdkey |> Cmd.exec |> Logger.output (Logger.cmdkey'filepath dir)
        }
      else
        empty'task

    task {
      do! winsrv'task
      do! edge'task
      do! ie'task
      do! usr'task
    }
    |> wait
  
#if DEBUG
[<EntryPoint>]
let main args =
  let dir = "./"
  //let winsrvTask =
  //  let path = Logger.winsrv'filepath dir
  //  Winsrv.getServices() |> Winsrv.collect |> toJson |> Logger.output path

  //let edgeTask =
  //  let path = Logger.edge'filepath dir
  //  EdgePolicy.fetch () |> toJson |> Logger.output path

  //let ieTask =
  //  let path = Logger.ie'filepath dir
  //  IEReg.getIeRegistries () |> toJson |> Logger.output path
    
  //let basicTask =
  //  let path = Logger.basic'filepath dir
  //  Basic.getInfo () |> toJson |> Logger.output path

  //task {
  //  do! winsrvTask
  //  do! edgeTask
  //  do! ieTask
  //  do! basicTask
  //}
  //|> wait

  //{ Sample = [| {| foo = "bar" |} |] }
  //|> toJson
  //|> printfn "%s"

  //let dict = Dictionary<string, obj>()
  //dict.Add ("foo", {| value = "aaa"; id = 0 |})
  //dict.Add ("bar", {| value = "bbb"; id = 1 |})
  
  //dict
  //|> toJson
  //|> printfn "%s"

  //EdgePolicy.fetch ()
  //|> toJson
  //|> printfn "%s"

  //EdgePolicy.getvalues "SOFTWARE\Policies\Microsoft\Edge"
  ////EdgePolicy.getlistvalues "SOFTWARE\Policies\Microsoft\Edge"
  //|> toJson
  //|> printfn "%s"

  //EdgePolicy.getlistvalues "SOFTWARE\Policies\Microsoft\Edge"
  //|> toJson
  //|> printfn "%s"

  //EdgePolicy.fetch () |> toJson |> printfn "%s"

  //Cmd.exec [| @"cd C:\logs"; "dir" |]
  //|> printfn "%s"
  //EdgePolicy.getlistvalues "SOFTWARE\Policies\Microsoft\Edge"
  //|> Seq.iter (printfn "%A")
  
  //Cmd.dsregcmd
  //|> Cmd.exec
  //|> printfn "%s"

  //task {
  //  let path = Logger.dsregcmd'filepath dir
  //  do! Cmd.dsregcmd
  //      |> Cmd.exec
  //      |> Logger.output path
  //}
  //|> wait

  //task {
  //  let path = Logger.whoami'filepath dir
  //  do! Cmd.whoami
  //      |> Cmd.exec
  //      |> Logger.output path
  //}
  //|> wait

  //task {
  //  let path = Logger.cmdkey'filepath dir
  //  do! Cmd.cmdkey
  //      |> Cmd.exec
  //      |> Logger.output path
  //}
  //|> wait

  0

#else
[<EntryPoint>]
let main args =
  ConsoleApp.Run<Command>(args)
  0
#endif