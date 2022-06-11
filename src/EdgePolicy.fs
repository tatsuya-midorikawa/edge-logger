module EdgePolicy

  open Microsoft.Win32
  open FSharp.Core.CompilerServices
  
  type Metadata = { os: string; version: string; }
  type Policy = { path: string; name: string; vtype: string; value: obj; values: Policy[]}
  type Policies = { Metadata: Metadata; EdgePolicies: Policy[]; EdgeUpdate: Policy[]; WebView2: Policy[] }

  // ポリシーを再帰的に読み取る.
  let rec dig(key: RegistryKey) =
    if key = null
    then [||]
    else
      let values =
        key.GetValueNames()
        |> Array.map (fun name -> { path = key.Name; name = name; vtype = key.GetValueKind(name) |> string; value = Registry.GetValue(key.Name, name, ""); values = null })
      let children = 
        key.GetSubKeyNames()
        |> Array.map (fun name -> 
          use key = key.OpenSubKey(name)
          let xs = dig (key)
          { path = null; name = name; vtype = null; value = null; values = xs} )
      Array.append values children

  [<Literal>]
  let current'version = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion"
  [<Literal>]
  let edition'version = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\EditionVersion"

  // OS のバージョン情報を取得する.
  let inline fetchWindowsVersion () =
    use hklm = Registry.LocalMachine.OpenSubKey(current'version, false)
    let name = hklm.GetValue("ProductName") |> string
    let disp = hklm.GetValue("DisplayVersion") |> string
    use hklm = Registry.LocalMachine.OpenSubKey(edition'version, false)
    let build = hklm.GetValue("EditionBuildNumber") |> string
    let qfe = hklm.GetValue("EditionBuildQfe") |> string
    $"{name} {disp} (build {build}.{qfe})"

  // Edge のバージョン情報を取得する.
  let inline fetchEdgeVersion () = 
    let createVersionInfo = System.IO.Path.Combine >> System.Diagnostics.FileVersionInfo.GetVersionInfo
    try
       [| 
         System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86)
         @"Microsoft\Edge\Application\msedge.exe" 
       |]
       |> createVersionInfo
       |> (fun vi -> vi.ProductVersion)
     with _ -> "not found"

  [<Literal>]
  let edge = @"SOFTWARE\Policies\Microsoft\Edge"
  [<Literal>]
  let edge'update = @"SOFTWARE\Policies\Microsoft\EdgeUpdate"
  [<Literal>]
  let edge'webview2 = @"SOFTWARE\Policies\Microsoft\Edge\WebView2"

  // Edge ポリシー情報を取得する.
  let inline fetch () =

    let load reg =
      let mutable c = ArrayCollector<Policy>()
      use hklm = Registry.LocalMachine.OpenSubKey(reg, false)
      let xs = dig hklm
      c.AddMany xs
      use hkcu = Registry.CurrentUser.OpenSubKey(reg, false)
      let xs = dig(hkcu)
      c.AddMany xs
      c.Close()
      
    let os = fetchWindowsVersion()
    let version = fetchEdgeVersion()
    // SOFTWARE\Policies\Microsoft\Edge
    let edge' = load edge
    // SOFTWARE\Policies\Microsoft\EdgeUpdate
    let update = load edge'update
    // SOFTWARE\Policies\Microsoft\Edge\WebView2
    let webview2 = load edge'webview2

    { 
      Metadata = { os = os; version = version }; 
      EdgePolicies = edge'; 
      EdgeUpdate = update; 
      WebView2 = webview2; 
    }