open jp.dsi.logger.tools

open System.Diagnostics
open System
open System.Linq
open ConsoleAppFramework
open System.Runtime.InteropServices

let inline exists'proc process'name = 0 < Process.GetProcessesByName(process'name).Length

let private need'admin'cmds = [ "-e"; "--edge"; "-egi"; "--edgeinst"; "-nsh"; "--netsh"; "-nw"; "--network"; "-f"; "--full"; ]
let need'admin (args: string[]) =
  if is'admin
  then false
  else
    let rec judge (cmds: list<string>) =
      match cmds with
      | h::t -> if args.Any(fun x -> x = h) then true else judge t
      | _ -> false

    judge need'admin'cmds

let private must'be'terminated'cmds = [ "-nx"; "--netexport"; "-nsh"; "--netsh"; "-nw"; "--network"; ]
let must'be'terminated (args: string[]) =
  let rec judge (cmds: list<string>) =
    match cmds with
    | h::t -> if args.Any(fun x -> x = h) then true else judge t
    | _ -> false
  judge must'be'terminated'cmds && exists'proc "msedge"

type Command () =
  inherit ConsoleAppBase ()
  [<RootCommand>]
  member __.root(
    [<Option("o", "Specify the output dir for log.");Optional;DefaultParameterValue(@"C:\logs")>] dir: string,
    [<Option("wsrv", "Output Windows services info.");Optional;DefaultParameterValue(false)>] winsrv: bool,
    [<Option("e", "Output all Microsoft Edge's logs.");Optional;DefaultParameterValue(false)>] edge: bool,
    [<Option("egp", "Output Microsoft Edge policy info.");Optional;DefaultParameterValue(false)>] edgepolicy: bool,
    [<Option("egc", "Output Microsoft Edge crash reports.");Optional;DefaultParameterValue(false)>] edgecrash: bool,
    [<Option("egi", "Output Microsoft Edge installation log.");Optional;DefaultParameterValue(false)>] edgeinst: bool,
    [<Option("egu", "Output Microsoft Edge update log.");Optional;DefaultParameterValue(false)>] edgeupd: bool,
    [<Option("inet", "Output internet option info.");Optional;DefaultParameterValue(false)>] inetopt: bool,
    [<Option("env", "Output Logon user info and enviroment info.");Optional;DefaultParameterValue(false)>] env: bool,
    [<Option("nx", "Collecting net-export and psr logs.");Optional;DefaultParameterValue(false)>] netexport: bool,
    [<Option("nsh", "Collecting netsh and psr logs.");Optional;DefaultParameterValue(false)>] netsh: bool,
    [<Option("nw", "Collecting netsh, net-export and psr logs.");Optional;DefaultParameterValue(false)>] network: bool,
    [<Option("psr", "Collecting psr logs.");Optional;DefaultParameterValue(false)>] psr: bool,
    [<Option("f", "Output full info.");Optional;DefaultParameterValue(false)>] full: bool) = 
    
    let log = Logger.log dir

    let winsrv'task =
      if full || winsrv
      then 
        task {
        // Windows Service information
          try do! Winsrv.output'win32service'list dir |> log with e -> do! $"<winsrv>: {e.Message}" |> log
          // schtasks /query /V /FO CSV
          try do! Winsrv.output'schtasks dir |> log with e -> do! $"<schtasks>: {e.Message}" |> log
        }
      else empty'task

    let edgepolicy'task =
      if full || edge || edgepolicy
      then 
        task {
          // msedge version information
          try do! Edge.output'version dir |> log with e -> do! $"<msedge version>: {e.Message}" |> log
          // msedge and webview2 policy registry
          try do! Edge.output'policy dir |> log with e -> do! $"<msedge policy>: {e.Message}" |> log
          // msedge update policy registry
          try do! Edge.output'update'policy dir |> log with e -> do! $"<msedge update policy>: {e.Message}" |> log
          // msedge extensions policy registry
          try do! Edge.output'ext'policy dir |> log with e -> do! $"<msedge extensions policy>: {e.Message}" |> log
        }
      else empty'task

    let edgeupd'task =
      if full || edge || edgeupd
      then 
        task {
          // msedge update logs
          try do! Edge.output'msedge'update dir |> log with e -> do! $"<msedge update logs>: {e.Message}" |> log
        }
      else empty'task

    let edgeinst'task =
      if full || edge || edgeinst
      then 
        task {
          // msedge installation logs
          try do! Edge.output'msedge'install dir |> log with e -> do! $"<msedge installation logs>: {e.Message}" |> log
        }
      else empty'task

    let edgecrash'task =
      if full || edge || edgecrash
      then 
        task {
          // msedge crash report logs
          try do! Edge.output'crashreport dir |> log with e -> do! $"<msedge crash report logs>: {e.Message}" |> log
        }
      else empty'task

    let inetopt'task =
      if full || inetopt
      then 
        task {
          // internet options registries
          try do! IE.output'reg dir |> log with e -> do! $"<internet options>: {e.Message}" |> log
          // IEDigest logs
          try do! IEDigest.output dir |> log with e -> do! $"<IEDigest>: {e.Message}" |> log
        }
      else empty'task

    let env'task =
      if full || env
      then 
        task {
          // get-appxpackage | select-object name, version
          try do! Env.output'appxpackage'list dir |> log with e -> do! $"<get-appxpackage>: {e.Message}" |> log
          // cmdkey /list
          try do! Env.output'cmdkey dir |> log with e -> do! $"<cmdkey>: {e.Message}" |> log
          // dsregcmd /status
          try do! Env.output'dsregcmd dir |> log with e -> do! $"<dsregcmd>: {e.Message}" |> log
          // get-hotofix
          try do! Env.output'hotfix dir |> log with e -> do! $"<get-hotofix>: {e.Message}" |> log
          // wmic qfe list
          try do! Env.output'qfelist dir |> log with e -> do! $"<wmic qfe list>: {e.Message}" |> log
          // systeminfo
          try do! Env.output'systeminfo dir |> log with e -> do! $"<systeminfo>: {e.Message}" |> log
          // whoami
          try do! Env.output'whoami dir |> log with e -> do! $"<whoami>: {e.Message}" |> log
        }
      else empty'task

    // Collection of various configuration information.
    task {
      try do! winsrv'task with e -> $"<winsrv'task>: {e.Message}" |> (log >> wait)
      try do! edgepolicy'task with e -> $"<edgepolicy'task>: {e.Message}" |> (log >> wait)
      try do! edgeupd'task with e -> $"<edgeupd'task>: {e.Message}" |> (log >> wait)
      try do! edgeinst'task with e -> $"<edgeinst'task>: {e.Message}" |> (log >> wait)
      try do! edgecrash'task with e -> $"<edgecrash'task>: {e.Message}" |> (log >> wait)
      try do! inetopt'task with e -> $"<inetopt'task>: {e.Message}" |> (log >> wait)
      try do! env'task with e -> $"<env'task>: {e.Message}" |> (log >> wait)
    }
    |> wait
    
    // Determine if logging is possible
    if netexport && exists'proc "msedge" then msgbox'show "To log net-export, msedge.exe must be terminated and run this app again."
    else
      Console.CancelKeyPress.Add(fun _ -> if psr || netexport then Tools.psr'stop () |> ignore)

      // start PSR
      if network || netsh || netexport || psr then 
        try Tools.psr'start dir with e -> $"<psr start>: {e.Message}" 
        |> (log >> wait)
      
      // Collecting net-export logs.
      if network || netexport then
        try Tools.netexport dir with e -> $"<net-export>: {e.Message}"
        |> (log >> wait)
       
      // Collecting netsh logs.
      let nsh'proc =
        if network || netsh 
        then try Tools.netsh'start dir with e -> $"<netsh>: {e.Message}" |> (log >> wait); proc.empty
        else proc.empty

      // stop PSR and netsh
      if network || netsh || netexport || psr then 
        wait'for'input ()
        try Tools.netsh'stop nsh'proc with e -> $"<netsh>: {e.Message}"|> (log >> wait) 
        (try nsh'proc.close() with e -> $"<netsh>: {e.Message}") |> (log >> wait)
        (try Tools.psr'stop () with e -> $"<psr stop>: {e.Message}") |> (log >> wait)

      Cmd.exec [| $"explorer %s{dir}" |] |> ignore
    ()
  
  [<Command("stop")>]
  member __.stop ([<Option("p", "parameters");>] parameters: string[]) =
    let need'admin'cmds = [ "netsh" ]
    let need'admin (args: string[]) =
      if is'admin
      then false
      else
        let rec judge (cmds: list<string>) =
          match cmds with
          | h::t -> if args.Any(fun x -> x = h) then true else judge t
          | _ -> false
        judge need'admin'cmds
    
    if need'admin parameters 
    then relaunch'as'admin'if'user __.Context.Arguments |> ignore
    else
      if parameters.Any(fun p -> p = "netsh") then
        try Tools.netsh'force'stop () |> ignore with e -> printfn $"<netsh force stop>: {e.Message}"
      if parameters.Any(fun p -> p = "psr") then
        try Tools.psr'stop () |> ignore with e -> printfn $"psr stop>: {e.Message}"

#if DEBUG
[<EntryPoint>]
let main args =
  //let dir = "C:\\logs" |> Path.GetFullPath
  //args |> String.concat ", " |> printfn "%s"

  //if must'be'terminated args
  //then msgbox'show "To log net-export/netsh, msedge.exe must be terminated and run this app again."

  //if need'admin args 
  //then relaunch'as'admin'if'user args |> ignore
  //else System.Console.ReadKey() |> ignore
  if must'be'terminated args
  then msgbox'show "To log net-export/netsh, msedge.exe must be terminated and run this app again."
  else
    if need'admin args 
    then
      match relaunch'as'admin'if'user args with
      | Ok _ -> ()
      // TODO
      | Error msg -> raise (notimplexn "")
    else ConsoleApp.Run<Command>(args)
    //clear()
    printfn "This process has been completed."
  0

#else
[<EntryPoint>]
let main args =
  if must'be'terminated args
  then msgbox'show "To log net-export/netsh, msedge.exe must be terminated and run this app again."
  else
    if need'admin args 
    then relaunch'as'admin'if'user args |> ignore
    else ConsoleApp.Run<Command>(args)
    clear()
    printfn "This process has been completed."
  0
#endif