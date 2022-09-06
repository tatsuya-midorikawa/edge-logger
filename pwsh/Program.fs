open jp.dsi.logger.misc
open System.IO.Pipes
open System.Diagnostics
open System.Threading.Tasks
open System.Collections.Concurrent
open System.Text

let private queue = ConcurrentQueue<string>()

let private psi = ProcessStartInfo ("powershell", 
   // enable commnads input and reading of output
   UseShellExecute = false,
   RedirectStandardInput = true,
   RedirectStandardOutput = true,
   // hide console window
   CreateNoWindow = true)

let pwsh = Process.Start psi
let stdout = StringBuilder()
pwsh.OutputDataReceived.Add (fun e -> if e.Data <> null then stdout.AppendLine(e.Data) |> ignore)

//[<EntryPoint>]
//let main args =
//  if args.Length <> 1 then raise (exn "Specify ID as an arugment.")

//  let id = args[0]
//  use pipe'srv = Pipes.create'pwsh'server id
//  try
//    pipe'srv.WaitForConnection()

//    let mutable is'break = false
//    while not is'break do
//      let cmd = Pipes.read pipe'srv
//      match cmd with
//      // wait to recive message.
//      | "" -> task { do! Task.Delay(1) } |> Task.WaitAll
//      | _ ->
//        // exit pwsh.
//        if cmd.StartsWith "/exit" then 
//          is'break <- true
//        // TODO: execute command.
//        else
//          queue.Enqueue cmd
//          printfn "%s" cmd
    
//    // return result value
//    Pipes.write (stdout.ToString()) pipe'srv
//  finally
//    pipe'srv.Close()
//    pwsh.Close()
//    pwsh.Dispose()
//  0

[<EntryPoint>]
let main args =
  //printfn $"length: {args.Length}"
  //for arg in args do
  //  printfn $"  {arg}"
  if args.Length <> 1 then raise (exn "Specify ID as an arugment.")

  let id = args[0]
  use pipe'srv = Pipes.create'pwsh'server id
  try
    pipe'srv.WaitForConnection()

    let mutable is'break = false
    while not is'break do
      let cmd = Pipes.read pipe'srv
      match cmd with
      // wait to recive message.
      | "" -> task { do! Task.Delay(1) } |> Task.WaitAll
      | _ ->
        // exit pwsh.
        if cmd.StartsWith "/exit" then 
          is'break <- true
        // TODO: execute command.
        else
          queue.Enqueue cmd
          printfn "%s" cmd
    
    // return result value
    Pipes.write (stdout.ToString()) pipe'srv
  finally
    pipe'srv.Close()
    pwsh.Close()
    pwsh.Dispose()
  0
