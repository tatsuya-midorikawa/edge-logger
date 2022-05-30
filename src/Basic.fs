module Basic

type Basic = { edge_version: string }

let private createVersionInfo = System.IO.Path.Combine >> System.Diagnostics.FileVersionInfo.GetVersionInfo
let private edge'version () = 
  try
     [| 
       System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86)
       @"Microsoft\Edge\Application\msedge.exe" 
     |]
     |> createVersionInfo
     |> (fun vi -> vi.ProductVersion)
   with _ -> "not found"

let getInfo () =
  { edge_version = edge'version () }