module IEDigest

open System.IO

let private combine = System.IO.Path.Combine
let private iedigest'zip = [| current'dir; "iedigest.zip" |] |> combine
let private iedigest'exe = [| current'dir; "iedigest.exe" |] |> combine

let downloads'if'it'doesnt'exist () =
  let dir = current'dir
  let ep = "https://aka.ms/iedigest"
  let dst = iedigest'zip
  let file = iedigest'exe
  if exists file 
  then
    file
  else
    try
      [| Pwsh.remove dst; Pwsh.remove file |]
      |> (Array.concat >> Pwsh.exec >> ignore)
    with
      _ -> ()

    task {
      let! res = http.GetStreamAsync ep
      use fs = new FileStream(dst, FileMode.CreateNew)
      do! res.CopyToAsync fs
    }
    |> wait
    dst |> (Pwsh.unzip >> Pwsh.exec >> ignore)
    file

let output dir =
  if exists iedigest'exe
  then
    let dst = [| Logger.root'dir'path dir; "ie" |] |> combine
    dst |> create'dir
    [| $"{iedigest'exe} /accepteula /report {dst}" |]
    |> (Cmd.exec >> ignore)
  else
    raise (exn "iedigest.exe does not exist.")

let clean () = if exists iedigest'exe then iedigest'exe |> (Pwsh.remove >> Pwsh.exec >> ignore) else ()
    