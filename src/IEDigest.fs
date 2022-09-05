module IEDigest

open System.Net.Http
open System.IO

let http = new HttpClient()
let inline exists path = File.Exists path

let inline downloads'if'it'doesnt'exist dir =
  let dir = Path.GetFullPath dir
  let ep = "https://aka.ms/iedigest"
  let dst = Path.Combine(dir, "iedigest.zip")
  let file = Path.Combine(dir, "iedigest.exe")
  if exists file 
  then
    printfn "iedigest.exe exists"
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