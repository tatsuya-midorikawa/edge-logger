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

  let exec' (cmds: seq<string>) =
    let pi = ProcessStartInfo (cmd, 
      // enable commnads input and reading of output
      UseShellExecute = false,
      RedirectStandardInput = true,
      RedirectStandardOutput = true,
      // hide console window
      CreateNoWindow = true)

    let p = proc.start pi
    for cmd in cmds do p.exec cmd
    p


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
  let private sysroot'dir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Windows)
  let private allusrprofile'dir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData)
  let private programdata'dir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData)
  let private localapp'dir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)

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

  // Microsoft Edge update logs
  let private update'log = [| allusrprofile'dir; "Microsoft"; "EdgeUpdate"; "Log"; |] |> combine'
  let private update'cmd dst = $"copy /y \"%s{update'log}\" \"%s{dst}\"";
  let private intune'log = [| programdata'dir; "Microsoft"; "IntuneManagementExtension"; "Logs"; "IntuneManagementExtension*.log"|] |> combine'
  let private intune'cmd dst = $"copy /y \"%s{intune'log}\" \"%s{dst}\"";
  let private webview2'update'log = [| localapp'dir; "Temp"; "MicrosoftEdgeUpdate.log" |] |> combine'
  let private webview2'update'cmd dst =
    let dst = combine' [| dst; "MicrosoftEdgeUpdate_wv2.log" |]
    $"copy /y \"%s{webview2'update'log}\" \"%s{dst}\"";
  let output'msedge'update root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\edge\update
    let output'dir = combine' [| root'dir; "edge"; "update"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\edge\IntuneManagementExtension*.log
    //     and    C:\logs\yyyyMMdd_HHmmss\edge\MicrosoftEdgeUpdate.log
    //     and    C:\logs\yyyyMMdd_HHmmss\edge\MicrosoftEdgeUpdate_wv2.log
    [| update'cmd output'dir; intune'cmd output'dir; webview2'update'cmd output'dir; |] |> Cmd.exec
    
  // Microsoft Edge install logs
  let private installer'log = [| sysroot'dir; "TEMP"; "msedge_installer.log" |] |> combine'
  let private installer'cmd dst = $"copy /y \"%s{installer'log}\" \"%s{dst}\""
  let private webview2'inst'log = [| localapp'dir; "Temp"; "msedge_installer.log" |] |> combine'
  let private webview2'inst'cmd dst =
    let dst = combine' [| dst; "msedge_installer_wv2.log" |]
    $"copy /y \"%s{webview2'inst'log}\" \"%s{dst}\"";
  let output'msedge'install root'dir =
    if not is'admin then raise (notsupportedexn "Supported only if you have administrative privileges.")
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\edge\install
    let output'dir = combine' [| root'dir; "edge"; "install"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\edge\msedge_installer.log
    //     and    C:\logs\yyyyMMdd_HHmmss\edge\msedge_installer_wv2.log
    [| installer'cmd output'dir; webview2'inst'cmd output'dir; |] |> Cmd.exec

  // Microsoft Edge crash reports
  let private crashrep'log = [| localapp'dir; "Microsoft"; "Edge"; "User Data"; "Crashpad"; "reports"; |] |> combine'
  let private crashrep'cmd dst = $"copy /y \"%s{crashrep'log}\" \"%s{dst}\"";
  let output'crashreport root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\edge\crash
    let output'dir = combine' [| root'dir; "edge"; "crash"; |]
    Logger.create'output'dir output'dir
    // (3) Output %USERPROFILE%\AppData\Local\Microsoft\Edge\User Data\Crashpad\reports
    [| crashrep'cmd output'dir; |] |> Cmd.exec

module IE =
  let private regs = [|
    @"SOFTWARE\Microsoft\Active Setup\Installed Components"
    @"SOFTWARE\Microsoft\Internet Explorer"
    @"SOFTWARE\Wow6432Node\Microsoft\Internet Explorer"
    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Browser Helper Objects"
    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders"
    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders"
    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Ext"
    @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Ext"
    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings"
    @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Internet Settings"
    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Ext"
    @"SOFTWARE\Policies\Microsoft\Internet Explorer"
    @"SOFTWARE\Policies\Microsoft\Windows\CurrentVersion\Internet Settings"
    @"SOFTWARE\Microsoft\Windows\CurrentVersion\WinTrust\Trust Providers\Software Publishing"
  |]
  let output'reg root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\ie
    let output'dir = combine' [| root'dir; "ie"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\ie\ie.log
    let output'path = combine' [| output'dir; "ie.log"; |]

    let hklm =
      regs
      |> Array.map (fun reg -> $"REG QUERY \"HKLM\\{reg}\" >> \"{output'path}\"")
      |> Cmd.exec
    let hkcu =
      regs
      |> Array.map (fun reg -> $"REG QUERY \"HKCU\\{reg}\" >> \"{output'path}\"")
      |> Cmd.exec

    hklm + hkcu

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
      raise (notsupportedexn "Supported only if you have administrative privileges.")

  // TODO:
  let rem'hesp () =
    [| $"Set-ProcessMitigation -name msedge.exe -remove" |]
    |> Pwsh.exec

module Tools =
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

  let remove target = [| $"Remove-Item -Path \"%s{target}\" -Recurse -Force" |] |> Pwsh.exec

  // psr /start /output {path} /maxsc 350 /gui 0
  let private psr'start'cmd path = [| $@"psr /start /output ""{path}"" /maxsc 350 /gui 0" |]
  let psr'start root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    Logger.create'output'dir root'dir
    // (2) Output C:\logs\yyyyMMdd_HHmmss\psr.zip
    [| root'dir; "psr.zip" |]
    |> (combine' >> psr'start'cmd >> Cmd.exec)

  // psr /stop
  let private psr'stop'cmd = [| "psr /stop" |]
  let psr'stop () = psr'stop'cmd |> Cmd.exec
  
  // msedge --log-net-log={path}\export.json --net-log-capture-mode=Everything --no-sandbox
  let private netexport'cmd path = [| $@"""C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"" --log-net-log=""{path}"" --net-log-capture-mode=Everything --no-sandbox" |]
  let netexport root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    Logger.create'output'dir root'dir
    // (2) Output C:\logs\yyyyMMdd_HHmmss\export.json
    [| root'dir; "export.json" |]
    |> (combine' >> netexport'cmd >> Cmd.exec)
  
  // netsh trace start scenario=InternetClient_dbg tracefile="{path}" capture=yes maxSize=300
  let private netsh'start'cmd path = $"netsh trace start scenario=InternetClient_dbg tracefile=\"{path}\" capture=yes maxSize=500"
  let netsh'start root'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir root'dir
    Logger.create'output'dir root'dir
    // (2) Start logging
    let start'cmd = [| root'dir; "protcol.etl" |] |> (combine' >> netsh'start'cmd)
    [| start'cmd;|] |> Cmd.exec'

  // netsh trace stop
  let private netsh'stop'cmd = $"netsh trace stop"
  let netsh'stop (p: proc) =
    // Output C:\logs\yyyyMMdd_HHmmss\protcol.etl
    p.exec netsh'stop'cmd