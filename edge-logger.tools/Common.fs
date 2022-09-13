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
let get'fullpath = System.IO.Path.GetFullPath
let inline get'parentdir (path: string) = System.IO.Path.GetDirectoryName path
let file'exists = get'fullpath >> System.IO.File.Exists
let not'file'exists = file'exists >> not
let dir'exists = get'fullpath >> System.IO.Directory.Exists
let not'dir'exists = dir'exists >> not
let inline create'file path = System.IO.File.Create(get'fullpath path).Dispose()
let inline create'dir path = if not'dir'exists path then System.IO.Directory.CreateDirectory(get'fullpath path) |> ignore

let inline get'dir (FilePath file) = System.IO.Path.GetDirectoryName file
let combine = System.IO.Path.Combine >> FilePath

// System.Diagnostics
let inline run'as (app: string) (cmds: string[]) =
  let cmds = cmds |> String.concat " "

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

let inline relaunch'as'admin'if'user (cmds: string[]) =
  try
    if not is'admin then
      let asm = System.Reflection.Assembly.GetEntryAssembly()
      let app = (asm.Location, ".exe") |> System.IO.Path.ChangeExtension
      Ok (run'as app cmds)
    else
      Ok ()
  with
  | e -> Error e.Message