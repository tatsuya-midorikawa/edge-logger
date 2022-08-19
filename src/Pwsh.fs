module Pwsh

open System
open System.Diagnostics
open System.Text
open System.IO

let pwsh = "powershell"
let hotfix = [| "get-hotfix" |]
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
let remove target = [| $"Remove-Item -Path \"{target}\" -Force" |]


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