open System.IO.Pipes
open System.Diagnostics

[<Literal>]
let pipe'name = $"jp.dsi.pwsh.15632680-eedf-418a-aa2a-334dbb121d38"

[<EntryPoint>]
let main args =
  use pipe'srv = new NamedPipeServerStream(pipe'name, PipeDirection.InOut)
  pipe'srv.WaitForConnection()

  let stream = StreamString(pipe'srv)

  0
