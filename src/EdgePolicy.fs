module EdgePolicy

  open System.Text.Json.Serialization
  open Microsoft.Win32
  open FSharp.Core.CompilerServices


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
  
  type Policy = {
    [<JsonPropertyOrder(1)>] name: string
    [<JsonPropertyOrder(2)>] level: string;
    [<JsonPropertyOrder(3)>] scope: string;
    [<JsonPropertyOrder(4)>] value: obj;
  }
  type Policies = {
    [<JsonPropertyOrder(1)>] Metadata : {| os: string; version: string |}
    [<JsonPropertyOrder(2)>] EdgePolicies : Policy[]
    [<JsonPropertyOrder(3)>] EdgeUpdate : Policy[]
    [<JsonPropertyOrder(4)>] WebView2 : Policy[]
  }

  // ポリシーを再帰的に読み取る.
  let rec dig(key: RegistryKey) =
    if key = null
    then 
      [||]
    else
      let values =
        key.GetValueNames()
        |> Array.map (fun name -> { 
          name = name
          level = if key.Name.EndsWith("Recommended") then "recommended" else "mandatory"
          scope = if key.Name.StartsWith("HKEY_LOCAL_MACHINE") then "machine" else "user"
          value = Registry.GetValue(key.Name, name, "") } )

      let children = 
        key.GetSubKeyNames()
        |> Array.map (fun name -> 
          use key = key.OpenSubKey(name)
          let xs = dig (key)
          { name = name
            level = if key.Name.EndsWith("Recommended") then "recommended" else "mandatory"
            scope = if key.Name.StartsWith("HKEY_LOCAL_MACHINE") then "machine" else "user"
            value = xs :> obj })

      Array.append values children

  // Edge ポリシー情報を取得する.
  let inline fetch () =

    let load reg =
      let mutable c = ArrayCollector<Policy>()
      //let mutable c = ArrayCollector<Policy>()
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
      Metadata = {| os = os; version = version |}; 
      EdgePolicies = edge'; 
      EdgeUpdate = update; 
      WebView2 = webview2; 
    }