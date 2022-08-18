module IEDigest

open System.Net.Http
open System.IO

let http = new HttpClient()

let download dir =
  let ep = "https://aka.ms/iedigest"
  task {
    let! res = http.GetStreamAsync ep
    use fs = new FileStream(Path.Combine(dir, "iedigest.zip"), FileMode.CreateNew)
    do! res.CopyToAsync fs
  }
  |> wait
