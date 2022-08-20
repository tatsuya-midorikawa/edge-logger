open jp.dsi.logger.misc
open System.IO.Pipes
open System.Diagnostics
open System.Threading.Tasks

[<Literal>]
let pipe'name = $"jp.dsi.logger.15632680-eedf-418a-aa2a-334dbb121d38"

[<EntryPoint>]
let main args =
  //use pipe'srv = new NamedPipeServerStream(pipe'name, PipeDirection.InOut)
  use pipe'srv = Pipes.create'server()
  try
    pipe'srv.WaitForConnection()

    let mutable is'break = false
    while not is'break do
      //let cmd = IO.read pipe'srv
      let cmd = Pipes.read pipe'srv
      match cmd with
      // wait to recive message.
      | "" -> task { do! Task.Delay(1) } |> Task.WaitAll
      | _ ->
        // exit pwsh.
        if cmd.StartsWith "/exit" then 
          is'break <- true
          pipe'srv.Close()
        // TODO: execute command.
        else
          printfn "%s" cmd

    printfn "exit"
  finally
    pipe'srv.Close()
  0
