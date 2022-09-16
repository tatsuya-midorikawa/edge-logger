namespace jp.dsi.logger.tools

open System
open System.Diagnostics
open System.Text
open System.IO

module Pwsh =
  [<Literal>]
  let private pwsh = "powershell"

  let internal create'output'file'cmd filepath = $"out-file %s{get'fullpath filepath}"
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

  // TEST
  let exec'' (cmds: seq<string>) =
    let pi = ProcessStartInfo (pwsh, 
      // enable commnads input and reading of output
      UseShellExecute = false,
      RedirectStandardInput = true,
      RedirectStandardOutput = true,
      // hide console window
      CreateNoWindow = true)

    use p = Process.Start pi
    let stdout = StringBuilder()
    // wip
    p.OutputDataReceived.Add (fun e ->
      if e.Data <> null then
        let index = e.Data.IndexOf("> ")
        if 0 <= index then stdout.AppendLine(e.Data.Substring(index)) |> ignore)
    p.BeginOutputReadLine()
    for cmd in cmds do 
      p.StandardInput.WriteLine cmd
    p.StandardInput.WriteLine "exit"
    p.WaitForExit()
    stdout.ToString()

  let exec' (callback: string -> unit) (cmds: seq<string>) =
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
      callback cmd
    p.StandardInput.WriteLine "exit"
    p.WaitForExit()
    stdout.ToString()

module Cmd =
  let private cmd = Environment.GetEnvironmentVariable "ComSpec"

  let internal create'output'file'cmd filepath = $"%s{get'fullpath filepath}"
  let internal chain (cmds: seq<string>) = [| cmds |> String.concat " >> " |]

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

  // Disable Hardware-enforced Stack Protection
  type Hesp =
    | UserShadowStack of bool
    | UserShadowStackStrictMode of bool
    | AuditUserShadowStack of bool
    
  // TODO:
  // そもそも、msedge.exe が Exploit protection に登録されていない場合、
  // 設定値を取得する、以下のコマンドが null を返すため、そこの処理も必要。
  // > get-processMitigation -name msedge.exe | select-object "UserShadowStack"
  //
  // ただ、PowerShell 単体で考える場合においては、null でメソッド チェーンを繋いでも、
  // 例外にはならず、null が返ってくるので、それで判断してもよいやもしれない。
  let get'hesp () =
    // TODO
    // return ON or OFF or NOTSET: 以下の 3 つ状態を取得し、処理終了後に戻す処理が必要
    //"(get-processMitigation -name msedge.exe | select-object \"UserShadowStack\").UserShadowStack.UserShadowStack"
    //"(get-processMitigation -name msedge.exe | select-object \"UserShadowStack\").UserShadowStack.UserShadowStackStrictMode"
    //"(get-processMitigation -name msedge.exe | select-object \"UserShadowStack\").UserShadowStack.AuditUserShadowStack"

    // TODO
    [| "(get-processMitigation -name msedge.exe | select-object \"UserShadowStack\").UserShadowStack.UserShadowStack" |]
    |> Pwsh.exec''
    

  let set'hesp enabled = 
    if is'admin
    then
      let enabled = if enabled then "-enable" else "-disable"
      [| $"Set-ProcessMitigation -name msedge.exe {enabled} UserShadowStack" |]
      |> Pwsh.exec
    else
      raise (NotSupportedException "Supported only if you have administrative privileges.")

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
