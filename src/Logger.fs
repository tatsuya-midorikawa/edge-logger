module Logger
open System.IO

let now = System.DateTime.Now.ToString("yyyyMMdd_HHmmss")
let root'dir dir = FilePath (Path.Combine(Path.GetFullPath dir, now))
let err'filepath dir = FilePath (Path.Combine(Path.GetFullPath dir, now, "err.log"))
let winsrv'filepath dir = FilePath (Path.Combine(Path.GetFullPath dir, now, "winsrv.log"))
let edge'filepath dir = FilePath (Path.Combine(Path.GetFullPath dir, now, "edge.log"))
let edge'installer'filepath dir = FilePath (Path.Combine(Path.GetFullPath dir, now, "msedge_installer.log"))
let edge'update'filepath dir = FilePath (Path.Combine(Path.GetFullPath dir, now, "MicrosoftEdgeUpdate.log"))
let ie'filepath dir = FilePath (Path.Combine(Path.GetFullPath dir, now, "ie.log"))
let dsregcmd'filepath dir = FilePath (Path.Combine(Path.GetFullPath dir, now, "os", "dsregcmd.log"))
let whoami'filepath dir = FilePath (Path.Combine(Path.GetFullPath dir, now, "os", "whoami.log"))
let cmdkey'filepath dir = FilePath (Path.Combine(Path.GetFullPath dir, now, "os", "cmdkey.log"))
let hotfix'filepath dir = FilePath (Path.Combine(Path.GetFullPath dir, now, "os", "hotfix.log"))
let schtasks'filepath dir = FilePath (Path.Combine(Path.GetFullPath dir, now, "schtasks.log"))

let inline exists (FilePath path) = path |> (Path.GetFullPath >> File.Exists)
let inline exists' (path: string) = path |> (Path.GetFullPath >> File.Exists)

let inline output (FilePath path) (msg: string) =
  let path = Path.GetFullPath path
  let dir = Path.GetDirectoryName path
  if not (Directory.Exists dir) then
    Directory.CreateDirectory(dir) |> ignore
    
  if not (exists' path) then 
    File.Create(path).Dispose()

  task {
    do! File.AppendAllLinesAsync(path, seq { msg })
  }

let inline copy (FilePath dst) (FilePath src) =
  let dst = dst |> (Path.GetFullPath >> Path.GetDirectoryName)
  let src = src |> (Path.GetFullPath >> Path.GetDirectoryName)
  let dir = Path.GetDirectoryName dst
  if not (Directory.Exists dir) then
    Directory.CreateDirectory(dir) |> ignore

  if File.Exists src
  then File.Copy(src, dst)
  else File.Create(dst).Dispose()

let inline log dir msg = 
  let path = err'filepath dir
  output path msg