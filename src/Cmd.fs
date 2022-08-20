module Cmd

open System
open System.Diagnostics
open System.Text
open System.IO

let cmd = Environment.GetEnvironmentVariable "ComSpec"
let dsregcmd = [| "dsregcmd /status" |]
let whoami = [| "whoami" |]
let cmdkey = [| "cmdkey /list" |]
let schtasks = [| "schtasks /query /V /FO CSV" |]
let netexport dir = 
  let nxpath = Path.Combine (dir, "export.json")
  [| $@"""C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"" --log-net-log=""{nxpath}"" --net-log-capture-mode=Everything --no-sandbox" |]
let psr'start dir = 
  let path = Path.Combine (dir, "psr.zip")
  [| $@"psr /start /output ""{path}"" /maxsc 999 /gui 0" |]
let psr'stop = [| $@"psr /stop" |]

let run'as (admin: bool) (cmds: seq<string>)  =
  let pi = ProcessStartInfo (cmd, 
    // enable commnads input and reading of output
    UseShellExecute = false,
    RedirectStandardInput = true,
    RedirectStandardOutput = true,
    // hide console window
    CreateNoWindow = true,
    // run as adminstrator
    Verb = if admin then "runas" else "")

  use p = Process.Start pi
  let stdout = StringBuilder()
  p.OutputDataReceived.Add (fun e -> if e.Data <> null then stdout.AppendLine(e.Data) |> ignore)
  p.BeginOutputReadLine()
  for cmd in cmds do 
    p.StandardInput.WriteLine cmd
  p.StandardInput.WriteLine "exit"
  p.WaitForExit()
  stdout.ToString()

let exec (cmds: seq<string>) = cmds |> run'as false