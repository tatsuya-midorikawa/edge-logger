[<AutoOpen>]
module Common

// Common types
type FilePath = FilePath of string
type root = HKLM = 0 | HKCU = 1
type dict<'T, 'U> = System.Collections.Generic.Dictionary<'T, 'U>
type pair = System.Collections.Generic.KeyValuePair<string, obj> 
type regs = seq<root * string>
type notsupportedexn = System.NotSupportedException
type proc = {
  value : System.Diagnostics.Process
  stdout : System.Text.StringBuilder
}
with
  static member empty = { value = null; stdout = null }
  static member start (pinf: System.Diagnostics.ProcessStartInfo) =
    let p = { value = System.Diagnostics.Process.Start pinf; stdout = System.Text.StringBuilder() }
    p.value.OutputDataReceived.Add (fun e -> if e.Data <> null then p.stdout.AppendLine(e.Data) |> ignore)
    p.value.BeginOutputReadLine()
    p
  member __.exec (cmd: string) =
    if __.value <> null then
      __.value.StandardInput.WriteLine cmd
  member __.close() =
    if __.value <> null 
    then
      __.value.StandardInput.WriteLine "exit"
      __.value.WaitForExit()    
      __.value.Dispose()
      __.stdout.ToString()
    else
      ""
// Common values
let defaultof<'T> = Unchecked.defaultof<'T>
let http = new System.Net.Http.HttpClient()
let empty'task = System.Threading.Tasks.Task.Run<unit>(fun () -> ())
let notimplexn msg = System.NotImplementedException msg
let current'dir = System.IO.Path.GetFullPath "./"
let is'admin =
  use identity = System.Security.Principal.WindowsIdentity.GetCurrent()
  let principal = System.Security.Principal.WindowsPrincipal(identity);
  principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);

// Common functions
let readkey() = System.Console.ReadKey()
let rec wait'for'input () =
  printfn "Entry (y) to exit."
  match readkey().Key with System.ConsoleKey.Y -> () | _ -> wait'for'input()

// System
let inline clear () = System.Console.Clear()

// System.Threading.Tasks
let inline wait<'T> (task: System.Threading.Tasks.Task<'T>) = System.Threading.Tasks.Task.WaitAll (task)

// System.Text.Json
let inline toJson<'T> (object: 'T) = System.Text.Json.JsonSerializer.Serialize(object, System.Text.Json.JsonSerializerOptions(WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, IgnoreNullValues = true))

// System.IO
let get'fullpath = System.IO.Path.GetFullPath
let inline get'parentdir (path: string) = System.IO.Path.GetDirectoryName path
let file'exists = get'fullpath >> System.IO.File.Exists
let not'file'exists = file'exists >> not
let dir'exists = get'fullpath >> System.IO.Directory.Exists
let not'dir'exists = dir'exists >> not
let inline create'file path = System.IO.File.Create(get'fullpath path).Dispose()
let inline create'dir path = if not'dir'exists path then System.IO.Directory.CreateDirectory(get'fullpath path) |> ignore

let inline get'dir (FilePath file) = System.IO.Path.GetDirectoryName file
[<System.Obsolete("Should use combine' function. This function will be replaced by combine' function in the future.")>]
let combine = System.IO.Path.Combine >> FilePath
let combine' = System.IO.Path.Combine

// System.Diagnostics
let inline run'as (app: string) (cmds: string[]) =
  let cmds = cmds |> String.concat " "

  let pi = System.Diagnostics.ProcessStartInfo (app,
    Arguments = cmds,
    UseShellExecute = true,
    //// hide console window
    //CreateNoWindow = true,
    // run as adminstrator
    Verb = "runas")

  System.Diagnostics.Process.Start pi

let inline relaunch'as'admin'if'user (cmds: string[]) =
  try
    printfn $"000"
    if not is'admin then
      printfn $"aaa"
      let mutable asm = System.Reflection.Assembly.GetEntryAssembly().Location
      asm <- if System.String.IsNullOrWhiteSpace asm then combine' [| System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase; "edge-logger.exe" |] else asm
      let app = (asm, ".exe") |> System.IO.Path.ChangeExtension
      Ok (run'as app cmds |> ignore)
    else
      Ok ()
  with
  | e ->
    printfn $"{e.Message}"
    Error e.Message