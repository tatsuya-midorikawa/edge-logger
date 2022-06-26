module Cmd

open System
open System.Diagnostics

let cmd = Environment.GetEnvironmentVariable "ComSpec"
let dsregcmd = [| "dsregcmd /status" |]
let whoami = [| "whoami" |]
let cmdkey = [| "cmdkey /list" |]

let inline exec (cmds: seq<string>) =
  let exec (cmd: string) (p: Process) = p.StandardInput.WriteLine cmd
  let pi = ProcessStartInfo (cmd, 
    // enable commnads input and reading of output
    UseShellExecute = false,
    RedirectStandardInput = true,
    RedirectStandardOutput = true,
    // hide console window
    CreateNoWindow = true)

  use p = Process.Start pi
  for cmd in cmds do p |> exec cmd
  p |> exec "exit"
  p.WaitForExit()
  p.StandardOutput.ReadToEnd()