namespace jp.dsi.logger.tools

open System.IO

module IEDigest =
  [<Literal>]
  let private endpoint = "https://aka.ms/iedigest"
  let private iedigest'zip = [| current'dir; "iedigest.zip" |] |> combine'
  let private iedigest'exe = [| current'dir; "iedigest.exe" |] |> combine'

  let private downloads'if'it'doesnt'exist () =
    let dst = iedigest'zip
    let file = iedigest'exe
    if file'exists file 
    then file
    else 
      try [| dst; file; |] |> Array.iter (Tools.remove >> ignore) with _ -> ()
      task {
        let! res = http.GetStreamAsync endpoint
        use fs = new FileStream(dst, FileMode.CreateNew)
        do! res.CopyToAsync fs
      } |> wait
      dst |> (Tools.unzip >> Pwsh.exec >> ignore)
      file

  let output root'dir = 
    let iedigest'exe = downloads'if'it'doesnt'exist ()
    if file'exists iedigest'exe
    then
      let root'dir' = Logger.get'root'dir root'dir
      let desktop'dir = combine' [| System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop); "IEDigest" |]
      let dst = [| root'dir'; "ie" |] |> combine'
      Logger.create'output'dir dst
      let r = 
        [| $"{iedigest'exe} /accepteula /report {dst}" |]
        |> Pwsh.exec
      if root'dir <> desktop'dir then Tools.remove desktop'dir |> ignore
      r
    else
      raise (exn "iedigest.exe does not exist.")

  let clean () = if file'exists iedigest'exe then iedigest'exe |> (Tools.remove >> ignore) else ()
    