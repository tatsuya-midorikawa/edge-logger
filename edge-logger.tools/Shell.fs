namespace jp.dsi.logger.tools

open System
open System.Diagnostics
open System.Text
open System.IO

module Pwsh =
  [<Literal>]
  let private pwsh = "powershell"

  let internal create'output'file'cmd filepath = $"out-file \"%s{get'fullpath filepath}\""
  let internal chain (cmds: seq<string>) = [| cmds |> String.concat " | " |]

  let exec (cmds: seq<string>) =
    let pi = ProcessStartInfo (pwsh, 
      // enable commnads input and reading of output
      UseShellExecute = false,
      RedirectStandardInput = true,
      RedirectStandardOutput = true,
      // hide console window
      CreateNoWindow = true)

    use p = Process.Start pi
    let stdout = StringBuilder()
    p.OutputDataReceived.Add (fun e -> if e.Data <> null then stdout.AppendLine(e.Data) |> ignore)
    p.BeginOutputReadLine()
    for cmd in cmds do 
      p.StandardInput.WriteLine cmd
    p.StandardInput.WriteLine "exit"
    p.WaitForExit()
    stdout.ToString()

  // 
  let exec' (cmds: seq<string>) =
    let pi = ProcessStartInfo (pwsh, 
      // enable commnads input and reading of output
      UseShellExecute = false,
      RedirectStandardInput = true,
      RedirectStandardOutput = true,
      // hide console window
      CreateNoWindow = true)

    use p = Process.Start pi
    let stdout = ResizeArray<string>()
    p.OutputDataReceived.Add (fun e ->
      if not (String.IsNullOrWhiteSpace e.Data) then
        let index = e.Data.IndexOf("> ")
        if 0 <= index then
          stdout.Add(e.Data.Substring (index + 2)) else stdout.Add(e.Data))
    p.BeginOutputReadLine()
    for cmd in cmds do 
      p.StandardInput.WriteLine cmd
    p.StandardInput.WriteLine "exit"
    p.WaitForExit()
    Array.ofSeq stdout

  //let exec'with (callback: string -> unit) (cmds: seq<string>) =
  //  let pi = ProcessStartInfo (pwsh, 
  //    // enable commnads input and reading of output
  //    UseShellExecute = false,
  //    RedirectStandardInput = true,
  //    RedirectStandardOutput = true,
  //    // hide console window
  //    CreateNoWindow = true)

  //  use p = Process.Start pi
  //  let stdout = StringBuilder()
  //  p.OutputDataReceived.Add (fun e -> if e.Data <> null then stdout.AppendLine(e.Data) |> ignore)
  //  p.BeginOutputReadLine()
  //  for cmd in cmds do 
  //    p.StandardInput.WriteLine cmd
  //    callback cmd
  //  p.StandardInput.WriteLine "exit"
  //  p.WaitForExit()
  //  stdout.ToString()

module Cmd =
  let private cmd = Environment.GetEnvironmentVariable "ComSpec"

  let internal create'output'file'cmd filepath = $"%s{get'fullpath filepath}"
  let internal chain (cmds: seq<string>) = [| cmds |> String.concat " >> " |]
  let internal chains (cmds: seq<string[]>) = cmds |> Seq.map (String.concat " >> ")

  let run'as (wait'for'exit: bool) (cmd: string)  =
    let pi = ProcessStartInfo (cmd, 
      UseShellExecute = true,
      // hide console window
      CreateNoWindow = true,
      // run as adminstrator
      Verb = "runas")
  
    if wait'for'exit
    then
      use p = Process.Start pi
      p.WaitForExit()
    else
      Process.Start pi |> ignore

  let exec (cmds: seq<string>) =
    let pi = ProcessStartInfo (cmd, 
      // enable commnads input and reading of output
      UseShellExecute = false,
      RedirectStandardInput = true,
      RedirectStandardOutput = true,
      // hide console window
      CreateNoWindow = true)
  
    use p = Process.Start pi
    let stdout = StringBuilder()
    p.OutputDataReceived.Add (fun e -> if e.Data <> null then stdout.AppendLine(e.Data) |> ignore)
    p.BeginOutputReadLine()
    for cmd in cmds do 
      p.StandardInput.WriteLine cmd
    p.StandardInput.WriteLine "exit"
    p.WaitForExit()
    stdout.ToString()

  let exec' (callback: string -> unit) (cmds: seq<string>) =
    let pi = ProcessStartInfo (cmd, 
      // enable commnads input and reading of output
      UseShellExecute = false,
      RedirectStandardInput = true,
      RedirectStandardOutput = true,
      // hide console window
      CreateNoWindow = true)
  
    use p = Process.Start pi
    let stdout = StringBuilder()
    p.OutputDataReceived.Add (fun e -> if e.Data <> null then stdout.AppendLine(e.Data) |> ignore)
    p.BeginOutputReadLine()
    for cmd in cmds do 
      p.StandardInput.WriteLine cmd
      callback cmd
    p.StandardInput.WriteLine "exit"
    p.WaitForExit()
    stdout.ToString()

