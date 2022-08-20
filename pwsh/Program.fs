open System.IO.Pipes
open System.Diagnostics

let pipe'name =
  let pid = Process.GetCurrentProcess().Id
  $"jp.dsi.pwsh.{pid}"

[<EntryPoint>]
let main args =
  use pipe'srv = new NamedPipeServerStream(pipe'name, PipeDirection.InOut)
  pipe'srv.WaitForConnection()

  let stream = StreamString(pipe'srv)

  0
