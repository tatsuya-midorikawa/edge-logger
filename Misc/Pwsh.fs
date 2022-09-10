namespace jp.dsi.logger.misc

open System
open System.Diagnostics
open System.Text
open System.IO

// TODO:
module Pwsh =
  [<Literal>]
  let pwsh = "powershell"
  [<Literal>]
  let hotfix = "get-hotfix"

  let output'file filepath = $"out-file %s{get'fpath filepath}"
  let chain (cmds: seq<string>) = cmds |> String.concat " | "

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

  //// TODO:
  //// 1. Replaced by a process using the pwsh.
  //let run'as (cmd: string) =
  //  let pi = ProcessStartInfo (pwsh,
  //    Arguments = cmd,
  //    UseShellExecute = true,
  //    // hide console window
  //    CreateNoWindow = true,
  //    // run as adminstrator
  //    Verb = "runas")
  
  //  use p = Process.Start pi
  //  p.WaitForExit()
  //  p.Close()

  //let exec (cmds: seq<string>) =
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
  //  p.StandardInput.WriteLine "exit"
  //  p.WaitForExit()
  //  stdout.ToString()