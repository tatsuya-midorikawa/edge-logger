namespace jp.dsi.logger.tools

open System
open System.Diagnostics
open System.Text
open System.IO

// TODO:
module Pwsh =
  [<Literal>]
  let private pwsh = "powershell"

  let private create'output'file'cmd filepath = $"out-file %s{get'fullpath filepath}"
  let private chain (cmds: seq<string>) = [| cmds |> String.concat " | " |]

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
  
  // get-hotofix
  [<Literal>]
  let private hotfix'cmd = "get-hotfix"
  let get'hotfix output'dir =
    // (1) C:\logs to C:\logs\yyyyMMdd_HHmmss
    let root'dir = Logger.get'root'dir output'dir
    // (2) C:\logs\yyyyMMdd_HHmmss to C:\logs\yyyyMMdd_HHmmss\env
    let output'dir = combine' [| root'dir; "env"; |]
    Logger.create'output'dir output'dir
    // (3) Output C:\logs\yyyyMMdd_HHmmss\env\hotfix.log
    let output'file'cmd = [| output'dir; "hotfix.log" |] |> (combine' >> create'output'file'cmd)
    [| hotfix'cmd; output'file'cmd |] |> (chain >> exec)




  // Disable Hardware-enforced Stack Protection
  let set'hesp enabled = 
    let enabled = if enabled then "-enable" else "-disable"
    $"Set-ProcessMitigation -name msedge.exe {enabled} UserShadowStack"

  let rem'hesp () =
    $"Set-ProcessMitigation -name msedge.exe -remove"
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
  let remove target = [| $"Remove-Item -Path \"%s{target}\" -Force" |]
