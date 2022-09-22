[<AutoOpen>]
module Common
//// Common types
//type FilePath = FilePath of string
//type root = HKLM = 0 | HKCU = 1
//type dict<'T, 'U> = System.Collections.Generic.Dictionary<'T, 'U>
//type pair = System.Collections.Generic.KeyValuePair<string, obj> 
//type regs = seq<root * string>
let inline msgbox'show (msg: string) = System.Windows.Forms.MessageBox.Show (msg, "edge-logger.exe") |> ignore

//// Common values
//let defaultof<'T> = Unchecked.defaultof<'T>
//let http = new System.Net.Http.HttpClient()
//let empty'task = System.Threading.Tasks.Task.Run<unit>(fun () -> ())
//let notimplexn msg = System.NotImplementedException msg
//let current'dir = System.IO.Path.GetFullPath "./"

//// System
//let inline clear () = System.Console.Clear()

//// System.Threading.Tasks
//let inline wait<'T> (task: System.Threading.Tasks.Task<'T>) = System.Threading.Tasks.Task.WaitAll (task)

//// System.Text.Json
//let inline toJson<'T> (object: 'T) = System.Text.Json.JsonSerializer.Serialize(object, System.Text.Json.JsonSerializerOptions(WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, IgnoreNullValues = true))

//// System.IO
//let inline exists path = System.IO.File.Exists path
//let inline get'fpath path = System.IO.Path.GetFullPath path
//let inline get'dir (FilePath file) = System.IO.Path.GetDirectoryName file
//let inline create'dir dir = if not (System.IO.Directory.Exists dir) then System.IO.Directory.CreateDirectory(dir) |> ignore
//let combine = System.IO.Path.Combine >> FilePath