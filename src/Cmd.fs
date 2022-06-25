module Cmd

open System
open System.Diagnostics

let inline exec (cmds: seq<string>) =
  let pi = ProcessStartInfo ("cmd.exe")
  pi.UseShellExecute <- false
  pi.RedirectStandardInput <- true
  pi.CreateNoWindow <- true

  use p = Process.Start pi
  p.StandardInput.AutoFlush <- true

  for cmd in cmds do
    p.StandardInput.WriteLine cmd
  p.StandardInput.WriteLine "exit"
  p.WaitForExit()
  