module Env =
  // get-hotofix
  [<Literal>]
  let private hotfix'cmd = "get-hotfix"
  let output'hotfix root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\env
    let output'dir = combine' [| root'dir; "env"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\env\hotfix.log
    let output'file'cmd = [| output'dir; "hotfix.log" |] |> (combine' >> Pwsh.create'output'file'cmd)
    [| hotfix'cmd; output'file'cmd |] |> (Pwsh.chain >> Pwsh.exec)

  // systeminfo
  [<Literal>]
  let private systeminfo'cmd = "systeminfo"
  let output'systeminfo root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\env
    let output'dir = combine' [| root'dir; "env"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\env\systeminfo.log
    let output'file'cmd = [| output'dir; "systeminfo.log" |] |> (combine' >> Cmd.create'output'file'cmd)
    [| systeminfo'cmd; output'file'cmd |] |> (Cmd.chain >> Cmd.exec)

  // wmic qfe list
  [<Literal>]
  let private qfelist'cmd = "wmic qfe list"
  let output'qfelist root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\env
    let output'dir = combine' [| root'dir; "env"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\env\qfelist.log
    let output'file'cmd = [| output'dir; "qfelist.log" |] |> (combine' >> Cmd.create'output'file'cmd)
    [| qfelist'cmd; output'file'cmd |] |> (Cmd.chain >> Cmd.exec)

  // dsregcmd /status
  [<Literal>]
  let private dsregcmd'cmd = "dsregcmd /status"
  let output'dsregcmd root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\env
    let output'dir = combine' [| root'dir; "env"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\env\dsregcmd.log
    let output'file'cmd = [| output'dir; "dsregcmd.log" |] |> (combine' >> Cmd.create'output'file'cmd)
    [| dsregcmd'cmd; output'file'cmd |] |> (Cmd.chain >> Cmd.exec)
    
  // whoami
  [<Literal>]
  let private whoami'cmd = "whoami"
  let output'whoami root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\env
    let output'dir = combine' [| root'dir; "env"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\env\whoami.log
    let output'file'cmd = [| output'dir; "whoami.log" |] |> (combine' >> Cmd.create'output'file'cmd)
    [| whoami'cmd; output'file'cmd |] |> (Cmd.chain >> Cmd.exec)
    
  // cmdkey /list
  [<Literal>]
  let private cmdkey'cmd = "cmdkey /list"
  let output'cmdkey root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\env
    let output'dir = combine' [| root'dir; "env"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\env\cmdkey.log
    let output'file'cmd = [| output'dir; "cmdkey.log" |] |> (combine' >> Cmd.create'output'file'cmd)
    [| cmdkey'cmd; output'file'cmd |] |> (Cmd.chain >> Cmd.exec)

  // get-appxpackage -name {package'name}
  let private appxpackage'cmd pack'name = $"get-appxpackage -name %s{pack'name}"
  let output'appxpackage root'dir pack'name =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\env
    let output'dir = combine' [| root'dir; "env"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\env\cmdkey.log
    let output'file'cmd = [| output'dir; $"%s{pack'name}.log" |] |> (combine' >> Pwsh.create'output'file'cmd)
    [| appxpackage'cmd pack'name; output'file'cmd |] |> (Pwsh.chain >> Pwsh.exec)

  // get-appxpackage | select-object name, version
  [<Literal>]
  let private appxpackage'list'cmd = $"get-appxpackage | select-object name, version"
  let output'appxpackage'list root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\env
    let output'dir = combine' [| root'dir; "env"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\env\appxpackage list.log
    let output'file'cmd = [| output'dir; $"appxpackage list.log" |] |> (combine' >> Pwsh.create'output'file'cmd)
    [| appxpackage'list'cmd; output'file'cmd |] |> (Pwsh.chain >> Pwsh.exec)

module Winsrv =
  // Get-WmiObject win32_service
  [<Literal>]
  let private win32service'cmd = $"Get-WmiObject win32_service"
  let output'win32service'list root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\winsrv
    let output'dir = combine' [| root'dir; "winsrv"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\winsrv\win32service list.log
    let output'file'cmd = [| output'dir; $"win32service list.log" |] |> (combine' >> Pwsh.create'output'file'cmd)
    [| win32service'cmd; output'file'cmd |] |> (Pwsh.chain >> Pwsh.exec)

   // schtasks /query /V /FO LIST
  [<Literal>]
  let private schtasks'cmd = "schtasks /query /V /FO LIST"
  let output'schtasks root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\winsrv
    let output'dir = combine' [| root'dir; "winsrv"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\winsrv\schtasks.log
    let output'file'cmd = [| output'dir; "schtasks.log" |] |> (combine' >> Cmd.create'output'file'cmd)
    [| schtasks'cmd; output'file'cmd |] |> (Cmd.chain >> Cmd.exec)

module Edge =
  // REG QUERY "{HKLM|HKCU}\SOFTWARE\Policies\Microsoft\Edge"
  [<Literal>]
  let private policy'hklm'cmd = $"REG QUERY \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Edge\""
  [<Literal>]
  let private policy'hkcu'cmd = $"REG QUERY \"HKCU\\SOFTWARE\\Policies\\Microsoft\\Edge\""
  let output'policy root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\edge
    let output'dir = combine' [| root'dir; "edge"; "policy"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\edge\policy\edge.log
    let output'path = combine' [| output'dir; "edge.log"; |]
    [| [| policy'hklm'cmd; $"\"{output'path}\""; |]; [| policy'hkcu'cmd; $"\"{output'path}\""; |]; |]
    |> (Cmd.chains >> Cmd.exec)

  // REG QUERY "{HKLM|HKCU}\SOFTWARE\Policies\Microsoft\EdgeUpdate"
  [<Literal>]
  let private update'hklm'cmd = $"REG QUERY \"HKLM\\SOFTWARE\\Policies\\Microsoft\\EdgeUpdate\""
  [<Literal>]
  let private update'hkcu'cmd = $"REG QUERY \"HKCU\\SOFTWARE\\Policies\\Microsoft\\EdgeUpdate\""
  let output'update'policy root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\edge
    let output'dir = combine' [| root'dir; "edge"; "policy"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\edge\policy\update.log
    let output'path = combine' [| output'dir; "update.log"; |]
    [| [| update'hklm'cmd; $"\"{output'path}\""; |]; [| update'hkcu'cmd; $"\"{output'path}\"";|] |] |> (Cmd.chains >> Cmd.exec)

  // REG QUERY "{HKLM|HKCU}\SOFTWARE\Microsoft\Edge"
  [<Literal>]
  let private edge'hklm'cmd = $"REG QUERY \"HKLM\\SOFTWARE\\Microsoft\\Edge\""
  [<Literal>]
  let private edge'hkcu'cmd = $"REG QUERY \"HKCU\\SOFTWARE\\Microsoft\\Edge\""
  [<Literal>]
  let private wow6432'hklm'cmd = $"REG QUERY \"HKLM\\SOFTWARE\\WOW6432Node\\Microsoft\\Edge\""
  [<Literal>]
  let private wow6432'hkcu'cmd = $"REG QUERY \"HKCU\\SOFTWARE\\WOW6432Node\\Microsoft\\Edge\""
  let output'ext'policy root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\edge
    let output'dir = combine' [| root'dir; "edge"; "policy"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\edge\policy\ext.log
    let output'path = combine' [| output'dir; "ext.log"; |]
    [| [| edge'hklm'cmd; $"\"{output'path}\""; |]; [| edge'hkcu'cmd; $"\"{output'path}\""; |];
       [| wow6432'hklm'cmd; $"\"{output'path}\""; |]; [| wow6432'hkcu'cmd; $"\"{output'path}\""; |]; |]
    |> (Cmd.chains >> Cmd.exec)

  // (Get-Command "C:\Program Files (x86)\Microsoft\edge\Application\msedge.exe").Version
  let private ver'cmd = 
    let path = 
      [| 
        System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86)
        @"Microsoft\Edge\Application\msedge.exe" 
      |] |> System.IO.Path.Combine
    $"(Get-Command \"{path}\").Version"
  [<Literal>]
  let private winver'cmd = "ver"
  let output'version root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\edge
    let output'dir = combine' [| root'dir; "edge"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\edge\version.log
    let output'path = combine' [| output'dir; "winver.log"; |]
    let output'file'cmd = [| output'dir; "version.log"; |] |> (combine' >> Pwsh.create'output'file'cmd)
    let r1 = [| ver'cmd; output'file'cmd; |] |> (Pwsh.chain >> Pwsh.exec)
    let r2 = [| winver'cmd; $"\"{output'path}\"" |] |> (Cmd.chain >> Cmd.exec)
    r1 + r2

// WIP:
module ProcessMitigation =
  type State = On | Off | Notset | True | False
  module private State =
    let parse (cmd: string) =  match cmd.ToLower() with "on" -> On | "off" -> Off | "true" -> True | "false" -> False | _ -> Notset

  type Hesp = {
    UserShadowStack : State
    UserShadowStackStrictMode : State
    AuditUserShadowStack : State
    Override : State
  }
  with static member empty = { UserShadowStack = State.Notset;  UserShadowStackStrictMode = State.Off; AuditUserShadowStack = State.Notset; Override = State.False}
    
  // Get Hardware-enforced Stack Protection
  let get'hesp () =
    let cmd = "Get-ProcessMitigation -name msedge.exe"
    let lines = [| cmd |] |> Pwsh.exec'
    let cmd'idx = lines |> Array.findIndex ((=) cmd)
    let exit'idx = lines |> Array.findIndex ((=) "exit")
    // If there is no cmd result, return None.
    if exit'idx - cmd'idx = 1
    then None
    else 
      let start'idx = 
        lines 
        |> Array.findIndex (fun line -> line = "User Shadow Stack:")
        |> (+) 1

      let end'idx =
        let len = lines.Length
        let rec f(i: int) =
          if len < i
          then raise (exn "Index out of range.")
          else if lines[i].StartsWith(" ") then f (i + 1) else i - 1
        f start'idx

      let xs = 
        lines[start'idx..end'idx]
        |> Array.filter (fun x -> x.Contains(":"))
        |> Array.map (fun x -> 
          let x' = x.Replace(" ", "").Split(':')
          {| key = x'[0]; value = x'[1] |})

      let mutable hesp = Hesp.empty
      for x in xs do
        match x.key with
        | "UserShadowStack" -> hesp <- { hesp with UserShadowStack = State.parse x.value }
        | "UserShadowStackStrictMode" -> hesp <- { hesp with UserShadowStackStrictMode = State.parse x.value }
        | "AuditUserShadowStack" -> hesp <- { hesp with AuditUserShadowStack = State.parse x.value }
        | "OverrideUserShadowStack" -> hesp <- { hesp with Override = State.parse x.value }
        | _ -> ()
          
      //if hesp = Hesp.empty then None else Some hesp
      Some hesp

  // TODO:
  let set'hesp enabled = 
    if is'admin
    then
      let enabled = if enabled then "-enable" else "-disable"
      [| $"Set-ProcessMitigation -name msedge.exe {enabled} UserShadowStack" |]
      |> Pwsh.exec
    else
      raise (NotSupportedException "Supported only if you have administrative privileges.")

  // TODO:
  let rem'hesp () =
    [| $"Set-ProcessMitigation -name msedge.exe -remove" |]
    |> Pwsh.exec

module Tools =
  // TODO:
  let inline unzip (zip: string) =
    if Path.GetExtension zip = ".zip" 
    then
      let dir = Path.GetDirectoryName zip
      [| 
        $"Expand-Archive -Force \"{zip}\" \"{dir}\""
        $"Remove-Item -Path \"{zip}\" -Force"
      |]
     else
      [||]

  // TODO:
  let remove target = [| $"Remove-Item -Path \"%s{target}\" -Force" |]
