[<AutoOpen>]
module Common

// Common types
type FilePath = FilePath of string
type root = HKLM = 0 | HKCU = 1
type dict<'T, 'U> = System.Collections.Generic.Dictionary<'T, 'U>
type pair = System.Collections.Generic.KeyValuePair<string, obj> 
type regs = seq<root * string>

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

// System
let inline clear () = System.Console.Clear()

// System.Threading.Tasks
let inline wait<'T> (task: System.Threading.Tasks.Task<'T>) = System.Threading.Tasks.Task.WaitAll (task)

// System.Text.Json
let inline toJson<'T> (object: 'T) = System.Text.Json.JsonSerializer.Serialize(object, System.Text.Json.JsonSerializerOptions(WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, IgnoreNullValues = true))

// System.IO
let inline exists path = System.IO.File.Exists path
let inline get'fpath path = System.IO.Path.GetFullPath path
let inline get'dir (FilePath file) = System.IO.Path.GetDirectoryName file
let inline create'dir dir = if not (System.IO.Directory.Exists dir) then System.IO.Directory.CreateDirectory(dir) |> ignore
let combine = System.IO.Path.Combine >> FilePath

// System.Diagnostics
let inline run'as (cmds: string[]) =
  let asm = System.Reflection.Assembly.GetEntryAssembly()
  let app = (asm.Location, ".exe") |> System.IO.Path.ChangeExtension
  let cmds = cmds |> String.concat " "

  // TODO:
  let pi = System.Diagnostics.ProcessStartInfo (app,
    Arguments = cmds,
    UseShellExecute = true,
    // hide console window
    CreateNoWindow = true,
    // run as adminstrator
    Verb = "runas")

  use p = System.Diagnostics.Process.Start pi
  p.WaitForExit()
  p.Close()
