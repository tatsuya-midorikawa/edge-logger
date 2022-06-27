module Logger
open System.IO

type FilePath = FilePath of string

let now = System.DateTime.Now.ToString("yyyyMMdd_HHmmss")
let winsrv'filepath dir = FilePath (Path.Combine(dir, now, "winsrv.log"))
let edge'filepath dir = FilePath (Path.Combine(dir, now, "edge.log"))
let ie'filepath dir = FilePath (Path.Combine(dir, now, "ie.log"))
let dsregcmd'filepath dir = FilePath (Path.Combine(dir, now, "os", "dsregcmd.log"))
let whoami'filepath dir = FilePath (Path.Combine(dir, now, "os", "whoami.log"))
let cmdkey'filepath dir = FilePath (Path.Combine(dir, now, "os", "cmdkey.log"))
let hotfix'filepath dir = FilePath (Path.Combine(dir, now, "os", "hotfix.log"))

let inline exists (FilePath path) =
  File.Exists path

let inline exists' (path: string) =
  File.Exists path

let inline output (FilePath path) (msg: string) =
  let dir = Path.GetDirectoryName path
  if not (Directory.Exists dir) then
    Directory.CreateDirectory(dir) |> ignore
    
  if not (exists' path) then 
    use _ = System.IO.File.Create path
    ()

  task {
    do! System.IO.File.AppendAllLinesAsync(path, seq { msg })
  }
